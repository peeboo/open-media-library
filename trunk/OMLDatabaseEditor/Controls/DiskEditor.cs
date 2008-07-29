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
            cbxVideoFormat.Items.AddRange(Enum.GetNames(typeof(VideoFormat)));
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
                    try
                    {
                        Format = (VideoFormat)Enum.Parse(typeof(VideoFormat),
                            System.IO.Path.GetExtension(dlg.FileName).Replace(".", "").ToUpper(), true);
                    }
                    catch (System.ArgumentException ae)
                    {
                        Format = (VideoFormat)Enum.Parse(typeof(VideoFormat),
                            "DVD", true);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Error");
                    }
                }
                
            } //Folder
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

        public VideoFormat Format
        {
            get { return _videoFormat; }
            set
            {
                _videoFormat = value;
                cbxVideoFormat.SelectedItem = _videoFormat.ToString();
            }
        }

    }
}
