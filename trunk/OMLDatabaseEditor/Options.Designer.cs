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
            this.tpOptions = new DevExpress.XtraTab.XtraTabPage();
            this.ceUseMPAAList = new DevExpress.XtraEditors.CheckEdit();
            this.tpSkins = new DevExpress.XtraTab.XtraTabPage();
            this.lbcSkins = new DevExpress.XtraEditors.ListBoxControl();
            this.tpMPAAList = new DevExpress.XtraTab.XtraTabPage();
            this.lbcMPAA = new DevExpress.XtraEditors.ListBoxControl();
            this.beMPAA = new DevExpress.XtraEditors.ButtonEdit();
            this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
            this.sbOK = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.tpOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ceUseMPAAList.Properties)).BeginInit();
            this.tpSkins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbcSkins)).BeginInit();
            this.tpMPAAList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbcMPAA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beMPAA.Properties)).BeginInit();
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
            this.xtraTabControl1.SelectedTabPage = this.tpOptions;
            this.xtraTabControl1.Size = new System.Drawing.Size(278, 229);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpOptions,
            this.tpSkins,
            this.tpMPAAList});
            // 
            // tpOptions
            // 
            this.tpOptions.Controls.Add(this.ceUseMPAAList);
            this.tpOptions.Name = "tpOptions";
            this.tpOptions.Size = new System.Drawing.Size(269, 199);
            this.tpOptions.Text = "Options";
            // 
            // ceUseMPAAList
            // 
            this.ceUseMPAAList.Location = new System.Drawing.Point(7, 4);
            this.ceUseMPAAList.Name = "ceUseMPAAList";
            this.ceUseMPAAList.Properties.Caption = "Use MPAA Auto Complete List";
            this.ceUseMPAAList.Size = new System.Drawing.Size(259, 18);
            this.ceUseMPAAList.TabIndex = 0;
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
            // tpMPAAList
            // 
            this.tpMPAAList.Controls.Add(this.lbcMPAA);
            this.tpMPAAList.Controls.Add(this.beMPAA);
            this.tpMPAAList.Name = "tpMPAAList";
            this.tpMPAAList.Size = new System.Drawing.Size(269, 199);
            this.tpMPAAList.Text = "MPAA";
            // 
            // lbcMPAA
            // 
            this.lbcMPAA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbcMPAA.Location = new System.Drawing.Point(0, 0);
            this.lbcMPAA.Name = "lbcMPAA";
            this.lbcMPAA.Padding = new System.Windows.Forms.Padding(2);
            this.lbcMPAA.Size = new System.Drawing.Size(269, 179);
            this.lbcMPAA.TabIndex = 1;
            this.lbcMPAA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbcMPAA_KeyDown);
            // 
            // beMPAA
            // 
            this.beMPAA.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.beMPAA.Location = new System.Drawing.Point(0, 179);
            this.beMPAA.Name = "beMPAA";
            this.beMPAA.Padding = new System.Windows.Forms.Padding(2);
            this.beMPAA.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.beMPAA.Size = new System.Drawing.Size(269, 20);
            this.beMPAA.TabIndex = 0;
            this.beMPAA.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beMPAA_ButtonClick);
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
            this.tpOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ceUseMPAAList.Properties)).EndInit();
            this.tpSkins.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbcSkins)).EndInit();
            this.tpMPAAList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbcMPAA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beMPAA.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage tpSkins;
        private DevExpress.XtraTab.XtraTabPage tpOptions;
        private DevExpress.XtraEditors.ListBoxControl lbcSkins;
        private DevExpress.XtraEditors.SimpleButton sbCancel;
        private DevExpress.XtraEditors.SimpleButton sbOK;
        private DevExpress.XtraEditors.CheckEdit ceUseMPAAList;
        private DevExpress.XtraTab.XtraTabPage tpMPAAList;
        private DevExpress.XtraEditors.ListBoxControl lbcMPAA;
        private DevExpress.XtraEditors.ButtonEdit beMPAA;
    }
}