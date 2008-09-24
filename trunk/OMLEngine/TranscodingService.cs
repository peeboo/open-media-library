using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel;

using OMLEngine;

namespace OMLEngineService
{
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
            using (var host = new MyClientBase<ITranscodingService>())
                host.Channel.RegisterNotifyer(GetNotifierUri(), false);

            sHost.Close();
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
