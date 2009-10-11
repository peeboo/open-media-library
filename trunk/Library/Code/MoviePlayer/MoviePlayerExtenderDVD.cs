using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using System.Runtime.InteropServices;
using OMLEngine;
using OMLEngine.Settings;
using OMLGetDVDInfo;
using System.ComponentModel;
using OMLTranscoder;
using OMLEngineService;

namespace Library
{
    /// <summary>
    /// DVDPlayer class for playing a DVD
    /// </summary>
    public class ExtenderDVDPlayer : IPlayMovie
    {
        TranscodingAPI transcoder;
        MediaSource _source;
        DVDDiskInfo _info;

        public ExtenderDVDPlayer(MediaSource source)
        {
            _source = source;
            OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Attempting to connect to WCF Transcoder Service");
            transcoder = new TranscodingAPI(_source, null);
        }

        public static bool CanPlay(MediaSource source)
        {
            return true;
            //if (FindMPeg(source) != null)
            //    return true;

            //// TODO: allow to play auto-merging VOBs, asx, and other handings below
            //return false;
        }

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

        internal static bool IsNTFS(string rootPath) {
            StringBuilder sb = new StringBuilder(255);
            GetVolumeInformation(Directory.GetDirectoryRoot(rootPath), IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, sb, 255);
            return sb.ToString().CompareTo("NTFS") == 0;
        }

        public static string FindMPeg(MediaSource source)
        {
            DVDTitle dvdTitle = source.DVDTitle;
            if (dvdTitle == null)
                return null;

            // ensure DVD's with DTS audio get transcoded, since extenders don't support DTS audio playback
            if (dvdTitle.AudioTracks.Count > 0 && dvdTitle.AudioTracks[0].Format == AudioEncoding.DTS)
                return null;

            string videoTSDir = source.VIDEO_TS;
            int fileID = int.Parse(dvdTitle.File.Substring(4));
            string vts = string.Format("VTS_{0:D2}_", fileID);
            List<string> vobs = new List<string>(Directory.GetFiles(videoTSDir, vts + "*.VOB"));
            vobs.Remove(Path.Combine(videoTSDir, vts + "0.VOB"));

            // don't direct play unmerged .VOB files
            if (vobs.Count < 1 || vobs.Count > 1)
                return null;

            if (IsNTFS(videoTSDir))
            {
                string mpegFolder = Path.Combine(FileSystemWalker.ExtenderCacheDirectory, Guid.NewGuid().ToString());

                if (Directory.Exists(mpegFolder) == false)
                    Directory.CreateDirectory(mpegFolder);
                if (Directory.Exists(mpegFolder) == false)
                    return null;

                OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Attempting to connect to WCF Transcoder Service");
                TranscodingAPI transcoder = new TranscodingAPI(source, null);
                OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Calling MakeMPEGLink on WCF Transcoder Service");
                string mpegFile = transcoder.MakeMPEGLink(mpegFolder, vobs[0]);
                if (File.Exists(mpegFile))
                    return mpegFile;
            }
            else if (videoTSDir.StartsWith("\\\\"))
            {
                string mpegFile = Path.ChangeExtension(source.GetTranscodingFileName(), ".MPEG");
                if (File.Exists(mpegFile))
                {
                    OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Found '{0}' as pre-existing .MPEG soft-link", mpegFile);
                    return mpegFile;
                }

                string vob = Path.Combine(videoTSDir, vobs[0]);
                OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Trying to create '{0}' soft-link to '{1}'", mpegFile, vob);

                OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Attempting to connect to WCF Transcoder Service");
                TranscodingAPI transcoder = new TranscodingAPI(source, null);
                OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Calling CreateSymbolicLink on WCF Transcoder Service");
                bool ret = transcoder.CreateSymbolicLink(mpegFile, vob);
                string retMsg = ret ? "success" : "Sym-Link failed: " + new Win32Exception(Marshal.GetLastWin32Error()).Message;

                if (File.Exists(mpegFile))
                    return mpegFile;
                OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Soft-link creation failed! {0}", retMsg);
            }
            else
            {
                OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Media not on a network drive nor on a NTFS compatible drive, no supported");
            }

            return null;
        }

        public bool PlayMovie()
        {
            _info = _source.DVDDiskInfo;

            string videoFile = FindMPeg(_source);
            if (videoFile == null && _info != null)
            {
                OMLApplication.DebugLine("[MoviePlayerExtenderDVD] PlayMovie: DVD Disk info: '{0}'", _info);
                DVDTitle dvdTitle = _source.DVDTitle;
                if (dvdTitle != null)
                {
                    string videoTSDir = _source.VIDEO_TS;
                    int fileID = int.Parse(dvdTitle.File.Substring(4));
                    OMLApplication.DebugLine("[MoviePlayerExtenderDVD] PlayMovie: main title fileID={1} found for '{0}'", _source, fileID);
                    string vts = string.Format("VTS_{0:D2}_", fileID);
                    List<string> vobs = new List<string>(Directory.GetFiles(videoTSDir, vts + "*.VOB"));
                    vobs.Remove(Path.Combine(videoTSDir, vts + "0.VOB"));

                    if (vobs.Count < 1)
                    {
                        OMLApplication.DebugLine("[MoviePlayerExtenderDVD] PlayMovie: no VOB files found for '{0}'", _source);
                        return false;
                    }

                    string mpegFolder = Path.Combine(FileSystemWalker.ExtenderCacheDirectory, Guid.NewGuid().ToString());
                    if (Directory.Exists(mpegFolder) == false)
                    {
                        OMLApplication.DebugLine("[MoviePlayerExtenderDVD] PlayMovie: creating folder '{0}'", mpegFolder);
                        Directory.CreateDirectory(mpegFolder);
                    }

                    if (vobs.Count == 1)
                    {
                        OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Single VOB file, trying to use a hard-link approach and direct playback");
                        OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Calling MakeMPEGLink on WCF Transcoder Service");
                        string mpegFile = transcoder.MakeMPEGLink(mpegFolder, vobs[0]);
                        if (File.Exists(mpegFile))
                        {
                            OMLApplication.DebugLine("[MoviePlayerExtenderDVD] directly use MPEG");
                            videoFile = mpegFile;
                        }
                    }
                    else if (OMLSettings.Extender_MergeVOB)
                    {
                        OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Multiple VOB files, and Extender_MergeVOB == true, trying to use a hard-link and auto-merge into single large .VOB file approach ");
                        OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Calling MakeMPEGLink on WCF Transcoder Service");
                        string mpegFile = transcoder.MakeMPEGLink(mpegFolder, vobs[0]);
                        OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Merge VOB's, use first VOB to play and merge into");

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
                                    OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Deleting '{0}'", vobs[i]);
                                    File.Delete(vobs[i]);
                                }
                            }
                            finally
                            {
                                if (writer != null)
                                {
                                    OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Done merging into '{0}'", vobs[0]);
                                    writer.Close();
                                }
                            }
                        }, delegate
                        {
                            Utilities.DebugLine("[MoviePlayerExtenderDVD] Merging done, restart play, and reset posision");
                            if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                            {
                                TimeSpan cur = AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.Position;
                                Utilities.DebugLine("[MoviePlayerExtenderDVD] Current position: {0}", cur);
                                if (AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, videoFile, false))
                                {
                                    AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.Position = cur;
                                    Utilities.DebugLine("[MoviePlayerExtenderDVD] Done starting over, resetting position: {0}", cur);
                                }
                            }
                        }, null);
                    }
                    //else if (OMLSettings.Extender_UseAsx)
                    else
                    {
                        OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Multiple VOB files, and Extender_UseAsx == true, trying to use a hard-link and asx playlist approach ");
                        foreach (string vob in vobs) {
                            OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Calling MakeMPEGLink on WCF Transcoder Service");
                            transcoder.MakeMPEGLink(mpegFolder, vob);
                        }

                        if (File.Exists(GetMPEGName(mpegFolder, vobs[0])))
                            videoFile = CreateASX(mpegFolder, vts, _source.Name, vobs);
                    }
                }
                else
                    OMLApplication.DebugLine("[MoviePlayerExtenderDVD] PlayMovie: no main title found for {0}", _source);
            }
            else
                OMLApplication.DebugLine("[MoviePlayerExtenderDVD] PlayMovie: no DVD-DiskInfo found for {0}", _source);
            
            if (videoFile == null)
                videoFile = GetAsxFile();

            if (videoFile != null && AddInHost.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video, videoFile, false))
            {
                if (AddInHost.Current.MediaCenterEnvironment.MediaExperience != null)
                {
                    Utilities.DebugLine("[MoviePlayerExtenderDVD] PlayMovie: movie '{0}', Playing file '{1}'", _source.Name, videoFile);
                    OMLApplication.Current.NowPlayingMovieName = _source.Name;
                    OMLApplication.Current.NowPlayingStatus = PlayState.Playing;
                    if (_source.ResumeTime != null)
                    {
                        Utilities.DebugLine("[MoviePlayerExtenderDVD] PlayMovie: set resume time to: {0}", _source.ResumeTime);
                        AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.Position = _source.ResumeTime.Value;
                    }
                    //AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged -= MoviePlayerFactory.Transport_PropertyChanged;
                    //AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += MoviePlayerFactory.Transport_PropertyChanged;
                    //AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged -= this.Transport_PropertyChanged;
                    //AddInHost.Current.MediaCenterEnvironment.MediaExperience.Transport.PropertyChanged += this.Transport_PropertyChanged;
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
                Utilities.DebugLine("[MoviePlayerExtenderDVD] Transport_PropertyChanged: movie {0} property {1} playrate {2} state {3} pos {4}", OMLApplication.Current.NowPlayingMovieName, property, t.PlayRate, t.PlayState.ToString(), t.Position.ToString());
                if (property == "PlayState")
                {
                    switch (t.PlayState)
                    {
                        case PlayState.Finished:
                            Utilities.DebugLine("[MoviePlayerExtenderDVD] (Finished): clear resume time");
                            this._source.ClearResumeTime();
                            break;
                        case PlayState.Stopped:
                        case PlayState.Paused:
                            Utilities.DebugLine("[MoviePlayerExtenderDVD] ({0}): set resume time: {1}", t.PlayState, t.Position);
                            this._source.SetResumeTime(t.Position);
                            break;
                    }
                }
            });
        }

        static void MergeFile(FileStream writter, string vob0, string vobx)
        {
            byte[] buffer = new byte[128 * 1024 * 1024];
            OMLApplication.DebugLine("[MoviePlayerExtenderDVD] Appending '{0}' to '{1}'", vobx, vob0);
            using (FileStream reader = File.OpenRead(vobx))
                for (; ; )
                {
                    int read = reader.Read(buffer, 0, buffer.Length);
                    if (read == 0)
                        break;
                    writter.Write(buffer, 0, read);
                }
        }

        static string CreateASX(string mpegFolder, string vts, string name, IEnumerable<string> vobs)
        {
            string videoFile = Path.Combine(mpegFolder, vts.Trim('_')) + ".asx";
            if (File.Exists(videoFile))
                return videoFile;

            Utilities.DebugLine("[MoviePlayerExtenderDVD] PlayMovie: creating .asx playlist for extender for '{0}'/'{1}' " +
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
                Utilities.DebugLine("[MoviePlayerExtenderDVD] GetAsxFile: found asx {0}", fileName);
                return fileName;
            }
            Utilities.DebugLine("[MoviePlayerExtenderDVD] GetAsxFile: no asx file found {0}", _source.MediaPath);
            return null;
        }

        public static void Uninitialize()
        {
            try
            {
                foreach (string directory in Directory.GetDirectories(FileSystemWalker.ExtenderCacheDirectory))
                {
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch { }                    
                }
            }
            catch { }
        }
    }

}