using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using OMLEngine;

namespace OMLDatabaseEditor
{
    public partial class DatabaseTools : DevExpress.XtraEditors.XtraForm
    {
        public DatabaseTools()
        {
            InitializeComponent();

            // Get database versioning
            OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
            int dbMajor;
            int dbMinor;
            dbm.GetSchemaVersion(out dbMajor, out dbMinor);
            lcDatabaseVersion.Text = "Database version : " + dbMajor.ToString() + "." + dbMinor.ToString();
            int dbReqMajor;
            int dbRejMinor;
            dbm.GetRequiredSchemaVersion(out dbReqMajor, out dbRejMinor);
            lcReqDatabaseVersion.Text = "Required Database version :" + dbReqMajor.ToString() + "." + dbRejMinor.ToString();

            int DataSize;
            int LogSize;
            dbm.GetDatabaseSize(out DataSize, out LogSize);
            lcDatabaseSize.Text = "Database size : Data " + DataSize.ToString() + " MB, Logs " + LogSize.ToString() + " MB";
        }

        private void sbBackupDB_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.DefaultExt = "bak";
            sfd.FileName = "OMLDatabase";
            sfd.Filter = "Backup files (*.bak)|*.bak";
            sfd.FilterIndex = 1;
            sfd.OverwritePrompt = true;
            sfd.InitialDirectory = OMLEngine.FileSystemWalker.DBBackupDirectory;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
                Cursor = Cursors.WaitCursor;
                if (dbm.BackupDatabase(sfd.FileName))
                {
                    Cursor = Cursors.Default; 
                    XtraMessageBox.Show("The backup has been successful", "Database Backup");
                }
                else
                {
                    Cursor = Cursors.Default;
                    XtraMessageBox.Show("The backup has failed with error : " + OMLEngine.DatabaseManagement.DatabaseInformation.LastSQLError, "Database Backup");
                }
            }
        }

        private void sbRestoreDB_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = "bak";
            ofd.Filter = "Backup files (*.bak)|*.bak";
            ofd.FilterIndex = 1;
            ofd.InitialDirectory = OMLEngine.FileSystemWalker.DBBackupDirectory;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
                if (dbm.RestoreDatabase(ofd.FileName))
                {
                    Cursor = Cursors.Default; 
                    XtraMessageBox.Show("The restore has been successful. Please close and reopen any open OML applications", "Database Restore");
                }
                else
                {
                    Cursor = Cursors.Default;
                    XtraMessageBox.Show("The restore has failed with error : " + OMLEngine.DatabaseManagement.DatabaseInformation.LastSQLError, "Database Restore");
                }
            }
        }

        private void sbOptimizeDB_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("This may take some time, do you want to continue?", "Optimize Database", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
                if (dbm.OptimiseDatabase())
                {
                    Cursor = Cursors.Default;
                    XtraMessageBox.Show("The routine has been successful", "Database Optimization");
                }
                else
                {
                    Cursor = Cursors.Default;
                    XtraMessageBox.Show("The process has failed with error : " + OMLEngine.DatabaseManagement.DatabaseInformation.LastSQLError, "Database Optimization");
                }
            }
        }
    }
}