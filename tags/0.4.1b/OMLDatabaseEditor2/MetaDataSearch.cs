using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.ComponentModel;
using OMLEngine;
using OMLSDK;

namespace OMLDatabaseEditor
{
    public partial class frmSearchResult : Form
    {
        MetaDataPluginDescriptor _plugin;

        Title[] _titles = null;
        int _selectedTitle = -1;
        //bool _overwriteMetadata;

        // Store last search criteria to avoid researching if nothing changes
        string LastSearchTitle;
        string LastEpisodeName;
        int? LastSeasonNo;
        int? LastEpisodeNo;

        public string _lastMetaPluginName;

        List<KeyValuePair<string, string>> ImageLoadQueue = new List<KeyValuePair<string,string>>();

        bool TVSearch;
        bool SearchTVShowOnly;
        bool SearchDrillDownReq;
        bool TVShowFound;


        /*public bool OverwriteMetadata
        {
            get { return _overwriteMetadata; }
            set { _overwriteMetadata = value; }
        }*/

        public int SelectedTitleIndex
        {
            get { return _selectedTitle; }
        }

        public frmSearchResult(MetaDataPluginDescriptor plugin, string searchstr, string EpisodeName, int ? SeasonNo, int ? EpisodeNo, bool ShowTVFields, bool pSearchTVShowOnly) //MainEditor opener)
        {
            _plugin = plugin;
            SearchTVShowOnly = pSearchTVShowOnly;

            InitializeComponent();

            chkUpdateMissingDataOnly.Checked = !OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingData;
            chkUpdateTitleName.Checked = OMLEngine.Settings.OMLSettings.MetadataLookupUpdateName;

            if (string.IsNullOrEmpty(_plugin.DataProviderLink))
            {
                lcProviderMessage.Text = plugin.DataProviderMessage;
            }
            else
            {
                lcProviderMessage.Text = plugin.DataProviderMessage + " - Click to view website";
            }

            // Hide the tv fields of not reqiured
            if (!ShowTVFields)
            {
                teEpisodeName.Visible = false;
                seEpisodeNo.Visible = false;
                seSeasonNo.Visible = false;
                lcEpisodeLabel.Visible = false;
                lcSeasonNoLabel.Visible = false;
                lcEpisodeNoLabel.Visible = false;
                reSearchSubmitButton.Location = new Point(335, 3);
            }

            reSearchTitle.Text = searchstr;
            LastSearchTitle = searchstr;

            teEpisodeName.Text = EpisodeName;
            LastEpisodeName = EpisodeName;
            if (SeasonNo != null)
            {
                seSeasonNo.Value = Convert.ToInt32(SeasonNo);
                LastSeasonNo = Convert.ToInt32(SeasonNo);
            }
            if (EpisodeNo != null)
            {
                seEpisodeNo.Value = Convert.ToInt32(EpisodeNo);
                LastEpisodeNo = Convert.ToInt32(EpisodeNo);
            }

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
                if ((_plugin.DataProviderCapabilities & MetadataPluginCapabilities.SupportsMovieSearch) != 0)
                {
                    _plugin.PluginDLL.SearchForMovie(reSearchTitle.Text, OMLEngine.Settings.OMLSettings.MetadataLookupResultsQty);
                    TVSearch = false;
                    SearchDrillDownReq = false;
                }
                else
                {
                    if ((_plugin.DataProviderCapabilities & MetadataPluginCapabilities.SupportsTVSearch) != 0)
                    {
                        SearchDrillDownReq = _plugin.PluginDLL.SearchForTVSeries(reSearchTitle.Text, 
                            teEpisodeName.Text, 
                            Convert.ToInt32(seSeasonNo.Value), 
                            Convert.ToInt32(seEpisodeNo.Value), 
                            OMLEngine.Settings.OMLSettings.MetadataLookupResultsQty,
                            SearchTVShowOnly);
                        TVSearch = true;
                        TVShowFound = !SearchDrillDownReq;
                    }
                }

                _titles = _plugin.PluginDLL.GetAvailableTitles();
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

                        if (t.FrontCoverPath != null)
                        {
                            if (string.Compare(t.FrontCoverPath.Substring(0, 4), "http", true) == 0)
                            {
                                // Images are not downloaded. Add to lazy load queue
                                ImageLoadQueue.Add(new KeyValuePair<string, string>(i.ToString() + t.Name, t.FrontCoverPath));
                            }
                            else
                            {
                                if (File.Exists(t.FrontCoverPath))
                                {
                                    coverArt = Utilities.ReadImageFromFile(t.FrontCoverPath);
                                }
                            }
                        }

                        string releaseDate = "";
                        if (t.ReleaseDate.Year > 1900)
                            releaseDate = t.ReleaseDate.Year.ToString();

                        string Name = null;
                        if (TVSearch)
                        {
                            StringBuilder bd = new StringBuilder();
                            if (t.SeasonNumber > 0) bd.Append("Season " + t.SeasonNumber.ToString() + " : ");
                            if (t.EpisodeNumber > 0) bd.Append("Episode " + t.EpisodeNumber.ToString() + " : ");
                            bd.Append(t.Name);
                            Name = bd.ToString();
                        }
                        else
                        {
                            Name = t.Name;
                        }

                        DataGridViewRow dr = new DataGridViewRow();
                        dr.CreateCells(grdTitles,new object[] { i.ToString(), coverArt, Name, t.Synopsis, releaseDate, MakeStringFromList(t.Genres), MakeStringFromPersonList(t.Directors), MakeStringFromRoleList(t.ActingRoles) } );
                        dr.Height = 120;
                        dr.Tag = i.ToString() + t.Name;
                        grdTitles.Rows.Add(dr);

                        //grdTitles.Rows.Add(i.ToString(), coverArt, Name, t.Synopsis, releaseDate, MakeStringFromList(t.Genres), MakeStringFromPersonList(t.Directors), MakeStringFromRoleList(t.ActingRoles));
                        i++;
                    }
                }
            }
            //LoadImages();

            for (int i = 0; i < 5; i++)
            {
                if (ImageLoadQueue.Count > 0)
                {
                    LoadImage();
                }
            }
            //return ShowDialog();
        }

        private void btnSelectMovie_Click(object sender, EventArgs e)
        {
            OMLEngine.Settings.OMLSettings.MetadataLookupOverwriteExistingData = !chkUpdateMissingDataOnly.Checked;
            OMLEngine.Settings.OMLSettings.MetadataLookupUpdateName = chkUpdateTitleName.Checked;

            /*if (chkUpdateMissingDataOnly.Checked)
                _overwriteMetadata = false;
            else
                _overwriteMetadata = true;*/

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
                    if (SearchDrillDownReq)
                    {
                        if (grdTitles.SelectedRows.Count > 0)
                        {
                            Cursor = Cursors.WaitCursor;
                            SearchDrillDownReq = _plugin.PluginDLL.SearchForTVDrillDown(grdTitles.SelectedRows[0].Index, teEpisodeName.Text, Convert.ToInt32(seSeasonNo.Value), Convert.ToInt32(seEpisodeNo.Value), OMLEngine.Settings.OMLSettings.MetadataLookupResultsQty);
                            TVShowFound = true;
                            _titles = _plugin.PluginDLL.GetAvailableTitles();
                            ShowResults();
                            Cursor = Cursors.Default;
                        }
                    }
                    else
                    {
                        _selectedTitle = grdTitles.SelectedRows[0].Index;
                        DialogResult = DialogResult.OK;
                    }

                }
            }
        }

        private void grdTitles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void reSearchSubmitButton_Click(object sender, EventArgs e)
        {
            if (LastSearchTitle != reSearchTitle.Text)
            {
                // If the search criteria has changed perform new movie/show search search
                LastSearchTitle = reSearchTitle.Text;
                submitNewTitleSearch();
            }
            else
            {
                // Otherwise no need to perform new movie/show search
                // If we are searching for a TV episode, do that
                if (TVSearch)
                {
                    if ((LastEpisodeName != teEpisodeName.Text) ||
                        (LastSeasonNo != seSeasonNo.Value) ||
                        (LastEpisodeNo != seEpisodeNo.Value))
                    {
                        if (TVShowFound)
                        {
                            //if (grdTitles.SelectedRows.Count > 0)
                            //{
                            LastEpisodeName = teEpisodeName.Text;
                            LastSeasonNo = (int)seSeasonNo.Value;
                            LastEpisodeNo = (int)seEpisodeNo.Value;
                            Cursor = Cursors.WaitCursor;
                            //SearchDrillDownReq = _plugin.PluginDLL.SearchForTVDrillDown(grdTitles.SelectedRows[0].Index, teEpisodeName.Text, Convert.ToInt32(seSeasonNo.Value), Convert.ToInt32(seEpisodeNo.Value), OMLEngine.Settings.OMLSettings.MetadataLookupResultsQty);
                            SearchDrillDownReq = _plugin.PluginDLL.SearchForTVDrillDown(0, teEpisodeName.Text, Convert.ToInt32(seSeasonNo.Value), Convert.ToInt32(seEpisodeNo.Value), OMLEngine.Settings.OMLSettings.MetadataLookupResultsQty);
                            _titles = _plugin.PluginDLL.GetAvailableTitles();
                            ShowResults();
                            Cursor = Cursors.Default;
                            //}
                        }
                    }
                }
            }
        }

        private void reSearchTitleKeypress(Object o, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                reSearchSubmitButton_Click(null, null);
                e.Handled = true;
            }
        }

        private void teEpisodeName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                reSearchSubmitButton_Click(null, null);
                e.Handled = true;
            }
        }

        private void submitNewTitleSearch()
        {
            Search();
        }

        private void lcProviderMessage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_plugin.DataProviderLink))
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                string link = _plugin.DataProviderLink;

                p.StartInfo.FileName = link;
                p.Start();
            }
        }

        /*private void LoadImages()
        {
            /*for (int i = 0; i < 5; i++)
            {
                if (ImageLoadQueue.Count > 0)
                {
                    //LoadImage();
                }
            }
        }*/

        private void LoadImage()
        {
            if (ImageLoadQueue.Count > 0)
            {
                KeyValuePair<string, string> src = ImageLoadQueue[0];

                ImageLoadQueue.RemoveAt(0);

                WebClient web = new WebClient();

                web.DownloadFileCompleted += new AsyncCompletedEventHandler(FileDownloadedEvent);

                KeyValuePair<string, string> dest = new KeyValuePair<string, string>(src.Key, Path.GetTempFileName());

                web.DownloadFileAsync(new Uri(src.Value), dest.Value, dest);
            }
        }

        private void FileDownloadedEvent(object sender, AsyncCompletedEventArgs c)
        {
            KeyValuePair<string, string> img = (KeyValuePair<string, string>)c.UserState;

            if (File.Exists(img.Value))
            {
                Image coverArt = Utilities.ReadImageFromFile(img.Value);
                //grdTitles.Rows[0].tag

                // Find the row
                foreach (DataGridViewRow dr in grdTitles.Rows)
                {
                    if (string.Compare((string)dr.Tag, img.Key) == 0)
                    {
                        dr.Cells[colCoverArt.Index].Value = coverArt;
                    }
                }
                //((DataGridViewImageCell)grdTitles[colCoverArt.Index, img.Key]).Value = coverArt;

            }
            LoadImage();
        }
    }
}