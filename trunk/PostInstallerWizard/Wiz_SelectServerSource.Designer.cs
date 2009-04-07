namespace PostInstallerWizard
{
    partial class Wiz_SelectServerSource
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbSQLInstallationFile = new System.Windows.Forms.TextBox();
            this.rbDownloadSQLInstaller = new System.Windows.Forms.RadioButton();
            this.rbSetupFileLocal = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Installation file";
            // 
            // tbSQLInstallationFile
            // 
            this.tbSQLInstallationFile.Location = new System.Drawing.Point(140, 92);
            this.tbSQLInstallationFile.Name = "tbSQLInstallationFile";
            this.tbSQLInstallationFile.Size = new System.Drawing.Size(100, 20);
            this.tbSQLInstallationFile.TabIndex = 3;
            // 
            // rbDownloadSQLInstaller
            // 
            this.rbDownloadSQLInstaller.AutoSize = true;
            this.rbDownloadSQLInstaller.Location = new System.Drawing.Point(27, 29);
            this.rbDownloadSQLInstaller.Name = "rbDownloadSQLInstaller";
            this.rbDownloadSQLInstaller.Size = new System.Drawing.Size(142, 17);
            this.rbDownloadSQLInstaller.TabIndex = 4;
            this.rbDownloadSQLInstaller.TabStop = true;
            this.rbDownloadSQLInstaller.Text = "Download from Microsoft";
            this.rbDownloadSQLInstaller.UseVisualStyleBackColor = true;
            // 
            // rbSetupFileLocal
            // 
            this.rbSetupFileLocal.AutoSize = true;
            this.rbSetupFileLocal.Location = new System.Drawing.Point(27, 62);
            this.rbSetupFileLocal.Name = "rbSetupFileLocal";
            this.rbSetupFileLocal.Size = new System.Drawing.Size(100, 17);
            this.rbSetupFileLocal.TabIndex = 5;
            this.rbSetupFileLocal.TabStop = true;
            this.rbSetupFileLocal.Text = "Select setup file";
            this.rbSetupFileLocal.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(169, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Where is the SQL Setup program?";
            // 
            // Wiz_SelectServerSource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rbSetupFileLocal);
            this.Controls.Add(this.rbDownloadSQLInstaller);
            this.Controls.Add(this.tbSQLInstallationFile);
            this.Controls.Add(this.label1);
            this.Name = "Wiz_SelectServerSource";
            this.Size = new System.Drawing.Size(300, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox tbSQLInstallationFile;
        public System.Windows.Forms.RadioButton rbDownloadSQLInstaller;
        public System.Windows.Forms.RadioButton rbSetupFileLocal;
    }
}
