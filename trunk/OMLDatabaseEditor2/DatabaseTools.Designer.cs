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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.sbOptimizeDB = new DevExpress.XtraEditors.SimpleButton();
            this.sbRestoreDB = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.sbClose = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // sbBackupDB
            // 
            this.sbBackupDB.Location = new System.Drawing.Point(62, 31);
            this.sbBackupDB.Name = "sbBackupDB";
            this.sbBackupDB.Size = new System.Drawing.Size(75, 23);
            this.sbBackupDB.TabIndex = 0;
            this.sbBackupDB.Text = "Backup";
            this.sbBackupDB.Click += new System.EventHandler(this.sbBackupDB_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(85, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Backup && Restore";
            // 
            // sbOptimizeDB
            // 
            this.sbOptimizeDB.Location = new System.Drawing.Point(62, 122);
            this.sbOptimizeDB.Name = "sbOptimizeDB";
            this.sbOptimizeDB.Size = new System.Drawing.Size(75, 23);
            this.sbOptimizeDB.TabIndex = 3;
            this.sbOptimizeDB.Text = "Optimize";
            this.sbOptimizeDB.Click += new System.EventHandler(this.sbOptimizeDB_Click);
            // 
            // sbRestoreDB
            // 
            this.sbRestoreDB.Location = new System.Drawing.Point(62, 60);
            this.sbRestoreDB.Name = "sbRestoreDB";
            this.sbRestoreDB.Size = new System.Drawing.Size(75, 23);
            this.sbRestoreDB.TabIndex = 4;
            this.sbRestoreDB.Text = "Restore";
            this.sbRestoreDB.Click += new System.EventHandler(this.sbRestoreDB_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(12, 103);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(61, 13);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "Maintenance";
            // 
            // sbClose
            // 
            this.sbClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sbClose.Location = new System.Drawing.Point(62, 171);
            this.sbClose.Name = "sbClose";
            this.sbClose.Size = new System.Drawing.Size(75, 23);
            this.sbClose.TabIndex = 6;
            this.sbClose.Text = "Close";
            // 
            // DatabaseTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(191, 206);
            this.Controls.Add(this.sbClose);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.sbRestoreDB);
            this.Controls.Add(this.sbOptimizeDB);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.sbBackupDB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatabaseTools";
            this.Text = "DatabaseTools";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton sbBackupDB;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton sbOptimizeDB;
        private DevExpress.XtraEditors.SimpleButton sbRestoreDB;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton sbClose;
    }
}