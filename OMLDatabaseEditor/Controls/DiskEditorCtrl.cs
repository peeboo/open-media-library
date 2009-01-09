using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OMLEngine;

namespace OMLDatabaseEditor.Controls
{
    public partial class DiskEditorCtrl : UserControl
    {
        private Disk _currentDisk;

        public DiskEditorCtrl()
        {
            InitializeComponent();

            txtPath.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtPath.MaskBox.AutoCompleteSource = AutoCompleteSource.FileSystem;
        }

        public void LoadDisk(Disk disk)
        {
            _currentDisk = disk;
            if (disk == null)
                diskSource.DataSource = typeof(Disk);
            else
                diskSource.DataSource = _currentDisk;
        }

        public void SaveChanges()
        {
            diskSource.EndEdit();
        }

        private void ChangePath()
        {
            if (cbDVD.Checked)
            {
                folderDialog.SelectedPath = txtPath.Text;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = folderDialog.SelectedPath;
                    _currentDisk.Format = _currentDisk.GetFormatFromPath(folderDialog.SelectedPath);
                    diskSource.EndEdit();
                }
            }
            else
            {
                fileDialog.FileName = txtPath.Text;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = fileDialog.FileName;
                    _currentDisk.Format = _currentDisk.GetFormatFromPath(fileDialog.FileName);
                    diskSource.EndEdit();
                }
            }
        }

        private void EditMenu()
        {
            ExtenderDVDMenu dlg = new ExtenderDVDMenu();
            dlg.Disk = new Disk(_currentDisk.Name, _currentDisk.Path, _currentDisk.Format, _currentDisk.ExtraOptions);
            if (dlg.Disk.DVDDiskInfo == null)
                return;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _currentDisk.ExtraOptions = dlg.Disk.ExtraOptions;
            }
        }

        private void diskSource_DataSourceChanged(object sender, EventArgs e)
        {
            if (diskSource.DataSource is Disk && diskSource.DataSource != null)
            {
                cbDVD.Checked = ((Disk)diskSource.DataSource).Format == VideoFormat.DVD;
                cbDVD_CheckedChanged(null, null);
            }
        }

        private void txtPath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Tag.ToString() == "Browse")
                ChangePath();
            else
                EditMenu();
        }

        private void cbDVD_CheckedChanged(object sender, EventArgs e)
        {
            txtPath.Properties.Buttons[1].Enabled = cbDVD.Checked;
            if (cbDVD.Checked && !Directory.Exists(txtPath.Text)) txtPath.Text = "";
            if (!cbDVD.Checked)
                _currentDisk.ExtraOptions = "";
        }

        private void txtPath_Leave(object sender, EventArgs e)
        {
            // Validate the new path
            if (File.Exists(txtPath.Text))
            {
                _currentDisk.Format = _currentDisk.GetFormatFromPath(txtPath.Text);
            }
            else if (Directory.Exists(txtPath.Text))
            {
                _currentDisk.Format = _currentDisk.GetFormatFromPath(txtPath.Text);
                if (_currentDisk.Format == VideoFormat.UNKNOWN)
                    XtraMessageBox.Show("The new path does not contain a DVD, Blu-ray or HDDVD movie", "Error");
                else
                    cbDVD.Checked = true;
            }
            else
            {
                XtraMessageBox.Show("The new path is neither a file nor directory", "Error");
                txtPath.Focus();
                txtPath.SelectAll();
            }
        }
    }
}
