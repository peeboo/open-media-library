using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OMLDatabaseEditor.Controls
{
    public partial class DiskEditor : Form
    {
        public DiskEditor()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //TODO: Validate the Path
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (chkFile.Checked)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.CheckPathExists = true;
                dlg.FileName = txtPath.Text;
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dlg.FileName;
                }
            }
            else
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.SelectedPath = txtPath.Text;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dlg.SelectedPath;
                }
            }
        }

        public string FileName
        {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }

        public string Path
        {
            get { return txtPath.Text; }
            set { txtPath.Text = value; }
        }

    }
}
