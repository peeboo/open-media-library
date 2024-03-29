using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using DevExpress.Skins.Info;
using DevExpress.Utils;
using StSana;

using OMLEngine;
using OMLSDK;
using OMLDatabaseEditor.Controls;
using OMLEngine.Dao;

namespace OMLDatabaseEditor
{
    public partial class MainEditor : XtraForm
    {
        internal static List<OMLPlugin> _importPlugins = new List<OMLPlugin>();
        
        internal static List<MetaDataPluginDescriptor> _metadataPlugins = new List<MetaDataPluginDescriptor>();

        private const string APP_TITLE = "OML Movie Manager";
        private bool _loading = false;

        private Dictionary<int,Title> _movieList;
        private Dictionary<int, TreeNode> _mediaTree;
        private Dictionary<int, int> _movieCount;
        int _movieRootCount;

        DevExpress.XtraNavBar.NavBarGroup CurrentNavigation;

        public List<String> DXSkins;

        int? SelectedTreeRoot;
        
        LinearGradientBrush _brushTreeViewSelected;
        LinearGradientBrush _brushTitleListSelected;
        LinearGradientBrush _brushTitleListFolder;
        LinearGradientBrush _brushTitleListFolderSelected;

        Image ImgMetaPercentage1;
        Image ImgMetaPercentage2;
        Image ImgMetaPercentage3;
        Image ImgMetaPercentage4;
        Image ImgMetaPercentage5;
        Image ImgStars0;
        Image ImgStars1;
        Image ImgStars2;
        Image ImgStars3;
        Image ImgStars4;
        Image ImgStars5;
        
        #region Initialisation
        public MainEditor()
        {
            OMLEngine.Utilities.RawSetup();

            SplashScreen2.ShowSplashScreen();

            InitializeComponent();

            if (InitData())
            {
                // Setup views
                splitContainerDetails.Panel2.Controls["titleEditor"].Visible = true;
                splitContainerDetails.Panel2.Controls["bioDataEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["genreMetaDataEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["titlesListView"].Visible = false;
                splitContainerDetails.Panel2.Controls["titleEditor"].Dock = DockStyle.Fill;
                splitContainerDetails.Panel2.Controls["bioDataEditor"].Dock = DockStyle.Fill;
                splitContainerDetails.Panel2.Controls["genreMetaDataEditor"].Dock = DockStyle.Fill;
                splitContainerDetails.Panel2.Controls["titlesListView"].Dock = DockStyle.Fill;
                alwaysShowTitleListToolStripMenuItem.Checked = OMLEngine.Settings.OMLSettings.DBEAlwaysShowTitleList;
                showAllItemsInTitleListToolStripMenuItem.Checked = OMLEngine.Settings.OMLSettings.ShowSubFolderTitles;

                this.DesktopBounds = new Rectangle(Properties.Settings.Default.Location, Properties.Settings.Default.Size);
                this.WindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), Properties.Settings.Default.WindowState);

                CurrentNavigation = mainNav.ActiveGroup;
            }
            else
            {
                Environment.Exit(-1);
            }
        }


        /// <summary>
        /// Perform startup initialisation including updating the splash screen
        /// </summary>
        private bool InitData()
        {
            Cursor = Cursors.WaitCursor;
            //OMLEngine.Utilities.DebugLine("[MainEditor] InitData() : new TitleCollection()");
            //OMLEngine.Utilities.DebugLine("[MainEditor] InitData() : loadTitleCollection()");            

            SplashScreen2.SetStatus(14, "Checking database.");
            if (!ValidateDatabase())
            {
                SplashScreen2.CloseForm();
                Cursor = Cursors.Default;
                return false;
            }

            defaultLookAndFeel1.LookAndFeel.SkinName = OMLEngine.Settings.OMLSettings.DBEditorSkin;

            SplashScreen2.SetStatus(28, "Loading plugins.");
            SetupNewMovieAndContextMenu();

            SplashScreen2.SetStatus(42, "Setting up filter lists");
            SetupFilterListContextMenu();

            SplashScreen2.SetStatus(56, "Loading Skins.");
            GetDXSkins();

            _loading = true;
            
            SplashScreen2.SetStatus(70, "Loading Movies.");

            _movieList = new Dictionary<int,Title>();

            LoadMovies();
            PopulateMediaTree();
            PopulateMovieListV2(SelectedTreeRoot);

            SplashScreen2.SetStatus(85, "Loading MRU Lists.");
            this.titleEditor.SetMRULists();

            // Load resource images
            Stream imgStream = null;
            Assembly a = Assembly.GetExecutingAssembly();

            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.MetaDataIndicator1.png");
            ImgMetaPercentage1 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.MetaDataIndicator2.png");
            ImgMetaPercentage2 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.MetaDataIndicator3.png");
            ImgMetaPercentage3 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.MetaDataIndicator4.png");
            ImgMetaPercentage4 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.MetaDataIndicator5.png");
            ImgMetaPercentage5 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.Stars0.png");
            ImgStars0 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.Stars1.png");
            ImgStars1 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.Stars2.png");
            ImgStars2 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.Stars3.png");
            ImgStars3 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.Stars4.png");
            ImgStars4 = Image.FromStream(imgStream);
            imgStream = a.GetManifestResourceStream("OMLDatabaseEditor.Resources.Stars5.png");
            ImgStars5 = Image.FromStream(imgStream);


            SplashScreen2.SetStatus(100,"Completed.");

            _loading = false;

            SplashScreen2.CloseForm();
            Cursor = Cursors.Default;
            return true;
        }

        /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/
        /// <summary>
        /// Load the db connection settings and try to connect to the database.
        /// Give option to specify alternative sql connection details if connection
        /// attempt fails
        /// </summary>
        /// <returns></returns>
        private bool ValidateDatabase()
        {
            // Run database diagnostics
            OMLEngine.DatabaseManagement.DatabaseManagement dbm = new OMLEngine.DatabaseManagement.DatabaseManagement();

            OMLEngine.DatabaseManagement.DatabaseInformation.SQLState state = dbm.CheckDatabase();

            if (state == OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OMLDBVersionUpgradeRequired)
            {
                XtraMessageBox.Show("The OML database is an older version. Press OK to upgrade the database!", "Problem running DBEditor", MessageBoxButtons.OK);

                // Find the script path. Also include hack to find scripts if running from VS rathr than c:\program files....
                string ScriptsPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\SQLInstaller";
                if (Directory.Exists( Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\..\\..\\SQL Scripts"))
                {
                    ScriptsPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\..\\..\\SQL Scripts";
                }

                dbm.UpgradeSchemaVersion(ScriptsPath);
         
                // Retest the connection
                state = dbm.CheckDatabase();
            }

            switch (state)
            {
                case OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OK:
                    return true;

                case OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OMLDBVersionCodeOlderThanSchema:
                    XtraMessageBox.Show("The OML client is an older version than the database. Please upgrade the client components!", "Problem running DBEditor", MessageBoxButtons.OK);
                    return false;

                case OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OMLDBVersionNotFound:
                    return false;

                case OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.LoginFailure:
                    if (XtraMessageBox.Show("This could be caused by the OML Server computer being unavailable. Do you want to specify the server used?", "Problem accessing database!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SelectDatabaseServer sds = new SelectDatabaseServer();
                        if (sds.ShowDialog() == DialogResult.OK)
                        {
                            OMLEngine.DatabaseManagement.DatabaseInformation.SQLServerName = sds.Server;
                            OMLEngine.DatabaseManagement.DatabaseInformation.SQLInstanceName = sds.SQLInstance;
                            XtraMessageBox.Show("Please restart the program for the changes to take effect!");
                        }
                    }
                    return false;

                    break;
                default:
                    XtraMessageBox.Show(state.ToString() + " : " + OMLEngine.DatabaseManagement.DatabaseInformation.LastSQLError, "Problem accessing database!");
                    return false;
            }
        }
        /*** REMOVE ONCE THE UNIFIED WIX INSTALLERS ARE COMPLETE ***/

        private void GetDXSkins()
        {
            DXSkins = new List<string>();
            foreach (SkinContainer s in SkinManager.Default.Skins)
            {
                DXSkins.Add(s.SkinName);
            }
            DXSkins.Sort();
        }

        private void SetupNewMovieAndContextMenu()
        {
            LoadMetadataPlugins(PluginTypes.MetadataPlugin, _metadataPlugins);
            foreach (MetaDataPluginDescriptor plugin in _metadataPlugins)
            {
                ToolStripMenuItem newItem = new ToolStripMenuItem("From " + plugin.DataProviderName);
                newItem.Tag = plugin;
                newItem.Click += new EventHandler(this.fromMetaDataToolStripMenuItem_Click);
                newToolStripMenuItem.DropDownItems.Add(newItem);

                newItem = new ToolStripMenuItem("From " + plugin.DataProviderName);
                newItem.Tag = plugin;
                newItem.Click += new EventHandler(this.fromMetaDataToolStripMenuItem_Click);
                newMovieSplitButton.DropDownItems.Add(newItem);

                /*ToolStripMenuItem metadataItem = new ToolStripMenuItem("From " + plugin.DataProviderName);
                metadataItem.Tag = plugin;
                metadataItem.Click += new EventHandler(this.miMetadataMulti_Click);
                miMetadataMulti.DropDownItems.Add(metadataItem);*/
            }

            if (String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPluginMovies))
                fromPreferredSourcesToolStripMenuItem1.Visible = false;
        }

        private void SetupFilterListContextMenu()
        {
            // Set up filter lists
            ToolStripMenuItem item;

            filterByGenreToolStripMenuItem.DropDownItems.Clear();
            foreach (string genre in from g in TitleCollectionManager.GetAllGenreMetaDatas() select g.Name)
            {
                item = new ToolStripMenuItem(genre);
                item.CheckOnClick = true;
                item.Click += new EventHandler(filterTitles_Click);
                filterByGenreToolStripMenuItem.DropDownItems.Add(item);
            }

            filterByParentalRatingToolStripMenuItem.DropDownItems.Clear();
            foreach (string rating in from t in TitleCollectionManager.GetAllParentalRatings(null) select t.Name)
            {
                item = new ToolStripMenuItem(rating);
                item.CheckOnClick = true;
                item.Click += new EventHandler(filterTitles_Click);
                filterByParentalRatingToolStripMenuItem.DropDownItems.Add(item);
            }

            filterByTagToolStripMenuItem.DropDownItems.Clear();
            foreach (string tag in TitleCollectionManager.GetAllTagsList())
            {
                item = new ToolStripMenuItem(tag);
                item.CheckOnClick = true;
                item.Click += new EventHandler(filterTitles_Click);
                filterByTagToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private void LoadImportPlugins(string pluginType, List<OMLPlugin> pluginList)
        {
            pluginList.Clear();

            List<PluginServices.AvailablePlugin> plugins = new List<PluginServices.AvailablePlugin>();
            string path = FileSystemWalker.PluginsDirectory;
            plugins = PluginServices.FindPlugins(path, pluginType);
            OMLPlugin objPlugin;
            // Loop through available plugins, creating instances and add them
            if (plugins != null)
            {
                foreach (PluginServices.AvailablePlugin oPlugin in plugins)
                {
                    objPlugin = (OMLPlugin)PluginServices.CreateInstance(oPlugin);
                    pluginList.Add(objPlugin);
                }
                plugins = null;
            }
        }

        /// <summary>
        /// Load metadata plugins
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="pluginList"></param>
        private void LoadMetadataPlugins(string pluginType, List<MetaDataPluginDescriptor> pluginList)
        {
            PluginServices pgs = new PluginServices();
            pgs.Loaded += new PluginServices.PluginLoaded(MetadataPluginLoaded);
            pgs.LoadMetadataPlugins(pluginType, pluginList);

            /*pluginList.Clear();

            List<PluginServices.AvailablePlugin> plugins = new List<PluginServices.AvailablePlugin>();
            string path = FileSystemWalker.PluginsDirectory;
            plugins = PluginServices.FindPlugins(path, pluginType);
            IOMLMetadataPlugin objPlugin;
            // Loop through available plugins, creating instances and add them
            if (plugins != null)
            {
                foreach (PluginServices.AvailablePlugin oPlugin in plugins)
                {
                    // Create an instance to enumerate providers in the plugin
                    objPlugin = (IOMLMetadataPlugin)PluginServices.CreateInstance(oPlugin);

                    foreach (MetaDataPluginDescriptor provider in objPlugin.GetProviders)
                    {
                        SplashScreen2.SetStatus(32, "Loading plugin - " + provider.DataProviderName);

                        // Create instance of the plugin for this particular provider. This would create a unique instance per provider.
                        provider.PluginDLL = (IOMLMetadataPlugin)PluginServices.CreateInstance(oPlugin);
                        
                        // Initialise the plugin and select which provider it serves
                        provider.PluginDLL.Initialize(provider.DataProviderName, new Dictionary<string, string>());

                        // Configure the plugin with any settings stored in the db
                        if (provider.PluginDLL.GetOptions() != null)
                        {
                            foreach (OMLMetadataOption option in provider.PluginDLL.GetOptions())
                            {
                                string setting = OMLEngine.Settings.SettingsManager.GetSettingByName(option.Name, "PLG-" + provider.DataProviderName);
                                if (setting != null)
                                {
                                    provider.PluginDLL.SetOptionValue(option.Name, setting);
                                }
                            }
                        }

                        pluginList.Add(provider);
                    }


                }
                plugins = null;
            }*/
        }

        void MetadataPluginLoaded(string message)
        {
            SplashScreen2.SetStatus(32, "Loading plugin - " + message);
        }
        private void LoadImporters()
        {
            Cursor = Cursors.WaitCursor;
            lbImport.Items.Clear();
            LoadImportPlugins(PluginTypes.ImportPlugin, _importPlugins);
            lbImport.DataSource = _importPlugins;
            lbImport.SelectedItem = null;
            Cursor = Cursors.Default;
        }

        private void LoadMetadata()
        {
            Cursor = Cursors.WaitCursor;
            lbMetadata.Items.Clear();
            LoadMetadataPlugins(PluginTypes.MetadataPlugin, _metadataPlugins);
            lbMetadata.DataSource = _metadataPlugins;
            lbMetadata.SelectedItem = null;
            Cursor = Cursors.Default;
        }

        private void MainEditor_Load(object sender, EventArgs e)
        {
            // If the old DAT file still exists ask the user if they want to import those titles.
            LegacyTitleCollection coll = new LegacyTitleCollection();
            if (coll.OMLDatExists)
            {
                if (XtraMessageBox.Show("Would you like to convert your existing movie collection?", "Convert collection", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    coll.LoadTitleCollectionFromOML();


                    foreach (Title title in coll.Source)
                    {
                        TitleCollectionManager.AddTitle(title);
                    }
                }

                coll.RenameDATCollection();

                LoadMovies();
                PopulateMediaTree();
                PopulateMovieListV2(SelectedTreeRoot);
            }
        }
        #endregion


        #region Title change management, filter, Load & Save
        private void filterTitles_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ToolStripMenuItem filterItem = ((ToolStripMenuItem)sender);
            // Uncheck all other filters
            foreach (ToolStripMenuItem item in filterByCompletenessToolStripMenuItem.DropDownItems)
                if (item != filterItem) item.Checked = false;
            foreach (ToolStripMenuItem item in filterByGenreToolStripMenuItem.DropDownItems)
                if (item != filterItem) item.Checked = false;
            foreach (ToolStripMenuItem item in filterByParentalRatingToolStripMenuItem.DropDownItems)
                if (item != filterItem) item.Checked = false;
            foreach (ToolStripMenuItem item in filterByTagToolStripMenuItem.DropDownItems)
                if (item != filterItem) item.Checked = false;

            if (sender == allMoviesToolStripMenuItem1)
            {
                LoadMovies();
                PopulateMovieListV2(SelectedTreeRoot);
            }
            else
            {
                allMoviesToolStripMenuItem1.Checked = false;

                if (filterItem.OwnerItem == filterByCompletenessToolStripMenuItem)
                {
                    // Percentage filter
                    _movieList = (TitleCollectionManager.GetAllTitles(TitleTypes.AllFolders).Concat(TitleCollectionManager.GetTitlesByPercentComplete(TitleTypes.AllMedia, decimal.Parse("." + filterItem.Text.TrimEnd('%')))).ToDictionary(k => k.Id));
                }
                else
                {
                    List<TitleFilter> tf = new List<TitleFilter>();
                    if (filterItem.OwnerItem == filterByGenreToolStripMenuItem) tf.Add(new TitleFilter(TitleFilterType.Genre, filterItem.Text));

                    if (filterItem.OwnerItem == filterByParentalRatingToolStripMenuItem) tf.Add(new TitleFilter(TitleFilterType.ParentalRating, filterItem.Text));

                    if (filterItem.OwnerItem == filterByTagToolStripMenuItem) tf.Add(new TitleFilter(TitleFilterType.Tag, filterItem.Text));

                    tf.Add(new TitleTypeFilter(TitleTypes.AllMedia));

                    _movieList = (TitleCollectionManager.GetAllTitles(TitleTypes.AllFolders).
                        Concat(TitleCollectionManager.GetFilteredTitles(tf)).ToDictionary(k => k.Id));
                }

                PopulateMovieListV2(SelectedTreeRoot);
            }
            Cursor = Cursors.Default;
        }

        private void beSearch_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            SearchTitles();
        }

        private void beSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchTitles();
            }
        }

        private void SearchTitles()
        {
            List<TitleFilter> tf = new List<TitleFilter>();
            tf.Add(new TitleFilter(TitleFilterType.Name, beSearch.Text));
            tf.Add(new TitleTypeFilter(TitleTypes.AllMedia));

            _movieList = (TitleCollectionManager.GetAllTitles(TitleTypes.AllFolders).
                Concat(TitleCollectionManager.GetFilteredTitles(tf)).ToDictionary(k => k.Id));

            PopulateMovieListV2(SelectedTreeRoot);
        }

        private void LoadMovies()
        {
            if (mainNav.ActiveGroup != groupMediaTree) return;
            Cursor = Cursors.WaitCursor;

            if (allMoviesToolStripMenuItem1.Checked)
            {
                _movieList = TitleCollectionManager.GetAllTitles(TitleTypes.AllFolders | TitleTypes.AllMedia).ToDictionary(k => k.Id);
            }
            else
            {
                // Find currently checked filter menu item
                bool found = false;
                foreach (ToolStripMenuItem item in filterByCompletenessToolStripMenuItem.DropDownItems)
                {
                    if (item.Checked)
                    {
                        filterTitles_Click(item, null);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    foreach (ToolStripMenuItem item in filterByGenreToolStripMenuItem.DropDownItems)
                    {
                        if (item.Checked)
                        {
                            filterTitles_Click(item, null);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        foreach (ToolStripMenuItem item in filterByParentalRatingToolStripMenuItem.DropDownItems)
                        {
                            if (item.Checked)
                            {
                                filterTitles_Click(item, null);
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }
            Cursor = Cursors.Default;
        }

        private void CountMovies()
        {
            if (_movieCount == null)
            {
                _movieCount = new Dictionary<int, int>();
            }
            else
            {
                _movieCount.Clear();
            }

            _movieRootCount = 0;

            foreach (Title title in _movieList.Values)
            {
                if ((title.TitleType & TitleTypes.AllMedia) != 0)
                {
                    if ((title.TitleType & TitleTypes.Root) != 0)
                    {
                        // Root Item
                        _movieRootCount++;
                    }
                    else
                    {
                        if (_movieCount.ContainsKey((int)title.ParentTitleId))
                        {
                            _movieCount[(int)title.ParentTitleId] = _movieCount[(int)title.ParentTitleId] + 1;
                        }
                        else
                        {
                            _movieCount[(int)title.ParentTitleId] = 1;
                        }
                    }
                }
            }
        }

        private DialogResult SaveCurrentMovie()
        {
            DialogResult result;
            if (titleEditor.EditedTitle != null && titleEditor.Status == OMLDatabaseEditor.Controls.TitleEditor.TitleStatus.UnsavedChanges)
            {
                result = XtraMessageBox.Show("You have unsaved changes to " + titleEditor.EditedTitle.Name + ". Would you like to save your changes?", "Save Changes?", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    //_movieList[titleEditor.EditedTitle.Id] = titleEditor.EditedTitle;
                    SaveChanges();
                }
                if (result == DialogResult.No)
                {
                    // Force reload of title
                    titleEditor.EditedTitle.ReloadTitle();
                    titleEditor.LoadDVD(_movieList[titleEditor.EditedTitle.Id]);
                    this.Text = APP_TITLE + " - " + titleEditor.EditedTitle.Name;
                    lvTitles.Refresh();
                    ToggleSaveState(false);
                }
            }
            else
            {
                result = DialogResult.Yes;
            }
            return result;
        }
  
        private void SaveChanges()
        {
            if ((titleEditor.EditedTitle != null) && (titleEditor.Status == OMLDatabaseEditor.Controls.TitleEditor.TitleStatus.UnsavedChanges))
            {
                Title editedTitle = titleEditor.EditedTitle;
                //   Title collectionTitle = TitleCollectionManager.GetTitle(editedTitle.Id);

                //if (editedTitle.MetadataSourceID == String.Empty)
                if (string.IsNullOrEmpty(editedTitle.MetadataSourceName))
                {
                    DialogResult result = XtraMessageBox.Show("Would you like to retrieve metadata on this movie?", "Get data", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        LoadMetadataPlugins(PluginTypes.MetadataPlugin, _metadataPlugins);
                        MetaDataPluginSelect selectPlugin = new MetaDataPluginSelect(_metadataPlugins);
                        selectPlugin.ShowDialog();
                        MetaDataPluginDescriptor plugin = null;

                        if (selectPlugin.SelectedPlugin(out plugin))
                        {
                            StartMetadataImport(editedTitle, plugin, false);
                        }
                    }
                    else
                    {
                        editedTitle.MetadataSourceName = "Manual Title";
                    }
                    //_titleCollection.Add(editedTitle);
                    //TitleCollectionManager.AddTitle(editedTitle);
                }
                else
                {
                    // Title exists so we need to copy edited data to collection title
                    //_titleCollection.Replace(editedTitle);
                    // todo : solomon : i think a save should just work here unless this is working
                    // off a copy

                    // This gets called by titleEditor.SaveChanges(); anyway
                    TitleCollectionManager.SaveTitleUpdates();

                    // Code below moved into folder and title creation
                    /*if (editedTitle.ParentTitleId == 0)
                    {
                        editedTitle.ReloadTitle();
                        editedTitle.ParentTitleId = editedTitle.Id;
                    }*/
                }

                titleEditor.SaveChanges();
                lvTitles.Refresh();

                if ((editedTitle.TitleType & TitleTypes.AllFolders) != 0)
                {
                    // If the current edited title is a folder, refresh the MediaTree
                    PopulateMediaTree();
                    if (SelectedTreeRoot != null)
                    {
                        if (_mediaTree.ContainsKey((int)SelectedTreeRoot))
                        {
                            treeMedia.SelectedNode = _mediaTree[(int)SelectedTreeRoot];
                        }
                    }
                }
                //LoadMovies();
            }
        }

        private void ToggleSaveState(bool enabled)
        {
            saveToolStripButton.Enabled = enabled;
            saveToolStripMenuItem.Enabled = enabled;
        }

        private void titleEditor_TitleChanged(object sender, EventArgs e)
        {
            if (titleEditor.EditedTitle != null)
                this.Text = APP_TITLE + " - " + titleEditor.EditedTitle.Name + "*";
            else
                this.Text = APP_TITLE;
            ToggleSaveState(true);
        }

        private void titleEditor_TitleNameChanged(object sender, OMLDatabaseEditor.Controls.TitleNameChangedEventArgs e)
        {
            this.Text = APP_TITLE + " - " + e.NewName + "*";
            ToggleSaveState(true);
        }
        #endregion


        #region MetaData Import
        private bool StartMetadataImport(Title title, MetaDataPluginDescriptor plugin, bool coverArtOnly)
        {
            int? SeasonNo = null;
            int? EpisodeNo = null;

            if (((title.TitleType & TitleTypes.Episode) != 0) ||
            ((title.TitleType & TitleTypes.Season) != 0) ||
            ((title.TitleType & TitleTypes.TVShow) != 0))
            {
                // TV Search
                if (title.SeasonNumber != null) SeasonNo = title.SeasonNumber.Value;
                if (title.EpisodeNumber != null) EpisodeNo = title.EpisodeNumber.Value;
                string Showname = null;

                // Try to find show name be looking up the folder structure.
                Title parenttitle = title;
                while ((parenttitle.TitleType & TitleTypes.Root) == 0)
                {
                    // Get parent
                    parenttitle = parenttitle.ParentTitle;
                    if ((parenttitle.TitleType & TitleTypes.TVShow) != 0)
                    {
                        Showname = parenttitle.Name;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(Showname))
                {
                    // Cannot find a show name in the folder structure
                    return StartMetadataImport(title, plugin, coverArtOnly, title.Name, "", SeasonNo, EpisodeNo);
                }
                else
                {
                    return StartMetadataImport(title, plugin, coverArtOnly, Showname, title.Name, SeasonNo, EpisodeNo);
                }
            }
            else
            {
                // Movie Search
                return StartMetadataImport(title, plugin, coverArtOnly, title.Name, "", SeasonNo, EpisodeNo);
            }
        }

        internal bool StartMetadataImport(Title title, string pluginName, bool coverArtOnly, string titleNameSearch, string EpisodeName, int SeasonNo, int EpisodeNo)
        {
            foreach (MetaDataPluginDescriptor plugin in _metadataPlugins)
            {
                if (plugin.DataProviderName == pluginName) return StartMetadataImport(title, plugin, coverArtOnly, titleNameSearch, EpisodeName, SeasonNo, EpisodeNo); 
            }
            return false;
        }

        internal bool StartMetadataImport(Title title, MetaDataPluginDescriptor plugin, bool coverArtOnly, string titleNameSearch, string EpisodeName, int? SeasonNo, int? EpisodeNo)
        {
            try
            {
                if (titleNameSearch != null)
                {
                    if (plugin != null)
                    {
                        frmSearchResult searchResultForm = null;

                        if ((plugin.DataProviderCapabilities & MetadataPluginCapabilities.SupportsTVSearch) != 0)
                        {
                            // TV Search
                            if (((title.TitleType & TitleTypes.Season) != 0) ||
                                ((title.TitleType & TitleTypes.TVShow) != 0))
                            {
                                // Only searching for the Show / season
                                searchResultForm = new frmSearchResult(plugin, titleNameSearch, "", 0, 0, true, true);
                            }
                            else
                            {
                                // Searching for an episode
                                searchResultForm = new frmSearchResult(plugin, titleNameSearch, EpisodeName, SeasonNo, EpisodeNo, true, false);
                            }
                        }
                        else
                        {
                            searchResultForm = new frmSearchResult(plugin, titleNameSearch, EpisodeName, SeasonNo, EpisodeNo, false, false);
                        }

                        DialogResult result = searchResultForm.ShowDialog(); // ShowResults(plugin.GetAvailableTitles());
                        if (result == DialogResult.OK)
                        {
                            Cursor = Cursors.WaitCursor;
                            Title searchresult = OMLSDK.SDKUtilities.ConvertOMLSDKTitleToTitle(plugin.PluginDLL.GetTitle(searchResultForm.SelectedTitleIndex));
                            title.MetadataSourceName = plugin.DataProviderName;

                            if (searchresult != null)
                            {
                                if (coverArtOnly)
                                {
                                    title.FrontCoverPath = searchresult.FrontCoverPath;
                                    title.BackCoverPath = searchresult.BackCoverPath;
                                }
                                else
                                {
                                    title.CopyMetadata(searchresult,
                                        OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataManual,
                                        OMLEngine.Settings.OMLSettings.MetadataLookupUpdateNameManual,
                                        OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataManual);
                                }

                                LoadFanartFromPlugin(plugin, title);

                            }
                            CheckGenresAgainstSupported(title);
                            titleEditor.RefreshEditor(); 
                            Cursor = Cursors.Default;

                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        // Preferred lookup. Offload the search to the MetadataSearchManagement class
                        Title searchresult = null;

                        MetadataSearchManagement mds = new MetadataSearchManagement(_metadataPlugins);

                        bool retval = mds.MetadataSearchUsingPreferred(title);

                        if (retval)
                        {    
                            // Successful lookup, process
                            LoadFanart(mds.FanArt, title); 
                            titleEditor.RefreshEditor();
                            Cursor = Cursors.Default;
                            return true;
                        }
                    }
                }
                Cursor = Cursors.Default;
                return false;

            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[OMLDatabaseEditor] Exception {0}", ex);
                Cursor = Cursors.Default;
                return false;
            }
        }


        private void LoadFanartFromPlugin(MetaDataPluginDescriptor metadata, Title title)
        {
            if ((metadata.DataProviderCapabilities & MetadataPluginCapabilities.SupportsBackDrops) != 0)
            {
                List<string> _images = metadata.PluginDLL.GetBackDropUrlsForTitle();
                if (_images != null)
                {
                    LoadFanart(metadata.PluginDLL.GetBackDropUrlsForTitle(), title);
                }
            }
        }

        private void LoadFanart(List<string> _images, Title title)
        {
            List<string> images = new List<string>();

            foreach (string image in _images)
            {
                if (images.Count < OMLEngine.Settings.OMLSettings.MetadataLookupMaxFanartQty)
                {
                    if (!ImageManager.CheckImageOriginalNameTitleThreadSafe(title.Id, image))
                    {
                        images.Add(image);
                    }
                }
            }

            if (images.Count > 0)
            {
                DownloadingBackDropsForm dbdForm =
                    new DownloadingBackDropsForm(title, images);

                dbdForm.ShowDialog();
            }
        }

        private void CheckGenresAgainstSupported(Title title)
        {
            List<String> genreList = new List<String>();
 //           if (Properties.Settings.Default.gsValidGenres != null
 //           && Properties.Settings.Default.gsValidGenres.Count > 0)
            int genreCount = TitleCollectionManager.GetAllGenreMetaDatas().Count();
            if (genreCount > 0)
            {
                //String[] arrGenre = new String[genreCount];
                //Properties.Settings.Default.gsValidGenres.CopyTo(arrGenre, 0);
                genreList.AddRange(from gm in TitleCollectionManager.GetAllGenreMetaDatas()
                                       select gm.Name);
                Dictionary<string, string> genreIssuesList = new Dictionary<string, string>();
                Dictionary<string, string> genreChanges = new Dictionary<string, string>();
                foreach (string genre in title.Genres)
                {
                    string newGenre = genre.Trim();
                    if (!genreList.Contains(newGenre))
                    {
                        if (OMLEngine.Settings.SettingsManager.GenreMap_GetMapping(newGenre) != null)
                        {
                            // Mapping already exists for genre
                            genreChanges[genre] = OMLEngine.Settings.SettingsManager.GenreMap_GetMapping(newGenre);
                        }
                        else
                        {
                            if (newGenre.EndsWith("Film", true, CultureInfo.InvariantCulture))
                                newGenre = newGenre.Replace(" Film", "");
                            if (genreList.Contains(newGenre))
                                genreIssuesList[genre] = newGenre;
                            else
                            {
                                string match = genreList.FirstOrDefault(s => s.Split(' ').Intersect(newGenre.Split(' ')).Count() != 0);
                                genreIssuesList[genre] = match;
                            }
                        }
                    }
                }
                foreach (string genre in genreChanges.Keys)
                {
                    //title.Genres.Remove(genre);
                    title.RemoveGenre(genre);
                    // Mapping contains empty string when user wants a specific genre ignored.
                    if (!String.IsNullOrEmpty(genreChanges[genre]) && !title.Genres.Contains(genreChanges[genre]))
                        title.AddGenre(genreChanges[genre]);
                        //title.Genres.Add(genreChanges[genre]);
                }
                if (genreIssuesList.Keys.Count > 0)
                {
                    ResolveGenres resolveGenres = new ResolveGenres(genreIssuesList, title);
                    resolveGenres.ShowDialog();
                }
            }
        }
        #endregion


        #region UI Event Handlers
        private void ToolStripOptionClick(object sender, EventArgs e)
        {
            if (sender == saveToolStripButton || sender == saveToolStripMenuItem)
            {
                if (mainNav.ActiveGroup == groupMediaTree)
                {
                    SaveChanges();
                    ToggleSaveState(false);
                    this.Text = APP_TITLE + " - " + titleEditor.EditedTitle.Name;
                }
                if (mainNav.ActiveGroup == groupGenresMetadata)
                {
                    TitleCollectionManager.SaveGenreMetaDataChanges();
                    genreMetaDataEditor.Status = GenreMetaDataEditor.GenreMetaDataStatus.Normal;
                    this.Text = APP_TITLE + " - " + genreMetaDataEditor.EditedGenreMetaData.Name;

                }
                if (mainNav.ActiveGroup == groupBioData)
                {
                    TitleCollectionManager.SaveBioMetaDataChanges();
                    bioDataEditor.Status = BioDataEditor.BioDataStatus.Normal;
                    this.Text = APP_TITLE + " - " + bioDataEditor.EditedBioData.FullName;
                }
            } 
            else if (sender == exitToolStripMenuItem)
            {
                SaveCurrentMovie();
                Application.Exit();
            }
            else if (sender == optionsToolStripMenuItem)
            {
                Options options = new Options();
                options.Owner = this;
                if (options.ShowDialog(this) == DialogResult.OK)
                {
                    if (options.OptionsDirty)
                    {
                        this.titleEditor.SetMRULists();
                        fromPreferredSourcesToolStripMenuItem1.Visible = !String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPluginMovies);
                    }
                    PopulateMovieListV2(SelectedTreeRoot);
                }
            }
            else if (sender == aboutToolStripMenuItem)
            {
                AboutOML about = new AboutOML();
                about.Show();
            }
            else if (sender == moveDisksToolStripMenuItem)
            {
                // Build list of folders in current title list view
                List<int> titleid = new List<int>();

                foreach (ListViewItem Item in lvTitles.Items)
                {
                    if (Item.Text != "All Media")
                    {
                        titleid.Add(Convert.ToInt32(Item.Text));
                    }
                }
 
                List<string> folders = (from title in _movieList 
                                        where titleid.Contains(title.Key)
                                        from disk in title.Value.Disks
                                        orderby System.IO.Path.GetDirectoryName(disk.Path) ascending
                                        select System.IO.Path.GetDirectoryName(disk.Path)).Distinct().ToList<string>();


                DiskMoverFrm dsm = new DiskMoverFrm(folders);

                if (dsm.ShowDialog(this) == DialogResult.OK)
                {
                    string fromFolder = dsm.fromFolder;
                    string toFolder = dsm.toFolder;

                    List<Disk> disks = (from title in _movieList
                                        from disk in title.Value.Disks
                                        where System.IO.Path.GetDirectoryName(disk.Path).StartsWith(fromFolder)
                                        select disk).ToList();

                    foreach (Disk disk in disks)
                    {
                        disk.Path = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(disk.Path).Replace(fromFolder, toFolder),
                            System.IO.Path.GetFileName(disk.Path));
                    }
                    /*if (dsm.withImages)
                    {
                        if (title.FrontCoverPath.StartsWith(fromFolder))
                        {
                            title.FrontCoverPath = title.FrontCoverPath.Replace(fromFolder, toFolder);
                        }
                        if (title.FrontCoverMenuPath.StartsWith(fromFolder))
                        {
                            title.FrontCoverMenuPath = title.FrontCoverMenuPath.Replace(fromFolder, toFolder);
                        }
                        if (title.BackCoverPath.StartsWith(fromFolder))
                        {
                            title.BackCoverPath = title.BackCoverPath.Replace(fromFolder, toFolder);
                        }
                    }*/

                    TitleCollectionManager.SaveTitleUpdates();
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (titleEditor.EditedTitle != null)
            {
                Title titleToRemove = TitleCollectionManager.GetTitle(titleEditor.EditedTitle.Id);
                if (titleToRemove != null)
                {
                    DialogResult result = XtraMessageBox.Show("Are you sure you want to delete " + titleToRemove.Name + "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        TitleCollectionManager.DeleteTitle(titleToRemove);                        
                        titleEditor.ClearEditor(true);

                        LoadMovies();
                        PopulateMovieListV2(SelectedTreeRoot);
                    }
                    this.Text = APP_TITLE;
                }
            }
        }

        private void MainEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = SaveCurrentMovie() == DialogResult.Cancel;

            Properties.Settings.Default.Location = this.DesktopBounds.Location;
            Properties.Settings.Default.Size = this.DesktopBounds.Size;
            Properties.Settings.Default.WindowState = Enum.GetName(typeof(FormWindowState), this.WindowState);

            Properties.Settings.Default.Save();
        }

        private void metaDataSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MetaDataSettings settings = new MetaDataSettings();
            settings.Show(_metadataPlugins);
        }

        private void deleteAllMoviesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = XtraMessageBox.Show("Are you sure you want to delete all the movies?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                TitleCollectionManager.DeleteAllTitles();
                LoadMovies();
                PopulateMediaTree();
                PopulateMovieListV2(SelectedTreeRoot);
            }
        }

        private void fromScratchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTitle(null, "New Title", TitleTypes.Unknown, null, true);
            /*
            Title newMovie = new Title();
            newMovie.Name = "New title";
            newMovie.TitleType = TitleTypes.Root | TitleTypes.Unknown;
            newMovie.DateAdded = DateTime.Now;

            // Add the title now to get the title ID
            TitleCollectionManager.AddTitle(newMovie);

            titleEditor.LoadDVD(newMovie);
            ToggleSaveState(true);*/
        }

        private void fromMetaDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
            MetaDataPluginDescriptor plugin = selectedItem.Tag as MetaDataPluginDescriptor;
            // Ask for name of movie
            NewMovieName movieName = new NewMovieName();
            if (movieName.ShowDialog() == DialogResult.OK)
            {
                CreateTitle(null, movieName.MovieName(), TitleTypes.Unknown, null, true);
                /*TitleTypes*string name = movieName.MovieName();
                Title newTitle = new Title();
                newTitle.DateAdded = DateTime.Now;
                newTitle.Name = name;

                // Add the title now to get the title ID
                TitleCollectionManager.AddTitle(newTitle);

                titleEditor.LoadDVD(newTitle);*/
                StartMetadataImport(titleEditor.EditedTitle, plugin, false);
                this.Text = APP_TITLE + " - " + titleEditor.EditedTitle.Name + "*";
                ToggleSaveState(true);
            }
        }

        private void currentMovieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (titleEditor.EditedTitle != null)
                //titleEditor.EditedTitle.BuildResizedMenuImage();
        }

        private void allMoviesToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            //foreach (Title t in TitleCollectionManager.GetAllTitles())
                //t.BuildResizedMenuImage();

            // saves all the updates
            TitleCollectionManager.SaveTitleUpdates();
        }

        private void miMetadataMulti_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            SaveCurrentMovie();
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
            MetaDataPluginDescriptor plugin = selectedItem.Tag as MetaDataPluginDescriptor;

            //BaseListBoxControl.SelectedItemCollection collection = lbMovies.SelectedItems;
            pgbProgress.Visible = true;
            pgbProgress.Maximum = lvTitles.SelectedItems.Count;
            pgbProgress.Value = 0;

            ListView.SelectedListViewItemCollection sic = lvTitles.SelectedItems;

            foreach (ListViewItem item in sic)
            {
                Title title = _movieList[Convert.ToInt32(item.Text)];

                pgbProgress.Value++;
                statusText.Text = "Getting metadata for " + title.Name;
                Application.DoEvents();
                titleEditor.LoadDVD(title);
                this.Text = APP_TITLE + " - " + title.Name;
                ToggleSaveState(false);

                if (StartMetadataImport(title, plugin, false))
                {
                    //_titleCollection.Replace(titleEditor.EditedTitle);
                    //_titleCollection.saveTitleCollection();

                    // todo : solomon : need to see how this update is happening since it'll update
                    // existing ones it pulls out out of the db but not new ones
                    TitleCollectionManager.SaveTitleUpdates();
                } 
                titleEditor.LoadDVD(title);
            }
            statusText.Text = "Finished updating metadata";
            pgbProgress.Visible = false;
            Application.DoEvents();
            Cursor = Cursors.Default;
        }

        private void fromPreferredSourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pgbProgress.Visible = true;
            pgbProgress.Maximum = lvTitles.SelectedItems.Count;
            pgbProgress.Value = 0;

            ListView.SelectedListViewItemCollection sic = lvTitles.SelectedItems;

            foreach (ListViewItem item in sic)
            {
                Title title = _movieList[Convert.ToInt32(item.Text)];

                pgbProgress.Value++;
                statusText.Text = "Getting metadata for " + title.Name;
                Application.DoEvents();
                titleEditor.LoadDVD(title);
                this.Text = APP_TITLE + " - " + title.Name;
                ToggleSaveState(false);

                if (StartMetadataImport(title, null, false))
                {
                    //_titleCollection.Replace(titleEditor.EditedTitle);
                    TitleCollectionManager.SaveTitleUpdates();
                }
                titleEditor.LoadDVD(title);
            }
            statusText.Text = "Finished updating metadata";
            pgbProgress.Visible = false;
            Application.DoEvents();
        }

        private void exportCurrentMovieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (titleEditor.EditedTitle != null)
            {
                Title _currentTitle = TitleCollectionManager.GetTitle(titleEditor.EditedTitle.Id);
                if (_currentTitle.Disks.Count > 0)
                    _currentTitle.SerializeToXMLFile(_currentTitle.Disks[0].Path + ".oml.xml");
            }
        }

        private void exportAllMoviesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Cursor = Cursors.WaitCursor;
            foreach (Title t in TitleCollectionManager.GetAllTitles(TitleTypes.AllMedia).ToList())
            {
                if (t.Disks.Count > 0)
                    t.SerializeToXMLFile(t.Disks[0].Path + ".oml.xml");
            }
            Cursor = Cursors.Default;
        }

        private void transcoderDiagnosticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mediafile = "";
            // Search for a movie file in selected title/titles

            ListView.SelectedListViewItemCollection sic = lvTitles.SelectedItems;

            foreach (ListViewItem item in sic)
            {
                Title title = _movieList[Convert.ToInt32(item.Text)];

                foreach (Disk disk in title.Disks)
                {
                    if (disk.Path != "")
                    {
                        if (mediafile == "")
                        {
                            mediafile = disk.Path;
                        }
                    }

                }
            }
            if (mediafile != "")
            {
                try
                {
                    Process pr = new Process();
                    pr.StartInfo.FileName = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)) + @"\openmedialibrary\TranscoderTester.exe";
                    pr.StartInfo.Arguments = "\"" + mediafile + "\"";
                    pr.Start();
                }
                catch
                {
                }
            }
        }

        private void manageMetadataMappingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MetadataMappings mdm = new MetadataMappings(this.titleEditor);
            mdm.ShowDialog();
        }

        private void databaseToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DatabaseTools dt = new DatabaseTools();
            dt.ShowDialog();
        }

        private void lbMetadata_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Cursor = Cursors.WaitCursor;
            MetaDataPluginDescriptor metadata = lbMetadata.SelectedItem as MetaDataPluginDescriptor;
            if (metadata != null)
            {
                StartMetadataImport(titleEditor.EditedTitle, metadata, false);
                lbMetadata.SelectedItem = null;
                lbMetadata.SelectedIndex = -1;
            }
            Cursor = Cursors.Default;
        }

        private void lbImport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Cursor = Cursors.WaitCursor;
            OMLPlugin importer = lbImport.SelectedItem as OMLPlugin;
            if (importer != null)
            {
                lblCurrentStatus.Text = "Importing movies";
                Importer imp = new Importer(importer);
                imp.ShowDialog();

                lblCurrentStatus.Text = "";

                this.Refresh();

                lbImport.SelectedItem = null;
                lbImport.SelectedIndex = -1;
            }
            Cursor = Cursors.Default;
        }

        private void alwaysShowTitleListToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (alwaysShowTitleListToolStripMenuItem.Checked)
            {
                OMLEngine.Settings.OMLSettings.DBEAlwaysShowTitleList = true;
                splitContainerDetails.PanelVisibility = SplitPanelVisibility.Both;
            }
            else
            {
                OMLEngine.Settings.OMLSettings.DBEAlwaysShowTitleList = false;

                if ((mainNav.ActiveGroup == groupMetadata) ||
                    (mainNav.ActiveGroup == groupImport) ||
                    (mainNav.ActiveGroup == groupBioData) ||
                    (mainNav.ActiveGroup == groupTags) ||
                    (mainNav.ActiveGroup == groupGenresMetadata))
                {
                    splitContainerDetails.PanelVisibility = SplitPanelVisibility.Panel2;
                }
                else
                {
                    splitContainerDetails.PanelVisibility = SplitPanelVisibility.Both;
                }
            }
        }

        private void showAllItemsInTitleListToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            OMLEngine.Settings.OMLSettings.ShowSubFolderTitles = showAllItemsInTitleListToolStripMenuItem.Checked;
            PopulateMovieListV2(SelectedTreeRoot);
        }

        #endregion


        #region NavBar functions
        private void mainNav_NavPaneStateChanged(object sender, EventArgs e)
        {
            if (mainNav.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Collapsed)
            {
                splitContainerNavigator.SplitterPosition = 50;
            }
            else
            {
                splitContainerNavigator.SplitterPosition = mainNav.OptionsNavPane.ExpandedWidth + 4;
            }

        }

        bool cancellingnavchange = false;
        private void mainNav_ActiveGroupChanged(object sender, DevExpress.XtraNavBar.NavBarGroupEventArgs e)
        {
            if (cancellingnavchange) return;
            
            // We are changing selected navigation, check if there are any unsaved changes before moving on
            if (CurrentNavigation == groupMediaTree)
            {
                if (SaveCurrentMovie() == DialogResult.Cancel)
                {
                    cancellingnavchange = true;
                    mainNav.ActiveGroup = groupMediaTree;
                    cancellingnavchange = false;
                    return;
                }
            }
            if (CurrentNavigation == groupBioData)
            {
                if (SaveCurrentBioData() == DialogResult.Cancel)
                {
                    cancellingnavchange = true;
                    mainNav.ActiveGroup = groupBioData;
                    cancellingnavchange = false;
                    return;
                }
            }
            if (CurrentNavigation == groupGenresMetadata)
            {
                if (SaveCurrentGenreMetaData() == DialogResult.Cancel)
                {
                    cancellingnavchange = true;
                    mainNav.ActiveGroup = groupGenresMetadata;
                    cancellingnavchange = false;
                    return;
                }
            }



            _loading = true;
            ToggleSaveState(false);

            CurrentNavigation = e.Group;

            if (e.Group == groupMetadata)
                LoadMetadata();

            if (e.Group == groupImport)
                LoadImporters();

            if (e.Group == groupMediaTree)
            {
                PopulateMediaTree();

                LoadMovies();
                PopulateMovieListV2(SelectedTreeRoot);

                splitContainerDetails.Panel2.Controls["bioDataEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["genreMetaDataEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["titlesListView"].Visible = false;
                splitContainerDetails.Panel2.Controls["titleEditor"].Visible = true;

                splitContainerDetails.PanelVisibility = SplitPanelVisibility.Both;
            }

            if (e.Group == groupBioData)
            {
                if (lbBioData.Items.Count == 0)
                {
                    // Only load the once
                    lbBioData.Items.AddRange(TitleCollectionManager.GetAllBioDatas().OrderBy(b => b.FullName).ToArray());
                }
                splitContainerDetails.Panel2.Controls["titleEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["genreMetaDataEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["titlesListView"].Visible = false;
                splitContainerDetails.Panel2.Controls["bioDataEditor"].Visible = true;
                
                bioDataEditor.RefreshEditor();

                SetWindowsTitleBioData();

                if (OMLEngine.Settings.OMLSettings.DBEAlwaysShowTitleList == true)
                {
                    splitContainerDetails.PanelVisibility = SplitPanelVisibility.Both;
                }
                else
                {
                    splitContainerDetails.PanelVisibility = SplitPanelVisibility.Panel2;
                }
            }

            if (e.Group == groupGenresMetadata)
            {
                if (lbGenreMetadata.Items.Count == 0)
                {
                    // Only load the once
                    lbGenreMetadata.Items.AddRange(TitleCollectionManager.GetAllGenreMetaDatas().OrderBy(g => g.Name).ToArray());
                }
                splitContainerDetails.Panel2.Controls["titleEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["bioDataEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["titlesListView"].Visible = false;
                splitContainerDetails.Panel2.Controls["genreMetaDataEditor"].Visible = true;

                genreMetaDataEditor.RefreshEditor();

                SetWindowsTitleGenreMetaData();
                if (OMLEngine.Settings.OMLSettings.DBEAlwaysShowTitleList == true)
                {
                    splitContainerDetails.PanelVisibility = SplitPanelVisibility.Both;
                }
                else
                {
                    splitContainerDetails.PanelVisibility = SplitPanelVisibility.Panel2;
                }
            }

            if (e.Group == groupTags)
            {
                lbTags.Items.Clear();
                lbTags.Items.AddRange(TitleCollectionManager.GetAllTagsList().OrderBy(g => g).ToArray());

                splitContainerDetails.Panel2.Controls["titleEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["bioDataEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["genreMetaDataEditor"].Visible = false;
                splitContainerDetails.Panel2.Controls["titlesListView"].Visible = true;

                if (OMLEngine.Settings.OMLSettings.DBEAlwaysShowTitleList == true)
                {
                    splitContainerDetails.PanelVisibility = SplitPanelVisibility.Both;
                }
                else
                {
                    splitContainerDetails.PanelVisibility = SplitPanelVisibility.Panel2;
                }
            }
            _loading = false;
        }

        private void navBarControl1_NavPaneStateChanged(object sender, EventArgs e)
        {
            if (navBarControl1.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Collapsed)
            {
                splitContainerDetails.SplitterPosition = 38;
            }
            else
            {
                splitContainerDetails.SplitterPosition = navBarControl1.OptionsNavPane.ExpandedWidth + 4;
            }
        }
        #endregion


        #region Media Tree handling functions
        /// <summary>
        /// This populates the media tree view
        /// </summary>
        private void PopulateMediaTree()
        {
            Dictionary<int, Title> mediatreefolders = TitleCollectionManager.GetAllTitles(TitleTypes.AllFolders).ToDictionary(k => k.Id);
            Dictionary<int, int> _parentchildRelationship  = new Dictionary<int, int>();  // titleid, parentid

            if (_mediaTree == null)
            {
                _mediaTree = new Dictionary<int, TreeNode>();
            }
            else
            {
                _mediaTree.Clear();
            }

            treeMedia.Nodes.Clear();
            TreeNode rootnode = new TreeNode("All Media");
            rootnode.Name = "All Media";
            treeMedia.Nodes.Add(rootnode);
            //SelectedTreeRoot = null;

            foreach (KeyValuePair<int, Title> title in mediatreefolders)
            {
                TreeNode tn = new TreeNode(title.Value.Name);
                tn.Name = title.Value.Id.ToString();
                _mediaTree[title.Value.Id] = tn;
                if (title.Value.ParentTitleId != null)
                {
                    _parentchildRelationship[title.Value.Id] = (int)title.Value.ParentTitleId;
                }
                //if (title.Value.Id == title.Value.ParentTitleId) { rootnode.Nodes.Add(tn); }
                if ((title.Value.TitleType & TitleTypes.Root) != 0) { rootnode.Nodes.Add(tn); }
            }


            foreach (KeyValuePair<int, TreeNode> kvp in _mediaTree)
            {
                if (_parentchildRelationship.ContainsKey(kvp.Key))
                {
                    if (kvp.Key != _parentchildRelationship[kvp.Key])
                    {
                        // This title has a parent.
                        if (_mediaTree.ContainsKey(_parentchildRelationship[kvp.Key]))
                        {
                            _mediaTree[_parentchildRelationship[kvp.Key]].Nodes.Add(_mediaTree[kvp.Key]);
                        }
                    }
                }
            }
        }

        private void treeMedia_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node.Bounds.X == 0)
            {
                // Stop occasional paint corruption
                return;
            }

            // Create the brushes
            if (_brushTreeViewSelected == null)
            {
                _brushTreeViewSelected = new LinearGradientBrush(new Point(0, 0), new Point(0, e.Bounds.Height), Color.LimeGreen, Color.PaleGreen);
            }


            int x = e.Node.Bounds.X;
            int y = e.Node.Bounds.Y;
            int w = e.Node.Bounds.Width;
            int h = e.Node.Bounds.Height;
            int wt = (int)e.Graphics.MeasureString(e.Node.Text, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular)).Width + 2;

            // Setup string formatting
            StringFormat stf = new StringFormat();
            stf.Trimming = StringTrimming.EllipsisCharacter;
            stf.FormatFlags = StringFormatFlags.NoWrap;

            e.Graphics.FillRectangle(new SolidBrush(Color.White), x, y, w, h);

            if (e.Node.IsSelected)
            {
                e.Graphics.FillRectangle(_brushTreeViewSelected, x, y, wt, h);
                e.Graphics.DrawLine(new Pen(Color.Black), x + 1, y, x - 2 + wt, y);
                e.Graphics.DrawLine(new Pen(Color.Black), x + 1, y - 1 + h, x - 2 + wt, y - 1 + h);
                e.Graphics.DrawLine(new Pen(Color.Black), x, y + 1, x, y - 2 + h);
                e.Graphics.DrawLine(new Pen(Color.Black), x - 1 + wt, y + 1, x - 1 + wt, y - 2 + h);

            }

            e.Graphics.DrawString(e.Node.Text, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(x, y + 2, e.Bounds.Width, h), stf);

            if (e.Node.Nodes.Count != 0)
            {
                if (e.Node.IsExpanded == true)
                {
                    Point p1 = new Point(x - 5, y + 5);
                    Point p2 = new Point(x - 13, y + 13);
                    Point p3 = new Point(x - 5, y + 13);
                    e.Graphics.FillPolygon(new SolidBrush(Color.Black), new Point[] { p1, p2, p3 }, FillMode.Winding);
                }
                else
                {
                    Point p1 = new Point(x - 11, y + 5);
                    Point p2 = new Point(x - 5, y + 9);
                    Point p3 = new Point(x - 11, y + 13);
                    e.Graphics.DrawLine(new Pen(Color.Black), p1, p2);
                    e.Graphics.DrawLine(new Pen(Color.Black), p2, p3);
                    e.Graphics.DrawLine(new Pen(Color.Black), p3, p1);
                }
            }
        }

        private void treeMedia_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lvTitles.Items.Clear();
            if (!string.IsNullOrEmpty(e.Node.Name))
            {
                if (treeMedia.SelectedNode.Name == "All Media")
                {
                    // Root Level selected
                    SelectedTreeRoot = null;    
                    PopulateMovieListV2(SelectedTreeRoot);
                    titleEditor.ClearEditor(true);
                }
                else
                {
                    SelectedTreeRoot = Convert.ToInt32(e.Node.Name);
                    PopulateMovieListV2(SelectedTreeRoot);
                    titleEditor.LoadDVD(_movieList[(int)SelectedTreeRoot]);
                }
            }
            else
            {
                PopulateMovieListV2(null);
            }
        }

        private void cmsMediaTree_Opening(object sender, CancelEventArgs e)
        {
            if (treeMedia.SelectedNode == null ||
                treeMedia.SelectedNode.Name == "All Media")
            {
                //miCreateMovieCollection.Visible = true;
                //miCreateFolderTVShow.Visible = true;
                //miCreateFolderTVSeason.Visible = false;
                //miCreateMovie.Visible = true;
                //miCreateTVEpisode.Visible = false;
                miDeleteFolder.Visible = false;
            }
            else
            {
                miDeleteFolder.Visible = true;
            }
              /*  int parentid = Convert.ToInt32(treeMedia.SelectedNode.Name);
                if ((_movieList[parentid].TitleType & TitleTypes.Collection) != 0)
                {
                    // Allow Collection and TV Show
                    //miCreateMovieCollection.Visible = true;
                    //miCreateFolderTVShow.Visible = true;
                    //miCreateFolderTVSeason.Visible = false;
                    //miCreateMovie.Visible = true;
                    //miCreateTVEpisode.Visible = false;
                    miDeleteFolder.Visible = true;
                }
                if ((_movieList[parentid].TitleType & TitleTypes.TVShow) != 0)
                {
                    // Allow Season
                    //miCreateMovieCollection.Visible = false;
                    //miCreateFolderTVShow.Visible = false;
                    //miCreateFolderTVSeason.Visible = true;
                    //miCreateMovie.Visible = false;
                    //miCreateTVEpisode.Visible = true;
                    //miDeleteFolder.Visible = true;
                }
                if ((_movieList[parentid].TitleType & TitleTypes.Season) != 0)
                {
                    // Allow Season
                    //miCreateMovieCollection.Visible = false;
                    //miCreateFolderTVShow.Visible = false;
                    //miCreateFolderTVSeason.Visible = false;
                    //miCreateMovie.Visible = false;
                    //miCreateTVEpisode.Visible = true;
                    //miDeleteFolder.Visible = true;
                }
            }*/
        }
 
        private void miCreateFolder_Click(object sender, EventArgs e)
        {
            CreateFolder("New Movies", TitleTypes.Collection, true);
        }
        
        private void miCreateFolderTVShow_Click(object sender, EventArgs e)
        {
            CreateFolder("New TV Show", TitleTypes.TVShow, true);
        }

        private void miCreateFolderTVSeason_Click(object sender, EventArgs e)
        {
            CreateFolder("New TV Season", TitleTypes.Season, true);
        }
  
        private void miCreateTVEpisode_Click(object sender, EventArgs e)
        {
            if (treeMedia.SelectedNode.Name == "All Media")
            {
                // Root Title
                CreateTitle(null, "New TV Episode", TitleTypes.Episode, null, true);
            }
            else
            {
                int parentid = Convert.ToInt32(treeMedia.SelectedNode.Name);
                CreateTitle(parentid, "New TV Episode", TitleTypes.Episode, null, true);
            }
        }
  
        private void miCreateTitle_Click(object sender, EventArgs e)
        {
            if (treeMedia.SelectedNode.Name == "All Media")
            {
                // Root Title
                CreateTitle(null, "New Movie", TitleTypes.Movie, null, true);
            }
            else
            {
                int parentid = Convert.ToInt32(treeMedia.SelectedNode.Name);
                CreateTitle(parentid, "New Movie", TitleTypes.Movie, null, true);
            }
        }

        private void miDeleteFolder_Click(object sender, EventArgs e)
        {   
            if (treeMedia.SelectedNode.Name != "All Media")
            {
                int titleid = Convert.ToInt32(treeMedia.SelectedNode.Name);
                SelectedTreeRoot = _movieList[titleid].ParentTitleId;

                if (TitleCollectionManager.GetFilteredTitles(TitleFilterType.Parent, titleid.ToString()).Count() == 0)
                {
                    // No child items, delete
                    TitleCollectionManager.DeleteTitle(_movieList[Convert.ToInt32(treeMedia.SelectedNode.Name)]);
                    LoadMovies();
                    PopulateMediaTree();
                    PopulateMovieListV2(SelectedTreeRoot);
                    if (SelectedTreeRoot != null)
                    {
                        treeMedia.SelectedNode = _mediaTree[(int)SelectedTreeRoot];
                    }
                }
                else
                {
                    XtraMessageBox.Show("Cannot delete this folder. Please move or delete any child items.", "Delete error!");
                }
            }
        }

        private void treeMedia_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);
                TreeNode nodeUnderMouse = treeMedia.GetNodeAt(p);
                if (nodeUnderMouse != null)
                {
                    treeMedia.SelectedNode = nodeUnderMouse;
                }
            }
        }

        private void treeMedia_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point p = new Point(e.X, e.Y);
                TreeNode nodeUnderMouse = treeMedia.GetNodeAt(p);

                if (nodeUnderMouse != null)
                {
                    if (e.X > nodeUnderMouse.Bounds.Left)
                    {
                        treeMedia.SelectedNode = nodeUnderMouse;

                        if (nodeUnderMouse.Name != "All Media")
                        {
                            OMLDragAndDropClass OMLDragAndDrop = new OMLDragAndDropClass();
                            OMLDragAndDrop.OMLDragAndDropType = OMLDragAndDropTypes.Title;
                            OMLDragAndDrop.iItems = new int[1];
                            OMLDragAndDrop.iItems[0] = Convert.ToInt32(nodeUnderMouse.Name);

                            treeMedia.DoDragDrop(OMLDragAndDrop, DragDropEffects.Move);
                            //lbTitles.DoDragDrop(sitems, DragDropEffects.Move);
                        }
                    }
                }
            }
        }

        private void treeMedia_DragEnter(object sender, DragEventArgs e)
        {
            currentmovetonode = null;
            treeMedia_DragOver(sender, e);
        }

        private void treeMedia_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (tt != null)
                {
                    tt.Hide(this);
                    tt.Dispose();
                    tt = null;
                }

                Point mouseLocation = treeMedia.PointToClient(new Point(e.X, e.Y));
                TreeNode selectednode = treeMedia.GetNodeAt(mouseLocation);

                if (selectednode != null)
                {
                    this.Cursor = Cursors.WaitCursor;

                    // Find the titleid of the destination title
                    int? parentid;

                    if (selectednode.Name == "All Media")
                    {
                        parentid = null;
                    }
                    else
                    {
                        parentid = Convert.ToInt32(selectednode.Name);
                    }

                    // Received a file drag and drop
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                        if (files.Length > 0)
                        {
                            CreateTitlesFromPathArray(parentid, files);
                        }
                    }

                    // Received an internal drag and drop
                    if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
                    {
                        bool foldermoved = false;

                        OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                        if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Title)
                        {
                            int[] items = OMLDragAndDrop.iItems;
                            foreach (int item in items)
                            {
                                if ((_movieList[item].TitleType & TitleTypes.AllFolders) != 0)
                                {
                                    foldermoved = true;
                                }

                                if (parentid == null)
                                {
                                    // Item is being moved to root
                                    _movieList[item].ParentTitleId = null;
                                    _movieList[item].TitleType = _movieList[item].TitleType | TitleTypes.Root;
                                }
                                else
                                {
                                    // Item is being moved to a folder
                                    _movieList[item].ParentTitleId = parentid;
                                    _movieList[item].TitleType = _movieList[item].TitleType & (TitleTypes.AllMedia | TitleTypes.AllFolders);
                                }
                            }
                            TitleCollectionManager.SaveTitleUpdates();

                            if (foldermoved)
                            {
                                PopulateMediaTree();
                                if (_mediaTree.ContainsKey((int)SelectedTreeRoot))
                                {
                                    treeMedia.SelectedNode = _mediaTree[(int)SelectedTreeRoot];
                                }
                            }

                            PopulateMovieListV2(SelectedTreeRoot);
                        }
                    }
                    this.Cursor = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[OMLDatabaseEditor] treeMedia_DragDrop exception" + ex.Message);
            }
        }

        private void treeMedia_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                if (tt == null)
                {
                    tt = new ToolTip();
                }

                Point mouseLocation = treeMedia.PointToClient(new Point(e.X, e.Y));
                TreeNode movetonode = treeMedia.GetNodeAt(mouseLocation);

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if ((files.Length > 0) && (movetonode != null))
                    {
                        e.Effect = DragDropEffects.Copy;
                        tt.Show("Add " + string.Join(", ", files) + " to " + movetonode.Text, this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                        tt.Show("Unable to move here!", this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                    }
                }

                if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
                {
                    if (currentmovetonode != movetonode)
                    {
                        bool validmove = true;

                        if (movetonode == null)
                        {
                            validmove = false;
                        }
                        else
                        {
                            try
                            {
                                OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                                if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Title)
                                {

                                    int[] items = OMLDragAndDrop.iItems;
                                    foreach (int item in items)
                                    {
                                        if ((_movieList[item].TitleType & TitleTypes.AllFolders) != 0)
                                        {
                                            // Item is a folder - dangerous. Make sure this will not result circular recursion

                                            if (movetonode.Name == "All Media")
                                            {
                                                if (_movieList[item].ParentTitleId == null)
                                                {
                                                    validmove = false;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                // Is it being moved into itself
                                                if (item == Convert.ToInt32(movetonode.Name))
                                                {
                                                    validmove = false;
                                                    break;
                                                }

                                                // Is it being moved to it's parent - it's allready there
                                                if (_movieList[item].ParentTitleId == Convert.ToInt32(movetonode.Name))
                                                {
                                                    validmove = false;
                                                    break;
                                                }

                                                // Check for circular parents
                                                int? parentid = _movieList[Convert.ToInt32(movetonode.Name)].ParentTitleId;
                                                while ((validmove) && (parentid != null))
                                                {
                                                    if ((parentid == item) || (parentid == _movieList[(int)parentid].ParentTitleId)) // Last bit stops run aways if parenttitleid==titleid (shouldn't happen)
                                                    {
                                                        validmove = false;
                                                        break;
                                                    }
                                                    parentid = _movieList[(int)parentid].ParentTitleId;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Utilities.DebugLine("[OMLDatabaseEditor] treeMedia_DragOver Inner exception" + ex.Message);
                            }
                        }

                        if (LastMousePoint != new Point(e.X, e.Y))
                        {
                            LastMousePoint = new Point(e.X, e.Y);
                            if (validmove)
                            {
                                e.Effect = DragDropEffects.Move;
                                tt.Show("Move to " + movetonode.Text, this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                            }
                            else
                            {
                                e.Effect = DragDropEffects.None;
                                tt.Show("Unable to move here!", this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                            }
                            currentmovetonode = movetonode;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[OMLDatabaseEditor] treeMedia_DragOver Outer exception : " + ex.Message);
            }
        }

        private void treeMedia_DragLeave(object sender, EventArgs e)
        {
            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }
        }
        #endregion


        #region TitleList functions
        private void PopulateMovieListV2(int? roottitleid)
        {
            CountMovies();

            this.Cursor = Cursors.WaitCursor;

            lvTitles.Items.Clear();
            if (roottitleid != null)
            {
                // We are looking at a parent item
                PopulateMovieListV2Sub(roottitleid, 0);
            }
            else
            {
                lvTitles.Items.Add("All Media", "All Media", null);
             
                // Get All Root Titles
                var titles = SortTitles((from t in _movieList
                              where (t.Value.TitleType & TitleTypes.AllMedia) != 0 &&
                              (t.Value.TitleType & TitleTypes.Root) != 0
                              select t.Value).ToList());

                foreach (Title t in titles)
                {
                    lvTitles.Items.Add(t.Id.ToString(), t.Id.ToString(), null);
                }

                // Get All Root Folders
                titles = (from t in _movieList
                          where (t.Value.TitleType & TitleTypes.AllFolders) != 0 &&
                          (t.Value.TitleType & TitleTypes.Root) != 0
                          select t.Value).ToList();

                foreach (Title title in SortTitles(titles))
                {
                    // This is a folder. Query child items
                    PopulateMovieListV2Sub(title.Id, 1);
                }
            }


            // Disabled
            /*
            if (titleEditor.EditedTitle != null)
            {
                lvTitles.SelectedItems.Clear();
                if (lvTitles.Items.ContainsKey(titleEditor.EditedTitle.Id.ToString()))
                {
                    lvTitles.Items[titleEditor.EditedTitle.Id.ToString()].Selected = true;
                }
            }*/

            // Set the width of the list view column to ensure painting has the correct width
            lvTitles.Columns[0].Width = lvTitles.ClientRectangle.Width;

            this.Cursor = Cursors.Default;
        }

        private void PopulateMovieListV2Sub(int? roottitleid, int Level)
        {
            if ((OMLEngine.Settings.OMLSettings.ShowSubFolderTitles) || (Level < 1))
            {
                if (_movieList.ContainsKey((int)roottitleid))
                {
                    lvTitles.Items.Add(roottitleid.ToString(), roottitleid.ToString(), null);

                    // Get All Root Titles
                    var titles = SortTitles((from t in _movieList
                                  where (t.Value.TitleType & TitleTypes.AllMedia) != 0 &&
                                  t.Value.ParentTitleId == roottitleid
                                  select t.Value).ToList());

                    
                    foreach (Title t in titles)
                    {
                        lvTitles.Items.Add(t.Id.ToString(), t.Id.ToString(), null);
                    }

                    // Get All Root Folders
                    titles = (from t in _movieList
                              where (t.Value.TitleType & TitleTypes.AllFolders) != 0 &&
                              t.Value.ParentTitleId == roottitleid
                              select t.Value).ToList();

                    foreach (Title title in SortTitles(titles))
                    {
                        // If statement to stop stack overflow when parenttitid = id
                        if (title.Id != roottitleid)
                        {
                            PopulateMovieListV2Sub(title.Id, 1);
                        }
                    }
                }
            }
        }

        private void lvTitles_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //if (e.Index < 0) return;

            // Find the printing bounds
            int x = e.Bounds.X;
            int y = e.Bounds.Y;
            //int w = e.Bounds.Width;
            //int w = lvTitles.Size.Width - 20;
            int w = lvTitles.ClientRectangle.Width;
            int h = e.Bounds.Height;

            // Create the brushes
            if (_brushTitleListSelected == null)
            {
                _brushTitleListSelected = new LinearGradientBrush(new Point(0, 0), new Point(0, e.Bounds.Height), Color.LimeGreen, Color.PaleGreen);
                _brushTitleListFolder = new LinearGradientBrush(new Point(0, 0), new Point(0, e.Bounds.Height), Color.Gainsboro, Color.Silver);
                _brushTitleListFolderSelected = new LinearGradientBrush(new Point(0, 0), new Point(0, e.Bounds.Height), Color.Silver, Color.LightGreen);
            }


            Title currentTitle = null;
            int? currentTitleID = null;
            if (e.Item.Text == "All Media")
            {
                currentTitle = new Title();
                currentTitle.Name = e.Item.Text;
                currentTitle.TitleType = TitleTypes.Collection;
            }
            else
            {
                currentTitleID = Convert.ToInt32(e.Item.Text);
                currentTitle = _movieList[(int)currentTitleID];
            }

           
            // Setup string formatting
            StringFormat stf = new StringFormat();
            stf.Trimming = StringTrimming.EllipsisCharacter;
            stf.FormatFlags = StringFormatFlags.NoWrap;

            e.DrawBackground();


            if ((currentTitle.TitleType & TitleTypes.AllFolders) != 0)
            {
                // Folder specific paint goes here
                if (lvTitles.SelectedItems.ContainsKey(currentTitle.Id.ToString()))
                {
                    e.Graphics.FillRectangle(_brushTitleListFolderSelected, x, y, w, h);
                }
                else
                {
                    e.Graphics.FillRectangle(_brushTitleListFolder, x, y, w, h);
                }
                e.Graphics.DrawString(currentTitle.Name, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold), new SolidBrush(Color.Black), new RectangleF(x, y + 2, w - 65, h), stf);
                
                int titleCount = 0;
                if (currentTitleID == null)
                {
                    titleCount = _movieRootCount;
                }
                else
                {
                    if (_movieCount.ContainsKey((int)currentTitleID))
                    {
                        titleCount = _movieCount[(int)currentTitleID];
                    }
                }

                e.Graphics.DrawString("Total titles " + titleCount.ToString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(x, y + 18, w - 65, h), stf);
            
                e.Graphics.DrawString(currentTitle.Name, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold), new SolidBrush(Color.Black), new RectangleF(x, y + 2, w - 65, h), stf);
            }
            else
            {
                if (lvTitles.SelectedItems.ContainsKey(currentTitle.Id.ToString()))
                {
                    e.Graphics.FillRectangle(_brushTitleListSelected, x, y, w, h);
                }

                // Media specific paint goes here
                e.Graphics.DrawString(currentTitle.Name, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(x, y + 2, w - 65, h), stf);
                e.Graphics.DrawString(currentTitle.ReleaseDate.ToShortDateString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(w - 60, y + 2, w, h), stf);
                //e.Graphics.DrawString(currentTitle.Runtime.ToString() + " minutes, " + currentTitle.Studio, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Gray), new RectangleF(8, y + 16, w - 40, h), stf);
                e.Graphics.DrawString(currentTitle.Runtime.ToString() + " minutes", new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Gray), new RectangleF(x + 22, y + 19, w - 102, h), stf);
                
                // Draw percentage complete box
                Image MetaPercentage = ImgMetaPercentage1;

                if (currentTitle.PercentComplete <= .2M)
                {
                    MetaPercentage = ImgMetaPercentage1;
                }
                else if (currentTitle.PercentComplete <= .4M)
                {
                    MetaPercentage = ImgMetaPercentage2;
                }
                else if (currentTitle.PercentComplete <= .6M)
                {
                    MetaPercentage = ImgMetaPercentage3;
                }
                else if (currentTitle.PercentComplete <= .8M)
                {
                    MetaPercentage = ImgMetaPercentage4;
                }
                else 
                {
                    MetaPercentage = ImgMetaPercentage5;
                }

                e.Graphics.DrawImageUnscaled(MetaPercentage,x + 4,y + 18);
                //e.Graphics.FillEllipse(bb, new Rectangle(x + w - 30, y + 16, 14, 14));
                //e.Graphics.DrawEllipse(new Pen(Color.Black), new Rectangle(x + w - 30, y + 16, 14, 14));
            }

            // Common painting goes here
            e.Graphics.DrawLine(new Pen(Color.Gray), 0, y + h - 1, w, y + h - 1);
            
            // Draw rating stars
            Image Stars = null;
            switch (currentTitle.UserStarRating)
            {
                case 0: Stars = ImgStars0; break;
                case 1: 
                case 2: Stars = ImgStars1; break;
                case 3:
                case 4: Stars = ImgStars2; break;
                case 5:
                case 6: Stars = ImgStars3; break;
                case 7:
                case 8:
                case 9: Stars = ImgStars4; break;
                default: Stars = ImgStars5; break;
            }
            e.Graphics.DrawImageUnscaled(Stars, x + w - 82, y + 18);
        }

        private void lvTitles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Setup context menu for title

                // Find titles type selected so we present the right plugins
                bool EpisodeSelected = false;
                bool MovieSelected = false;
                bool MusicVideoSelected = false;
                bool UnknownSelected = false;
                ListView.SelectedListViewItemCollection sic = lvTitles.SelectedItems;

                foreach (ListViewItem item in sic)
                {
                    if (item.Text == "All Media") return;

                    Title title = _movieList[Convert.ToInt32(item.Text)];
                    if ((title.TitleType & TitleTypes.Episode) != 0) EpisodeSelected = true;
                    if ((title.TitleType & TitleTypes.Season) != 0) EpisodeSelected = true;
                    if ((title.TitleType & TitleTypes.TVShow) != 0) EpisodeSelected = true;

                    if ((title.TitleType & TitleTypes.Movie) != 0) MovieSelected = true;
                    if ((title.TitleType & TitleTypes.Video) != 0) MovieSelected = true;
                    if ((title.TitleType & TitleTypes.MusicVideo) != 0) MusicVideoSelected = true;
                    if ((title.TitleType & TitleTypes.Unknown) != 0) UnknownSelected = true;
                }

                // Build custom context menu strip
                ContextMenuStrip cms = new ContextMenuStrip();

                ToolStripMenuItem metadata = new ToolStripMenuItem("Update metadata");
                cms.Items.Add(metadata);

                // Preferred sources
                if (((MovieSelected) && (!String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPluginMovies))) ||
                    ((EpisodeSelected) && (!String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPluginTV))))
                {
                    ToolStripMenuItem metadataItem = new ToolStripMenuItem("From Preferred Sources");
                    metadataItem.Click += new System.EventHandler(this.fromPreferredSourcesToolStripMenuItem_Click);
                    metadata.DropDownItems.Add(metadataItem);
                }

                foreach (MetaDataPluginDescriptor plugin in _metadataPlugins)
                {
                    bool showplugin = false;

                    if (EpisodeSelected &&
                        ((plugin.DataProviderCapabilities & MetadataPluginCapabilities.SupportsTVSearch) != 0)) showplugin = true;

                    if (MovieSelected &&
                        ((plugin.DataProviderCapabilities & MetadataPluginCapabilities.SupportsMovieSearch) != 0)) showplugin = true;

                    if (UnknownSelected || showplugin)
                    {
                        ToolStripMenuItem metadataItem = new ToolStripMenuItem("From " + plugin.DataProviderName);
                        metadataItem.Tag = plugin;
                        metadataItem.Click += new EventHandler(this.miMetadataMulti_Click);
                        metadata.DropDownItems.Add(metadataItem);
                    }
                }

                cms.Items.Add("Delete", null, new System.EventHandler(this.deleteSelectedMoviesToolStripMenuItem_Click));
                cms.Items.Add("Add Tag", null, new System.EventHandler(this.addTagMenuItem1_Click));
                cms.Items.Add("Add Genre", null, new System.EventHandler(this.addGenreMenuItem1_Click));
#if DEBUG
                cms.Items.Add("Create Shortcut", null, new System.EventHandler(this.CreateTitleShortcut_Click));
#endif 
                cms.Show(lvTitles.PointToScreen(e.Location));
            }
        }
      
        private void lvTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvTitles_SelectedIndexChanged();
            //lvTitles.Refresh();
        }

        bool cancellingsave = false;
        private void lvTitles_SelectedIndexChanged()
        {
            if (_loading) return;

            // This gets called on both clearing a selection and selecting a new title.
            // Don't check the clearing
            if (lvTitles.SelectedItems.Count == 0) return;

            Cursor = Cursors.WaitCursor;

            // Is there a save cancel pending
            if (cancellingsave)
            { 
                // Reset current editing title       
                this.lvTitles.SelectedIndexChanged -= new System.EventHandler(this.lvTitles_SelectedIndexChanged);

                lvTitles.SelectedItems.Clear();
                if (lvTitles.Items.ContainsKey(titleEditor.EditedTitle.Id.ToString()))
                {
                    lvTitles.Items[titleEditor.EditedTitle.Id.ToString()].Selected = true;
                }
                cancellingsave = false;
                this.lvTitles.SelectedIndexChanged += new System.EventHandler(this.lvTitles_SelectedIndexChanged);
                Cursor = Cursors.Default;
                return; 
            }


            if (titleEditor.EditedTitle != null && titleEditor.Status == OMLDatabaseEditor.Controls.TitleEditor.TitleStatus.UnsavedChanges)
            {
                // Unsaved changes.
                if (SaveCurrentMovie() == DialogResult.Cancel)
                {
                    cancellingsave = true;
                    _loading = true; //bypasses second save movie dialog

                    lvTitles.SelectedItems.Clear();
                    if (lvTitles.Items.ContainsKey(titleEditor.EditedTitle.Id.ToString()))
                    {
                        lvTitles.Items[titleEditor.EditedTitle.Id.ToString()].Selected = true;
                    }
                    _loading = false;
                    Cursor = Cursors.Default;
                    return;
                }
            }
            
        
            if (lvTitles.SelectedItems.Count == 1)
            {
                if (lvTitles.SelectedItems[0].Text != "All Media")
                {

                    Title selectedTitle = _movieList[Convert.ToInt32(lvTitles.SelectedItems[0].Text)];

                    if (selectedTitle != null)
                    {
                        titleEditor.LoadDVD(selectedTitle);
                        this.Text = APP_TITLE + " - " + selectedTitle.Name;
                    }
                    else
                    {
                        titleEditor.ClearEditor(true);
                        this.Text = APP_TITLE;
                    }
                    ToggleSaveState(false);
                }
                else
                {
                    titleEditor.ClearEditor(true);
                    this.Text = APP_TITLE;
                    ToggleSaveState(false);
                }
            }
            else
            {
                titleEditor.ClearEditor(true);
                this.Text = APP_TITLE;
            }

            Cursor = Cursors.Default;
        }

        private void deleteSelectedMoviesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection sic = lvTitles.SelectedItems;
              
            DialogResult result = XtraMessageBox.Show("Are you sure you want to delete the selected items?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {

                foreach (ListViewItem item in sic)
                {
                    int id = Convert.ToInt32(item.Text);

                    if (titleEditor.EditedTitle != null && titleEditor.EditedTitle.Id == id)
                        titleEditor.ClearEditor(true);

                    TitleCollectionManager.DeleteTitle(_movieList[id]);

                    _movieList.Remove(id);
                }
            }
            //_titleCollection.saveTitleCollection();
            PopulateMovieListV2(SelectedTreeRoot);
        }
        
        int oldsize;
        private void lvTitles_Resize(object sender, EventArgs e)
        {
            if (oldsize != lvTitles.Size.Width)
            {
                lvTitles.Columns[0].Width = lvTitles.ClientRectangle.Width;
                oldsize = lvTitles.Size.Width;

            }
        }

        private void lvTitles_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ListView.SelectedListViewItemCollection sic = lvTitles.SelectedItems;

                int[] sitems = (from ListViewItem si in sic
                                where si.Text != "All Media"
                                select Convert.ToInt32(si.Text)).ToArray();
                if (sitems.Count() != 0)
                {
                    OMLDragAndDropClass OMLDragAndDrop = new OMLDragAndDropClass();
                    OMLDragAndDrop.OMLDragAndDropType = OMLDragAndDropTypes.Title;
                    OMLDragAndDrop.iItems = sitems;

                    lvTitles.DoDragDrop(OMLDragAndDrop, DragDropEffects.Move);
                }
            }
        }
 
        private void lvTitles_DragEnter(object sender, DragEventArgs e)
        {
            currentmovetonode = null;
            lvTitles_DragOver(sender, e);
        }

        private void lvTitles_DragOver(object sender, DragEventArgs e)
        {
            bool validmove = false;

            string[] items = null;
            string title = null;
            if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
            {
                OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                if ((OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Genre) ||
                    (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Person) ||
                    (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Tag))
                {
                    Point pt = lvTitles.PointToClient(new Point(e.X, e.Y));
                                    
                    ListViewItem lvitem = lvTitles.GetItemAt(pt.X, pt.Y);

                    if (lvitem != null)
                    {
                        title = _movieList[int.Parse(lvitem.Text)].Name;
                        validmove = true;
                    }

                    items = OMLDragAndDrop.sItems;
                }
            }

            if (LastMousePoint != new Point(e.X, e.Y))
            {
                LastMousePoint = new Point(e.X, e.Y);

                if (tt == null)
                {
                    tt = new ToolTip();
                }

                if (validmove)
                {
                    e.Effect = DragDropEffects.Move;
                    tt.Show("Add " + string.Join(", ", items) + " to " + title, this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    tt.Show("Invalid Drag", this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                }
            }
        }

        private void lvTitles_DragLeave(object sender, EventArgs e)
        {
            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }
        }

        private void lvTitles_DragDrop(object sender, DragEventArgs e)
        {
            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }

            if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
            {
                this.Cursor = Cursors.WaitCursor;
                
                OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                Point pt = lvTitles.PointToClient(new Point(e.X, e.Y));

                ListViewItem lvitem = lvTitles.GetItemAt(pt.X, pt.Y);

                if (lvitem != null)
                {
                    Title title = _movieList[int.Parse(lvitem.Text)];
                    if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Genre)
                    {
                        foreach (string Genre in OMLDragAndDrop.sItems)
                        {
                            if (!title.Genres.Contains(Genre))
                            {
                                title.AddGenre(Genre);
                            }
                        }
                    }

                    if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Person)
                    {
                        foreach (string Person in OMLDragAndDrop.sItems)
                        {
                            if ((from t in title.ActingRoles
                                 where t.PersonName == Person
                                 select t.PersonName).Count() == 0)
                            {
                                title.AddActingRole(Person, "");
                            }
                        }
                    } 
                    
                    if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Tag)
                    {
                        foreach (string Tag in OMLDragAndDrop.sItems)
                        {
                            if (!title.Tags.Contains(Tag))
                            {
                                title.AddTag(Tag);
                            }
                        }
                    }
                }
                TitleCollectionManager.SaveTitleUpdates();
                this.Cursor = Cursors.Default;
            }
        }
        #endregion


        #region GenreMetaData Editing functions
        private void lbGenreMetadata_DrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            e.Handled = true;

            // Create the brushes
            if (_brushTreeViewSelected == null)
            {
                _brushTreeViewSelected = new LinearGradientBrush(new Point(0, 0), new Point(0, e.Bounds.Height), Color.LimeGreen, Color.PaleGreen);
            }


            int x = e.Bounds.X + 4;
            int y = e.Bounds.Y;
            int w = e.Bounds.Width;
            int h = e.Bounds.Height;
            int wt = (int)e.Graphics.MeasureString(e.Item.ToString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular)).Width + 2;

            // Setup string formatting
            StringFormat stf = new StringFormat();
            stf.Trimming = StringTrimming.EllipsisCharacter;
            stf.FormatFlags = StringFormatFlags.NoWrap;

            e.Graphics.FillRectangle(new SolidBrush(Color.White), x, y, w, h);

            bool highlight = false;
            foreach (int selectedindex in lbGenreMetadata.SelectedIndices)
            {
                if (lbGenreMetadata.Items[selectedindex] == e.Item)
                {
                    highlight = true;
                }
            }

            if (highlight)
            //if (lbGenreMetadata.SelectedItem == e.Item)
            {
                e.Graphics.FillRectangle(_brushTreeViewSelected, x, y, wt, h);
                e.Graphics.DrawLine(new Pen(Color.Black), x + 1, y, x - 2 + wt, y);
                e.Graphics.DrawLine(new Pen(Color.Black), x + 1, y - 1 + h, x - 2 + wt, y - 1 + h);
                e.Graphics.DrawLine(new Pen(Color.Black), x, y + 1, x, y - 2 + h);
                e.Graphics.DrawLine(new Pen(Color.Black), x - 1 + wt, y + 1, x - 1 + wt, y - 2 + h);

            }

            e.Graphics.DrawString(e.Item.ToString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(x, y + 2, e.Bounds.Width, h), stf);
        }
         
        private void lbGenreMetadata_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cancellingsave)
            {
                // Check if there are any unsaved changes, prompt to save
                DialogResult dr = SaveCurrentGenreMetaData();

                if ((dr == DialogResult.Yes) || (dr == DialogResult.No))
                {
                    // Data is properly saved or aborted, select new genre to edit
                    if (lbGenreMetadata.SelectedItem != null)
                    {
                        if (genreMetaDataEditor.EditedGenreMetaData != lbGenreMetadata.SelectedItem)
                        {
                            // If a different genre is selected, load it into the editor
                            genreMetaDataEditor.LoadGenre((GenreMetaData)lbGenreMetadata.SelectedItem);
                            SetWindowsTitleGenreMetaData();
                        }
                    }
                }
            }
        }

        private void AddGenreMetaData()
        {
            SaveCurrentGenreMetaData();

            var e = from Object li in lbGenreMetadata.Items
                    where li.ToString() == "New Genre"
                    select li;

            if (e.Count() == 0)
            {
                GenreMetaData gmd = new GenreMetaData();
                gmd.Name = "New Genre";

                lbGenreMetadata.Items.Add(gmd);
                lbGenreMetadata.SelectedItem = gmd;

                TitleCollectionManager.AddGenreMetaData(gmd);

                genreMetaDataEditor.LoadGenre((GenreMetaData)gmd);
            }
            else
            {
                lbGenreMetadata.SelectedItem = e.First();
            }

            SetupFilterListContextMenu();
        }

        private void RemoveGenreMetaData()
        {
            if (lbGenreMetadata.SelectedItem != null)
            {
                List<TitleFilter> tfl = new List<TitleFilter>();
                TitleFilter tf = new TitleFilter(TitleFilterType.Genre, ((GenreMetaData)lbGenreMetadata.SelectedItem).Name);
                tfl.Add(tf);

                if (TitleCollectionManager.GetFilteredTitles(tfl).Count() == 0)
                {
                    TitleCollectionManager.RemoveGenreMetaData((GenreMetaData)lbGenreMetadata.SelectedItem);
                    lbGenreMetadata.Items.Remove(lbGenreMetadata.SelectedItem);
                    SetupFilterListContextMenu();
                }
                else
                {
                    if (XtraMessageBox.Show("The genre " + lbGenreMetadata.SelectedItem + " is assigned to movies. Do you want to set filter for this genre?", "Cannot delete genres.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        allMoviesToolStripMenuItem1.Checked = false;

                        foreach (ToolStripMenuItem item in filterByGenreToolStripMenuItem.DropDownItems)
                            if (item.Text != lbGenreMetadata.SelectedItem.ToString())
                                item.Checked = false;
                            else
                                item.Checked = true;
                        
                        showAllItemsInTitleListToolStripMenuItem.Checked = true;

                        LoadMovies();
                        PopulateMovieListV2(SelectedTreeRoot);
                    }
                }
            }
        }

        private DialogResult SaveCurrentGenreMetaData()
        {
            DialogResult result = DialogResult.Yes; ;
            if (genreMetaDataEditor.EditedGenreMetaData != null && genreMetaDataEditor.Status == GenreMetaDataEditor.GenreMetaDataStatus.UnsavedChanges)
            {
                result = XtraMessageBox.Show("You have unsaved changes to " + genreMetaDataEditor.EditedGenreMetaData.Name + ". Would you like to save your changes?", "Save Changes?", MessageBoxButtons.YesNoCancel);
                
                if (result == DialogResult.Yes)
                {
                    // Save the changes
                    TitleCollectionManager.SaveGenreMetaDataChanges(); 
                    genreMetaDataEditor.Status = GenreMetaDataEditor.GenreMetaDataStatus.Normal;
                    SetupFilterListContextMenu();
                }

                if (result == DialogResult.Cancel)
                {
                    // Cancel the changes (reselect the current item if selected item changed)
                    if (lbGenreMetadata.SelectedItem != genreMetaDataEditor.EditedGenreMetaData)
                    {
                        cancellingsave = true;
                        lbGenreMetadata.SelectedItem = genreMetaDataEditor.EditedGenreMetaData; 
                        cancellingsave = false;
                    }
                }

                if (result == DialogResult.No)
                {
                    // Don't save the changes - force reload of matadata
                    genreMetaDataEditor.EditedGenreMetaData.ReloadGenreMetaData();

                    SetWindowsTitleGenreMetaData(); 
                    genreMetaDataEditor.Status = GenreMetaDataEditor.GenreMetaDataStatus.Normal;
                    lbGenreMetadata.Refresh();
                    ToggleSaveState(false);
                }

            }
            return result;
        }

        private void genreMetaDataEditor_GenreMetaDataChanged(object sender, EventArgs e)
        {
            SetWindowsTitleGenreMetaData();
            ToggleSaveState(true);
        }

        private void SetWindowsTitleGenreMetaData()
        {
            if (genreMetaDataEditor.EditedGenreMetaData != null)
            {
                if (genreMetaDataEditor.Status == GenreMetaDataEditor.GenreMetaDataStatus.UnsavedChanges)
                    this.Text = APP_TITLE + " - " + genreMetaDataEditor.EditedGenreMetaData.Name + "*";
                else
                    this.Text = APP_TITLE + " - " + genreMetaDataEditor.EditedGenreMetaData.Name;
            }
            else
            {
                this.Text = APP_TITLE;
            }
        }

        private void lbGenreMetadata_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (lbGenreMetadata.SelectedIndices.Count > 0)
                {
                    OMLDragAndDropClass OMLDragAndDrop = new OMLDragAndDropClass();

                    OMLDragAndDrop.OMLDragAndDropType = OMLDragAndDropTypes.Genre;


                    OMLDragAndDrop.sItems = new string[lbGenreMetadata.SelectedIndices.Count];


                    for (int i = 0; i < lbGenreMetadata.SelectedIndices.Count; i++)
                    {
                        OMLDragAndDrop.sItems[i] = ((GenreMetaData)(lbGenreMetadata.Items[lbGenreMetadata.SelectedIndices[i]])).Name;
                    }

                    lbGenreMetadata.DoDragDrop(OMLDragAndDrop, DragDropEffects.Move);
                }
            }
        }

        private void lbGenreMetadata_DragEnter(object sender, DragEventArgs e)
        {
            currentmovetonode = null;
            lbGenreMetadata_DragOver(sender, e);
        }

        private void lbGenreMetadata_DragOver(object sender, DragEventArgs e)
        {
            bool validmove = false;

            GenreMetaData Genre = null;
            string[] titles = null;

            if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
            {
                OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Title)
                {

                    Point mouseLocation = lbGenreMetadata.PointToClient(new Point(e.X, e.Y));
                    int itemIndex = lbGenreMetadata.IndexFromPoint(mouseLocation);
                    if (itemIndex >= 0)
                    {
                        Genre = (GenreMetaData)lbGenreMetadata.Items[itemIndex];

                        if (OMLDragAndDrop.iItems.Count() > 0)
                        {
                            titles = new string[OMLDragAndDrop.iItems.Count()];
                            for (int i = 0; i < OMLDragAndDrop.iItems.Count(); i++)
                            {
                                titles[i] = _movieList[OMLDragAndDrop.iItems[i]].Name;
                            }
                            validmove = true;
                        }
                    }
                }
            }

            if (LastMousePoint != new Point(e.X, e.Y))
            {
                LastMousePoint = new Point(e.X, e.Y);

                if (tt == null)
                {
                    tt = new ToolTip();
                }

                if (validmove)
                {
                    e.Effect = DragDropEffects.Move;
                    tt.Show("Add " + Genre.Name + " to " + string.Join(", ", titles), this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    tt.Show("Invalid Drag", this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                }
            }
        }
        
        private void lbGenreMetadata_DragLeave(object sender, EventArgs e)
        {
            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }
        }

        private void lbGenreMetadata_DragDrop(object sender, DragEventArgs e)
        {
            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }

            GenreMetaData Genre = null;

            if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
            {
                this.Cursor = Cursors.WaitCursor;
 
                OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Title)
                {   
                    // Find Genre
                    Point mouseLocation = lbGenreMetadata.PointToClient(new Point(e.X, e.Y));
                    int itemIndex = lbGenreMetadata.IndexFromPoint(mouseLocation);
                    if (itemIndex >= 0)
                    {
                        Genre = (GenreMetaData)lbGenreMetadata.Items[itemIndex];

                        // Perform changes
                        foreach (int titleid in OMLDragAndDrop.iItems)
                        {
                            if (!_movieList[titleid].Genres.Contains(Genre.Name))
                            {
                                _movieList[titleid].AddGenre(Genre.Name);
                            }
                        }
                    } 
                    TitleCollectionManager.SaveTitleUpdates();
                } 
                
                this.Cursor = Cursors.Default;
            }
        }
        #endregion


        #region BioData Editing functions
        private void lbBioData_DrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            e.Handled = true;

            // Create the brushes
            if (_brushTreeViewSelected == null)
            {
                _brushTreeViewSelected = new LinearGradientBrush(new Point(0, 0), new Point(0, e.Bounds.Height), Color.LimeGreen, Color.PaleGreen);
            }


            int x = e.Bounds.X + 4;
            int y = e.Bounds.Y;
            int w = e.Bounds.Width;
            int h = e.Bounds.Height;
            int wt = (int)e.Graphics.MeasureString(e.Item.ToString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular)).Width + 2;

            // Setup string formatting
            StringFormat stf = new StringFormat();
            stf.Trimming = StringTrimming.EllipsisCharacter;
            stf.FormatFlags = StringFormatFlags.NoWrap;

            e.Graphics.FillRectangle(new SolidBrush(Color.White), x, y, w, h);
            
            bool highlight = false;
            foreach (int selectedindex in lbBioData.SelectedIndices)
            {
                if (lbBioData.Items[selectedindex] == e.Item)
                {
                    highlight = true;
                }
            }

            if (highlight == true)
            //if (lbBioData.SelectedItem == e.Item)
            {
                e.Graphics.FillRectangle(_brushTreeViewSelected, x, y, wt, h);
                e.Graphics.DrawLine(new Pen(Color.Black), x + 1, y, x - 2 + wt, y);
                e.Graphics.DrawLine(new Pen(Color.Black), x + 1, y - 1 + h, x - 2 + wt, y - 1 + h);
                e.Graphics.DrawLine(new Pen(Color.Black), x, y + 1, x, y - 2 + h);
                e.Graphics.DrawLine(new Pen(Color.Black), x -1 + wt, y + 1, x -1 + wt, y - 2 + h);

            }

            e.Graphics.DrawString(e.Item.ToString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(x, y + 2, e.Bounds.Width, h), stf);
        }

        private void lbBioData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cancellingsave)
            {
                // Check if there are any unsaved changes, prompt to save
                DialogResult dr = SaveCurrentBioData();

                if ((dr == DialogResult.Yes) || (dr == DialogResult.No))
                {   
                    // Data is properly saved or aborted, select new genre to edit
                    if (lbBioData.SelectedItem != null)
                    {
                        if (bioDataEditor.EditedBioData != lbBioData.SelectedItem)
                        {
                            // If a different person is selected, load it into the editor
                            bioDataEditor.LoadBioData((BioData)lbBioData.SelectedItem);
                            SetWindowsTitleBioData();
                        }
                    }
                }
            }
        }

        private void AddBioData()
        {
            SaveCurrentBioData();

            var e = from Object li in lbBioData.Items
                    where li.ToString() == "New Person"
                    select li;

            if (e.Count() == 0)
            {
                BioData bd = new BioData();
                bd.FullName = "New Person";

                lbBioData.Items.Add(bd);
                lbBioData.SelectedItem = bd;

                TitleCollectionManager.AddBioData(bd);

                bioDataEditor.LoadBioData((BioData)bd);
            }
            else
            {
                lbBioData.SelectedItem = e.First();
            }
        }

        private void RemoveBioData()
        {
            if (lbBioData.SelectedItem != null)
            {
                TitleCollectionManager.RemoveBioData((BioData)lbBioData.SelectedItem);
                lbBioData.Items.Remove(lbBioData.SelectedItem);
            }
        }

        private DialogResult SaveCurrentBioData()
        {
            DialogResult result;
            if (bioDataEditor.EditedBioData != null && bioDataEditor.Status == BioDataEditor.BioDataStatus.UnsavedChanges)
            {
                result = XtraMessageBox.Show("You have unsaved changes to " + bioDataEditor.EditedBioData.FullName + ". Would you like to save your changes?", "Save Changes?", MessageBoxButtons.YesNoCancel);
                
                if (result == DialogResult.Yes)
                {
                    TitleCollectionManager.SaveBioMetaDataChanges(); 
                    bioDataEditor.Status = BioDataEditor.BioDataStatus.Normal;
                }

                if (result == DialogResult.Cancel)
                {
                    if (lbBioData.SelectedItem != bioDataEditor.EditedBioData)
                    {
                        cancellingsave = true;
                        lbBioData.SelectedItem = bioDataEditor.EditedBioData;
                        cancellingsave = false;
                    }
                }

                if (result == DialogResult.No)
                {
                    // Don't save the changes - force reload of the person
                    bioDataEditor.EditedBioData.ReloadBioData();

                    SetWindowsTitleBioData();
                    bioDataEditor.Status = BioDataEditor.BioDataStatus.Normal;
                    lbBioData.Refresh();
                    ToggleSaveState(false);
                }
            }
            else
            {
                result = DialogResult.Yes;
            }
            return result;
        }

        private void bioDataEditor_BioDataChanged(object sender, EventArgs e)
        {
            SetWindowsTitleBioData();
            ToggleSaveState(true);
        }

        private void SetWindowsTitleBioData()
        {
            if (bioDataEditor.EditedBioData != null)
            {
                if (bioDataEditor.Status == BioDataEditor.BioDataStatus.UnsavedChanges)
                    this.Text = APP_TITLE + " - " + bioDataEditor.EditedBioData.FullName + "*";
                else
                    this.Text = APP_TITLE + " - " + bioDataEditor.EditedBioData.FullName;
            }
            else
            {
                this.Text = APP_TITLE;
            }
        }

        private void lbBioData_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (lbBioData.SelectedIndices.Count > 0)
                {
                    OMLDragAndDropClass OMLDragAndDrop = new OMLDragAndDropClass();

                    OMLDragAndDrop.OMLDragAndDropType = OMLDragAndDropTypes.Person;


                    OMLDragAndDrop.sItems = new string[lbBioData.SelectedIndices.Count];


                    for (int i = 0; i < lbBioData.SelectedIndices.Count; i++)
                    {
                        OMLDragAndDrop.sItems[i] = ((BioData)(lbBioData.Items[lbBioData.SelectedIndices[i]])).FullName;
                    }

                    lbBioData.DoDragDrop(OMLDragAndDrop, DragDropEffects.Move);
                }
            }
        }

        private void lbBioData_DragEnter(object sender, DragEventArgs e)
        {
            currentmovetonode = null;
            lbBioData_DragOver(sender, e);
        }

        private void lbBioData_DragOver(object sender, DragEventArgs e)
        {
            bool validmove = false;

            BioData Person = null;
            string[] titles = null;

            if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
            {
                OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Title)
                {

                    Point mouseLocation = lbBioData.PointToClient(new Point(e.X, e.Y));
                    int itemIndex = lbBioData.IndexFromPoint(mouseLocation);
                    if (itemIndex >= 0)
                    {
                        Person = (BioData)lbBioData.Items[itemIndex];

                        if (OMLDragAndDrop.iItems.Count() > 0)
                        {
                            titles = new string[OMLDragAndDrop.iItems.Count()];
                            for (int i = 0; i < OMLDragAndDrop.iItems.Count(); i++)
                            {
                                titles[i] = _movieList[OMLDragAndDrop.iItems[i]].Name;
                            }
                            validmove = true;
                        }
                    }
                }
            }

            if (LastMousePoint != new Point(e.X, e.Y))
            {
                LastMousePoint = new Point(e.X, e.Y);

                if (tt == null)
                {
                    tt = new ToolTip();
                }

                if (validmove)
                {
                    e.Effect = DragDropEffects.Move;
                    tt.Show("Add " + Person.FullName + " to " + string.Join(", ", titles), this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    tt.Show("Invalid Drag", this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                }
            }
        }
 
        private void lbBioData_DragLeave(object sender, EventArgs e)
        {
            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }
        }

        private void lbBioData_DragDrop(object sender, DragEventArgs e)
        {
            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }
           
            BioData Person = null;

            if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
            {
                this.Cursor = Cursors.WaitCursor;

                OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Title)
                { 
                    // Find Person
                    Point mouseLocation = lbBioData.PointToClient(new Point(e.X, e.Y));
                    int itemIndex = lbBioData.IndexFromPoint(mouseLocation);
                    if (itemIndex >= 0)
                    {
                        Person = (BioData)lbBioData.Items[itemIndex];

                        // Perform changes
                        foreach (int titleid in OMLDragAndDrop.iItems)
                        {
                            if ((from t in _movieList[titleid].ActingRoles
                                 where t.PersonName == Person.FullName
                                 select t.PersonName).Count() == 0)
                            {
                                _movieList[titleid].AddActingRole(Person.FullName, "");
                            }
                        }
                    }
                    TitleCollectionManager.SaveTitleUpdates();
                }
                this.Cursor = Cursors.Default;
            }
        }
        #endregion


        #region Tag Functions
        private void lbTags_DrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            e.Handled = true;

            // Create the brushes
            if (_brushTreeViewSelected == null)
            {
                _brushTreeViewSelected = new LinearGradientBrush(new Point(0, 0), new Point(0, e.Bounds.Height), Color.LimeGreen, Color.PaleGreen);
            }


            int x = e.Bounds.X + 4;
            int y = e.Bounds.Y;
            int w = e.Bounds.Width;
            int h = e.Bounds.Height;
            int wt = (int)e.Graphics.MeasureString(e.Item.ToString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular)).Width + 2;

            // Setup string formatting
            StringFormat stf = new StringFormat();
            stf.Trimming = StringTrimming.EllipsisCharacter;
            stf.FormatFlags = StringFormatFlags.NoWrap;

            e.Graphics.FillRectangle(new SolidBrush(Color.White), x, y, w, h);
            
            bool highlight = false;
            foreach (int selectedindex in lbTags.SelectedIndices)
            {
                if (lbTags.Items[selectedindex] == e.Item)
                {
                    highlight = true;
                }
            }

            if (highlight == true)
            //if (lbBioData.SelectedItem == e.Item)
            {
                e.Graphics.FillRectangle(_brushTreeViewSelected, x, y, wt, h);
                e.Graphics.DrawLine(new Pen(Color.Black), x + 1, y, x - 2 + wt, y);
                e.Graphics.DrawLine(new Pen(Color.Black), x + 1, y - 1 + h, x - 2 + wt, y - 1 + h);
                e.Graphics.DrawLine(new Pen(Color.Black), x, y + 1, x, y - 2 + h);
                e.Graphics.DrawLine(new Pen(Color.Black), x -1 + wt, y + 1, x -1 + wt, y - 2 + h);

            }

            e.Graphics.DrawString(e.Item.ToString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(x, y + 2, e.Bounds.Width, h), stf);
        }
        
        private void lbTags_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<TitleFilter> tf = new List<TitleFilter>();
            TitleFilter tag = new TitleFilter(TitleFilterType.Tag, (string)lbTags.SelectedItem);
            tf.Add(tag);
            titlesListView.SetFilter(tf);
        }
        
        private void lbTags_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (lbTags.SelectedIndices.Count > 0)
                {
                    OMLDragAndDropClass OMLDragAndDrop = new OMLDragAndDropClass();

                    OMLDragAndDrop.OMLDragAndDropType = OMLDragAndDropTypes.Tag;


                    OMLDragAndDrop.sItems = new string[lbTags.SelectedIndices.Count];


                    for (int i = 0; i < lbTags.SelectedIndices.Count; i++)
                    {
                        OMLDragAndDrop.sItems[i] = (string)lbTags.Items[lbTags.SelectedIndices[i]];
                    }

                    lbTags.DoDragDrop(OMLDragAndDrop, DragDropEffects.Move);
                }
            }
        }
        
        private void lbTags_DragEnter(object sender, DragEventArgs e)
        {
            currentmovetonode = null;
            lbTags_DragOver(sender, e);
        }

        private void lbTags_DragOver(object sender, DragEventArgs e)
        {
            bool validmove = false;

            string Tag = null;
            string[] titles = null;

            if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
            {
                OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Title)
                {

                    Point mouseLocation = lbTags.PointToClient(new Point(e.X, e.Y));
                    int itemIndex = lbTags.IndexFromPoint(mouseLocation);
                    if (itemIndex >= 0)
                    {
                        Tag = (string)lbTags.Items[itemIndex];

                        if (OMLDragAndDrop.iItems.Count() > 0)
                        {
                            titles = new string[OMLDragAndDrop.iItems.Count()];
                            for (int i = 0; i < OMLDragAndDrop.iItems.Count(); i++)
                            {
                                titles[i] = _movieList[OMLDragAndDrop.iItems[i]].Name;
                            }
                            validmove = true;
                        }
                    }
                }
            }

            if (LastMousePoint != new Point(e.X, e.Y))
            {
                LastMousePoint = new Point(e.X, e.Y);

                if (tt == null)
                {
                    tt = new ToolTip();
                }

                if (validmove)
                {
                    e.Effect = DragDropEffects.Move;
                    tt.Show("Add " + Tag + " to " + string.Join(", ", titles), this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    tt.Show("Invalid Drag", this, this.PointToClient(new Point(e.X + 20, e.Y + 30)));
                }
            }
        }
        
        private void lbTags_DragLeave(object sender, EventArgs e)
        {
            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }
        }

        private void lbTags_DragDrop(object sender, DragEventArgs e)
        {
            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }

            string Tag = null;

            if (e.Data.GetDataPresent(typeof(OMLDragAndDropClass)))
            {
                this.Cursor = Cursors.WaitCursor;

                OMLDragAndDropClass OMLDragAndDrop = (OMLDragAndDropClass)e.Data.GetData(typeof(OMLDragAndDropClass));

                if (OMLDragAndDrop.OMLDragAndDropType == OMLDragAndDropTypes.Title)
                {
                    // Find Genre
                    Point mouseLocation = lbTags.PointToClient(new Point(e.X, e.Y));
                    int itemIndex = lbTags.IndexFromPoint(mouseLocation);
                    if (itemIndex >= 0)
                    {
                        Tag = (string)lbTags.Items[itemIndex];

                        // Perform changes
                        foreach (int titleid in OMLDragAndDrop.iItems)
                        {
                            if (!_movieList[titleid].Tags.Contains(Tag))
                            {
                                _movieList[titleid].AddTag(Tag);
                            }
                        }
                    }
                    TitleCollectionManager.SaveTitleUpdates();
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion


        #region Title Drag and Drop support
        private ToolTip tt;
        TreeNode currentmovetonode;
        Point LastMousePoint;

        enum OMLDragAndDropTypes
        {
            Title,
            Genre,
            Person,
            Tag
        }
        class OMLDragAndDropClass
        {
            public OMLDragAndDropTypes OMLDragAndDropType;
            public int[] iItems;
            public string[] sItems;
        }
        #endregion
        
        
        #region Title & Folder Creation.
        // These wrap the functionality provided by the engine with
        // some code to update the editor with the added titles
        
        private int CreateTitle(int? parentid, string Name, TitleTypes titletype, Disk[] disks, bool RefreshUI)
        {
            Title addedTitle = TitleCollectionManager.CreateTitle(parentid, Name, titletype, null, null, null, disks);
            
            AddCreatedTitle(addedTitle, RefreshUI);

            return addedTitle.Id;
        }

        private int CreateTitle(int? parentid, string Name, TitleTypes titletype, short? SeasonNumber, short? EpisodeNumber, Disk[] disks, bool RefreshUI)
        {
            Title addedTitle = TitleCollectionManager.CreateTitle(parentid, Name, titletype, null, SeasonNumber, EpisodeNumber, disks);
            
            AddCreatedTitle(addedTitle, RefreshUI);

            return addedTitle.Id;

            /*Title newTitle = new Title();
            newTitle.Name = Name;

            if (parentid == null)
            {
                newTitle.TitleType = TitleTypes.Root | titletype;
            }
            else
            {
                newTitle.TitleType = titletype;
                newTitle.ParentTitleId = (int)parentid;

                if ((titletype & TitleTypes.Unknown) != 0)
                {
                    // Title type is unknown. Attempt to find title type by looking at parent
                    if (((_movieList[(int)parentid].TitleType & TitleTypes.TVShow) != 0) ||
                    (((_movieList[(int)parentid].TitleType & TitleTypes.Season) != 0)))
                    {
                        newTitle.TitleType = TitleTypes.Episode;
                    }

                    if ((_movieList[(int)parentid].TitleType & TitleTypes.Collection) != 0)
                    {
                        newTitle.TitleType = TitleTypes.Movie;
                    }
                }
            }
            newTitle.DateAdded = DateTime.Now;

            newTitle.SeasonNumber = SeasonNumber;
            newTitle.EpisodeNumber = EpisodeNumber;

            if (disks != null)
            {
                foreach (Disk disk in disks)
                {
                    newTitle.AddDisk(disk);
                }
            }


            // Add the title now to get the title ID
            TitleCollectionManager.AddTitle(newTitle);

            // Get the new title from the DB and add it to the title list 
            Title addedTitle = TitleCollectionManager.GetTitle(newTitle.Id);*/
        }

        private int CreateFolder(int? parentid, string Name, TitleTypes titletype, short? seriesNumber, bool RefreshUI)
        {
            Title addedTitle = TitleCollectionManager.CreateFolder(parentid, Name, titletype, seriesNumber);
           
            AddCreatedTitle(addedTitle, RefreshUI);
     
            return addedTitle.Id;
            /*if (parentid == null) {
                return CreateTitle(null, Name, titletype, null, RefreshUI);
            } else {
                if ((titletype & TitleTypes.Unknown) != 0) {
                    // Title type is unknown. Attempt to find title type by looking at parent
                    if (((_movieList[(int)parentid].TitleType & TitleTypes.TVShow) != 0) ||
                    (((_movieList[(int)parentid].TitleType & TitleTypes.Season) != 0))) {
                        titletype = TitleTypes.Season;
                    }

                    if ((_movieList[(int)parentid].TitleType & TitleTypes.Collection) != 0) {
                        titletype = TitleTypes.Collection;
                    }
                }
                return CreateTitle(parentid, Name, titletype, seriesNumber, null, null, RefreshUI);
            }*/
        }

        private int CreateFolder(int? parentid, string Name, TitleTypes titletype, bool RefreshUI)
        {
            Title addedTitle = TitleCollectionManager.CreateFolder(parentid, Name, titletype);

            AddCreatedTitle(addedTitle, RefreshUI);
            
            return addedTitle.Id;

            /*if (parentid == null)
            {
                // Root Node
                return CreateTitle(null, Name, titletype, null, RefreshUI);
            }
            else
            {
                if ((titletype & TitleTypes.Unknown) != 0)
                {
                    // Title type is unknown. Attempt to find title type by looking at parent
                    if (((_movieList[(int)parentid].TitleType & TitleTypes.TVShow) != 0) ||
                    (((_movieList[(int)parentid].TitleType & TitleTypes.Season) != 0)))
                    {
                        titletype = TitleTypes.Season;
                    }

                    if ((_movieList[(int)parentid].TitleType & TitleTypes.Collection) != 0)
                    {
                        titletype = TitleTypes.Collection;
                    }
                }
                return CreateTitle(parentid, Name, titletype, null, RefreshUI);
            }*/
        }

        private int CreateFolder(string Name, TitleTypes titletype, bool RefreshUI)
        {
            if (treeMedia.SelectedNode.Name == "All Media")
            {
                // Root Node
                return CreateFolder(null, Name, titletype, RefreshUI);
            }
            else
            {
                int parentid = Convert.ToInt32(treeMedia.SelectedNode.Name);

                /*if ((titletype & TitleTypes.Unknown) != 0)
                {
                    // Title type is unknown. Attempt to find title type by looking at parent
                    if (((_movieList[(int)parentid].TitleType & TitleTypes.TVShow) != 0) ||
                    (((_movieList[(int)parentid].TitleType & TitleTypes.Season) != 0)))
                    {
                        titletype = TitleTypes.Season;
                    }

                    if ((_movieList[(int)parentid].TitleType & TitleTypes.Collection) != 0)
                    {
                        titletype = TitleTypes.Collection;
                    }
                }*/
                return CreateFolder(parentid, Name, titletype, RefreshUI);
            }
        }

        private void AddCreatedTitle(Title addedTitle, bool RefreshUI)
        {
            _movieList.Add(addedTitle.Id, addedTitle);

            if (RefreshUI)
            {
                if ((addedTitle.TitleType & TitleTypes.AllMedia) != 0)
                {
                    // Added a media
                    PopulateMovieListV2(SelectedTreeRoot);
                    titleEditor.LoadDVD(addedTitle);
                    //ToggleSaveState(true);
                }
                else
                {
                    // Added a folder
                    PopulateMediaTree();
                    if (_mediaTree.ContainsKey(addedTitle.Id))
                    {
                        treeMedia.SelectedNode = _mediaTree[addedTitle.Id];
                    }
                }
            }
        }
        #endregion


        #region St Sana
        /*private void CreateTitlesFromPathArray(int? parentid, string[] path)
        {
            StSanaEvents eventsForm = new StSanaEvents();
            eventsForm.Show();
            eventsForm.Activate();

            // TODO - Need to check for images in folder
            // TODO - Wrap this up in another thread
            foreach (string file in path)
            {
                try
                {
                    if (Directory.Exists(file))
                    {
                        // Folder passed in. This is where St Sana kicks in
                        Servant stsana = new Servant();
                        stsana.Log += new Servant.SSEventHandler(stsana_Log);
                        stsana.BasePaths.Add(file);
                        stsana.Scan();

                        int? a_parent;

                        if (OMLEngine.Settings.OMLSettings.StSanaCreateTLFolder)
                        {
                            a_parent = CreateFolderNonDuplicate(parentid, Path.GetFileName(file), TitleTypes.Collection, null, false);
                        }
                        else
                        {
                            a_parent = parentid;
                        }

                        if (stsana.Entities != null)
                        {
                            foreach (Entity e in stsana.Entities)
                            {
                                int? e_parent = a_parent;
                                if (e.Name != file)
                                {
                                    switch (e.EntityType)
                                    {
                                        case Serf.EntityType.COLLECTION:
                                        case Serf.EntityType.MOVIE:
                                            if ((e.Series.Count() > 1) || (OMLEngine.Settings.OMLSettings.StSanaAlwaysCreateMovieFolder))
                                            {
                                                e_parent = CreateFolderNonDuplicate(a_parent, e.Name, TitleTypes.Collection, null, false);
                                            }
                                            else
                                            {
                                                e_parent = a_parent;
                                            }
                                            break;
                                        case Serf.EntityType.TV_SHOW:
                                            e_parent = CreateFolderNonDuplicate(a_parent, e.Name, TitleTypes.TVShow, null, false);
                                            break;
                                    }
                                }

                                foreach (Series s in e.Series)
                                {
                                    int? s_parent = e_parent;
                                    // if the s.name and e.name are the same, its a movie, to be sure though lets check s.number, it should be
                                    // -1 for non tv shows.
                                    if (s.Name.ToUpperInvariant().CompareTo(e.Name.ToUpperInvariant()) != 0 || s.Number > -1)
                                    {
                                        //s_parent = CreateFolderNonDuplicate(e_parent, s.Name, TitleTypes.Collection, false);
                                        if (s.Name != e.Name)
                                        {
                                            switch (e.EntityType)
                                            {
                                                case Serf.EntityType.COLLECTION:
                                                case Serf.EntityType.MOVIE:
                                                    if ((e_parent == a_parent) || (OMLEngine.Settings.OMLSettings.StSanaAlwaysCreateMovieFolder))
                                                    {
                                                        s_parent = CreateFolderNonDuplicate(e_parent, s.Name, TitleTypes.Collection, null, false);
                                                    }
                                                    else
                                                    {
                                                        s_parent = e_parent;
                                                    }
                                                    break;
                                                case Serf.EntityType.TV_SHOW:
                                                    s_parent = CreateFolderNonDuplicate(e_parent, s.Name, TitleTypes.Season, (short)s.Number, false);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            s_parent = e_parent;
                                        }
                                    }

                                    foreach (Video v in s.Videos)
                                    {
                                        stsana_Log("Processing " + v.Name);
                                        //int v_parent = CreateFolder(s_parent, Path.GetFileNameWithoutExtension(v.Name), TitleTypes.Collection, false);

                                        List<Disk> disks = new List<Disk>();

                                        if ((e.EntityType == Serf.EntityType.COLLECTION) ||
                                            (e.EntityType == Serf.EntityType.MOVIE))
                                        {
                                            // Collection or movie mode. Create one title per folder with multiple disks
                                            foreach (string f in v.Files)
                                            {
                                                if (!TitleCollectionManager.ContainsDisks(OMLEngine.FileSystem.NetworkScanner.FixPath(f)))
                                                {
                                                    Disk disk = new Disk();
                                                    disk.Path = f;
                                                    disk.Format = disk.GetFormatFromPath(f); // (VideoFormat)Enum.Parse(typeof(VideoFormat), fileExtension.ToUpperInvariant());
                                                    disk.Name = string.Format("Disk {0}", 0);
                                                    if (disk.Format != VideoFormat.UNKNOWN)
                                                    {
                                                        disks.Add(disk);
                                                    }
                                                }
                                            }
                                            if (disks.Count != 0)
                                            {
                                                int newTitleID = CreateTitle(s_parent,
                                                    Path.GetFileNameWithoutExtension(v.Name),
                                                    TitleTypes.Unknown,
                                                    disks.ToArray(),
                                                    false);

                                                // Reload title from db, lookup metadata and find images
                                                Title newTitle = TitleCollectionManager.GetTitle(newTitleID);
                                                LookupPreferredMetaData(newTitle);
                                                CheckDiskPathForImages(newTitle, disks[0]);
                                                TitleCollectionManager.SaveTitleUpdates();
                                            }
                                        }
                                        else
                                        {
                                            // TV mode. Create one title per file, each with single disks
                                            foreach (string f in v.Files)
                                            {
                                                if (!TitleCollectionManager.ContainsDisks(OMLEngine.FileSystem.NetworkScanner.FixPath(f)))
                                                {
                                                    Disk disk = new Disk();
                                                    disk.Path = f;
                                                    disk.Format = disk.GetFormatFromPath(f); //(VideoFormat)Enum.Parse(typeof(VideoFormat), fileExtension.ToUpperInvariant());
                                                    disk.Name = string.Format("Disk {0}", 0);
                                                    if (disk.Format != VideoFormat.UNKNOWN)
                                                    {
                                                        disks.Add(disk);
                                                    }
                                                }
                                                if (disks.Count != 0)
                                                {
                                                    short episodeno = 0;
                                                    if (v.EpisodeNumbers.Count > 0) episodeno = (short)v.EpisodeNumbers[0];

                                                    int newTitleID = CreateTitle(s_parent,
                                                        Path.GetFileNameWithoutExtension(f),
                                                        TitleTypes.Episode,
                                                        (short)s.Number,
                                                        episodeno,
                                                        disks.ToArray(),
                                                        false);

                                                    // Reload title from db, lookup metadata and find images
                                                    Title newTitle = TitleCollectionManager.GetTitle(newTitleID);
                                                    LookupPreferredMetaData(newTitle);
                                                    CheckDiskPathForImages(newTitle, disks[0]);
                                                    TitleCollectionManager.SaveTitleUpdates();

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (File.Exists(file))
                    {
                        stsana_Log("Processing " + file);
                        string extension = Path.GetExtension(file).ToUpper().Replace(".", "");
                        extension = extension.Replace("-", "");

                        Disk disk = new Disk();
                        disk.Path = file;
                        disk.Format = disk.GetFormatFromPath(file); // (VideoFormat)Enum.Parse(typeof(VideoFormat), extension.ToUpperInvariant());
                        disk.Name = string.Format("Disk {0}", 0);

                        if (disk.Format != VideoFormat.UNKNOWN)
                        {
                            int newTitleID = CreateTitle(parentid,
                                Path.GetFileNameWithoutExtension(file),
                                TitleTypes.Unknown,
                                new Disk[1] { disk },
                                false);
                            
                            // Reload title from db, lookup metadata and find images
                            Title newTitle = TitleCollectionManager.GetTitle(newTitleID);
                            LookupPreferredMetaData(newTitle);
                            CheckDiskPathForImages(newTitle, disk);
                            TitleCollectionManager.SaveTitleUpdates();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.DebugLine("[OMLDatabaseEditor] CreateTitlesFromPathArray exception" + ex.Message);
                }
            }

            eventsForm.Hide();
            eventsForm.Dispose();
            eventsForm = null;
            PopulateMovieListV2(SelectedTreeRoot);
            PopulateMediaTree();
        }*/


        private void CreateTitlesFromPathArray(int? parentid, string[] path)
        {
            StSanaEvents eventsForm = new StSanaEvents();
            eventsForm.Show();
            eventsForm.Activate();

            // Create the St Sana class
            StSanaServices StSana = new StSanaServices();

            StSana.Log += new StSanaServices.SSEventHandler(stsana_Log);
            
            List<Title> titles= StSana.CreateTitlesFromPathArray(parentid, path, null);

            foreach (Title t in titles)
            {
                try
                {
                    AddCreatedTitle(t, false);

                    if ((t.TitleType & TitleTypes.AllMedia) != 0)
                    {
                        stsana_Log("Looking up metadata for " + t.Name);
                        LookupPreferredMetaData(t);
                    }
                    TitleCollectionManager.SaveTitleUpdates();
                }
                catch (Exception ex)
                {
                    Utilities.DebugLine("[OMLDatabaseEditor] CreateTitlesFromPathArray exception  on title : " + t.Name + " : " + ex.Message);
                }
            }

            eventsForm.Hide();
            eventsForm.Dispose();
            eventsForm = null;
            PopulateMovieListV2(SelectedTreeRoot);
            PopulateMediaTree();
        }

        void stsana_Log(string message)
        {
            StSanaEvents.UpdateStatus(message);
        }

        /*private void CheckDiskPathForImages(Title title, Disk disk)
        {
            if ((disk == null) || (string.IsNullOrEmpty(disk.Path))) return;

            string diskFolder = disk.GetDiskFolder;
            string diskPathWithExtension = null;
            string diskPathWithoutExtension = null;

            if (!string.IsNullOrEmpty(disk.GetDiskFile))
            {
                diskPathWithExtension = disk.Path;
                diskPathWithoutExtension = disk.GetDiskFolder + "\\" + Path.GetFileNameWithoutExtension(disk.GetDiskFile);
            }

            string image = null;

            // If the Disk is a media file, look for an image in the disk 
            // folder with the same name as the media file.
            if (!string.IsNullOrEmpty(diskPathWithExtension))
            {
                if (File.Exists(diskPathWithExtension + ".jpg"))
                {
                    image = diskPathWithExtension + ".jpg";
                }
                else if (File.Exists(diskPathWithExtension + ".png"))
                {
                    image = diskPathWithExtension + ".png";
                }
                else if (File.Exists(diskPathWithoutExtension + ".jpg"))
                {
                    image = diskPathWithoutExtension + ".jpg";
                }
                else if (File.Exists(diskPathWithoutExtension + ".png"))
                {
                    image = diskPathWithoutExtension + ".png";
                }
            }

            // Look for a generic folder.xxx image
            if (string.IsNullOrEmpty(image))
            {
                if (File.Exists(Path.Combine(diskFolder, "folder.jpg")))
                {
                    image = Path.Combine(diskFolder, "folder.jpg");
                }
                else if (File.Exists(Path.Combine(diskFolder, "folder.png")))
                {
                    image = Path.Combine(diskFolder, "folder.png");
                }
            } 
            
            // Look for any jpg image
            if (string.IsNullOrEmpty(image))
            {
                string[] imagefiles = Directory.GetFiles(diskFolder, "*.jpg");
                if (imagefiles.Count() > 0)
                {
                    image = imagefiles[0];
                }
            }

            // Look for any jpg image
            if (string.IsNullOrEmpty(image))
            {
                string[] imagefiles = Directory.GetFiles(diskFolder, "*.png");
                if (imagefiles.Count() > 0)
                {
                    image = imagefiles[0];
                }
            }


            if (!string.IsNullOrEmpty(image))
            {
                title.FrontCoverPath = image;
            }

            // Check for fanart
            string fanartfolder = Path.Combine(diskFolder, "Fanart");
            if (Directory.Exists(fanartfolder))
            {
                foreach (string imagefile in Directory.GetFiles(fanartfolder))
                {
                    string extension = Path.GetExtension(imagefile);
                    if (!string.IsNullOrEmpty(extension))
                    {
                        if ((string.Compare(extension, ".jpg", true) == 0) ||
                            (string.Compare(extension, ".png", true) == 0) ||
                            (string.Compare(extension, ".bmp", true) == 0))
                        {
                            title.AddFanArtImage(imagefile);
                        }
                    }
                }
            }
        }
        */
        private void LookupPreferredMetaData(Title title)
        {
            if (OMLEngine.Settings.OMLSettings.StSanaAutoLookupMeta)
            {
                if ((title == null) || (string.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPluginMovies))) return;
                StartMetadataImport(title, null, false);
            }
        }

        /// <summary>
        /// Creates a folder but first checks to see if one of the same name allready exists
        /// If so then this id is returned
        /// </summary>
        /// <param name="parentid"></param>
        /// <param name="Name"></param>
        /// <param name="titletype"></param>
        /// <param name="RefreshUI"></param>
        /*private int CreateFolderNonDuplicate(int? parentid, string Name, TitleTypes titletype, short? seriesNumber, bool RefreshUI)
        {
            int titleid;

            // Build filter
            TitleFilter tf1 = new TitleFilter(TitleFilterType.Parent, parentid.ToString());
            TitleFilter tf2 = new TitleFilter(TitleFilterType.Name, Name);
            List<TitleFilter> tf = new List<TitleFilter>();
            tf.Add(tf1);
            tf.Add(tf2);
            List<Title> existingTitle = (from t in TitleCollectionManager.GetFilteredTitles(tf)
                                         where t.Name == Name
                                         select t).ToList();



            if (existingTitle.Count() > 0)
            {
                titleid = existingTitle[0].Id;
            }
            else
            {
                titleid = CreateFolder(parentid, Name, titletype, seriesNumber, RefreshUI);
            }
            return titleid;
        }*/
        #endregion


        #region Title Sorting
        private IEnumerable<Title> SortTitles(IEnumerable<Title> titles)
        {
            if (OMLEngine.Settings.OMLSettings.DBETitleSortAsc)
            {
                switch (OMLEngine.Settings.OMLSettings.DBETitleSortField)
                {
                    case "Name":
                        return titles.OrderBy(st => st.Name);
                    case "Sort Name":
                        return titles.OrderBy(st => st.SortName);
                    case "Run Time":
                        return titles.OrderBy(st => st.Runtime);
                    case "Date Added":
                        return titles.OrderBy(st => st.DateAdded);
                    case "Metadata":
                        return titles.OrderBy(st => st.PercentComplete);
                    case "Modified Date":
                        return titles.OrderBy(st => st.ModifiedDate);
                    case "Production Year":
                        return titles.OrderBy(st => st.ProductionYear);
                    case "Release Date":
                        return titles.OrderBy(st => st.ReleaseDate);
                    case "User Rating":
                        return titles.OrderBy(st => st.UserStarRating);
                     default:
                        return titles.OrderBy(st => st.SortName);
                }
            }
            else
            {
                switch (OMLEngine.Settings.OMLSettings.DBETitleSortField)
                {
                    case "Name":
                        return titles.OrderByDescending(st => st.Name);
                    case "Sort Name":
                        return titles.OrderByDescending(st => st.SortName);
                    case "Run Time":
                        return titles.OrderByDescending(st => st.Runtime);
                    case "Date Added":
                        return titles.OrderByDescending(st => st.DateAdded);
                    case "Metadata":
                        return titles.OrderByDescending(st => st.PercentComplete);
                    case "Modified Date":
                        return titles.OrderByDescending(st => st.ModifiedDate);
                    case "Production Year":
                        return titles.OrderByDescending(st => st.ProductionYear);
                    case "Release Date":
                        return titles.OrderByDescending(st => st.ReleaseDate);
                    case "User Rating":
                        return titles.OrderByDescending(st => st.UserStarRating);
                    default:
                        return titles.OrderByDescending(st => st.SortName);
                }
            }
        }

        private void SortControl_Paint(object sender, PaintEventArgs e)
        {
            int x = Convert.ToInt32(e.Graphics.ClipBounds.Left);
            int y = Convert.ToInt32(e.Graphics.ClipBounds.Top);

            e.Graphics.DrawString("Sort : " + OMLEngine.Settings.OMLSettings.DBETitleSortField.ToString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), x + 2, y + 2);
            //e.Graphics.DrawLine(new Pen(Color.Black), new Point(x + 50, y + 2), new Point(x + 50, y + 8));

            Point p1;
            Point p2;
            Point p3;

            if (OMLEngine.Settings.OMLSettings.DBETitleSortAsc)
            {
                int wt = (int)e.Graphics.MeasureString("Ascending", new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular)).Width + 2;
                e.Graphics.DrawString("Ascending", new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), e.Graphics.ClipBounds.Right - wt - 18, y + 2);

                p1 = new Point(((int)e.Graphics.ClipBounds.Right) - 10, 13);
                p2 = new Point(((int)e.Graphics.ClipBounds.Right) - 14, 8);
                p3 = new Point(((int)e.Graphics.ClipBounds.Right) - 18, 13);
            }
            else
            {
                int wt = (int)e.Graphics.MeasureString("Descending", new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular)).Width + 2;
                e.Graphics.DrawString("Descending", new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), e.Graphics.ClipBounds.Right - wt - 18, y + 2);

                p1 = new Point(((int)e.Graphics.ClipBounds.Right) - 10, 8);
                p2 = new Point(((int)e.Graphics.ClipBounds.Right) - 14, 13);
                p3 = new Point(((int)e.Graphics.ClipBounds.Right) - 18, 8); 
            }
         
            e.Graphics.FillPolygon(new SolidBrush(Color.Black), new Point[] { p1, p2, p3 }, FillMode.Winding);
        }

        private void SortControl_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > SortControl.Size.Width - 80)
            {
                OMLEngine.Settings.OMLSettings.DBETitleSortAsc = !OMLEngine.Settings.OMLSettings.DBETitleSortAsc;
                PopulateMovieListV2(SelectedTreeRoot);
                SortControl.Refresh();
            }
            else
            {
                ContextMenuStrip cms = new ContextMenuStrip();
                cms.ItemClicked += new ToolStripItemClickedEventHandler(SortOrderChanged);
                cms.Items.Add("Name");
                cms.Items.Add("Sort Name");
                cms.Items.Add("Run Time");
                cms.Items.Add("Date Added");
                cms.Items.Add("Metadata");
                cms.Items.Add("Modified Date");
                cms.Items.Add("Production Year");
                cms.Items.Add("Release Date");
                cms.Items.Add("User Rating");
                cms.Show(SortControl, e.Location);
            }
        }

        public void SortOrderChanged(object sender, ToolStripItemClickedEventArgs e)
        {
            OMLEngine.Settings.OMLSettings.DBETitleSortField = e.ClickedItem.Text;
            PopulateMovieListV2(SelectedTreeRoot);
            SortControl.Refresh();
        }
        #endregion


        private void addTagMenuItem1_Click(object sender, EventArgs e)
        {
            List<string> tags = new List<string>();
            ListEditor editor = new ListEditor("Tags", tags);
            editor.ShowDialog(); 
            
            ListView.SelectedListViewItemCollection sic = lvTitles.SelectedItems;

            foreach (string tag in tags)
            {
                foreach (ListViewItem item in sic)
                {
                    Title title = _movieList[Convert.ToInt32(item.Text)];

                    if (!title.Tags.Contains(tag))
                        title.AddTag(tag);
                }
            }
            TitleCollectionManager.SaveTitleUpdates();
        }

        private void addGenreMenuItem1_Click(object sender, EventArgs e)
        {
            List<string> genres = new List<string>();
            ListEditor editor = new ListEditor("Genres", genres);
            editor.ShowDialog();

            ListView.SelectedListViewItemCollection sic = lvTitles.SelectedItems;

            foreach (string genre in genres)
            {
                foreach (ListViewItem item in sic)
                {
                    Title title = _movieList[Convert.ToInt32(item.Text)];

                    if (!title.Genres.Contains(genre))
                        title.AddGenre(genre);
                }
            }
            TitleCollectionManager.SaveTitleUpdates();
        }

        private void miAdd_Click(object sender, EventArgs e)
        {
            if (mainNav.ActiveGroup == groupBioData)
            {
                AddBioData();
            }

            if (mainNav.ActiveGroup == groupGenresMetadata)
            {
                AddGenreMetaData();
            }
        }

        private void miRemove_Click(object sender, EventArgs e)
        {
            if (mainNav.ActiveGroup == groupBioData)
            {
                RemoveBioData();
            }

            if (mainNav.ActiveGroup == groupGenresMetadata)
            {
                RemoveGenreMetaData();
            }
        }

        private void CreateTitleShortcut_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection sic = lvTitles.SelectedItems;
            if (sic.Count < 1) return;

            // Get the parent for the shortcut
            MediaTreePicker mtp = new MediaTreePicker();
            if (mtp.ShowDialog() == DialogResult.OK)
            {
                //lvi.SubItems[1].Text = mtp.SelectedTitleName;
                //lvi.Tag = mtp.SelectedTitleID;
                foreach (ListViewItem item in sic)
                {
                    Title title = _movieList[Convert.ToInt32(item.Text)];

                    if ((title.TitleType & TitleTypes.Shortcut) != 0)
                    {
                        XtraMessageBox.Show("Cannot create a shortcut to '" + title.Name + "'", "Error creating shortcut!");
                    }
                    else
                    {
                        if ((title.TitleType & TitleTypes.AllFolders) != 0)
                        {
                            // Folder
                            CreateFolder(mtp.SelectedTitleID,
                                "Shortcut to : " + title.Name,
                                title.TitleType | TitleTypes.Shortcut,
                                title.SeasonNumber,
                                true);

                        }

                        if ((title.TitleType & TitleTypes.AllMedia) != 0)
                        {
                            // Movie or episode
                            CreateTitle(mtp.SelectedTitleID,
                                "Shortcut to : " + title.Name,
                                title.TitleType | TitleTypes.Shortcut,
                                title.SeasonNumber,
                                title.EpisodeNumber,
                                null,
                                true);
                        }
                    }
                }
            }

            TitleCollectionManager.SaveTitleUpdates();
        }

        private void lvTitles_ScrollUp(object sender, EventArgs e)
        {
            lvTitles.Invalidate();
        }
    }
}
