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
        List<MetaDataSettings> metaDataSettings = null;

        public bool Enabled { get { return settings.Enabled ?? true; } set { settings.Enabled = value; } }
        public DateTime LastUpdated { get { return settings.LastModified; } }
        public IList<MetaDataSettings> MetaDataPlugins
        {
            get
            {
                if (metaDataSettings == null)
                {
                    metaDataSettings = new List<MetaDataSettings>(MetaDataSettings.CreateMetaDataSettingsFromString(settings.MetaData));
                }

                return metaDataSettings.AsReadOnly();
            }
        }
        
        /// <summary>
        /// Constructor for creating off the db objects
        /// </summary>
        /// <param name="settings"></param>
        internal WatcherSettings(ScannerSetting settings)
        {           
            this.settings = settings;
        }

        /// <summary>
        /// Overwrites the current meta data options for the scanner with new new ones
        /// The scanner will use the meta data settings in the order they are in the list
        /// </summary>
        /// <param name="metaSettings"></param>
        public void SetMetaDataPlugins(IList<MetaDataSettings> metaSettings)
        {
            string metaString = MetaDataSettings.GetStringFromMetaDataSettings(metaSettings);

            settings.MetaData = metaString;

            WatcherSettingsManager.SaveSettingUpdates();
        }
    }
}
