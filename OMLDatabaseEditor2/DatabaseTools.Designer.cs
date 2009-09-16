namespace OMLDatabaseEditor
{
    partial class DatabaseTools
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sbBackupDB = new DevExpress.XtraEditors.SimpleButton();
            this.sbOptimizeDB = new DevExpress.XtraEditors.SimpleButton();
            this.sbRestoreDB = new DevExpress.XtraEditors.SimpleButton();
            this.sbClose = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lcDatabaseSize = new DevExpress.XtraEditors.LabelControl();
            this.lcReqDatabaseVersion = new DevExpress.XtraEditors.LabelControl();
            this.lcDatabaseVersion = new DevExpress.XtraEditors.LabelControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.sbDatabaseIntegrity = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // sbBackupDB
            // 
            this.sbBackupDB.Location = new System.Drawing.Point(59, 27);
            this.sbBackupDB.Name = "sbBackupDB";
            this.sbBackupDB.Size = new System.Drawing.Size(113, 23);
            this.sbBackupDB.TabIndex = 0;
            this.sbBackupDB.Text = "Backup to a file";
            this.sbBackupDB.Click += new System.EventHandler(this.sbBackupDB_Click);
            // 
            // sbOptimizeDB
            // 
            this.sbOptimizeDB.Location = new System.Drawing.Point(59, 30);
            this.sbOptimizeDB.Name = "sbOptimizeDB";
            this.sbOptimizeDB.Size = new System.Drawing.Size(113, 23);
            this.sbOptimizeDB.TabIndex = 3;
            this.sbOptimizeDB.Text = "Optimize";
            this.sbOptimizeDB.Click += new System.EventHandler(this.sbOptimizeDB_Click);
            // 
            // sbRestoreDB
            // 
            this.sbRestoreDB.Location = new System.Drawing.Point(59, 56);
            this.sbRestoreDB.Name = "sbRestoreDB";
            this.sbRestoreDB.Size = new System.Drawing.Size(113, 23);
            this.sbRestoreDB.TabIndex = 4;
            this.sbRestoreDB.Text = "Restore from a file";
            this.sbRestoreDB.Click += new System.EventHandler(this.sbRestoreDB_Click);
            // 
            // sbClose
            // 
            this.sbClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sbClose.Location = new System.Drawing.Point(71, 335);
            this.sbClose.Name = "sbClose";
            this.sbClose.Size = new System.Drawing.Size(113, 23);
            this.sbClose.TabIndex = 6;
            this.sbClose.Text = "Close";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lcDatabaseSize);
            this.groupBox1.Controls.Add(this.lcReqDatabaseVersion);
            this.groupBox1.Controls.Add(this.lcDatabaseVersion);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(234, 82);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // lcDatabaseSize
            // 
            this.lcDatabaseSize.Location = new System.Drawing.Point(11, 58);
            this.lcDatabaseSize.Name = "lcDatabaseSize";
            this.lcDatabaseSize.Size = new System.Drawing.Size(77, 13);
            this.lcDatabaseSize.TabIndex = 12;
            this.lcDatabaseSize.Text = "Database size : ";
            // 
            // lcReqDatabaseVersion
            // 
            this.lcReqDatabaseVersion.Location = new System.Drawing.Point(11, 39);
            this.lcReqDatabaseVersion.Name = "lcReqDatabaseVersion";
            this.lcReqDatabaseVersion.Size = new System.Drawing.Size(137, 13);
            this.lcReqDatabaseVersion.TabIndex = 11;
            this.lcReqDatabaseVersion.Text = "Required Database version :";
            // 
            // lcDatabaseVersion
            // 
            this.lcDatabaseVersion.Location = new System.Drawing.Point(11, 20);
            this.lcDatabaseVersion.Name = "lcDatabaseVersion";
            this.lcDatabaseVersion.Size = new System.Drawing.Size(91, 13);
            this.lcDatabaseVersion.TabIndex = 10;
            this.lcDatabaseVersion.Text = "Database version :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.sbBackupDB);
            this.groupBox2.Controls.Add(this.sbRestoreDB);
            this.groupBox2.Location = new System.Drawing.Point(12, 100);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(234, 96);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Backup && Restore";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.sbDatabaseIntegrity);
            this.groupBox3.Controls.Add(this.sbOptimizeDB);
            this.groupBox3.Location = new System.Drawing.Point(12, 214);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(234, 100);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Maintenance";
            // 
            // sbDatabaseIntegrity
            // 
            this.sbDatabaseIntegrity.Location = new System.Drawing.Point(59, 59);
            this.sbDatabaseIntegrity.Name = "sbDatabaseIntegrity";
            this.sbDatabaseIntegrity.Size = new System.Drawing.Size(113, 23);
            this.sbDatabaseIntegrity.TabIndex = 4;
            this.sbDatabaseIntegrity.Text = "Check DB Integrity";
            this.sbDatabaseIntegrity.Click += new System.EventHandler(this.sbDatabaseIntegrity_Click);
            // 
            // DatabaseTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 376);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.sbClose);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatabaseTools";
            this.Text = "DatabaseTools";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton sbBackupDB;
        private DevExpress.XtraEditors.SimpleButton sbOptimizeDB;
        private DevExpress.XtraEditors.SimpleButton sbRestoreDB;
        private DevExpress.XtraEditors.SimpleButton sbClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private DevExpress.XtraEditors.LabelControl lcDatabaseVersion;
        private DevExpress.XtraEditors.LabelControl lcReqDatabaseVersion;
        private DevExpress.XtraEditors.LabelControl lcDatabaseSize;
        private DevExpress.XtraEditors.SimpleButton sbDatabaseIntegrity;
    }
}