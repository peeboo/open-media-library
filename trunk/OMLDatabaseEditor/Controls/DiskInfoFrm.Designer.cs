namespace OMLDatabaseEditor.Controls
{
    partial class DiskInfoFrm
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
            this.tvDiskInfo = new System.Windows.Forms.TreeView();
            this.sbClose = new DevExpress.XtraEditors.SimpleButton();
            this.sbSetAsMainFeature = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            // 
            // tvDiskInfo
            // 
            this.tvDiskInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvDiskInfo.Location = new System.Drawing.Point(12, 12);
            this.tvDiskInfo.Name = "tvDiskInfo";
            this.tvDiskInfo.Size = new System.Drawing.Size(458, 294);
            this.tvDiskInfo.TabIndex = 0;
            // 
            // sbClose
            // 
            this.sbClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.sbClose.Location = new System.Drawing.Point(163, 338);
            this.sbClose.Name = "sbClose";
            this.sbClose.Size = new System.Drawing.Size(75, 23);
            this.sbClose.TabIndex = 1;
            this.sbClose.Text = "Close";
            // 
            // sbSetAsMainFeature
            // 
            this.sbSetAsMainFeature.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbSetAsMainFeature.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sbSetAsMainFeature.Location = new System.Drawing.Point(245, 338);
            this.sbSetAsMainFeature.Name = "sbSetAsMainFeature";
            this.sbSetAsMainFeature.Size = new System.Drawing.Size(75, 23);
            this.sbSetAsMainFeature.TabIndex = 2;
            this.sbSetAsMainFeature.Text = "Update Title";
            this.sbSetAsMainFeature.Click += new System.EventHandler(this.sbSetAsMainFeature_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 312);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(448, 13);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "To update the titles\' resolution, aspect ratio && length, select a feature and cl" +
                "ick \'Update Title\'.";
            // 
            // DiskInfoFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 368);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.sbSetAsMainFeature);
            this.Controls.Add(this.sbClose);
            this.Controls.Add(this.tvDiskInfo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiskInfoFrm";
            this.Text = "Disk Information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvDiskInfo;
        private DevExpress.XtraEditors.SimpleButton sbClose;
        private DevExpress.XtraEditors.SimpleButton sbSetAsMainFeature;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}
