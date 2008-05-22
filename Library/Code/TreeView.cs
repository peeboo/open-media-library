using System;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;
using System.Collections;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Library
{
    delegate void AsyncChildNodeRequest(int level);

    public class TreeView : ModelItem
    {
        private ArrayListDataSet _childNodes = new ArrayListDataSet();
        private TreeNode _checkedNode = null;
        private ArrayListDataSet _checkedNodes = new ArrayListDataSet();

        public event EventHandler<TreeNodeEventArgs> CheckedNodeChanged;

        public TreeNode CheckedNode
        {
            get
            {
                OMLApplication.DebugLine("Get called on TreeView:CheckedNode");
                if (_checkedNode == null && ChildNodes.Count > 0)
                    _checkedNode = ChildNodes[0] as TreeNode;

                _checkedNodes.Add(_checkedNode);
                OMLApplication.DebugLine("Total Checked Nodes: " + _checkedNodes.Count);
                return _checkedNode;
            }

            set
            {
                OMLApplication.DebugLine("Set called on TreeView:CheckedNode");
                _checkedNodes.Add(value);
                FirePropertyChanged("CheckedNode");
                if (CheckedNodeChanged != null)
                {
                    TreeNodeEventArgs e = new TreeNodeEventArgs(CheckedNode);
                    CheckedNodeChanged(this, e);
                }
            }
        }

        public ArrayListDataSet CheckedNodes
        {
            get { return _checkedNodes; }
        }

        public ArrayListDataSet ChildNodes
        {
            get { return _childNodes; }
            set { _childNodes = value; }
        }

    }

    public class TreeNodeEventArgs : EventArgs
    {
        private TreeNode _node = null;

        public TreeNodeEventArgs(TreeNode node)
        {
            _node = node;
        }

        public TreeNode Node
        {
            get { return _node; }
            set { _node = value; }
        }

    }
}
