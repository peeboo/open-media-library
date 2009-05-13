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
        private Title _currentTitle;

        public DiskEditorCtrl()
        {
            InitializeComponent();

            txtPath.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtPath.MaskBox.AutoCompleteSource = AutoCompleteSource.FileSystem;
        }

        public void LoadDisk(Title title, Disk disk)
        {
            _currentDisk = disk;
            _currentTitle = title;
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
                    AutoScanDisk();
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
                    AutoScanDisk();
                }
            }

            Parent.Controls["lbDisks"].Refresh();
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
                AutoScanDisk();
            }
            else if (Directory.Exists(txtPath.Text))
            {
                _currentDisk.Format = _currentDisk.GetFormatFromPath(txtPath.Text);
                if (_currentDisk.Format == VideoFormat.UNKNOWN)
                    XtraMessageBox.Show("The new path does not contain a DVD, Blu-ray or HDDVD movie", "Error");
                else
                {
                    cbDVD.Checked = true;
                    AutoScanDisk();
                }
            }
            else
            {
                XtraMessageBox.Show("The new path is neither a file nor directory", "Error");
                txtPath.Focus();
                txtPath.SelectAll();
            }

            Parent.Controls["lbDisks"].Refresh();
        }

        private void AutoScanDisk()
        {
            try
            {
                // Auto scan disk
                if (OMLEngine.Settings.OMLSettings.AutoScanDiskOnAdding)
                {
                    int Feature = 0;
                    int Length = 0;
                    for (int i = 0; i < _currentDisk.DiskFeatures.Count; i++)
                    {
                        int calclength = _currentDisk.DiskFeatures[Feature].Duration.Hours * 60 + _currentDisk.DiskFeatures[Feature].Duration.Minutes;
                        if (calclength > Length)
                        {
                            Length = calclength;
                            Feature = i;
                        }

                    }
                    _currentDisk.MainFeatureXRes = _currentDisk.DiskFeatures[Feature].VideoStreams[0].Resolution.Width;
                    _currentDisk.MainFeatureYRes = _currentDisk.DiskFeatures[Feature].VideoStreams[0].Resolution.Height;
                    _currentDisk.MainFeatureAspectRatio = _currentDisk.DiskFeatures[Feature].VideoStreams[0].AspectRatio;
                    _currentDisk.MainFeatureFPS = _currentDisk.DiskFeatures[Feature].VideoStreams[0].FrameRate;
                    _currentDisk.MainFeatureLength = _currentDisk.DiskFeatures[Feature].Duration.Hours * 60 + _currentDisk.DiskFeatures[Feature].Duration.Minutes;

                    if (OMLEngine.Settings.OMLSettings.ScanDiskRollInfoToTitle)
                    {
                        _currentTitle.VideoResolution = _currentDisk.MainFeatureXRes.ToString() + "x" + _currentDisk.MainFeatureYRes.ToString();
                        _currentTitle.AspectRatio = _currentDisk.MainFeatureAspectRatio;
                        //_disk.MainFeatureFPS = _disk.DiskFeatures[Feature].VideoStreams[VideoStream].FrameRate;
                        _currentTitle.Runtime = _currentDisk.MainFeatureLength ?? 0;
                    }

                    (Parent as DiskEditorFrm).DiskDirty = true;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void txtName_Leave(object sender, EventArgs e)
        {
            Parent.Controls["lbDisks"].Refresh();
        }
    }
}
