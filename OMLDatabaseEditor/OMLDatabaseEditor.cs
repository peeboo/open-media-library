using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OMLEngine;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class OMLDatabaseEditor : Form
    {
        private static TitleCollection _titleCollection = new TitleCollection();
        private static List<OMLPlugin> _importPlugins = new List<OMLPlugin>();
        private static List<IOMLMetadataPlugin> _metadataPlugins = new List<IOMLMetadataPlugin>();

        public OMLDatabaseEditor()
        {
            OMLEngine.Utilities.RawSetup();

            InitializeComponent();
            InitData();      
            LoadImportPlugins( PluginTypes.ImportPlugin, _importPlugins);
            LoadMetadataPlugins(PluginTypes.MetadataPlugin, _metadataPlugins);
            SetupPluginList();
        }

        private void InitData()
        {
            Cursor = Cursors.WaitCursor;
            _titleCollection = new TitleCollection();
            _titleCollection.loadTitleCollection();
            _titleCollection.Sort();

            SetupTitleList();
            Cursor = Cursors.Default;
        }


        private static void LoadImportPlugins(string pluginType, List<OMLPlugin> pluginList)
        {
            pluginList.Clear();

            List<PluginServices.AvailablePlugin> plugins = new List<PluginServices.AvailablePlugin>();
            string path = FileSystemWalker.PluginsDirectory;
            plugins = PluginServices.FindPlugins(path, pluginType);
            OMLPlugin objPlugin;
            // Loop through available plugins, creating instances and adding them
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
            // Loop through available plugins, creating instances and adding them
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


        private void SetupTitleList()
        {
            foreach (Title t in _titleCollection)
            {
                tvSourceList_AddItem(t.Name, t.InternalItemID.ToString(), "Movies");
            }
        }

        public void HandleImportMenuItem(Object sender, EventArgs e)
        {
            ToolStripDropDownItem item = sender as ToolStripDropDownItem;
            StartImport(Convert.ToInt32(item.Name));            
        }

        public void HandleMetadataMenuItem(Object sender, EventArgs e)
        {
            ToolStripDropDownItem item = sender as ToolStripDropDownItem;
            StartMetadataImport(Convert.ToInt32(item.Name), false);
        }

        public void HandleMetadataCoverArtMenuItem(Object sender, EventArgs e)
        {
            ToolStripDropDownItem item = sender as ToolStripDropDownItem;
            StartMetadataImport(Convert.ToInt32(item.Name), true);
        }

        void StartMetadataImport(int pluginId, bool coverArtOnly)
        {
            try
            {
                Controls.MediaEditor e = GetCurrentEditor();
                if (e != null)
                {
                    Cursor = Cursors.WaitCursor;
                    _metadataPlugins[pluginId].SearchForMovie(e.TitleName);
                    frmSearchResult searchResultForm = new frmSearchResult();
                    Cursor = Cursors.Default;
                    DialogResult result = searchResultForm.ShowResults(_metadataPlugins[pluginId].GetAvailableTitles());
                    if (result == DialogResult.OK)
                    {
                        Title t = searchResultForm.SelectedTitle;
                        if (t != null)
                        {
                            if (coverArtOnly)
                            {
                                if (!String.IsNullOrEmpty(t.FrontCoverPath))
                                {
                                    e.CurrentTitle.CopyFrontCoverFromFile( t.FrontCoverPath, true );
                                    e.LoadTitle(e.CurrentTitle);
                                }

                            }
                            else
                            {
                                e.CurrentTitle.CopyMetadata(t, searchResultForm.OverwriteMetadata);
                                e.LoadTitle(e.CurrentTitle);
                            }
                        }
                    }
                }
            }
            catch
            {
                Cursor = Cursors.Default;
            }
        }

        private void SetupPluginList()
        {
            int i = 0;
            foreach (OMLPlugin plugin in _importPlugins)
            {
                //tvSourceList_AddItem(plugin.Menu, i.ToString(), "Importers");
                ToolStripMenuItem item = new ToolStripMenuItem(plugin.Menu, null, HandleImportMenuItem, i.ToString());
                importToolStripMenuItem.DropDownItems.Add(item);
                i++;
            }

            i = 0;
            foreach (IOMLMetadataPlugin metadataPlugin in _metadataPlugins)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(metadataPlugin.PluginName, null, HandleMetadataMenuItem, i.ToString());
                metadataToolStripMenuItem.DropDownItems.Insert(i, item);

                item = new ToolStripMenuItem(metadataPlugin.PluginName, null, HandleMetadataCoverArtMenuItem, i.ToString());
                coverArtOnlyToolStripMenuItem.DropDownItems.Insert(i, item);
                i++;
            }
        }

        private void tvSourceList_AddItem(string text, string name, string tag)
        {
            TreeNode node = new TreeNode();
            node.Name = name;
            node.Text = text;
            node.Tag = tag;
            tvSourceList.Nodes["OML Database"].Nodes[tag].Nodes.Add(node);
            tvSourceList.Nodes["OML Database"].ExpandAll();
            tvSourceList.Nodes["OML Database"].Nodes[tag].ExpandAll();
        }

        private void SaveTitleChangesToDB(Title t)
        {
            _titleCollection.Replace(t);
            _titleCollection.saveTitleCollection();

            reloadDatabase();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabsMediaPanel.SelectedTab != null)
            {
                Controls.MediaEditor _currentEditor = (Controls.MediaEditor)this.tabsMediaPanel.SelectedTab.Controls[0].Controls[0];
                Title _currentTitle = (Title)_titleCollection.MoviesByItemId[_currentEditor.itemID];

                _currentEditor.SaveToTitle(_currentTitle);
                SaveTitleChangesToDB(_currentTitle);
            }
        }

        private void SaveAll()
        {
            foreach (TabPage page in tabsMediaPanel.TabPages)
            {
                Controls.MediaEditor editor = (Controls.MediaEditor)page.Tag;
                if (editor.Status ==  global::OMLDatabaseEditor.Controls.MediaEditor.TitleStatus.UnsavedChanges)
                {
                    Title _currentTitle = (Title)_titleCollection.MoviesByItemId[editor.itemID];

                    editor.SaveToTitle(_currentTitle);
                    _titleCollection.Replace(_currentTitle);
                }
            }
            _titleCollection.saveTitleCollection();

        }

        private void tsbNewTitle_Click(object sender, EventArgs e)
        {
            Title t = new Title();
            t.Name = "New Movie";
            _titleCollection.Add(t);

            tvSourceList_AddItem("New Movie", t.InternalItemID.ToString(), "Movies");
            EditNewTab(t.InternalItemID);
        }

        
        private void tsbClose_Click(object sender, EventArgs e)
        {
            if (tabsMediaPanel.SelectedTab != null && tabsMediaPanel.SelectedTab.Controls != null && tabsMediaPanel.SelectedTab.Controls[0].Controls != null)
            {
                Controls.MediaEditor _currentEditor = (Controls.MediaEditor)tabsMediaPanel.SelectedTab.Controls[0].Controls[0];

                if (_currentEditor == null) return;

                bool _bClose = true;

                if (_currentEditor.Status == global::OMLDatabaseEditor.Controls.MediaEditor.TitleStatus.UnsavedChanges)
                {
                    DialogResult result = MessageBox.Show("Do you want to save the changes to the current movie?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Cancel)
                    {
                        _bClose = false;
                    }
                    else if (result == DialogResult.Yes)
                    {
                        Title _currentTitle = (Title)_titleCollection.MoviesByItemId[_currentEditor.itemID];

                        _currentEditor.SaveToTitle(_currentTitle);
                        SaveTitleChangesToDB(_currentTitle);
                    }
                    else
                    {
                    }
                }

                if (_bClose)
                {
                    TabPage _currentTab = tabsMediaPanel.SelectedTab;
                    tabsMediaPanel.TabPages.Remove(_currentTab);
                }
            }
        }

        private void SelectMovieOrImporter()
        {
            if (tvSourceList.SelectedNode.Tag != null)
            {
                if (tvSourceList.SelectedNode.Tag.ToString() == "Movies")
                {
                    int itemId = int.Parse(tvSourceList.SelectedNode.Name);
                    TabPage existingPage = GetPageForTitle(itemId);
                    if (existingPage != null)
                    {
                        tabsMediaPanel.SelectTab(existingPage);
                    }
                    else
                    {
                        EditNewTab(itemId);
                    }
                }
                else if (tvSourceList.SelectedNode.Tag.ToString() == "Importers")
                {
                    int pluginID = int.Parse(tvSourceList.SelectedNode.Name);

                    StartImport(pluginID);
                }
            }
        }

        private void MenuItemEditTab_Click(object sender, EventArgs e)
        {
            SelectMovieOrImporter();
        }

        private void StartImport(int pluginID)
        {
            
            Cursor = Cursors.WaitCursor;
            this.Refresh();

            OMLPlugin plugin = new OMLPlugin();
            plugin = _importPlugins[pluginID];
            //plugin.FileFound += new OMLPlugin.FileFoundEventHandler(FileFound);
            //if (plugin.CanCopyImages) AskIfShouldCopyImages();
            plugin.CopyImages = true;// Program._copyImages;

            string[] work = plugin.GetWork();
            if (work != null)
            {
                plugin.DoWork(work);
                LoadTitlesIntoDatabase(plugin);
            }

            Cursor = Cursors.Default;
            this.Refresh();
        }

        public void LoadTitlesIntoDatabase(OMLPlugin plugin)
        {
            try
            {
                Utilities.DebugLine("[OMLImporter] Titles loaded, beginning Import process");
                //TitleCollection tc = new TitleCollection();
                IList<Title> titles = plugin.GetTitles();
                Utilities.DebugLine("[OMLImporter] " + titles.Count + " titles found in input file");
                //Console.WriteLine("Found " + titles.Count + " titles");

                int numberOfTitlesAdded = 0;
                int numberOfTitlesSkipped = 0;
                bool YesToAll = true;// false;

                //if (Console.In.Peek() > 0)
                //    Console.In.ReadToEnd(); // flush out anything still in there

                foreach (Title t in titles)
                {
                    if (_titleCollection.ContainsDisks(t.Disks))
                    {
                        //Console.WriteLine("Title {0} skipped because already in the collection", t.Name);
                        numberOfTitlesSkipped++;
                        continue;
                    }


                    //Console.WriteLine("Adding: " + t.Name);
                    if (YesToAll == false)
                    {
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
                        tvSourceList_AddItem(t.Name, t.InternalItemID.ToString(), "Movies");
                    }
                }

                _titleCollection.saveTitleCollection();

                MessageBox.Show("Added " + numberOfTitlesAdded + " titles\nSkipped " + numberOfTitlesSkipped + " titles\n", "Import Results");

                plugin.GetTitles().Clear();

                /*if (numberOfTitlesAdded > 0) isDirty = true;
                Console.WriteLine();
                Console.WriteLine("Added " + numberOfTitlesAdded + " titles");
                Console.WriteLine("Skipped " + numberOfTitlesSkipped + " titles");*/
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception in LoadTitlesIntoDatabase: %1", e.Message);
                //Console.WriteLine("Exception in LoadTitlesIntoDatabase: %1", e.Message);
            }
            //tc.saveTitleCollection();
            //Console.WriteLine("Complete");
        }

        private void EditNewTab(int itemID)
        {
            Controls.MediaEditor Editor = new Controls.MediaEditor();

            Editor.AutoScroll = true;
            Editor.AutoSize = true;
            Editor.BackColor = System.Drawing.SystemColors.Control;
            Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            Editor.Location = new System.Drawing.Point(0, 0);
            Editor.Name = "ME" + itemID.ToString();

            Title currentTitle = new Title();
            currentTitle = (Title)_titleCollection.MoviesByItemId[itemID];

            System.Windows.Forms.Panel p = new Panel();

            p.AutoScroll = true;
            p.AllowDrop = true;
            p.Size = new System.Drawing.Size(680, 772);
            TabPage newpage = new TabPage(currentTitle.Name);
            newpage.AutoScroll = true;
            p.Controls.Add(Editor);

            newpage.Controls.Add(p);
            newpage.Tag = Editor;

            tabsMediaPanel.TabPages.Add(newpage);
            
            Editor.LoadTitle(currentTitle);
            Editor.TitleChanged += new Controls.MediaEditor.TitleChangeEventHandler(this.TitleChanges);
            Editor.TitleNameChanged += new Controls.MediaEditor.TitleNameChangeEventHandler(this.TitleNameChanges);
            Editor.SavedTitle += new Controls.MediaEditor.SavedEventHandler(this.SavedTitle);

            tabsMediaPanel.SelectTab(newpage);
        }

        private void tvSourceList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = tvSourceList.GetNodeAt(p);
                if (node != null)
                {

                    // Select the node the user has clicked.
                    tvSourceList.SelectedNode = node;

                    // Find the appropriate ContextMenu depending on the selected node.
                    switch (Convert.ToString(node.Tag))
                    {
                        case "Movies":
                            MenuStripTitle.Show(tvSourceList, p);
                            break;
                    }
                }
            }
        }

        private void MarkChangedItem(Controls.MediaEditor editor, bool changed)
        {
            foreach (TabPage page in tabsMediaPanel.TabPages)
            {
                if (page.Tag == editor)
                {
                    if (changed)
                        page.Text = "*" + editor.TitleName;
                    else
                        page.Text = editor.TitleName;
                    break;
                }
            }
        }

        private TabPage GetPageForTitle(int itemId)
        {
            Controls.MediaEditor editor = null;
            foreach (TabPage page in tabsMediaPanel.TabPages)
            {
                editor = (Controls.MediaEditor)page.Tag;
                if (editor.itemID == itemId)
                {
                    return page;
                }
            }

            return null;
        }


        private void TitleChanges(object sender, EventArgs e)
        {
            Controls.MediaEditor _currentEditor = (Controls.MediaEditor)sender;
            MarkChangedItem(_currentEditor, true);
        }

        private void TitleNameChanges(object sender, EventArgs e)
        {
            Controls.MediaEditor _currentEditor = (Controls.MediaEditor)sender;
            MarkChangedItem(_currentEditor, true);
        }

        private void SavedTitle(object sender, EventArgs e)
        {
            Controls.MediaEditor _currentEditor = (Controls.MediaEditor)sender;
            MarkChangedItem(_currentEditor, false);
        }

        void DeleteMovie()
        {
            if (tvSourceList.SelectedNode.Tag != null)
            {
                if (tvSourceList.SelectedNode.Tag.ToString() == "Movies")
                {
                    Title titleToRemove = _titleCollection.MoviesByItemId[int.Parse(tvSourceList.SelectedNode.Name)];
                    DialogResult result = MessageBox.Show("Are you sure you want to delete " + titleToRemove.Name + "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        tvSourceList.Nodes.Remove(tvSourceList.SelectedNode);
                        _titleCollection.Remove(titleToRemove);
                        _titleCollection.saveTitleCollection();
                    }
                }
            }

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteMovie();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        private void tsbSaveAll_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

        public static DialogResult GetFile(ref string file_to_import, OMLPlugin plugin)
        {
            OpenFileDialog ofDiag = new OpenFileDialog();
            ofDiag.InitialDirectory = "c:\\";
            ofDiag.Filter = "Xml files (*.xml)|*.xml|DVR-MS Files (*.dvr-ms)|*.dvr-ms|All files (*.*)|*.*";
            ofDiag.FilterIndex = 1;
            ofDiag.RestoreDirectory = true;
            ofDiag.AutoUpgradeEnabled = true;
            ofDiag.CheckFileExists = true;
            ofDiag.CheckPathExists = true;
            ofDiag.Multiselect = false;
            ofDiag.Title = "Select " + plugin.Name + " file to import";
            DialogResult dlgRslt = ofDiag.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                Utilities.DebugLine("[OMLImporter] Valid file found (" + ofDiag.FileName + ")");
                file_to_import = ofDiag.FileName;
            }
            return dlgRslt;
        }

        private void tvSourceList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                SelectMovieOrImporter();
            else if (e.KeyData == Keys.Delete)
                DeleteMovie();
        }

        private void reloadDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reloadDatabase();
        }

        private void reloadDatabase()
        {
            tvSourceList.Nodes["OML Database"].Nodes["Movies"].Nodes.Clear();
            //tvSourceList.Nodes["OML Database"].Nodes["Importers"].Nodes.Clear();
            InitData();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutOML about = new AboutOML();
            about.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForChanges();
            this.Close();
        }

        Controls.MediaEditor GetCurrentEditor()
        {
            if (tabsMediaPanel.SelectedTab != null)
            {
                return (Controls.MediaEditor)this.tabsMediaPanel.SelectedTab.Controls[0].Controls[0];
            }
            else
            {
                return null;
            }
        }

        private void saveToXMLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.tabsMediaPanel.SelectedTab != null)
            {
                Controls.MediaEditor _currentEditor = (Controls.MediaEditor)this.tabsMediaPanel.SelectedTab.Controls[0].Controls[0];
                Title _currentTitle = (Title)_titleCollection.MoviesByItemId[_currentEditor.itemID];

                if (_currentTitle.Disks.Count > 0)
                {
                    Disk firstDisc = _currentTitle.Disks[0];
                    string fileName = firstDisc.Path + ".oml.xml";
                    _currentTitle.SerializeToXMLFile(fileName);
                }
                else
                {
                    // prompt where to save the movie information
                }

                //_currentEditor.SaveToTitle(_currentTitle);
                //SaveTitleChangesToDB(_currentTitle);
            }

        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofDiag = new OpenFileDialog();
            ofDiag.InitialDirectory = "c:\\";
            ofDiag.Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*";
            ofDiag.FilterIndex = 1;
            ofDiag.RestoreDirectory = true;
            ofDiag.AutoUpgradeEnabled = true;
            ofDiag.CheckFileExists = true;
            ofDiag.CheckPathExists = true;
            ofDiag.Multiselect = false;
            ofDiag.Title = "Select file to import";
            DialogResult dlgRslt = ofDiag.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                Title t = Title.CreateFromXML(ofDiag.FileName);
                bool bAdded = false;
                if (_titleCollection.ContainsDisks(t.Disks))
                {
                    if (MessageBox.Show("This movie is already on the list!\n\n Do you want to overwrite it?", "Confirm" ,MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes )
                    {
                        Title oldTitle = _titleCollection.FindByDisks(t.Disks);
                        tvSourceList.Nodes["OML Database"].Nodes["Movies"].Nodes.RemoveByKey(oldTitle.InternalItemID.ToString());
                        _titleCollection.Replace(t, oldTitle);
                        tvSourceList_AddItem(t.Name, t.InternalItemID.ToString(), "Movies");
                        bAdded = true;
                    }
                }
                else
                {
                    _titleCollection.Add(t);
                    tvSourceList_AddItem(t.Name, t.InternalItemID.ToString(), "Movies");
                 
                    bAdded = true;
                }
                //Cursor = Cursors.WaitCursor;
                //_titleCollection.saveTitleCollection();
                //Cursor = Cursors.Default;

                if (bAdded)
                {
                    EditNewTab(t.InternalItemID);
                    Controls.MediaEditor _currentEditor = (Controls.MediaEditor)this.tabsMediaPanel.SelectedTab.Controls[0].Controls[0];
                    _currentEditor.Status = global::OMLDatabaseEditor.Controls.MediaEditor.TitleStatus.UnsavedChanges;
                    MarkChangedItem(_currentEditor, true);
                }
            }
        }


        private void OMLDatabaseEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckForChanges();
        }

        private void CheckForChanges()
        {
            bool acceptedSaved = false;
            
            foreach (TabPage page in tabsMediaPanel.TabPages)
            {
                Controls.MediaEditor editor = (Controls.MediaEditor)page.Tag;
                if (editor.Status ==  global::OMLDatabaseEditor.Controls.MediaEditor.TitleStatus.UnsavedChanges)
                {
                    // First run through if they accept the save this will only show once, if they deny then finish exit
                    if (acceptedSaved == false)
                    {
                        if (MessageBox.Show("Would you like to save your changes?"
                            , "Unsaved Changes"
                            ,MessageBoxButtons.YesNo
                            ,MessageBoxIcon.Question
                            ,MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            acceptedSaved = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                    Title _currentTitle = (Title)_titleCollection.MoviesByItemId[editor.itemID];

                    editor.SaveToTitle(_currentTitle);
                    _titleCollection.Replace(_currentTitle);
                }
            }

            if (acceptedSaved == true)
            {
                _titleCollection.saveTitleCollection();
            }
        }

        private void tvSourceList_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            foreach (Title t in _titleCollection)
            {
                if (t.Disks.Count > 0)
                {
                    t.SerializeToXMLFile(t.Disks[0].Path + ".oml.xml");
                }
            }
            Cursor = Cursors.Default;
        }

        private void clearDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete all the movies?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _titleCollection = new TitleCollection();
                _titleCollection.saveTitleCollection();
                reloadDatabase();
            }
        }

        private void btnAmazon_Click(object sender, EventArgs e)
        {
            if (this.tabsMediaPanel.SelectedTab != null)
            {
                Controls.MediaEditor _currentEditor = (Controls.MediaEditor)this.tabsMediaPanel.SelectedTab.Controls[0].Controls[0];
                Title _currentTitle = (Title)_titleCollection.MoviesByItemId[_currentEditor.itemID];


            }

        }

        private void tabsMediaPanel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MetaDataSettings settings = new MetaDataSettings();
            settings.Show(_metadataPlugins);
        }

        private void metadataToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        //}

        ////private Control GetControl()
        ////{
        ////    ContainerControl cControl = ContainerControl;
        ////    Control aControl = Control;

        ////    aControl = this.ActiveControl;
        ////    if (aControl.GetType() == TextBoxBase)
        ////    {
        ////        return aControl;
        ////    }
        ////    else
        ////    {
        ////        return null;
        ////    }
        ////}
    }
}


