namespace OMLDatabaseEditor.Controls
{
    partial class DiskEditorFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiskEditorFrm));
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
            this.lbDisks = new DevExpress.XtraEditors.ListBoxControl();
            this.btnDiskInfo = new DevExpress.XtraEditors.SimpleButton();
            this.diskEditor = new OMLDatabaseEditor.Controls.DiskEditorCtrl();
            ((System.ComponentModel.ISupportInitialize)(this.lbDisks)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(84, 135);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 9;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(3, 135);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lbDisks
            // 
            this.lbDisks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDisks.Location = new System.Drawing.Point(3, 3);
            this.lbDisks.Name = "lbDisks";
            this.lbDisks.Size = new System.Drawing.Size(353, 132);
            this.lbDisks.TabIndex = 6;
            this.lbDisks.Click += new System.EventHandler(this.lbDisks_SelectedIndexChanged);
            // 
            // btnDiskInfo
            // 
            this.btnDiskInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDiskInfo.Location = new System.Drawing.Point(165, 135);
            this.btnDiskInfo.Name = "btnDiskInfo";
            this.btnDiskInfo.Size = new System.Drawing.Size(75, 23);
            this.btnDiskInfo.TabIndex = 10;
            this.btnDiskInfo.Text = "Disk Info";
            this.btnDiskInfo.Click += new System.EventHandler(this.btnDiskInfo_Click);
            // 
            // diskEditor
            // 
            this.diskEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.diskEditor.Location = new System.Drawing.Point(3, 159);
            this.diskEditor.Name = "diskEditor";
            this.diskEditor.Size = new System.Drawing.Size(352, 58);
            this.diskEditor.TabIndex = 7;
            this.diskEditor.Visible = false;
            // 
            // DiskEditorFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 221);
            this.Controls.Add(this.btnDiskInfo);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.diskEditor);
            this.Controls.Add(this.lbDisks);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiskEditorFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Disks";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DiskEditorFrm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.lbDisks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SimpleButton btnAdd;
        private DiskEditorCtrl diskEditor;
        private DevExpress.XtraEditors.ListBoxControl lbDisks;
        private DevExpress.XtraEditors.SimpleButton btnDiskInfo;
    }
}

