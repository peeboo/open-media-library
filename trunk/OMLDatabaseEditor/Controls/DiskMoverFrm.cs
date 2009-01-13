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
        public DiskMoverFrm()
        {
            InitializeComponent();
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
            lbcFolders.Items.AddRange(MainEditor._titleCollection.GetFolders.ToArray());
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
    }
}