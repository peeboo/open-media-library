namespace PostInstallerWizard
{
    partial class Wiz_SelectServer
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
            this.rbInstall = new System.Windows.Forms.RadioButton();
            this.rbUseExistingInstance = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rbInstall
            // 
            this.rbInstall.AutoSize = true;
            this.rbInstall.Location = new System.Drawing.Point(27, 44);
            this.rbInstall.Name = "rbInstall";
            this.rbInstall.Size = new System.Drawing.Size(76, 17);
            this.rbInstall.TabIndex = 0;
            this.rbInstall.TabStop = true;
            this.rbInstall.Text = "Install SQL";
            this.rbInstall.UseVisualStyleBackColor = true;
            // 
            // rbUseExistingInstance
            // 
            this.rbUseExistingInstance.AutoSize = true;
            this.rbUseExistingInstance.Location = new System.Drawing.Point(27, 77);
            this.rbUseExistingInstance.Name = "rbUseExistingInstance";
            this.rbUseExistingInstance.Size = new System.Drawing.Size(209, 17);
            this.rbUseExistingInstance.TabIndex = 1;
            this.rbUseExistingInstance.TabStop = true;
            this.rbUseExistingInstance.Text = "Use Existing SQL Instance (Advanced)";
            this.rbUseExistingInstance.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please choose SQL server installation?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 3;
            // 
            // Wiz_SelectServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rbUseExistingInstance);
            this.Controls.Add(this.rbInstall);
            this.Name = "Wiz_SelectServer";
            this.Size = new System.Drawing.Size(300, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.RadioButton rbInstall;
        public System.Windows.Forms.RadioButton rbUseExistingInstance;
    }
}
