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
        private TreeNode m_OldSelectNode;

        public OMLDatabaseEditor()
        {
            InitializeComponent();
            _titleCollection = new TitleCollection();
            _titleCollection.loadTitleCollection();
            SetupTitleList();

        }

        private void SetupTitleList()
        {
            foreach (Title t in _titleCollection)
            {
                tvSourceList_AddItem(t.Name, t.InternalItemID, "Movies");
            }
        }

        private void tvSourceList_AddItem(string text, int id, string type)
        {
            TreeNode nod = new TreeNode();
            nod.Name = id.ToString();
            nod.Text = text;
            nod.Tag = "Movies";
            tvSourceList.Nodes["OML Database"].Nodes[type].Nodes.Add(nod);
            tvSourceList.Nodes["OML Database"].ExpandAll();
            tvSourceList.Nodes["OML Database"].Nodes[type].ExpandAll();
        }

        private void SaveTitleChangesToDB(Title t)
        {
            _titleCollection.Replace(t);
            _titleCollection.saveTitleCollection();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Controls.MediaEditor _currentEditor = (Controls.MediaEditor)this.tabsMediaPanel.SelectedTab.Controls[0];
            Title _currentTitle = (Title)_titleCollection.MoviesByItemId[int.Parse(this.tabsMediaPanel.SelectedTab.Name)];

            _currentEditor.SaveToTitle(_currentTitle);
            SaveTitleChangesToDB(_currentTitle);
        }

        private void tsbNewTitle_Click(object sender, EventArgs e)
        {
            Title t = new Title();
            _titleCollection.Add(t);

            tvSourceList_AddItem("New Movie", t.InternalItemID, "Movies");
        }

        
        private void tsbClose_Click(object sender, EventArgs e)
        {
            Controls.MediaEditor _currentEditor = (Controls.MediaEditor)tabsMediaPanel.SelectedTab.Controls[0];
            bool _bClose = true;

            if (_currentEditor.Status == global::OMLDatabaseEditor.Controls.MediaEditor.TitleStatus.UnsavedChanges)
            {
                DialogResult result = MessageBox.Show("Do you want to save the changes to the current movie?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Cancel)
                {
                    _bClose = false;
                }
                else if (result == DialogResult.Yes)
                {
                    Title _currentTitle = (Title)_titleCollection.MoviesByItemId[int.Parse(this.tabsMediaPanel.SelectedTab.Name)];

                    _currentEditor.SaveToTitle(_currentTitle);
                    SaveTitleChangesToDB(_currentTitle);
                }
                else
                {
                }
            }

            if (_bClose)
            {
                TabPage _currentTab = tabsMediaPanel.SelectedTab;
                tabsMediaPanel.TabPages.Remove(_currentTab);
            }
        }

        private void MenuItemEditTab_Click(object sender, EventArgs e)
        {
            if (tvSourceList.SelectedNode.Tag != null)
            {
                if (tvSourceList.SelectedNode.Tag.ToString() == "Movies")
                {
                    EditNewTab(int.Parse(tvSourceList.SelectedNode.Name));
                }
            }
        }

        private void EditNewTab(int itemID)
        {
            Controls.MediaEditor Editor = new Controls.MediaEditor();

            Editor.AutoScroll = true;
            Editor.AutoSize = true;
            Editor.BackColor = System.Drawing.SystemColors.Control;
            Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            Editor.Font = new System.Drawing.Font("Cambria", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Editor.Location = new System.Drawing.Point(3, 3);
            Editor.MinimumSize = new System.Drawing.Size(600, 836);
            Editor.Name = "ME" + itemID.ToString();
            Editor.Size = new System.Drawing.Size(616, 836);
            Editor.TabIndex = 0;

            Title currentTitle = new Title();
            currentTitle = (Title)_titleCollection.MoviesByItemId[itemID];
            tabsMediaPanel.TabPages.Add(itemID.ToString(), currentTitle.Name);
            tabsMediaPanel.TabPages[itemID.ToString()].Controls.Add(Editor);
            Editor.LoadTitle(currentTitle);
            Editor.TitleChanged += new Controls.MediaEditor.TitleChangeEventHandler(this.TitleChanges);
            Editor.TitleNameChanged += new Controls.MediaEditor.TitleNameChangeEventHandler(this.TitleNameChanges);
            Editor.SavedTitle += new Controls.MediaEditor.SavedEventHandler(this.SavedTitle);
        }

        private void tvSourceList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = tvSourceList.GetNodeAt(p);
                if (node != null)
                {

                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    m_OldSelectNode = tvSourceList.SelectedNode;
                    tvSourceList.SelectedNode = node;

                    // Find the appropriate ContextMenu depending on the selected node.
                    switch (Convert.ToString(node.Tag))
                    {
                        case "Movies":
                            MenuStripTitle.Show(tvSourceList, p);
                            break;
                    }

                    // Highlight the sel`ected node.
                    tvSourceList.SelectedNode = m_OldSelectNode;
                    m_OldSelectNode = null;
                }
            }
        }

        private void TitleChanges(object sender, EventArgs e)
        {
            Controls.MediaEditor _currentEditor = (Controls.MediaEditor)sender;

            tabsMediaPanel.TabPages[_currentEditor.itemID.ToString()].Text = "*" + _currentEditor.TitleName;
        }

        private void TitleNameChanges(object sender, EventArgs e)
        {
            Controls.MediaEditor _currentEditor = (Controls.MediaEditor)sender;

            tabsMediaPanel.TabPages[_currentEditor.itemID.ToString()].Text = "*" + _currentEditor.TitleName;
        }

        private void SavedTitle(object sender, EventArgs e)
        {
            Controls.MediaEditor _currentEditor = (Controls.MediaEditor)sender;

            tabsMediaPanel.TabPages[_currentEditor.itemID.ToString()].Text = _currentEditor.TitleName;
        }

    }
}

