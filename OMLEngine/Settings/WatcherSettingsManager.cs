using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine.Dao;

namespace OMLEngine.Settings
{
    public static class WatcherSettingsManager
    {
        /// <summary>
        /// Returns all the folders that were selected to be watched
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetWatchedFolders()
        {            
            return from w in WatcherDataContext.Instance.WatchedFolders
                   select w.Folder;            
        }

        /// <summary>
        /// Returns the settings for the directory watcher
        /// </summary>
        /// <returns></returns>
        public static WatcherSettings GetSettings()
        {
            ScannerSetting settings = WatcherDataContext.Instance.ScannerSettings.SingleOrDefault();

            // if the settings hasn't been saved yet - add it now
            if (settings == null)
            {
                settings = new ScannerSetting();
                settings.LastModified = DateTime.Now;
                WatcherDataContext.Instance.ScannerSettings.InsertOnSubmit(settings);
                WatcherDataContext.Instance.SubmitChanges();
            }

            return new WatcherSettings(settings);            
        }

        /// <summary>
        /// Updates any settings changes that were made
        /// </summary>
        public static void SaveSettingUpdates()
        {
            WatcherDataContext.Instance.SubmitChanges();            
        }

        /// <summary>
        /// Returns if there have been any changes to the settings
        /// </summary>
        /// <param name="lastModified"></param>
        /// <returns></returns>
        public static bool ModifiedSince(DateTime lastModified)
        {
            using (LocalWatcherDataContext dbContext = new LocalWatcherDataContext())
            {
                return (from s in dbContext.Context.ScannerSettings
                        where s.LastModified > lastModified
                        select s).Count() > 0;
            }
        }

        private static DateTime UpdateModifiedTime()
        {
            DateTime updateTime = DateTime.Now;

            ScannerSetting settings = WatcherDataContext.Instance.ScannerSettings.SingleOrDefault();

            if (settings == null)
            {
                settings = new ScannerSetting();
                settings.LastModified = updateTime;
                WatcherDataContext.Instance.ScannerSettings.InsertOnSubmit(settings);
            }
            else
            {
                settings.LastModified = updateTime;
            }

            WatcherDataContext.Instance.SubmitChanges();

            return updateTime;
        }

        /// <summary>
        /// Sets the folders to watch
        /// </summary>
        /// <param name="folders"></param>
        /// <returns></returns>
        public static DateTime SetWatchFolders(IList<string> folders)
        {            
            // clear the existing ones
            var deleteMonitorFolders = from d in WatcherDataContext.Instance.WatchedFolders
                                       select d;

            WatcherDataContext.Instance.WatchedFolders.DeleteAllOnSubmit(deleteMonitorFolders);

            foreach (string folder in folders)
            {
                WatchedFolder watchFolder = new WatchedFolder();
                watchFolder.Folder = folder;

                WatcherDataContext.Instance.WatchedFolders.InsertOnSubmit(watchFolder);               
            }

            return UpdateModifiedTime();
        }

        /// <summary>
        /// Closes the settings db connection
        /// </summary>
        public static void CloseDBConnection()
        {
            if (Dao.WatcherDataContext.InstanceOrNull != null &&
                Dao.WatcherDataContext.Instance.Connection != null &&
                Dao.WatcherDataContext.Instance.Connection.State != System.Data.ConnectionState.Closed)
            {
                Dao.WatcherDataContext.Instance.Connection.Close();
                Dao.WatcherDataContext.Instance.Connection.Dispose();
            }
            else if (Dao.WatcherDataContext.InstanceOrNull != null &&
                Dao.WatcherDataContext.Instance.Connection != null)
            {
                Dao.WatcherDataContext.Instance.Connection.Dispose();
            }
        }
    }
}
