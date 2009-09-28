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

            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
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
            /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/

            /*** ADD ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
            //OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
            /*** ADD ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
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
            OMLEngine.ImageManager.RemoveAllCachedImages();
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

        private void sbDatabaseIntegrity_Click(object sender, EventArgs e)
        {
            OMLEngine.DatabaseManagement.DatabaseIntegrityChecks dbi = new OMLEngine.DatabaseManagement.DatabaseIntegrityChecks();

            List<Title> OrphanedTitles = new List<Title>();

            if (dbi.CheckOrphanedTitles(out OrphanedTitles))
            {
                // There are orphaned titles
                // TODO - Must do something about them
                StringBuilder sb = new StringBuilder();
                foreach (Title t in OrphanedTitles)
                {
                    if (sb.Length < 255)
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append(t.Name);
                    }
                }
                XtraMessageBox.Show("We have found the following orphaned titles - " + sb.ToString(), "Orphaned Titles!");
            }
            else
            {
                XtraMessageBox.Show("No orphaned titles were found.", "Orphaned Titles!");
            }
        }

        private void sbViewDatabaseFileInfo_Click(object sender, EventArgs e)
        {
            OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
            
            List<OMLEngine.DatabaseManagement.DatabaseManagement.DatabaseFile> DBFS = dbm.GetDatabaseFileInfo();

            if (DBFS != null)
            {
                StringBuilder sb = new StringBuilder();

                foreach (OMLEngine.DatabaseManagement.DatabaseManagement.DatabaseFile DBF in DBFS)
                {
                    sb.AppendLine("Name : " + DBF.Name + ", " +
                        "Size : " + DBF.SizeString + ", " +
                        "Max Size : " + DBF.MaxSizeString + ", " +
                        "Growth : " + DBF.GrowthString);

                }

                MessageBox.Show(sb.ToString(), "Database File Information");
            }
        }

        private void sbRecreateDatabase_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will recreate the database deleting ALL movies, genres, people etc.\n\t Are you sure?", "Recreate database", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (MessageBox.Show("Are you really sure?", "Recreate database", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {      
                    OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();
                    dbm.CreateSchema();
                    OMLEngine.ImageManager.RemoveAllCachedImages();
                    MessageBox.Show("The database has been recreated.\n Please restart any open OML appliations.", "Recreate database", MessageBoxButtons.OK);
                }
            }
        }
    }
}