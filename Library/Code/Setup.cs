using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Data;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using OMLEngine;
using System.Diagnostics;
using OMLSDK;

namespace Library
{
    public class Setup : ModelItem
    {
        private static Setup current;
        private Choice _ImporterSelection = null;
        private TreeView _treeView = null;
        private BooleanChoice _shouldCopyImages = new BooleanChoice();

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
            OMLApplication.DebugLine("Adding node: " + node.Title);
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
            List<string> _Importers = new List<string>();
            _Importers.Add("MyMovies");
            _Importers.Add("DVD Profiler");
            _Importers.Add("Movie Collectorz");
            _Importers.Add("DVR-MS Files");
            _Importers.Add("DVDID XML Directory Scanner");

            ImporterSelection.Options = _Importers;

            /*
            */
        }

        public Choice ImporterSelection
        {
            get
            {
                if (_ImporterSelection == null)
                {
                    _ImporterSelection = new Choice();
                    List<string> _items = new List<string>();
                    _items.Add("MyMovies");
                    _items.Add("DVD Profiler");
                    _items.Add("Movie Collectorz");
                    _items.Add("DVR-MS Files");

                    _ImporterSelection.Options = _items;
                    _ImporterSelection.ChosenChanged += new EventHandler(_ImporterSelection_ChosenChanged);
                }
                FirePropertyChanged("ImporterSelection");
                return _ImporterSelection;
            }
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
    }
}
