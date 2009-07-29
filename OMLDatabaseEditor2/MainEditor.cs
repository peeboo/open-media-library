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

            InitData();
            
            splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Dock = DockStyle.Fill;

            splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Visible = true;
            splitContainerNavigator.Panel2.Controls["bioDataEditor"].Visible = false;
            splitContainerNavigator.Panel2.Controls["genreMetaDataEditor"].Visible = false;
        }


        /// <summary>
        /// Perform startup initialisation including updating the splash screen
        /// </summary>
        private void InitData()
        {
            Cursor = Cursors.WaitCursor;
            //OMLEngine.Utilities.DebugLine("[MainEditor] InitData() : new TitleCollection()");
            //OMLEngine.Utilities.DebugLine("[MainEditor] InitData() : loadTitleCollection()");            

            SplashScreen2.SetStatus(16, "Checking database.");
            if (!ValidateDatabase())
            {
                SplashScreen2.CloseForm();
                Cursor = Cursors.Default;
                return;
            }

            defaultLookAndFeel1.LookAndFeel.SkinName = OMLEngine.Settings.OMLSettings.DBEditorSkin;

            SplashScreen2.SetStatus(32, "Setting up Menus.");
            SetupNewMovieAndContextMenu();

            SplashScreen2.SetStatus(48, "Loading Skins.");
            GetDXSkins();

            _loading = true;
            
            SplashScreen2.SetStatus(64, "Loading Movies.");

            _movieList = new Dictionary<int,Title>();

            LoadMovies();
            PopulateMediaTree();
            PopulateMovieListV2(SelectedTreeRoot);

            SplashScreen2.SetStatus(80, "Loading MRU Lists.");
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
        }


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
                    return true;

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

            if (String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin))
                fromPreferredSourcesToolStripMenuItem1.Visible = false;

            // Set up filter lists
            ToolStripMenuItem item;
            foreach (string genre in from g in TitleCollectionManager.GetAllGenres(new List<TitleFilter>()) select g.Name)
            {
                item = new ToolStripMenuItem(genre);
                item.CheckOnClick = true;
                item.Click += new EventHandler(filterTitles_Click);
                filterByGenreToolStripMenuItem.DropDownItems.Add(item);
            }

            foreach (string rating in from t in TitleCollectionManager.GetAllParentalRatings(null) select t.Name)
            {
                item = new ToolStripMenuItem(rating);
                item.CheckOnClick = true;
                item.Click += new EventHandler(filterTitles_Click);
                filterByParentalRatingToolStripMenuItem.DropDownItems.Add(item);
            }

            foreach (string tag in TitleCollectionManager.GetAllTagsList())
            {
                item = new ToolStripMenuItem(tag);
                item.CheckOnClick = true;
                item.Click += new EventHandler(filterTitles_Click);
                filterByTagToolStripMenuItem.DropDownItems.Add(item);
            }
        }

        private static void LoadImportPlugins(string pluginType, List<OMLPlugin> pluginList)
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
        private static void LoadMetadataPlugins(string pluginType, List<MetaDataPluginDescriptor> pluginList)
        {
            pluginList.Clear();

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
                                if (!string.IsNullOrEmpty(setting))
                                {
                                    provider.PluginDLL.SetOptionValue(option.Name, setting);
                                }
                            }
                        }

                        pluginList.Add(provider);
                    }


                }
                plugins = null;
            }
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


        #region Title filter, Load & Save
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
                        MetaDataPluginDescriptor plugin = selectPlugin.SelectedPlugin();
                        StartMetadataImport(plugin, false);
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
        #endregion


        #region MetaData Import
        private bool StartMetadataImport(MetaDataPluginDescriptor plugin, bool coverArtOnly)
        {
            int? SeasonNo = null;
            int? EpisodeNo = null;

            if ((titleEditor.EditedTitle.TitleType & TitleTypes.Episode) != 0)
            {
                // TV Search
                if (titleEditor.EditedTitle.SeasonNumber != null) SeasonNo = titleEditor.EditedTitle.SeasonNumber.Value;
                if (titleEditor.EditedTitle.EpisodeNumber != null) EpisodeNo = titleEditor.EditedTitle.EpisodeNumber.Value;
                string Showname = null;

                Title t = titleEditor.EditedTitle;
                // Try to find show name be looking up the folder structure.
                while ((t.TitleType & TitleTypes.Root) == 0)
                {
                    // Get parent
                    t = t.ParentTitle;
                    if ((t.TitleType & TitleTypes.TVShow) != 0)
                    {
                        Showname = t.Name;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(Showname))
                {
                    // Cannot find a show name in the folder structure
                    return StartMetadataImport(plugin, coverArtOnly, titleEditor.EditedTitle.Name, "", SeasonNo, EpisodeNo);
                }
                else
                {
                    return StartMetadataImport(plugin, coverArtOnly, Showname, titleEditor.EditedTitle.Name, SeasonNo, EpisodeNo);
                }
            }
            else
            {
                // Movie Search
                return StartMetadataImport(plugin, coverArtOnly, titleEditor.EditedTitle.Name, "", SeasonNo, EpisodeNo);
            }
        }

        internal bool StartMetadataImport(string pluginName, bool coverArtOnly, string titleNameSearch, string EpisodeName, int SeasonNo, int EpisodeNo)
        {
            foreach (MetaDataPluginDescriptor plugin in _metadataPlugins)
            {
                if (plugin.DataProviderName == pluginName) return StartMetadataImport(plugin, coverArtOnly, titleNameSearch, EpisodeName, SeasonNo, EpisodeNo); 
            }
            return false;
        }

        internal bool StartMetadataImport(MetaDataPluginDescriptor plugin, bool coverArtOnly, string titleNameSearch, string EpisodeName, int ? SeasonNo, int ? EpisodeNo)
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
                            searchResultForm = new frmSearchResult(plugin, titleNameSearch, EpisodeName, SeasonNo, EpisodeNo, true);
                        }
                        else
                        {
                            searchResultForm = new frmSearchResult(plugin, titleNameSearch, EpisodeName, SeasonNo, EpisodeNo, false);
                        }

                        DialogResult result = searchResultForm.ShowDialog(); // ShowResults(plugin.GetAvailableTitles());
                        if (result == DialogResult.OK)
                        {
                            Cursor = Cursors.WaitCursor;
                            Title t = plugin.PluginDLL.GetTitle(searchResultForm.SelectedTitleIndex);
                            t.MetadataSourceName = plugin.DataProviderName;

                            if (t != null)
                            {
                                LoadFanartFromPlugin(plugin, t);

                                if (coverArtOnly)
                                {
                                    titleEditor.EditedTitle.FrontCoverPath = t.FrontCoverPath;
                                    titleEditor.EditedTitle.FrontCoverPath = t.BackCoverPath;
                                }
                                else
                                {
                                    titleEditor.EditedTitle.CopyMetadata(t, searchResultForm.OverwriteMetadata);
                                }
                            }
                            CheckGenresAgainstSupported(titleEditor.EditedTitle);
                            titleEditor.RefreshEditor(); 
                            Cursor = Cursors.Default;

                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        // Import metadata based on field mappings and configured default plugin
                        Dictionary<string, List<string>> mappings = OMLEngine.Settings.SettingsManager.MetaDataMap_PropertiesByPlugin();
                        // Loop through configured mappings
                        Type tTitle = typeof(Title);
                        MetaDataPluginDescriptor metadata;
                        Title title;
                        bool loadedfanart = false;

                        foreach (KeyValuePair<string, List<string>> map in mappings)
                        {
                            try
                            {
                                if (map.Key == OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin) continue;
                                metadata = _metadataPlugins.First(p => p.DataProviderName == map.Key);
                                if ((metadata.DataProviderCapabilities & MetadataPluginCapabilities.SupportsMovieSearch) != 0)
                                {
                                    metadata.PluginDLL.SearchForMovie(titleNameSearch,1);
                                    title = metadata.PluginDLL.GetBestMatch();
                                    if (title != null)
                                    {
                                        Utilities.DebugLine("[OMLDatabaseEditor] Found movie " + titleNameSearch + " using plugin " + map.Key);
                                        foreach (string property in map.Value)
                                        {
                                            switch (property)
                                            {
                                                case "FanArt":
                                                    loadedfanart = true;
                                                    LoadFanartFromPlugin(metadata, title);
                                                    break;
                                                case "Genres":
                                                    titleEditor.EditedTitle.Genres.Clear();
                                                    //titleEditor.EditedTitle.Genres.AddRange(title.Genres.ToArray<string>());
                                                    foreach (string genre in title.Genres.ToArray<string>())
                                                        titleEditor.EditedTitle.AddGenre(genre);
                                                    break;
                                                default:
                                                    Utilities.DebugLine("[OMLDatabaseEditor] Using value for " + property + " from plugin " + map.Key);
                                                    System.Reflection.PropertyInfo prop = tTitle.GetProperty(property);
                                                    prop.SetValue(titleEditor.EditedTitle, prop.GetValue(title, null), null);
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Utilities.DebugLine("[OMLDatabaseEditor] Processing date from {0} Caused an Exception {1}", map.Key, ex);

                            }
                        }
                        // Use default plugin for remaining fields
                        metadata = _metadataPlugins.First(p => p.DataProviderName == OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin);

                        if ((metadata.DataProviderCapabilities & MetadataPluginCapabilities.SupportsMovieSearch) != 0)
                        {
                            metadata.PluginDLL.SearchForMovie(titleNameSearch,1);
                            title = metadata.PluginDLL.GetBestMatch();
                            title.MetadataSourceName = metadata.DataProviderName;

                            if (title != null)
                            {
                                if (!loadedfanart) { LoadFanartFromPlugin(metadata, title); }

                                Utilities.DebugLine("[OMLDatabaseEditor] Found movie " + titleNameSearch + " using default plugin " + metadata.DataProviderName);
                                titleEditor.EditedTitle.CopyMetadata(title, false);
                            }
                        }

                        Cursor = Cursors.Default;
                        CheckGenresAgainstSupported(titleEditor.EditedTitle);
                        titleEditor.RefreshEditor();
                        return true;
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
                DownloadingBackDropsForm dbdForm = new DownloadingBackDropsForm();
                dbdForm.Show();
                metadata.PluginDLL.DownloadBackDropsForTitle(titleEditor.EditedTitle, 0);
                dbdForm.Hide();
                dbdForm.Dispose();                
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
                        fromPreferredSourcesToolStripMenuItem1.Visible = !String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin);
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
            CreateTitle(null, "New Title", TitleTypes.Unknown);
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
                CreateTitle(null, movieName.MovieName(), TitleTypes.Unknown);
                /*TitleTypes*string name = movieName.MovieName();
                Title newTitle = new Title();
                newTitle.DateAdded = DateTime.Now;
                newTitle.Name = name;

                // Add the title now to get the title ID
                TitleCollectionManager.AddTitle(newTitle);

                titleEditor.LoadDVD(newTitle);*/
                StartMetadataImport(plugin, false);
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

                if (StartMetadataImport(plugin, false))
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

                if (StartMetadataImport(null, false))
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
                    pr.StartInfo.FileName = @"c:\program files\openmedialibrary\TranscoderTester.exe";
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
        #endregion


        private void lbMetadata_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Cursor = Cursors.WaitCursor;
            MetaDataPluginDescriptor metadata = lbMetadata.SelectedItem as MetaDataPluginDescriptor;
            if (metadata != null)
            {
                StartMetadataImport(metadata, false);
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

        private void mainNav_ActiveGroupChanged(object sender, DevExpress.XtraNavBar.NavBarGroupEventArgs e)
        {
            _loading = true;
            ToggleSaveState(false);


            if (e.Group == groupMetadata)
                LoadMetadata();

            if (e.Group == groupImport)
                LoadImporters();


            if (e.Group == groupMediaTree)
            {
                //LoadFolderSet();
                PopulateMediaTree();

                LoadMovies();
                PopulateMovieListV2(SelectedTreeRoot);

                splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Dock = DockStyle.Fill;
                splitContainerNavigator.Panel2.Controls["genreMetaDataEditor"].Visible = false;
                splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Visible = true;
                splitContainerNavigator.Panel2.Controls["bioDataEditor"].Visible = false;
            }

            if (e.Group == groupBioData)
            {
                lbBioData.Items.Clear();
                lbBioData.Items.AddRange(TitleCollectionManager.GetAllBioDatas().OrderBy(b => b.FullName).ToArray());
                splitContainerNavigator.Panel2.Controls["bioDataEditor"].Dock = DockStyle.Fill;
                splitContainerNavigator.Panel2.Controls["genreMetaDataEditor"].Visible = false;
                splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Visible = false;
                splitContainerNavigator.Panel2.Controls["bioDataEditor"].Visible = true;
            }

            if (e.Group == groupGenresMetadata)
            {
                lbGenreMetadata.Items.Clear();
                lbGenreMetadata.Items.AddRange(TitleCollectionManager.GetAllGenreMetaDatas().OrderBy(g => g.Name).ToArray());

                splitContainerNavigator.Panel2.Controls["genreMetaDataEditor"].Dock = DockStyle.Fill;
                splitContainerNavigator.Panel2.Controls["genreMetaDataEditor"].Visible = true;
                splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Visible = false;
                splitContainerNavigator.Panel2.Controls["bioDataEditor"].Visible = false;
            }

            _loading = false;
        }

        private void navBarControl1_NavPaneStateChanged(object sender, EventArgs e)
        {
            if (navBarControl1.OptionsNavPane.NavPaneState == DevExpress.XtraNavBar.NavPaneState.Collapsed)
            {
                splitContainerTitles.SplitterPosition = 38;
            }
            else
            {
                splitContainerTitles.SplitterPosition = navBarControl1.OptionsNavPane.ExpandedWidth + 4;
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
            CreateFolder("New Movies", TitleTypes.Collection);
        }
        
        private void miCreateFolderTVShow_Click(object sender, EventArgs e)
        {
            CreateFolder("New TV Show", TitleTypes.TVShow);
        }

        private void miCreateFolderTVSeason_Click(object sender, EventArgs e)
        {
            CreateFolder("New TV Season", TitleTypes.Season);
        }
  
        private void miCreateTVEpisode_Click(object sender, EventArgs e)
        {
            if (treeMedia.SelectedNode.Name == "All Media")
            {
                // Root Title
                CreateTitle(null, "New TV Episode", TitleTypes.Episode);
            }
            else
            {
                int parentid = Convert.ToInt32(treeMedia.SelectedNode.Name);
                CreateTitle(parentid, "New TV Episode", TitleTypes.Episode);
            }
        }
  
        private void miCreateTitle_Click(object sender, EventArgs e)
        {
            if (treeMedia.SelectedNode.Name == "All Media")
            {
                // Root Title
                CreateTitle(null, "New Movie", TitleTypes.Movie);
            }
            else
            {
                int parentid = Convert.ToInt32(treeMedia.SelectedNode.Name);
                CreateTitle(parentid, "New Movie", TitleTypes.Movie);
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
                            int[] sitems = new int[1];
                            sitems[0] = Convert.ToInt32(nodeUnderMouse.Name);
                            treeMedia.DoDragDrop(sitems, DragDropEffects.Move);
                            //lbTitles.DoDragDrop(sitems, DragDropEffects.Move);
                        }
                    }
                }
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
                if (!String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin))
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

                cms.Show(lvTitles.PointToScreen(e.Location));
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
                    lvTitles.DoDragDrop(sitems, DragDropEffects.Move);
                }
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

            foreach (ListViewItem item in sic)
            {
                int id = Convert.ToInt32(item.Text);

                if (titleEditor.EditedTitle != null && titleEditor.EditedTitle.Id == id)
                    titleEditor.ClearEditor(true);

                TitleCollectionManager.DeleteTitle(_movieList[id]);

                _movieList.Remove(id);
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

            if (lbGenreMetadata.SelectedItem == e.Item)
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
            SaveCurrentGenreMetaData();
            if (lbGenreMetadata.SelectedItem != null)
            {
                genreMetaDataEditor.LoadGenre((GenreMetaData)lbGenreMetadata.SelectedItem);
                this.Text = APP_TITLE + " - " + genreMetaDataEditor.EditedGenreMetaData.Name;
            }
        }

        private void AddGenreMetaData()
        {
            SaveCurrentGenreMetaData();

            GenreMetaData gmd = new GenreMetaData();
            gmd.Name = "New Genre";
            
            lbGenreMetadata.Items.Add(gmd);
            lbGenreMetadata.SelectedItem = gmd;

            TitleCollectionManager.AddGenreMetaData(gmd);

            genreMetaDataEditor.LoadGenre((GenreMetaData)gmd);
        }

        private void RemoveGenreMetaData()
        {
            if (lbGenreMetadata.SelectedItem != null)
            {
                TitleCollectionManager.RemoveGenreMetaData((GenreMetaData)lbGenreMetadata.SelectedItem);
                lbGenreMetadata.Items.Remove(lbGenreMetadata.SelectedItem);
            }
        }

        private DialogResult SaveCurrentGenreMetaData()
        {
            DialogResult result;
            if (genreMetaDataEditor.EditedGenreMetaData != null && genreMetaDataEditor.Status == GenreMetaDataEditor.GenreMetaDataStatus.UnsavedChanges)
            {
                result = XtraMessageBox.Show("You have unsaved changes to " + genreMetaDataEditor.EditedGenreMetaData.Name + ". Would you like to save your changes?", "Save Changes?", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    TitleCollectionManager.SaveGenreMetaDataChanges();           

                    //_movieList[titleEditor.EditedTitle.Id] = titleEditor.EditedTitle;
                    //SaveChanges();
                }
                if (result == DialogResult.No)
                {
                    // Force reload of title
                    //titleEditor.EditedTitle.ReloadTitle();
                    //lbTitles.Refresh();
                }
            }
            else
            {
                result = DialogResult.Yes;
            }
            return result;
        }

        private void genreMetaDataEditor_GenreMetaDataChanged(object sender, EventArgs e)
        {
            if (genreMetaDataEditor.EditedGenreMetaData != null)
                this.Text = APP_TITLE + " - " + genreMetaDataEditor.EditedGenreMetaData.Name + "*";
            else
                this.Text = APP_TITLE;

            ToggleSaveState(true);
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

            if (lbBioData.SelectedItem == e.Item)
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
            SaveCurrentBioData();
            if (lbBioData.SelectedItem != null)
            {
                bioDataEditor.LoadBioData((BioData)lbBioData.SelectedItem);
                this.Text = APP_TITLE + " - " + bioDataEditor.EditedBioData.FullName;
            }
        }

        private void AddBioData()
        {
            SaveCurrentBioData();

            BioData bd = new BioData();
            bd.FullName = "New Person";
            lbBioData.Items.Add(bd);
            lbBioData.SelectedItem = bd;

            TitleCollectionManager.AddBioData(bd);

            bioDataEditor.LoadBioData((BioData)bd);
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

                    //_movieList[titleEditor.EditedTitle.Id] = titleEditor.EditedTitle;
                    //SaveChanges();
                }
                if (result == DialogResult.No)
                {
                    // Force reload of title
                    //titleEditor.EditedTitle.ReloadTitle();
                    //lbTitles.Refresh();
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
            if (bioDataEditor.EditedBioData != null)
                this.Text = APP_TITLE + " - " +bioDataEditor.EditedBioData.FullName + "*";
            else
                this.Text = APP_TITLE;

            ToggleSaveState(true);
        }
        #endregion


        #region Title & Folder Creation
        private void CreateTitle(int? parentid, string Name, TitleTypes titletype)
        {
            Title newTitle = new Title();
            newTitle.Name = Name;
            
            if (parentid == null)
            {
                newTitle.TitleType = TitleTypes.Root | titletype;
            }
            else
            {
                newTitle.TitleType = titletype;
                newTitle.ParentTitleId = (int)parentid;
            }
            newTitle.DateAdded = DateTime.Now;

            // Add the title now to get the title ID
            TitleCollectionManager.AddTitle(newTitle);

            // Get the new title from the DB and add it to the title list 
            Title addedTitle = TitleCollectionManager.GetTitle(newTitle.Id);
            _movieList.Add(newTitle.Id, addedTitle);

            if ((titletype & TitleTypes.AllMedia) != 0)
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

        private void CreateFolder(string Name, TitleTypes titletype)
        {
            if (treeMedia.SelectedNode.Name == "All Media")
            {
                // Root Node
                CreateTitle(null, Name, titletype);
            }
            else
            {
                int parentid = Convert.ToInt32(treeMedia.SelectedNode.Name);
                CreateTitle(parentid, Name, titletype);
            }
        }
        #endregion


        #region Title Drag and Drop support
        private ToolTip tt;
        TreeNode currentmovetonode;
        
        private void treeMedia_DragEnter(object sender, DragEventArgs e)
        {
            currentmovetonode = null;

            treeMedia_DragOver(sender, e);
            //if (e.Data.GetDataPresent(typeof(int[])))
            //{
            //    e.Effect = DragDropEffects.Move;
            //}
        }

        private void treeMedia_DragDrop(object sender, DragEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            
            bool foldermoved = false;

            if (tt != null)
            {
                tt.Hide(this);
                tt.Dispose();
                tt = null;
            }

            TreeNode selectednode = treeMedia.GetNodeAt(treeMedia.PointToClient(new Point(e.X, e.Y)));

            if (e.Data.GetDataPresent(typeof(int[])))
            {
                int[] items = (int[])e.Data.GetData(typeof(int[]));
                foreach (int item in items)
                {
                    if ((_movieList[item].TitleType & TitleTypes.AllFolders) != 0)
                    {
                        foldermoved = true;
                    }

                    if (selectednode.Name == "All Media")
                    {
                        // Item is being moved to root
                        _movieList[item].ParentTitleId = null;
                        _movieList[item].TitleType = _movieList[item].TitleType | TitleTypes.Root;
                    }
                    else
                    {
                        // Item is being moved to a folder
                        int parentid = Convert.ToInt32(selectednode.Name);
                        _movieList[item].ParentTitleId = parentid;
                        _movieList[item].TitleType = _movieList[item].TitleType & (TitleTypes.AllMedia | TitleTypes.AllFolders);
                    }
                }
                TitleCollectionManager.SaveTitleUpdates();
            }

            if (foldermoved)
            {
                PopulateMediaTree();
                if (_mediaTree.ContainsKey((int)SelectedTreeRoot))
                {
                    treeMedia.SelectedNode = _mediaTree[(int)SelectedTreeRoot];
                }
            }

            PopulateMovieListV2(SelectedTreeRoot);

            this.Cursor = Cursors.Default;
        }

        private void treeMedia_DragOver(object sender, DragEventArgs e)
        {
            if (tt == null)
            {
                tt = new ToolTip();
            }

            Point mouseLocation = treeMedia.PointToClient(new Point(e.X, e.Y));
            TreeNode movetonode = treeMedia.GetNodeAt(mouseLocation);

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
                        if (e.Data.GetDataPresent(typeof(int[])))
                        {
                            int[] items = (int[])e.Data.GetData(typeof(int[]));
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
                    }
                }

                if (validmove)
                {
                    e.Effect = DragDropEffects.Move;
                    tt.Show("Move to " + movetonode.Text, this, this.PointToClient(new Point(e.X, e.Y + 30)));
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                    tt.Show("Unable to move here!", this, this.PointToClient(new Point(e.X, e.Y + 30)));
                }
                currentmovetonode = movetonode;
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
                //cms.ite
                cms.ItemClicked += new ToolStripItemClickedEventHandler(SortOrderChanged);
                cms.Items.Add("Name");
                cms.Items.Add("Sort Name");
                cms.Items.Add("Run Time");
                cms.Items.Add("Date Added");
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
    }
}
