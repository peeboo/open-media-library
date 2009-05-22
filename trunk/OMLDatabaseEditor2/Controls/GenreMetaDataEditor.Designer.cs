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
            this.teGenre = new DevExpress.XtraEditors.TextEdit();
            this.genremetadatasource = new System.Windows.Forms.BindingSource(this.components);
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teGenre.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.genremetadatasource)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.teGenre);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(255, 236);
            this.panelControl1.TabIndex = 0;
            // 
            // teGenre
            // 
            this.teGenre.DataBindings.Add(new System.Windows.Forms.Binding("EditValue", this.genremetadatasource, "Name", true));
            this.teGenre.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.genremetadatasource, "Name", true));
            this.teGenre.Location = new System.Drawing.Point(66, 33);
            this.teGenre.Name = "teGenre";
            this.teGenre.Size = new System.Drawing.Size(100, 20);
            this.teGenre.TabIndex = 2;
            // 
            // genremetadatasource
            // 
            this.genremetadatasource.DataSource = typeof(OMLEngine.GenreMetaData);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(30, 78);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(30, 13);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Image";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(31, 36);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(29, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Genre";
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
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teGenre.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.genremetadatasource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit teGenre;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.BindingSource genremetadatasource;
    }
}
