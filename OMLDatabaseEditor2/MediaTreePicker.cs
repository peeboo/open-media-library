using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using OMLEngine;
using DevExpress.XtraEditors;

namespace OMLDatabaseEditor
{
    public partial class MediaTreePicker : DevExpress.XtraEditors.XtraForm
    {
        private Dictionary<int, TreeNode> _mediaTree;

        public int? SelectedTitleID { get; set; }
        public string SelectedTitleName { get; set; }

        
        public MediaTreePicker()
        {
            InitializeComponent();
            PopulateMediaTree();
        }

        private void sbOK_Click(object sender, EventArgs e)
        {
            ProcessSelectedNode();
            DialogResult = DialogResult.OK;
        }

        private void ProcessSelectedNode()
        {
            TreeNode sn = treeMedia.SelectedNode;

            if (treeMedia.SelectedNode.Name == "All Media")
            {
                // Root Level selected
                SelectedTitleID = null;
                SelectedTitleName = "All Media";
            }
            else
            {
                SelectedTitleID = Convert.ToInt32(treeMedia.SelectedNode.Name);
                SelectedTitleName = treeMedia.SelectedNode.Text;
            }
        }

        private void PopulateMediaTree()
        {
            Dictionary<int, Title> mediatreefolders = TitleCollectionManager.GetAllTitles(TitleTypes.AllFolders).ToDictionary(k => k.Id);
            Dictionary<int, int> _parentchildRelationship = new Dictionary<int, int>();  // titleid, parentid

            if (_mediaTree == null)
            {
                _mediaTree = new Dictionary<int, TreeNode>();
            }
            else
            {
                _mediaTree.Clear();
            }

            treeMedia.Nodes.Clear();
            TreeNode rootnode = new TreeNode("All Media");
            rootnode.Name = "All Media";
            treeMedia.Nodes.Add(rootnode);
            //SelectedTreeRoot = null;

            foreach (KeyValuePair<int, Title> title in mediatreefolders)
            {
                TreeNode tn = new TreeNode(title.Value.Name);
                tn.Name = title.Value.Id.ToString();
                _mediaTree[title.Value.Id] = tn;
                if (title.Value.ParentTitleId != null)
                {
                    _parentchildRelationship[title.Value.Id] = (int)title.Value.ParentTitleId;
                }
                //if (title.Value.Id == title.Value.ParentTitleId) { rootnode.Nodes.Add(tn); }
                if ((title.Value.TitleType & TitleTypes.Root) != 0) { rootnode.Nodes.Add(tn); }
            }


            foreach (KeyValuePair<int, TreeNode> kvp in _mediaTree)
            {
                if (_parentchildRelationship.ContainsKey(kvp.Key))
                {
                    if (kvp.Key != _parentchildRelationship[kvp.Key])
                    {
                        // This title has a parent.
                        if (_mediaTree.ContainsKey(_parentchildRelationship[kvp.Key]))
                        {
                            _mediaTree[_parentchildRelationship[kvp.Key]].Nodes.Add(_mediaTree[kvp.Key]);
                        }
                    }
                }
            }

            treeMedia.Refresh();
        }

        private void treeMedia_DoubleClick(object sender, EventArgs e)
        {
            ProcessSelectedNode();
            DialogResult = DialogResult.OK;
        }
    }
}