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
            this.exportCurrentMovieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllMoviesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedMoviesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllMoviesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regenerateThumbnailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentMovieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allMoviesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allMoviesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.filterByGenreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterByCompletenessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.filterByParentalRatingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterByTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.metaDataSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.moveDisksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.mainNav = new DevExpress.XtraNavBar.NavBarControl();
            this.groupMovies = new DevExpress.XtraNavBar.NavBarGroup();
            this.navBarGroupControlContainer1 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lbMovies = new DevExpress.XtraEditors.ListBoxControl();
            this.cmsMoviesList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miMetadataMulti = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.navBarGroupControlContainer2 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lbMetadata = new DevExpress.XtraEditors.ListBoxControl();
            this.navBarGroupControlContainer3 = new DevExpress.XtraNavBar.NavBarGroupControlContainer();
            this.lbImport = new DevExpress.XtraEditors.ListBoxControl();
            this.groupMetadata = new DevExpress.XtraNavBar.NavBarGroup();
            this.groupImport = new DevExpress.XtraNavBar.NavBarGroup();
            this.titleEditor = new OMLDatabaseEditor.Controls.TitleEditor();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.newMovieSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.fromScratchToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
            this.fromPreferredSourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainNav)).BeginInit();
            this.mainNav.SuspendLayout();
            this.navBarGroupControlContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbMovies)).BeginInit();
            this.cmsMoviesList.SuspendLayout();
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
            this.statusStrip.Location = new System.Drawing.Point(0, 719);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip.Size = new System.Drawing.Size(745, 22);
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
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(745, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveToolStripMenuItem,
            this.exportCurrentMovieToolStripMenuItem,
            this.exportAllMoviesToolStripMenuItem,
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
            this.newToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
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
            this.toolStripSeparator2.Size = new System.Drawing.Size(183, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.saveToolStripMenuItem.Text = "&Save Movie";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.ToolStripOptionClick);
            // 
            // exportCurrentMovieToolStripMenuItem
            // 
            this.exportCurrentMovieToolStripMenuItem.Name = "exportCurrentMovieToolStripMenuItem";
            this.exportCurrentMovieToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exportCurrentMovieToolStripMenuItem.Text = "Export Current Movie";
            this.exportCurrentMovieToolStripMenuItem.Click += new System.EventHandler(this.exportCurrentMovieToolStripMenuItem_Click);
            // 
            // exportAllMoviesToolStripMenuItem
            // 
            this.exportAllMoviesToolStripMenuItem.Name = "exportAllMoviesToolStripMenuItem";
            this.exportAllMoviesToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exportAllMoviesToolStripMenuItem.Text = "Export All Movies";
            this.exportAllMoviesToolStripMenuItem.Click += new System.EventHandler(this.exportAllMoviesToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(183, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ToolStripOptionClick);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.deleteSelectedMoviesToolStripMenuItem,
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
            // deleteSelectedMoviesToolStripMenuItem
            // 
            this.deleteSelectedMoviesToolStripMenuItem.Name = "deleteSelectedMoviesToolStripMenuItem";
            this.deleteSelectedMoviesToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.deleteSelectedMoviesToolStripMenuItem.Text = "Delete Selected Movies";
            this.deleteSelectedMoviesToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedMoviesToolStripMenuItem_Click);
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
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allMoviesToolStripMenuItem1,
            this.filterByGenreToolStripMenuItem,
            this.filterByCompletenessToolStripMenuItem,
            this.filterByParentalRatingToolStripMenuItem,
            this.filterByTagToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // allMoviesToolStripMenuItem1
            // 
            this.allMoviesToolStripMenuItem1.Checked = true;
            this.allMoviesToolStripMenuItem1.CheckOnClick = true;
            this.allMoviesToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allMoviesToolStripMenuItem1.Name = "allMoviesToolStripMenuItem1";
            this.allMoviesToolStripMenuItem1.Size = new System.Drawing.Size(199, 22);
            this.allMoviesToolStripMenuItem1.Text = "All Movies";
            this.allMoviesToolStripMenuItem1.Click += new System.EventHandler(this.filterTitles_Click);
            // 
            // filterByGenreToolStripMenuItem
            // 
            this.filterByGenreToolStripMenuItem.Name = "filterByGenreToolStripMenuItem";
            this.filterByGenreToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.filterByGenreToolStripMenuItem.Text = "Filter By Genre";
            // 
            // filterByCompletenessToolStripMenuItem
            // 
            this.filterByCompletenessToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.filterByCompletenessToolStripMenuItem.Name = "filterByCompletenessToolStripMenuItem";
            this.filterByCompletenessToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.filterByCompletenessToolStripMenuItem.Text = "Filter By Completeness";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.CheckOnClick = true;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(96, 22);
            this.toolStripMenuItem2.Text = "25%";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.filterTitles_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.CheckOnClick = true;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(96, 22);
            this.toolStripMenuItem3.Text = "50%";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.filterTitles_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.CheckOnClick = true;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(96, 22);
            this.toolStripMenuItem4.Text = "75%";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.filterTitles_Click);
            // 
            // filterByParentalRatingToolStripMenuItem
            // 
            this.filterByParentalRatingToolStripMenuItem.Name = "filterByParentalRatingToolStripMenuItem";
            this.filterByParentalRatingToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.filterByParentalRatingToolStripMenuItem.Text = "Filter By Parental Rating";
            // 
            // filterByTagToolStripMenuItem
            // 
            this.filterByTagToolStripMenuItem.Name = "filterByTagToolStripMenuItem";
            this.filterByTagToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.filterByTagToolStripMenuItem.Text = "Filter By Tag";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customizeToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolStripSeparator1,
            this.metaDataSettingsToolStripMenuItem,
            this.toolStripSeparator4,
            this.moveDisksToolStripMenuItem});
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
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.ToolStripOptionClick);
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
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(167, 6);
            // 
            // moveDisksToolStripMenuItem
            // 
            this.moveDisksToolStripMenuItem.Name = "moveDisksToolStripMenuItem";
            this.moveDisksToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.moveDisksToolStripMenuItem.Text = "Move Disks...";
            this.moveDisksToolStripMenuItem.Click += new System.EventHandler(this.ToolStripOptionClick);
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
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.ToolStripOptionClick);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainerControl1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(745, 670);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(745, 719);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip);
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.mainNav);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.titleEditor);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(745, 670);
            this.splitContainerControl1.SplitterPosition = 208;
            this.splitContainerControl1.TabIndex = 6;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // mainNav
            // 
            this.mainNav.ActiveGroup = this.groupMovies;
            this.mainNav.ContentButtonHint = null;
            this.mainNav.Controls.Add(this.navBarGroupControlContainer1);
            this.mainNav.Controls.Add(this.navBarGroupControlContainer2);
            this.mainNav.Controls.Add(this.navBarGroupControlContainer3);
            this.mainNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainNav.Groups.AddRange(new DevExpress.XtraNavBar.NavBarGroup[] {
            this.groupMovies,
            this.groupMetadata,
            this.groupImport});
            this.mainNav.Location = new System.Drawing.Point(0, 0);
            this.mainNav.Name = "mainNav";
            this.mainNav.OptionsNavPane.ExpandedWidth = 140;
            this.mainNav.Size = new System.Drawing.Size(204, 666);
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
            this.navBarGroupControlContainer1.Size = new System.Drawing.Size(202, 457);
            this.navBarGroupControlContainer1.TabIndex = 0;
            // 
            // lbMovies
            // 
            this.lbMovies.ContextMenuStrip = this.cmsMoviesList;
            this.lbMovies.DisplayMember = "Name";
            this.lbMovies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbMovies.Location = new System.Drawing.Point(0, 0);
            this.lbMovies.Name = "lbMovies";
            this.lbMovies.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbMovies.Size = new System.Drawing.Size(202, 457);
            this.lbMovies.TabIndex = 0;
            this.lbMovies.ValueMember = "InternalItemID";
            this.lbMovies.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lbMovies_DrawItem);
            this.lbMovies.SelectedIndexChanged += new System.EventHandler(this.lbMovies_SelectedIndexChanged);
            // 
            // cmsMoviesList
            // 
            this.cmsMoviesList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miMetadataMulti,
            this.deleteToolStripMenuItem1});
            this.cmsMoviesList.Name = "cmsMoviesList";
            this.cmsMoviesList.Size = new System.Drawing.Size(166, 70);
            // 
            // miMetadataMulti
            // 
            this.miMetadataMulti.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromPreferredSourcesToolStripMenuItem});
            this.miMetadataMulti.Name = "miMetadataMulti";
            this.miMetadataMulti.Size = new System.Drawing.Size(165, 22);
            this.miMetadataMulti.Text = "Update metadata";
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(165, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteSelectedMoviesToolStripMenuItem_Click);
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
            // titleEditor
            // 
            this.titleEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleEditor.Location = new System.Drawing.Point(0, 0);
            this.titleEditor.Name = "titleEditor";
            this.titleEditor.Size = new System.Drawing.Size(527, 666);
            this.titleEditor.Status = OMLDatabaseEditor.Controls.TitleEditor.TitleStatus.Normal;
            this.titleEditor.TabIndex = 2;
            this.titleEditor.TitleChanged += new OMLDatabaseEditor.Controls.TitleEditor.TitleChangeEventHandler(this.titleEditor_TitleChanged);
            this.titleEditor.TitleNameChanged += new OMLDatabaseEditor.Controls.TitleEditor.TitleNameChangeEventHandler(this.titleEditor_TitleNameChanged);
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMovieSplitButton,
            this.saveToolStripButton,
            this.toolStripSeparator,
            this.helpToolStripButton});
            this.toolStrip.Location = new System.Drawing.Point(3, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(96, 25);
            this.toolStrip.TabIndex = 2;
            // 
            // newMovieSplitButton
            // 
            this.newMovieSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newMovieSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromScratchToolStripMenuItem1});
            this.newMovieSplitButton.Image = global::OMLDatabaseEditor.Properties.Resources.NewDocumentHS;
            this.newMovieSplitButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.newMovieSplitButton.Name = "newMovieSplitButton";
            this.newMovieSplitButton.Size = new System.Drawing.Size(32, 22);
            this.newMovieSplitButton.Text = "New Movie";
            // 
            // fromScratchToolStripMenuItem1
            // 
            this.fromScratchToolStripMenuItem1.Name = "fromScratchToolStripMenuItem1";
            this.fromScratchToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
            this.fromScratchToolStripMenuItem1.Text = "From Scratch";
            this.fromScratchToolStripMenuItem1.Click += new System.EventHandler(this.fromScratchToolStripMenuItem_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save Movie";
            this.saveToolStripButton.Click += new System.EventHandler(this.ToolStripOptionClick);
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
            // fromPreferredSourcesToolStripMenuItem
            // 
            this.fromPreferredSourcesToolStripMenuItem.Name = "fromPreferredSourcesToolStripMenuItem";
            this.fromPreferredSourcesToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.fromPreferredSourcesToolStripMenuItem.Text = "From Preferred Sources";
            this.fromPreferredSourcesToolStripMenuItem.Click += new System.EventHandler(this.fromPreferredSourcesToolStripMenuItem_Click);
            // 
            // MainEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 741);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainEditor";
            this.Text = "OML Movie Manager";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainNav)).EndInit();
            this.mainNav.ResumeLayout(false);
            this.navBarGroupControlContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lbMovies)).EndInit();
            this.cmsMoviesList.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.ToolStripStatusLabel lblCurrentStatus;
        private System.Windows.Forms.ToolStripProgressBar pgbProgress;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem metaDataSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllMoviesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromScratchToolStripMenuItem;
        private OMLDatabaseEditor.Controls.TitleEditor titleEditor;
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
        private System.Windows.Forms.ToolStripMenuItem regenerateThumbnailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentMovieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allMoviesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsMoviesList;
        private System.Windows.Forms.ToolStripMenuItem miMetadataMulti;
        private System.Windows.Forms.ToolStripMenuItem exportCurrentMovieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAllMoviesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton newMovieSplitButton;
        private System.Windows.Forms.ToolStripMenuItem fromScratchToolStripMenuItem1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        public DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterByGenreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterByCompletenessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterByParentalRatingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem allMoviesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem filterByTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem moveDisksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedMoviesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fromPreferredSourcesToolStripMenuItem;
    }
}

