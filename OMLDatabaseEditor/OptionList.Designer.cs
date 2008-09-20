namespace OMLDatabaseEditor
{
    partial class OptionValues
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionValues));
            this.grdOptions = new System.Windows.Forms.DataGridView();
            this.colOptionValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOptionDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnOk = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdOptions)).BeginInit();
            this.SuspendLayout();
            // 
            // grdOptions
            // 
            this.grdOptions.AllowUserToAddRows = false;
            this.grdOptions.AllowUserToDeleteRows = false;
            this.grdOptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdOptions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colOptionValue,
            this.colOptionDescription});
            resources.ApplyResources(this.grdOptions, "grdOptions");
            this.grdOptions.MultiSelect = false;
            this.grdOptions.Name = "grdOptions";
            this.grdOptions.ReadOnly = true;
            this.grdOptions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // colOptionValue
            // 
            resources.ApplyResources(this.colOptionValue, "colOptionValue");
            this.colOptionValue.Name = "colOptionValue";
            this.colOptionValue.ReadOnly = true;
            // 
            // colOptionDescription
            // 
            this.colOptionDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colOptionDescription, "colOptionDescription");
            this.colOptionDescription.Name = "colOptionDescription";
            this.colOptionDescription.ReadOnly = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // OptionValues
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.grdOptions);
            this.Name = "OptionValues";
            this.Load += new System.EventHandler(this.OptionList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdOptions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

#pragma warning restore 436
        
        private System.Windows.Forms.DataGridView grdOptions;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOptionValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOptionDescription;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button button1;
    }
}