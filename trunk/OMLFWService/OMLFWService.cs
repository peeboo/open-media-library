using OMLFileWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using System.Text;
using Microsoft.Win32;

namespace OMLFWService
{
    public partial class OMLFWService : ServiceBase
    {
        OMLFileWatcher.OMLFileWatcher fw;
        RegistryKey rkOML;
        List<string> watches = new List<string>();
        string[] watch;
        Boolean logging = false;
        Boolean subdirs;
        Timer _timer;

        public OMLFWService()
        {
            InitializeComponent();

            fw = new OMLFileWatcher.OMLFileWatcher();

            RegistryKey rkCU = Registry.CurrentUser;
            RegistryKey rkSoftware = rkCU.OpenSubKey(@"Software");
            rkOML = rkSoftware.OpenSubKey(@"OpenMediaLibrary");

            _timer = new Timer(60000); // 1 minute intervals
            _timer.Elapsed +=new ElapsedEventHandler(_timer_Elapsed);

            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;

            //int shouldLog = (Int32)rkOML.GetValue("logging", 0);
            //logging = shouldLog == 0 ? false : true;
            logging = true;

            try
            {
                foreach (string watchitem in ((string[])rkOML.GetValue("Watches")))
                {
                    watches.Add(watchitem);
                }
            }
            catch (Exception e)
            {
                watches.Add("c:\\users\\public\\recorded tv;*.*");
            }

            foreach (string set in watches)
            {
                watch = set.Split(';');
                switch (watch.Length)
                {
                    case 1:
                    case 2:
                        fw.AddWatch(watch[0], watch[1]);
                        break;
                    case 3:
                        if (!Boolean.TryParse(watch[2], out subdirs))
                        {
                            subdirs = false;
                        }
                        fw.AddWatch(watch[0], watch[1], subdirs);
                        break;
                    default:
                        break;
                }
            }
            rkSoftware.Close();
            rkCU.Close();
        }

        protected override void OnPause()
        {
            base.OnPause();
            fw.EnableRaisingEvents = false;
            _timer.Stop();
            WriteToLog(EventLogEntryType.Information, "OMLFWService Paused");
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            fw.EnableRaisingEvents = true;
            _timer.Start();
            WriteToLog(EventLogEntryType.Information, "OMLFWService Continued");
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            fw.EnableRaisingEvents = true;
            _timer.Start();
            WriteToLog(EventLogEntryType.Information, "OMLFWService Started");
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            fw.EnableRaisingEvents = false;
            _timer.Stop();
            WriteToLog(EventLogEntryType.Information, "OMLFWService Stopped");
        }

        private void WriteToLog(EventLogEntryType type, string msg)
        {
            string sSource = @"OMLFWService";
            string sLog = string.Empty;

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);

            EventLog evt = new EventLog(sLog);
            string message = msg + ": " +
                                   DateTime.Now.ToShortDateString() + " " +
                                   DateTime.Now.ToLongTimeString();
            evt.Source = sSource;
            evt.WriteEntry(message, type);
        }

        protected void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            WriteToLog(EventLogEntryType.Information, "OMLFWService Timer triggered");
        }
    }

}
