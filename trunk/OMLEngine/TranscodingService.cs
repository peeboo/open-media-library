using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel;

using OMLEngine;

namespace OMLEngineService
{
    public delegate void TranscodingStatusChanged(MediaSource source, TranscodingStatus status);

    public class TranscodingAPI
    {
        public MediaSource Source { get; private set; }
        public TranscodingStatus Status { get; private set; }

        TranscodingStatusChanged _Action;

        public TranscodingAPI(MediaSource source, TranscodingStatusChanged action)
        {
            Source = source;
            _Action = action;

            TranscodingNotifyingService.Start();
            TranscodingNotifyingService.OnMediaSourceStatusChanged += TranscodingNotifyingService_OnMediaSourceStatusChanged;
        }

        void TranscodingNotifyingService_OnMediaSourceStatusChanged(string key, TranscodingStatus status)
        {
            Utilities.DebugLine("Key: {0}, Status: {1}", key, status);
            if (Source.Key == key)
            {
                Status = status;
                _Action(Source, status);
                if (IsRunning == false)
                    TranscodingNotifyingService.Stop();
            }
        }

        public void Transcode()
        {
            using (var host = new MyClientBase<ITranscodingService>())
                host.Channel.Transcode(Source);
        }

        public void Cancel()
        {
            using (var host = new MyClientBase<ITranscodingService>())
                host.Channel.Cancel(Source.Key);
        }

        public bool IsRunning { get { return Status != TranscodingStatus.Done && Status != TranscodingStatus.Error && Status != TranscodingStatus.Stopped; } }

        public void Stop()
        {
            while (IsRunning)
                Thread.Sleep(500);
            TranscodingNotifyingService.Stop();
        }
    }

    public delegate void MediaSourceStatusChanged(string key, TranscodingStatus status);

    public class TranscodingNotifyingService : ITranscodingNotifyingService
    {
        static ServiceHost sHost;

        public static string GetNotifierUri()
        {
            return string.Format("net.pipe://localhost/OMLTNS{0}", Process.GetCurrentProcess().Id);
        }

        public static void Start()
        {
            if (sHost != null)
                return;

            string uri = GetNotifierUri();
            sHost = new ServiceHost(typeof(TranscodingNotifyingService), new Uri(uri));
            sHost.Description.Endpoints.Clear();
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;
            sHost.AddServiceEndpoint(typeof(ITranscodingNotifyingService), binding, "");
            sHost.Open();

            using (var host = new MyClientBase<ITranscodingService>())
                host.Channel.RegisterNotifyer(uri, true);
        }

        public static void Stop()
        {
            if (sHost == null)
                return;

            using (var host = new MyClientBase<ITranscodingService>())
                host.Channel.RegisterNotifyer(GetNotifierUri(), false);

            sHost.Close();
            sHost = null;
        }

        public static event MediaSourceStatusChanged OnMediaSourceStatusChanged;

        public void StatusChanged(string key, TranscodingStatus status)
        {
            Utilities.DebugLine("StatusChanged: {0}, status={1}", key, status);
            if (OnMediaSourceStatusChanged != null)
                OnMediaSourceStatusChanged(key, status);
        }
    }
}
