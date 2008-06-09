using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library
{
    public class TreeNodeEventArgs : EventArgs
    {
        private TreeNode _node = null;

        public TreeNodeEventArgs(TreeNode node)
        {
            OMLApplication.DebugLine("Checked Node Changed: " + node.Title);
            _node = node;
        }

        public TreeNode Node
        {
            get { return _node; }
            set { _node = value; }
        }
    }
}
