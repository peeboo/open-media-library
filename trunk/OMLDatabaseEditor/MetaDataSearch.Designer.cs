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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSearchResult));
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
            ((System.ComponentModel.ISupportInitialize)(this.grdTitles)).BeginInit();
            this.SuspendLayout();
            // 
            // grdTitles
            // 
            this.grdTitles.AllowUserToAddRows = false;
            this.grdTitles.AllowUserToDeleteRows = false;
            this.grdTitles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdTitles.ColumnHeadersHeight = 25;
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
            this.grdTitles.Location = new System.Drawing.Point(12, 23);
            this.grdTitles.MultiSelect = false;
            this.grdTitles.Name = "grdTitles";
            this.grdTitles.ReadOnly = true;
            this.grdTitles.RowTemplate.Height = 120;
            this.grdTitles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdTitles.Size = new System.Drawing.Size(888, 479);
            this.grdTitles.TabIndex = 0;
            this.grdTitles.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdTitles_CellContentClick);
            // 
            // colIndex
            // 
            this.colIndex.Frozen = true;
            this.colIndex.HeaderText = "Index";
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            this.colIndex.Visible = false;
            // 
            // colCoverArt
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle8.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle8.NullValue")));
            this.colCoverArt.DefaultCellStyle = dataGridViewCellStyle8;
            this.colCoverArt.HeaderText = "Cover Art";
            this.colCoverArt.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.colCoverArt.Name = "colCoverArt";
            this.colCoverArt.ReadOnly = true;
            this.colCoverArt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colCoverArt.Width = 120;
            // 
            // colName
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colName.DefaultCellStyle = dataGridViewCellStyle9;
            this.colName.HeaderText = "Movie Title";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 150;
            // 
            // colSynopsis
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colSynopsis.DefaultCellStyle = dataGridViewCellStyle10;
            this.colSynopsis.HeaderText = "Synopsis";
            this.colSynopsis.Name = "colSynopsis";
            this.colSynopsis.ReadOnly = true;
            this.colSynopsis.Width = 200;
            // 
            // colYear
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colYear.DefaultCellStyle = dataGridViewCellStyle11;
            this.colYear.HeaderText = "Year";
            this.colYear.Name = "colYear";
            this.colYear.ReadOnly = true;
            this.colYear.Width = 80;
            // 
            // colGenres
            // 
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colGenres.DefaultCellStyle = dataGridViewCellStyle12;
            this.colGenres.HeaderText = "Genres";
            this.colGenres.Name = "colGenres";
            this.colGenres.ReadOnly = true;
            this.colGenres.Width = 150;
            // 
            // colDirector
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colDirector.DefaultCellStyle = dataGridViewCellStyle13;
            this.colDirector.HeaderText = "Director";
            this.colDirector.Name = "colDirector";
            this.colDirector.ReadOnly = true;
            this.colDirector.Width = 120;
            // 
            // colActors
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colActors.DefaultCellStyle = dataGridViewCellStyle14;
            this.colActors.HeaderText = "Actors";
            this.colActors.Name = "colActors";
            this.colActors.ReadOnly = true;
            this.colActors.Width = 150;
            // 
            // btnSelectMovie
            // 
            this.btnSelectMovie.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSelectMovie.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSelectMovie.Location = new System.Drawing.Point(338, 531);
            this.btnSelectMovie.Name = "btnSelectMovie";
            this.btnSelectMovie.Size = new System.Drawing.Size(138, 31);
            this.btnSelectMovie.TabIndex = 1;
            this.btnSelectMovie.Text = "Select Movie";
            this.btnSelectMovie.UseVisualStyleBackColor = true;
            this.btnSelectMovie.Click += new System.EventHandler(this.btnSelectMovie_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(491, 531);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(149, 31);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkUpdateMissingDataOnly
            // 
            this.chkUpdateMissingDataOnly.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.chkUpdateMissingDataOnly.AutoSize = true;
            this.chkUpdateMissingDataOnly.Location = new System.Drawing.Point(338, 508);
            this.chkUpdateMissingDataOnly.Name = "chkUpdateMissingDataOnly";
            this.chkUpdateMissingDataOnly.Size = new System.Drawing.Size(149, 17);
            this.chkUpdateMissingDataOnly.TabIndex = 3;
            this.chkUpdateMissingDataOnly.Text = "Update Missing Data Only";
            this.chkUpdateMissingDataOnly.UseVisualStyleBackColor = true;
            // 
            // frmSearchResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 574);
            this.Controls.Add(this.chkUpdateMissingDataOnly);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSelectMovie);
            this.Controls.Add(this.grdTitles);
            this.Name = "frmSearchResult";
            this.Text = "Search Results";
            this.Load += new System.EventHandler(this.frmSearchResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdTitles)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
    }
}