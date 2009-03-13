using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

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
            if ((name == "Genres") || (name == "Tags"))
            {
                SetMRULists();
            }
        }

        private void lbItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lbItems.SelectedItems.Count > 0)
            {
                foreach(object item in lbItems.SelectedItems)
                {
                    _list.Remove((string)item);
                }
                (this.BindingContext[_list] as CurrencyManager).Refresh();
                lbItems.Refresh();
            }
        }

        public void SetMRULists()
        {
            if ((this.Text == "Genres") && (Properties.Settings.Default.gbUseGenreList) && Properties.Settings.Default.gsValidGenres != null)
            {
                string[] aGenres = new string[0];
                if (Properties.Settings.Default.gsValidGenres != null)
                {
                    aGenres = new string[Properties.Settings.Default.gsValidGenres.Count];
                    Properties.Settings.Default.gsValidGenres.CopyTo(aGenres, 0);
                }

                cbeItem.Properties.Items.AddRange(aGenres);
            }
            else if ((this.Text == "Tags"))
            {
                cbeItem.Properties.Items.AddRange(Properties.Settings.Default.gsTags.Split('|'));
            }
        }

        private void cbeItem_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Plus)
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
                (this.BindingContext[_list] as CurrencyManager).Refresh();
                lbItems.Refresh();

                cbeItem.Text = "";
            }
        }
    }
}