using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraEditors;

using OMLEngine;

namespace OMLDatabaseEditor.Controls
{
    public partial class DiskEditorFrm : XtraForm
    {
        private List<Disk> _disks;

        public DiskEditorFrm(List<Disk> disks)
        {
            InitializeComponent();
            _disks = disks;
            lbDisks.DataSource = _disks;
        }

        private void lbDisks_SelectedIndexChanged(object sender, EventArgs e)
        {
            diskEditor.LoadDisk(lbDisks.SelectedItem as Disk);
            diskEditor.Visible = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lbDisks.SelectedItem != null)
            {
                _disks.Remove(lbDisks.SelectedItem as Disk);
                if (_disks.Count == 0)
                {
                    diskEditor.LoadDisk(null);
                    diskEditor.Visible = false;
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Disk newDisk = new Disk("Disk " + (_disks.Count + 1).ToString(), "", VideoFormat.MPG);
            _disks.Add(newDisk);
            lbDisks.SelectedItem = newDisk;
            diskEditor.LoadDisk(newDisk);
            diskEditor.Visible = true;
        }

        private void DiskEditorFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            diskEditor.SaveChanges();
        }

        private void btnDiskInfo_Click(object sender, EventArgs e)
        {
            if (lbDisks.SelectedItem != null)
            {
                DiskInfoFrm di = new DiskInfoFrm(lbDisks.SelectedItem as Disk);
                di.ShowDialog();
            }
        }
    }
}