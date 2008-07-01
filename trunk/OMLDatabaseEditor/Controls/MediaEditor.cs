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
        public delegate void TitleChangeEventHandler(object sender, EventArgs e);
        public delegate void TitleNameChangeEventHandler(object sender, EventArgs e);
        public delegate void SavedEventHandler(object sender, EventArgs e);

        public event TitleChangeEventHandler TitleChanged;
        public event TitleNameChangeEventHandler TitleNameChanged;
        public event SavedEventHandler SavedTitle;

        private TitleStatus _status;
        private int _itemID = 0;
        private string _titleName = "";
        
        public enum TitleStatus {Initial, UnsavedChanges, Saved}

        public int itemID 
        {
            get { return _itemID; }
        }
             
        public TitleStatus Status 
        {
            get { return _status; }
        }

        public string TitleName
        {
            get { return _titleName; }
        }

        private void _titleChanged(EventArgs e)
        {
            if (TitleChanged != null && Status != TitleStatus.UnsavedChanges)
            {
                _status = TitleStatus.UnsavedChanges;
                TitleChanged(this, e);
            }
        }

        private void _titleNameChanged(EventArgs e)
        {
            if (TitleNameChanged != null)
            {
                _titleName = tbName.Text;
                _status = TitleStatus.UnsavedChanges;
                TitleNameChanged(this, e);
            }
        }

        private void _savedTitle(EventArgs e)
        {
            if (SavedTitle != null && Status == TitleStatus.UnsavedChanges)
            {
                _status = TitleStatus.Saved;
                SavedTitle(this, e);
            }
        }

        public MediaEditor()
        {
            InitializeComponent();
        }

        public void LoadTitle(Title t)
        {
            UpdateUIFromTitle(t);
        }

        public void SaveToTitle(Title t)
        {
            UpdateTitleFromUI(t);
            _savedTitle(EventArgs.Empty);
        }

        private void UpdateUIFromTitle(Title t)
        {
            _itemID = t.InternalItemID;

            // Movie Locations
            pbFrontCover.Image = ReadImageFromFile(t.FrontCoverPath);
            tbFrontCover.Text = t.FrontCoverPath;
            pbBackCover.Image = ReadImageFromFile(t.BackCoverPath);
            tbBackCover.Text = t.BackCoverPath;
            foreach (Disk d in t.Disks)
            {
                lstDisks.Items.Add(d);
            }
            //tbFileLocation.Text = t.FileLocation;

            // Other
            tbUPC.Text = t.UPC;
            tbWebsite.Text = t.OfficialWebsiteURL;
            tbWatchedCount.Text = t.WatchedCount.ToString();
            dtpDateAdded.Value = t.DateAdded;
            tbUserRating.Text = t.UserStarRating.ToString();
            tbSortName.Text = t.SortName;

            // Movie Details
            tbName.Text = t.Name;
            _titleName = t.Name;
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
            tbRunTime.Text = Convert.ToString(t.Runtime);
            tbOriginalName.Text = t.OriginalName;
            tbCountry.Text = t.CountryOfOrigin;
            tbStudio.Text = t.Studio;


            // Categores
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

            grdProducers.Rows.Clear();
            foreach (string p in t.Producers)
            {
                grdProducers.Rows.Add(p.ToString());
            }

            grdActors.Rows.Clear();
            foreach (KeyValuePair<string, string> role in t.ActingRoles)
            {
                grdActors.Rows.Add(role.Key, role.Value);
            }

            grdNonActors.Rows.Clear();
            foreach (KeyValuePair<string, string> role in t.NonActingRoles)
            {
                grdNonActors.Rows.Add(role.Key, role.Value);
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
            
            try
            {
                dtpDateAdded.Value = t.DateAdded;
            }
            catch
            {
                dtpDateAdded.Value = DateTimePicker.MinimumDateTime;
            }
            tbUserRating.Text = Convert.ToString(t.UserStarRating);
            

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

        private void UpdateTitleFromUI(Title t)
        {
            int tmpint;

            // Movie Locations
            t.FrontCoverPath = tbFrontCover.Text;
            t.BackCoverPath = tbBackCover.Text;
            t.Disks.Clear();
            foreach (Disk d in lstDisks.Items)
            {
                t.Disks.Add(d); 
            }
            
            //t.FileLocation = tbFileLocation.Text;

            // Other
            t.UPC = tbUPC.Text;
            t.OfficialWebsiteURL = tbWebsite.Text;
            if (int.TryParse(tbWatchedCount.Text, out tmpint) == false)
            {
                tmpint = 0;
            }
            t.WatchedCount = tmpint;
            t.DateAdded = dtpDateAdded.Value;
            if (int.TryParse(tbUserRating.Text, out tmpint) == false)
            {
                tmpint = 0;
            }
            t.UserStarRating = tmpint;
            t.SortName = tbSortName.Text;

            // Movie Details
            t.Name = tbName.Text.Trim();
            t.Synopsis = tbSummary.Text.Trim();
            t.ReleaseDate = dtpReleaseDate.Value;
            t.ParentalRating = cbRating.Text.Trim();
            t.ParentalRatingReason = tbRatingReason.Text.Trim();
            if (int.TryParse(tbRunTime.Text, out tmpint) == false)
            {
                tmpint = 0;
            }
            t.Runtime = tmpint;
            t.OriginalName = tbOriginalName.Text.Trim();
            t.CountryOfOrigin = tbCountry.Text.Trim();
            t.Studio = tbStudio.Text.Trim();


            // Categores
            t.Genres.Clear();
            foreach (DataGridViewRow row in grdGenres.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.Genres.Add((string)row.Cells[0].Value);
                }
            }

            t.Tags.Clear();
            foreach (DataGridViewRow row in grdTags.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.Tags.Add((string)row.Cells[0].Value);
                }
            }

            // Credits
            t.Directors.Clear();
            foreach (DataGridViewRow row in grdDirectors.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.Directors.Add(new Person((string)row.Cells[0].Value));
                }
            }

            t.Writers.Clear();
            foreach (DataGridViewRow row in grdWriters.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.Writers.Add(new Person((string)row.Cells[0].Value));
                }
            }

            t.Producers.Clear();
            foreach (DataGridViewRow row in grdProducers.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.Producers.Add(row.Cells[0].Value);
                }
            }

            t.ActingRoles.Clear();
            foreach (DataGridViewRow row in grdActors.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    t.ActingRoles.Add((string)row.Cells[0].Value, (string)row.Cells[1].Value);
                }
                
            }

            t.NonActingRoles.Clear();
            foreach (DataGridViewRow row in grdNonActors.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {

                    t.NonActingRoles.Add((string)row.Cells[0].Value, (string)row.Cells[1].Value);
                }

            }

            // Your Movie Details
            if (tbSortName.Text.Trim().Length > 0)
                t.SortName = tbSortName.Text.Trim();
            else
                t.SortName = t.Name;

            t.DateAdded = dtpDateAdded.Value;
            int UserStarRating;
            if (int.TryParse(tbUserRating.Text, out UserStarRating) == false)
            {
                UserStarRating = 0;
            }
            t.UserStarRating = UserStarRating;
            
            // File Details
            t.AspectRatio = cbAspectRatio.Text.Trim();
           
            t.AudioTracks.Clear();
            foreach (DataGridViewRow row in grdSubtitles.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.AudioTracks.Add((string)row.Cells[0].Value);
                }
            }

            t.Subtitles.Clear();
            foreach (DataGridViewRow row in grdAudioTracks.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.Subtitles.Add((string)row.Cells[0].Value);
                }
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
                Utilities.DebugLine(ex.ToString());
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
                    if (sender.Equals(button2) || sender.Equals(pbFrontCover)) 
                    { 
                        tbFrontCover.Text = ofd.FileName; 
                        //pbFrontCover.ImageLocation = ofd.FileName; 
                    } else { 
                        tbBackCover.Text = ofd.FileName; 
                        //pbBackCover.ImageLocation = ofd.FileName; 
                    } 
                } 
                catch (Exception ex) 
                {
                    Utilities.DebugLine(ex.ToString());
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

        private void button3_Click(object sender, EventArgs e)
        {
            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = @"C:\";
            //ofd.CheckPathExists = true;
            //ofd.CheckFileExists = true;
            //ofd.RestoreDirectory = true;
            //if (ofd.ShowDialog() == DialogResult.OK)
            //{
            //    try
            //    {
            //        tbFileLocation.Text = ofd.FileName;
            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}

            DiskEditor dlg = new DiskEditor();
            dlg.FileName = "Disc " + lstDisks.Items.Count + 1;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                lstDisks.Items.Add(
                    new Disk(
                        dlg.FileName, 
                        dlg.Path,
                        (VideoFormat)Enum.Parse(
                            typeof(VideoFormat), 
                            Path.GetExtension(dlg.Path).ToUpper().Substring(1).Replace("-",""),
                            true)));
            }

        }

        private void TitleChanges(object sender, EventArgs e)
        {
            _titleChanged(EventArgs.Empty);
        }

        private void grd_CellChanges(object sender, DataGridViewCellEventArgs e)
        {
            _titleChanged(EventArgs.Empty);
        }

        private void TitleNameChanges(Object sender, EventArgs e)
        {
            _titleNameChanged(EventArgs.Empty);
        }

        private void lstDisks_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateButtonState();            
        }

        private void ValidateButtonState()
        {
            bool buttonstate = !(lstDisks.SelectedIndex < 0);
            this.btnDiskEdit.Enabled = buttonstate;
            this.btnDisksRemove.Enabled = buttonstate;
        }

        private void MediaEditor_Load(object sender, EventArgs e)
        {
            ValidateButtonState();
        }

        private void btnDisksRemove_Click(object sender, EventArgs e)
        {
            lstDisks.Items.RemoveAt(lstDisks.SelectedIndex);
        }

        private void btnDiskEdit_Click(object sender, EventArgs e)
        {
            DiskEditor dlg = new DiskEditor();
            dlg.Path = ((Disk)lstDisks.SelectedItem).Path;
            dlg.FileName = ((Disk)lstDisks.SelectedItem).Name;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int currentselection = lstDisks.SelectedIndex;
                lstDisks.Items.RemoveAt(currentselection);                    
                lstDisks.Items.Insert(currentselection, 
                    new Disk(
                        dlg.FileName,
                        dlg.Path,
                        (VideoFormat)Enum.Parse(
                            typeof(VideoFormat),
                            Path.GetExtension(dlg.Path).ToUpper().Substring(1),
                            true)));
            }
        }

    }
}
