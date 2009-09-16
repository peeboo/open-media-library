namespace OMLDatabaseEditor.Controls
{
    partial class TitlesListView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TitlesListView));
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
            ((System.ComponentModel.ISupportInitialize)(this.grdTitles)).BeginInit();
            this.SuspendLayout();
            // 
            // grdTitles
            // 
            this.grdTitles.AllowUserToAddRows = false;
            this.grdTitles.AllowUserToDeleteRows = false;
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
            this.grdTitles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdTitles.Location = new System.Drawing.Point(0, 0);
            this.grdTitles.MultiSelect = false;
            this.grdTitles.Name = "grdTitles";
            this.grdTitles.ReadOnly = true;
            this.grdTitles.RowTemplate.Height = 120;
            this.grdTitles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdTitles.Size = new System.Drawing.Size(491, 397);
            this.grdTitles.TabIndex = 1;
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle1.NullValue")));
            this.colCoverArt.DefaultCellStyle = dataGridViewCellStyle1;
            this.colCoverArt.HeaderText = "Cover Art";
            this.colCoverArt.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.colCoverArt.Name = "colCoverArt";
            this.colCoverArt.ReadOnly = true;
            this.colCoverArt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colCoverArt.Width = 120;
            // 
            // colName
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colName.DefaultCellStyle = dataGridViewCellStyle2;
            this.colName.HeaderText = "Movie Title";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 150;
            // 
            // colSynopsis
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colSynopsis.DefaultCellStyle = dataGridViewCellStyle3;
            this.colSynopsis.HeaderText = "Synopsis";
            this.colSynopsis.Name = "colSynopsis";
            this.colSynopsis.ReadOnly = true;
            this.colSynopsis.Width = 200;
            // 
            // colYear
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colYear.DefaultCellStyle = dataGridViewCellStyle4;
            this.colYear.HeaderText = "Year";
            this.colYear.Name = "colYear";
            this.colYear.ReadOnly = true;
            this.colYear.Width = 80;
            // 
            // colGenres
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colGenres.DefaultCellStyle = dataGridViewCellStyle5;
            this.colGenres.HeaderText = "Genres";
            this.colGenres.Name = "colGenres";
            this.colGenres.ReadOnly = true;
            this.colGenres.Width = 150;
            // 
            // colDirector
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colDirector.DefaultCellStyle = dataGridViewCellStyle6;
            this.colDirector.HeaderText = "Director";
            this.colDirector.Name = "colDirector";
            this.colDirector.ReadOnly = true;
            this.colDirector.Width = 120;
            // 
            // colActors
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.colActors.DefaultCellStyle = dataGridViewCellStyle7;
            this.colActors.HeaderText = "Actors";
            this.colActors.Name = "colActors";
            this.colActors.ReadOnly = true;
            this.colActors.Width = 150;
            // 
            // TitlesListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grdTitles);
            this.Name = "TitlesListView";
            this.Size = new System.Drawing.Size(491, 397);
            ((System.ComponentModel.ISupportInitialize)(this.grdTitles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdTitles;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewImageColumn colCoverArt;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSynopsis;
        private System.Windows.Forms.DataGridViewTextBoxColumn colYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGenres;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDirector;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActors;
    }
}
