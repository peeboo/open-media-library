using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

using OMLEngine;
using OMLEngine.Settings;
using OMLEngine.FileSystem;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class Options : DevExpress.XtraEditors.XtraForm
    {
        public Options()
        {
            InitializeComponent();
        }

        public Boolean OptionsDirty = false;
        private Boolean MPAAdirty = false;
        private Boolean TagsDirty = false;
        private List<String> MPAAList;
        private List<String> TagList;

        private void Options_Load(object sender, EventArgs e)
        {
            int genreCount = 0;
            this.lbcSkins.DataSource = ((MainEditor)this.Owner).DXSkins;
            String skin = OMLEngine.Settings.OMLSettings.DBEditorSkin;
            int idx = this.lbcSkins.FindItem(skin);
            if (idx < 0)
            {
                skin = ((MainEditor)this.Owner).LookAndFeel.SkinName;
                idx = this.lbcSkins.FindItem(skin);
            }
            this.lbcSkins.SetSelected(idx, true);
            this.ceUseMPAAList.Checked = OMLSettings.UseMPAAList;
            MPAAList = new List<String>();
            MPAAList.AddRange(OMLSettings.MPAARatings.Split('|'));
            MPAAList.Sort();
            this.lbcMPAA.DataSource = MPAAList;

            TagList = new List<string>();
            if (String.IsNullOrEmpty(OMLSettings.Tags.Trim()))
            {
                //TagList.AddRange(MainEditor._titleCollection.GetAllTags); NEEDS_TO_BE_RESOLVED
            }
            else
            {
                // TagList.AddRange(MainEditor._titleCollection.GetAllTags.Union(Properties.Settings.Default.gsTags.Split('|'))); NEEDS_TO_BE_RESOLVED
            }
            int iTags = OMLSettings.Tags.Split('|').Count();
            if (iTags < TagList.Count()) TagsDirty = true;
            TagList.Sort();
            lbcTags.DataSource = TagList;

            this.ceUseGenreList.Checked = OMLSettings.UseGenreList;
            
            /*
                if (XtraMessageBox.Show("No allowable genres have been defined. Would you like to load them from your current movie collection?", "No Genres", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Properties.Settings.Default.gsValidGenres = new StringCollection();

                    IEnumerable<FilteredCollection> genres = TitleCollectionManager.GetAllGenres(new List<TitleFilter>());
                    string[] genreNames = new string[genres.Count()];

                    int index = 0;
                    foreach (FilteredCollection genre in genres)
                        genreNames[index++] = genre.Name;

                    Properties.Settings.Default.gsValidGenres.AddRange(genreNames);
                }
            }*/
            // I disabled this line because gsValidGenres is not just empty, its undef

            this.ceFoldersAsTitles.Checked = OMLSettings.TreatFoldersAsTitles;
            this.cePrependParentFolder.Checked = OMLSettings.AddParentFoldersToTitleName;
            cePrependParentFolder.Enabled = this.ceFoldersAsTitles.Checked;

            foreach (MetaDataPluginDescriptor plugin in MainEditor._metadataPlugins)
            {
                cmbDefaultMetadataPlugin.Properties.Items.Add(plugin.DataProviderName);
                cmbDefaultMetadataPluginTV.Properties.Items.Add(plugin.DataProviderName);
            }
            cmbDefaultMetadataPlugin.SelectedItem = OMLEngine.Settings.OMLSettings.DefaultMetadataPluginMovies;
            cmbDefaultMetadataPluginTV.SelectedItem = OMLEngine.Settings.OMLSettings.DefaultMetadataPluginTV;
            ceUpdateMissingDataOnlyMovies.Checked = !OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataPrefMovies;
            ceUpdateTitleNameMovies.Checked = OMLEngine.Settings.OMLSettings.MetadataLookupUpdateNamePrefMovies;
            ceUpdateMissingDataOnlyTV.Checked = !OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataPrefTV;
            ceUpdateTitleNameTV.Checked = OMLEngine.Settings.OMLSettings.MetadataLookupUpdateNamePrefTV;

            seMetadataLookupResultsQty.Value = OMLEngine.Settings.OMLSettings.MetadataLookupResultsQty;

            ceTitledFanArtFolder.Checked = OMLSettings.TitledFanArtFolder;
            beTitledFanArtPath.EditValue = OMLSettings.TitledFanArtPath;
            if (string.IsNullOrEmpty(beTitledFanArtPath.EditValue.ToString())) beTitledFanArtPath.EditValue = OMLEngine.FileSystemWalker.FanArtDirectory;

            beTitledFanArtPath.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            beTitledFanArtPath.MaskBox.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;

            ceStSanaCreateTLFolder.Checked = OMLSettings.StSanaCreateTLFolder;
            ceStSanaAlwaysCreateMovieFolder.Checked = OMLSettings.StSanaAlwaysCreateMovieFolder;
            ceDBEStSanaAutoLookupMeta.Checked = OMLEngine.Settings.OMLSettings.StSanaAutoLookupMeta;

            // Mounting Tools
            foreach (string toolName in Enum.GetNames(typeof(OMLEngine.FileSystem.MountingTool.Tool)))
            {
                RadioGroupItem rg = new RadioGroupItem(toolName, toolName);
                rgMountingTool.Properties.Items.Add(rg);
            }

            
            rgMountingTool.EditValue = OMLSettings.MountingToolSelection.ToString();
            cmbMntToolVDrive.Text = OMLSettings.VirtualDiscDrive;
            teMntToolPath.Text = OMLSettings.MountingToolPath;
        }

        private void SimpleButtonClick(object sender, EventArgs e)
        {
            if (sender == this.sbOK)
            {
                Boolean bDirty = false;
                String skin = (String)this.lbcSkins.SelectedValue;
                if (skin != OMLSettings.DBEditorSkin)
                {
                    bDirty = true;
                    OMLSettings.DBEditorSkin = skin;
                }
                if (OMLSettings.UseMPAAList != this.ceUseMPAAList.Checked)
                {
                    bDirty = true;
                    OMLSettings.UseMPAAList = this.ceUseMPAAList.Checked;
                }
                if (MPAAdirty)
                {
                    bDirty = true;
                    String MPAAs = String.Join("|", MPAAList.ToArray());
                    OMLEngine.Settings.OMLSettings.MPAARatings = MPAAs;
                }
                if (OMLSettings.UseGenreList != this.ceUseGenreList.Checked)
                {
                    bDirty = true;
                    OMLSettings.UseGenreList = this.ceUseGenreList.Checked;
                }
                if (TagsDirty)
                {
                    bDirty = true;
                    String Tags = String.Join("|", TagList.ToArray());
                    OMLSettings.Tags = Tags;
                }

                // TODO - needs tidying, do not save on evert OMLEngine change, might need to add another dirty flag
                if (OMLSettings.TitledFanArtFolder != this.ceTitledFanArtFolder.Checked)
                {
                    bDirty = true;
                    OMLSettings.TitledFanArtFolder = this.ceTitledFanArtFolder.Checked;
                }

                if (OMLSettings.TitledFanArtFolder &&
                    OMLSettings.TitledFanArtPath != (string)beTitledFanArtPath.EditValue)
                {
                    bDirty = true;
                    OMLSettings.TitledFanArtPath = (string)this.beTitledFanArtPath.EditValue;
                }

                bDirty = true;

                // Preferred metadata lookup
                OMLEngine.Settings.OMLSettings.DefaultMetadataPluginMovies = (string)cmbDefaultMetadataPlugin.SelectedItem;
                OMLEngine.Settings.OMLSettings.DefaultMetadataPluginTV = (string)cmbDefaultMetadataPluginTV.SelectedItem;
                OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataPrefMovies = !ceUpdateMissingDataOnlyMovies.Checked;
                OMLEngine.Settings.OMLSettings.MetadataLookupUpdateNamePrefMovies = ceUpdateTitleNameMovies.Checked;
                OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingDataPrefTV = !ceUpdateMissingDataOnlyTV.Checked;
                OMLEngine.Settings.OMLSettings.MetadataLookupUpdateNamePrefTV = ceUpdateTitleNameTV.Checked;

                OMLEngine.Settings.OMLSettings.MetadataLookupResultsQty = (int)seMetadataLookupResultsQty.Value;
                
                OMLSettings.StSanaCreateTLFolder = ceStSanaCreateTLFolder.Checked;
                OMLSettings.StSanaAlwaysCreateMovieFolder = ceStSanaAlwaysCreateMovieFolder.Checked;
                OMLEngine.Settings.OMLSettings.StSanaAutoLookupMeta = ceDBEStSanaAutoLookupMeta.Checked;
                
                if (bDirty)
                {
                    OptionsDirty = bDirty;
                    Properties.Settings.Default.Save();
                }
                if (OMLSettings.TreatFoldersAsTitles != this.ceFoldersAsTitles.Checked)
                {
                    OMLSettings.TreatFoldersAsTitles = this.ceFoldersAsTitles.Checked;
                }
                if (OMLSettings.AddParentFoldersToTitleName != this.cePrependParentFolder.Checked)
                {
                    OMLSettings.AddParentFoldersToTitleName = this.cePrependParentFolder.Checked;
                }

                MountingTool.Tool tool = (MountingTool.Tool)Enum.Parse(typeof(MountingTool.Tool), rgMountingTool.Text);
                OMLSettings.MountingToolSelection = tool;
                OMLSettings.VirtualDiscDrive = cmbMntToolVDrive.Text;
                OMLSettings.MountingToolPath = teMntToolPath.Text;

            }
            else if (sender == this.sbCancel)
            {
                String skin = OMLEngine.Settings.OMLSettings.DBEditorSkin;
                ((MainEditor)this.Owner).defaultLookAndFeel1.LookAndFeel.SkinName = skin;
            }
            this.Close();
        }

        private void lbcSkins_SelectedValueChanged(object sender, EventArgs e)
        {
            String skin = (String)this.lbcSkins.SelectedValue;
            ((MainEditor)this.Owner).defaultLookAndFeel1.LookAndFeel.SkinName = skin;
        }

        private void beOptions_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind != ButtonPredefines.Plus) return;

            if (sender == beMPAA)
            {
                if (String.IsNullOrEmpty((String)beMPAA.Text)) return;
                if (MPAAList.Contains(beMPAA.Text)) return;

                MPAAList.Add((String)beMPAA.Text);
                MPAAdirty = true;
                lbcMPAA.Refresh();
                beMPAA.Text = String.Empty;
            }
            else if (sender == beTags)
            {
                if (String.IsNullOrEmpty((String)beTags.Text)) return;
                if (TagList.Contains(beTags.Text)) return;

                TagList.Add((String)beTags.Text);
                TagsDirty = true;
                lbcTags.Refresh();
                beTags.Text = String.Empty;
            }
        }

        private void lbcMPAA_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete) return;
            if (((ListBoxControl)sender).SelectedItems.Count <= 0) return;

            foreach (object item in ((ListBoxControl)sender).SelectedItems)
            {
                String MPAA = item as String;
                MPAAList.Remove(MPAA);
                MPAAdirty = true;
            }
            ((ListBoxControl)sender).Refresh();
        }

/*        private void lbGenres_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lbGenres.SelectedItems.Count > 0)
            {
                foreach (object item in lbGenres.SelectedItems)
                {
                    string genre = item as string;
                    IEnumerable<Title> titles = TitleCollectionManager.GetFilteredTitles(TitleFilterType.Genre, genre);

                    int titleCount = titles.Count();

                    if (titleCount > 0)
                    {
                        StringBuilder message = new StringBuilder(titleCount + " movie(s) in your collection are associated with the " + genre + " genre. Would you like to remove the association?\r\n\r\n");
                        foreach (Title title in titles)
                            message.Append(title.Name + "\r\n");

                        if (XtraMessageBox.Show(message.ToString(), "Remove Genre", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            foreach (Title title in TitleCollectionManager.GetFilteredTitles(TitleFilterType.Genre, genre))
                            {
                                title.Genres.Remove(genre);
                            }
                        }
                    }
                    GenreList.Remove(genre);
                    GenreDirty = true;
                    //Properties.Settings.Default.gsValidGenres.Remove(genre);
                }
                lbGenres.Refresh();
            }
        }*/


        void item_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (XtraMessageBox.Show(String.Format("Are you sure you want to remove the mapping of {0} to {1}?", item.Text, item.Tag), "Remove Mapping", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // TODO : Create SQL Version
                /*MainEditor._titleCollection.GenreMap.Remove(item.Text);
                MainEditor._titleCollection.saveTitleCollection();*/
            }
        }

        private void lbGenres_DrawItem(object sender, ListBoxDrawItemEventArgs e)
        {
            // TODO : Create SQL Version
            /*IEnumerable<KeyValuePair<string, string>> matches = MainEditor._titleCollection.GenreMap.Where(kvp => kvp.Value == (string)e.Item);
            if (matches.Count() > 0)
            {
                AppearanceObject appearance = (AppearanceObject)e.Appearance.Clone();
                appearance.Font = new Font(appearance.Font, FontStyle.Bold);
                e.Appearance.Combine(appearance);
            }*/
        }

        private void ceFoldersAsTitles_CheckStateChanged(object sender, EventArgs e)
        {
            cePrependParentFolder.Enabled = this.ceFoldersAsTitles.Checked;
        }

        private void beTitledFanArtPath_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = @"Titled FanArt Path";
            fbd.SelectedPath = OMLSettings.TitledFanArtPath;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                beTitledFanArtPath.EditValue = fbd.SelectedPath;
            }
        }

        private void ceTitledFanArtFolder_CheckStateChanged(object sender, EventArgs e)
        {
            beTitledFanArtPath.Enabled = this.ceTitledFanArtFolder.Checked;
        }

        private void simpleButtonScanMntTool_Click(object sender, EventArgs e)
        {
            OMLEngine.FileSystem.MountingTool mnt = new OMLEngine.FileSystem.MountingTool();
            teMntToolPath.Text =
                mnt.ScanForMountTool((OMLEngine.FileSystem.MountingTool.Tool)Enum.Parse(typeof(OMLEngine.FileSystem.MountingTool.Tool), rgMountingTool.Text), cmbMntToolScan.Text);

            if (teMntToolPath.Text == "") XtraMessageBox.Show("Cannot find the selected image mounting tool!");

        }
    }
}
