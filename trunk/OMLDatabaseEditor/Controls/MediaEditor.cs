using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OMLEngine;
using System.IO;
using System.Diagnostics;
using System.Net;

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
        private string _FrontCoverMenu = "";

        private Title _currentTitle;

        public Title CurrentTitle
        {
            get { return _currentTitle; }
        }

        public enum TitleStatus {Initial, UnsavedChanges, Saved}

        public int itemID 
        {
            get { return _itemID; }
        }
             
        public TitleStatus Status 
        {
            get { return _status; }
            set { _status = value; }
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
            _currentTitle = t;
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
            _FrontCoverMenu = t.FrontCoverMenuPath;
            pbBackCover.Image = ReadImageFromFile(t.BackCoverPath);
            tbBackCover.Text = t.BackCoverPath;

            lstDisks.Items.Clear();
            foreach (Disk d in t.Disks)
            {
                lstDisks.Items.Add(d);
            }
            //tbFileLocation.Text = t.FileLocation;

            // Other
            tbUPC.Text = t.UPC;
            tbWebsite.Text = t.OfficialWebsiteURL;
            tbWatchedCount.Text = t.WatchedCount.ToString();
            if( t.DateAdded < DateTimePicker.MinimumDateTime )
                dtpDateAdded.Value = DateTimePicker.MinimumDateTime;
            else
                dtpDateAdded.Value =  t.DateAdded;

            tbUserRating.Text = t.UserStarRating.ToString();
            tbSortName.Text = t.SortName;

            // Movie Details
            tbName.Text = t.Name;
            _titleName = t.Name;
            tbSummary.Text = t.Synopsis;

            if (t.ReleaseDate < DateTimePicker.MinimumDateTime)
                dtpReleaseDate.Value = DateTimePicker.MinimumDateTime;
            else
                dtpReleaseDate.Value = t.ReleaseDate;

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
            
            //File Details
            cbAspectRatio.Text = t.AspectRatio;
            cbVideoStandard.Text = t.VideoStandard;
            //cbVideoFormat.Text = t.VideoFormat;
            tbVideoResolution.Text = t.VideoResolution;

            grdAudioTracks.Rows.Clear();
            foreach (string a in t.AudioTracks)
            {
                grdAudioTracks.Rows.Add(a);
            }

            grdSubtitles.Rows.Clear();
            foreach (string s in t.Subtitles)
            {
                grdSubtitles.Rows.Add(s);
            }

            grdExtraFeatures.Rows.Clear();
            foreach (string ef in t.ExtraFeatures)
            {
                grdExtraFeatures.Rows.Add(ef);
            }

            grdVideoDetails.Rows.Clear();
            grdVideoDetails.Rows.Add(t.VideoDetails);

            //foreach (string vd in t.VideoDetails)
            //{
            //    grdVideoDetails.Rows.Add(vd);
            //}
        }

        private void UpdateTitleFromUI(Title t)
        {
            int tmpint;

            // Movie Locations
            t.FrontCoverPath = tbFrontCover.Text;
            t.FrontCoverMenuPath = _FrontCoverMenu;
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
                    t.Producers.Add((string)row.Cells[0].Value);
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

            // File Details
            t.AspectRatio = cbAspectRatio.Text.Trim();
            t.VideoStandard = cbVideoStandard.Text;
           // t.VideoFormat = cbVideoFormat.Text;
            t.VideoResolution = tbVideoResolution.Text;
           
            t.AudioTracks.Clear();
            foreach (DataGridViewRow row in grdAudioTracks.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.AudioTracks.Add((string)row.Cells[0].Value);
                }
            }

            t.Subtitles.Clear();
            foreach (DataGridViewRow row in grdSubtitles.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.Subtitles.Add((string)row.Cells[0].Value);
                }
            }

            t.ExtraFeatures.Clear();
            foreach (DataGridViewRow row in grdExtraFeatures.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.ExtraFeatures.Add((string)row.Cells[0].Value);
                }
            }
            foreach (DataGridViewRow row in grdVideoDetails.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    t.VideoDetails = (string)row.Cells[0].Value;
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

                            g.Dispose();
                            return bmp2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[MediaEditor] " + ex.ToString());
            }

            return null;
        }

        private void CoverButton_Click(object sender, EventArgs e)
        {
            string initialDirectory = ""; 
            if (CurrentTitle != null)
            {
                if (CurrentTitle.Disks != null && CurrentTitle.Disks.Count > 0 && !String.IsNullOrEmpty(CurrentTitle.Disks[0].Path))
                {
                    initialDirectory = Path.GetDirectoryName(CurrentTitle.Disks[0].Path);
                    if (initialDirectory == null) initialDirectory = "";
                }
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = initialDirectory;
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
                    } else { 
                        tbBackCover.Text = ofd.FileName; 
                    } 
                } 
                catch (Exception ex) 
                {
                    Utilities.DebugLine("[MediaEditor] " + ex.ToString());
                } 
            }
        }

        private void tbFrontCover_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(((TextBox)sender).Text))
                {
                    if (sender.Equals(tbFrontCover))
                    {
                        pbFrontCover.ImageLocation = ((TextBox)sender).Text;
                        // if its a new cover, remake the thumbnail image.
                        Utilities.DebugLine("DBEditor: Front Cover text changed, checking if thumbnail location is different");
                        if (pbFrontCover.ImageLocation != _FrontCoverMenu)
                        {
                            using (Image menuCoverArtImage = Utilities.ScaleImageByHeight(ReadImageFromFile(pbFrontCover.ImageLocation), 200))
                            {
                                string img_path = FileSystemWalker.ImageDirectory +
                                                  @"\MF" + _itemID + ".jpg";
                                menuCoverArtImage.Save(img_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                                Utilities.DebugLine("DBEditor: Cover Image Thumbnail created filename: " + img_path);
                                _FrontCoverMenu = img_path;
                            }
                        }
                    }
                    else
                    {
                        pbBackCover.ImageLocation = ((TextBox)sender).Text;
                    }
                }
                _titleChanged(EventArgs.Empty);
            }
            catch (Exception ex)
            {
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DiskEditor dlg = new DiskEditor();
            dlg.FileName = "Disc " + (lstDisks.Items.Count + 1);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                lstDisks.Items.Add(
                    new Disk(
                        dlg.FileName, 
                        dlg.Path,
                        dlg.Format
                    )
                );

                _titleChanged(EventArgs.Empty);
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
            _titleChanged(EventArgs.Empty);
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
            try
            {
                if (lstDisks.SelectedItem != null)
                {
                    DiskEditor dlg = new DiskEditor();
                    dlg.Path = ((Disk)lstDisks.SelectedItem).Path;
                    dlg.FileName = ((Disk)lstDisks.SelectedItem).Name;
                    dlg.Format = ((Disk)lstDisks.SelectedItem).Format;

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        int currentselection = lstDisks.SelectedIndex;
                        lstDisks.Items.RemoveAt(currentselection);

                        lstDisks.Items.Insert(currentselection,
                            new Disk(
                                dlg.FileName,
                                dlg.Path,
                                dlg.Format
                            )
                        );

                        _titleChanged(EventArgs.Empty);
                    }
                }
            }
            catch
            {
            }
        }


        private void MediaEditor_DragOver(object sender, DragEventArgs e)
        {
            string[] formats = e.Data.GetFormats();
            string debug = "";
            foreach (string f in formats)
            {
                debug = debug + ";" + f;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Html))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }

        public void SetPicture(PictureBox pb, string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    if (pb.Image != null)
                    {
                        pb.Image.Dispose();
                        pb.Image = null;
                    }

                    // if we don't do this GDI will still keep the file locked
                    // and it cannot be deleted unless we close the app
                    pb.Image = ReadImageFromFile(fileName);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private string GetStringFromDragDropData(IDataObject data, string format)
        {
            string value = "";
            if (data.GetDataPresent(format))
            {
                object param = data.GetData(format);
                if (param.GetType() == typeof(MemoryStream))
                {
                    MemoryStream ms = param as MemoryStream;
                    StreamReader sr = new StreamReader(ms);
                    value = sr.ReadToEnd();

                    // remove the '/0' at the end
                    if (value.EndsWith("\0"))
                    {
                        value = value.Replace("\0", "");
                    }
                }
                else if (param.GetType() == typeof(string))
                {
                    value = param as string;
                }
            }

            return value;
        }

        static private bool DownloadImage(string url, string fileName)
        {
            WebClient web = new WebClient();
            try
            {
                if (File.Exists(fileName)) File.Delete(fileName);
                web.DownloadFile(url, fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void MediaEditor_DragDrop(object sender, DragEventArgs e)
        {
            string fileName = "";
            try
            {
                Point pointTranslatedToFrontCover = pbFrontCover.PointToClient(new Point(e.X, e.Y));
                Point pointTranslatedToBackCover = pbBackCover.PointToClient(new Point(e.X, e.Y));


                Array dataArray = (Array)e.Data.GetData(DataFormats.FileDrop);
                String[] strs = e.Data.GetData(DataFormats.FileDrop) as String[];
                if (dataArray != null)
                {
                    fileName = dataArray.GetValue(0).ToString();
                    if (File.Exists(fileName))
                    {
                        //MessageBox.Show("x = " + e.X + " y = " + e.Y + "cx = " + pbFrontCover.ClientRectangle.ToString());
                        if (pbBackCover.ClientRectangle.Contains(pointTranslatedToBackCover.X, pointTranslatedToBackCover.Y))
                        {
                            if (CurrentTitle != null)
                            {
                                CurrentTitle.CopyBackCoverFromFile(fileName, false);

                                SetPicture(pbBackCover, CurrentTitle.BackCoverPath);
                                tbBackCover.Text = CurrentTitle.BackCoverPath;
                            }
                        }
                        else //if (pbFrontCover.ClientRectangle.Contains(pointTranslatedToFrontCover.X, pointTranslatedToFrontCover.Y))
                        {
                            if (CurrentTitle != null)
                            {
                                CurrentTitle.CopyFrontCoverFromFile(fileName, false);

                                SetPicture(pbFrontCover, CurrentTitle.FrontCoverPath);
                                tbFrontCover.Text = CurrentTitle.FrontCoverPath;
                            }
                        }
                    }
                }
                else if (e.Data.GetDataPresent(DataFormats.Html))
                {
                    string url = GetStringFromDragDropData(e.Data, "UniformResourceLocator");
                    if (!String.IsNullOrEmpty(url))
                    {
                        if (pbBackCover.ClientRectangle.Contains(pointTranslatedToBackCover.X, pointTranslatedToBackCover.Y))
                        {
                            string coverArtFile = CurrentTitle.GetDefaultBackCoverName();
                            if (!String.IsNullOrEmpty(CurrentTitle.BackCoverPath))
                                coverArtFile = CurrentTitle.BackCoverPath;
                            
                            if (DownloadImage(url, coverArtFile))
                            {
                                CurrentTitle.BackCoverPath = coverArtFile;
                                SetPicture(pbBackCover, CurrentTitle.BackCoverPath);
                                tbBackCover.Text = CurrentTitle.BackCoverPath;
                            }
                        }
                        else
                        {
                            string coverArtFile = CurrentTitle.GetDefaultFrontCoverName();
                            
                            if (!String.IsNullOrEmpty(CurrentTitle.FrontCoverPath))
                                coverArtFile = CurrentTitle.FrontCoverPath;

                            if (DownloadImage(url, coverArtFile))
                            {
                                CurrentTitle.FrontCoverPath = coverArtFile;
                                CurrentTitle.BuildResizedMenuImage();
                                SetPicture(pbFrontCover, CurrentTitle.FrontCoverPath);
                                tbFrontCover.Text = CurrentTitle.FrontCoverPath;
                            }

                        }
                    }

                    //foreach (string f in formats)
                    //{
                    //    Debug.Print("");
                    //    Debug.Print(f);
                    //    if (e.Data.GetData(f) != null)
                    //        Debug.Print(e.Data.GetData(f).ToString());
                    //    else
                    //        Debug.Print("null");
                    //}

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file. " + ex.Message);
            }
        }

        private void grd_CellChanges(object sender, EventArgs e)
        {
            _titleChanged(EventArgs.Empty);
        }

        private void pbFrontCover_Click(object sender, EventArgs e)
        {

        }

        private void lstDisks_DoubleClick(object sender, EventArgs e)
        {
            btnDiskEdit_Click(sender, e);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
