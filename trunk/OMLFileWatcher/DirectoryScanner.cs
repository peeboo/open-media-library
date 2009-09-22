using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;
using OMLEngine.FileSystem;
using OMLEngine.Settings;
using System.Xml;
using System.IO;
using System.Threading;
using OMLSDK;

namespace OMLFileWatcher
{
    public class DirectoryScanner
    {
        private const string DEFAULT_DISK_NAME = "Main Title";
        private const int CHANGE_TIMER_TIMEOUT = 8000;

        private System.Timers.Timer timer;
        private System.Timers.Timer badWatcherTimer;

        private List<FileSystemWatcher> watchers = null;
        private List<FileSystemWatcher> badWatchers = new List<FileSystemWatcher>();

        private IList<OMLSettings.WatchedFolder> watchFolders;
        private object lockObject = new object();

        private static object staticLockObject = new object();
        private static object syncObject = new object();

        private List<string> waitCreatedList = new List<string>();
        private List<string> waitDeletedList = new List<string>();

        internal static List<MetaDataPluginDescriptor> _metadataPlugins = null;

        public delegate void TitleAdded(object sender, Title title);
        public event TitleAdded Added;

        public delegate void ProgressMessage(object sender, string message);
        public event ProgressMessage Progress;

        public DirectoryScanner()
        {                       
            timer = new System.Timers.Timer();
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            badWatcherTimer = new System.Timers.Timer();
            badWatcherTimer.AutoReset = false;
            badWatcherTimer.Interval = CHANGE_TIMER_TIMEOUT;
            badWatcherTimer.Elapsed += new System.Timers.ElapsedEventHandler(badWatcherTimer_Elapsed);

            // Load the plugins
            _metadataPlugins = new List<MetaDataPluginDescriptor>();
            PluginServices plg = new PluginServices();
            plg.LoadMetadataPlugins(PluginTypes.MetadataPlugin, _metadataPlugins);

        }            


        /// <summary>
        /// Scans the list of directories and adds the new titles to the db
        /// </summary>
        /// <param name="directories"></param>
        public void Scan(IList<OMLSettings.WatchedFolder> directories)
        {
            // only one thread should be doing a scan at a time
            // this shouldln't happen but I'm putting a lock here for safety
            lock (staticLockObject)
            {
                //try
                {
                    int addedCount = 0;
                    bool updated = false;

                    StSanaServices StSana = new StSanaServices();
                    StSana.Log += new StSanaServices.SSEventHandler(stsana_Log);

                    DateTime start = DateTime.Now;

                    foreach (OMLSettings.WatchedFolder watchedfolder in directories)
                    {
                        DebugLine("Performing a media scan on - " + watchedfolder.Folder);
                        List<Title> titles = StSana.CreateTitlesFromPathArray(watchedfolder.ParentID,
                            new string[] { watchedfolder.Folder }, 
                            OMLSettings.ScannerSettingsTagTitlesWith);

                        addedCount += titles.Count();

                        if (titles.Count() > 0) updated = true;

                        foreach (Title title in titles)
                        {
                            if (Added != null)
                                Added(this, title);

                            if ((title.TitleType & TitleTypes.AllMedia) != 0)
                            {
                                try
                                {
                                    DebugLine("Looking up metadata for " + title.Name);
                                    LookupPreferredMetaData(title);
                                    TitleCollectionManager.SaveTitleUpdates();
                                }
                                catch (Exception ex)
                                {
                                    DebugLineError(ex, "Metadata lookup exception ");
                                }
                            }
                        }
                        DebugLine("Media scan on - " + watchedfolder.Folder + " finished");
                    }


                    // save all the image updates
                    if (updated)
                    {
                        TitleCollectionManager.SaveTitleUpdates();
                        DebugLine("Folder scanning completed.  Took {0} seconds and added {1} title(s).", (DateTime.Now - start).TotalSeconds, addedCount);
                        OMLSettings.ScannerSettingsNewTitles = OMLSettings.ScannerSettingsNewTitles + addedCount;
                    }
                    else
                        DebugLine("Folder scan resulted in no updates. Took {0} seconds", (DateTime.Now - start).TotalSeconds);
                }
                //catch (Exception err)
                {
                    //DebugLineError(err, "Error scanning folders");
                }
                //finally
                {
                    // null out the meta data plugin
                    //metaDataPlugin = null;
                }
            }
        }


        void stsana_Log(string message)
        {
            if (Progress != null)
                Progress(this, "        St Sana message : " + message);

            DebugLine("        St Sana message : " + message);
        }


        private void LookupPreferredMetaData(Title title)
        {
            MetadataSearchManagement mds = new MetadataSearchManagement(_metadataPlugins);

            bool retval = mds.MetadataSearchUsingPreferred(title);

            if (retval)
            {
                //LoadFanart(mds.FanArt, title);
            }
        }


        /// <summary>
        /// Sets up file watchers for the given folders.  When there's a change it'll scan all the
        /// folders for new items and add them to OML
        /// </summary>
        /// <param name="folders"></param>
        /// <returns>Returns false if it's in the middle of a scan and can't update the watch folders</returns>
        public bool WatchFolders(IList<OMLSettings.WatchedFolder> folders)
        {
            if (!Monitor.TryEnter(lockObject))
            {
                // if we can't enter the lock that means a scan is already in progress
                DebugLine("Unable to watch new folders since a scan is in progress.  Try again later.");
                return false;
            }
            else
            {
                try
                {
                    // stop any existing scans and watchers
                    Stop();

                    // clean up any existing watchers
                    if (watchers != null)
                    {
                        foreach (FileSystemWatcher watcher in watchers)
                        {
                            watcher.Dispose();
                        }
                    }

                    watchFolders = new List<OMLSettings.WatchedFolder>(folders);

                    // if there are no watch folders bail out
                    if (watchFolders.Count == 0)
                    {
                        DebugLine("No folders found to watch");
                        watchFolders = null;
                        Stop();
                        return true;
                    }

                    watchers = new List<FileSystemWatcher>(watchFolders.Count);

                    foreach (OMLSettings.WatchedFolder watchedfolder in watchFolders)
                    {
                        if (Directory.Exists(watchedfolder.Folder))
                        {
                            watchers.Add(CreateWatcher(watchedfolder.Folder));
                        }
                    }
                }
                catch (Exception err)
                {
                    DebugLineError(err, "Error setting up watch folders");
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }

                // immediately do an async scan of the new watch folders
                ScanASync();

                return true;
            }
        }

        /// <summary>
        /// Creates a file system watcher
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private FileSystemWatcher CreateWatcher(string folder)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(folder);
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            // only listen for file events for now
            watcher.NotifyFilter = NotifyFilters.FileName;
            watcher.Created += new FileSystemEventHandler(watcher_Created);
            watcher.Deleted += new FileSystemEventHandler(watcher_Deleted);
            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
            watcher.Error += new ErrorEventHandler(watcher_Error);

            DebugLine("Creating Watcher on " + folder);
            return watcher;
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            DebugLine("Watcher File Renamed Triggered. " + e.OldFullPath + " to " + e.FullPath);

            if (e.OldFullPath.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase))
                return;

            // reset the timer
            timer.Stop();            

            lock (syncObject)
            {                
                waitDeletedList.Add(e.OldFullPath);
                waitCreatedList.Add(e.FullPath);
            }

            StartTimer();  
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            DebugLine("Watcher File Deleted Triggered. " + e.FullPath);

            // reset the timer
            timer.Stop();  

            lock (syncObject)
            {
                if (waitCreatedList.Contains(e.FullPath, StringComparer.OrdinalIgnoreCase))
                    waitCreatedList.Remove(e.FullPath);

                waitDeletedList.Add(e.FullPath);
            }

            StartTimer();  
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            DebugLine("Watcher File Created Triggered. " + e.FullPath);
            
            // ignore directory creation
            if (Directory.Exists(e.FullPath))
                return;

            // reset the timer
            timer.Stop();  

            lock (syncObject)
            {
                if (waitDeletedList.Contains(e.FullPath, StringComparer.OrdinalIgnoreCase))
                    waitDeletedList.Remove(e.FullPath);

                waitCreatedList.Add(e.FullPath);
            }

            StartTimer();  
        }

        /// <summary>
        /// Runs a scan using the timer thread
        /// </summary>
        private void ScanASync()
        {
            timer.Interval = 250;
            timer.Start();
        }

        /// <summary>
        /// Starts the timer 
        /// </summary>
        private void StartTimer()
        {
            timer.Interval = CHANGE_TIMER_TIMEOUT;
            timer.Start();
        }

        /// <summary>
        /// if the filesystem watcher throws an error - we mostly care about it throwing when 
        /// it can't see a network share anymore.  in that case we re-create the file watcher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_Error(object sender, ErrorEventArgs e)
        {
            DebugLine("Watcher Error Triggered. " + e.ToString());

            try
            {    
                FileSystemWatcher watcher = sender as FileSystemWatcher;
              
                if ( watcher == null )
                    return;

                // if the watcher path no longer exists for whatever reason, add this watcher to the bad queue
                // to re-attempt to see if it exists every 5 seconds.  This can happen if a network drive disconnects
                // or someone removes an attached drive.  re-creating the watcher is required when the device comes
                // back online
                if (!Directory.Exists(watcher.Path))
                {
                    lock (badWatchers)
                    {
                        // add it to the bad watcher list
                        if (watchers.Contains(watcher))
                        {
                            badWatchers.Add(watcher);
                            watchers.Remove(watcher);

                            badWatcherTimer.Start();
                        }
                    }
                }                
            }
            catch (Exception err)
            {
                DebugLineError(err, "Error re-setting up watcher");
            }
        }

        /// <summary>
        /// Handle seeing if the bad watchers are fixed up yet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void badWatcherTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {            
            badWatcherTimer.Stop();

            bool badWatchersStillExist = false;

            lock (badWatchers)
            {
                for(int i = badWatchers.Count - 1 ; i >= 0 ; i --)
                {
                    // if the directory exists again re-create the watcher
                    if (Directory.Exists(badWatchers[i].Path))
                    {
                        try
                        {
                            watchers.Add(CreateWatcher(badWatchers[i].Path));
                            badWatchers.RemoveAt(i);
                        }
                        catch (Exception err)
                        {
                            badWatchersStillExist = true;
                            DebugLineError(err, "Error re-creating the watcher");
                        }
                    }
                    else
                    {
                        badWatchersStillExist = true;
                    }
                }                
            }

            // if there are still bad watchers forward the timer
            if (badWatchersStillExist)
                badWatcherTimer.Start();
        }   

        /// <summary>
        /// Starts watching folders that are being watched
        /// </summary>
        public void Start()
        {
            if (watchers != null)
            {
                foreach (FileSystemWatcher watcher in watchers)
                    watcher.EnableRaisingEvents = true;

                DebugLine("Scanning folders on service startup");

                // immediately do a scan
                ScanASync();
            }
            else
            {
                DebugLine("Start was called before folder watchers setup");
            }
        }

        /// <summary>
        /// Stops watching folders that are being watched
        /// </summary>
        public void Stop()
        {
            // stop the timer if it's around
            timer.Stop();
            
            if (watchers != null)
            {
                // turn off the file system events            
                foreach (FileSystemWatcher watcher in watchers)
                    watcher.EnableRaisingEvents = false;
            }
        }

        /// <summary>
        /// Triggers when there is a file system change.  This sets a timer to 15 seconds from now and resets
        /// the timer on every subsequent change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*private void watcher_Changed(object sender, FileSystemEventArgs e)
        {                        
            // reset the timer
            timer.Stop();
            StartTimer();  
        }*/

        /// <summary>
        /// Returns if the file is done copying which means it can be opened in exclusive read mode without error
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsFileDoneCopying(string path)
        {
            if (!File.Exists(path))            
                return false;            

            try
            {
                using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }            
        }

        /// <summary>
        /// Attempts to cleanup the pending action queues.  Will only remove files if they are
        /// properly created, removed, or renamed.
        /// </summary>
        /// <returns></returns>
        private void TryCleanupFileLists()
        {            
            // make sure all the file operations are completed
            lock (syncObject)
            {
                for (int i = waitCreatedList.Count - 1; i >= 0; i--)
                {
                    if (!IsFileDoneCopying(waitCreatedList[i]))
                    {                        
                        continue;
                    }

                    // that file is successfully created, remove it
                    waitCreatedList.RemoveAt(i);
                }

                for (int i = waitDeletedList.Count - 1; i >= 0; i--)
                {
                    if (File.Exists(waitDeletedList[i]) ||
                        Directory.Exists(waitDeletedList[i]))
                    {                        
                        continue;
                    }
                    else
                    {
                        // that file is successfully deleted, remove it
                        waitDeletedList.RemoveAt(i);
                    }
                }
            }            
        }

        /// <summary>
        /// Triggers when the timer has not seen a change for 15 seconds and causes the scan to begin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {            
            timer.Stop();

            // if there are any files pending actions make sure they're cleaned up.
            // we want to wait 8 seconds after a clean queue before we start our scan.  this way if
            // multiple files are copying we should wait till everything is done before starting our
            // scan process
            if (waitCreatedList.Count != 0 ||
                waitDeletedList.Count != 0)
            {
                // attempt to cleanup the list of pending file actions
                TryCleanupFileLists();

                StartTimer();
                return;
            }

            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    Scan(watchFolders);
                    OMLSettings.ScannerSettingsLastScanDateTime = DateTime.Now;
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }
            else
            {
                // else forward the timer to try again in 8 seconds
                StartTimer();    
            }            
        }

        /// <summary>
        /// Sends a message to the trace log prefixed this class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        private void DebugLine(string message, params object[] parameters)
        {
            if (Progress != null)
                Progress(this, string.Format(message, parameters));

            Utilities.DebugLine("[DirectoryScanner] " + message, parameters);
        }

        /// <summary>
        /// Sends an error to the tracelog that can include a message
        /// </summary>
        /// <param name="err"></param>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        private void DebugLineError(Exception err, string message, params object[] parameters)
        {
            if (err == null)
                DebugLine(message, parameters);
            else
            {
                Utilities.DebugLine("[DirectoryScanner] " + message + " '" + err.Message + "' " + err.StackTrace, parameters);
                if (Progress != null)
                    Progress(this, string.Format(message, parameters) + err.Message);
            }
        }
    }
}
