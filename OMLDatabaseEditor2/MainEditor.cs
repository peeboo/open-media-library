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
        internal static List<IOMLMetadataPlugin> _metadataPlugins = new List<IOMLMetadataPlugin>();
        private const string APP_TITLE = "OML Movie Manager";
        private bool _loading = false;
        private AppearanceObject Percent30 = null;
        private AppearanceObject Percent40 = null;
        private AppearanceObject Percent50 = null;
        private AppearanceObject Percent60 = null;
        private AppearanceObject Percent70 = null;
        private AppearanceObject Percent80 = null;
        //private List<Title> _movieList;
        private Dictionary<int,Title> _movieList;
        private Dictionary<int, TreeNode> _mediaTree;
        private Dictionary<int, int> _parentchildRelationship; // titleid, parentid
        public List<String> DXSkins;

        int? SelectedTreeRoot;

        LinearGradientBrush _brushTreeViewSelected; 

        public MainEditor()
        {
            OMLEngine.Utilities.RawSetup();

            SplashScreen2.ShowSplashScreen();

            InitializeComponent();

            InitData();
            
            splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Dock = DockStyle.Fill;

            splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Visible = true;
            splitContainerNavigator.Panel2.Controls["personEditor1"].Visible = false;
            splitContainerNavigator.Panel2.Controls["genreEditor1"].Visible = false;

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
            LoadMovies();
            PopulateMediaTree();
            PopulateMovieListV2(SelectedTreeRoot);


            SplashScreen2.SetStatus(80, "Loading MRU Lists.");
            this.titleEditor.SetMRULists();

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

            switch (state)
            {
                case OMLEngine.DatabaseManagement.DatabaseInformation.SQLState.OK:
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
            foreach (IOMLMetadataPlugin plugin in _metadataPlugins)
            {
                ToolStripMenuItem newItem = new ToolStripMenuItem("From " + plugin.PluginName);
                newItem.Tag = plugin;
                newItem.Click += new EventHandler(this.fromMetaDataToolStripMenuItem_Click);
                newToolStripMenuItem.DropDownItems.Add(newItem);

                newItem = new ToolStripMenuItem("From " + plugin.PluginName);
                newItem.Tag = plugin;
                newItem.Click += new EventHandler(this.fromMetaDataToolStripMenuItem_Click);
                newMovieSplitButton.DropDownItems.Add(newItem);

                ToolStripMenuItem metadataItem = new ToolStripMenuItem("From " + plugin.PluginName);
                metadataItem.Tag = plugin;
                metadataItem.Click += new EventHandler(this.miMetadataMulti_Click);
                miMetadataMulti.DropDownItems.Add(metadataItem);
            }

            if (String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin))
                fromPreferredSourcesToolStripMenuItem.Visible = false;

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

            foreach (string tag in from t in TitleCollectionManager.GetAllTags(null) select t.Name)
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

        private static void LoadMetadataPlugins(string pluginType, List<IOMLMetadataPlugin> pluginList)
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
                    objPlugin = (IOMLMetadataPlugin)PluginServices.CreateInstance(oPlugin);
                    pluginList.Add(objPlugin);
                    objPlugin.Initialize(new Dictionary<string, string>());
                }
                plugins = null;
            }
        }

        private void LoadMovies()
        {
            if (mainNav.ActiveGroup != groupMediaTree) return;
            Cursor = Cursors.WaitCursor;
            if (allMoviesToolStripMenuItem1.Checked)
            {
                //_movieList = TitleCollectionManager.GetAllTitles().ToList<Title>();
                _movieList = TitleCollectionManager.GetAllTitles(TitleTypes.AllMedia | TitleTypes.AllFolders).ToDictionary(k => k.Id);
                //PopulateMovieList(_movieList);
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

        /// <summary>
        /// This populates the media tree view
        /// </summary>
        private void PopulateMediaTree()
        {
            Dictionary<int, Title> mediatreefolders = TitleCollectionManager.GetAllTitles(TitleTypes.AllFolders).ToDictionary(k => k.Id);

            if (_mediaTree == null)
            {
                _mediaTree = new Dictionary<int, TreeNode>();
            }
            else
            {
                _mediaTree.Clear();
            }

            if (_parentchildRelationship == null)
            {
                _parentchildRelationship = new Dictionary<int, int>();
            }
            else
            {
                _parentchildRelationship.Clear();
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
                _parentchildRelationship[title.Value.Id] = title.Value.ParentTitleId;
                //if (title.Value.Id == title.Value.ParentTitleId) { rootnode.Nodes.Add(tn); }
                if ((title.Value.TitleType & TitleTypes.Root) != 0) { rootnode.Nodes.Add(tn); }
            }


            foreach (KeyValuePair<int, TreeNode> kvp in _mediaTree)
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

/*        private void PopulateMovieListV2(int? roottitleid)
        {
            lbTitles.Items.Clear();

            Dictionary<int, List<int>> _parentchildRelationship = new Dictionary<int,List<int>>(); // titleid, parentid
            foreach (KeyValuePair<int, Title> title in _movieList)
            {
                // Create relationship dictionary
                if (title.Value.Id != title.Value.ParentTitleId)
                {
                    if (!_parentchildRelationship.ContainsKey(title.Value.ParentTitleId))
                    {
                        _parentchildRelationship[title.Value.ParentTitleId] = new List<int>();
                    }
                    _parentchildRelationship[title.Value.ParentTitleId].Add(title.Value.Id);
                }
            }

            if (roottitleid != null)
            {
                // We are looking at a sub folder
                PopulateMovieListV2Sub(roottitleid, _parentchildRelationship, 0);
            }
            else
            {
                // We are looking at all items including root items
                foreach (KeyValuePair<int, Title> title in _movieList)
                {
                    if ((title.Value.TitleType & TitleTypes.Root) != 0)
                    {
                        if ((title.Value.TitleType & TitleTypes.AllFolders) != 0)
                        {
                            // This is a folder. Query child items
                            PopulateMovieListV2Sub(title.Value.Id, _parentchildRelationship, 1);
                        }
                        else
                        {
                            // This is a media title at the root level. Show it
                            lbTitles.Items.Insert(0,title.Value);
                        }
                    }
                }
            }


            if (titleEditor.EditedTitle != null)
            {
                lbTitles.SelectedItem = TitleCollectionManager.GetTitle(titleEditor.EditedTitle.Id);
            }
        }

        private void PopulateMovieListV2Sub(int? roottitleid, Dictionary<int, List<int>> _parentchildRelationship, int Level)
        {
            if (_movieList.ContainsKey((int)roottitleid))
            {
                if ((OMLEngine.Settings.OMLSettings.ShowSubFolderTitles) ||
                    ((_movieList[(int)roottitleid].TitleType & TitleTypes.AllMedia) != 0))
                {
                    lbTitles.Items.Add(_movieList[(int)roottitleid]);
                }

                if ((OMLEngine.Settings.OMLSettings.ShowSubFolderTitles) || (Level < 1))
                {

                    if (_parentchildRelationship.ContainsKey((int)roottitleid))
                    {
                        foreach (int id in _parentchildRelationship[(int)roottitleid])
                        {
                            PopulateMovieListV2Sub(id, _parentchildRelationship, Level + 1);
                        }
                    }
                }
            }
        }*/


        private void PopulateMovieListV2(int? roottitleid)
        {
            lbTitles.Items.Clear();

            if (roottitleid != null)
            {
                // We are looking at a parent item
                PopulateMovieListV2Sub(roottitleid, 0);
            }
            else
            {
                // We are looking at all items including root items
                Title amt = new Title();
                amt.Name = "All Media";
                amt.TitleType = TitleTypes.Collection;
                lbTitles.Items.Add(amt);

                // Get All Root Titles
                var titles = (from t in _movieList
                                      where (t.Value.TitleType & TitleTypes.AllMedia) != 0 &&
                                      (t.Value.TitleType & TitleTypes.Root) != 0
                                      select t.Value).ToList();
                
                lbTitles.Items.AddRange(SortTitles(titles).ToArray());

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


            if (titleEditor.EditedTitle != null)
            {
                lbTitles.SelectedItem = TitleCollectionManager.GetTitle(titleEditor.EditedTitle.Id);
            }
        }

        private void PopulateMovieListV2Sub(int? roottitleid, int Level)
        {
            if ((OMLEngine.Settings.OMLSettings.ShowSubFolderTitles) || (Level < 1))
            {
                if (_movieList.ContainsKey((int)roottitleid))
                {

                    lbTitles.Items.Add(_movieList[(int)roottitleid]);

                    // Get All Root Titles
                    var titles = (from t in _movieList
                                          where (t.Value.TitleType & TitleTypes.AllMedia) != 0 &&
                                          t.Value.ParentTitleId == roottitleid
                                          select t.Value).ToList();

                    lbTitles.Items.AddRange(SortTitles(titles).ToArray());

                    // Get All Root Folders
                    titles =  (from t in _movieList
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

        private IEnumerable<Title> SortTitles(IEnumerable<Title> titles)
        {
            switch (OMLEngine.Settings.OMLSettings.DBETitleSortOrder)
            {
                case "Name":
                    return titles.OrderBy(st => st.SortName);
                case "Runtime":
                    return titles.OrderBy(st => st.Runtime);
                case "DateAdded":
                    return titles.OrderBy(st => st.DateAdded);
                case "ModifiedDate":
                    return titles.OrderBy(st => st.ModifiedDate);
                case "ProductionYear":
                    return titles.OrderBy(st => st.ProductionYear);
                case "ReleaseDate":
                    return titles.OrderBy(st => st.ReleaseDate);
                default:
                    return titles.OrderBy(st => st.SortName);
            }
        }


 //       private void PopulateMovieList(List<Title> titles)
        /*private void PopulateMovieList(Dictionary<int, Title> titles)
        {
        


            //lbTitles.DataSource = titles;

            lbMovies.Items.Clear();
            //_titleCollection.SortBy("SortName", true);

            // throwing this into a new list shouldn't be needed but it seems like this control wants it
            //lbMovies.DataSource = titles;
            if (titleEditor.EditedTitle != null)
            {
                List<Title> matches = (from title in titles
                                       where title.Id == titleEditor.EditedTitle.Id
                                       select title).ToList<Title>();
                if (matches.Count == 0)
                {
                    lbMovies.SelectedIndex = -1;
                    lbMovies.SelectedItem = null;
                    titleEditor.ClearEditor();
                }
                else
                {
                    lbMovies.SelectedItem = matches[0];
                }
            }
            else
            {
                lbMovies.SelectedIndex = -1;
                lbMovies.SelectedItem = null;
                titleEditor.ClearEditor();
            }
        }
*/
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
                    lbTitles.Refresh();
                }
            }
            else
            {
                result = DialogResult.Yes;
            }
            return result;
        }

        public void LoadTitlesIntoDatabase(OMLPlugin plugin)
        {
            try
            {
                Utilities.DebugLine("[OMLImporter] Titles loaded, beginning Import process");
                IList<Title> titles = plugin.GetTitles();
                Utilities.DebugLine("[OMLImporter] " + titles.Count + " titles found in input file");

                int numberOfTitlesAdded = 0;
                int numberOfTitlesSkipped = 0;
                pgbProgress.Value = 0;
                bool YesToAll = true;// false;

                pgbProgress.Maximum = titles.Count;

                foreach (Title t in titles)
                {
                    pgbProgress.Value++;
                    if (TitleCollectionManager.ContainsDisks(t.Disks))
                    {
                        numberOfTitlesSkipped++;
                        continue;
                    }

                    if (!YesToAll)
                    {
                        //TODO: Need to show a UI that let's the User decide whether to import all titles or be selective about it

                        /*Console.WriteLine("Would you like to add this title? (y/n/a)");
                        string response = Console.ReadLine();
                        switch (response.ToUpper())
                        {
                            case "Y":
                                mainTitleCollection.Add(t);
                                numberOfTitlesAdded++;
                                break;
                            case "N":
                                numberOfTitlesSkipped++;
                                break;
                            case "A":
                                YesToAll = true;
                                mainTitleCollection.Add(t);
                                numberOfTitlesAdded++;
                                break;
                            default:
                                break;
                        }*/
                    }
                    else
                    {                        
                        TitleCollectionManager.AddTitle(t);
                        numberOfTitlesAdded++;
                    }
                }

                // this is for saving the resized image
                TitleCollectionManager.SaveTitleUpdates();

                XtraMessageBox.Show("Added " + numberOfTitlesAdded + " titles\nSkipped " + numberOfTitlesSkipped + " titles\n", "Import Results");

                plugin.GetTitles().Clear();
            }
            catch (Exception e)
            {
                XtraMessageBox.Show("Exception in LoadTitlesIntoDatabase: {0}", e.Message);
                Utilities.DebugLine("[OMLImporter] Exception in LoadTitlesIntoDatabase: " + e.Message);
            }
        }

        private void ShowNonFatalErrors(string[] errors)
        {
            Controls.NonFatalErrors nfe = new Controls.NonFatalErrors();
            nfe.LoadErrors(errors);
            nfe.Show();
        }

        private bool StartMetadataImport(IOMLMetadataPlugin plugin, bool coverArtOnly)
        {
            return StartMetadataImport(plugin, coverArtOnly, titleEditor.EditedTitle.Name, null);
        }

        internal bool StartMetadataImport(string pluginName, bool coverArtOnly, string titleNameSearch, Form targetForm)
        {
            foreach (IOMLMetadataPlugin plugin in _metadataPlugins)
            {
                if (plugin.PluginName == pluginName) return StartMetadataImport(plugin, coverArtOnly, titleNameSearch, targetForm);
            }
            return false;
        }

        internal bool StartMetadataImport(IOMLMetadataPlugin plugin, bool coverArtOnly, string titleNameSearch, Form targetForm)
        {
            try
            {
                if (titleNameSearch != null)
                {
                    Cursor = Cursors.WaitCursor;
                    if (plugin != null)
                    {
                        // Update movie based on specified plugin
                        plugin.SearchForMovie(titleNameSearch);
                        frmSearchResult searchResultForm;
                        if (targetForm == null) searchResultForm = new frmSearchResult(this);
                        else searchResultForm = targetForm as frmSearchResult;
                        searchResultForm.ReSearchText = titleNameSearch;
                        searchResultForm.LastMetaPluginName = (string)plugin.PluginName;
                        Cursor = Cursors.Default;
                        DialogResult result = searchResultForm.ShowResults(plugin.GetAvailableTitles());
                        if (result == DialogResult.OK)
                        {
                            Title t = plugin.GetTitle(searchResultForm.SelectedTitleIndex);
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
                        IOMLMetadataPlugin metadata;
                        Title title;
                        bool loadedfanart = false;

                        foreach (KeyValuePair<string, List<string>> map in mappings)
                        {
                            try
                            {
                                if (map.Key == OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin) continue;
                                metadata = _metadataPlugins.First(p => p.PluginName == map.Key);
                                metadata.SearchForMovie(titleNameSearch);
                                title = metadata.GetBestMatch();
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
                            catch (Exception ex)
                            {
                                Utilities.DebugLine("[OMLDatabaseEditor] Processing date from {0} Caused an Exception {1}", map.Key, ex);

                            }
                        }
                        // Use default plugin for remaining fields
                        metadata = _metadataPlugins.First(p => p.PluginName == OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin);
                        metadata.SearchForMovie(titleNameSearch);
                        title = metadata.GetBestMatch();
                        if (title != null)
                        {
                            if (!loadedfanart) { LoadFanartFromPlugin(metadata, title); }
 
                            Utilities.DebugLine("[OMLDatabaseEditor] Found movie " + titleNameSearch + " using default plugin " + metadata.PluginName);
                            titleEditor.EditedTitle.CopyMetadata(title, false);
                        }

                        Cursor = Cursors.Default;
                        CheckGenresAgainstSupported(titleEditor.EditedTitle);
                        titleEditor.RefreshEditor();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[OMLDatabaseEditor] Exception {0}", ex);
                Cursor = Cursors.Default;
                return false;
            }
        }

        private void LoadFanartFromPlugin(IOMLMetadataPlugin metadata, Title title)
        {
            if (metadata.SupportsBackDrops())
            {                
                DownloadingBackDropsForm dbdForm = new DownloadingBackDropsForm();
                dbdForm.Show();
                metadata.DownloadBackDropsForTitle(titleEditor.EditedTitle, 0);
                dbdForm.Hide();
                dbdForm.Dispose();                
            }
        }

        private void CheckGenresAgainstSupported(Title title)
        {
     /*       List<String> genreList = new List<String>();
 //           if (Properties.Settings.Default.gsValidGenres != null
 //           && Properties.Settings.Default.gsValidGenres.Count > 0)
            int genreCount = OMLEngine.Settings.SettingsManager.GetAllGenres.Count();
            if (genreCount > 0)
            {
                //String[] arrGenre = new String[genreCount];
                //Properties.Settings.Default.gsValidGenres.CopyTo(arrGenre, 0);
                genreList.AddRange(OMLEngine.Settings.SettingsManager.GetAllGenres);
                Dictionary<string, string> genreIssuesList = new Dictionary<string, string>();
                Dictionary<string, string> genreChanges = new Dictionary<string, string>();
                foreach (string genre in title.Genres)
                {
                    string newGenre = genre.Trim();
                    if (!genreList.Contains(newGenre))
                    {
                        if (!string.IsNullOrEmpty(OMLEngine.Settings.SettingsManager.GenreMap_GetMapping(newGenre)))
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
                    title.Genres.Remove(genre);
                    // Mapping contains empty string when user wants a specific genre ignored.
                    if (!String.IsNullOrEmpty(genreChanges[genre]) && !title.Genres.Contains(genreChanges[genre]))
                        title.Genres.Add(genreChanges[genre]);
                }
                if (genreIssuesList.Keys.Count > 0)
                {
                    ResolveGenres resolveGenres = new ResolveGenres(genreIssuesList, title);
                    resolveGenres.ShowDialog();
                }
            }*/
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
                        IOMLMetadataPlugin plugin = selectPlugin.SelectedPlugin();
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
                lbTitles.Refresh();
                
                if ((editedTitle.TitleType & TitleTypes.AllFolders) != 0)
                {
                    // If the current edited title is a folder, refresh the MediaTree
                    PopulateMediaTree();
                    if (_mediaTree.ContainsKey((int)SelectedTreeRoot))
                    {
                        treeMedia.SelectedNode = _mediaTree[(int)SelectedTreeRoot];
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

        private void ToolStripOptionClick(object sender, EventArgs e)
        {
            if (sender == saveToolStripButton || sender == saveToolStripMenuItem)
            {
                SaveChanges();
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
                        fromPreferredSourcesToolStripMenuItem.Visible = !String.IsNullOrEmpty(OMLEngine.Settings.OMLSettings.DefaultMetadataPlugin);
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
                /*DiskMoverFrm dsm = new DiskMoverFrm();
                if (dsm.ShowDialog(this) == DialogResult.OK)
                {
                    string fromFolder = dsm.fromFolder;
                    string toFolder = dsm.toFolder;
                    List<Title> titles = _titleCollection.FindByFolder(fromFolder);
                    foreach (Title title in titles)
                    {
                        foreach (Disk disk in title.Disks)
                        {
                            if (disk.Path.StartsWith(fromFolder))
                            {
                                disk.Path = disk.Path.Replace(fromFolder, toFolder);
                            }
                        }
                        if (dsm.withImages)
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
                        }
                    }
                    TitleCollectionManager.SaveTitleUpdates();
                }*/
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
                        titleEditor.ClearEditor();
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
            }
        }

        private void fromScratchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTitle(null);
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
            IOMLMetadataPlugin plugin = selectedItem.Tag as IOMLMetadataPlugin;
            // Ask for name of movie
            NewMovieName movieName = new NewMovieName();
            if (movieName.ShowDialog() == DialogResult.OK)
            {
                string name = movieName.MovieName();
                Title newTitle = new Title();
                newTitle.DateAdded = DateTime.Now;
                newTitle.Name = name;

                // Add the title now to get the title ID
                TitleCollectionManager.AddTitle(newTitle);

                titleEditor.LoadDVD(newTitle);
                StartMetadataImport(plugin, false);
                this.Text = APP_TITLE + " - " + titleEditor.EditedTitle.Name + "*";
                ToggleSaveState(true);
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
                LoadMovies();

                splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Dock = DockStyle.Fill;
                splitContainerNavigator.Panel2.Controls["genreEditor1"].Visible = false;
                splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Visible = true;
                splitContainerNavigator.Panel2.Controls["personEditor1"].Visible = false;
            }
            
            if (e.Group == groupPeople)
            {
                lbPeople.Items.Clear();
                lbPeople.Items.AddRange(TitleCollectionManager.GetAllBioDatas().ToArray());
                splitContainerNavigator.Panel2.Controls["personEditor1"].Dock = DockStyle.Fill;
                splitContainerNavigator.Panel2.Controls["genreEditor1"].Visible = false;
                splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Visible = false;
                splitContainerNavigator.Panel2.Controls["personEditor1"].Visible = true;
            }

            if (e.Group == groupGenres)
            {
                lbGenreMetadata.Items.Clear();
                lbGenreMetadata.Items.AddRange(TitleCollectionManager.GetAllGenreMetaDatas().ToArray());

                splitContainerNavigator.Panel2.Controls["genreEditor1"].Dock = DockStyle.Fill;
                splitContainerNavigator.Panel2.Controls["genreEditor1"].Visible = true;
                splitContainerNavigator.Panel2.Controls["splitContainerTitles"].Visible = false;
                splitContainerNavigator.Panel2.Controls["personEditor1"].Visible = false;
            }

            _loading = false;
        }

        /*private void lbMovies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Cursor = Cursors.WaitCursor;
            if (SaveCurrentMovie() == DialogResult.Cancel)
            {
                _loading = true; //bypasses second save movie dialog
                lbMovies.SelectedItem = TitleCollectionManager.GetTitle(titleEditor.EditedTitle.Id);
                _loading = false;
            }
            else
            {
                Title selectedTitle = lbMovies.SelectedItem as Title;
                if (selectedTitle == null) return;

                titleEditor.LoadDVD(selectedTitle);
                this.Text = APP_TITLE + " - " + selectedTitle.Name;
                ToggleSaveState(false);
            }
            Cursor = Cursors.Default;
        }*/

        private void lbMetadata_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Cursor = Cursors.WaitCursor;
            IOMLMetadataPlugin metadata = lbMetadata.SelectedItem as IOMLMetadataPlugin;
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
                pgbProgress.Visible = true;
                string[] work = importer.GetWork();
                if (work != null)
                {
                    importer.DoWork(work);
                    LoadTitlesIntoDatabase(importer);
                }
                pgbProgress.Visible = false;
                lblCurrentStatus.Text = "";

                this.Refresh();
                string[] nonFatalErrors = importer.GetErrors;
                if (nonFatalErrors.Length > 0)
                    ShowNonFatalErrors(nonFatalErrors);
                lbImport.SelectedItem = null;
                lbImport.SelectedIndex = -1;
            }
            Cursor = Cursors.Default;
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
            IOMLMetadataPlugin plugin = selectedItem.Tag as IOMLMetadataPlugin;

            //BaseListBoxControl.SelectedItemCollection collection = lbMovies.SelectedItems;
            pgbProgress.Visible = true;
            pgbProgress.Maximum = lbTitles.SelectedItems.Count;
            pgbProgress.Value = 0;
            foreach (Title title in lbTitles.SelectedItems)
            {
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
            }
            statusText.Text = "Finished updating metadata";
            pgbProgress.Visible = false;
            Application.DoEvents();
        }

        private void fromPreferredSourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*pgbProgress.Visible = true;
            pgbProgress.Maximum = lbMovies.SelectedItems.Count;
            pgbProgress.Value = 0;
            foreach (Title title in lbMovies.SelectedItems)
            {
                pgbProgress.Value++;
                statusText.Text = "Getting metadata for " + title.Name;
                Application.DoEvents();
                titleEditor.LoadDVD(title);
                this.Text = APP_TITLE + " - " + title.Name;
                ToggleSaveState(false);

                if (StartMetadataImport(null, false))
                {
                    _titleCollection.Replace(titleEditor.EditedTitle);
                    _titleCollection.saveTitleCollection();
                }
            }
            statusText.Text = "Finished updating metadata";
            pgbProgress.Visible = false;
            Application.DoEvents();*/
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
            // exporting is going to have to wait
            /*
            Cursor = Cursors.WaitCursor; 
            foreach (Title t in _titleCollection) 
                if (t.Disks.Count > 0) 
                    t.SerializeToXMLFile(t.Disks[0].Path + ".oml.xml"); 
            Cursor = Cursors.Default; 
             */
        }

       /* private void lbMovies_DrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            if (e.Item == null)
                return;

            Title currentTitle = TitleCollectionManager.GetTitle((int)e.Item);
            if (currentTitle.PercentComplete <= .3M)
            {
                if (Percent30 == null)
                {
                    Percent30 = (AppearanceObject)e.Appearance.Clone();
                    Percent30.BackColor = Color.Coral;
                    Percent30.BackColor2 = Color.Crimson;
                    Percent30.GradientMode = LinearGradientMode.Vertical;
                }
                e.Appearance.Combine(Percent30);
            }
            else if (currentTitle.PercentComplete <= .4M)
            {
                if (Percent40 == null)
                {
                    Percent40 = (AppearanceObject)e.Appearance.Clone();
                    Percent40.ForeColor = Color.White;
                    Percent40.BackColor = Color.CornflowerBlue;
                    Percent40.BackColor2 = Color.CadetBlue;
                    Percent40.GradientMode = LinearGradientMode.Vertical;
                }
                e.Appearance.Combine(Percent40);
            }
            else if (currentTitle.PercentComplete <= .5M)
            {
                if (Percent50 == null)
                {
                    Percent50 = (AppearanceObject)e.Appearance.Clone();
                    Percent50.ForeColor = Color.Black;
                    Percent50.BackColor = Color.MediumSpringGreen;
                    Percent50.BackColor2 = Color.LightSeaGreen;
                    Percent50.GradientMode = LinearGradientMode.Vertical;
                }
                e.Appearance.Combine(Percent50);
            }
            else if (currentTitle.PercentComplete <= .6M)
            {
                if (Percent60 == null)
                {
                    Percent60 = (AppearanceObject)e.Appearance.Clone();
                    Percent60.ForeColor = Color.Black;
                    Percent60.BackColor = Color.Yellow;
                    Percent60.BackColor2 = Color.Gold;
                    Percent60.GradientMode = LinearGradientMode.Vertical;
                }
                e.Appearance.Combine(Percent60);
            }
            else if (currentTitle.PercentComplete <= .7M)
            {
                if (Percent70 == null)
                {
                    Percent70 = (AppearanceObject)e.Appearance.Clone();
                    Percent70.ForeColor = Color.Black;
                    Percent70.BackColor = Color.Silver;
                    Percent70.BackColor2 = Color.SkyBlue;
                    Percent70.GradientMode = LinearGradientMode.Vertical;
                }
                e.Appearance.Combine(Percent70);
            }
            else if (currentTitle.PercentComplete <= .8M)
            {
                if (Percent80 == null)
                {
                    Percent80 = (AppearanceObject)e.Appearance.Clone();
                    Percent80.ForeColor = Color.Black;
                    Percent80.BackColor = Color.SkyBlue;
                    Percent80.BackColor2 = Color.White;
                    Percent80.GradientMode = LinearGradientMode.Vertical;
                }
                e.Appearance.Combine(Percent80);
            }
        }*/

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
            }
            else
            {
/*                allMoviesToolStripMenuItem1.Checked = false;
                if (filterItem.OwnerItem == filterByGenreToolStripMenuItem)
                    _movieList = TitleCollectionManager.GetFilteredTitles(TitleFilterType.Genre, filterItem.Text).ToList<Title>();
                else if (filterItem.OwnerItem == filterByCompletenessToolStripMenuItem)
                    _movieList = TitleCollectionManager.GetTitlesByPercentComplete(decimal.Parse("." + filterItem.Text.TrimEnd('%'))).ToList<Title>();
                else if (filterItem.OwnerItem == filterByParentalRatingToolStripMenuItem)
                    _movieList = TitleCollectionManager.GetFilteredTitles(TitleFilterType.ParentalRating, filterItem.Text).ToList<Title>();
                else if (filterItem.OwnerItem == filterByTagToolStripMenuItem)
                    _movieList = TitleCollectionManager.GetFilteredTitles(TitleFilterType.Tag, filterItem.Text).ToList<Title>();
*/
                PopulateMovieListV2(0);
            }
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
            }
        }

        private void deleteSelectedMoviesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Title title in lbTitles.SelectedItems)
            {
                if (titleEditor.EditedTitle != null && titleEditor.EditedTitle.Id == title.Id)
                    titleEditor.ClearEditor();
                TitleCollectionManager.DeleteTitle(title);
                //_titleCollection.Remove(title);
            }
            //_titleCollection.saveTitleCollection();
            LoadMovies();
        }

        private void transcoderDiagnosticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mediafile = "";
            // Search for a movie file in selected title/titles
            foreach (Title title in lbTitles.SelectedItems)
            {
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

        #region TitleList functions
        private void lbTitles_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            Title currentTitle = ((Title)lbTitles.Items[e.Index]);

            // Find the printing bounds
            int x = e.Bounds.X;
            int y = e.Bounds.Y;
            int w = e.Bounds.Width;
            int h = e.Bounds.Height;
            
            // Setup string formatting
            StringFormat stf = new StringFormat();
            stf.Trimming = StringTrimming.EllipsisCharacter;
            stf.FormatFlags = StringFormatFlags.NoWrap;
            
            e.DrawBackground();


            if ((currentTitle.TitleType & TitleTypes.AllFolders) != 0)
            {
                // Folder specific paint goes here
                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), x, y, w, h);
                e.Graphics.DrawString(currentTitle.Name, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold), new SolidBrush(Color.Black), new RectangleF(x, y + 2, w - 65, h), stf);
            }
            else
            {
                // Media specific paint goes here
                e.Graphics.DrawString(currentTitle.Name, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(x, y + 2, w - 65, h), stf);
                e.Graphics.DrawString(currentTitle.ReleaseDate.ToShortDateString(), new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Black), new RectangleF(w - 60, y + 2, w, h), stf);
                e.Graphics.DrawString(currentTitle.Runtime.ToString() + " minutes, " + currentTitle.Studio, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular), new SolidBrush(Color.Gray), new RectangleF(8, y + 16, w - 40, h), stf);

                // Draw percentage complete box
                Color c1 = Color.Coral;
                Color c2 = Color.Crimson;
                if (currentTitle.PercentComplete <= .3M)
                {
                }
                else if (currentTitle.PercentComplete <= .4M)
                {
                    c1 = Color.CornflowerBlue;
                    c2 = Color.CadetBlue;
                }
                else if (currentTitle.PercentComplete <= .5M)
                {
                    c1 = Color.MediumSpringGreen;
                    c2 = Color.LightSeaGreen;
                }
                else if (currentTitle.PercentComplete <= .6M)
                {
                    c1 = Color.Yellow;
                    c2 = Color.Gold;
                }
                else if (currentTitle.PercentComplete <= .7M)
                {
                    c1 = Color.Silver;
                    c2 = Color.SkyBlue;
                }
                else if (currentTitle.PercentComplete <= .8M)
                {
                    c1 = Color.SkyBlue;
                    c2 = Color.White;
                }
                
                LinearGradientBrush bb = new LinearGradientBrush(new Rectangle(x + w - 30, y + 16, 14, 14), c1, c2, 2);

                e.Graphics.FillEllipse(bb, new Rectangle(x + w - 30, y + 16, 14, 14));
                e.Graphics.DrawEllipse(new Pen(Color.Black), new Rectangle(x + w - 30, y + 16, 14, 14));
            }

            // Common painting goes here
            e.Graphics.DrawLine(new Pen(Color.Gray), 0, y + h - 1, w, y + h - 1);


            e.DrawFocusRectangle();
        }

        private void lbTitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Cursor = Cursors.WaitCursor;
            if (SaveCurrentMovie() == DialogResult.Cancel)
            {
                _loading = true; //bypasses second save movie dialog
                lbTitles.SelectedItem = TitleCollectionManager.GetTitle(titleEditor.EditedTitle.Id);
                _loading = false;
            }
            else
            {
                Title selectedTitle = lbTitles.SelectedItem as Title;
                
                if (selectedTitle == null) return;

                titleEditor.LoadDVD(selectedTitle);
                this.Text = APP_TITLE + " - " + selectedTitle.Name;
                ToggleSaveState(false);
            }
            Cursor = Cursors.Default;
        }
        #endregion

        #region Media Tree handling functions
        private void treeMedia_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
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
                e.Graphics.DrawLine(new Pen(Color.Black), x -1 + wt, y + 1, x -1 + wt, y - 2 + h);

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
            lbTitles.Items.Clear();
            if (!string.IsNullOrEmpty(e.Node.Name))
            {
                if (treeMedia.SelectedNode.Name == "All Media")
                {
                    // Root Level selected
                    SelectedTreeRoot = null;
                    PopulateMovieListV2(SelectedTreeRoot);
                }
                else
                {
                    SelectedTreeRoot = Convert.ToInt32(e.Node.Name);
                    PopulateMovieListV2(SelectedTreeRoot);
                }
            }
            else
            {
                PopulateMovieListV2(null);
            }
        }

        private void miCreateFolder_Click(object sender, EventArgs e)
        {
            Title folder = new Title();

            //int selectedfolderid = 0;

            if (treeMedia.SelectedNode.Name == "All Media")
            {
                // Root Node
                folder.TitleType =  TitleTypes.Root | TitleTypes.Collection;
            }
            else
            {
                int parentid = Convert.ToInt32(treeMedia.SelectedNode.Name);
                folder.ParentTitleId = parentid;
                folder.TitleType = TitleTypes.Collection;
                //selectedfolderid = Convert.ToInt32(treeMedia.SelectedNode.Name);
            }
           
            folder.Name = "New Folder";
            TitleCollectionManager.AddTitle(folder);

            // Generate the parentid if required
            if (folder.ParentTitleId == 0)
            {
                folder.ParentTitleId = folder.Id;
                TitleCollectionManager.SaveTitleUpdates();
            }

            _movieList.Add(folder.Id, folder);

            PopulateMediaTree();

            if (_mediaTree.ContainsKey(folder.Id))
            {
                treeMedia.SelectedNode = _mediaTree[folder.Id];
            }
        }

        private void miDeleteFolder_Click(object sender, EventArgs e)
        {

        }

        private void miCreateTitle_Click(object sender, EventArgs e)
        {
            if (treeMedia.SelectedNode.Name == "All Media")
            {
                // Root Title
                CreateTitle(null);
            }
            else
            {
                int parentid = Convert.ToInt32(treeMedia.SelectedNode.Name);
                CreateTitle(parentid);
            }
            //LoadMovies();
            //_movieList.Add(titleEditor.EditedTitle.Id, titleEditor.EditedTitle);
            //PopulateMovieListV2(SelectedTreeRoot);
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
        #endregion

        private void CreateTitle(int? parentid)
        {
            Title newMovie = new Title();
            newMovie.Name = "New title";
            if (parentid == null)
            {
                newMovie.TitleType = TitleTypes.Root | TitleTypes.Unknown;
            }
            else
            {
                newMovie.TitleType = TitleTypes.Unknown;
                newMovie.ParentTitleId = (int)parentid;
            }
            newMovie.DateAdded = DateTime.Now;

            // Add the title now to get the title ID
            TitleCollectionManager.AddTitle(newMovie);

            // Generate the parentid if required
            if (newMovie.ParentTitleId == 0)
            {
                newMovie.ParentTitleId = newMovie.Id;
                TitleCollectionManager.SaveTitleUpdates();
            }

            // Add new title to title list and populate the screen control
            _movieList.Add(newMovie.Id, newMovie);
            PopulateMovieListV2(SelectedTreeRoot);

            titleEditor.LoadDVD(newMovie);
            ToggleSaveState(true);
        }


    }
}
