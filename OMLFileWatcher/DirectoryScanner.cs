using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;
using OMLEngine.FileSystem;
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
        private List<FileSystemWatcher> watchers;
        private IList<string> watchFolders;
        private object lockObject = new object();        

        IOMLMetadataPlugin metaDataPlugin = null;        

        public DirectoryScanner()
        {                       
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

            metaDataPlugin.SearchForMovie(name);

            returnTitle = metaDataPlugin.GetBestMatch();

            return returnTitle ?? new Title() { Name = name };            
        }

        /// <summary>
        /// Initializes the meta data source of choice
        /// </summary>
        private void SetupMetaDataPlugin()
        {
            List<PluginServices.AvailablePlugin> plugins = PluginServices.FindPlugins(FileSystemWalker.PluginsDirectory, PluginTypes.MetadataPlugin);

            // Loop through available plugins, creating instances and add them
            if (plugins != null)
            {
                foreach (PluginServices.AvailablePlugin plugin in plugins)
                {
                    IOMLMetadataPlugin foundPlugin = (IOMLMetadataPlugin)PluginServices.CreateInstance(plugin);
                    if (foundPlugin.PluginName.Equals("DVDProfiler", StringComparison.OrdinalIgnoreCase))
                    {
                        metaDataPlugin = foundPlugin;
                        metaDataPlugin.SetOptionValue("Collection Path", "c:\\Collection.xml");
                        metaDataPlugin.Initialize(null);
                    }
                }
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
            bool updated = false;
            IEnumerable<string> media = FileScanner.GetAllMediaFromPath(directories);
            IEnumerable<string> uniqueMedia = TitleCollectionManager.GetUniquePaths(media);

            foreach (string path in uniqueMedia)
            {
                Title title = GetTitleMetaData(FileHelper.GetNameFromPath(path));

                title.AddDisk(new Disk(DEFAULT_DISK_NAME, path, FileScanner.GetVideoFormatFromPath(path)));

                // save the title to the database
                TitleCollectionManager.AddTitle(title);

                // resize the images
                OMLPlugin.BuildResizedMenuImage(title);

                updated = true;
            }

            // save all the image updates
            if (updated)
                TitleCollectionManager.SaveTitleUpdates();
        }

        /// <summary>
        /// Sets up file watchers for the given folders.  When there's a change it'll scan all the
        /// folders for new items and add them to OML
        /// </summary>
        /// <param name="folders"></param>
        public void WatchFolders(IList<string> folders)
        {
            watchFolders = folders;
            watchers = new List<FileSystemWatcher>(folders.Count);

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

            timer = new System.Timers.Timer(CHANGE_TIMER_TIMEOUT);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            // immediately scan when the scanner starts
            timer_Elapsed(null, null);
        }        

        /// <summary>
        /// if the filesystem watcher throws an error - we mostly care about it throwing when 
        /// it can't see a network share anymore.  in that case we re-create the file watcher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_Error(object sender, ErrorEventArgs e)
        {
            // if we can't find the drive anymore setup a new watcher
            if (e.GetException().Message == "The specified network name is no longer available")
                sender = new FileSystemWatcher(((FileSystemWatcher)sender).Path);
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
            timer.Start();            
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
                timer.Start();
            }
        }
    }
}
