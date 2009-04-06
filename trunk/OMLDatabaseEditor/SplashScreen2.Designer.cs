namespace OMLDatabaseEditor
{
    partial class SplashScreen2
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
            this.progressBarControl1 = new DevExpress.XtraEditors.ProgressBarControl();
            this.labelControlStatus = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBarControl1
            // 
            this.progressBarControl1.Location = new System.Drawing.Point(12, 345);
            this.progressBarControl1.Name = "progressBarControl1";
            this.progressBarControl1.Size = new System.Drawing.Size(417, 18);
            this.progressBarControl1.TabIndex = 0;
            // 
            // labelControlStatus
            // 
            this.labelControlStatus.Appearance.BackColor = System.Drawing.Color.White;
            this.labelControlStatus.Appearance.Options.UseBackColor = true;
            this.labelControlStatus.Appearance.Options.UseTextOptions = true;
            this.labelControlStatus.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControlStatus.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControlStatus.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.labelControlStatus.Location = new System.Drawing.Point(12, 324);
            this.labelControlStatus.Name = "labelControlStatus";
            this.labelControlStatus.Size = new System.Drawing.Size(417, 18);
            this.labelControlStatus.TabIndex = 1;
            this.labelControlStatus.Text = "Initializing.";
            // 
            // SplashScreen2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Stretch;
            this.BackgroundImageStore = global::OMLDatabaseEditor.Properties.Resources.Application;
            this.ClientSize = new System.Drawing.Size(441, 375);
            this.ControlBox = false;
            this.Controls.Add(this.labelControlStatus);
            this.Controls.Add(this.progressBarControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Open Media Library - Loading";
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControl1.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ProgressBarControl progressBarControl1;
        private DevExpress.XtraEditors.LabelControl labelControlStatus;
    }
}