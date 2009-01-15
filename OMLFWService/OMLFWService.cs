using OMLFileWatcher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using System.Text;
using Microsoft.Win32;
using OMLEngine;
using OMLEngine.Settings;

namespace OMLFWService
{
    public partial class OMLFWService : ServiceBase
    {
        DirectoryScanner watcher;

        public OMLFWService()
        {
            InitializeComponent();

            watcher = new DirectoryScanner();

            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;            
        }

        private void ReloadSettings()
        {
            WatcherSettings settings = WatcherSettingsManager.GetSettings();

            if (!settings.Enabled)
            {
                watcher.Stop();
                return;
            }                        

            try
            {
                watcher.WatchFolders(WatcherSettingsManager.GetWatchedFolders());
            }
            catch (Exception err)
            {
                DebugLineError(err, "Error setting up watch folders");
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            watcher.Stop();
            WriteToLog(EventLogEntryType.Information, "OMLFWService Paused");
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            watcher.Start();
            WriteToLog(EventLogEntryType.Information, "OMLFWService Continued");
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            ReloadSettings();
            WriteToLog(EventLogEntryType.Information, "OMLFWService Started");
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            watcher.Stop();
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

        /// <summary>
        /// Sends a message to the trace log prefixed this class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        private void DebugLine(string message, params object[] parameters)
        {
            Utilities.DebugLine("[OMLFWService] " + message, parameters);
        }

        /// <summary>
        /// Sends an error to the tracelog that can include a message
        /// </summary>
        /// <param name="err"></param>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        private void DebugLineError(Exception err, string message, params object[] parameters)
        {
            Utilities.DebugLine("[OMLFWService] " + message + " '" + err.Message + "' " + err.StackTrace, parameters);
        }
    }

}
