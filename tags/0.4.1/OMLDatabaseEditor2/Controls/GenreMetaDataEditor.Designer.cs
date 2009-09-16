namespace OMLDatabaseEditor.Controls
{
    partial class GenreMetaDataEditor
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
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.Details = new DevExpress.XtraTab.XtraTabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmsImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miSelectImage = new System.Windows.Forms.ToolStripMenuItem();
            this.teGenre = new DevExpress.XtraEditors.TextEdit();
            this.genremetadatasource = new System.Windows.Forms.BindingSource(this.components);
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.openCoverFile = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.Details.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.cmsImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teGenre.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.genremetadatasource)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.xtraTabControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(255, 236);
            this.panelControl1.TabIndex = 0;
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(2, 2);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.Details;
            this.xtraTabControl1.Size = new System.Drawing.Size(251, 232);
            this.xtraTabControl1.TabIndex = 3;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.Details});
            // 
            // Details
            // 
            this.Details.Controls.Add(this.pictureBox1);
            this.Details.Controls.Add(this.teGenre);
            this.Details.Controls.Add(this.labelControl1);
            this.Details.Name = "Details";
            this.Details.Size = new System.Drawing.Size(242, 201);
            this.Details.Text = "Details";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.ContextMenuStrip = this.cmsImage;
            this.pictureBox1.DataBindings.Add(new System.Windows.Forms.Binding("ImageLocation", this.genremetadatasource, "ImagePath", true));
            this.pictureBox1.ErrorImage = global::OMLDatabaseEditor.Properties.Resources.emblem_symbolic_link;
            this.pictureBox1.Image = global::OMLDatabaseEditor.Properties.Resources.emblem_symbolic_link;
            this.pictureBox1.InitialImage = global::OMLDatabaseEditor.Properties.Resources.emblem_symbolic_link;
            this.pictureBox1.Location = new System.Drawing.Point(13, 64);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(216, 123);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // cmsImage
            // 
            this.cmsImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSelectImage});
            this.cmsImage.Name = "cmsImage";
            this.cmsImage.Size = new System.Drawing.Size(142, 26);
            // 
            // miSelectImage
            // 
            this.miSelectImage.Name = "miSelectImage";
            this.miSelectImage.Size = new System.Drawing.Size(141, 22);
            this.miSelectImage.Text = "Select Image";
            this.miSelectImage.Click += new System.EventHandler(this.miSelectImage_Click);
            // 
            // teGenre
            // 
            this.teGenre.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.genremetadatasource, "Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.teGenre.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.genremetadatasource, "Name", true));
            this.teGenre.Location = new System.Drawing.Point(98, 27);
            this.teGenre.Name = "teGenre";
            this.teGenre.Size = new System.Drawing.Size(100, 20);
            this.teGenre.TabIndex = 2;
            this.teGenre.TextChanged += new System.EventHandler(this.teGenre_TextChanged);
            // 
            // genremetadatasource
            // 
            this.genremetadatasource.DataSource = typeof(OMLEngine.GenreMetaData);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(63, 30);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(29, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Genre";
            // 
            // openCoverFile
            // 
            this.openCoverFile.DefaultExt = "jpg";
            this.openCoverFile.Filter = "JPG Files|*.jpg|All Files|*.*";
            this.openCoverFile.Title = "Select Cover Art";
            // 
            // GenreMetaDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Name = "GenreMetaDataEditor";
            this.Size = new System.Drawing.Size(255, 236);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.Details.ResumeLayout(false);
            this.Details.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.cmsImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.teGenre.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.genremetadatasource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit teGenre;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.BindingSource genremetadatasource;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage Details;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ContextMenuStrip cmsImage;
        private System.Windows.Forms.ToolStripMenuItem miSelectImage;
        private System.Windows.Forms.OpenFileDialog openCoverFile;
    }
}
