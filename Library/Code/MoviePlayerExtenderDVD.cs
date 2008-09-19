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
        MediaSource _source;
        DVDDiskInfo _info;

        public ExtenderDVDPlayer(MediaSource source)
        {
            _source = source;
        }

        public bool PlayMovie()
        {
            _info = _source.Disk.DVDDiskInfo;

            string videoFile = null;
            if (_info != null)
            {
                OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: DVD Disk info: '{0}'", _info);
                DVDTitle dvdTitle = _info.GetMainTitle();
                if (dvdTitle != null)
                {
                    string videoTSDir = _source.MediaPath;
                    if (string.Compare(new DirectoryInfo(videoTSDir).Name, "VIDEO_TS", true) != 0)
                        videoTSDir = Path.Combine(videoTSDir, "VIDEO_TS");
                    int fileID = int.Parse(dvdTitle.File.Substring(4));
                    OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: main title fileID={1} found for '{0}'", _source, fileID);
                    string vts = string.Format("VTS_{0:D2}_", fileID);
                    List<string> vobs = new List<string>(Directory.GetFiles(videoTSDir, vts + "*.VOB"));
                    vobs.Remove(Path.Combine(videoTSDir, vts + "0.VOB"));

                    if (vobs.Count < 1)
                    {
                        OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: no VOB files found for '{0}'", _source);
                        return false;
                    }

                    string mpegFolder = Path.Combine(videoTSDir, "Extender-MyMovies");
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

                        Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread(delegate
                        {
                            FileStream writer = null;
                            try
                            {
                                writer = File.Open(vobs[0], FileMode.Append, FileAccess.Write, FileShare.Read);
                                for (int i = 1; i < vobs.Count; ++i)
                                    MergeFile(writer, vobs[0], vobs[i]);

                                for (int i = 1; i < vobs.Count; ++i)
                                {
                                    OMLApplication.DebugLine("Deleting '{0}'", vobs[i]);
                                    File.Delete(vobs[i]);
                                }
                            }
                            finally
                            {
                                if (writer != null)
                                {
                                    OMLApplication.DebugLine("Done merging into '{0}'", vobs[0]);
                                    writer.Close();
                                }
                            }
                        }, delegate
                        {
                            Utilities.DebugLine("Merging done, restart play, and reset posision");
                            if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                            {
                                TimeSpan cur = AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.Position;
                                Utilities.DebugLine("Current position: {0}", cur);
                                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, videoFile, false))
                                {
                                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.Position = cur;
                                    Utilities.DebugLine("Done starting over, resetting position: {0}", cur);
                                }
                            }
                        }, null);
                    }
                }
                else
                    OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: no main title found for {0}", _source);
            }
            else
                OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: no DVD-DiskInfo found for {0}", _source);
            
            if (videoFile == null)
                videoFile = GetAsxFile();

            if (videoFile != null && AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, videoFile, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    Utilities.DebugLine("ExtenderDVDPlayer.PlayMovie: movie '{0}', Playing file '{1}'", _source.Name, videoFile);
                    OMLApplication.Current.NowPlayingMovieName = _source.Name;
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

        static void MergeFile(FileStream writter, string vob0, string vobx)
        {
            byte[] buffer = new byte[128 * 1024 * 1024];
            OMLApplication.DebugLine("Appending '{0}' to '{1}'", vobx, vob0);
            using (FileStream reader = File.OpenRead(vobx))
                for (; ; )
                {
                    int read = reader.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                        break;
                    writter.Write(buffer, 0, read);
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
                "containing multiple .VOB files", _source.Name, vts);
            using (StreamWriter writer = File.CreateText(videoFile))
            {
                writer.WriteLine("<asx version=\"3.0\">");
                writer.WriteLine("\t<title>" + HttpUtility.HtmlEncode(_source.Name) + "</title>");
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
            foreach (string fileName in Directory.GetFiles(Path.Combine(_source.MediaPath, ".."), "*.asx"))
            {
                Utilities.DebugLine("ExtenderDVDPlayer.GetAsxFile: found asx {0}", fileName);
                return fileName;
            }
            Utilities.DebugLine("ExtenderDVDPlayer.GetAsxFile: no asx file found {0}", _source.MediaPath);
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

    }

}