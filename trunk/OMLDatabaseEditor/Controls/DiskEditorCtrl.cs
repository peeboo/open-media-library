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
                    _currentDisk.Format = VideoFormat.DVD;
                    diskSource.EndEdit();
                }
            }
            else
            {
                fileDialog.FileName = txtPath.Text;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = fileDialog.FileName;
                    ValidateFile(fileDialog.FileName);
                    diskSource.EndEdit();
                }
            }
        }

        private void ValidateFile(string fileName)
        {
            string cleanedExtension = System.IO.Path.GetExtension(fileName).ToUpper();
            // remove the . from extension
            cleanedExtension = cleanedExtension.Replace(".", "");
            cleanedExtension = cleanedExtension.Replace("-", "");
            try
            {
                _currentDisk.Format = (VideoFormat)Enum.Parse(typeof(VideoFormat),
                    cleanedExtension, true);
            }
            catch (System.ArgumentException ae)
            {
                XtraMessageBox.Show("Unable to match extension " + cleanedExtension + " defaulting to MPG, " +
                    " if this is a valid video extension please post about it in our forusm.",
                    "Error Matching File Extension");
                Utilities.DebugLine("[DiskEditor] Error trying to match file extension " + cleanedExtension +
                    " to video format", ae);
                _currentDisk.Format = VideoFormat.MPG;
            }
            catch (System.Exception ex)
            {
                XtraMessageBox.Show(ex.ToString(), "Error");
                Utilities.DebugLine("[DiskEditor] Error trying to match file extension " + cleanedExtension +
                    " to video format", ex);
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
                ValidateFile(txtPath.Text);
            }
            else if (Directory.Exists(txtPath.Text))
            {
                _currentDisk.Format = VideoFormat.DVD;
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
