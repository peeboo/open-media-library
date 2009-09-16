using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace OMLDatabaseEditor.Controls
{
    public partial class DiskMoverFrm : DevExpress.XtraEditors.XtraForm
    {
        List<string> folders;

        public DiskMoverFrm(List<string> pfolders)
        {
            InitializeComponent();
            folders = pfolders;
        }

        public string fromFolder
        {
            get
            {
                return lbcFolders.SelectedValue.ToString();
            }
        }

        public string toFolder
        {
            get
            {
                return beDestination.Text;
            }
        }

        public Boolean withImages
        {
            get
            {
                return ceWithImages.Checked;
            }
        }

        private void DiskMoverFrm_Load(object sender, EventArgs e)
        {
            lbcFolders.Items.AddRange(folders.ToArray());
            beDestination.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            beDestination.MaskBox.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
        }

        private void sbOK_Click(object sender, EventArgs e)
        {
            if (!System.IO.Directory.Exists(beDestination.Text))
            {
                this.DialogResult = DialogResult.None;
                XtraMessageBox.Show("Path does not exist");
            }
        }

        private void beDestination_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                beDestination.Text = fbd.SelectedPath;
            }
        }
    }
}