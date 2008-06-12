using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OMLEngine;

namespace OMLDatabaseEditor
{
    public partial class OMLDatabaseEditor : Form
    {
        private TitleCollection _titleCollection;
        private Title current_title;
        private DataTable _collectionAsDataTable;
        private Bitmap front_cover;
        private Bitmap back_cover;
        private Title _currentTitle = null;
        private bool _titleChanged = false;

        public OMLDatabaseEditor()
        {
            InitializeComponent();
            _titleCollection = new TitleCollection();
            _titleCollection.loadTitleCollection();
            SetupTitleList();
            
        }

        private void SetupTitleList()
        {
            //_collectionAsDataTable = GetTitlesDataTable();
            foreach (Title t in _titleCollection)
            {
                tvSourceList_AddItem(t.Name, t.InternalItemID, "Movies");
            }

            //grdTitleList.AutoGenerateColumns = false;
            //grdTitleList.DataSource = _collectionAsDataTable;
        }

        private void tvSourceList_AddItem(string text, int id, string type)
        {
            TreeNode nod = new TreeNode();
            nod.Name = id.ToString();
            nod.Text = text;
            //nod.Tag = n.NewNodeTag.ToString();

            //TreeNode TreeNodeParent = new TreeNode();
            //TreeNodeParent = tvSourceList.Nodes.Find(type, true)//.SelectedNode.Nodes.Add(nod);
            
            //nod.Parent.Name = type;
            tvSourceList.Nodes["OML Database"].Nodes[type].Nodes.Add(nod);
            tvSourceList.Nodes["OML Database"].ExpandAll();
            tvSourceList.Nodes["OML Database"].Nodes[type].ExpandAll();
        }
        
        

        private void dgv_title_list_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void OMLDatabaseEditor_Load(object sender, EventArgs e)
        {
            //_currencyManager = (CurrencyManager)this.BindingContext[_titleCollection];
        }


        


        private void tbTitle_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void tbSortName_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void tbOriginalName_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void cbRating_SelectedIndexChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void tbCountryOfOrigin_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }


        private void cbVideoStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void cbAspectRatio_SelectedIndexChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }

        private void tbRunTime_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }

        private void tbUserRating_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }

        private void dtpDateAdded_ValueChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }

        private void dtpReleaseDate_ValueChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }

        private void SelectNewTitle( int titleID)
        {
            bool bSelectNewTitle = true;

            if (_titleChanged)
            {
                DialogResult result = MessageBox.Show("Do you want to save the changes to the current movie?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Cancel)
                {
                    bSelectNewTitle = false;
                }
                else if (result == DialogResult.Yes)
                {
                    ctrMediaEditor.UpdateTitleFromUI(_currentTitle);
                    UpdateDBFromCurrentTitle(_currentTitle);
                }
                else
                {
                }
            }

            if (bSelectNewTitle)
            {
                int itemId = titleID;
                _currentTitle = (Title)_titleCollection.MoviesByItemId[itemId];
                ctrMediaEditor.ChangeTitle(_currentTitle);
                _titleChanged = false;
            }

        }

        private void UpdateDBFromCurrentTitle(Title t)
        {
            _titleCollection.Replace(t);
            _titleCollection.saveTitleCollection();
        }

        private void grdTitleList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectNewTitle(e.RowIndex);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ctrMediaEditor.UpdateTitleFromUI(_currentTitle);
            UpdateDBFromCurrentTitle(_currentTitle);
            _titleChanged = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //MapEditor.ChangeTitle(_currentTitle);
            _titleChanged = false;
        }

        private void tsbNewTitle_Click(object sender, EventArgs e)
        {
            Title t = new Title();
            _titleCollection.Add(t);

            _currentTitle = (Title)_titleCollection.MoviesByItemId[t.InternalItemID];

            tvSourceList_AddItem("New Movie", t.InternalItemID, "Movies");
        }

        private void tvSourceList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                if (e.Node.Parent.Name == "Movies")
                {
                    SelectNewTitle(int.Parse(e.Node.Name));
                }
            }
        }

    } 
}

