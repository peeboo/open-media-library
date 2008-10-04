using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace OMLEngine
{
    public static class WCFUtilites
    {
        public static NetNamedPipeBinding NetNamedPipeBinding()
        {
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            return binding;
        }

        public static NetTcpBinding NetTcpBinding()
        {
            var binding = new NetTcpBinding(SecurityMode.None);
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            return binding;
        }

        public static int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Parse("127.0.0.1"), 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        public static ServiceHost StartService(string source, Type serviceType)
        {
            try
            {
                var ret = new ServiceHost(serviceType);
                ret.Open();
                WriteToLog(source, EventLogEntryType.Information, "StartService({0})", ret.Description.Name);
                return ret;
            }
            catch (Exception ex)
            {
                WriteToLog(source, EventLogEntryType.Error, "OnStart: {0}", ex);
                return null;
            }
        }

        public static void StopService(string source, ref ServiceHost host)
        {
            try
            {
                if (host != null)
                {
                    host.Close();
                    WriteToLog(source, EventLogEntryType.Information, "StopService({0})", host.Description.Name);
                    host = null;
                }
            }
            catch (Exception ex)
            {
                WriteToLog(source, EventLogEntryType.Error, "StopService: {0}", ex);
            }
        }

        public static void WriteToLog(string source, EventLogEntryType type, string msg, params object[] args)
        {
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, string.Empty);

            msg = string.Format(msg, args);
            EventLog evt = new EventLog(string.Empty) { Source = source };
            evt.WriteEntry(msg + ": " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(), type);
            Utilities.DebugLine(msg);
        }

        public static byte[] Serialize<T>(T value) where T : class
        {
            if (value == null)
                return null;

            BinaryFormatter formatter = new BinaryFormatter();
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                formatter.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] buffer)
        {
            if (buffer == null)
                return default(T);

            BinaryFormatter formatter = new BinaryFormatter();
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer))
                return (T)formatter.Deserialize(stream);
        }
    }

    public class MyClientBase<T> : ClientBase<T>, IDisposable
      where T : class
    {
        public MyClientBase(Binding binding, EndpointAddress endPointAddress) : base(binding, endPointAddress) { }
        public MyClientBase(string endPoint, string uri) : base(endPoint, uri) { }

        public void Dispose()
        {
            if (this.State == CommunicationState.Opened)
                base.Close();
        }

        public new T Channel { get { return base.Channel; } }

        public static T GetProxy(Binding binding, EndpointAddress endPointAddress)
        {
            return new MyClientBase<T>(binding, endPointAddress).Channel;
        }
    }
}
