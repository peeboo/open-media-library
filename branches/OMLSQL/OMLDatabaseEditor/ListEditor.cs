using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace OMLDatabaseEditor
{
    public partial class ListEditor : DevExpress.XtraEditors.XtraForm
    {
        private IList<string> _list;

        public ListEditor(string name, IList<string> list)
        {
            InitializeComponent();
            _list = list;
            this.Text = name;
            lbItems.DataSource = _list;
            if (name == "Genres")
            {
                SetMRULists();
            }
        }

        private void btnItem_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (_list.Contains(cbeItem.Text)) return;

            if (this.Text == "Genres")
            {
                if (!Properties.Settings.Default.gsValidGenres.Contains(cbeItem.Text))
                {
                    DialogResult result = XtraMessageBox.Show("You are attempting to add a genre that is not in the allowed list. Would you like to add it to the list?\r\nClick \"Yes\" to add it to the list, \"No\" to add it to the movie but not the allowed list or \"Cancel\" to do nothing.", "Allowed Genre Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    switch (result)
                    {
                        case DialogResult.Yes:
                            Properties.Settings.Default.gsValidGenres.Add(cbeItem.Text);
                            break;
                        case DialogResult.No:
                            break;
                        case DialogResult.Cancel:
                            return;
                    }
                }
            }
            _list.Add(cbeItem.Text);

            cbeItem.Text = "";
        }

        private void lbItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lbItems.SelectedItems.Count > 0)
            {
                foreach(object item in lbItems.SelectedItems)
                {
                    _list.Remove((string)item);
                }
            }
        }

        public void SetMRULists()
        {
            if (Properties.Settings.Default.gbUseGenreList)
            {
                string[] aGenres = new string[Properties.Settings.Default.gsValidGenres.Count];
                Properties.Settings.Default.gsValidGenres.CopyTo(aGenres, 0);
                cbeItem.Properties.Items.AddRange(aGenres);
            }
        }
    }
}