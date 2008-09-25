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
        public void Transcode(MediaSource source)
        {
            string key = source.Key;
            lock (sTranscoders)
                if (sTranscoders.ContainsKey(key))
                {
                    Utilities.DebugLine("Transcode [Already Processing]: {0}", source);
                    NotifyAll(key, sTranscoders[key].Status);
                }
                else
                    sTranscoders[key] = new Transcoder(source);
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
            lock (sProxies)
                if (register)
                {
                    if (sProxies.ContainsKey(url) == false)
                        sProxies[url] = true;
                }
                else
                    sProxies.Remove(url);
            Utilities.DebugLine("RegisterNotifyer({0}, {1}) #{2}", url, register, sProxies.Count);
        }

        #region -- Implementation --
        internal static void NotifyAll(string key, TranscodingStatus status)
        {
            Utilities.DebugLine("NotifyAll({0}, {1})", key, status);
            lock (sProxies)
                foreach (var uri in new List<string>(sProxies.Keys))
                {
                    Utilities.DebugLine("NotifyAll({0}, {1}, {2})", uri, key, status);
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        var proxyHost = new MyClientBase<ITranscodingNotifyingService>("TranscodingNetNotifyingService", uri);
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
            FileInfo file = new FileInfo(Source.GetTranscodingFileName(FileSystemWalker.TranscodeBufferDirectory));

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
                    Process.Kill();
                }
        }
    }
}
