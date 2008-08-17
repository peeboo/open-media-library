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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
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
            this.grdOptions.Location = new System.Drawing.Point(12, 12);
            this.grdOptions.MultiSelect = false;
            this.grdOptions.Name = "grdOptions";
            this.grdOptions.ReadOnly = true;
            this.grdOptions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdOptions.Size = new System.Drawing.Size(402, 307);
            this.grdOptions.TabIndex = 0;
            // 
            // colOptionValue
            // 
            this.colOptionValue.HeaderText = "Option";
            this.colOptionValue.Name = "colOptionValue";
            this.colOptionValue.ReadOnly = true;
            this.colOptionValue.Width = 120;
            // 
            // colOptionDescription
            // 
            this.colOptionDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colOptionDescription.HeaderText = "Description";
            this.colOptionDescription.Name = "colOptionDescription";
            this.colOptionDescription.ReadOnly = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(137, 335);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(218, 335);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // OptionList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 370);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.grdOptions);
            this.Name = "OptionList";
            this.Text = "Options for setting";
            this.Load += new System.EventHandler(this.OptionList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdOptions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdOptions;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOptionValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOptionDescription;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button button1;
    }
}