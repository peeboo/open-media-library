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

        private IList<string> watchFolders;
        private object lockObject = new object();

        private static object staticLockObject = new object();
        private static object syncObject = new object();

        private List<string> waitCreatedList = new List<string>();
        private List<string> waitDeletedList = new List<string>();

        IOMLMetadataPlugin metaDataPlugin = null;

        public delegate void TitleAdded(object sender, Title title);
        public event TitleAdded Added;


        public DirectoryScanner()
        {                       
            timer = new System.Timers.Timer();
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            badWatcherTimer = new System.Timers.Timer();
            badWatcherTimer.AutoReset = false;
            badWatcherTimer.Interval = CHANGE_TIMER_TIMEOUT;
            badWatcherTimer.Elapsed += new System.Timers.ElapsedEventHandler(badWatcherTimer_Elapsed);
        }            

        /// <summary>
        /// Get's a meta data filled title for the given name
        /// </summary>
        /// <param name="title"></param>
        /*public Title GetTitleMetaData(string name)
        {            
            Title returnTitle = null;

            // setup the meta data plugin the first time we need it
            if (metaDataPlugin == null)
                SetupMetaDataPlugin();

            // if the setup failed let's bail out
            if (metaDataPlugin == null)
            {
                return new Title() { Name = name };
            }

            DebugLine("Attemping to get meta data for '{0}'", name);

            try
            {
                metaDataPlugin.SearchForMovie(name);
                returnTitle = metaDataPlugin.GetBestMatch();
            }
            catch (Exception err)
            {
                DebugLineError(err, "Error getting meta data");
            }

            if (returnTitle == null)
                DebugLine("Not meta data found for " + name);
            else
                DebugLine("Meta Data found for " + name);

            return returnTitle ?? new Title() { Name = name };            
        }*/

        /// <summary>
        /// Initializes the meta data source of choice
        /// </summary>
        /*private void SetupMetaDataPlugin()
        {
            try
            {
                IList<string> plugins = OMLSettings.ScannerMetaDataPlugins;

                // if no meta plugins are set just bail out
                if (plugins == null && plugins.Count == 0)
                    return;

                // todo : solomon : eventually we should support an order of meta data plugins to allow
                // for more than one - sadly that day isn't here yet
                MetaDataSettings metaDataSettings = GetTitleMetaData(plugins[0]);

                List<PluginServices.AvailablePlugin> plugins = PluginServices.FindPlugins(FileSystemWalker.PluginsDirectory, PluginTypes.MetadataPlugin);

                // Loop through available plugins, creating instances and add them
                if (plugins != null)
                {
                    foreach (PluginServices.AvailablePlugin plugin in plugins)
                    {
                        IOMLMetadataPlugin foundPlugin = (IOMLMetadataPlugin)PluginServices.CreateInstance(plugin);

                        if (foundPlugin.PluginName.Equals(metaDataSettings.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            metaDataPlugin = foundPlugin;

                            if (metaDataSettings.Options != null)
                            {
                                foreach (KeyValuePair<string, string> pair in metaDataSettings.Options)
                                {
                                    DebugLine("Setting meta data plugin options '{0}' : '{1]'", pair.Key, pair.Value);
                                    
                                    metaDataPlugin.SetOptionValue(pair.Key, pair.Value);
                                }
                            }

                            metaDataPlugin.Initialize(null);

                            DebugLine("Loaded meta data plugin '{0}'", foundPlugin.PluginName);
                            break;
                        }
                    }
                }

                if (metaDataPlugin == null)
                {
                    DebugLine("Meta data plugin not found : " + metaDataSettings.Name);
                }
            }
            catch (Exception err)
            {
                DebugLineError(err, "Error could not load meta data plugin");
            }
        }*/

        /// <summary>
        /// Scans the directory for any new media and adds the titles to the db
        /// </summary>
        /// <param name="directory"></param>
        public void Scan(string directory)
        {
            Scan(new string[] { directory });
        }

        /// <summary>
        /// Scans the list of directories and adds the new titles to the db
        /// </summary>
        /// <param name="directories"></param>
        public void Scan(IList<string> directories)
        {
            // only one thread should be doing a scan at a time
            // this shouldln't happen but I'm putting a lock here for safety
            lock (staticLockObject)
            {
                try
                {
                    DateTime start = DateTime.Now;
                    int adddedCount = 0;
                    bool updated = false;
                    IEnumerable<string> media = FileScanner.GetAllMediaFromPath(directories);
                    IEnumerable<string> uniqueMedia = TitleCollectionManager.GetUniquePaths(media);

                    foreach (string path in uniqueMedia)
                    {
                        // todo : solomon : i'm ignoring the meta data stuff for now

                        // add the new title as unkown so the UI can filter on it
                        Title title = new Title() { Name = FileHelper.GetNameFromPath(path), TitleType = TitleTypes.Root | TitleTypes.Unknown }; 
                        //GetTitleMetaData(FileHelper.GetNameFromPath(path));

                        title.AddDisk(new Disk(DEFAULT_DISK_NAME, path, FileScanner.GetVideoFormatFromPath(path)));

                        // save the title to the database
                        TitleCollectionManager.AddTitle(title);

                        updated = true;
                        adddedCount++;

                        // fire the event
                        if (Added != null)
                            Added(this, title);                                
                    }

                    // save all the image updates
                    if (updated)
                    {
                        TitleCollectionManager.SaveTitleUpdates();
                        DebugLine("Folder scanning completed.  Took {0} seconds and added {1} title(s).", (DateTime.Now - start).TotalSeconds, adddedCount);
                    }
                    else
                        DebugLine("Folder scan resulted in no updates. Took {0} seconds", (DateTime.Now - start).TotalSeconds);
                }
                catch (Exception err)
                {
                    DebugLineError(err, "Error scanning folders");
                }
                finally
                {
                    // null out the meta data plugin
                    metaDataPlugin = null;
                }
            }
        }

        /// <summary>
        /// Sets up file watchers for the given folders.  When there's a change it'll scan all the
        /// folders for new items and add them to OML
        /// </summary>
        /// <param name="folders"></param>
        /// <returns>Returns false if it's in the middle of a scan and can't update the watch folders</returns>
        public bool WatchFolders(IEnumerable<string> folders)
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

                    watchFolders = new List<string>(folders);

                    // if there are no watch folders bail out
                    if (watchFolders.Count == 0)
                    {
                        DebugLine("No folders found to watch");
                        watchFolders = null;
                        Stop();
                        return true;
                    }

                    watchers = new List<FileSystemWatcher>(watchFolders.Count);

                    foreach (string folder in watchFolders)
                    {
                        if (Directory.Exists(folder))
                        {                            
                            watchers.Add(CreateWatcher(folder));
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

            return watcher;
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {            
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
                Utilities.DebugLine("[DirectoryScanner] " + message + " '" + err.Message + "' " + err.StackTrace, parameters);
        }
    }
}
