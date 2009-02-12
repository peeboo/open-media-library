using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

using OMLEngine;
using OMLSDK;

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
        private List<String> listPersona = new List<string>();
        private List<String> filterPersona = new List<string>();

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
            LoadBackdrops();
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

        public bool IsNew()
        {
            return MainEditor._titleCollection.GetTitleById(_dvdTitle.InternalItemID) == null;
        }

        public void SaveChanges()
        {
            titleSource.CurrencyManager.Refresh();
            if (IsNew())
                MainEditor._titleCollection.Add(_dvdTitle);
            else
                MainEditor._titleCollection.Replace(_dvdTitle);
            MainEditor._titleCollection.saveTitleCollection();
            ClearEditor();
        }

        private void _titleChanged(EventArgs e)
        {
            if (TitleChanged != null && Status != TitleStatus.UnsavedChanges && !_isLoading)
            {
                Status = TitleStatus.UnsavedChanges;
                TitleChanged(this, e);
            }
        }

        private void _titleNameChanged(TitleNameChangedEventArgs e)
        {
            if (TitleNameChanged != null && !_isLoading)
            {
                Status = TitleStatus.UnsavedChanges;
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

        private void LoadBackdrops()
        {
            tblBackdrops.Controls.Clear();
            if (_dvdTitle.BasePath() != null)
            {
                if (Directory.Exists(_dvdTitle.BackDropFolder))
                {
                    string[] images = Directory.GetFiles(_dvdTitle.BackDropFolder);
                    foreach (string image in images)
                    {
                        PictureBox pb = new PictureBox();
                        pb.ImageLocation = image;
                        pb.Height = 150;
                        pb.Dock = DockStyle.Fill;
                        pb.SizeMode = PictureBoxSizeMode.Zoom;
                        //if (pb.Image == null) continue;
                        tblBackdrops.Controls.Add(pb);
                    }
                }
            }
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
                listPersona.Clear();
                switch (selectedPeople)
                {
                    case 0: //Directors
                        lbPeople.DataSource = EditedTitle.Directors;
                        lbPeople.DisplayMember = "full_name";
                        lbPeople.Tag = "Directors";
                        listPersona.AddRange(MainEditor._titleCollection.GetAllDirectors.ToArray());
                        listPersona.Sort();
                        break;
                    case 1: //Writers
                        lbPeople.DataSource = EditedTitle.Writers;
                        lbPeople.DisplayMember = "full_name";
                        lbPeople.Tag = "Writers";
                        listPersona.AddRange(MainEditor._titleCollection.GetAllWriters.ToArray());
                        listPersona.Sort();
                        break;
                    case 2: //Producers
                        lbPeople.DataSource = EditedTitle.Producers;
                        lbPeople.DisplayMember = "";
                        lbPeople.Tag = "Producers";
                        listPersona.AddRange(MainEditor._titleCollection.GetAllProducers.ToArray());
                        listPersona.Sort();
                        break;
                    case 3: //Actors
                        lbPeople.DataSource = EditedTitle.ActingRolesBinding;
                        lbPeople.Tag = "ActingRoles";
                        lbPeople.DisplayMember = "Display";
                        listPersona.AddRange(MainEditor._titleCollection.GetAllActors.ToArray());
                        listPersona.Sort();
                        break;
                    case 4: //Non-Actors
                        lbPeople.DataSource = EditedTitle.NonActingRolesBinding;
                        lbPeople.Tag = "NonActingRoles";
                        lbPeople.DisplayMember = "Display";
                        break;
                }
                filterPersona.Clear();
                filterPersona.AddRange(listPersona);
                lbcPersona.DataSource = filterPersona;
                lbcPersona.Refresh();
            }
        }

        private void EditPicture(string imagePath)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            // Don't use the system's shell
            //psi.UseShellExecute = false;
            psi.UseShellExecute = true;
            psi.FileName = imagePath;
            if (psi.Verbs.Contains("Edit"))
            {
                psi.Verb = "Edit";
                Process.Start(psi);
                return;
            }
            else if (psi.Verbs.Contains("edit"))
            {
                psi.Verb = "edit";
                Process.Start(psi);
                return;
            }
            XtraMessageBox.Show("No editor found for this image file type", "Edit Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnDisks_Click(object sender, EventArgs e)
        {
            if (EditedTitle != null)
            {
                DiskEditorFrm diskEditor = new DiskEditorFrm(EditedTitle.Disks);
                diskEditor.ShowDialog();
                TitleChanges(null, EventArgs.Empty);
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
            if (_dvdTitle != null)
            {
                if (e.ChangeType == WatcherChangeTypes.Changed && e.Name == Path.GetFileName(_dvdTitle.FrontCoverPath))
                    _dvdTitle.BuildResizedMenuImage();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // delete selected item from source list
            foreach (object item in lbPeople.SelectedItems)
            {
                switch (rgPeople.SelectedIndex)
                {
                    case 0: //Directors
                        EditedTitle.Directors.Remove(item as Person);
                        break;
                    case 1: //Writers
                        EditedTitle.Writers.Remove(item as Person);
                        break;
                    case 2: //Producers
                        EditedTitle.Producers.Remove(item as string);
                        break;
                    case 3: //Actors
                        EditedTitle.ActingRoles.Remove(((Role)item).PersonName);
                        break;
                    case 4: //Non-Actors
                        EditedTitle.NonActingRoles.Remove(((Role)item).PersonName);
                        break;
                }
            }
            TitleChanges(null, EventArgs.Empty);
            TogglePeople(rgPeople.SelectedIndex);
        }

        private void lbPeople_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbPeople.SelectedIndices.Count == 0)
            {
                deleteToolStripMenuItem.Visible = false;
            }
            else
            {
                deleteToolStripMenuItem.Visible = true;
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PersonEditorFrm frmPerson = new PersonEditorFrm(rgPeople.SelectedIndex == 3 || rgPeople.SelectedIndex == 4);
            frmPerson.PersonList = listPersona;
            if (frmPerson.ShowDialog() == DialogResult.OK)
            {
                switch (rgPeople.SelectedIndex)
                {
                    case 0: //Directors
                        EditedTitle.AddDirector(new Person(frmPerson.PersonName));
                        break;
                    case 1: //Writers
                        EditedTitle.AddWriter(new Person(frmPerson.PersonName));
                        break;
                    case 2: //Producers
                        EditedTitle.AddProducer(frmPerson.PersonName);
                        break;
                    case 3: //Actors
                        EditedTitle.AddActingRole(frmPerson.PersonName, frmPerson.PersonRole);
                        break;
                    case 4: //Non-Actors
                        EditedTitle.AddNonActingRole(frmPerson.PersonName, frmPerson.PersonRole);
                        break;
                }
                TitleChanges(null, EventArgs.Empty);
                TogglePeople(rgPeople.SelectedIndex);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (e.Page == tpPeople)
            {
                TogglePeople(rgPeople.SelectedIndex);
            }
        }

        public void SetMRULists()
        {
            if (Properties.Settings.Default.gbUseMPAAList)
            {
                // MaskBox is a hidden property
                // It is explained on the DevExpress Website at:
                //
                // http://www.devexpress.com/Support/Center/p/Q181219.aspx
                //
                teParentalRating.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                teParentalRating.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                teParentalRating.MaskBox.AutoCompleteCustomSource.AddRange(Properties.Settings.Default.gsMPAARatings.Split('|'));
            }
            else
            {
                teParentalRating.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                teParentalRating.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                teParentalRating.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllParentalRatings.ToArray());
            }

            txtStudio.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtStudio.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtStudio.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllStudios.ToArray());

            txtAspectRatio.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtAspectRatio.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtAspectRatio.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllAspectRatios.ToArray());

            txtCountryOfOrigin.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtCountryOfOrigin.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtCountryOfOrigin.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllCountryofOrigin.ToArray());

            teVideoResolution.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            teVideoResolution.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            teVideoResolution.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllVideoResolutions.ToArray());

            teVideoStandard.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            teVideoStandard.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            teVideoStandard.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllVideoStandards.ToArray());

            teImporter.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            teImporter.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            teImporter.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllImporterSources.ToArray());

        }

        private void lbPeople_DoubleClick(object sender, EventArgs e)
        {
            List<String> listToEdit = new List<string>();
            listToEdit.AddRange(MainEditor._titleCollection.GetAllActors.ToArray());
            listToEdit.Sort();
            lbcPersona.DataSource = listToEdit;
            //String name = "Actors";
            //ListEditor editor = new ListEditor(name, listToEdit);
            //List<string> original = listToEdit.ToList<string>();
            //editor.ShowDialog();
            //if (listToEdit.Union(original).Count<string>() != original.Count)
            //{
            //    TitleChanges(null, null);
            //}
        }

        private void buttonEdit1_TextChanged(object sender, EventArgs e)
        {
            sFilter = beFilter.Text.ToLower();
            filterPersona.Clear();
            if (String.IsNullOrEmpty(sFilter))
            {
                filterPersona.AddRange(listPersona);
            }
            else
            {
                filterPersona.AddRange(listPersona.FindAll(StartsWithFilter));
            }
            lbcPersona.Refresh();
        }

        private static String sFilter;
        private static bool StartsWithFilter(String s)
        {
            if (s.ToLower().StartsWith(sFilter))
            {
                return true;
            } else {
                return false;
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
                else if (pb.Name.Contains("Backdrop"))
                    _dvdTitle.BackDropImage = openCoverFile.FileName;
                else
                    _dvdTitle.CopyBackCoverFromFile(openCoverFile.FileName, false);
                titleSource.ResetCurrentItem();
            }
        }

        private void deleteImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox pb = contextImage.Tag as PictureBox;
            if (pb.Name.Contains("Front"))
            {
                TitleCollection.DeleteImageNoException(_dvdTitle.FrontCoverPath);
                TitleCollection.DeleteImageNoException(_dvdTitle.FrontCoverMenuPath);
                _dvdTitle.FrontCoverMenuPath = string.Empty;
                _dvdTitle.FrontCoverPath = string.Empty;
            }
            else
            {
                TitleCollection.DeleteImageNoException(_dvdTitle.BackCoverPath);
                _dvdTitle.BackCoverPath = string.Empty;
            }
            titleSource.ResetCurrentItem();
        }

        private void updateFromMetadataToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PictureBox pb = contextImage.Tag as PictureBox;
            Cursor = Cursors.WaitCursor;
            string propertyName = pb.DataBindings[0].BindingMemberInfo.BindingMember;
            MetadataSelect mdSelect = new MetadataSelect(_dvdTitle, propertyName, PropertyTypeEnum.Image);
            if (mdSelect.ShowDialog() == DialogResult.OK)
                TitleChanges(null, EventArgs.Empty);
            PropertyInfo pInfo = _dvdTitle.GetType().GetProperty(propertyName);
            pb.ImageLocation = (string)pInfo.GetValue(_dvdTitle, null);
            Cursor = Cursors.Default;
        }

        private void field_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.D)
            {
                if (sender is Control)
                {
                    Cursor = Cursors.WaitCursor;
                    BaseEdit ctrl = sender as BaseEdit;
                    string propertyName;
                    if (ctrl != null)
                        propertyName = ctrl.DataBindings[0].BindingMemberInfo.BindingMember;
                    else
                        propertyName = (string)(sender as Control).Tag;
                    MetadataSelect mdSelect = null;
                    PropertyTypeEnum propType = PropertyTypeEnum.String;
                    if (sender is SpinEdit)
                        propType = PropertyTypeEnum.Number;
                    else if (sender is PictureBox)
                        propType = PropertyTypeEnum.Image;
                    else if (sender is DateEdit)
                        propType = PropertyTypeEnum.Date;
                    else if (sender is ListBoxControl)
                        propType = PropertyTypeEnum.List;
                    mdSelect = new MetadataSelect(_dvdTitle, propertyName, propType);
                    if (mdSelect.ShowDialog() == DialogResult.OK)
                        TitleChanges(null, EventArgs.Empty);
                    if (ctrl != null) (ctrl.BindingManager as CurrencyManager).Refresh();
                    if (sender is ListBoxControl)
                        TogglePeople(rgPeople.SelectedIndex);
                    Cursor = Cursors.Default;
                }
            }
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
