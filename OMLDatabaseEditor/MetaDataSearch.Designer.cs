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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
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
            ((System.ComponentModel.ISupportInitialize)(this.grdTitles)).BeginInit();
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            this.colCoverArt.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.colCoverArt, "colCoverArt");
            this.colCoverArt.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.colCoverArt.Name = "colCoverArt";
            this.colCoverArt.ReadOnly = true;
            this.colCoverArt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colName
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colName.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.colName, "colName");
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colSynopsis
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colSynopsis.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.colSynopsis, "colSynopsis");
            this.colSynopsis.Name = "colSynopsis";
            this.colSynopsis.ReadOnly = true;
            // 
            // colYear
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colYear.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.colYear, "colYear");
            this.colYear.Name = "colYear";
            this.colYear.ReadOnly = true;
            // 
            // colGenres
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colGenres.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.colGenres, "colGenres");
            this.colGenres.Name = "colGenres";
            this.colGenres.ReadOnly = true;
            // 
            // colDirector
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colDirector.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.colDirector, "colDirector");
            this.colDirector.Name = "colDirector";
            this.colDirector.ReadOnly = true;
            // 
            // colActors
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colActors.DefaultCellStyle = dataGridViewCellStyle7;
            resources.ApplyResources(this.colActors, "colActors");
            this.colActors.Name = "colActors";
            this.colActors.ReadOnly = true;
            // 
            // btnSelectMovie
            // 
            resources.ApplyResources(this.btnSelectMovie, "btnSelectMovie");
            this.btnSelectMovie.DialogResult = System.Windows.Forms.DialogResult.OK;
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
            // frmSearchResult
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
    }
}