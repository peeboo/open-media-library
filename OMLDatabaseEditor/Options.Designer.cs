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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.tpOptions = new DevExpress.XtraTab.XtraTabPage();
            this.ceAutoScanDiskOnAdd = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.beTitledFanArtPath = new DevExpress.XtraEditors.ButtonEdit();
            this.ceTitledFanArtFolder = new DevExpress.XtraEditors.CheckEdit();
            this.cmbDefaultMetadataPlugin = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.cePrependParentFolder = new DevExpress.XtraEditors.CheckEdit();
            this.ceUseGenreList = new DevExpress.XtraEditors.CheckEdit();
            this.ceUseMPAAList = new DevExpress.XtraEditors.CheckEdit();
            this.ceFoldersAsTitles = new DevExpress.XtraEditors.CheckEdit();
            this.tpSkins = new DevExpress.XtraTab.XtraTabPage();
            this.lbcSkins = new DevExpress.XtraEditors.ListBoxControl();
            this.tpMPAAList = new DevExpress.XtraTab.XtraTabPage();
            this.lbcMPAA = new DevExpress.XtraEditors.ListBoxControl();
            this.beMPAA = new DevExpress.XtraEditors.ButtonEdit();
            this.tpGenres = new DevExpress.XtraTab.XtraTabPage();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnGenre = new DevExpress.XtraEditors.ButtonEdit();
            this.lbGenres = new DevExpress.XtraEditors.ListBoxControl();
            this.tpTags = new DevExpress.XtraTab.XtraTabPage();
            this.lbcTags = new DevExpress.XtraEditors.ListBoxControl();
            this.beTags = new DevExpress.XtraEditors.ButtonEdit();
            this.tpMountTools = new DevExpress.XtraTab.XtraTabPage();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.teMntToolPath = new DevExpress.XtraEditors.TextEdit();
            this.cmbMntToolVDrive = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cmbMntToolScan = new DevExpress.XtraEditors.ComboBoxEdit();
            this.simpleButtonScanMntTool = new DevExpress.XtraEditors.SimpleButton();
            this.rgMountingTool = new DevExpress.XtraEditors.RadioGroup();
            this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
            this.sbOK = new DevExpress.XtraEditors.SimpleButton();
            this.cmGenreMappings = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ceScanDiskRollInfoToTitle = new DevExpress.XtraEditors.CheckEdit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.tpOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ceAutoScanDiskOnAdd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beTitledFanArtPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceTitledFanArtFolder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDefaultMetadataPlugin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cePrependParentFolder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceUseGenreList.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceUseMPAAList.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceFoldersAsTitles.Properties)).BeginInit();
            this.tpSkins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbcSkins)).BeginInit();
            this.tpMPAAList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbcMPAA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beMPAA.Properties)).BeginInit();
            this.tpGenres.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnGenre.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbGenres)).BeginInit();
            this.tpTags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbcTags)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.beTags.Properties)).BeginInit();
            this.tpMountTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teMntToolPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMntToolVDrive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMntToolScan.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgMountingTool.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceScanDiskRollInfoToTitle.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
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
            this.tpGenres,
            this.tpTags,
            this.tpMountTools});
            // 
            // tpOptions
            // 
            this.tpOptions.Controls.Add(this.ceScanDiskRollInfoToTitle);
            this.tpOptions.Controls.Add(this.ceAutoScanDiskOnAdd);
            this.tpOptions.Controls.Add(this.labelControl3);
            this.tpOptions.Controls.Add(this.beTitledFanArtPath);
            this.tpOptions.Controls.Add(this.ceTitledFanArtFolder);
            this.tpOptions.Controls.Add(this.cmbDefaultMetadataPlugin);
            this.tpOptions.Controls.Add(this.labelControl2);
            this.tpOptions.Controls.Add(this.cePrependParentFolder);
            this.tpOptions.Controls.Add(this.ceUseGenreList);
            this.tpOptions.Controls.Add(this.ceUseMPAAList);
            this.tpOptions.Controls.Add(this.ceFoldersAsTitles);
            this.tpOptions.Name = "tpOptions";
            this.tpOptions.Size = new System.Drawing.Size(346, 270);
            this.tpOptions.Text = "Options";
            // 
            // ceAutoScanDiskOnAdd
            // 
            this.ceAutoScanDiskOnAdd.Location = new System.Drawing.Point(7, 187);
            this.ceAutoScanDiskOnAdd.Name = "ceAutoScanDiskOnAdd";
            this.ceAutoScanDiskOnAdd.Properties.Caption = "Auto scan disk when adding new disks";
            this.ceAutoScanDiskOnAdd.Size = new System.Drawing.Size(259, 19);
            this.ceAutoScanDiskOnAdd.TabIndex = 9;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(9, 155);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(91, 13);
            this.labelControl3.TabIndex = 8;
            this.labelControl3.Text = "Titled FanArt Path:";
            // 
            // beTitledFanArtPath
            // 
            this.beTitledFanArtPath.Enabled = false;
            this.beTitledFanArtPath.Location = new System.Drawing.Point(106, 152);
            this.beTitledFanArtPath.Name = "beTitledFanArtPath";
            this.beTitledFanArtPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.beTitledFanArtPath.Size = new System.Drawing.Size(204, 20);
            this.beTitledFanArtPath.TabIndex = 7;
            this.beTitledFanArtPath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beTitledFanArtPath_ButtonClick);
            // 
            // ceTitledFanArtFolder
            // 
            this.ceTitledFanArtFolder.Location = new System.Drawing.Point(7, 127);
            this.ceTitledFanArtFolder.Name = "ceTitledFanArtFolder";
            this.ceTitledFanArtFolder.Properties.Caption = "Titled FanArt Folders";
            this.ceTitledFanArtFolder.Size = new System.Drawing.Size(259, 19);
            this.ceTitledFanArtFolder.TabIndex = 6;
            this.ceTitledFanArtFolder.CheckStateChanged += new System.EventHandler(this.ceTitledFanArtFolder_CheckStateChanged);
            // 
            // cmbDefaultMetadataPlugin
            // 
            this.cmbDefaultMetadataPlugin.Location = new System.Drawing.Point(135, 101);
            this.cmbDefaultMetadataPlugin.Name = "cmbDefaultMetadataPlugin";
            this.cmbDefaultMetadataPlugin.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cmbDefaultMetadataPlugin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbDefaultMetadataPlugin.Properties.NullText = "Not Specified";
            this.cmbDefaultMetadataPlugin.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbDefaultMetadataPlugin.Size = new System.Drawing.Size(175, 20);
            this.cmbDefaultMetadataPlugin.TabIndex = 5;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(9, 104);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(119, 13);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Default Metadata Plugin:";
            // 
            // cePrependParentFolder
            // 
            this.cePrependParentFolder.Location = new System.Drawing.Point(7, 76);
            this.cePrependParentFolder.Name = "cePrependParentFolder";
            this.cePrependParentFolder.Properties.Caption = "Prepend Parent Folder to Title on folder scan";
            this.cePrependParentFolder.Size = new System.Drawing.Size(259, 19);
            this.cePrependParentFolder.TabIndex = 3;
            // 
            // ceUseGenreList
            // 
            this.ceUseGenreList.Location = new System.Drawing.Point(7, 28);
            this.ceUseGenreList.Name = "ceUseGenreList";
            this.ceUseGenreList.Properties.Caption = "Use Genre Auto Complete List";
            this.ceUseGenreList.Size = new System.Drawing.Size(259, 19);
            this.ceUseGenreList.TabIndex = 1;
            // 
            // ceUseMPAAList
            // 
            this.ceUseMPAAList.Location = new System.Drawing.Point(7, 4);
            this.ceUseMPAAList.Name = "ceUseMPAAList";
            this.ceUseMPAAList.Properties.Caption = "Use MPAA Auto Complete List";
            this.ceUseMPAAList.Size = new System.Drawing.Size(259, 19);
            this.ceUseMPAAList.TabIndex = 0;
            // 
            // ceFoldersAsTitles
            // 
            this.ceFoldersAsTitles.Location = new System.Drawing.Point(7, 52);
            this.ceFoldersAsTitles.Name = "ceFoldersAsTitles";
            this.ceFoldersAsTitles.Properties.Caption = "Folders are Titles";
            this.ceFoldersAsTitles.Size = new System.Drawing.Size(259, 19);
            this.ceFoldersAsTitles.TabIndex = 2;
            this.ceFoldersAsTitles.CheckStateChanged += new System.EventHandler(this.ceFoldersAsTitles_CheckStateChanged);
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
            this.lbcMPAA.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbcMPAA.Location = new System.Drawing.Point(0, 0);
            this.lbcMPAA.Name = "lbcMPAA";
            this.lbcMPAA.Padding = new System.Windows.Forms.Padding(2);
            this.lbcMPAA.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbcMPAA.Size = new System.Drawing.Size(346, 245);
            this.lbcMPAA.TabIndex = 1;
            this.lbcMPAA.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbcMPAA_KeyDown);
            // 
            // beMPAA
            // 
            this.beMPAA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.beMPAA.Location = new System.Drawing.Point(0, 251);
            this.beMPAA.Name = "beMPAA";
            this.beMPAA.Padding = new System.Windows.Forms.Padding(2);
            this.beMPAA.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.beMPAA.Size = new System.Drawing.Size(346, 20);
            this.beMPAA.TabIndex = 0;
            this.beMPAA.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beOptions_ButtonClick);
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
            this.labelControl1.Size = new System.Drawing.Size(330, 13);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Changes to genres take affect immediately. Bold indicates mappings.";
            // 
            // btnGenre
            // 
            this.btnGenre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenre.Location = new System.Drawing.Point(3, 249);
            this.btnGenre.Name = "btnGenre";
            this.btnGenre.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.btnGenre.Size = new System.Drawing.Size(340, 20);
            this.btnGenre.TabIndex = 6;
            this.btnGenre.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.beOptions_ButtonClick);
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
            this.lbGenres.Size = new System.Drawing.Size(340, 226);
            this.lbGenres.SortOrder = System.Windows.Forms.SortOrder.Ascending;
            this.lbGenres.TabIndex = 5;
            this.lbGenres.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lbGenres_DrawItem);
            this.lbGenres.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbGenres_MouseClick);
            this.lbGenres.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbGenres_KeyDown);
            // 
            // tpTags
            // 
            this.tpTags.Controls.Add(this.lbcTags);
            this.tpTags.Controls.Add(this.beTags);
            this.tpTags.Name = "tpTags";
            this.tpTags.Size = new System.Drawing.Size(346, 270);
            this.tpTags.Text = "Tags";
            // 
            // lbcTags
            // 
            this.lbcTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbcTags.Location = new System.Drawing.Point(0, 0);
            this.lbcTags.Name = "lbcTags";
            this.lbcTags.Padding = new System.Windows.Forms.Padding(2);
            this.lbcTags.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbcTags.Size = new System.Drawing.Size(346, 245);
            this.lbcTags.TabIndex = 3;
            // 
            // beTags
            // 
            this.beTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.beTags.Location = new System.Drawing.Point(0, 251);
            this.beTags.Name = "beTags";
            this.beTags.Padding = new System.Windows.Forms.Padding(2);
            this.beTags.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)});
            this.beTags.Size = new System.Drawing.Size(346, 20);
            this.beTags.TabIndex = 2;
            // 
            // tpMountTools
            // 
            this.tpMountTools.Controls.Add(this.labelControl7);
            this.tpMountTools.Controls.Add(this.labelControl6);
            this.tpMountTools.Controls.Add(this.labelControl5);
            this.tpMountTools.Controls.Add(this.labelControl4);
            this.tpMountTools.Controls.Add(this.teMntToolPath);
            this.tpMountTools.Controls.Add(this.cmbMntToolVDrive);
            this.tpMountTools.Controls.Add(this.cmbMntToolScan);
            this.tpMountTools.Controls.Add(this.simpleButtonScanMntTool);
            this.tpMountTools.Controls.Add(this.rgMountingTool);
            this.tpMountTools.Name = "tpMountTools";
            this.tpMountTools.Size = new System.Drawing.Size(346, 270);
            this.tpMountTools.Text = "Mounting Tools";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(9, 21);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(99, 13);
            this.labelControl7.TabIndex = 17;
            this.labelControl7.Text = "Which Mounting Tool";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(9, 166);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(92, 13);
            this.labelControl6.TabIndex = 16;
            this.labelControl6.Text = "Mounting Tool Path";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(9, 121);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(180, 13);
            this.labelControl5.TabIndex = 15;
            this.labelControl5.Text = "Scan drive for selected Mounting Tool";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(9, 211);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(58, 13);
            this.labelControl4.TabIndex = 14;
            this.labelControl4.Text = "Virtual Drive";
            // 
            // teMntToolPath
            // 
            this.teMntToolPath.Location = new System.Drawing.Point(24, 185);
            this.teMntToolPath.Name = "teMntToolPath";
            this.teMntToolPath.Size = new System.Drawing.Size(313, 20);
            this.teMntToolPath.TabIndex = 13;
            // 
            // cmbMntToolVDrive
            // 
            this.cmbMntToolVDrive.Location = new System.Drawing.Point(24, 230);
            this.cmbMntToolVDrive.Name = "cmbMntToolVDrive";
            this.cmbMntToolVDrive.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbMntToolVDrive.Properties.Items.AddRange(new object[] {
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z"});
            this.cmbMntToolVDrive.Size = new System.Drawing.Size(121, 20);
            this.cmbMntToolVDrive.TabIndex = 12;
            // 
            // cmbMntToolScan
            // 
            this.cmbMntToolScan.Location = new System.Drawing.Point(24, 140);
            this.cmbMntToolScan.Name = "cmbMntToolScan";
            this.cmbMntToolScan.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbMntToolScan.Properties.Items.AddRange(new object[] {
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z"});
            this.cmbMntToolScan.Size = new System.Drawing.Size(121, 20);
            this.cmbMntToolScan.TabIndex = 11;
            // 
            // simpleButtonScanMntTool
            // 
            this.simpleButtonScanMntTool.Location = new System.Drawing.Point(151, 140);
            this.simpleButtonScanMntTool.Name = "simpleButtonScanMntTool";
            this.simpleButtonScanMntTool.Size = new System.Drawing.Size(75, 23);
            this.simpleButtonScanMntTool.TabIndex = 10;
            this.simpleButtonScanMntTool.Text = "Begin Scan";
            this.simpleButtonScanMntTool.Click += new System.EventHandler(this.simpleButtonScanMntTool_Click);
            // 
            // rgMountingTool
            // 
            this.rgMountingTool.Location = new System.Drawing.Point(24, 40);
            this.rgMountingTool.Name = "rgMountingTool";
            this.rgMountingTool.Size = new System.Drawing.Size(121, 75);
            this.rgMountingTool.TabIndex = 9;
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
            this.sbOK.Location = new System.Drawing.Point(202, 310);
            this.sbOK.Name = "sbOK";
            this.sbOK.Size = new System.Drawing.Size(75, 23);
            this.sbOK.TabIndex = 2;
            this.sbOK.Text = "&OK";
            this.sbOK.Click += new System.EventHandler(this.SimpleButtonClick);
            // 
            // cmGenreMappings
            // 
            this.cmGenreMappings.Name = "cmGenreMappings";
            this.cmGenreMappings.Size = new System.Drawing.Size(61, 4);
            // 
            // ceScanDiskRollInfoToTitle
            // 
            this.ceScanDiskRollInfoToTitle.Location = new System.Drawing.Point(7, 212);
            this.ceScanDiskRollInfoToTitle.Name = "ceScanDiskRollInfoToTitle";
            this.ceScanDiskRollInfoToTitle.Properties.Caption = "Roll scan disk info into title";
            this.ceScanDiskRollInfoToTitle.Size = new System.Drawing.Size(259, 19);
            this.ceScanDiskRollInfoToTitle.TabIndex = 10;
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
            this.tpOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ceAutoScanDiskOnAdd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beTitledFanArtPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceTitledFanArtFolder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDefaultMetadataPlugin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cePrependParentFolder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceUseGenreList.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceUseMPAAList.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceFoldersAsTitles.Properties)).EndInit();
            this.tpSkins.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbcSkins)).EndInit();
            this.tpMPAAList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbcMPAA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beMPAA.Properties)).EndInit();
            this.tpGenres.ResumeLayout(false);
            this.tpGenres.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnGenre.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbGenres)).EndInit();
            this.tpTags.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbcTags)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.beTags.Properties)).EndInit();
            this.tpMountTools.ResumeLayout(false);
            this.tpMountTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teMntToolPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMntToolVDrive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbMntToolScan.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rgMountingTool.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceScanDiskRollInfoToTitle.Properties)).EndInit();
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
        private DevExpress.XtraEditors.CheckEdit ceFoldersAsTitles;
        private DevExpress.XtraTab.XtraTabPage tpMPAAList;
        private DevExpress.XtraEditors.ListBoxControl lbcMPAA;
        private DevExpress.XtraEditors.ButtonEdit beMPAA;
        private DevExpress.XtraTab.XtraTabPage tpGenres;
        private DevExpress.XtraEditors.ButtonEdit btnGenre;
        private DevExpress.XtraEditors.ListBoxControl lbGenres;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.CheckEdit ceUseGenreList;
        private DevExpress.XtraTab.XtraTabPage tpTags;
        private DevExpress.XtraEditors.ListBoxControl lbcTags;
        private DevExpress.XtraEditors.ButtonEdit beTags;
        private System.Windows.Forms.ContextMenuStrip cmGenreMappings;
        private DevExpress.XtraEditors.CheckEdit cePrependParentFolder;
        private DevExpress.XtraEditors.ComboBoxEdit cmbDefaultMetadataPlugin;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.CheckEdit ceTitledFanArtFolder;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.ButtonEdit beTitledFanArtPath;
        private DevExpress.XtraTab.XtraTabPage tpMountTools;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit teMntToolPath;
        private DevExpress.XtraEditors.ComboBoxEdit cmbMntToolVDrive;
        private DevExpress.XtraEditors.ComboBoxEdit cmbMntToolScan;
        private DevExpress.XtraEditors.SimpleButton simpleButtonScanMntTool;
        private DevExpress.XtraEditors.RadioGroup rgMountingTool;
        private DevExpress.XtraEditors.CheckEdit ceAutoScanDiskOnAdd;
        private DevExpress.XtraEditors.CheckEdit ceScanDiskRollInfoToTitle;
    }
}