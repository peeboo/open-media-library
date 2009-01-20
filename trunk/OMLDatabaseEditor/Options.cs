using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

using OMLEngine;

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
        private Boolean GenreDirty = false;
        private Boolean TagsDirty = false;
        private List<String> MPAAList;
        private List<String> GenreList;
        private List<String> TagList;

        private void Options_Load(object sender, EventArgs e)
        {
            int genreCount = 0;
            this.lbcSkins.DataSource = ((MainEditor)this.Owner).DXSkins;
            String skin = Properties.Settings.Default.gsAppSkin;
            int idx = this.lbcSkins.FindItem(skin);
            if (idx < 0)
            {
                skin = ((MainEditor)this.Owner).LookAndFeel.SkinName;
                idx = this.lbcSkins.FindItem(skin);
            }
            this.lbcSkins.SetSelected(idx, true);
            this.ceUseMPAAList.Checked = Properties.Settings.Default.gbUseMPAAList;
            MPAAList = new List<String>();
            MPAAList.AddRange(Properties.Settings.Default.gsMPAARatings.Split('|'));
            MPAAList.Sort();
            this.lbcMPAA.DataSource = MPAAList;

            TagList = new List<string>();
            if (String.IsNullOrEmpty(Properties.Settings.Default.gsTags.Trim()))
            {
                TagList.AddRange(MainEditor._titleCollection.GetAllTags);
            }
            else
            {
                TagList.AddRange(MainEditor._titleCollection.GetAllTags.Union(Properties.Settings.Default.gsTags.Split('|')));
            }
            int iTags = Properties.Settings.Default.gsTags.Split('|').Count();
            if (iTags < TagList.Count()) TagsDirty = true;
            TagList.Sort();
            lbcTags.DataSource = TagList;

            this.ceUseGenreList.Checked = Properties.Settings.Default.gbUseGenreList;
            GenreList = new List<String>();
            if (Properties.Settings.Default.gsValidGenres != null 
            && Properties.Settings.Default.gsValidGenres.Count > 0)
            {
                genreCount = Properties.Settings.Default.gsValidGenres.Count;
                String[] arrGenre = new String[genreCount];
                Properties.Settings.Default.gsValidGenres.CopyTo(arrGenre, 0);
                GenreList.AddRange(arrGenre);
            }
            else
            {
                if (XtraMessageBox.Show("No allowable genres have been defined. Would you like to load them from your current movie collection?", "No Genres", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Properties.Settings.Default.gsValidGenres = new StringCollection();
                    Properties.Settings.Default.gsValidGenres.AddRange(MainEditor._titleCollection.GetAllGenres.ToArray());
                    GenreList.AddRange(MainEditor._titleCollection.GetAllGenres.ToArray());
                }
            }
            // I disabled this line because gsValidGenres is not just empty, its undef
            GenreList.Sort();
            lbGenres.DataSource = GenreList;

            this.ceFoldersAsTitles.Checked = OMLEngine.Properties.Settings.Default.FoldersAreTitles;
        }

        private void SimpleButtonClick(object sender, EventArgs e)
        {
            if (sender == this.sbOK)
            {
                Boolean bDirty = false;
                String skin = (String)this.lbcSkins.SelectedValue;
                if (skin != Properties.Settings.Default.gsAppSkin)
                {
                    bDirty = true;
                    Properties.Settings.Default.gsAppSkin = skin;
                }
                if (Properties.Settings.Default.gbUseMPAAList != this.ceUseMPAAList.Checked)
                {
                    bDirty = true;
                    Properties.Settings.Default.gbUseMPAAList = this.ceUseMPAAList.Checked;
                }
                if (MPAAdirty)
                {
                    bDirty = true;
                    String MPAAs = String.Join("|", MPAAList.ToArray());
                    Properties.Settings.Default.gsMPAARatings = MPAAs;
                }
                if (Properties.Settings.Default.gbUseGenreList != this.ceUseGenreList.Checked)
                {
                    bDirty = true;
                    Properties.Settings.Default.gbUseGenreList = this.ceUseGenreList.Checked;
                }
                if (GenreDirty)
                {
                    bDirty = true;
                    Properties.Settings.Default.gsValidGenres.Clear();
                    Properties.Settings.Default.gsValidGenres.AddRange(GenreList.ToArray());
                }
                if (TagsDirty)
                {
                    bDirty = true;
                    String Tags = String.Join("|", TagList.ToArray());
                    Properties.Settings.Default.gsTags = Tags;
                }
                if (bDirty)
                {
                    OptionsDirty = bDirty;
                    Properties.Settings.Default.Save();
                }
                if (OMLEngine.Properties.Settings.Default.FoldersAreTitles != this.ceFoldersAsTitles.Checked)
                {
                    OMLEngine.Properties.Settings.Default.FoldersAreTitles = this.ceFoldersAsTitles.Checked;
                    OMLEngine.Properties.Settings.Default.Save();
                }
            }
            else if (sender == this.sbCancel)
            {
                String skin = Properties.Settings.Default.gsAppSkin;
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
            else if (sender == btnGenre)
            {
                if (String.IsNullOrEmpty((String)btnGenre.Text)) return;
                if (GenreList.Contains(btnGenre.Text)) return;

                GenreList.Add(btnGenre.Text);
                GenreDirty = true;
                lbGenres.Refresh();
                btnGenre.Text = String.Empty;
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

        private void lbGenres_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lbGenres.SelectedItems.Count > 0)
            {
                foreach (object item in lbGenres.SelectedItems)
                {
                    string genre = item as string;
                    List<Title> titles = MainEditor._titleCollection.FindByGenre(genre);
                    if (titles.Count > 0)
                    {
                        StringBuilder message = new StringBuilder(titles.Count + " movie(s) in your collection are associated with the " + genre + " genre. Would you like to remove the association?\r\n\r\n");
                        foreach (Title title in titles)
                            message.Append(title.Name + "\r\n");
                        if (XtraMessageBox.Show(message.ToString(), "Remove Genre", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            foreach (Title title in MainEditor._titleCollection.FindByGenre(genre))
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
        }
    }
}