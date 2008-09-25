using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel;

using OMLEngine;

namespace OMLEngineService
{
    public class TranscodingService : ITranscodingService
    {
        public void Transcode(OMLEngine.MediaSource source)
        {
            Utilities.DebugLine("Transcode: {0}", source);
        }

        public void Cancel(string key)
        {
            Utilities.DebugLine("Cancel: {0}", key);
            NotifyAll("test-notify", TranscodingStatus.BufferReady);
        }

        public TranscodingStatus GetStatus(string key)
        {
            Utilities.DebugLine("GetStatus: {0}", key);
            return TranscodingStatus.Error;
        }

        static void NotifyAll(string key, TranscodingStatus status)
        {
            Utilities.DebugLine("NotifyAll({0}, {1})", key, status);
            lock (sProxies)
                foreach (var proxyHost in sProxies.Values)
                {
                    Utilities.DebugLine("NotifyAll({0}, {1}, {2})", proxyHost.Endpoint.Address, key, status);
                    var proxy = proxyHost.Channel;
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        try
                        {
                            proxy.StatusChanged(key, status);
                        }
                        catch (Exception ex)
                        {
                            Utilities.DebugLine("NotifyAll({0}, {1}, {2}) {3}", proxyHost.Endpoint.Address, key, status, ex);
                        }
                    });
                }
        }

        static IDictionary<string, MyClientBase<ITranscodingNotifyingService>> sProxies = new Dictionary<string, MyClientBase<ITranscodingNotifyingService>>();

        public void RegisterNotifyer(string url, bool register)
        {
            lock (sProxies)
                if (register)
                {
                    if (sProxies.ContainsKey(url) == false)
                        sProxies[url] = new MyClientBase<ITranscodingNotifyingService>("TranscodingNetNotifyingService", url);
                }
                else
                    sProxies.Remove(url);
            Utilities.DebugLine("RegisterNotifyer({0}, {1}) #{2}", url, register, sProxies.Count);
        }
    }
}
