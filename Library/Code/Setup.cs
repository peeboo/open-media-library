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
        private string _currentTitleName = string.Empty;
        private Image _currentTitleImage = null;
        private int _TotalTitlesAdded = 0;
        private int _TotalTitlesFound = 0;
        private static Setup current;
        private Choice _ImporterSelection = null;
        private TreeView _treeView = null;
        private bool _isDirty = false;
        private OMLPlugin plugin = null;
        private string file_to_import = string.Empty;

        private BooleanChoice _shouldCopyImages = new BooleanChoice();
        private TitleCollection _titleCollection = new TitleCollection();

        public Image CurrentTitleImage
        {
            get { return _currentTitleImage; }
            set
            {
                _currentTitleImage = value;
                FirePropertyChanged("CurrentTitleImage");
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
                FirePropertyChanged("TotalTItlesFound");
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

        public void ProcessFile(OMLPlugin plugin, string file_to_import)
        {
            try
            {
                if (File.Exists(file_to_import) || Directory.Exists(file_to_import))
                {
                    if (ImportFile(plugin, file_to_import))
                        LoadTitlesIntoDatabase(plugin);
                }
            }
            catch (Exception e)
            {
                OMLApplication.DebugLine("Error loading titles");
            }
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
    }
}
