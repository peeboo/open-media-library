namespace OMLDatabaseEditor.Controls
{
    partial class DiskEditorCtrl
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
            this.components = new System.ComponentModel.Container();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.txtPath = new DevExpress.XtraEditors.ButtonEdit();
            this.diskSource = new System.Windows.Forms.BindingSource(this.components);
            this.cbDVD = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.diskSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbDVD.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.txtPath);
            this.panelControl1.Controls.Add(this.cbDVD);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.txtName);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(313, 58);
            this.panelControl1.TabIndex = 0;
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.diskSource, "Path", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.diskSource, "Path", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtPath.Location = new System.Drawing.Point(136, 31);
            this.txtPath.Name = "txtPath";
            this.txtPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton("Browse", DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis),
            new DevExpress.XtraEditors.Controls.EditorButton("Menu", DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.txtPath.Size = new System.Drawing.Size(172, 20);
            this.txtPath.TabIndex = 5;
            this.txtPath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtPath_ButtonClick);
            this.txtPath.Leave += new System.EventHandler(this.txtPath_Leave);
            // 
            // diskSource
            // 
            this.diskSource.AllowNew = false;
            this.diskSource.DataSource = typeof(OMLEngine.Disk);
            this.diskSource.DataSourceChanged += new System.EventHandler(this.diskSource_DataSourceChanged);
            // 
            // cbDVD
            // 
            this.cbDVD.Location = new System.Drawing.Point(87, 31);
            this.cbDVD.Name = "cbDVD";
            this.cbDVD.Properties.Caption = "DVD";
            this.cbDVD.Size = new System.Drawing.Size(42, 18);
            this.cbDVD.TabIndex = 4;
            this.cbDVD.CheckedChanged += new System.EventHandler(this.cbDVD_CheckedChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(30, 34);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(22, 13);
            this.labelControl2.TabIndex = 3;
            this.labelControl2.Text = "Path";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(30, 8);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(27, 13);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.diskSource, "Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.diskSource, "Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtName.Location = new System.Drawing.Point(89, 5);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(219, 20);
            this.txtName.TabIndex = 0;
            // 
            // folderDialog
            // 
            this.folderDialog.ShowNewFolderButton = false;
            // 
            // DiskEditorCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Name = "DiskEditorCtrl";
            this.Size = new System.Drawing.Size(313, 58);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.diskSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbDVD.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private System.Windows.Forms.BindingSource diskSource;
        private DevExpress.XtraEditors.ButtonEdit txtPath;
        private DevExpress.XtraEditors.CheckEdit cbDVD;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
    }
}
