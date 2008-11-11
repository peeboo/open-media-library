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
        private List<string> _list;

        public ListEditor(string name, List<string> list)
        {
            InitializeComponent();
            _list = list;
            this.Text = name;
            lbItems.DataSource = _list;
        }

        private void btnItem_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (_list.Contains(btnItem.Text)) return;

            _list.Add(btnItem.Text);

            btnItem.Text = "";
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
    }
}