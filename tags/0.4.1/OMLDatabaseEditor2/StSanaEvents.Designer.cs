namespace OMLDatabaseEditor
{
    partial class StSanaEvents
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
            this.StSanaEventLabel = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            // 
            // StSanaEventLabel
            // 
            this.StSanaEventLabel.Location = new System.Drawing.Point(1, 12);
            this.StSanaEventLabel.Name = "StSanaEventLabel";
            this.StSanaEventLabel.Size = new System.Drawing.Size(0, 13);
            this.StSanaEventLabel.TabIndex = 0;
            this.StSanaEventLabel.UseWaitCursor = true;
            // 
            // StSanaEvents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 35);
            this.ControlBox = false;
            this.Controls.Add(this.StSanaEventLabel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "StSanaEvents";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Status";
            this.UseWaitCursor = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevExpress.XtraEditors.LabelControl StSanaEventLabel;



    }
}