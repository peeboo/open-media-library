using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OMLEngine;

namespace OMLDatabaseEditor
{
    public partial class OMLDatabaseEditor : Form
    {
        private TitleCollection _titleCollection;
        private Title current_title;
        private DataTable dt;
        private Bitmap front_cover;
        private Bitmap back_cover;
        private bool current_title_has_changed;

        public OMLDatabaseEditor()
        {
            InitializeComponent();
            _titleCollection = new TitleCollection();
            _titleCollection.loadTitleCollection();
            SetupTitleList();
            SetupRating();
            current_title = new Title();
            current_title_has_changed = false;
        }

        private void SetupRating()
        {
            //TODO
            //////string[] rating_names = Enum.GetNames(typeof(Rating));
            //////foreach (string rating in rating_names)
            //////{
            //////    cbRating.Items.Add(rating);
            //////}
        }

        private void SetupTitleList()
        {
            dt = GetTitlesDataTable();
            dgv_title_list.AutoGenerateColumns = false;
            dgv_title_list.DataSource = dt;
        }
        private DataTable GetTitlesDataTable()
        {
            DataTable dt = new DataTable("Titles");
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
            foreach (Title title in _titleCollection)
            {
                row = dt.NewRow();
                row["itemId"] = title.InternalItemID;
                row["TitleName"] = title.Name;
                dt.Rows.Add(row);
            }

            return dt;
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
        }
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            _titleCollection.RemoveTitle(current_title);
        }
        private void dgv_title_list_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataRow row = dt.Rows[e.RowIndex];

            foreach (Title t in _titleCollection)
            {
                if (t.InternalItemID.CompareTo((int)row["itemId"]) == 0)
                    if (CheckAndUpdateCurrentTitle())
                        SetNewCurrentTitle(t);
            }
        }
        private bool CheckAndUpdateCurrentTitle()
        {
            if (current_title != null)
            {
                if (current_title_has_changed)
                {
                    DialogResult result = MessageBox.Show(
                        "The current title seems to have some changes, would you like to save them?",
                        "Save Changes", MessageBoxButtons.YesNo);

                    switch (result)
                    {
                        case DialogResult.Yes:
                            SaveCurrentTitle();
                            break;
                        case DialogResult.No:
                            break;
                    }
                }
            }
            return true;
        }
        private void SaveCurrentTitle()
        {
            current_title.Name = tbTitle.Text;
            current_title.Description = tbDescription.Text;
            current_title.ReleaseDate = dtpReleaseDate.Value;
            current_title.Runtime = Int32.Parse(tbRunTime.Text);
            //current_title.MPAARating = cbRating.Text;
            current_title.Distributor = tbDistributor.Text;
            current_title.CountryOfOrigin = tbCountryOfOrigin.Text;
            current_title.OfficialWebsiteURL = tbWebsite.Text;
            current_title.ImporterSource = tbImporterSource.Text;
            current_title.DateAdded = dtpDateAdded.Value;
            current_title.Synopsis = tbSynopsis.Text;

            _titleCollection.saveTitleCollection();
        }
        private void SetNewCurrentTitle(Title t)
        {
            current_title = t;
            tbTitle.Text = current_title.Name;
            tbDescription.Text = current_title.Description;
            if (current_title.ReleaseDate.Year > 0001)
                dtpReleaseDate.Value = current_title.ReleaseDate;

            tbRunTime.Text = current_title.Runtime.ToString();
            //cbRating.SelectedIndex = (int)current_title.MPAARating;
            tbDistributor.Text = current_title.Distributor;
            tbCountryOfOrigin.Text = current_title.CountryOfOrigin;
            tbWebsite.Text = current_title.OfficialWebsiteURL;
            tbImporterSource.Text = current_title.ImporterSource;
            //dtpDateAdded.Value = current_title.DateAdded;
            tbSynopsis.Text = current_title.Synopsis;

            if (current_title.FrontCoverPath != null && current_title.FrontCoverPath.Length > 0)
            {
                try
                {
                    front_cover = new Bitmap(current_title.FrontCoverPath);
                    pbFrontCover.Image = (Image)front_cover;
                }
                catch (Exception)
                {
                }
            }
            else
            {
                pbFrontCover.Image = pbFrontCover.InitialImage;
            }

            if (current_title.BackCoverPath != null && current_title.BackCoverPath.Length > 0)
            {
                back_cover = new Bitmap(current_title.BackCoverPath);
                pbBackCover.Image = (Image)back_cover;
            }
            else
            {
                pbBackCover.Image = pbBackCover.InitialImage;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            current_title_has_changed = true;
        }
        /*
        private void OMLDatabaseEditor_DragDrop(object sender, DragEventArgs e)
        {
            MessageBox.Show("haa!");
        }

        private void OMLDatabaseEditor_DragEnter(object sender, DragEventArgs e)
        {
            MessageBox.Show("Boo!");
        }
        */
    }
}
