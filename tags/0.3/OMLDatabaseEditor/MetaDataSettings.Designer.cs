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

#pragma warning disable 436 // Remove warning from ResourceManager substitution

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetaDataSettings));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbMetadataPlugins = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.grdOptions = new System.Windows.Forms.DataGridView();
            this.colOptionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOptionValue = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdOptions)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
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
            // 
            // lbMetadataPlugins
            // 
            resources.ApplyResources(this.lbMetadataPlugins, "lbMetadataPlugins");
            this.lbMetadataPlugins.FormattingEnabled = true;
            this.lbMetadataPlugins.Name = "lbMetadataPlugins";
            this.lbMetadataPlugins.SelectedIndexChanged += new System.EventHandler(this.lbMetadataPlugins_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // grdOptions
            // 
            this.grdOptions.AllowUserToAddRows = false;
            this.grdOptions.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.grdOptions, "grdOptions");
            this.grdOptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdOptions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOptionName,
            this.colOptionValue,
            this.colDescription});
            this.grdOptions.MultiSelect = false;
            this.grdOptions.Name = "grdOptions";
            this.grdOptions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdOptions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdOptions_CellContentClick);
            // 
            // colOptionName
            // 
            resources.ApplyResources(this.colOptionName, "colOptionName");
            this.colOptionName.Name = "colOptionName";
            // 
            // colOptionValue
            // 
            resources.ApplyResources(this.colOptionValue, "colOptionValue");
            this.colOptionValue.Name = "colOptionValue";
            this.colOptionValue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colDescription, "colDescription");
            this.colDescription.Name = "colDescription";
            // 
            // MetaDataSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MetaDataSettings";
            this.Load += new System.EventHandler(this.MetaDataSettings_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdOptions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

#pragma warning restore 436

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