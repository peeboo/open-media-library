namespace OMLDatabaseEditor
{
    partial class MetaDataSettings
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbMetadataPlugins = new System.Windows.Forms.ListBox();
            this.grdOptions = new System.Windows.Forms.DataGridView();
            this.colOptionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOptionValue = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdOptions)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbMetadataPlugins);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.btnOk);
            this.splitContainer1.Panel2.Controls.Add(this.grdOptions);
            this.splitContainer1.Size = new System.Drawing.Size(788, 426);
            this.splitContainer1.SplitterDistance = 163;
            this.splitContainer1.TabIndex = 0;
            // 
            // lbMetadataPlugins
            // 
            this.lbMetadataPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbMetadataPlugins.FormattingEnabled = true;
            this.lbMetadataPlugins.Location = new System.Drawing.Point(0, 0);
            this.lbMetadataPlugins.Name = "lbMetadataPlugins";
            this.lbMetadataPlugins.Size = new System.Drawing.Size(160, 381);
            this.lbMetadataPlugins.TabIndex = 0;
            this.lbMetadataPlugins.SelectedIndexChanged += new System.EventHandler(this.lbMetadataPlugins_SelectedIndexChanged);
            // 
            // grdOptions
            // 
            this.grdOptions.AllowUserToAddRows = false;
            this.grdOptions.AllowUserToDeleteRows = false;
            this.grdOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdOptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdOptions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOptionName,
            this.colOptionValue,
            this.colDescription});
            this.grdOptions.Location = new System.Drawing.Point(3, 3);
            this.grdOptions.MultiSelect = false;
            this.grdOptions.Name = "grdOptions";
            this.grdOptions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdOptions.Size = new System.Drawing.Size(618, 379);
            this.grdOptions.TabIndex = 0;
            this.grdOptions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdOptions_CellContentClick);
            // 
            // colOptionName
            // 
            this.colOptionName.HeaderText = "Option";
            this.colOptionName.Name = "colOptionName";
            this.colOptionName.Width = 120;
            // 
            // colOptionValue
            // 
            this.colOptionValue.HeaderText = "Value";
            this.colOptionValue.Name = "colOptionValue";
            this.colOptionValue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colOptionValue.Width = 120;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(234, 391);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(153, 391);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // MetaDataSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 426);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MetaDataSettings";
            this.Text = "Metadata Settings";
            this.Load += new System.EventHandler(this.MetaDataSettings_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdOptions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lbMetadataPlugins;
        private System.Windows.Forms.DataGridView grdOptions;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOptionName;
        private System.Windows.Forms.DataGridViewButtonColumn colOptionValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnOk;
    }
}