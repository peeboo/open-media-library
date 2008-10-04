namespace OMLDatabaseEditor
{
    partial class MetaDataPluginSelect
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
            this.cmbPlugins = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.btnSelectPlugin = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).BeginInit();
            this.kryptonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel
            // 
            this.kryptonPanel.Controls.Add(this.btnSelectPlugin);
            this.kryptonPanel.Controls.Add(this.cmbPlugins);
            this.kryptonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel.Name = "kryptonPanel";
            this.kryptonPanel.Size = new System.Drawing.Size(263, 72);
            this.kryptonPanel.TabIndex = 0;
            // 
            // cmbPlugins
            // 
            this.cmbPlugins.DisplayMember = "PluginName";
            this.cmbPlugins.DropDownWidth = 241;
            this.cmbPlugins.FormattingEnabled = false;
            this.cmbPlugins.Location = new System.Drawing.Point(12, 12);
            this.cmbPlugins.Name = "cmbPlugins";
            this.cmbPlugins.Size = new System.Drawing.Size(241, 21);
            this.cmbPlugins.TabIndex = 0;
            // 
            // btnSelectPlugin
            // 
            this.btnSelectPlugin.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSelectPlugin.Location = new System.Drawing.Point(161, 39);
            this.btnSelectPlugin.Name = "btnSelectPlugin";
            this.btnSelectPlugin.Size = new System.Drawing.Size(90, 25);
            this.btnSelectPlugin.TabIndex = 1;
            this.btnSelectPlugin.Text = "Select Plugin";
            this.btnSelectPlugin.Values.ExtraText = "";
            this.btnSelectPlugin.Values.Image = null;
            this.btnSelectPlugin.Values.ImageStates.ImageCheckedNormal = null;
            this.btnSelectPlugin.Values.ImageStates.ImageCheckedPressed = null;
            this.btnSelectPlugin.Values.ImageStates.ImageCheckedTracking = null;
            this.btnSelectPlugin.Values.Text = "Select Plugin";
            // 
            // MetadataPluginSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 72);
            this.ControlBox = false;
            this.Controls.Add(this.kryptonPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MetadataPluginSelect";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Select the metadata plugin";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel)).EndInit();
            this.kryptonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonManager kryptonManager;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox cmbPlugins;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSelectPlugin;
    }
}

