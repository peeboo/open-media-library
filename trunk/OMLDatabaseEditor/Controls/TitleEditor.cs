using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

using OMLEngine;

namespace OMLDatabaseEditor.Controls
{
    public partial class TitleEditor : UserControl
    {
        public delegate void TitleChangeEventHandler(object sender, EventArgs e);
        public delegate void TitleNameChangeEventHandler(object sender, TitleNameChangedEventArgs e);
        public delegate void SavedEventHandler(object sender, EventArgs e);

        public event TitleChangeEventHandler TitleChanged;
        public event TitleNameChangeEventHandler TitleNameChanged;
        public event SavedEventHandler SavedTitle;

        private bool _isLoading = false;

        public enum TitleStatus { Normal, UnsavedChanges }

        private TitleStatus _status;
        public TitleStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private Title _dvdTitle;
        public Title EditedTitle
        {
            get { return _dvdTitle; }
        }

        public TitleEditor()
        {
            InitializeComponent();
        }

        public void LoadDVD(Title dvd)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, dvd);
                stream.Position = 0;
                _dvdTitle = (Title)formatter.Deserialize(stream);
            }
            _isLoading = true;
            titleSource.DataSource = _dvdTitle;
            Status = TitleStatus.Normal;
            imageWatcherFront.Path = Path.GetDirectoryName(_dvdTitle.FrontCoverPath);
            imageWatcherFront.Filter = "F*.jpg";
            _isLoading = false;
        }

        public void RefreshEditor()
        {
            titleSource.ResetCurrentItem();
        }

        public void ClearEditor()
        {
            _dvdTitle = null;
            titleSource.DataSource = typeof(Title);
            Status = TitleStatus.Normal;
        }

        private void _titleChanged(EventArgs e)
        {
            if (TitleChanged != null && Status != TitleStatus.UnsavedChanges && !_isLoading)
            {
                _status = TitleStatus.UnsavedChanges;
                TitleChanged(this, e);
            }
        }

        private void _titleNameChanged(TitleNameChangedEventArgs e)
        {
            if (TitleNameChanged != null && !_isLoading)
            {
                _status = TitleStatus.UnsavedChanges;
                TitleNameChanged(this, e);
            }
        }

        private void TitleChanges(object sender, EventArgs e)
        {
            _titleChanged(EventArgs.Empty);
        }

        private void TitleNameChanges(object sender, EventArgs e)
        {
            _titleNameChanged(new TitleNameChangedEventArgs(txtName.Text));
        }

        private void EditList(string name, List<string> listToEdit)
        {
            ListEditor editor = new ListEditor(name, listToEdit);
            List<string> original = listToEdit.ToList<string>();
            editor.ShowDialog();
            if (listToEdit.Union(original).Count<string>() != original.Count)
            {
                TitleChanges(null, null);
            }
        }

        private void TogglePeople(int selectedPeople)
        {
            if (EditedTitle != null)
            {
                switch (selectedPeople)
                {
                    case 0: //Directors
                        lbPeople.DataSource = EditedTitle.Directors;
                        break;
                    case 1: //Writers
                        lbPeople.DataSource = EditedTitle.Writers;
                        break;
                    case 2: //Producers
                        lbPeople.DataSource = EditedTitle.Producers;
                        break;
                }
            }
        }

        private void EditPicture(string imagePath)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            // Don't use the system's shell
            //psi.UseShellExecute = false;
            psi.UseShellExecute = true;
            psi.FileName = imagePath;
            if (psi.Verbs.Contains<string>("edit"))
            {
                psi.Verb = "edit";
                Process.Start(psi);
                return;
            }
            MessageBox.Show("No editor found for this image file type", "Edit Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnDisks_Click(object sender, EventArgs e)
        {
            if (EditedTitle != null)
            {
                DiskEditorFrm diskEditor = new DiskEditorFrm(EditedTitle.Disks);
                diskEditor.ShowDialog();
                _titleChanged(EventArgs.Empty);
            }
        }

        private void pbCovers_MouseClick(object sender, MouseEventArgs e)
        {
            if (_dvdTitle == null) return;
            if (e.Button == MouseButtons.Right)
            {
                contextImage.Tag = sender;
                contextImage.Show(sender as PictureBox, e.Location);
            }
        }

        private void selectImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openCoverFile.ShowDialog() == DialogResult.OK)
            {
                PictureBox pb = contextImage.Tag as PictureBox;
                if (pb.Name.Contains("Front"))
                    _dvdTitle.CopyFrontCoverFromFile(openCoverFile.FileName, false);
                else
                    _dvdTitle.CopyBackCoverFromFile(openCoverFile.FileName, false);
                titleSource.ResetCurrentItem();
            }
        }

        private void btnGenres_Click(object sender, EventArgs e)
        {
            if (_dvdTitle != null)
                EditList("Genres", _dvdTitle.Genres);
        }

        private void btnTags_Click(object sender, EventArgs e)
        {
            if (_dvdTitle != null)
                EditList("Tags", _dvdTitle.Tags);
        }

        private void btnExtras_Click(object sender, EventArgs e)
        {
            if (_dvdTitle != null)
                EditList("Extra Features", _dvdTitle.ExtraFeatures);
        }

        private void btnTracks_Click(object sender, EventArgs e)
        {
            if (_dvdTitle != null)
                EditList("Audio Tracks", _dvdTitle.AudioTracks);
        }

        private void btnSubtitles_Click(object sender, EventArgs e)
        {
            if (_dvdTitle != null)
                EditList("Subtitles", _dvdTitle.Subtitles);
        }

        private void btnTrailers_Click(object sender, EventArgs e)
        {
            if (_dvdTitle != null)
                EditList("Trailers", _dvdTitle.Trailers);
        }

        private void rgPeople_Properties_SelectedIndexChanged(object sender, EventArgs e)
        {
            TogglePeople(rgPeople.SelectedIndex);
        }

        private void titleSource_CurrentChanged(object sender, EventArgs e)
        {
            TogglePeople(rgPeople.SelectedIndex);
        }

        private void pbFrontCover_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            EditPicture(pb.ImageLocation);
        }

        private void imageWatcherFront_Changed(object sender, FileSystemEventArgs e)
        {
            // Update scaled menu image
            if (e.ChangeType == WatcherChangeTypes.Changed && e.Name == Path.GetFileName(_dvdTitle.FrontCoverPath))
                _dvdTitle.BuildResizedMenuImage();
        }
    }

    public class TitleNameChangedEventArgs : EventArgs
    {
        private string _newName = string.Empty;
        public string NewName
        {
            get { return _newName; }
        }

        public TitleNameChangedEventArgs(string newName)
        {
            _newName = newName;
        }
    }
}
