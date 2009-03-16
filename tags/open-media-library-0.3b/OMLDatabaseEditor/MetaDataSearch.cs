﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using OMLEngine;

namespace OMLDatabaseEditor
{
    public partial class frmSearchResult : Form
    {
        Title[] _titles = null;
        int _selectedTitle = -1;
        bool _overwriteMetadata;
        MainEditor _openerForm;
        public string _lastMetaPluginName;

        public string LastMetaPluginName
        {
            get { return _lastMetaPluginName; }
            set { _lastMetaPluginName = value; }
        }

        public string ReSearchText
        {
            get { return reSearchTitle.Text; }
            set { reSearchTitle.Text = value; }
        }

        public bool OverwriteMetadata
        {
            get { return _overwriteMetadata; }
            set { _overwriteMetadata = value; }
        }

        public int SelectedTitleIndex
        {
            get { return _selectedTitle; }
        }

        public frmSearchResult(MainEditor opener)
        {
            _openerForm = opener;
            InitializeComponent();
        }
        public frmSearchResult()
        {
            InitializeComponent();
        }

        private void frmSearchResult_Load(object sender, EventArgs e)
        {

        }

        private string MakeStringFromList(List<string> list)
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

        private string MakeStringFromPersonList(List<Person> list)
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

        public DialogResult ShowResults(Title[] titles)
        {
            _titles = null;
            grdTitles.Rows.Clear();
            _titles = titles;
            if (titles != null)
            {
                int i = 0;
                foreach (Title t in titles)
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

                        grdTitles.Rows.Add(i.ToString(), coverArt, t.Name, t.Synopsis, releaseDate, MakeStringFromList(t.Genres), MakeStringFromPersonList(t.Directors), MakeStringFromDictionary(t.ActingRoles));
                        i++;
                    }
                }
            }
            return ShowDialog();
        }

        private void btnSelectMovie_Click(object sender, EventArgs e)
        {
            if (grdTitles.SelectedRows != null && grdTitles.SelectedRows.Count > 0)
            {
                if (chkUpdateMissingDataOnly.Checked)
                    _overwriteMetadata = false;
                else
                    _overwriteMetadata = true;

                _selectedTitle = grdTitles.SelectedRows[0].Index;
            }
            else
            {
                _selectedTitle = -1;
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
            try
            {
                if (_openerForm != null)
                {
                    bool reSearch = _openerForm.StartMetadataImport(_lastMetaPluginName, false, reSearchTitle.Text, this);
                }
            }
            catch
            {
                Cursor = Cursors.Default;
            }
        }
    }
}