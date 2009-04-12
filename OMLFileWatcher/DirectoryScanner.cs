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
        private const int CHANGE_TIMER_TIMEOUT = 15000;

        private System.Timers.Timer timer;
        private List<FileSystemWatcher> watchers = null;
        private IList<string> watchFolders;
        private object lockObject = new object();
        private static object staticLockObject = new object();

        IOMLMetadataPlugin metaDataPlugin = null;        

        public DirectoryScanner()
        {                       
            timer = new System.Timers.Timer();
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
        }       

        /// <summary>
        /// Get's a meta data filled title for the given name
        /// </summary>
        /// <param name="title"></param>
        public Title GetTitleMetaData(string name)
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
        }

        /// <summary>
        /// Initializes the meta data source of choice
        /// </summary>
        private void SetupMetaDataPlugin()
        {
            try
            {
                WatcherSettings settings = WatcherSettingsManager.GetSettings();

                // if no meta plugins are set just bail out
                if (settings.MetaDataPlugins == null && settings.MetaDataPlugins.Count == 0)
                    return;

                // todo : solomon : eventually we should support an order of meta data plugins to allow
                // for more than one - sadly that day isn't here yet
                MetaDataSettings metaDataSettings = settings.MetaDataPlugins[0];

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
        }

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
                        Title title = GetTitleMetaData(FileHelper.GetNameFromPath(path));

                        title.AddDisk(new Disk(DEFAULT_DISK_NAME, path, FileScanner.GetVideoFormatFromPath(path)));

                        // save the title to the database
                        TitleCollectionManager.AddTitle(title);

                        updated = true;
                        adddedCount++;
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
            try
            {
                // if we can't enter the lock that means a scan is already in progress
                if (!Monitor.TryEnter(lockObject))
                {
                    DebugLine("Unable to watch new folders since a scan is in progress.  Try again later.");
                    return false;
                }

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

                foreach (string folder in folders)
                {
                    if (Directory.Exists(folder))
                    {
                        FileSystemWatcher watcher = new FileSystemWatcher(folder);
                        watcher.IncludeSubdirectories = true;
                        watcher.EnableRaisingEvents = true;
                        //watcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;                    
                        watcher.Changed += new FileSystemEventHandler(watcher_Changed);
                        watcher.Error += new ErrorEventHandler(watcher_Error);

                        watchers.Add(watcher);
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
                DebugLineError(e.GetException(), "Error in Filesystem watcher - mostly likely a disconnected network");                

                // if we can't find the drive anymore setup a new watcher
                if (e.GetException().Message == "The specified network name is no longer available")
                    sender = new FileSystemWatcher(((FileSystemWatcher)sender).Path);
            }
            catch (Exception err)
            {
                DebugLineError(err, "Error re-setting up watcher");
            }
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
        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {                        
            // reset the timer
            timer.Stop();
            StartTimer();  
        }

        /// <summary>
        /// Triggers when the timer has not seen a change for 15 seconds and causes the scan to begin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {            
            timer.Stop();

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
                // else forward the timer to try again in 15 seconds
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
