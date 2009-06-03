namespace OMLDatabaseEditor.Controls
{
    partial class BioDataEditor
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
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.Details = new DevExpress.XtraTab.XtraTabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmsImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.biodatasource = new System.Windows.Forms.BindingSource(this.components);
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtSynposis = new DevExpress.XtraEditors.MemoEdit();
            this.deDateOfBirth = new DevExpress.XtraEditors.DateEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.teNationality = new DevExpress.XtraEditors.TextEdit();
            this.teName = new DevExpress.XtraEditors.TextEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.openCoverFile = new System.Windows.Forms.OpenFileDialog();
            this.miSelectImage = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.Details.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.cmsImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.biodatasource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSynposis.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deDateOfBirth.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deDateOfBirth.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teNationality.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(2, 2);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.Details;
            this.xtraTabControl1.Size = new System.Drawing.Size(488, 313);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.Details});
            // 
            // Details
            // 
            this.Details.Controls.Add(this.pictureBox1);
            this.Details.Controls.Add(this.labelControl2);
            this.Details.Controls.Add(this.txtSynposis);
            this.Details.Controls.Add(this.deDateOfBirth);
            this.Details.Controls.Add(this.labelControl4);
            this.Details.Controls.Add(this.labelControl3);
            this.Details.Controls.Add(this.labelControl1);
            this.Details.Controls.Add(this.teNationality);
            this.Details.Controls.Add(this.teName);
            this.Details.Name = "Details";
            this.Details.Size = new System.Drawing.Size(479, 283);
            this.Details.Text = "Details";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.ContextMenuStrip = this.cmsImage;
            this.pictureBox1.DataBindings.Add(new System.Windows.Forms.Binding("ImageLocation", this.biodatasource, "ImagePath", true));
            this.pictureBox1.ErrorImage = global::OMLDatabaseEditor.Properties.Resources.text_html;
            this.pictureBox1.Image = global::OMLDatabaseEditor.Properties.Resources.text_html;
            this.pictureBox1.InitialImage = global::OMLDatabaseEditor.Properties.Resources.text_html;
            this.pictureBox1.Location = new System.Drawing.Point(15, 237);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(451, 44);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // cmsImage
            // 
            this.cmsImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSelectImage});
            this.cmsImage.Name = "contextMenuStrip1";
            this.cmsImage.Size = new System.Drawing.Size(153, 48);
            // 
            // biodatasource
            // 
            this.biodatasource.DataSource = typeof(OMLEngine.BioData);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(44, 116);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 13);
            this.labelControl2.TabIndex = 10;
            this.labelControl2.Text = "Biography";
            // 
            // txtSynposis
            // 
            this.txtSynposis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSynposis.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.biodatasource, "Biography", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtSynposis.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.biodatasource, "Biography", true));
            this.txtSynposis.Location = new System.Drawing.Point(98, 114);
            this.txtSynposis.Name = "txtSynposis";
            this.txtSynposis.Size = new System.Drawing.Size(378, 103);
            this.txtSynposis.TabIndex = 9;
            this.txtSynposis.TextChanged += new System.EventHandler(this.teBio_TextChanged);
            // 
            // deDateOfBirth
            // 
            this.deDateOfBirth.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.biodatasource, "DateOfBirth", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.deDateOfBirth.DataBindings.Add(new System.Windows.Forms.Binding("DateTime", this.biodatasource, "DateOfBirth", true));
            this.deDateOfBirth.EditValue = null;
            this.deDateOfBirth.Location = new System.Drawing.Point(98, 55);
            this.deDateOfBirth.Name = "deDateOfBirth";
            this.deDateOfBirth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.deDateOfBirth.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.deDateOfBirth.Size = new System.Drawing.Size(100, 20);
            this.deDateOfBirth.TabIndex = 7;
            this.deDateOfBirth.DateTimeChanged += new System.EventHandler(this.teBio_TextChanged);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(41, 87);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(51, 13);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "Nationality";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(31, 58);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(61, 13);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "Date of Birth";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(65, 30);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(27, 13);
            this.labelControl1.TabIndex = 3;
            this.labelControl1.Text = "Name";
            // 
            // teNationality
            // 
            this.teNationality.Location = new System.Drawing.Point(98, 84);
            this.teNationality.Name = "teNationality";
            this.teNationality.Size = new System.Drawing.Size(100, 20);
            this.teNationality.TabIndex = 1;
            this.teNationality.TextChanged += new System.EventHandler(this.teBio_TextChanged);
            // 
            // teName
            // 
            this.teName.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.biodatasource, "FullName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.teName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.biodatasource, "FullName", true));
            this.teName.Location = new System.Drawing.Point(98, 27);
            this.teName.Name = "teName";
            this.teName.Size = new System.Drawing.Size(100, 20);
            this.teName.TabIndex = 0;
            this.teName.TextChanged += new System.EventHandler(this.teBio_TextChanged);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.xtraTabControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(492, 317);
            this.panelControl1.TabIndex = 1;
            // 
            // openCoverFile
            // 
            this.openCoverFile.DefaultExt = "jpg";
            this.openCoverFile.Filter = "JPG Files|*.jpg|All Files|*.*";
            this.openCoverFile.Title = "Select Cover Art";
            // 
            // miSelectImage
            // 
            this.miSelectImage.Name = "miSelectImage";
            this.miSelectImage.Size = new System.Drawing.Size(152, 22);
            this.miSelectImage.Text = "Select Image";
            this.miSelectImage.Click += new System.EventHandler(this.miSelectImage_Click);
            // 
            // BioDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Name = "BioDataEditor";
            this.Size = new System.Drawing.Size(492, 317);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.Details.ResumeLayout(false);
            this.Details.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.cmsImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.biodatasource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSynposis.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deDateOfBirth.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deDateOfBirth.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teNationality.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage Details;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit teNationality;
        private DevExpress.XtraEditors.TextEdit teName;
        private System.Windows.Forms.BindingSource biodatasource;
        private DevExpress.XtraEditors.DateEdit deDateOfBirth;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.MemoEdit txtSynposis;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip cmsImage;
        private System.Windows.Forms.OpenFileDialog openCoverFile;
        private System.Windows.Forms.ToolStripMenuItem miSelectImage;
    }
}
