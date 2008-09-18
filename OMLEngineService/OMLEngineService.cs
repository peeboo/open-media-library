using System;
using System.ServiceProcess;
using System.Timers;
using System.Diagnostics;
using OMLEngine;

namespace OMLEngineService
{
    public partial class OMLEngineService : ServiceBase
    {
        private TitleCollection tc;
        private Timer _timer;

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
                    WriteToLog("I have NO idea what command you wanted " + command);
                    break;
            }
        }

        public OMLEngineService()
        {
            this.ServiceName = "OMLEngineService";
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
            WriteToLog("OMLEngineService Start ");
            _timer.Start();
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
            WriteToLog("OMLEngineService Stop ");
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            _timer.Stop();
        }
        #endregion

        private void WriteToLog(string msg)
        {
            string sSource = @"OMLEngineService";
            string sLog = string.Empty;

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);

            EventLog evt = new EventLog(sLog);
            string message = msg + ": " +
                                   DateTime.Now.ToShortDateString() + " " +
                                   DateTime.Now.ToLongTimeString();
            evt.Source = sSource;
            evt.WriteEntry(message, EventLogEntryType.Information);
        }

        protected void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WriteToLog("OMLEngineService Timer");
        }
    }
}
