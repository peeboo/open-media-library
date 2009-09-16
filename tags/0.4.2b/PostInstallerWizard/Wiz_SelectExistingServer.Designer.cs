namespace PostInstallerWizard
{
    partial class Wiz_SelectExistingServer
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
            this.teInstance = new System.Windows.Forms.TextBox();
            this.teSAPwd = new System.Windows.Forms.TextBox();
            this.labelSAPwd = new System.Windows.Forms.Label();
            this.labelInstanceName = new System.Windows.Forms.Label();
            this.labelServername = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbServers = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // teInstance
            // 
            this.teInstance.Location = new System.Drawing.Point(100, 60);
            this.teInstance.Name = "teInstance";
            this.teInstance.Size = new System.Drawing.Size(100, 20);
            this.teInstance.TabIndex = 11;
            // 
            // teSAPwd
            // 
            this.teSAPwd.Location = new System.Drawing.Point(100, 85);
            this.teSAPwd.Name = "teSAPwd";
            this.teSAPwd.Size = new System.Drawing.Size(100, 20);
            this.teSAPwd.TabIndex = 10;
            // 
            // labelSAPwd
            // 
            this.labelSAPwd.AutoSize = true;
            this.labelSAPwd.Location = new System.Drawing.Point(21, 88);
            this.labelSAPwd.Name = "labelSAPwd";
            this.labelSAPwd.Size = new System.Drawing.Size(70, 13);
            this.labelSAPwd.TabIndex = 8;
            this.labelSAPwd.Text = "SA Password";
            // 
            // labelInstanceName
            // 
            this.labelInstanceName.AutoSize = true;
            this.labelInstanceName.Location = new System.Drawing.Point(15, 63);
            this.labelInstanceName.Name = "labelInstanceName";
            this.labelInstanceName.Size = new System.Drawing.Size(79, 13);
            this.labelInstanceName.TabIndex = 7;
            this.labelInstanceName.Text = "Instance Name";
            // 
            // labelServername
            // 
            this.labelServername.AutoSize = true;
            this.labelServername.Location = new System.Drawing.Point(25, 37);
            this.labelServername.Name = "labelServername";
            this.labelServername.Size = new System.Drawing.Size(69, 13);
            this.labelServername.TabIndex = 6;
            this.labelServername.Text = "Server Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(261, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Please enter the details of the server you wish to use?";
            // 
            // cbServers
            // 
            this.cbServers.FormattingEnabled = true;
            this.cbServers.Location = new System.Drawing.Point(100, 34);
            this.cbServers.Name = "cbServers";
            this.cbServers.Size = new System.Drawing.Size(100, 21);
            this.cbServers.TabIndex = 13;
            // 
            // Wiz_SelectExistingServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbServers);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.teInstance);
            this.Controls.Add(this.teSAPwd);
            this.Controls.Add(this.labelSAPwd);
            this.Controls.Add(this.labelInstanceName);
            this.Controls.Add(this.labelServername);
            this.Name = "Wiz_SelectExistingServer";
            this.Size = new System.Drawing.Size(300, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSAPwd;
        private System.Windows.Forms.Label labelInstanceName;
        private System.Windows.Forms.Label labelServername;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.ComboBox cbServers;
        public System.Windows.Forms.TextBox teInstance;
        public System.Windows.Forms.TextBox teSAPwd;
    }
}
