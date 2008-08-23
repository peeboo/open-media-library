using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;

using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;

using OMLEngine;
using OMLGetDVDInfo;

namespace Library
{
    /// <summary>
    /// DVDPlayer class for playing a DVD
    /// </summary>
    public class ExtenderDVDPlayer : IPlayMovie
    {
        public ExtenderDVDPlayer(MovieItem title)
        {
            _title = title;
        }

        public bool PlayMovie()
        {
            _info = _title.SelectedDisk.DVDDiskInfo;

            string videoFile = null;
            if (_info != null)
            {
                OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: DVD Disk info: '{0}'", _info);
                DVDTitle dvdTitle = _info.GetMainTitle();
                if (dvdTitle != null)
                {
                    int fileID = int.Parse(dvdTitle.File.Substring(4));
                    OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: main title fileID={1} found for '{0}'", _title.SelectedDisk, fileID);
                    string vts = string.Format("VTS_{0:D2}_", fileID);
                    List<string> vobs = new List<string>(Directory.GetFiles(_title.SelectedDisk.Path, vts + "*.VOB"));
                    vobs.Remove(Path.Combine(_title.SelectedDisk.Path, vts + "0.VOB"));

                    if (vobs.Count < 1)
                        return false;

                    string mpegFolder = Path.Combine(_title.SelectedDisk.Path, "Extender-MyMovies");
                    if (Directory.Exists(mpegFolder) == false)
                    {
                        OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: creating folder '{0}'", mpegFolder);
                        Directory.CreateDirectory(mpegFolder);
                    }

                    if (vobs.Count == 1)
                    {
                        OMLApplication.DebugLine("Single VOB file, trying to use a hard-link approach and direct playback");
                        string mpegFile = MakeMPEGLink(mpegFolder, vobs[0]);
                        if (File.Exists(mpegFile))
                        {
                            OMLApplication.DebugLine("directly use MPEG");
                            videoFile = mpegFile;
                        }
                    }
                    else if (Properties.Settings.Default.Extender_UseAsx)
                    {
                        OMLApplication.DebugLine("Multiple VOB files, and Extender_UseAsx == true, trying to use a hard-link and asx playlist approach ");
                        foreach (string vob in vobs)
                            MakeMPEGLink(mpegFolder, vob);

                        if (File.Exists(GetMPEGName(mpegFolder, vobs[0])))
                            videoFile = CreateASX(mpegFolder, vts, vobs);
                    }
                    else if (Properties.Settings.Default.Extender_MergeVOB)
                    {
                        OMLApplication.DebugLine("Multiple VOB files, and Extender_MergeVOB == true, trying to use a hard-link and auto-merge into single large .VOB file approach ");
                        string mpegFile = MakeMPEGLink(mpegFolder, vobs[0]);
                        OMLApplication.DebugLine("Merge VOB's, use first VOB to play and merge into");

                        if (File.Exists(mpegFile) == false)
                            return false;

                        videoFile = mpegFile;

                        FileStream writter = File.Open(vobs[0], FileMode.Append, FileAccess.Write, FileShare.Read);
                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            try
                            {
                                for (int i = 1; i < vobs.Count; ++i)
                                {
                                    byte[] buffer = new byte[128 * 1024 * 1024];
                                    OMLApplication.DebugLine("Appending '{0}' to '{1}'", vobs[i], vobs[0]);
                                    using (FileStream reader = File.OpenRead(vobs[i]))
                                        for (; ; )
                                        {
                                            int read = reader.Read(buffer, 0, buffer.Length);
                                            if (read == 0)
                                                break;
                                            writter.Write(buffer, 0, read);
                                        }
                                }

                                for (int i = 1; i < vobs.Count; ++i)
                                {
                                    OMLApplication.DebugLine("Deleting '{0}'", vobs[i]);
                                    File.Delete(vobs[i]);
                                }
                            }
                            finally
                            {
                                OMLApplication.DebugLine("Done merging into '{0}'", vobs[0]);
                                writter.Close();
                            }
                        });
                    }
                }
                else
                    OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: no main title found for {0}", _title.SelectedDisk);
            }
            else
                OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: no DVD-DiskInfo found for {0}", _title.SelectedDisk);
            
            if (videoFile == null)
                videoFile = GetAsxFile();

            if (videoFile != null && AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, videoFile, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    Utilities.DebugLine("ExtenderDVDPlayer.PlayMovie: movie '{0}', Playing file '{1}'", _title.Name, videoFile);
                    OMLApplication.Current.NowPlayingMovieName = _title.Name;
                    OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string MakeMPEGLink(string mpegFolder, string vob)
        {
            string mpegFile = GetMPEGName(mpegFolder, vob);
            if (File.Exists(mpegFile) == false)
            {
                if (IsNTFS(mpegFolder) == false)
                {
                    OMLApplication.DebugLine("File system not NTFS: can't create hard-links: {0}", mpegFolder);
                    //break;
                }
                if (CreateHardLinkAPI(mpegFile, vob, IntPtr.Zero) == false)
                {
                    OMLApplication.DebugLine("Failed to create a hard-link {0} -> {1}", vob, mpegFile);
                    //break;
                }
                OMLApplication.DebugLine("created a hard-link {0} -> {1}, Success:{2}", vob, mpegFile, File.Exists(mpegFile));
            }
            return mpegFile;
        }

        string CreateASX(string mpegFolder, string vts, IEnumerable<string> vobs)
        {
            string videoFile = Path.Combine(mpegFolder, vts.Trim('_')) + ".asx";
            if (File.Exists(videoFile))
                return videoFile;

            Utilities.DebugLine("ExtenderDVDPlayer.PlayMovie: creating .asx playlist for extender for '{0}'/'{1}' " +
                "containing multiple .VOB files", _title.Name, vts);
            using (StreamWriter writer = File.CreateText(videoFile))
            {
                writer.WriteLine("<asx version=\"3.0\">");
                writer.WriteLine("\t<title>" + HttpUtility.HtmlEncode(_title.Name) + "</title>");
                writer.WriteLine("\t<param name=\"AllowShuffle\" value=\"no\" />");
                writer.WriteLine("\t<param name=\"CanPause\" value=\"yes\" />");
                writer.WriteLine("\t<param name=\"CanSeek\" value=\"yes\" />");
                writer.WriteLine("\t<param name=\"CanSkipBack\" value=\"yes\" />");
                writer.WriteLine("\t<param name=\"CanSkipForward\" value=\"yes\" />");
                writer.WriteLine("\t<param name=\"PreBuffer\" value=\"true\" />");
                writer.WriteLine("\t<param name=\"ShowWhileBuffering\" value=\"true\" />");

                foreach (string vob in vobs)
                    writer.WriteLine("\t<entry><ref href=\"" + 
                        HttpUtility.HtmlAttributeEncode(HttpUtility.UrlPathEncode(Path.GetFileNameWithoutExtension(vob))) + 
                        ".MPG" + "\"/></entry>");

                writer.WriteLine("</asx>");
            }
            return videoFile;
        }

        static string GetMPEGName(string mpegFolder, string vob)
        {
            return Path.Combine(mpegFolder, Path.GetFileNameWithoutExtension(vob)) + ".MPG";
        }

        string GetAsxFile()
        {
            foreach (string fileName in Directory.GetFiles(Path.Combine(_title.SelectedDisk.Path, ".."), "*.asx"))
            {
                Utilities.DebugLine("ExtenderDVDPlayer.GetAsxFile: found asx {0}", fileName);
                return fileName;
            }
            Utilities.DebugLine("ExtenderDVDPlayer.GetAsxFile: no asx file found {0}", _title.SelectedDisk.Path);
            return null;
        }

        #region -- NTFS helper functions to create hard-links --
        [DllImport("Kernel32.Dll", CharSet = CharSet.Unicode, EntryPoint = "CreateHardLink")]
        static extern bool CreateHardLinkAPI(
            [MarshalAs(UnmanagedType.LPTStr)] string lpFileName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpExistingFileName,
            IntPtr /* LPSECURITY_ATTRIBUTES */ lpSecurityAttributes
        );

        [DllImport("Kernel32.Dll", CharSet = CharSet.Unicode)]
        static extern bool GetVolumeInformation(
            string lpRootPathName,
            IntPtr /*StringBulider*/ lpVolumeNameBuffer,
            int nVolumeNameSize,
            /*ref*/ IntPtr lpVolumeSerialNumber,
            /*ref*/ IntPtr lpMaximumComponentLength,
            /*ref*/ IntPtr lpFileSystemFlags,
            StringBuilder lpFileSystemNameBuffer,
            int nFileSystemNameSize
        );

        static bool IsNTFS(string rootPath)
        {
            StringBuilder sb = new StringBuilder(255);
            GetVolumeInformation(Directory.GetDirectoryRoot(rootPath), IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, sb, 255);
            return sb.ToString().CompareTo("NTFS") == 0;
        }
        #endregion

        MovieItem _title;
        DVDDiskInfo _info;
    }

}