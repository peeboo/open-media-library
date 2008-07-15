using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Data;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using OMLEngine;
using OMLSDK;

namespace Library
{
    public class Setup : ModelItem
    {
        #region variables
        private Title _currentTitle;
        private string _filename = string.Empty;
        private int _currentTitleIndex;
        private int _TotalTitlesAdded = 0;
        private int _TotalTitlesFound = 0;
        private int _TotalTitlesSkipped = 0;
        private bool _AllTitlesProcessed = false;
        private static Setup current;
        private Choice _ImporterSelection = null;
        private TreeView _treeView = null;
        private bool _isDirty = false;
        private OMLPlugin plugin = null;
        private string file_to_import = string.Empty;
        private List<Title> _titles;
        private bool _loadStarted = false;
        private bool _loadComplete = false;
        private bool _addingAllStarted = false;
        private bool _addingAllComplete = false;

        private BooleanChoice _shouldCopyImages = new BooleanChoice();
        private static List<OMLPlugin> availablePlugins = new List<OMLPlugin>();
        private TitleCollection _titleCollection = new TitleCollection();
        #endregion

        #region Properties
        public ArrayListDataSet CheckedNodes
        {
            get { return _treeView.CheckedNodes; }
        }
        public Image DefaultImage
        {
            get
            {
                return MovieItem.NoCoverImage;
            }
        }
        public bool AddingAllStarted
        {
            get { return _addingAllStarted; }
            set
            {
                _addingAllStarted = value;
                FirePropertyChanged("AddingAllStarted");
            }
        }
        public bool AddingAllComplete
        {
            get { return _addingAllComplete; }
            set
            {
                _addingAllComplete = value;
                FirePropertyChanged("AddingAllComplete");
            }
        }
        public bool LoadStarted
        {
            get { return _loadStarted; }
            set
            {
                _loadStarted = value;
                FirePropertyChanged("LoadStarted");
            }
        }
        public bool LoadComplete
        {
            get { return _loadComplete; }
            set
            {
                _loadComplete = value;
                FirePropertyChanged("LoadComplete");
            }
        }
        public Title CurrentTitle
        {
            get { return _currentTitle; }
            set
            {
                _currentTitle = value;
                FirePropertyChanged("CurrentTitleName");
                FirePropertyChanged("CurrentTitleVideoFormat");
                FirePropertyChanged("CurrentTitleImage");
                FirePropertyChanged("CurrentTitleReleaseYear");
                FirePropertyChanged("CurrentTitleRuntime");
                FirePropertyChanged("CurrentTitleRating");
            }
        }
        public string CurrentTitleRating
        {
            get
            {
                if (CurrentTitle != null)
                    return CurrentTitle.ParentalRating;

                return string.Empty;
            }
        }
        public string CurrentTitleRuntime
        {
            get
            {
                if (CurrentTitle != null)
                    return CurrentTitle.Runtime.ToString();

                return string.Empty;
            }
        }
        public string CurrentTitleVideoFormat
        {
            get
            {
                if (CurrentTitle != null)
                    if (CurrentTitle.Disks.Count > 0)
                        return Enum.GetName(typeof(VideoFormat), _currentTitle.Disks[0].Format); //again should be per disk in future

                return string.Empty;
            }
        }
        public string CurrentTitleReleaseYear
        {
            get
            {
                if (CurrentTitle != null)
                    return CurrentTitle.ReleaseDate.ToLongDateString();

                return string.Empty;
            }
        }
        public Image CurrentTitleImage
        {
            get
            {
                if (CurrentTitle != null)
                    return MovieItem.LoadImage(CurrentTitle.FrontCoverPath);

                return MovieItem.NoCoverImage;
            }
        }
        public int CurrentTitleIndex
        {
            get { return _currentTitleIndex; }
            set
            {
                _currentTitleIndex = value;
                FirePropertyChanged("CurrentTitleIndex");
            }
        }
        public bool AllTitlesProcessed
        {
            get { return _AllTitlesProcessed; }
            set
            {
                _AllTitlesProcessed = true;
                _titleCollection.saveTitleCollection();
                FirePropertyChanged("AllTitlesProcessed");
            }
        }
        public int TotalTitlesSkipped
        {
            get { return _TotalTitlesSkipped; }
            set
            {
                _TotalTitlesSkipped = value;
                FirePropertyChanged("TotalTitlesSkipped");
            }
        }
        public int TotalTitlesAdded
        {
            get { return _TotalTitlesAdded; }
            set
            {
                _TotalTitlesAdded = value;
                FirePropertyChanged("TotalTitlesAdded");
            }
        }
        public string CurrentTitleName
        {
            get
            {
                if (CurrentTitle != null)
                    return _currentTitle.Name;

                return string.Empty;
            }
        }
        public int TotalTitlesFound
        {
            get { return _TotalTitlesFound; }
            set
            {
                _TotalTitlesFound = value;
                FirePropertyChanged("TotalTitlesFound");
            }
        }
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                FirePropertyChanged("IsDirty");
            }
        }
        public BooleanChoice ShouldCopyImages
        {
            get { return _shouldCopyImages; }
            set
            {
                _shouldCopyImages = value;
                FirePropertyChanged("ShouldCopyImages");
            }
        }
        public static Setup Current
        {
            get { return current; }
            set { current = value; }
        }
        #endregion

        public void SkipCurrentTitle()
        {
            TotalTitlesSkipped++;
            if (TotalTitlesFound > CurrentTitleIndex + 1)
            {
                CurrentTitleIndex++;
                CurrentTitle = _titles[CurrentTitleIndex];
            }
            else
            {
                AllTitlesProcessed = true;
            }
        }

        public void AddCurrentTitle()
        {
            TotalTitlesAdded++;
            OMLApplication.DebugLine("[Setup UI] Adding title: " + CurrentTitle.InternalItemID);
            OMLPlugin.BuildResizedMenuImage(CurrentTitle);
            _titleCollection.Add(CurrentTitle);
            if (TotalTitlesFound > CurrentTitleIndex + 1)
            {
                CurrentTitleIndex++;
                CurrentTitle = _titles[CurrentTitleIndex];
            }
            else
            {
                AllTitlesProcessed = true;
            }
        }

        public void Reset()
        {
            OMLApplication.DebugLine("Resetting the Setup object");
            _AllTitlesProcessed = false;
            _currentTitle = null;
            _currentTitleIndex = 0;
            _filename = string.Empty;
            _loadComplete = false;
            _loadStarted = false;
            _titles = null;
            _titleCollection.loadTitleCollection();
            _treeView.CheckedNodes.Clear();
        }

        public void gotoMenu()
        {
            TitleCollection tc = OMLApplication.Current.ReloadTitleCollection();
            OMLApplication.Current.GoToMenu(new MovieGallery(tc, Filter.Home));
        }

        public void AddAllCurrentTitles()
        {
            OMLApplication.DebugLine("[Setup] Starting deferred all titles import");
            Application.DeferredInvokeOnWorkerThread(new DeferredHandler(_AddAllCurrentTitles),
                                                     new DeferredHandler(_DoneAddingAllCurrentTitles),
                                                     new object[] { });
        }

        public void _AddAllCurrentTitles(object args)
        {
            OMLApplication.DebugLine("[Setup] AddingAllCurrentTitles Started");
            AddingAllStarted = true;
            OMLApplication.DebugLine("[Setup UI] Adding title: " + CurrentTitle.InternalItemID);
            _titleCollection.Add(CurrentTitle);
            while (TotalTitlesFound > CurrentTitleIndex + 1)
            {
                _currentTitle = _titles[CurrentTitleIndex];
                _TotalTitlesAdded++;
                OMLApplication.DebugLine("[Setup] Adding title: " + _currentTitle.Name);
                OMLPlugin.BuildResizedMenuImage(_currentTitle);
                _titleCollection.Add(_currentTitle);
                _currentTitle = _titles[++CurrentTitleIndex];
            }
        }

        public void _DoneAddingAllCurrentTitles(object args)
        {
            OMLApplication.DebugLine("[Setup] AddingAllCurrentTitles Completed");
            AddingAllComplete = true;
            AllTitlesProcessed = true;
            FirePropertyChanged("TotalTitlesAdded");
        }

        public void BeginLoading()
        {
            OMLApplication.DebugLine("Start loading new titles");
            plugin = GetPlugin();

            foreach (TreeNode node in _treeView.CheckedNodes)
            {
                plugin.ProcessDir(node.FullPath);
            }

            if (plugin.IsSingleFileImporter()) {
                OMLApplication.DebugLine("This importer requires a file, determining file to load.");
                _filename = determineFileToLoad(plugin);
            }
            //_filename = "C:\\titles.xml";

            Application.DeferredInvokeOnWorkerThread(new DeferredHandler(_BeginLoading),
                                                     new DeferredHandler(_LoadingComplete),
                                                     new object[] { });
        }

        public string determineFileToLoad(OMLPlugin plugin)
        {
            OMLApplication.DebugLine("Which file to load?");
            return string.Empty;
        }

        public void _LoadingComplete(object args)
        {
            OMLApplication.DebugLine("[Setup UI] _LoadingComplete called");
            LoadComplete = true;
            _titles = plugin.GetTitles();

            if (_titles.Count > 0)
            {
                TotalTitlesFound = _titles.Count;
                CurrentTitleIndex = 0;
                CurrentTitle = _titles[CurrentTitleIndex];
            }
        }

        public void _BeginLoading(object args)
        {
            OMLApplication.DebugLine("[Setup UI] _BeginLoading called");
            LoadStarted = true;
            try
            {
                plugin.DoWork(plugin.GetWork());
            }
            catch (Exception e)
            {
                OMLApplication.DebugLine("Error finding file: " + e.Message);
            }
        }

        public void AddCheckedNode(TreeNode node)
        {
            OMLApplication.DebugLine("Adding node: " + node.FullPath);
            TreeView.CheckedNodes.Add(node);
            FirePropertyChanged("CheckedNodes");
        }

        public void RemoveCheckedNode(TreeNode node)
        {
            OMLApplication.DebugLine("Removing node: " + node.Title);
            if (TreeView.CheckedNodes.Contains(node))
            {
                TreeView.CheckedNodes.Remove(node);
                FirePropertyChanged("CheckedNodes");
            }
        }

        public Setup()
        {
            LoadPlugins();
            current = this;
            _titleCollection.loadTitleCollection();
            _ImporterSelection = new Choice();
            List<string> _Importers = new List<string>();
            foreach (OMLPlugin _plugin in availablePlugins) {
                _Importers.Add(_plugin.Name);
            }

            _ImporterSelection.Options = _Importers;
            _ImporterSelection.ChosenChanged += new EventHandler(_ImporterSelection_ChosenChanged);
        }

        public Choice ImporterSelection
        {
            get { return _ImporterSelection; }
            set
            {
                _ImporterSelection = value;
                FirePropertyChanged("ImporterSelection");
            }
        }

        public void _ImporterSelection_ChosenChanged(object sender, EventArgs e)
        {
            Choice c = (Choice)sender;
            OMLApplication.DebugLine("Item Chosed: " + c.Options[c.ChosenIndex]);

        }

        public TreeView TreeView
        {
            get
            {
                if (_treeView == null)
                {
                    _treeView = new TreeView();
                    foreach (DriveInfo dInfo in DriveInfo.GetDrives())
                    {
                        if (dInfo.DriveType == DriveType.Fixed || dInfo.DriveType == DriveType.Network)
                        {
                            if (dInfo.IsReady)
                            {
                                DirectoryTreeNode node = new DirectoryTreeNode(dInfo.Name + " (" + dInfo.VolumeLabel + ")",
                                                                               dInfo.RootDirectory.FullName,
                                                                               _treeView);
                                _treeView.ChildNodes.Add(node);
                            }
                        }
                    }
                }

                return _treeView;
            }
            set { _treeView = value; }
        }

        public OMLPlugin GetPlugin()
        {
            string strChosenImporter = (string)Setup.Current._ImporterSelection.Chosen;
            OMLApplication.DebugLine("Chosen Importer is: " + strChosenImporter);
            for (int i = 0; i < availablePlugins.Count; i++)
            {
                if (availablePlugins[i].Name.CompareTo(strChosenImporter) == 0)
                    return availablePlugins[i];
            }
            return null;
        }

        private static void LoadPlugins()
        {
            List<PluginServices.AvailablePlugin> Pluginz = new List<PluginServices.AvailablePlugin>();
            string path = Path.GetDirectoryName(FileSystemWalker.PluginsDirectory + @"\\Plugins");
            OMLApplication.DebugLine("Path is: " + path);
            Pluginz = PluginServices.FindPlugins(path, "OMLSDK.IOMLPlugin");
            OMLPlugin objPlugin;
            // Loop through available plugins, creating instances and adding them
            foreach (PluginServices.AvailablePlugin oPlugin in Pluginz)
            {
                objPlugin = (OMLPlugin) PluginServices.CreateInstance(oPlugin);
                availablePlugins.Add(objPlugin);
            }
            Pluginz = null;
        }
    }
}
