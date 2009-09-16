using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.ServiceModel;

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
        internal Process Process { get { return TranscodingProcess.Process; } }
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
