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
using System.ComponentModel;

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

        public static bool CanPlay(MediaSource source)
        {
            if (FindMPeg(source) != null)
                return true;

            // TODO: allow to play auto-merging VOBs, asx, and other handings below
            return false;
        }

        public static string FindMPeg(MediaSource source)
        {
            DVDTitle dvdTitle = source.DVDTitle;
            if (dvdTitle == null)
                return null;

            string videoTSDir = source.VIDEO_TS;
            int fileID = int.Parse(dvdTitle.File.Substring(4));
            string vts = string.Format("VTS_{0:D2}_", fileID);
            List<string> vobs = new List<string>(Directory.GetFiles(videoTSDir, vts + "*.VOB"));
            vobs.Remove(Path.Combine(videoTSDir, vts + "0.VOB"));

            if (vobs.Count < 1)
                return null;

            if (IsNTFS(videoTSDir))
            {
                string mpegFolder = Path.Combine(videoTSDir, "Extender-MyMovies");

                if (Directory.Exists(mpegFolder) == false)
                    Directory.CreateDirectory(mpegFolder);
                if (Directory.Exists(mpegFolder) == false)
                    return null;

                if (vobs.Count == 1)
                {
                    string mpegFile = MakeMPEGLink(mpegFolder, vobs[0]);
                    if (File.Exists(mpegFile))
                        return mpegFile;
                }
                else if (Properties.Settings.Default.Extender_UseAsx)
                {
                    foreach (string vob in vobs)
                        MakeMPEGLink(mpegFolder, vob);

                    if (File.Exists(GetMPEGName(mpegFolder, vobs[0])))
                        return CreateASX(mpegFolder, vts, source.Name, vobs);
                }
                else if (Properties.Settings.Default.Extender_MergeVOB)
                {
                    string mpegFile = MakeMPEGLink(mpegFolder, vobs[0]);
                    if (File.Exists(mpegFile))
                        return mpegFile;
                }
            }
            else if (videoTSDir.StartsWith("\\\\"))
            {
                string mpegFile = Path.ChangeExtension(source.GetTranscodingFileName(), ".MPEG");
                if (File.Exists(mpegFile))
                {
                    OMLApplication.DebugLine("Found '{0}' as pre-existing .MPEG soft-link", mpegFile);
                    return mpegFile;
                }

                string vob = Path.Combine(videoTSDir, vobs[0]);
                OMLApplication.DebugLine("Trying to create '{0}' soft-link to '{1}'", mpegFile, vob);

                bool ret = CreateSymbolicLink(mpegFile, vob, SYMLINK_FLAG_FILE);
                string retMsg = ret ? "success" : "Sym-Link failed: " + new Win32Exception(Marshal.GetLastWin32Error()).Message;

                if (File.Exists(mpegFile))
                    return mpegFile;
                OMLApplication.DebugLine("Soft-link creation failed! {0}", retMsg);
            }
            else
            {
                OMLApplication.DebugLine("Media not on a network drive nor on a NTFS compatible drive, no supported");
            }

            return null;
        }

        public bool PlayMovie()
        {
            _info = _source.DVDDiskInfo;

            string videoFile = FindMPeg(_source);
            if (videoFile == null && _info != null)
            {
                OMLApplication.DebugLine("ExtenderDVDPlayer.PlayMovie: DVD Disk info: '{0}'", _info);
                DVDTitle dvdTitle = _source.DVDTitle;
                if (dvdTitle != null)
                {
                    string videoTSDir = _source.VIDEO_TS;
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
                            videoFile = CreateASX(mpegFolder, vts, _source.Name, vobs);
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
                    if (_source.ResumeTime != null)
                    {
                        Utilities.DebugLine("ExtenderDVDPlayer.PlayMovie: set resume time to: {0}", _source.ResumeTime);
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.Position = _source.ResumeTime.Value;
                    }
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += this.Transport_PropertyChanged;
                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        void Transport_PropertyChanged(Microsoft.MediaCenter.UI.IPropertyObject sender, string property)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                MediaTransport t = (MediaTransport)sender;
                Utilities.DebugLine("ExtenderDVDPlayer.Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    switch (t.PlayState)
                    {
                        case PlayState.Finished:
                            Utilities.DebugLine("ExtenderDVDPlayer (Finished): clear resume time");
                            this._source.ClearResumeTime();
                            break;
                        case PlayState.Stopped:
                        case PlayState.Paused:
                            Utilities.DebugLine("ExtenderDVDPlayer ({0}): set resume time: {1}", t.PlayState, t.Position);
                            this._source.SetResumeTime(t.Position);
                            break;
                    }
                }
            });
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

        static string MakeMPEGLink(string mpegFolder, string vob)
        {
            string mpegFile = GetMPEGName(mpegFolder, vob);
            if (File.Exists(mpegFile))
                return mpegFile;

            bool ret = CreateSymbolicLink(mpegFile, vob, SYMLINK_FLAG_FILE);
            string retMsg = ret ? "success" : "Sym-Link failed: " + new Win32Exception(Marshal.GetLastWin32Error()).Message;
            if (File.Exists(mpegFile))
            {
                OMLApplication.DebugLine("created a sym-link {0} -> {1}, kernel32 success", vob, mpegFile);
                return mpegFile;
            }

            OMLApplication.DebugLine("created a sym-link {0} -> {1}, failed, {2}", vob, mpegFile, retMsg);

            string args = string.Format("/c mklink \"{0}\" \"{1}\"", mpegFile, vob);
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("cmd.exe", 
                args);
            p.WaitForExit();
            int exitCode = p.ExitCode;

            if (File.Exists(mpegFile))
            {
                OMLApplication.DebugLine("created a sym-link {0} -> {1}, cmd.exe success", vob, mpegFile);
                return mpegFile;
            }

            OMLApplication.DebugLine("created a sym-link {0} -> {1}, mklink failed: args:'{2}', exit code: {3}", vob, mpegFile, args, exitCode);

            ret = CreateHardLinkAPI(mpegFile, vob, IntPtr.Zero);
            retMsg = ret ? "success" : "Hard-Link failed: " + new Win32Exception(Marshal.GetLastWin32Error()).Message;
            if (File.Exists(mpegFile))
            {
                OMLApplication.DebugLine("created a hard-link {0} -> {1}, success", vob, mpegFile);
                return mpegFile;
            }

            OMLApplication.DebugLine("failed to create link {0} -> {1}, neither with sym-link, mklink nor hard-link", vob, mpegFile);
            return null;
        }

        static string CreateASX(string mpegFolder, string vts, string name, IEnumerable<string> vobs)
        {
            string videoFile = Path.Combine(mpegFolder, vts.Trim('_')) + ".asx";
            if (File.Exists(videoFile))
                return videoFile;

            Utilities.DebugLine("ExtenderDVDPlayer.PlayMovie: creating .asx playlist for extender for '{0}'/'{1}' " +
                "containing multiple .VOB files", name, vts);
            using (StreamWriter writer = File.CreateText(videoFile))
            {
                writer.WriteLine("<asx version=\"3.0\">");
                writer.WriteLine("\t<title>" + HttpUtility.HtmlEncode(name) + "</title>");
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
        [DllImport("Kernel32.Dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool CreateSymbolicLink(
            [MarshalAs(UnmanagedType.LPTStr)] string lpSymlinkFileName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpTargetFileName, 
            int dwFlags);
        const int SYMLINK_FLAG_DIRECTORY = 1;
        const int SYMLINK_FLAG_FILE = 0;

        [DllImport("Kernel32.Dll", CharSet = CharSet.Unicode, EntryPoint = "CreateHardLink", SetLastError = true)]
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

        internal static bool IsNTFS(string rootPath)
        {
            StringBuilder sb = new StringBuilder(255);
            GetVolumeInformation(Directory.GetDirectoryRoot(rootPath), IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, sb, 255);
            return sb.ToString().CompareTo("NTFS") == 0;
        }
        #endregion

        public static void Uninitialize(MediaSource ms)
        {
            if (ms.Format != VideoFormat.DVD)
                return;

            string videoTSDir = ms.VIDEO_TS;
            if (videoTSDir == null || IsNTFS(videoTSDir) == false)
                return;

            string mpegFolder = Path.Combine(videoTSDir, "Extender-MyMovies");
            if (Directory.Exists(mpegFolder))
            {
                OMLApplication.DebugLine("ExtenderDVDPlayer.Uninitialize: deleting folder '{0}'", mpegFolder);
                try { Directory.Delete(mpegFolder, true); }
                catch { }
            }
        }

        public static void Uninitialize(IEnumerable<Title> titles)
        {
            foreach (Title title in titles)
                foreach (Disk disk in title.Disks)
                    Uninitialize(new MediaSource(disk));
        }
    }

}