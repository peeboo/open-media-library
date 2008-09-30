using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OMLEngine;

namespace OMLDatabaseEditor.Controls
{
    public partial class DiskEditor : Form
    {
        private VideoFormat _videoFormat;

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
            // File?
            if (rbFile.Checked) 
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.CheckPathExists = true;
                dlg.FileName = txtPath.Text;
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dlg.FileName;
                    string cleanedExtension = System.IO.Path.GetExtension(dlg.FileName).ToUpper();
                    // remove the . from extension
                    cleanedExtension = cleanedExtension.Replace(".", "");
                    cleanedExtension = cleanedExtension.Replace("-", "");
                    try
                    {
                        Format = (VideoFormat)Enum.Parse(typeof(VideoFormat),
                            cleanedExtension, true);
                    }
                    catch (System.ArgumentException ae)
                    {
                        MessageBox.Show("Unable to match extension " + cleanedExtension + " defaulting to MPG, " +
                            " if this is a valid video extension please post about it in our forusm.", 
                            "Error Matching File Extension");
                        Utilities.DebugLine("[DiskEditor] Error trying to match file extension " + cleanedExtension +
                            " to video format", ae);
                        Format = VideoFormat.MPG;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Error");
                        Utilities.DebugLine("[DiskEditor] Error trying to match file extension " + cleanedExtension +
                            " to video format", ex);
                    }
                }
            } 
            // DVD - Folder
            else
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.SelectedPath = txtPath.Text;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = dlg.SelectedPath;
                    Format = VideoFormat.DVD;
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

        public string ExtraOptions
        {
            get { return txtExtraOptions.Text; }
            set { txtExtraOptions.Text = string.IsNullOrEmpty(value) ? string.Empty : value; }
        }

        public VideoFormat Format
        {
            get { return _videoFormat; }
            set 
            { 
                _videoFormat = value;
                if (_videoFormat.ToString() == "DVD")
                {
                    rbFolder.Checked = true;
                    rbFile.Checked = false;
                }
                else
                {
                    rbFolder.Checked = false;
                    rbFile.Checked = true;
                }
            }
        }

        private void rbFolder_CheckedChanged(object sender, EventArgs e)
        {
            btnExtraOptions.Enabled = txtExtraOptions.Enabled = rbFolder.Checked;
            if (rbFolder.Checked == false)
                txtExtraOptions.Text = "";
        }

        private void btnExtraOptions_Click(object sender, EventArgs e)
        {
            ExtenderExtraOptions dlg = new ExtenderExtraOptions();
            dlg.Disk = new Disk(this.FileName, this.Path, this.Format, this.ExtraOptions);
            if (dlg.Disk.DVDDiskInfo == null)
                return;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.ExtraOptions = dlg.MediaSource.ExtraOptions;
            }
        }

    }
}
