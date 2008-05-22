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

namespace Library
{
    public class Setup : ModelItem
    {
        private static Setup current;
        private Choice _ImporterSelection = new Choice();
        private TreeView _treeView = new TreeView();

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

            _ImporterSelection.Options = _Importers;

            foreach (DriveInfo dInfo in DriveInfo.GetDrives())
            {
                if (dInfo.DriveType == DriveType.Fixed || dInfo.DriveType == DriveType.Network)
                {
                    DirectoryTreeNode node = new DirectoryTreeNode(dInfo.Name + " (" + dInfo.VolumeLabel + ")",
                                                                   dInfo.RootDirectory.FullName,
                                                                   TreeView);
                    TreeView.ChildNodes.Add(node);
                }
            }
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

                    _ImporterSelection.Options = _items;
                }
                return _ImporterSelection;
            }
            set
            {
                _ImporterSelection = value;
            }
        }

        public TreeView TreeView
        {
            get { return _treeView; }
            set { _treeView = value; }
        }
    }
}
