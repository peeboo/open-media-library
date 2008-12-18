using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraEditors;
using DevExpress.Skins;
using DevExpress.Skins.Info;

using OMLEngine;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class MainEditor : XtraForm
    {
        private static TitleCollection _titleCollection;
        private static List<OMLPlugin> _importPlugins = new List<OMLPlugin>();
        private static List<IOMLMetadataPlugin> _metadataPlugins = new List<IOMLMetadataPlugin>();
        private const string APP_TITLE = "OML Movie Manager";
        private bool _loading = false;
        public List<String> DXSkins;

        public MainEditor()
        {
            OMLEngine.Utilities.RawSetup();

            InitializeComponent();

            defaultLookAndFeel1.LookAndFeel.SkinName = Properties.Settings.Default.gsAppSkin;
            InitData();
        }

        private void InitData()
        {
            Cursor = Cursors.WaitCursor;
            OMLEngine.Utilities.DebugLine("[MainEditor] InitData() : new TitleCollection()");
            _titleCollection = new TitleCollection();
            OMLEngine.Utilities.DebugLine("[MainEditor] InitData() : loadTitleCollection()");
            _titleCollection.loadTitleCollection();
            SetupNewMovieAndContextMenu();
            GetDXSkins();

            _loading = true;
            LoadMovies();
            _loading = false;
            Cursor = Cursors.Default;
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
            if (mainNav.ActiveGroup != groupMovies) return;
            Cursor = Cursors.WaitCursor;
            lbMovies.Items.Clear();
            _titleCollection.SortBy("SortName", true);
            lbMovies.DataSource = _titleCollection.Source;
            if (titleEditor.EditedTitle != null)
                lbMovies.SelectedItem = _titleCollection.GetTitleById(titleEditor.EditedTitle.InternalItemID);
            else
            {
                lbMovies.SelectedIndex = -1;
                lbMovies.SelectedItem = null;
            }
            Cursor = Cursors.Default;
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

        private DialogResult SaveCurrentMovie()
        {
            DialogResult result;
            if (titleEditor.EditedTitle != null && titleEditor.Status == OMLDatabaseEditor.Controls.TitleEditor.TitleStatus.UnsavedChanges)
            {
                result = XtraMessageBox.Show("You have unsaved changes to " + titleEditor.EditedTitle.Name + ". Would you like to save your changes?", "Save Changes?", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    lbMovies.SelectedValue = titleEditor.EditedTitle.InternalItemID;
                }
                else if (result == DialogResult.Yes)
                {
                    SaveChanges();
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
                    if (_titleCollection.ContainsDisks(t.Disks))
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
                        OMLPlugin.BuildResizedMenuImage(t);
                        _titleCollection.Add(t);
                        numberOfTitlesAdded++;
                    }
                }

                _titleCollection.saveTitleCollection();

                XtraMessageBox.Show("Added " + numberOfTitlesAdded + " titles\nSkipped " + numberOfTitlesSkipped + " titles\n", "Import Results");

                plugin.GetTitles().Clear();
            }
            catch (Exception e)
            {
                XtraMessageBox.Show("Exception in LoadTitlesIntoDatabase: %1", e.Message);
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
            try
            {
                if (titleEditor.EditedTitle != null)
                {
                    Cursor = Cursors.WaitCursor;
                    plugin.SearchForMovie(titleEditor.EditedTitle.Name);
                    frmSearchResult searchResultForm = new frmSearchResult();
                    Cursor = Cursors.Default;
                    DialogResult result = searchResultForm.ShowResults(plugin.GetAvailableTitles());
                    if (result == DialogResult.OK)
                    {
                        Title t = plugin.GetTitle(searchResultForm.SelectedTitleIndex);
                        if (t != null)
                        {
                            if (coverArtOnly)
                            {
                                if (!String.IsNullOrEmpty(t.FrontCoverPath))
                                {
                                    titleEditor.EditedTitle.CopyFrontCoverFromFile(t.FrontCoverPath, true);
                                }
                            }
                            else
                            {
                                titleEditor.EditedTitle.CopyMetadata(t, searchResultForm.OverwriteMetadata);
                            }
                        }
                        titleEditor.RefreshEditor();
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch
            {
                Cursor = Cursors.Default;
                return false;
            }
        }

        private void SaveChanges()
        {
            if (titleEditor.Status == OMLDatabaseEditor.Controls.TitleEditor.TitleStatus.UnsavedChanges)
            {
                Title editedTitle = titleEditor.EditedTitle;
                Title collectionTitle = _titleCollection.GetTitleById(editedTitle.InternalItemID);
                if (collectionTitle == null)
                {
                    // Title doesn't exist so we'll add it
                    if (editedTitle.MetadataSourceID == String.Empty)
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
                    }
                    _titleCollection.Add(editedTitle);
                }
                else
                {
                    // Title exists so we need to copy edited data to collection title
                    _titleCollection.Replace(editedTitle);
                }
                _titleCollection.saveTitleCollection();
                titleEditor.ClearEditor();
                LoadMovies();
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

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveChanges();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (titleEditor.EditedTitle != null)
            {
                Title titleToRemove = _titleCollection.GetTitleById(titleEditor.EditedTitle.InternalItemID);
                if (titleToRemove != null)
                {
                    DialogResult result = XtraMessageBox.Show("Are you sure you want to delete " + titleToRemove.Name + "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        _titleCollection.Remove(titleToRemove);
                        _titleCollection.saveTitleCollection();
                        titleEditor.ClearEditor();
                        LoadMovies();
                    }
                    this.Text = APP_TITLE;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveCurrentMovie();
            Application.Exit();
        }

        private void MainEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = SaveCurrentMovie() == DialogResult.Cancel;
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
                _titleCollection = new TitleCollection();
                _titleCollection.saveTitleCollection();
                LoadMovies();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutOML about = new AboutOML();
            about.Show();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open settings form
        }

        private void fromScratchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Title newMovie = new Title();
            newMovie.Name = "New Movie";
            newMovie.DateAdded = DateTime.Now;
            titleEditor.LoadDVD(newMovie);
            ToggleSaveState(true);
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
            if (e.Group == groupMovies)
                LoadMovies();
            else if (e.Group == groupMetadata)
                LoadMetadata();
            else
                LoadImporters();
            _loading = false;
        }

        private void lbMovies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Cursor = Cursors.WaitCursor;
            SaveCurrentMovie();

            Title selectedTitle = lbMovies.SelectedItem as Title;
            if (selectedTitle == null) return;

            titleEditor.LoadDVD(selectedTitle);
            this.Text = APP_TITLE + " - " + selectedTitle.Name;
            ToggleSaveState(false);
            Cursor = Cursors.Default;
        }

        private void lbMetadata_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Cursor = Cursors.WaitCursor;
            IOMLMetadataPlugin metadata = lbMetadata.SelectedItem as IOMLMetadataPlugin;
            StartMetadataImport(metadata, false);
            Cursor = Cursors.Default;
        }

        private void lbImport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Cursor = Cursors.WaitCursor;
            OMLPlugin importer = lbImport.SelectedItem as OMLPlugin;
            importer.CopyImages = true;

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

            Cursor = Cursors.Default;
            this.Refresh();
            string[] nonFatalErrors = importer.GetErrors;
            if (nonFatalErrors.Length > 0)
                ShowNonFatalErrors(nonFatalErrors);
        }

        private void currentMovieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (titleEditor.EditedTitle != null)
                titleEditor.EditedTitle.BuildResizedMenuImage();
        }

        private void allMoviesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_titleCollection.Count > 0)
            {
                foreach (Title t in _titleCollection)
                    t.BuildResizedMenuImage();
                _titleCollection.saveTitleCollection();
            }
        }

        private void miMetadataMulti_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            SaveCurrentMovie();
            ToolStripMenuItem selectedItem = sender as ToolStripMenuItem;
            IOMLMetadataPlugin plugin = selectedItem.Tag as IOMLMetadataPlugin;

            //BaseListBoxControl.SelectedItemCollection collection = lbMovies.SelectedItems;
            foreach (Title title in lbMovies.SelectedItems)
            {
                titleEditor.LoadDVD(title);
                this.Text = APP_TITLE + " - " + title.Name;
                ToggleSaveState(false);

                if (StartMetadataImport(plugin, false))
                {
                    _titleCollection.Replace(titleEditor.EditedTitle);
                    _titleCollection.saveTitleCollection();
                }
            }
        }

        private void exportCurrentMovieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (titleEditor.EditedTitle != null)
            {
                Title _currentTitle = (Title)_titleCollection.MoviesByItemId[titleEditor.EditedTitle.InternalItemID];
                if (_currentTitle.Disks.Count > 0)
                    _currentTitle.SerializeToXMLFile(_currentTitle.Disks[0].Path + ".oml.xml");
            }
        }

        private void exportAllMoviesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor; 
            foreach (Title t in _titleCollection) 
                if (t.Disks.Count > 0) 
                    t.SerializeToXMLFile(t.Disks[0].Path + ".oml.xml"); 
            Cursor = Cursors.Default; 
        }
    }
}
