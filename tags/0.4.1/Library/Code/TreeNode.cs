using System;
using System.Diagnostics;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class TreeNode : BaseModelItem
    {
        private string _fullPath = string.Empty;
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
            set
            {
                _title = value;
                FirePropertyChanged("Title");
            }
        }
        public String FullPath
        {
            get { return _fullPath; }
            set
            {
                _fullPath = value;
                FirePropertyChanged("FullPath");
            }
        }
        public bool HasChildNodes
        {
            get { return _hasChildNodes; }
            set { _hasChildNodes = value; }
        }
        public BooleanChoice Checked
        {
            get { return _checked; }
            set
            {
                BooleanChoice is_checked = value;
                OMLApplication.DebugLine("Checked called: " + is_checked);
                if (is_checked.Selected)
                    _treeView.CheckedNodes.Add(this);
                else
                    _treeView.CheckedNodes.Remove(this);

                _checked = value;
            }
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

        public TreeNode(String title, String fullPath)
        {
            Title = title;
            FullPath = fullPath;
            Checked.ChosenChanged += delegate(object sender, EventArgs e)
            {
                OMLApplication.ExecuteSafe(delegate
                {
                    // this is the one that works
                    OMLApplication.DebugLine("Changed: " + this.GetType().ToString());
                    Setup.Current.AddCheckedNode(this);
                });
            };
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
}