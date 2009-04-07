namespace PostInstallerWizard
{
    partial class Wiz_Start
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
            this.label2 = new System.Windows.Forms.Label();
            this.cbAdvanced = new System.Windows.Forms.CheckBox();
            this.lMessage1 = new System.Windows.Forms.Label();
            this.lMessage2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Open Media Library is now installed.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(227, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "There is a few more steps to go through before";
            // 
            // cbAdvanced
            // 
            this.cbAdvanced.AutoSize = true;
            this.cbAdvanced.Location = new System.Drawing.Point(6, 129);
            this.cbAdvanced.Name = "cbAdvanced";
            this.cbAdvanced.Size = new System.Drawing.Size(75, 17);
            this.cbAdvanced.TabIndex = 2;
            this.cbAdvanced.Text = "Advanced";
            this.cbAdvanced.UseVisualStyleBackColor = true;
            // 
            // lMessage1
            // 
            this.lMessage1.AutoSize = true;
            this.lMessage1.Location = new System.Drawing.Point(3, 70);
            this.lMessage1.Name = "lMessage1";
            this.lMessage1.Size = new System.Drawing.Size(35, 13);
            this.lMessage1.TabIndex = 3;
            this.lMessage1.Text = "label3";
            // 
            // lMessage2
            // 
            this.lMessage2.AutoSize = true;
            this.lMessage2.Location = new System.Drawing.Point(3, 94);
            this.lMessage2.Name = "lMessage2";
            this.lMessage2.Size = new System.Drawing.Size(35, 13);
            this.lMessage2.TabIndex = 4;
            this.lMessage2.Text = "label3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(45, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "you can use this software";
            // 
            // Wiz_Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lMessage2);
            this.Controls.Add(this.lMessage1);
            this.Controls.Add(this.cbAdvanced);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Wiz_Start";
            this.Size = new System.Drawing.Size(300, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.CheckBox cbAdvanced;
        public System.Windows.Forms.Label lMessage1;
        public System.Windows.Forms.Label lMessage2;
        private System.Windows.Forms.Label label3;
    }
}
