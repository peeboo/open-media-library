using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using OMLEngine;

namespace OMLDatabaseEditor
{
    public partial class ResolveGenres : DevExpress.XtraEditors.XtraForm
    {
        List<GenreMapping> _genreMapping = new List<GenreMapping>();
        List<String> _genreList = new List<String>();
        TitleCollection _titleCollection;
        Title _title;

        public ResolveGenres(Dictionary<string, string> genreIssueList, Title title)
        {
            InitializeComponent();

            _title = title;
            if (Properties.Settings.Default.gsValidGenres != null
            && Properties.Settings.Default.gsValidGenres.Count > 0)
            {
                int genreCount = Properties.Settings.Default.gsValidGenres.Count;
                String[] arrGenre = new String[genreCount];
                Properties.Settings.Default.gsValidGenres.CopyTo(arrGenre, 0);
                _genreList.AddRange(arrGenre);
            }

            foreach (string key in genreIssueList.Keys)
            {
                if (String.IsNullOrEmpty(genreIssueList[key]))
                    _genreMapping.Add(new GenreMapping() { SourceGenre = key, DestinationGenre = string.Empty, Action = MapActionEnum.Ignore });
                else
                    _genreMapping.Add(new GenreMapping() { SourceGenre = key, DestinationGenre = genreIssueList[key], Action = MapActionEnum.Map });
            }
            RenderGenreMapping();
        }

        private void RenderGenreMapping()
        {
            int i = 0;
            foreach (GenreMapping map in _genreMapping)
            {
                LabelControl lblSource = new LabelControl();
                lblSource.Name = "lblSource" + i;
                lblSource.Text = map.SourceGenre;
                lblSource.Dock = DockStyle.Fill;
                tableLayoutPanel1.Controls.Add(lblSource);
                ComboBoxEdit cbeGenre = new ComboBoxEdit();
                cbeGenre.Name = "cbeGenre" + i;
                cbeGenre.Text = map.DestinationGenre;
                cbeGenre.Properties.Items.AddRange(_genreList);
                
                // Add the genre from the plugin into the combo box if not allready there.
                if (!_genreList.Contains(map.SourceGenre))
                {
                    cbeGenre.Properties.Items.Add(map.SourceGenre);
                }

                cbeGenre.Dock = DockStyle.Fill;
                tableLayoutPanel1.Controls.Add(cbeGenre);
                RadioGroup grpActions = new RadioGroup();
                grpActions.Name = "grpActions" + i;
                grpActions.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(MapActionEnum.Ignore, "Ignore"));
                grpActions.Properties.Items.Add(new DevExpress.XtraEditors.Controls.RadioGroupItem(MapActionEnum.Map, "Map"));
                grpActions.SelectedIndex = map.Action == MapActionEnum.Ignore ? 0 : 1;
                grpActions.Properties.Columns = 2;
                grpActions.Height = 25;
                grpActions.Dock = DockStyle.Fill;
                tableLayoutPanel1.Controls.Add(grpActions);
                i++;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int i = 0;
            foreach (GenreMapping map in _genreMapping)
            {
                ComboBoxEdit cbeGenre = tableLayoutPanel1.Controls.Find("cbeGenre" + i, false)[0] as ComboBoxEdit;
                RadioGroup grpActions = tableLayoutPanel1.Controls.Find("grpActions" + i, false)[0] as RadioGroup;
                if (grpActions.SelectedIndex == 0)
                {
                    // ignore genre
                    if (_title.Genres.Contains(map.SourceGenre))
                        _title.RemoveGenre(map.SourceGenre);

                    
                    OMLEngine.Dao.GenreMapping mapping = new OMLEngine.Dao.GenreMapping();
                    mapping.GenreName = map.SourceGenre;
                    mapping.GenreMapTo = string.Empty;
                    OMLEngine.Settings.SettingsManager.GenreMap_Add(mapping);
                }
                else
                {
                    // map genre
                    if (_title.Genres.Contains(map.SourceGenre))
                        _title.RemoveGenre(map.SourceGenre);
                    if (!_title.Genres.Contains(cbeGenre.Text))
                        _title.AddGenre(cbeGenre.Text);

                    OMLEngine.Settings.SettingsManager.GenreMap_Remove(map.SourceGenre);
                    OMLEngine.Dao.GenreMapping mapping = new OMLEngine.Dao.GenreMapping();
                    mapping.GenreName = map.SourceGenre;
                    mapping.GenreMapTo = cbeGenre.Text;
                    OMLEngine.Settings.SettingsManager.GenreMap_Add(mapping);
                }
                i++;
            }
        }
    }

    public enum MapActionEnum
    {
        Ignore
        , Map
    }

    internal class GenreMapping
    {
        private string _sourceGenre = string.Empty;
        public string SourceGenre
        {
            get { return _sourceGenre; }
            set { _sourceGenre = value; }
        }

        private string _destinationGenre = string.Empty;
        public string DestinationGenre
        {
            get { return _destinationGenre; }
            set { _destinationGenre = value; }
        }

        private MapActionEnum _action = MapActionEnum.Ignore;
        public MapActionEnum Action
        {
            get { return _action; }
            set { _action = value; }
        }
    }
}