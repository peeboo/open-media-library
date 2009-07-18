using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using OMLEngine;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class frmSearchResult : Form
    {
        IOMLMetadataPlugin _plugin;

        Title[] _titles = null;
        int _selectedTitle = -1;
        bool _overwriteMetadata;
        MainEditor _openerForm;
        public string _lastMetaPluginName;

        bool TVSearch;
        int TVSearchStage;

        /*public string LastMetaPluginName
        {
            get { return _lastMetaPluginName; }
            set { _lastMetaPluginName = value; }
        }

        public string ReSearchText
        {
            get { return reSearchTitle.Text; }
            set { reSearchTitle.Text = value; }
        }*/

        public bool OverwriteMetadata
        {
            get { return _overwriteMetadata; }
            set { _overwriteMetadata = value; }
        }

        public int SelectedTitleIndex
        {
            get { return _selectedTitle; }
        }

        public frmSearchResult(IOMLMetadataPlugin plugin, string searchstr, MainEditor opener)
        {
            _plugin = plugin;
            _openerForm = opener;

            InitializeComponent();

            if (string.IsNullOrEmpty(_plugin.ProviderLink))
            {
                lcProviderMessage.Text = plugin.ProviderMessage;
            }
            else
            {
                lcProviderMessage.Text = plugin.ProviderMessage + " - Click to view website";
            }

            reSearchTitle.Text = searchstr;

            Search();
        }

        public frmSearchResult()
        {
            InitializeComponent();
        }

        private void frmSearchResult_Load(object sender, EventArgs e)
        {

        }

        private void Search()
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                if ((_plugin.GetPluginCapabilities & MetadataPluginCapabilities.SupportsMovieSearch) != 0)
                {
                    _plugin.SearchForMovie(reSearchTitle.Text);
                    TVSearch = false;
                }
                else
                {
                    if ((_plugin.GetPluginCapabilities & MetadataPluginCapabilities.SupportsTVSearch) != 0)
                    {
                        _plugin.SearchForTVSeries(reSearchTitle.Text);
                        TVSearch = true;
                        TVSearchStage = 0;
                    }
                }

                _titles = _plugin.GetAvailableTitles();
                ShowResults();
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[OMLDatabaseEditor] Metadata search caused an Exception {0}", ex);
            }
 
            Cursor = Cursors.Default;
         }

        private string MakeStringFromList(IList<string> list)
        {
            string ret = "";
            if (list != null)
            {
                foreach (string s in list)
                {
                    if (ret.Length > 0) ret += ", ";
                    ret += s;
                }
            }
            return ret;
        }

        private string MakeStringFromRoleList(IList<Role> list)
        {
            string ret = "";
            if (list != null)
            {
                foreach (Role p in list)
                {
                    if (ret.Length > 0) ret += ", ";
                    ret += p.PersonName;
                }
            }
            return ret;
        }

        private string MakeStringFromPersonList(IList<Person> list)
        {
            string ret = "";
            if (list != null)
            {
                foreach (Person p in list)
                {
                    if (ret.Length > 0) ret += ", ";
                    ret += p.full_name;
                }
            }
            return ret;
        }

        private string MakeStringFromDictionary(Dictionary<string, string> list)
        {
            string ret = "";
            if (list != null)
            {
                foreach (KeyValuePair<string, string> kvp in list)
                {
                    if (ret.Length > 0) ret += ", ";
                    ret += kvp.Key;
                }
            }
            return ret;
        }

        public void ShowResults()//Title[] titles)
        {
            //_titles = null;
            grdTitles.Rows.Clear();
            //_titles = titles;
            if (_titles != null)
            {
                int i = 0;
                foreach (Title t in _titles)
                {
                    if (t != null)
                    {
                        Image coverArt = null;

                        if (t.FrontCoverPath != null && File.Exists(t.FrontCoverPath))
                        {
                            coverArt = Utilities.ReadImageFromFile(t.FrontCoverPath);
                        }
                        string releaseDate = "";
                        if (t.ReleaseDate.Year > 1900)
                            releaseDate = t.ReleaseDate.Year.ToString();

                        grdTitles.Rows.Add(i.ToString(), coverArt, t.Name, t.Synopsis, releaseDate, MakeStringFromList(t.Genres), MakeStringFromPersonList(t.Directors), MakeStringFromRoleList(t.ActingRoles));
                        i++;
                    }
                }
            }
            //return ShowDialog();
        }

        private void btnSelectMovie_Click(object sender, EventArgs e)
        {
            if (chkUpdateMissingDataOnly.Checked)
                _overwriteMetadata = false;
            else
                _overwriteMetadata = true;

            if (TVSearch == false)
            {
                if (grdTitles.SelectedRows != null && grdTitles.SelectedRows.Count > 0)
                {


                    _selectedTitle = grdTitles.SelectedRows[0].Index;
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    _selectedTitle = -1;
                }
            }
            else
            {
                Cursor = Cursors.WaitCursor;

                if (grdTitles.SelectedRows != null && grdTitles.SelectedRows.Count > 0)
                {
                    switch (TVSearchStage)
                    {
                        case 0:
                            _plugin.SearchForTVEpisodes(grdTitles.SelectedRows[0].Index);
                            _titles = _plugin.GetAvailableTitles();
                            TVSearchStage = 1;
                            ShowResults();
                            break;
                        case 1:
                            _selectedTitle = grdTitles.SelectedRows[0].Index;
                            DialogResult = DialogResult.OK;
                            break;
                    }
                }
                Cursor = Cursors.Default;

            }
        }

        private void grdTitles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void reSearchSubmitButton_Click(object sender, EventArgs e)
        {
            submitNewTitleSearch();
        }

        private void reSearchTitleKeypress(Object o, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                submitNewTitleSearch();
                e.Handled = true;
            }
        }

        private void submitNewTitleSearch()
        {
            Search();
            /*try
            {
                if (_openerForm != null)
                {
                    bool reSearch = _openerForm.StartMetadataImport(_lastMetaPluginName, false, reSearchTitle.Text, this);
                }
            }
            catch
            {
                Cursor = Cursors.Default;
            }*/
        }

        private void lcProviderMessage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_plugin.ProviderLink))
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                string link = _plugin.ProviderLink;

                p.StartInfo.FileName = link;
                p.Start();
            }
        }
    }
}