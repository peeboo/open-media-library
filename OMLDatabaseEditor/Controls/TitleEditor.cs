﻿using DevExpress.XtraEditors;
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
                        listPersona.AddRange(MainEditor._titleCollection.GetAllDirectors.ToArray());
                        listPersona.Sort();
                        break;
                    case 1: //Writers
                        lbPeople.DataSource = EditedTitle.Writers;
                        lbPeople.DisplayMember = "full_name";
                        listPersona.AddRange(MainEditor._titleCollection.GetAllWriters.ToArray());
                        listPersona.Sort();
                        break;
                    case 2: //Producers
                        lbPeople.DataSource = EditedTitle.Producers;
                        lbPeople.DisplayMember = "";
                        listPersona.AddRange(MainEditor._titleCollection.GetAllProducers.ToArray());
                        listPersona.Sort();
                        break;
                    case 3: //Actors
                        lbPeople.DataSource = EditedTitle.ActingRolesBinding;
                        lbPeople.DisplayMember = "Display";
                        listPersona.AddRange(MainEditor._titleCollection.GetAllActors.ToArray());
                        listPersona.Sort();
                        break;
                    case 4: //Non-Actors
                        lbPeople.DataSource = EditedTitle.NonActingRolesBinding;
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
            if (psi.Verbs.Contains<string>("Edit"))
            {
                psi.Verb = "Edit";
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

            txtStudio.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtStudio.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtStudio.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllStudios.ToArray());

            txtAspectRatio.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtAspectRatio.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtAspectRatio.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllAspectRatios.ToArray());

            txtCountryOfOrigin.MaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtCountryOfOrigin.MaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtCountryOfOrigin.MaskBox.AutoCompleteCustomSource.AddRange(MainEditor._titleCollection.GetAllCountryofOrigin.ToArray());

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

        private void updateFromMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            List<PluginServices.AvailablePlugin> plugins = new List<PluginServices.AvailablePlugin>();
            string path = FileSystemWalker.PluginsDirectory;
            plugins = PluginServices.FindPlugins(path, PluginTypes.MetadataPlugin);
            IOMLMetadataPlugin objPlugin;
            // Loop through available plugins, creating instances and add them
            if (plugins != null)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                foreach (PluginServices.AvailablePlugin oPlugin in plugins)
                {
                    objPlugin = (IOMLMetadataPlugin)PluginServices.CreateInstance(oPlugin);
                    objPlugin.Initialize(new Dictionary<string, string>());
                    objPlugin.SearchForMovie(txtName.Text);
                    Title title = objPlugin.GetBestMatch();
                    if (title != null)
                    {
                        data[objPlugin.PluginName] = title.Synopsis;
                    }
                    plugins = null;
                }
                TableLayoutPanel tblMetadata = new TableLayoutPanel();
                pnlMetadata.Controls.Add(tblMetadata);
                tblMetadata.Dock = DockStyle.Fill;
                tblMetadata.ColumnCount = data.Keys.Count;
                tblMetadata.RowCount = 2;
                for (int i = 0; i < data.Keys.Count; i++)
                {
                    tblMetadata.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (float)1 / data.Keys.Count));
                }
                int column = 0;
                foreach (string key in data.Keys)
                {
                    LabelControl lblField = new LabelControl();
                    lblField.Name = "lblField" + column;
                    lblField.Text = key;
                    lblField.Dock = DockStyle.Fill;
                    tblMetadata.Controls.Add(lblField, column, 0);
                    MemoEdit txtValue = new MemoEdit();
                    txtValue.Name = "txtValue" + column;
                    txtValue.DoubleClick += new EventHandler(txtValue_DoubleClick);
                    txtValue.Text = data[key];
                    txtValue.Dock = DockStyle.Fill;
                    tblMetadata.Controls.Add(txtValue, column, 1);
                    column++;
                }
            }
            pnlMetadata.Visible = true;
            Cursor = Cursors.Default;
        }

        void txtValue_DoubleClick(object sender, EventArgs e)
        {
            txtSynposis.EditValue = (sender as MemoEdit).Text;
            txtSynposis.BindingManager.EndCurrentEdit();
            pnlMetadata.Controls.Clear();
            pnlMetadata.Visible = false;
        }

        private void deleteImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox pb = contextImage.Tag as PictureBox;
            if (pb.Name.Contains("Front"))
            {
                DeleteImageNoException(_dvdTitle.FrontCoverPath);
                DeleteImageNoException(_dvdTitle.FrontCoverMenuPath);
                _dvdTitle.FrontCoverMenuPath = string.Empty;
                _dvdTitle.FrontCoverPath = string.Empty;
            }
            else
            {
                DeleteImageNoException(_dvdTitle.BackCoverPath);
                _dvdTitle.BackCoverPath = string.Empty;
            }
            titleSource.ResetCurrentItem();
        }

        private void DeleteImageNoException(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch(Exception e)
            {
                OMLEngine.Utilities.DebugLine("[TitleEditor] DeleteImageNoException(" + path + ") : failed deleting image because " + e.Message);
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
