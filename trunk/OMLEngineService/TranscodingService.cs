using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ComponentModel;

using OMLEngine;
using OMLTranscoder;

namespace OMLEngineService
{
    public class TranscodingService : ITranscodingService
    {
        public void Transcode(MediaSource source, string mcxuser)
        {
            // Modified to ensure a user will only transcode one file at a time.
            string key = source.Key;
            lock (sTranscoders)
            {
                if (sTranscoders.ContainsKey(key))
                {
                    // Transcoding service is/was allready transcoding the file
                    Utilities.DebugLine("Transcode [Already Processing]: {0}, status={1}", source, sTranscoders[key].Status);

                    if ((sTranscoders[key].Status == TranscodingStatus.BufferReady) ||
                        (sTranscoders[key].Status == TranscodingStatus.Done) ||
                        (sTranscoders[key].Status == TranscodingStatus.Initializing))
                    {
                        NotifyAll(key, sTranscoders[key].Status);
                        return;
                    }
                }
            }

            // This file if not allready being transcoded. Check user is not allready transcoding
            lock (sUserSession)
            {
                if (sUserSession.ContainsKey(mcxuser))
                {
                    // Allready transcoding, cancel the job
                    Utilities.DebugLine("Transcode [User allready transcoding]: {0}", mcxuser);
                    if (sTranscoders.ContainsKey(sUserSession[mcxuser]))
                    {
                        if ((sTranscoders[sUserSession[mcxuser]].Status == TranscodingStatus.BufferReady) ||
                            (sTranscoders[sUserSession[mcxuser]].Status == TranscodingStatus.Initializing))
                        {
                            Cancel(sUserSession[mcxuser]);
                        }
                    }
                    sUserSession.Remove(mcxuser);
                }
            }

            // Start transcoding session
            sTranscoders[key] = new Transcoder(source);
            sUserSession[mcxuser] = key;
        }

        public void Cancel(string key)
        {
            Transcoder t = null;
            lock (sTranscoders)
                if (sTranscoders.ContainsKey(key) == false)
                    Utilities.DebugLine("Cancel [Not present]: {0}", key);
                else
                {
                    t = sTranscoders[key];
                    sTranscoders.Remove(key);
                }
            if (t != null)
                t.Cancel();
        }

        public TranscodingStatus GetStatus(string key)
        {
            TranscodingStatus ret = TranscodingStatus.Error;
            lock (sTranscoders)
                if (sTranscoders.ContainsKey(key))
                    ret = sTranscoders[key].Status;
            Utilities.DebugLine("GetStatus: {0}, status={1}", key, ret);
            return ret;
        }

        public void RegisterNotifyer(string url, bool register)
        {
            Utilities.DebugLine("RegisterNotifyer({0}, {1}) #{2}", url, register, sProxies.Count);

            lock (sProxies)
                if (register)
                {
                    if (sProxies.ContainsKey(url) == false)
                    {
                        sProxies[url] = true;
                        Utilities.DebugLine("RegisterNotifyer Success({0}, {1}) #{2}", url, register, sProxies.Count);
                    }
                }
                else
                    sProxies.Remove(url);
        }

        public string MakeMPEGLink(string mpegFolder, string vob) {
            string mpegFile = GetMPEGName(mpegFolder, vob);
            if (File.Exists(mpegFile))
                return mpegFile;

            bool ret = CreateSymbolicLink(mpegFile, vob, SYMLINK_FLAG_FILE);
            string retMsg = ret ? "success" : "Sym-Link failed: " + new Win32Exception(Marshal.GetLastWin32Error()).Message;
            if (File.Exists(mpegFile)) {
                Utilities.DebugLine(string.Format("created a sym-link {0} -> {1}, kernel32 success", vob, mpegFile));
                return mpegFile;
            }

            Utilities.DebugLine("created a sym-link {0} -> {1}, failed, {2}", vob, mpegFile, retMsg);

            string args = string.Format("/c mklink \"{0}\" \"{1}\"", mpegFile, vob);
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("cmd.exe", args);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
            p.WaitForExit();
            int exitCode = p.ExitCode;

            if (File.Exists(mpegFile)) {
                Utilities.DebugLine("created a sym-link {0} -> {1}, cmd.exe success", vob, mpegFile);
                return mpegFile;
            }

            Utilities.DebugLine("created a sym-link {0} -> {1}, mklink failed: args:'{2}', exit code: {3}", vob, mpegFile, args, exitCode);

            ret = CreateHardLinkAPI(mpegFile, vob, IntPtr.Zero);
            retMsg = ret ? "success" : "Hard-Link failed: " + new Win32Exception(Marshal.GetLastWin32Error()).Message;
            if (File.Exists(mpegFile)) {
                Utilities.DebugLine("created a hard-link {0} -> {1}, success", vob, mpegFile);
               return mpegFile;
            }

            Utilities.DebugLine("failed to create link {0} -> {1}, neither with sym-link, mklink nor hard-link", vob, mpegFile);
            return null;
        }

        public bool CreateSymbolicLink(string mpegFile, string vob) {
            return CreateSymbolicLink(mpegFile, vob, SYMLINK_FLAG_FILE);
        }

        private static string GetMPEGName(string mpegFolder, string vob) {
            return Path.Combine(mpegFolder, Path.GetFileNameWithoutExtension(vob)) + ".MPG";
        }

        #region -- Implementation --
        internal static void NotifyAll(string key, TranscodingStatus status)
        {
            Utilities.DebugLine("NotifyAll({0}, {1}) #{2}", key, status, sProxies.Count);
            lock (sProxies)
                foreach (var uri in new List<string>(sProxies.Keys))
                {
                    Utilities.DebugLine("NotifyAll({0}, {1}, {2})", uri, key, status);
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        var proxyHost = TranscodingNotifyingService.NewTranscodingNotifyingServiceProxy(uri);
                        try
                        {
                            proxyHost.Channel.StatusChanged(key, status);
                        }
                        catch (CommunicationException ex)
                        {
                            lock (sProxies)
                                sProxies.Remove(uri);
                            Utilities.DebugLine("NotifyAll({0}, {1}, {2}) Removing from registration: {3}", proxyHost.Endpoint.Address, key, status, ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Utilities.DebugLine("NotifyAll({0}, {1}, {2}) {3}", proxyHost.Endpoint.Address, key, status, ex);
                        }
                        finally
                        {
                            try
                            {
                                proxyHost.Close();
                            }
                            catch { }
                        }
                    });
                }
        }

        static IDictionary<string, Transcoder> sTranscoders = new Dictionary<string, Transcoder>();
        static IDictionary<string, string> sUserSession = new Dictionary<string, string>();
        static IDictionary<string, bool> sProxies = new Dictionary<string, bool>();
        #endregion

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
        #endregion
    }

    internal class Transcoder
    {
        internal Transcoder(MediaSource source)
        {
            TranscodingProcess = new Transcode(source);
            TranscodingProcess.Exited += TranscodingProcess_Exited;
            Timer = new System.Timers.Timer(1000);
            Timer.AutoReset = true;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
            lock (this)
                Status = TranscodingProcess.BeginTranscodeJob() != 0 ? TranscodingStatus.Error : TranscodingStatus.Initializing;
            TranscodingService.NotifyAll(source.Key, Status);
        }

        static int BufferSize = 5 * 1024 * 1024;

        static Transcoder()
        {
            int buffer;
            if (int.TryParse(ConfigurationManager.AppSettings["Transcode.BufferSize"], out buffer))
                BufferSize = buffer;
        }

        void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FileInfo file = new FileInfo(Source.GetTranscodingFileName());

            if (Status != TranscodingStatus.Initializing)
            {
                Utilities.DebugLine("Timer: stopping timer, file={0}, status={1}", file.Name, Status);
                Timer.Stop();
                return;
            }

            Utilities.DebugLine("Timer: outfile {0}, exists: {1}, size: {2}", file.Name, file.Exists, file.Exists ? file.Length : -1);
            if (file.Exists && file.Length > 5 * 1024 * 1024)
            {
                Timer.Stop();
                lock (this)
                {
                    if (Status == TranscodingStatus.Initializing)
                    {
                        Status = TranscodingStatus.BufferReady;
                        TranscodingService.NotifyAll(Source.Key, Status);
                    }
                }
            }
        }

        void TranscodingProcess_Exited(object sender, EventArgs e)
        {
            lock (this)
                if (Status != TranscodingStatus.Stopped)
                    Status = TranscodingProcess.Process.ExitCode != 0 ? TranscodingStatus.Error : TranscodingStatus.Done;
            TranscodingService.NotifyAll(Source.Key, Status);
        }

        internal MediaSource Source { get { return TranscodingProcess.Source; } }
        internal Transcode TranscodingProcess { get; private set; }
        internal System.Diagnostics.Process Process { get { return TranscodingProcess.Process; } }
        internal TranscodingStatus Status { get; private set; }
        internal System.Timers.Timer Timer { get; private set; }

        internal void Cancel()
        {
            Timer.Stop();
            lock (this)
                if (Status != TranscodingStatus.Error && Status != TranscodingStatus.Done && Status != TranscodingStatus.Stopped)
                {
                    Status = TranscodingStatus.Stopped;
                    TranscodingProcess.Exited -= TranscodingProcess_Exited;
                    Process.Kill();
                }
        }
    }
}
