using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine.Dao;

namespace OMLEngine.Settings
{
    public class WatcherSettings
    {
        ScannerSetting settings = null;

        public bool Enabled { get { return settings.Enabled ?? true; } set { settings.Enabled = value; } }        

        internal WatcherSettings(ScannerSetting settings)
        {           
            this.settings = settings;
        }
    }
}
