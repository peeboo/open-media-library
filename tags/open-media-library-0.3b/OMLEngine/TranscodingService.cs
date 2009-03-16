using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading;

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
            Status = TranscodingStatus.Unknown;

            TranscodingNotifyingService.Start();
            TranscodingNotifyingService.OnMediaSourceStatusChanged += TranscodingNotifyingService_OnMediaSourceStatusChanged;
        }

        void TranscodingNotifyingService_OnMediaSourceStatusChanged(string key, TranscodingStatus status)
        {
            Utilities.DebugLine("[TranscodingAPI] MediaSourceStatusChanged [Key: {0}, Status: {1}]", key, status);
            if (Source.Key == key)
            {
                if (Status != status)
                {
                    Status = status;
                    _Action(Source, status);
                }
                if (IsRunning == false)
                    TranscodingNotifyingService.Stop();
            }
        }

        public void Transcode()
        {
            Utilities.DebugLine("[TranscodingAPI] Transcode", Source);
            using (var host = TranscodingNotifyingService.NewTranscodingServiceProxy())
                host.Channel.Transcode(Source, Environment.UserName);
        }

        public void Cancel()
        {
            Utilities.DebugLine("[TranscodingAPI] Cancel", Source);
            using (var host = TranscodingNotifyingService.NewTranscodingServiceProxy())
                host.Channel.Cancel(Source.Key);
        }

        public bool IsRunning { get { return Status != TranscodingStatus.Done && Status != TranscodingStatus.Error && Status != TranscodingStatus.Stopped; } }

        public void Stop(bool wait)
        {
            Utilities.DebugLine("[TranscodingAPI] Stop", Source);
            while (wait && IsRunning)
                Thread.Sleep(500);
            //TranscodingNotifyingService.Stop();
        }
    }

    public delegate void MediaSourceStatusChanged(string key, TranscodingStatus status);

    public class TranscodingNotifyingService : ITranscodingNotifyingService
    {
        static ServiceHost sHost;
        static string sURI;

        public static string GetNotifierUri()
        {
            return string.Format("net.tcp://localhost:{0}/OMLTNS", WCFUtilites.FreeTcpPort());
        }

        public static void Start()
        {
            if (sHost != null)
            {
                Stop();
                //return;
            }

            sURI = GetNotifierUri();
            sHost = new ServiceHost(typeof(TranscodingNotifyingService), new Uri(sURI));
            sHost.AddServiceEndpoint(typeof(ITranscodingNotifyingService), WCFUtilites.NetTcpBinding(), sURI);
            sHost.Open();
            Utilities.DebugLine("[TranscodingNotifier] Starting WCF notifying service: {0}", sURI);

            using (var host = NewTranscodingServiceProxy())
                host.Channel.RegisterNotifyer(sURI, true);
        }

        public static MyClientBase<ITranscodingService> NewTranscodingServiceProxy()
        {
            return new MyClientBase<ITranscodingService>(WCFUtilites.NetTcpBinding(), new EndpointAddress("net.tcp://localhost:8321/OMLTS"));
        }

        public static MyClientBase<ITranscodingNotifyingService> NewTranscodingNotifyingServiceProxy(string uri)
        {
            return new MyClientBase<ITranscodingNotifyingService>(WCFUtilites.NetTcpBinding(), new EndpointAddress(uri));
        }

        public static void Stop()
        {
            if (sHost == null)
                return;

            using (var host = NewTranscodingServiceProxy())
                host.Channel.RegisterNotifyer(sURI, false);

            Utilities.DebugLine("[TranscodingNotifier] Stopping WCF notifying service: {0}", sURI);
            sHost.Close();
            sHost = null;
        }

        public static event MediaSourceStatusChanged OnMediaSourceStatusChanged;

        public void StatusChanged(string key, TranscodingStatus status)
        {
            Utilities.DebugLine("[TranscodingNotifier] WCF event: StatusChanged: {0}, status={1}", key, status);
            if (OnMediaSourceStatusChanged != null)
                OnMediaSourceStatusChanged(key, status);
        }
    }
}
