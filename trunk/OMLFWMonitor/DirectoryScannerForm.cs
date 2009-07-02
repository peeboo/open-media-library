using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using OMLFileWatcher;

using System.Diagnostics;
using System.Timers;
using Microsoft.Win32;
using OMLEngine;
using OMLEngine.Settings;

namespace OMLFWMonitor
{
    public partial class DirectoryScannerForm : Form
    {
        private const int SETTINGS_UPDATE_TIMEOUT = 30000;
        DirectoryScanner watcher;
        System.Timers.Timer timer;
        DateTime settingsLastUpdated = DateTime.Now.AddYears(-30);

        private delegate void AddPathDelegate(string path);

        public DirectoryScannerForm()
        {
            InitializeComponent();

            SettingsManager.DisableResultCaching = true;
        }

        void watcher_Added(object sender, Title title)
        {
            BeginInvoke(new AddPathDelegate(AddPath), new object[] { title.Disks[0].Path });           
        }

        private void AddPath(string path)
        {
            listAdded.Items.Add(path);
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

            //try
            //{
                // attempt to watch some folders
                // this will return false if the folders can't be watched at this time - 
                // just use the 30 second timeout to try again then
                if (watcher.WatchFolders(OMLSettings.ScannerWatchedFolders))
                {
                    settingsLastUpdated = updatedTime;
                }                
            //}
            //catch (Exception err)
            //{
                //MessageBox.Show(err.ToString());
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OMLSettings.ScannerWatchedFolders = new List<string> { "d:\\test" };
            watcher = new DirectoryScanner();

            ReloadSettings();

            watcher.Added += new DirectoryScanner.TitleAdded(watcher_Added);

            timer = new System.Timers.Timer(SETTINGS_UPDATE_TIMEOUT);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.AutoReset = true;
            timer.Start();                        
        }
    }
}
