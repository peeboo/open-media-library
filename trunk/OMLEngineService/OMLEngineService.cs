using System;
using System.ServiceProcess;
using System.Timers;
using System.Diagnostics;
using OMLEngine;
using System.ServiceModel;

namespace OMLEngineService
{
    public partial class OMLEngineService : ServiceBase
    {
        private TitleCollection tc;
        private Timer _timer;
        private ServiceHost _serviceHost;

        public enum Commands
        {
            WriteToLog = 128,
            StartOMLFWService = 129,
            StopOMLFWService = 130
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);

            switch (command)
            {
                case (int)Commands.WriteToLog:
                    break;
                case (int)Commands.StartOMLFWService:
                    StartOMLFWService();
                    break;
                case (int)Commands.StopOMLFWService:
                    StopOMLFWService();
                    break;
                default:
                    WriteToLog(EventLogEntryType.Error, "I have NO idea what command you wanted " + command);
                    break;
            }
        }

        public OMLEngineService()
        {
            this.ServiceName = @"OMLEngineService";
            _timer = new Timer(20000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);

            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;

            tc = new TitleCollection();
            tc.loadTitleCollection();
        }

        #region overridden control methods (start, stop, pause, continue, etc)
        protected override void OnStart(string[] args)
        {
            WriteToLog(EventLogEntryType.Information, "OMLEngineService Start");
            _timer.Start();

            try
            {
                _serviceHost = new ServiceHost(typeof(TranscodingService));
                _serviceHost.Open();
            }
            catch (Exception ex)
            {
                WriteToLog(EventLogEntryType.Error, "OnStart: {0}", ex);
            }
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            _timer.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _timer.Stop();
        }

        protected override void OnStop()
        {
            _timer.Stop();
            try
            {
                if (_serviceHost != null)
                {
                    _serviceHost.Close();
                    _serviceHost = null;
                }
            }
            catch (Exception ex)
            {
                WriteToLog(EventLogEntryType.Error, "OnStop: {0}", ex);
            }
            WriteToLog(EventLogEntryType.Information, "OMLEngineService Stop");
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _timer.Stop();
        }
        #endregion

        internal static void WriteToLog(EventLogEntryType type, string msg, params object[] args)
        {
            const string source = @"OMLEngineService";

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, string.Empty);

            msg = string.Format(msg, args);
            EventLog evt = new EventLog(string.Empty) { Source = source };
            evt.WriteEntry(msg + ": " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(), type);
            Utilities.DebugLine(msg);
        }

        protected void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // WriteToLog(EventLogEntryType.Information, "OMLEngineService Timer triggered");
        }

        private void StartOMLFWService()
        {
            WriteToLog(EventLogEntryType.Information, "Starting OMLFWService");
            ServiceController sc = new ServiceController("OMLFWService");
            if (sc != null)
            {
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(_StartOMLFWService, sc);
                    sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 10));
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        WriteToLog(EventLogEntryType.Error, "OMLFWService: service startup timed out");
                    }
                }
            }
        }

        private void _StartOMLFWService(object o)
        {
            try
            {
                ServiceController sc = (ServiceController)o;
                sc.Start();
            }
            catch (Exception ex)
            {
                WriteToLog(EventLogEntryType.Error, "Failed to convert passed object to a ServiceController: " + ex.Message);
            }
        }

        private void StopOMLFWService()
        {
            WriteToLog(EventLogEntryType.Information, "Stopping OMLFWService");
            ServiceController sc = new ServiceController("OMLFWService");
            if (sc != null)
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(_StopOMLFWService, sc);
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 10));
                    if (sc.Status != ServiceControllerStatus.Stopped)
                    {
                        WriteToLog(EventLogEntryType.Error, "OMLFWService: service shutdown timed out");
                    }
                }
            }
        }

        private void _StopOMLFWService(object o)
        {
            try
            {
                ServiceController sc = (ServiceController)o;
                sc.Stop();
            }
            catch (Exception ex)
            {
                WriteToLog(EventLogEntryType.Error, "Failed to convert passed object to a ServiceController: " + ex.Message);
            }
        }
    }
}
