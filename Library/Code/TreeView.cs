using System;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;
using System.Collections;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Library
{
    public class TreeNodeEventArgs : EventArgs
    {
        public TreeNodeEventArgs(TreeNode node)
        {
            Node = node;
        }

        public TreeNode Node
        {
            get { return _node; }
            set { _node = value; }
        }

        #region Fields

        private TreeNode _node = null;

        #endregion

    }

    delegate void AsyncChildNodeRequest(int level);

    public class DirectoryTreeNode : TreeNode
    {
        public string FullPath
        {
            get { return _fullPath; }
            set
            {
                _fullPath = value;
                HasChildNodes = (Directory.Exists(_fullPath) &&
                                 Directory.GetDirectories(_fullPath).Length > 0);
            }
        }

        public DirectoryTreeNode(String title, String fullPath, TreeView treeView)
            : base(title)
        {
            FullPath = fullPath;
            TreeView = treeView;
            TreeView.CheckedNodeChanged +=
                new EventHandler<TreeNodeEventArgs>(TreeView_OnCheckedNodeChanged);
        }

        public override void GetChildNodes()
        {
            if (!String.IsNullOrEmpty(FullPath))
            {
                ChildNodes.Clear();
                string[] directories = Directory.GetDirectories(FullPath);

                foreach (string directory in directories)
                {
                    try
                    {
                        DirectoryTreeNode node =
                            new DirectoryTreeNode(Path.GetFileName(directory), directory, TreeView);
                        node.Level = Level + 1;
                        node.TreeView = TreeView;
                        TreeView.CheckedNodeChanged +=
                            new EventHandler<TreeNodeEventArgs>(TreeView_OnCheckedNodeChanged);
                        node.HasChildNodes = (Directory.GetDirectories(node.FullPath).Length > 0);
                        ChildNodes.Add(node);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    catch (DriveNotFoundException ex)
                    {
                        Debug.WriteLine(ex);
                    }
                    catch (IOException ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                HasChildNodes = (ChildNodes.Count > 0);
            }

            base.GetChildNodes();
        }
        public override string ToString()
        {
            return FullPath;
        }
        private void TreeView_OnCheckedNodeChanged(object sender, TreeNodeEventArgs e)
        {
            Checked.Value = (e.Node == this);
        }

        private string _fullPath = String.Empty;
    }

    public class TreeNode : ModelItem
    {
        private string _title = string.Empty;
        private ArrayListDataSet _childNodes = new ArrayListDataSet();
        private bool _hasChildNodes = false;
        private int _level = 0;
        private BooleanChoice _checked = new BooleanChoice();
        private TreeView _treeView = null;

        public event EventHandler ChildNodesRequestCompleted;
        public event EventHandler Collapsed;
        public event EventHandler Expanded;

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        public ArrayListDataSet ChildNodes
        {
            get { return _childNodes; }
            set { _childNodes = value; }
        }
        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public bool HasChildNodes
        {
            get { return _hasChildNodes; }
            set { _hasChildNodes = value; }
        }
        public BooleanChoice Checked
        {
            get { return _checked; }
            set { _checked = value; }
        }
        public TreeView TreeView
        {
            get { return _treeView; }
            set { _treeView = value; }
        }
        public Inset LevelMargin
        {
            get { return new Inset((50 * Level), 0, 0, 0); }
        }

        public TreeNode(String title)
        {
            Title = title;
            Checked.ChosenChanged += new EventHandler(delegate(object sender, EventArgs e)
            {
                if (Checked.Value) TreeView.CheckedNode = this;
            });
        }

        public virtual void OnCollapsed()
        {
            if (Collapsed != null)
                Collapsed(this, EventArgs.Empty);
        }
        public virtual void OnExpanded()
        {
            if (Expanded != null)
                Expanded(this, EventArgs.Empty);
        }
        public virtual void GetChildNodes()
        {
            if (ChildNodesRequestCompleted != null)
                ChildNodesRequestCompleted(this, EventArgs.Empty);
        }

    }

    public class TreeView : ModelItem
    {
        private ArrayListDataSet _childNodes = new ArrayListDataSet();
        private TreeNode _checkedNode = null;

        public event EventHandler<TreeNodeEventArgs> CheckedNodeChanged;

        public TreeNode CheckedNode
        {
            get
            {
                if (_checkedNode == null && ChildNodes.Count > 0)
                    _checkedNode = ChildNodes[0] as TreeNode;

                return _checkedNode;
            }
            set
            {
                if (_checkedNode != value)
                {
                    _checkedNode = value;
                    FirePropertyChanged("CheckedNode");
                    if (CheckedNodeChanged != null)
                    {
                        TreeNodeEventArgs e = new TreeNodeEventArgs(CheckedNode);
                        CheckedNodeChanged(this, e);
                    }
                }
            }
        }
        public ArrayListDataSet ChildNodes
        {
            get { return _childNodes; }
            set { _childNodes = value; }
        }

    }
}
