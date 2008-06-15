using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OMLEngine;
using System.IO;

namespace OMLDatabaseEditor.Controls
{
    public partial class MediaEditor : UserControl
    {
        public MediaEditor()
        {
            InitializeComponent();
        }

        private void MediaEditor_Load(object sender, EventArgs e)
        {

        }

        public void ChangeTitle(Title t)
        {
            UpdateUIFromTitle(t);
        }

        private void UpdateUIFromTitle(Title t)
        {
            // Movie Locations
            pbFrontCover.Image = ReadImageFromFile(t.FrontCoverPath);
            tbFrontCover.Text = t.FrontCoverPath;
            pbBackCover.Image = ReadImageFromFile(t.BackCoverPath);
            tbBackCover.Text = t.BackCoverPath;
            tbFileLocation.Text = t.FileLocation;

            // Movie Details
            tbName.Text = t.Name;
            tbSummary.Text = t.Synopsis;
            try
            {
                dtpReleaseDate.Value = t.ReleaseDate;
            }
            catch
            {
                dtpReleaseDate.Value = DateTimePicker.MinimumDateTime;
            }
            cbRating.Text = t.ParentalRating;
            tbRatingReason.Text = t.ParentalRatingReason;
            tbCountry.Text = t.CountryOfOrigin;
            tbStudio.Text = t.Studio;
            tbRunTime.Text = Convert.ToString(t.Runtime);

            // Credits
            grdDirectors.Rows.Clear();
            foreach (Person d in t.Directors)
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
            grdActors.Rows.Clear();
            foreach (Person actor in t.Actors)
            {
                if (!t.ActingRoles.ContainsKey(actor.full_name))
                    grdActors.Rows.Add(actor.full_name, " ");
            }
            foreach (KeyValuePair<string, string> role in t.ActingRoles)
            {
                grdActors.Rows.Add(role.Key, role.Value);
            }

            // Your Movie Details
            if (t.SortName.Trim().Length > 0)
            {
                tbSortName.Text = t.SortName;
            }
            else
            {
                tbSortName.Text = t.Name;
            }
            tbOriginalName.Text = t.OriginalName;
            try
            {
                dtpDateAdded.Value = t.DateAdded;
            }
            catch
            {
                dtpDateAdded.Value = DateTimePicker.MinimumDateTime;
            }
            tbUserRating.Text = Convert.ToString(t.UserStarRating);
            grdTags.Rows.Clear();
            foreach (string tag in t.Tags)
            {
                grdTags.Rows.Add(tag);
            }

            //File Details
            cbAspectRatio.Text = t.AspectRatio;
            grdSubtitles.Rows.Clear();
            foreach (string l in t.Subtitles)
            {
                grdSubtitles.Rows.Add(l);
            }
            grdAudioTracks.Rows.Clear();
            foreach (string a in t.AudioTracks)
            {
                grdAudioTracks.Rows.Add(a);
            }
        }



        public void UpdateTitleFromUI(Title t)
        {
            // Movie Locations
            t.FrontCoverPath = tbFrontCover.Text;
            t.BackCoverPath = tbBackCover.Text;
            t.FileLocation = tbFileLocation.Text;

            // Movie Details
            t.Name = tbName.Text.Trim();
            t.Synopsis = tbSummary.Text.Trim();
            t.ReleaseDate = dtpReleaseDate.Value;
            t.ParentalRating = cbRating.Text.Trim();
            t.ParentalRatingReason = tbRatingReason.Text.Trim();
            t.CountryOfOrigin = tbCountry.Text.Trim();
            t.Studio = tbStudio.Text.Trim();
            int RunTime;
            if (int.TryParse(tbRunTime.Text, out RunTime) == false)
            {
                RunTime = 0;
            }
            t.Runtime = RunTime;

            // Credits
            t.Directors.Clear();
            foreach (DataGridViewRow row in grdDirectors.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.Directors.Add(new Person((string)row.Cells[0].Value));
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
            t.Actors.Clear();
            t.ActingRoles.Clear();
            foreach (DataGridViewRow row in grdTags.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[1].Value == null) return;
                t.Actors.Add(new Person((string)row.Cells[0].Value));
                t.ActingRoles.Add((string)row.Cells[0].Value, (string)row.Cells[1].Value);
            }

            // Your Movie Details
            if (tbSortName.Text.Trim().Length > 0)
                t.SortName = tbSortName.Text.Trim();
            else
                t.SortName = t.Name;
            t.OriginalName = tbOriginalName.Text.Trim();
            t.DateAdded = dtpDateAdded.Value;
            int UserStarRating;
            if (int.TryParse(tbUserRating.Text, out UserStarRating) == false)
            {
                UserStarRating = 0;
            }
            t.UserStarRating = UserStarRating;
            t.Tags.Clear();
            foreach (DataGridViewRow row in grdTags.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.Tags.Add((string)row.Cells[0].Value);
            }

            // File Details
            t.AspectRatio = cbAspectRatio.Text.Trim();
           
            t.AudioTracks.Clear();
            foreach (DataGridViewRow row in grdSubtitles.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.AudioTracks.Add((string)row.Cells[0].Value);
            }

            t.Subtitles.Clear();
            foreach (DataGridViewRow row in grdAudioTracks.Rows)
            {
                if (row.Cells[0].Value == null) return;
                t.Subtitles.Add((string)row.Cells[0].Value);
            }
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

        private void CoverButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = @"C:\";
            ofd.Filter = @"JPG files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (sender.Equals(button2))
                    {
                        tbFrontCover.Text = ofd.FileName;
                        //pbFrontCover.ImageLocation = ofd.FileName;
                    }
                    else
                    {
                        tbBackCover.Text = ofd.FileName;
                        //pbBackCover.ImageLocation = ofd.FileName;
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void tbFrontCover_TextChanged(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(((TextBox) sender).Text))
            {
                if (sender.Equals(tbFrontCover))
                {
                    pbFrontCover.ImageLocation = ((TextBox) sender).Text;
                }
                else
                {
                    pbBackCover.ImageLocation = ((TextBox) sender).Text;
                }
            }
        }


    }
}
