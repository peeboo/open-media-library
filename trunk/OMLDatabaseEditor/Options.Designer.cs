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
            this.ceUseGenreList = new DevExpress.XtraEditors.CheckEdit();
            this.ceUseMPAAList = new DevExpress.XtraEditors.CheckEdit();
            this.tpSkins = new DevExpress.XtraTab.XtraTabPage();
            this.lbcSkins = new DevExpress.XtraEditors.ListBoxControl();
            this.tpMPAAList = new DevExpress.XtraTab.XtraTabPage();
            this.lbcMPAA = new DevExpress.XtraEditors.ListBoxControl();
            this.beMPAA = new DevExpress.XtraEditors.ButtonEdit();
            this.tpGenres = new DevExpress.XtraTab.XtraTabPage();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnGenre = new DevExpress.XtraEditors.ButtonEdit();
            this.lbGenres = new DevExpress.XtraEditors.ListBoxControl();
            this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
            this.sbOK = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.tpOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ceUseGenreList.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceUseMPAAList.Properties)).BeginInit();
            this.tpSkins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbcSkins)).BeginInit();
            this.tpMPAAList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbcMPAA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beMPAA.Properties)).BeginInit();
            this.tpGenres.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnGenre.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbGenres)).BeginInit();
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(361, 336);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // xtraTabControl1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.xtraTabControl1, 3);
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(3, 3);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.tpOptions;
            this.xtraTabControl1.Size = new System.Drawing.Size(355, 301);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tpOptions,
            this.tpSkins,
            this.tpMPAAList,
            this.tpGenres});
            // 
            // tpOptions
            // 
            this.tpOptions.Controls.Add(this.ceUseGenreList);
            this.tpOptions.Controls.Add(this.ceUseMPAAList);
            this.tpOptions.Name = "tpOptions";
            this.tpOptions.Size = new System.Drawing.Size(346, 271);
            this.tpOptions.Text = "Options";
            // 
            // ceUseGenreList
            // 
            this.ceUseGenreList.Location = new System.Drawing.Point(7, 28);
            this.ceUseGenreList.Name = "ceUseGenreList";
            this.ceUseGenreList.Properties.Caption = "Use Genre Auto Complete List";
            this.ceUseGenreList.Size = new System.Drawing.Size(259, 18);
            this.ceUseGenreList.TabIndex = 1;
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
            this.tpSkins.Size = new System.Drawing.Size(346, 270);
            this.tpSkins.Text = "Skins";
            // 
            // lbcSkins
            // 
            this.lbcSkins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbcSkins.Location = new System.Drawing.Point(0, 0);
            this.lbcSkins.Name = "lbcSkins";
            this.lbcSkins.Size = new System.Drawing.Size(346, 270);
            this.lbcSkins.TabIndex = 0;
            this.lbcSkins.SelectedValueChanged += new System.EventHandler(this.lbcSkins_SelectedValueChanged);
            // 
            // tpMPAAList
            // 
            this.tpMPAAList.Controls.Add(this.lbcMPAA);
            this.tpMPAAList.Controls.Add(this.beMPAA);
            this.tpMPAAList.Name = "tpMPAAList";
            this.tpMPAAList.Size = new System.Drawing.Size(346, 270);
            this.tpMPAAList.Text = "MPAA";
            // 
            // lbcMPAA
            // 
            this.lbcMPAA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbcMPAA.Location = new System.Drawing.Point(0, 0);
            this.lbcMPAA.Name = "lbcMPAA";
            this.lbcMPAA.Padding = new System.Windows.Forms.Padding(2);
            this.lbcMPAA.Size = new System.Drawing.Size(346, 250);
            this.lbcMPAA.TabIndex = 1;
            this.lbcMPAA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbcMPAA_KeyDown);
            // 
            // beMPAA
            // 
            this.beMPAA.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.beMPAA.Location = new System.Drawing.Point(0, 250);
            this.beMPAA.Name = "beMPAA";
            this.beMPAA.Padding = new System.Windows.Forms.Padding(2);
            this.beMPAA.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.beMPAA.Size = new System.Drawing.Size(346, 20);
            this.beMPAA.TabIndex = 0;
            this.beMPAA.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beMPAA_ButtonClick);
            // 
            // tpGenres
            // 
            this.tpGenres.Controls.Add(this.labelControl1);
            this.tpGenres.Controls.Add(this.btnGenre);
            this.tpGenres.Controls.Add(this.lbGenres);
            this.tpGenres.Name = "tpGenres";
            this.tpGenres.Size = new System.Drawing.Size(346, 270);
            this.tpGenres.Text = "Genres";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(6, 3);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(206, 13);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Changes to genres take affect immediately";
            // 
            // btnGenre
            // 
            this.btnGenre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenre.Location = new System.Drawing.Point(3, 248);
            this.btnGenre.Name = "btnGenre";
            this.btnGenre.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.btnGenre.Size = new System.Drawing.Size(340, 20);
            this.btnGenre.TabIndex = 6;
            this.btnGenre.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnGenre_ButtonClick);
            // 
            // lbGenres
            // 
            this.lbGenres.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbGenres.HotTrackItems = true;
            this.lbGenres.Location = new System.Drawing.Point(3, 20);
            this.lbGenres.Name = "lbGenres";
            this.lbGenres.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbGenres.Size = new System.Drawing.Size(340, 225);
            this.lbGenres.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.lbGenres.TabIndex = 5;
            this.lbGenres.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbGenres_KeyDown);
            // 
            // sbCancel
            // 
            this.sbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.sbCancel.Location = new System.Drawing.Point(283, 310);
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
            this.sbOK.Location = new System.Drawing.Point(163, 310);
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
            this.ClientSize = new System.Drawing.Size(361, 336);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Options";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.tpOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ceUseGenreList.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceUseMPAAList.Properties)).EndInit();
            this.tpSkins.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbcSkins)).EndInit();
            this.tpMPAAList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbcMPAA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beMPAA.Properties)).EndInit();
            this.tpGenres.ResumeLayout(false);
            this.tpGenres.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnGenre.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbGenres)).EndInit();
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
        private DevExpress.XtraTab.XtraTabPage tpGenres;
        private DevExpress.XtraEditors.ButtonEdit btnGenre;
        private DevExpress.XtraEditors.ListBoxControl lbGenres;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit ceUseGenreList;
    }
}