namespace OMLDatabaseEditor
{
    partial class DownloadingBackDropsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.pbProgress = new DevExpress.XtraEditors.ProgressBarControl();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.lcStatus = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(308, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please wait, OML is downloading BackDrop images for this title.";
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(15, 48);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(302, 18);
            this.pbProgress.TabIndex = 1;
            // 
            // simpleButton1
            // 
            this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.simpleButton1.Location = new System.Drawing.Point(127, 76);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(75, 23);
            this.simpleButton1.TabIndex = 2;
            this.simpleButton1.Text = "Cancel";
            // 
            // lcStatus
            // 
            this.lcStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lcStatus.Location = new System.Drawing.Point(141, 29);
            this.lcStatus.Name = "lcStatus";
            this.lcStatus.Size = new System.Drawing.Size(45, 13);
            this.lcStatus.TabIndex = 3;
            this.lcStatus.Text = "labelControl1";
            // 
            // DownloadingBackDropsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 108);
            this.ControlBox = false;
            this.Controls.Add(this.lcStatus);
            this.Controls.Add(this.simpleButton1);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.label1);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DownloadingBackDropsForm";
            this.Text = "Downloading BackDrops";
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.ProgressBarControl pbProgress;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.LabelControl lcStatus;
    }
}