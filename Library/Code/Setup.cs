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
using DVRMSPlugin;
using MovieCollectorzPlugin;
using DVDProfilerPlugin;
using MyMoviesPlugin;
using VMCDVDLibraryPlugin;

namespace Library
{
    public class Setup : ModelItem
    {
        private string _filename = string.Empty;
        private OMLPlugin _plugin;
        private int _currentTitleIndex;
        private string _currentTitleName = string.Empty;
        private Image _currentTitleImage = null;
        private int _TotalTitlesAdded = 0;
        private int _TotalTitlesFound = 0;
        private int _TotalTitlesSkipped = 0;
        private static Setup current;
        private Choice _ImporterSelection = null;
        private TreeView _treeView = null;
        private bool _isDirty = false;
        private OMLPlugin plugin = null;
        private string file_to_import = string.Empty;
        private List<Title> _titles;
        private bool _loadStarted = false;
        private bool _loadComplete = false;

        private BooleanChoice _shouldCopyImages = new BooleanChoice();
        private TitleCollection _titleCollection = new TitleCollection();

        public Image DefaultImage
        {
            get
            {
                return MovieItem.NoCoverImage;
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

        public Image CurrentTitleImage
        {
            get { return _currentTitleImage; }
            set
            {
                _currentTitleImage = value;
                FirePropertyChanged("CurrentTitleImage");
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
            get { return _currentTitleName; }
            set
            {
                _currentTitleName = value;
                FirePropertyChanged("CurrentTitleName");
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

        public void SkipCurrentTitle()
        {
            TotalTitlesSkipped = TotalTitlesSkipped + 1;
            if (TotalTitlesFound > CurrentTitleIndex + 1)
            {
                CurrentTitleIndex = CurrentTitleIndex + 1;
                CurrentTitleName = _titles[CurrentTitleIndex].Name;
                CurrentTitleImage = MovieItem.LoadImage(_titles[CurrentTitleIndex].FrontCoverPath);
            }
            else
            {
                CurrentTitleName = "No more Titles";
                CurrentTitleImage = MovieItem.NoCoverImage;
            }
        }

        public void AddCurrentTitle()
        {
            TotalTitlesAdded = TotalTitlesAdded + 1;
            OMLApplication.DebugLine("[Setup UI] Adding title: " + _titles[CurrentTitleIndex].InternalItemID);
            _titleCollection.Add(_titles[CurrentTitleIndex]);
            if (TotalTitlesFound > CurrentTitleIndex + 1)
            {
                CurrentTitleIndex = CurrentTitleIndex + 1;
                CurrentTitleName = _titles[CurrentTitleIndex].Name;
                CurrentTitleImage = MovieItem.LoadImage(_titles[CurrentTitleIndex].FrontCoverPath);
            }
            else
            {
                CurrentTitleName = "No more Titles";
                CurrentTitleImage = MovieItem.NoCoverImage;
            }
        }

        public void AddAllTitles()
        {
            for (CurrentTitleIndex = CurrentTitleIndex; CurrentTitleIndex < TotalTitlesFound - 1; CurrentTitleIndex = CurrentTitleIndex + 1)
            {
                TotalTitlesAdded = TotalTitlesAdded + 1;
                _titleCollection.Add(_titles[CurrentTitleIndex]);
            }
            CurrentTitleName = "No more Titles";
            CurrentTitleImage = MovieItem.NoCoverImage;
        }

        public void BeginLoading()
        {
            OMLApplication.DebugLine("Start loading new titles");
            _plugin = GetPlugin();
            _filename = "C:\\titles.xml";

            Application.DeferredInvokeOnWorkerThread(new DeferredHandler(_BeginLoading),
                                                     new DeferredHandler(_LoadingComplete),
                                                     new object[] { });
        }

        public void _LoadingComplete(object args)
        {
            OMLApplication.DebugLine("[Setup UI] _LoadingComplete called");
            LoadComplete = true;
            _titles = _plugin.GetTitles();

            if (_titles.Count > 0)
            {
                TotalTitlesFound = _titles.Count;
                CurrentTitleIndex = 0;
                CurrentTitleName = _titles[CurrentTitleIndex].Name;
                CurrentTitleImage = MovieItem.LoadImage(_titles[CurrentTitleIndex].FrontCoverPath);
            }
        }

        public void _BeginLoading(object args)
        {
            OMLApplication.DebugLine("[Setup UI] _BeginLoading called");
            LoadStarted = true;
            try
            {
                if (File.Exists(_filename))
                {
                    ImportFile(_plugin, _filename);
                }
            }
            catch (Exception e)
            {
                OMLApplication.DebugLine("Error finding file");
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
            current = this;
            _titleCollection.loadTitleCollection();
            _ImporterSelection = new Choice();
            List<string> _Importers = new List<string>();
            _Importers.Add("DVDID XML Files");
            _Importers.Add("MyMovies");
            _Importers.Add("DVD Profiler");
            _Importers.Add("Movie Collectorz");
            _Importers.Add("DVR-MS Files");

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

        public void TreeView_OnCheckedNodeChanged(object sender, TreeNodeEventArgs e)
        {
            OMLApplication.DebugLine("CheckedNodeChanged: " + e.Node.Title);
            //Checked.Value = (e.Node == this);
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
                        if (dInfo.DriveType == DriveType.Fixed)
                        {
                            DirectoryTreeNode node = new DirectoryTreeNode(dInfo.Name + " (" + dInfo.VolumeLabel + ")",
                                                                           dInfo.RootDirectory.FullName,
                                                                           _treeView);
                            _treeView.ChildNodes.Add(node);
                            TreeView.CheckedNodeChanged +=
                                new EventHandler<TreeNodeEventArgs>(TreeView_OnCheckedNodeChanged);
                        }
                    }
                }

                return _treeView;
            }
            set { _treeView = value; }
        }

        public bool ImportFile(OMLPlugin plugin, string file_to_import)
        {
            return plugin.Load(file_to_import, _shouldCopyImages.Value);
        }

        public void LoadTitlesIntoDatabase(OMLPlugin plugin)
        {
            List<Title> titles = plugin.GetTitles();

            foreach (Title t in titles)
            {
                CurrentTitleName = t.Name;
                CurrentTitleImage = GalleryItem.LoadImage(t.FrontCoverPath);
                _titleCollection.Add(t);
                TotalTitlesAdded = TotalTitlesAdded + 1;
            }
        }

        public OMLPlugin GetPlugin()
        {
            string strChosenImporter = (string)Setup.Current._ImporterSelection.Chosen;
            OMLApplication.DebugLine("Chosen Importer is: " + strChosenImporter);
            switch (strChosenImporter)
            {
                case "DVDID XML Files":
                    return new MyMoviesImporter();
                    break;
                case "MyMovies":
                    return new MyMoviesImporter();
                    break;
                case "DVD Profiler":
                    return new MyMoviesImporter();
                    break;
                case "Movie Collectorz":
                    return new MyMoviesImporter();
                    break;
                case "DVR-MS Files":
                    return new MyMoviesImporter();
                    break;
                default:
                    return new MyMoviesImporter();
                    break;
            }
        }
    }
}
