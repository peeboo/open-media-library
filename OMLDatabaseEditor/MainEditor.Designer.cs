namespace OMLDatabaseEditor
{
    partial class MainEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainEditor));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblCurrentStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pgbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromScratchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllMoviesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regenerateThumbnailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentMovieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allMoviesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.metaDataSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.titleEditor = new OMLDatabaseEditor.Controls.TitleEditor();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.mainNav = new DevExpress.XtraNavBar.NavBarControl();
            this.groupMovies = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lbMovies = new DevExpress.XtraEditors.ListBoxControl();
            this.navBarGroupControlContainer2 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lbMetadata = new DevExpress.XtraEditors.ListBoxControl();
            this.navBarGroupControlContainer3 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lbImport = new DevExpress.XtraEditors.ListBoxControl();
            this.groupMetadata = new DevExpress.XtraNavBar.NavBarGroup();
            this.groupImport = new DevExpress.XtraNavBar.NavBarGroup();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainNav)).BeginInit();
            this.mainNav.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbMovies)).BeginInit();
            this.navBarGroupControlContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbMetadata)).BeginInit();
            this.navBarGroupControlContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbImport)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCurrentStatus,
            this.pgbProgress});
            this.statusStrip.Location = new System.Drawing.Point(0, 418);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip.Size = new System.Drawing.Size(721, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblCurrentStatus
            // 
            this.lblCurrentStatus.Name = "lblCurrentStatus";
            this.lblCurrentStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // pgbProgress
            // 
            this.pgbProgress.Name = "pgbProgress";
            this.pgbProgress.Size = new System.Drawing.Size(100, 16);
            this.pgbProgress.Visible = false;
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(721, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromScratchToolStripMenuItem});
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.newToolStripMenuItem.Text = "&New Movie";
            // 
            // fromScratchToolStripMenuItem
            // 
            this.fromScratchToolStripMenuItem.Name = "fromScratchToolStripMenuItem";
            this.fromScratchToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.fromScratchToolStripMenuItem.Text = "From Scratch";
            this.fromScratchToolStripMenuItem.Click += new System.EventHandler(this.fromScratchToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(171, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.saveToolStripMenuItem.Text = "&Save Movie";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(171, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.deleteAllMoviesToolStripMenuItem,
            this.regenerateThumbnailToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.deleteToolStripMenuItem.Text = "&Delete Current Movie";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // deleteAllMoviesToolStripMenuItem
            // 
            this.deleteAllMoviesToolStripMenuItem.Name = "deleteAllMoviesToolStripMenuItem";
            this.deleteAllMoviesToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.deleteAllMoviesToolStripMenuItem.Text = "Delete All Movies";
            this.deleteAllMoviesToolStripMenuItem.Click += new System.EventHandler(this.deleteAllMoviesToolStripMenuItem_Click);
            // 
            // regenerateThumbnailToolStripMenuItem
            // 
            this.regenerateThumbnailToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentMovieToolStripMenuItem,
            this.allMoviesToolStripMenuItem});
            this.regenerateThumbnailToolStripMenuItem.Name = "regenerateThumbnailToolStripMenuItem";
            this.regenerateThumbnailToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.regenerateThumbnailToolStripMenuItem.Text = "Regenerate Thumbnails";
            // 
            // currentMovieToolStripMenuItem
            // 
            this.currentMovieToolStripMenuItem.Name = "currentMovieToolStripMenuItem";
            this.currentMovieToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.currentMovieToolStripMenuItem.Text = "Current Movie";
            this.currentMovieToolStripMenuItem.Click += new System.EventHandler(this.currentMovieToolStripMenuItem_Click);
            // 
            // allMoviesToolStripMenuItem
            // 
            this.allMoviesToolStripMenuItem.Name = "allMoviesToolStripMenuItem";
            this.allMoviesToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.allMoviesToolStripMenuItem.Text = "All Movies";
            this.allMoviesToolStripMenuItem.Click += new System.EventHandler(this.allMoviesToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customizeToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolStripSeparator1,
            this.metaDataSettingsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // customizeToolStripMenuItem
            // 
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            this.customizeToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.customizeToolStripMenuItem.Text = "&Customize";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
            // 
            // metaDataSettingsToolStripMenuItem
            // 
            this.metaDataSettingsToolStripMenuItem.Name = "metaDataSettingsToolStripMenuItem";
            this.metaDataSettingsToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.metaDataSettingsToolStripMenuItem.Text = "MetaData Settings";
            this.metaDataSettingsToolStripMenuItem.Click += new System.EventHandler(this.metaDataSettingsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator7,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(119, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panelControl1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(721, 369);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(721, 418);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.titleEditor);
            this.panelControl1.Controls.Add(this.splitterControl1);
            this.panelControl1.Controls.Add(this.mainNav);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(721, 369);
            this.panelControl1.TabIndex = 5;
            // 
            // titleEditor
            // 
            this.titleEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleEditor.Location = new System.Drawing.Point(185, 2);
            this.titleEditor.Name = "titleEditor";
            this.titleEditor.Size = new System.Drawing.Size(534, 365);
            this.titleEditor.Status = OMLDatabaseEditor.Controls.TitleEditor.TitleStatus.Normal;
            this.titleEditor.TabIndex = 2;
            this.titleEditor.TitleChanged += new OMLDatabaseEditor.Controls.TitleEditor.TitleChangeEventHandler(this.titleEditor_TitleChanged);
            this.titleEditor.TitleNameChanged += new OMLDatabaseEditor.Controls.TitleEditor.TitleNameChangeEventHandler(this.titleEditor_TitleNameChanged);
            // 
            // splitterControl1
            // 
            this.splitterControl1.Location = new System.Drawing.Point(179, 2);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(6, 365);
            this.splitterControl1.TabIndex = 1;
            this.splitterControl1.TabStop = false;
            // 
            // mainNav
            // 
            this.mainNav.ActiveGroup = this.groupMovies;
            this.mainNav.ContentButtonHint = null;
            this.mainNav.Controls.Add(this.navBarGroupControlContainer1);
            this.mainNav.Controls.Add(this.navBarGroupControlContainer2);
            this.mainNav.Controls.Add(this.navBarGroupControlContainer3);
            this.mainNav.Dock = System.Windows.Forms.DockStyle.Left;
            this.mainNav.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.groupMovies,
            this.groupMetadata,
            this.groupImport});
            this.mainNav.Location = new System.Drawing.Point(2, 2);
            this.mainNav.Name = "mainNav";
            this.mainNav.OptionsNavPane.ExpandedWidth = 140;
            this.mainNav.Size = new System.Drawing.Size(177, 365);
            this.mainNav.TabIndex = 0;
            this.mainNav.Text = "navBarControl1";
            this.mainNav.View = new DevExpress.XtraNavBar.ViewInfo.SkinNavigationPaneViewInfoRegistrator();
            this.mainNav.ActiveGroupChanged += new DevExpress.XtraNavBar.NavBarGroupEventHandler(this.mainNav_ActiveGroupChanged);
            // 
            // groupMovies
            // 
            this.groupMovies.Caption = "Movies";
            this.groupMovies.ControlContainer = this.navBarGroupControlContainer1;
            this.groupMovies.Expanded = true;
            this.groupMovies.GroupClientHeight = 80;
            this.groupMovies.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.groupMovies.LargeImage = global::OMLDatabaseEditor.Properties.Resources.applications_multimedia;
            this.groupMovies.Name = "groupMovies";
            // 
            // navBarGroupControlContainer1
            // 
            this.navBarGroupControlContainer1.Controls.Add(this.lbMovies);
            this.navBarGroupControlContainer1.Name = "navBarGroupControlContainer1";
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(175, 156);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // lbMovies
            // 
            this.lbMovies.DisplayMember = "Name";
            this.lbMovies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbMovies.Location = new System.Drawing.Point(0, 0);
            this.lbMovies.Name = "lbMovies";
            this.lbMovies.Size = new System.Drawing.Size(175, 156);
            this.lbMovies.TabIndex = 0;
            this.lbMovies.ValueMember = "InternalItemID";
            this.lbMovies.SelectedIndexChanged += new System.EventHandler(this.lbMovies_SelectedIndexChanged);
            // 
            // navBarGroupControlContainer2
            // 
            this.navBarGroupControlContainer2.Controls.Add(this.lbMetadata);
            this.navBarGroupControlContainer2.Name = "navBarGroupControlContainer2";
            this.navBarGroupControlContainer2.Size = new System.Drawing.Size(138, 210);
            this.navBarGroupControlContainer2.TabIndex = 1;
            // 
            // lbMetadata
            // 
            this.lbMetadata.DisplayMember = "PluginName";
            this.lbMetadata.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbMetadata.HotTrackItems = true;
            this.lbMetadata.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
            this.lbMetadata.Location = new System.Drawing.Point(0, 0);
            this.lbMetadata.Name = "lbMetadata";
            this.lbMetadata.Size = new System.Drawing.Size(138, 210);
            this.lbMetadata.TabIndex = 0;
            this.lbMetadata.SelectedIndexChanged += new System.EventHandler(this.lbMetadata_SelectedIndexChanged);
            // 
            // navBarGroupControlContainer3
            // 
            this.navBarGroupControlContainer3.Controls.Add(this.lbImport);
            this.navBarGroupControlContainer3.Name = "navBarGroupControlContainer3";
            this.navBarGroupControlContainer3.Size = new System.Drawing.Size(138, 210);
            this.navBarGroupControlContainer3.TabIndex = 2;
            // 
            // lbImport
            // 
            this.lbImport.DisplayMember = "Menu";
            this.lbImport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbImport.HotTrackItems = true;
            this.lbImport.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
            this.lbImport.Location = new System.Drawing.Point(0, 0);
            this.lbImport.Name = "lbImport";
            this.lbImport.Size = new System.Drawing.Size(138, 210);
            this.lbImport.TabIndex = 0;
            this.lbImport.SelectedIndexChanged += new System.EventHandler(this.lbImport_SelectedIndexChanged);
            // 
            // groupMetadata
            // 
            this.groupMetadata.Caption = "Metadata";
            this.groupMetadata.ControlContainer = this.navBarGroupControlContainer2;
            this.groupMetadata.GroupClientHeight = 80;
            this.groupMetadata.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.groupMetadata.LargeImage = global::OMLDatabaseEditor.Properties.Resources.text_html;
            this.groupMetadata.Name = "groupMetadata";
            // 
            // groupImport
            // 
            this.groupImport.Caption = "Import";
            this.groupImport.ControlContainer = this.navBarGroupControlContainer3;
            this.groupImport.GroupClientHeight = 80;
            this.groupImport.GroupStyle = DevExpress.XtraNavBar.NavBarGroupStyle.ControlContainer;
            this.groupImport.LargeImage = global::OMLDatabaseEditor.Properties.Resources.emblem_symbolic_link;
            this.groupImport.Name = "groupImport";
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator,
            this.helpToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(3, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(87, 25);
            this.toolStrip.TabIndex = 2;
            // 
            // newToolStripButton
            // 
            this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
            this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.newToolStripButton.Name = "newToolStripButton";
            this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.newToolStripButton.Text = "&New Movie";
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save Movie";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.helpToolStripButton.Text = "He&lp";
            // 
            // defaultLookAndFeel1
            // 
            this.defaultLookAndFeel1.LookAndFeel.SkinName = "Blue";
            // 
            // MainEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 440);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainEditor";
            this.Text = "OML Movie Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainEditor_FormClosing);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainNav)).EndInit();
            this.mainNav.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbMovies)).EndInit();
            this.navBarGroupControlContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbMetadata)).EndInit();
            this.navBarGroupControlContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbImport)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton newToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.ToolStripStatusLabel lblCurrentStatus;
        private System.Windows.Forms.ToolStripProgressBar pgbProgress;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem metaDataSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllMoviesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromScratchToolStripMenuItem;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private OMLDatabaseEditor.Controls.TitleEditor titleEditor;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraNavBar.NavBarControl mainNav;
        private DevExpress.XtraNavBar.NavBarGroup groupMovies;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer1;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer2;
        private DevExpress.XtraNavBar.NavBarGroupControlContainer navBarGroupControlContainer3;
        private DevExpress.XtraNavBar.NavBarGroup groupMetadata;
        private DevExpress.XtraNavBar.NavBarGroup groupImport;
        private DevExpress.XtraEditors.ListBoxControl lbMovies;
        private DevExpress.XtraEditors.ListBoxControl lbMetadata;
        private DevExpress.XtraEditors.ListBoxControl lbImport;
        private DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        private System.Windows.Forms.ToolStripMenuItem regenerateThumbnailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentMovieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allMoviesToolStripMenuItem;
    }
}

