namespace OMLDatabaseEditor
{
    partial class MetadataSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetadataSelect));
            this.lblTitleProperty = new DevExpress.XtraEditors.LabelControl();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.cbDefault = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.tblData = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.tblMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbDefault.Properties)).BeginInit();
            this.xtraScrollableControl1.SuspendLayout();
            this.tblData.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitleProperty
            // 
            this.lblTitleProperty.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblTitleProperty.Appearance.Options.UseFont = true;
            this.lblTitleProperty.Location = new System.Drawing.Point(3, 3);
            this.lblTitleProperty.Name = "lblTitleProperty";
            this.lblTitleProperty.Size = new System.Drawing.Size(94, 19);
            this.lblTitleProperty.TabIndex = 0;
            this.lblTitleProperty.Text = "labelControl3";
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.xtraScrollableControl1, 0, 2);
            this.tblMain.Controls.Add(this.lblTitleProperty, 0, 0);
            this.tblMain.Controls.Add(this.cbDefault, 0, 3);
            this.tblMain.Controls.Add(this.labelControl3, 0, 1);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 4;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tblMain.Size = new System.Drawing.Size(420, 535);
            this.tblMain.TabIndex = 2;
            // 
            // cbDefault
            // 
            this.cbDefault.EditValue = true;
            this.cbDefault.Location = new System.Drawing.Point(3, 514);
            this.cbDefault.Name = "cbDefault";
            this.cbDefault.Properties.Caption = "Use selected plugin as default source for field";
            this.cbDefault.Size = new System.Drawing.Size(262, 18);
            this.cbDefault.TabIndex = 2;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(3, 31);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(329, 13);
            this.labelControl3.TabIndex = 6;
            this.labelControl3.Text = "Double-click desired value to use it in the title. Bold indicates default.";
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.tblData);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl1.Location = new System.Drawing.Point(3, 51);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(414, 457);
            this.xtraScrollableControl1.TabIndex = 7;
            // 
            // tblData
            // 
            this.tblData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tblData.AutoSize = true;
            this.tblData.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tblData.ColumnCount = 2;
            this.tblData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.84541F));
            this.tblData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.15459F));
            this.tblData.Controls.Add(this.labelControl1, 0, 0);
            this.tblData.Controls.Add(this.labelControl2, 1, 0);
            this.tblData.Location = new System.Drawing.Point(0, 1);
            this.tblData.Name = "tblData";
            this.tblData.RowCount = 1;
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblData.Size = new System.Drawing.Size(414, 21);
            this.tblData.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(4, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(28, 13);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "Plugin";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(111, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(23, 13);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "Data";
            // 
            // MetadataSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 535);
            this.Controls.Add(this.tblMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MetadataSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Desired Metadata Value";
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbDefault.Properties)).EndInit();
            this.xtraScrollableControl1.ResumeLayout(false);
            this.xtraScrollableControl1.PerformLayout();
            this.tblData.ResumeLayout(false);
            this.tblData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblTitleProperty;
        private System.Windows.Forms.TableLayoutPanel tblMain;
        private DevExpress.XtraEditors.CheckEdit cbDefault;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private System.Windows.Forms.TableLayoutPanel tblData;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl3;
    }
}