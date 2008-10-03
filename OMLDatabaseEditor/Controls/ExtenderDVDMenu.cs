using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using OMLEngine;
using OMLGetDVDInfo;

namespace OMLDatabaseEditor.Controls
{
    public partial class ExtenderDVDMenu : Form
    {
        Disk _disk;
        IDictionary<DVDTitle, MediaSource> _menu;
        EventHandler cbEnabledCheckedChanged;

        public Disk Disk
        {
            get { return _disk; }
            set
            {
                _disk = value;
                _menu = new Dictionary<DVDTitle, MediaSource>();
                // TODO: load up _menu from Disk

                this.lblDVDName.Text = value.Name + " @ " + new DirectoryInfo(value.VIDEO_TS_Parent).Name;

                foreach (DVDTitle title in _disk.DVDDiskInfo.Titles)
                    if (title.Duration > TimeSpan.FromSeconds(5) && title.AudioTracks.Count > 0)
                        this.cbTitle.Items.Add(title);
                if (this.cbTitle.Items.Count > 0)
                    this.cbTitle.SelectedIndex = 0;
            }
        }

        public ExtenderDVDMenu()
        {
            InitializeComponent();
            cbEnabledCheckedChanged = new EventHandler(cbEnabled_CheckedChanged);
        }

        private void cbTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbEnabled.CheckedChanged -= cbEnabledCheckedChanged;

            DVDTitle title = (DVDTitle)this.cbTitle.SelectedItem;
            if (title == _disk.DVDDiskInfo.GetMainTitle())
            {
                cbEnabled.Checked = true;
                txtName.Text = lblDVDName.Text;
                txtStartChapters.Text = txtEndChapters.Text = string.Empty;
                FillAudioTrack(title);
                FillSubtitle(title);
                cbEnabled.Enabled = txtName.Enabled = txtStartChapters.Enabled = txtEndChapters.Enabled = false;
                return;
            }

            cbEnabled.Enabled = txtName.Enabled = txtStartChapters.Enabled = txtEndChapters.Enabled = true;

            MediaSource source = _menu.ContainsKey(title) ? _menu[title] : null;

            cbEnabled.Checked = source != null;

            cbEnabled.CheckedChanged += cbEnabledCheckedChanged;

            lblChapters.Text = title.Chapters.Count.ToString();
            if (cbEnabled.Checked)
            {
                txtName.Text = source.Name;
                txtStartChapters.Text = source.StartChapter != null ? source.StartChapter.ToString() : "";
                txtEndChapters.Text = source.EndChapter != null ? source.EndChapter.ToString() : "";
            }
            else
            {
                txtName.Text = "Title " + title.TitleNumber;
                txtStartChapters.Text = txtEndChapters.Text = string.Empty;
            }
            txtStartChapters.Enabled = txtEndChapters.Enabled = title.Chapters.Count > 1;

            FillAudioTrack(title);
            FillSubtitle(title);
        }

        private void FillSubtitle(DVDTitle title)
        {
            txtSubtitles.Text = string.Empty;
            foreach (DVDSubtitle st in title.Subtitles)
                txtSubtitles.Text += ", " + st.LanguageID;
            txtSubtitles.Text = txtSubtitles.Text.TrimStart(',', ' ');
        }

        private void FillAudioTrack(DVDTitle title)
        {
            txtAudioTracks.Text = string.Empty;
            foreach (DVDAudioTrack at in title.AudioTracks)
            {
                txtAudioTracks.Text += ", " + at.LanguageID + " (" + at.Format + "," + at.SubFormat;
                if (at.Extension != AudioExtension.Normal && at.Extension != AudioExtension.Unspecified)
                    txtAudioTracks.Text += ", " + DVDAudioTrack.GetExtension(at.Extension);
                txtAudioTracks.Text += ")";
            }
            txtAudioTracks.Text = txtAudioTracks.Text.TrimStart(',', ' ');
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            DVDTitle title = (DVDTitle)this.cbTitle.SelectedItem;
            if (cbEnabled.Checked)
                _menu[title] = new MediaSource(new Disk(txtName.Text, _disk.Path, _disk.Format)) { Title = title.TitleNumber };
            else
                _menu.Remove(title);
            UpdateMenu();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            DVDTitle title = (DVDTitle)this.cbTitle.SelectedItem;
            MediaSource source = _menu.ContainsKey(title) ? _menu[title] : null;
            if (source == null)
                return;
            source.Disk.Name = txtName.Text;
            UpdateMenu();
        }

        private void txtStartChapters_TextChanged(object sender, EventArgs e)
        {
            DVDTitle title = (DVDTitle)this.cbTitle.SelectedItem;
            MediaSource source = _menu.ContainsKey(title) ? _menu[title] : null;
            if (source == null)
                return;
            int val;
            source.StartChapter = int.TryParse(txtStartChapters.Text, out val) ? (int?)val : null;
            UpdateMenu();
        }

        private void txtEndChapters_TextChanged(object sender, EventArgs e)
        {
            DVDTitle title = (DVDTitle)this.cbTitle.SelectedItem;
            MediaSource source = _menu.ContainsKey(title) ? _menu[title] : null;
            if (source == null)
                return;
            int val;
            source.EndChapter = int.TryParse(txtEndChapters.Text, out val) ? (int?)val : null;
            UpdateMenu();
        }

        private void UpdateMenu()
        {
            lblMenu.Text = _disk.Name + "\n";
            foreach (KeyValuePair<DVDTitle, MediaSource> kv in _menu)
                lblMenu.Text += kv.Value + "\n";
        }
    }
}
