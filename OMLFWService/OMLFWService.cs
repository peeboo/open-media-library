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
        private const int SETTINGS_UPDATE_TIMEOUT = 30000;
        DirectoryScanner watcher;
        Timer timer;
        DateTime settingsLastUpdated = DateTime.Now.AddYears(-30);

        public OMLFWService()
        {
            SettingsManager.DisableResultCaching = true;

            InitializeComponent();

            watcher = new DirectoryScanner();

            timer = new Timer(SETTINGS_UPDATE_TIMEOUT);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.AutoReset = true;
            timer.Start();

            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;            
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (OMLSettings.ScannerSettingsLastUpdated > settingsLastUpdated)
            {
                ReloadSettings();
            }
        }      

        private void ReloadSettings()
        {
            DateTime updatedTime = OMLSettings.ScannerSettingsLastUpdated;

            if (!OMLSettings.ScannerEnabled)
            {
                settingsLastUpdated = updatedTime;
                watcher.Stop();
                return;
            }                        

            try
            {
                // attempt to watch some folders
                // this will return false if the folders can't be watched at this time - 
                // just use the 30 second timeout to try again then
                if (watcher.WatchFolders(OMLSettings.ScannerWatchedFolders))
                {
                    settingsLastUpdated = updatedTime;
                }                
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
            timer.Enabled = false;            
            WriteToLog(EventLogEntryType.Information, "OMLFWService Paused");
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            watcher.Start();
            timer.Enabled = true;
            WriteToLog(EventLogEntryType.Information, "OMLFWService Continued");
        }

        protected override void OnStart(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            // TODO: Add code here to start your service.
            ReloadSettings();
            timer.Enabled = true;
            WriteToLog(EventLogEntryType.Information, "OMLFWService Started");
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            watcher.Stop();
            timer.Enabled = false;
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
