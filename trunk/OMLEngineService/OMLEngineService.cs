using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;

using OMLEngine;

namespace OMLEngineService
{
    public partial class OMLEngineService : ServiceBase
    {
        private TitleCollection tc;
        private Timer _timer;
        private ServiceHost _transcodingServiceHost, _titleCollectionServiceHost;

        public enum Commands
        {
            WriteToLog = 128
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);

            switch (command)
            {
                case (int)Commands.WriteToLog:
                    break;
                default:
                    WriteToLog(EventLogEntryType.Error, "I have NO idea what command you wanted " + command);
                    break;
            }
        }

        public OMLEngineService()
        {
            this.ServiceName = @"OMLEngineService";
            _timer = new Timer(60000); // 1 minute
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
//            System.Diagnostics.Debugger.Launch();
            WriteToLog(EventLogEntryType.Information, "OMLEngineService Start");
            _timer.Start();

            _transcodingServiceHost = WCFUtilites.StartService(EventSource, typeof(TranscodingService));
            _titleCollectionServiceHost = WCFUtilites.StartService(EventSource, typeof(TitleCollectionAPI));
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
            WCFUtilites.StopService(EventSource, ref _transcodingServiceHost);
            WCFUtilites.StopService(EventSource, ref _titleCollectionServiceHost);
            WriteToLog(EventLogEntryType.Information, "OMLEngineService Stop");
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _timer.Stop();
        }
        #endregion

        const string EventSource = "OMLEngineService";
        internal static void WriteToLog(EventLogEntryType type, string msg, params object[] args)
        {
            WCFUtilites.WriteToLog(EventSource, type, msg, args);
        }

        protected void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // WriteToLog(EventLogEntryType.Information, "OMLEngineService Timer triggered");
        }
    }
}
