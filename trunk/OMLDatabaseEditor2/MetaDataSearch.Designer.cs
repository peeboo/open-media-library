namespace OMLDatabaseEditor
{
    partial class frmSearchResult
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

#pragma warning disable 436 // Remove warning from ResourceManager substitution

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSearchResult));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grdTitles = new System.Windows.Forms.DataGridView();
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCoverArt = new System.Windows.Forms.DataGridViewImageColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSynopsis = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGenres = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDirector = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActors = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSelectMovie = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkUpdateMissingDataOnly = new System.Windows.Forms.CheckBox();
            this.reSearchAdjustTitleLabel = new System.Windows.Forms.Label();
            this.reSearchSubmitButton = new System.Windows.Forms.Button();
            this.reSearchTitle = new System.Windows.Forms.TextBox();
            this.lcProviderMessage = new DevExpress.XtraEditors.LabelControl();
            this.teEpisodeName = new System.Windows.Forms.TextBox();
            this.seSeasonNo = new DevExpress.XtraEditors.SpinEdit();
            this.seEpisodeNo = new DevExpress.XtraEditors.SpinEdit();
            this.lcEpisodeLabel = new DevExpress.XtraEditors.LabelControl();
            this.reSearchSubmitEpisodeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdTitles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seSeasonNo.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEpisodeNo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grdTitles
            // 
            this.grdTitles.AllowUserToAddRows = false;
            this.grdTitles.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.grdTitles, "grdTitles");
            this.grdTitles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdTitles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIndex,
            this.colCoverArt,
            this.colName,
            this.colSynopsis,
            this.colYear,
            this.colGenres,
            this.colDirector,
            this.colActors});
            this.grdTitles.MultiSelect = false;
            this.grdTitles.Name = "grdTitles";
            this.grdTitles.ReadOnly = true;
            this.grdTitles.RowTemplate.Height = 120;
            this.grdTitles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdTitles.DoubleClick += new System.EventHandler(this.btnSelectMovie_Click);
            this.grdTitles.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdTitles_CellContentClick);
            // 
            // colIndex
            // 
            this.colIndex.Frozen = true;
            resources.ApplyResources(this.colIndex, "colIndex");
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            // 
            // colCoverArt
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle8.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle8.NullValue")));
            this.colCoverArt.DefaultCellStyle = dataGridViewCellStyle8;
            resources.ApplyResources(this.colCoverArt, "colCoverArt");
            this.colCoverArt.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.colCoverArt.Name = "colCoverArt";
            this.colCoverArt.ReadOnly = true;
            this.colCoverArt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colName
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colName.DefaultCellStyle = dataGridViewCellStyle9;
            resources.ApplyResources(this.colName, "colName");
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colSynopsis
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colSynopsis.DefaultCellStyle = dataGridViewCellStyle10;
            resources.ApplyResources(this.colSynopsis, "colSynopsis");
            this.colSynopsis.Name = "colSynopsis";
            this.colSynopsis.ReadOnly = true;
            // 
            // colYear
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colYear.DefaultCellStyle = dataGridViewCellStyle11;
            resources.ApplyResources(this.colYear, "colYear");
            this.colYear.Name = "colYear";
            this.colYear.ReadOnly = true;
            // 
            // colGenres
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colGenres.DefaultCellStyle = dataGridViewCellStyle12;
            resources.ApplyResources(this.colGenres, "colGenres");
            this.colGenres.Name = "colGenres";
            this.colGenres.ReadOnly = true;
            // 
            // colDirector
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colDirector.DefaultCellStyle = dataGridViewCellStyle13;
            resources.ApplyResources(this.colDirector, "colDirector");
            this.colDirector.Name = "colDirector";
            this.colDirector.ReadOnly = true;
            // 
            // colActors
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colActors.DefaultCellStyle = dataGridViewCellStyle14;
            resources.ApplyResources(this.colActors, "colActors");
            this.colActors.Name = "colActors";
            this.colActors.ReadOnly = true;
            // 
            // btnSelectMovie
            // 
            resources.ApplyResources(this.btnSelectMovie, "btnSelectMovie");
            this.btnSelectMovie.Name = "btnSelectMovie";
            this.btnSelectMovie.UseVisualStyleBackColor = true;
            this.btnSelectMovie.Click += new System.EventHandler(this.btnSelectMovie_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkUpdateMissingDataOnly
            // 
            resources.ApplyResources(this.chkUpdateMissingDataOnly, "chkUpdateMissingDataOnly");
            this.chkUpdateMissingDataOnly.Checked = true;
            this.chkUpdateMissingDataOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUpdateMissingDataOnly.Name = "chkUpdateMissingDataOnly";
            this.chkUpdateMissingDataOnly.UseVisualStyleBackColor = true;
            // 
            // reSearchAdjustTitleLabel
            // 
            resources.ApplyResources(this.reSearchAdjustTitleLabel, "reSearchAdjustTitleLabel");
            this.reSearchAdjustTitleLabel.Name = "reSearchAdjustTitleLabel";
            // 
            // reSearchSubmitButton
            // 
            resources.ApplyResources(this.reSearchSubmitButton, "reSearchSubmitButton");
            this.reSearchSubmitButton.Name = "reSearchSubmitButton";
            this.reSearchSubmitButton.UseVisualStyleBackColor = true;
            this.reSearchSubmitButton.Click += new System.EventHandler(this.reSearchSubmitButton_Click);
            // 
            // reSearchTitle
            // 
            resources.ApplyResources(this.reSearchTitle, "reSearchTitle");
            this.reSearchTitle.Name = "reSearchTitle";
            this.reSearchTitle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.reSearchTitleKeypress);
            // 
            // lcProviderMessage
            // 
            resources.ApplyResources(this.lcProviderMessage, "lcProviderMessage");
            this.lcProviderMessage.Appearance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lcProviderMessage.Appearance.Options.UseForeColor = true;
            this.lcProviderMessage.Name = "lcProviderMessage";
            this.lcProviderMessage.Click += new System.EventHandler(this.lcProviderMessage_Click);
            // 
            // teEpisodeName
            // 
            resources.ApplyResources(this.teEpisodeName, "teEpisodeName");
            this.teEpisodeName.Name = "teEpisodeName";
            // 
            // seSeasonNo
            // 
            resources.ApplyResources(this.seSeasonNo, "seSeasonNo");
            this.seSeasonNo.Name = "seSeasonNo";
            this.seSeasonNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seSeasonNo.Properties.IsFloatValue = false;
            this.seSeasonNo.Properties.Mask.EditMask = resources.GetString("seSeasonNo.Properties.Mask.EditMask");
            // 
            // seEpisodeNo
            // 
            resources.ApplyResources(this.seEpisodeNo, "seEpisodeNo");
            this.seEpisodeNo.Name = "seEpisodeNo";
            this.seEpisodeNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.seEpisodeNo.Properties.IsFloatValue = false;
            this.seEpisodeNo.Properties.Mask.EditMask = resources.GetString("seEpisodeNo.Properties.Mask.EditMask");
            // 
            // lcEpisodeLabel
            // 
            resources.ApplyResources(this.lcEpisodeLabel, "lcEpisodeLabel");
            this.lcEpisodeLabel.Name = "lcEpisodeLabel";
            // 
            // reSearchSubmitEpisodeButton
            // 
            resources.ApplyResources(this.reSearchSubmitEpisodeButton, "reSearchSubmitEpisodeButton");
            this.reSearchSubmitEpisodeButton.Name = "reSearchSubmitEpisodeButton";
            this.reSearchSubmitEpisodeButton.UseVisualStyleBackColor = true;
            this.reSearchSubmitEpisodeButton.Click += new System.EventHandler(this.reSearchSubmitEpisodeButton_Click);
            // 
            // frmSearchResult
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.reSearchSubmitEpisodeButton);
            this.Controls.Add(this.lcEpisodeLabel);
            this.Controls.Add(this.seEpisodeNo);
            this.Controls.Add(this.seSeasonNo);
            this.Controls.Add(this.teEpisodeName);
            this.Controls.Add(this.lcProviderMessage);
            this.Controls.Add(this.reSearchAdjustTitleLabel);
            this.Controls.Add(this.reSearchSubmitButton);
            this.Controls.Add(this.reSearchTitle);
            this.Controls.Add(this.chkUpdateMissingDataOnly);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelectMovie);
            this.Controls.Add(this.grdTitles);
            this.Name = "frmSearchResult";
            this.Load += new System.EventHandler(this.frmSearchResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdTitles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seSeasonNo.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seEpisodeNo.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

#pragma warning restore 436

        private System.Windows.Forms.DataGridView grdTitles;
        private System.Windows.Forms.Button btnSelectMovie;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewImageColumn colCoverArt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSynopsis;
        private System.Windows.Forms.DataGridViewTextBoxColumn colYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGenres;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDirector;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActors;
        private System.Windows.Forms.CheckBox chkUpdateMissingDataOnly;
        private System.Windows.Forms.Label reSearchAdjustTitleLabel;
        private System.Windows.Forms.Button reSearchSubmitButton;
        private System.Windows.Forms.TextBox reSearchTitle;
        private DevExpress.XtraEditors.LabelControl lcProviderMessage;
        private System.Windows.Forms.TextBox teEpisodeName;
        private DevExpress.XtraEditors.SpinEdit seSeasonNo;
        private DevExpress.XtraEditors.SpinEdit seEpisodeNo;
        private DevExpress.XtraEditors.LabelControl lcEpisodeLabel;
        private System.Windows.Forms.Button reSearchSubmitEpisodeButton;
    }
}