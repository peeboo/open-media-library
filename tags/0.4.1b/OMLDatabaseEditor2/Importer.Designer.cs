namespace OMLDatabaseEditor
{
    partial class Importer
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
            this.lbStatus = new DevExpress.XtraEditors.LabelControl();
            this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
            this.sbOK = new DevExpress.XtraEditors.SimpleButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.pbProgress = new DevExpress.XtraEditors.ProgressBarControl();
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lbStatus
            // 
            this.lbStatus.Location = new System.Drawing.Point(22, 21);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(63, 13);
            this.lbStatus.TabIndex = 0;
            this.lbStatus.Text = "labelControl1";
            // 
            // sbCancel
            // 
            this.sbCancel.Location = new System.Drawing.Point(145, 75);
            this.sbCancel.Name = "sbCancel";
            this.sbCancel.Size = new System.Drawing.Size(75, 23);
            this.sbCancel.TabIndex = 1;
            this.sbCancel.Text = "Cancel";
            this.sbCancel.Click += new System.EventHandler(this.sbCancel_Click);
            // 
            // sbOK
            // 
            this.sbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sbOK.Location = new System.Drawing.Point(64, 75);
            this.sbOK.Name = "sbOK";
            this.sbOK.Size = new System.Drawing.Size(75, 23);
            this.sbOK.TabIndex = 2;
            this.sbOK.Text = "OK";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(12, 51);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(260, 18);
            this.pbProgress.TabIndex = 3;
            // 
            // Importer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 112);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.sbOK);
            this.Controls.Add(this.sbCancel);
            this.Controls.Add(this.lbStatus);
            this.Name = "Importer";
            this.Text = "Importer";
            ((System.ComponentModel.ISupportInitialize)(this.pbProgress.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lbStatus;
        private DevExpress.XtraEditors.SimpleButton sbCancel;
        private DevExpress.XtraEditors.SimpleButton sbOK;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private DevExpress.XtraEditors.ProgressBarControl pbProgress;
    }
}