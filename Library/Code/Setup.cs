using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Data;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using OMLEngine;
using OMLSDK;

namespace Library
{
    public class Setup : BaseModelItem
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
        private IList<Title> _titles;
        private bool _loadStarted = false;
        private bool _loadComplete = false;
        private bool _addingAllStarted = false;
        private bool _addingAllComplete = false;
        private bool _hasCheckedNodes = false;
        private string _ImporterDescription = string.Empty;

        private BooleanChoice _shouldCopyImages = new BooleanChoice();
        private static List<OMLPlugin> availablePlugins = new List<OMLPlugin>();
        private TitleCollection _titleCollection = new TitleCollection();
        #endregion

        #region Properties

        public string ImporterDescription
        {
            get { return _ImporterDescription; }
            set
            {
                _ImporterDescription = value;
                FirePropertyChanged("ImporterDescription");
            }
        }

        public bool HasCheckedNodes
        {
            get { return _hasCheckedNodes; }
            set
            {
                _hasCheckedNodes = value;
                FirePropertyChanged("HasCheckedNodes");
            }
        }
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
                //_titleCollection.saveTitleCollection();
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

        public bool IsSingleFileImporter
        {
            get { return GetPlugin().IsSingleFileImporter(); }
        }

        public string DefaultFileToImport
        {
            get { return GetPlugin().DefaultFileToImport(); }
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
            if (_titleCollection.ContainsDisks(CurrentTitle.Disks))
            {
                OMLApplication.DebugLine("[Setup UI] Skipping title: " + CurrentTitle.Name + " because already in the collection");
                AddInHost.Current.MediaCenterEnvironment.Dialog(CurrentTitle.Name + " was found to already exist in your database and has been skipped.",
                                                                "Skipped Title",
                                                                DialogButtons.Ok,
                                                                2,
                                                                false);
                TotalTitlesSkipped++;
            }
            else
            {
                OMLApplication.DebugLine("[Setup UI] Adding title: " + CurrentTitle.Id);
                OMLPlugin.BuildResizedMenuImage(CurrentTitle);
                _titleCollection.Add(CurrentTitle);
                TotalTitlesAdded++;
            }
            CurrentTitleIndex++;

            if (TotalTitlesFound > CurrentTitleIndex)
            {
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
            //TitleCollection tc = OMLApplication.Current.ReloadTitleCollection();
            OMLApplication.Current.GoToMenu(new MovieGallery());
        }
        public void AddAllCurrentTitles()
        {
            OMLApplication.DebugLine("[Setup] Starting deferred all titles import");
            Application.DeferredInvokeOnWorkerThread(delegate
            {
                OMLApplication.DebugLine("[Setup] AddingAllCurrentTitles Started");
                AddingAllStarted = true;
                for (CurrentTitleIndex = CurrentTitleIndex; TotalTitlesFound > CurrentTitleIndex; CurrentTitleIndex++)
                {
                    CurrentTitle = _titles[CurrentTitleIndex];
                    if (_titleCollection.ContainsDisks(CurrentTitle.Disks))
                    {
                        OMLApplication.DebugLine("[Setup UI] Skipping title: " + CurrentTitle.Name + " because already in the collection");
                        TotalTitlesSkipped++;
                    }
                    else
                    {
                        OMLApplication.DebugLine("[Setup UI] Adding title: " + CurrentTitle.Id);
                        OMLPlugin.BuildResizedMenuImage(CurrentTitle);
                        _titleCollection.Add(CurrentTitle);
                        TotalTitlesAdded++;
                    }
                }
            }, delegate
            {
                OMLApplication.DebugLine("[Setup] AddingAllCurrentTitles Completed");
                AddingAllComplete = true;
                AllTitlesProcessed = true;
                FirePropertyChanged("TotalTitlesAdded");
            }, null);
        }

        public void BeginLoading()
        {
            OMLApplication.DebugLine("Start loading new titles");
            plugin = GetPlugin();

            Application.DeferredInvokeOnWorkerThread(delegate
            {
                OMLApplication.DebugLine("[Setup UI] _BeginLoading called");
                LoadStarted = true;
                try
                {
                    for (int i = 0; i <= _treeView.CheckedNodes.Count - 1; i++)
                    {
                        OMLApplication.DebugLine("[Setup UI] Found a node");
                        TreeNode node = (TreeNode)_treeView.CheckedNodes[i];
                        if (node != null)
                        {
                            OMLApplication.DebugLine("[Setup UI] Scanning node: " + node.FullPath);
                            if (plugin.IsSingleFileImporter())
                            {
                                string fileNameToFind = plugin.DefaultFileToImport();
                                if (fileNameToFind != null)
                                {
                                    OMLApplication.DebugLine("[Setup UI] Looking for file: " + fileNameToFind);
                                    if (Directory.Exists(node.FullPath))
                                    {
                                        DirectoryInfo dirInfo = new DirectoryInfo(node.FullPath);
                                        if (dirInfo != null)
                                        {
                                            FileInfo[] fileInfos = dirInfo.GetFiles(fileNameToFind, SearchOption.TopDirectoryOnly);
                                            foreach (FileInfo fInfo in fileInfos)
                                            {
                                                OMLApplication.DebugLine("[Setup UI] File Found: " + fInfo.Name);
                                                plugin.CopyImages = ShouldCopyImages.Value;
                                                plugin.DoWork(new string[] { fInfo.FullName });
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                OMLApplication.DebugLine("[Setup UI] Processing path: " + node.FullPath);
                                plugin.CopyImages = ShouldCopyImages.Value;
                                plugin.DoWork(new string[] { node.FullPath });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    OMLApplication.DebugLine("[Setup UI] Error finding file: " + e.Message);
                }
            },
            delegate
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
            }, null);
        }

        public void NoTitlesFoundNotice()
        {
            OMLApplication.Current.MediaCenterEnvironment.Dialog("No titles were found, would you like to try again?",
                 "Notice",
                 new object[] { DialogButtons.Yes, DialogButtons.No, },
                 0, true, null,
                 delegate(DialogResult result)
                 {
                     OMLApplication.DebugLine("[Setup UI] _NoTitlesFoundNoticeCallback called");
                     if (result.CompareTo(DialogButtons.Yes) == 0)
                     {
                         OMLApplication.Current.MediaCenterEnvironment.Dialog("This functionality has not been completed",
                            "Notice", DialogButtons.Ok, 10, false);
                     }
                 });
        }

        public void AddCheckedNode(TreeNode node)
        {
            OMLApplication.DebugLine("Adding node: " + node.FullPath);
            TreeView.CheckedNodes.Add(node);
            FirePropertyChanged("CheckedNodes");
            HasCheckedNodes = true;
        }

        public void RemoveCheckedNode(TreeNode node)
        {
            OMLApplication.DebugLine("Removing node: " + node.Title);
            if (TreeView.CheckedNodes.Contains(node))
            {
                TreeView.CheckedNodes.Remove(node);
                FirePropertyChanged("CheckedNodes");
            }
            if (TreeView.CheckedNodes.Count > 0)
                HasCheckedNodes = false;
        }

        public Setup()
        {
            LoadPlugins();
            AllTitlesProcessed = false;
            CurrentTitle = null;
            CurrentTitleIndex = 0;
            current = this;
            _titleCollection.loadTitleCollection();
            _ImporterSelection = new Choice();
            List<string> _Importers = new List<string>();
            foreach (OMLPlugin _plugin in availablePlugins) {
                OMLApplication.DebugLine("[Setup] Adding " + _plugin.Name + " to the list of Importers");
                _Importers.Add(_plugin.Description);
            }

            _ImporterSelection.Options = _Importers;
            _ImporterSelection.ChosenChanged += delegate(object sender, EventArgs e)
            {
                OMLApplication.ExecuteSafe(delegate
                {
                    Choice c = (Choice)sender;
                    ImporterDescription = @"Notice: " + GetPlugin().SetupDescription();
                    OMLApplication.DebugLine("Item Chosed: " + c.Options[c.ChosenIndex]);
                });
            };

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
                if (availablePlugins[i].Description.CompareTo(strChosenImporter) == 0)
                    return availablePlugins[i];
            }
            return null;
        }

        private static void LoadPlugins()
        {
            if (availablePlugins.Count == 0)
            {
                List<PluginServices.AvailablePlugin> Pluginz = new List<PluginServices.AvailablePlugin>();
                string path = Path.GetDirectoryName(FileSystemWalker.PluginsDirectory + @"\\Plugins");
                OMLApplication.DebugLine("Path is: " + path);
                Pluginz = PluginServices.FindPlugins(path, "OMLSDK.IOMLPlugin");
                OMLPlugin objPlugin;
                // Loop through available plugins, creating instances and adding them
                foreach (PluginServices.AvailablePlugin oPlugin in Pluginz)
                {
                    objPlugin = (OMLPlugin)PluginServices.CreateInstance(oPlugin);
                    availablePlugins.Add(objPlugin);
                }
                Pluginz = null;
            }
        }

        public void RequestNodeSelection()
        {
            AddInHost.Current.MediaCenterEnvironment.Dialog("Please select one or more directories before clicking begin scan.",
                                                            "Missing Selection",
                                                            Microsoft.MediaCenter.DialogButtons.Ok,
                                                            0,
                                                            true);
        }
    }
}
