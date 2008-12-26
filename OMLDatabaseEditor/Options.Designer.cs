namespace OMLDatabaseEditor
{
    partial class Options
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.tpSkins = new DevExpress.XtraTab.XtraTabPage();
            this.lbcSkins = new DevExpress.XtraEditors.ListBoxControl();
            this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
            this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
            this.sbOK = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.tpSkins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbcSkins)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tableLayoutPanel1.Controls.Add(this.xtraTabControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sbCancel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.sbOK, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 264);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // xtraTabControl1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.xtraTabControl1, 3);
            this.xtraTabControl1.Location = new System.Drawing.Point(3, 3);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.tpSkins;
            this.xtraTabControl1.Size = new System.Drawing.Size(278, 229);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpSkins,
            this.xtraTabPage2});
            // 
            // tpSkins
            // 
            this.tpSkins.Controls.Add(this.lbcSkins);
            this.tpSkins.Name = "tpSkins";
            this.tpSkins.Size = new System.Drawing.Size(269, 199);
            this.tpSkins.Text = "Skins";
            // 
            // lbcSkins
            // 
            this.lbcSkins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbcSkins.Location = new System.Drawing.Point(0, 0);
            this.lbcSkins.Name = "lbcSkins";
            this.lbcSkins.Size = new System.Drawing.Size(269, 199);
            this.lbcSkins.TabIndex = 0;
            this.lbcSkins.SelectedValueChanged += new System.EventHandler(this.lbcSkins_SelectedValueChanged);
            // 
            // xtraTabPage2
            // 
            this.xtraTabPage2.Name = "xtraTabPage2";
            this.xtraTabPage2.PageVisible = false;
            this.xtraTabPage2.Size = new System.Drawing.Size(269, 199);
            this.xtraTabPage2.Text = "xtraTabPage2";
            // 
            // sbCancel
            // 
            this.sbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.sbCancel.Location = new System.Drawing.Point(206, 238);
            this.sbCancel.Name = "sbCancel";
            this.sbCancel.Size = new System.Drawing.Size(75, 23);
            this.sbCancel.TabIndex = 1;
            this.sbCancel.Text = "&Cancel";
            this.sbCancel.Click += new System.EventHandler(this.SimpleButtonClick);
            // 
            // sbOK
            // 
            this.sbOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sbOK.Location = new System.Drawing.Point(111, 238);
            this.sbOK.Name = "sbOK";
            this.sbOK.Size = new System.Drawing.Size(75, 23);
            this.sbOK.TabIndex = 2;
            this.sbOK.Text = "&OK";
            this.sbOK.Click += new System.EventHandler(this.SimpleButtonClick);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Options";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.tpSkins.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbcSkins)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage tpSkins;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
        private DevExpress.XtraEditors.ListBoxControl lbcSkins;
        private DevExpress.XtraEditors.SimpleButton sbCancel;
        private DevExpress.XtraEditors.SimpleButton sbOK;
    }
}