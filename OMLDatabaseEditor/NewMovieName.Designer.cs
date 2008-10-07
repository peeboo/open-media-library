namespace OMLDatabaseEditor
{
    partial class NewMovieName
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
            this.components = new System.ComponentModel.Container();
            this.kryptonManager = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            this.kryptonPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnOK = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.txtNewMovie = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.btnCancel = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.btnCancel);
            this.kryptonPanel.Controls.Add(this.txtNewMovie);
            this.kryptonPanel.Controls.Add(this.btnOK);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.Size = new System.Drawing.Size(263, 72);
            this.kryptonPanel.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(65, 38);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 25);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Values.ExtraText = "";
            this.btnOK.Values.Image = null;
            this.btnOK.Values.ImageStates.ImageCheckedNormal = null;
            this.btnOK.Values.ImageStates.ImageCheckedPressed = null;
            this.btnOK.Values.ImageStates.ImageCheckedTracking = null;
            this.btnOK.Values.Text = "OK";
            // 
            // txtNewMovie
            // 
            this.txtNewMovie.Location = new System.Drawing.Point(12, 12);
            this.txtNewMovie.Name = "txtNewMovie";
            this.txtNewMovie.Size = new System.Drawing.Size(239, 20);
            this.txtNewMovie.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(161, 38);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 25);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Values.ExtraText = "";
            this.btnCancel.Values.Image = null;
            this.btnCancel.Values.ImageStates.ImageCheckedNormal = null;
            this.btnCancel.Values.ImageStates.ImageCheckedPressed = null;
            this.btnCancel.Values.ImageStates.ImageCheckedTracking = null;
            this.btnCancel.Values.Text = "Cancel";
            // 
            // NewMovieName
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(263, 72);
            this.ControlBox = false;
            this.Controls.Add(this.kryptonPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewMovieName";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Provide new movie name";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            this.kryptonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOK;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtNewMovie;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCancel;
    }
}

