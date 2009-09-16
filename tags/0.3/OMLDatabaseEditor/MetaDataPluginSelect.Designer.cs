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
            this.cmbPlugins = new DevExpress.XtraEditors.ComboBoxEdit();
            this.btnSelectPlugin = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.cmbPlugins.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbPlugins
            // 
            this.cmbPlugins.Location = new System.Drawing.Point(12, 12);
            this.cmbPlugins.Name = "cmbPlugins";
            this.cmbPlugins.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbPlugins.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbPlugins.Size = new System.Drawing.Size(230, 20);
            this.cmbPlugins.TabIndex = 1;
            // 
            // btnSelectPlugin
            // 
            this.btnSelectPlugin.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSelectPlugin.Location = new System.Drawing.Point(167, 38);
            this.btnSelectPlugin.Name = "btnSelectPlugin";
            this.btnSelectPlugin.Size = new System.Drawing.Size(75, 23);
            this.btnSelectPlugin.TabIndex = 2;
            this.btnSelectPlugin.Text = "Select Plugin";
            // 
            // MetaDataPluginSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 73);
            this.ControlBox = false;
            this.Controls.Add(this.btnSelectPlugin);
            this.Controls.Add(this.cmbPlugins);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MetaDataPluginSelect";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Select the metadata plugin";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.cmbPlugins.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ComboBoxEdit cmbPlugins;
        private DevExpress.XtraEditors.SimpleButton btnSelectPlugin;
    }
}

