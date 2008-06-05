using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OMLEngine;

namespace OMLDatabaseEditor
{
    public partial class OMLDatabaseEditor : Form
    {
        private TitleCollection _titleCollection;
        private Title current_title;
        private DataTable _collectionAsDataTable;
        private Bitmap front_cover;
        private Bitmap back_cover;
        private Title _currentTitle = null;
        private bool _titleChanged = false;
        
        CurrencyManager _currencyManager;

        public OMLDatabaseEditor()
        {
            InitializeComponent();
            _titleCollection = new TitleCollection();
            _titleCollection.loadTitleCollection();
            SetupTitleList();
            
        }

        private void SetupTitleList()
        {
            //_collectionAsDataTable = GetTitlesDataTable();
            grdTitleList.Rows.Clear();
            foreach (Title t in _titleCollection)
            {
                grdTitleList.Rows.Add(t.Name, t.InternalItemID);
            }

            //grdTitleList.AutoGenerateColumns = false;
            //grdTitleList.DataSource = _collectionAsDataTable;
        }
        
        private DataTable GetTitlesDataTable()
        {
            DataTable dt = new DataTable("Titles");

            DataColumn idIndex = new DataColumn("Index");
            idIndex.DataType = typeof(int);
            dt.Columns.Add(idIndex);

            DataColumn idCol = new DataColumn("itemId");
            idCol.DataType = typeof(int);
            dt.Columns.Add(idCol);

            DataColumn nameCol = new DataColumn("TitleName");
            nameCol.DataType = typeof(string);
            dt.Columns.Add(nameCol);

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = dt.Columns["itemId"];
            dt.PrimaryKey = PrimaryKeyColumns;

            DataRow row;
            int currentIndex = 0;
            foreach (Title title in _titleCollection)
            {
                row = dt.NewRow();
                row["Index"] = currentIndex;
                row["itemId"] = title.InternalItemID;
                row["TitleName"] = title.Name;
                dt.Rows.Add(row);
                currentIndex++;
            }

            return dt;
        }

        private void UpdateUIFromTitle(Title t)
        {
            labelDistributor.Text = t.Synopsis;
            tbTitle.Text = t.Name;
            tbOriginalName.Text = t.OriginalName;

            try
            {
                dtpReleaseDate.Value = t.ReleaseDate;
            }
            catch
            {
                dtpReleaseDate.Value = DateTimePicker.MinimumDateTime;
            }

            try
            {
                dtpDateAdded.Value = t.DateAdded;
            }
            catch
            {
                dtpDateAdded.Value = DateTimePicker.MinimumDateTime;
            }
            
            tbRunTime.Text = Convert.ToString(t.Runtime);
            cbRating.Text = t.MPAARating;
            tbCountryOfOrigin.Text = t.CountryOfOrigin;
            tbFrontCover.Text = t.FrontCoverPath;
            tbBackCover.Text = t.BackCoverPath;
            pbFrontCover.Image = ReadImageFromFile(t.FrontCoverPath);
            pbBackCover.Image = ReadImageFromFile(t.BackCoverPath);
            cbAspectRatio.Text = t.AspectRatio;
            if (t.SortName.Trim().Length > 0)
                tbSortName.Text = t.SortName;
            else
                tbSortName.Text = t.Name;
            tbRatingReason.Text = t.MPAARatingReason;
            tbStudio.Text = t.Distributor;
            tbFileLocation.Text = t.FileLocation;

            tbUserRating.Text = Convert.ToString(t.UserStarRating);
            
            grdDirectors.Rows.Clear();
            foreach( Person d in t.Directors )
            {
                grdDirectors.Rows.Add(d.full_name);
            }

            grdWriters.Rows.Clear();
            foreach (Person w in t.Writers)
            {
                grdWriters.Rows.Add(w.full_name);
            }

            grdGenres.Rows.Clear();
            foreach (string g in t.Genres)
            {
                grdGenres.Rows.Add(g);
            }

            grdTags.Rows.Clear();
            foreach (string tag in t.Tags)
            {
                grdTags.Rows.Add(tag);
            }

            grdActors.Rows.Clear();
            foreach (KeyValuePair<string, string> role in t.ActingRoles)
            {
                grdActors.Rows.Add(role.Key, role.Value);
            }

            foreach (Person actor in t.Actors)
            {
                if( !t.ActingRoles.ContainsKey(actor.full_name) )
                    grdActors.Rows.Add(actor.full_name, " ");
            }

            grdLanguages.Rows.Clear();
            foreach (string l in t.LanguageFormats)
            {
                grdLanguages.Rows.Add(l);
            }

            grdAudioTracks.Rows.Clear();
            foreach (string a in t.SoundFormats)
            {
                grdAudioTracks.Rows.Add(a);
            }

            _titleChanged = false;
        }

        private void UpdateTitleFromUI( Title t)
        {
            t.Synopsis = labelDistributor.Text.Trim();
            t.Name = tbTitle.Text.Trim();
            t.OriginalName = tbOriginalName.Text.Trim();

            t.ReleaseDate = dtpReleaseDate.Value;
            t.DateAdded = dtpDateAdded.Value;

            t.Runtime  = Convert.ToInt32(tbRunTime.Text);
            t.MPAARating = cbRating.Text.Trim();
            t.CountryOfOrigin = tbCountryOfOrigin.Text.Trim();
            t.FrontCoverPath = tbFrontCover.Text.Trim();
            t.BackCoverPath = tbBackCover.Text.Trim();
            t.AspectRatio = cbAspectRatio.Text.Trim();
            if (tbSortName.Text.Trim().Length > 0)
                t.SortName = tbSortName.Text.Trim();
            else
                t.SortName = t.Name;

            t.MPAARatingReason = tbRatingReason.Text.Trim();
            t.Distributor = tbStudio.Text.Trim();
            t.FileLocation = tbFileLocation.Text.Trim();

            t.UserStarRating = Convert.ToInt32(tbUserRating.Text);


            t.Directors.Clear();
            foreach (DataGridViewRow row in grdDirectors.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.Directors.Add( new Person((string)row.Cells[0].Value));
            }

            t.Writers.Clear();
            foreach (DataGridViewRow row in grdWriters.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.Writers.Add(new Person((string)row.Cells[0].Value));
            }

            t.Genres.Clear();
            foreach (DataGridViewRow row in grdGenres.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.Genres.Add((string)row.Cells[0].Value);
            }

            t.Tags.Clear();
            foreach (DataGridViewRow row in grdTags.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.Tags.Add((string)row.Cells[0].Value);
            }

            t.Actors.Clear();
            t.ActingRoles.Clear();
            foreach (DataGridViewRow row in grdTags.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[1].Value == null) return;
                t.Actors.Add(new Person((string)row.Cells[0].Value));
                t.ActingRoles.Add((string)row.Cells[0].Value, (string)row.Cells[1].Value);
            }

            t.LanguageFormats.Clear();
            foreach (DataGridViewRow row in grdLanguages.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.LanguageFormats.Add((string)row.Cells[0].Value);
            }

            t.SoundFormats.Clear();
            foreach (DataGridViewRow row in grdAudioTracks.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.SoundFormats.Add((string)row.Cells[0].Value);
            }
        }

        private void dgv_title_list_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void OMLDatabaseEditor_Load(object sender, EventArgs e)
        {
            _currencyManager = (CurrencyManager)this.BindingContext[_titleCollection];
        }


        // Image.FromFile keeps a lock on the file and it cannot be updated
        public static Image ReadImageFromFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        using (MemoryStream ms = new MemoryStream(buffer))
                        {
                            Bitmap bmp1 = new Bitmap(ms);
                            Bitmap bmp2 = new Bitmap(bmp1.Width, bmp1.Height, bmp1.PixelFormat);
                            Graphics g = Graphics.FromImage(bmp2);
                            GraphicsUnit pageUnit = new GraphicsUnit();
                            g.DrawImage(bmp1, bmp2.GetBounds(ref pageUnit));


                            return bmp2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            return null;
        }


        private void tbTitle_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void tbSortName_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void tbOriginalName_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void cbRating_SelectedIndexChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void tbCountryOfOrigin_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }


        private void cbVideoStandard_SelectedIndexChanged(object sender, EventArgs e)
        {
            _titleChanged = true;
        }

        private void cbAspectRatio_SelectedIndexChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }

        private void tbRunTime_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }

        private void tbUserRating_TextChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }

        private void dtpDateAdded_ValueChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }

        private void dtpReleaseDate_ValueChanged(object sender, EventArgs e)
        {
            _titleChanged = true;

        }
        private void UpdateDBFromCurrentTitle(Title t)
        {
            _titleCollection.Replace(t);
            _titleCollection.saveTitleCollection();
        }
        private void SelectNewTitle( int rowIndex)
        {
            bool bSelectNewTitle = true;

            if (_titleChanged)
            {
                DialogResult result = MessageBox.Show("Do you want to save the changes to the current movie?", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Cancel)
                {
                    bSelectNewTitle = false;
                }
                else if (result == DialogResult.Yes)
                {
                    UpdateTitleFromUI(_currentTitle);
                    UpdateDBFromCurrentTitle(_currentTitle);
                    _titleChanged = false;
                }
                else
                {
                    _titleChanged = false;
                }
            }

            if (bSelectNewTitle)
            {
                int itemId = (int)grdTitleList.Rows[rowIndex].Cells[1].Value;
                _currentTitle = (Title)_titleCollection.MoviesByItemId[itemId];
                UpdateUIFromTitle(_currentTitle);
            }

        }

        private void grdTitleList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectNewTitle(e.RowIndex);
        }

        private void grdTitleList_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateTitleFromUI(_currentTitle);
            UpdateDBFromCurrentTitle(_currentTitle);
            _titleChanged = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            UpdateTitleFromUI(_currentTitle);
            UpdateDBFromCurrentTitle(_currentTitle);
            _titleChanged = false;
        }

    }
}

