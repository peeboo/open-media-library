using System;
using System.IO;
using System.Windows.Forms;

using OMLEngine;
using OMLGetDVDInfo;

namespace OMLDatabaseEditor.Controls
{
    public partial class ExtenderExtraOptions : Form
    {
        public MediaSource MediaSource { get; set; }
        public Disk Disk
        {
            get { return MediaSource.Disk; }
            set
            {
                MediaSource = new MediaSource(value);
                this.lblDVDName.Text = value.Name + " @ " + new DirectoryInfo(value.VIDEO_TS_Parent).Name;

                DVDTitle curT = MediaSource.DVDTitle;
                this.cbTitle.Items.Add(string.Format("Default ({0})", curT));
                this.cbTitle.SelectedIndex = 0;
                foreach (DVDTitle title in MediaSource.DVDDiskInfo.Titles)
                    if (title.Duration > TimeSpan.FromSeconds(5))
                    {
                        this.cbTitle.Items.Add(title);
                        if (MediaSource.Title != null && title == curT)
                            this.cbTitle.SelectedItem = title;
                    }

                //if (MediaSource.AudioStream != null)
                //    this.cbAudioTracks.SelectedItem = ;
                //if (MediaSource.Subtitle != null)
                //    this.cbSubtitles.SelectedItem = ;
                if (MediaSource.StartChapter != null)
                    txtStartChapters.Text = MediaSource.StartChapter.ToString();
                if (MediaSource.EndChapter != null)
                    txtEndChapters.Text = MediaSource.EndChapter.ToString();
            }
        }

        public ExtenderExtraOptions()
        {
            InitializeComponent();
        }

        private void cbTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            DVDTitle title = this.cbTitle.SelectedItem as DVDTitle;

            MediaSource.Title = title == null ? (int?)null : title.TitleNumber;
            MediaSource.AudioStream = null;
            MediaSource.Subtitle = null;
            MediaSource.StartChapter = MediaSource.EndChapter = null;

            if (title == null)
                title = this.MediaSource.DVDTitle;

            cbAudioTracks.Items.Clear();
            cbAudioTracks.Items.Add(string.Format("Default ({0})", title.AudioTracks[0]));
            foreach (DVDAudioTrack at in title.AudioTracks)
                cbAudioTracks.Items.Add(at);
            cbAudioTracks.SelectedIndex = 0;
            cbAudioTracks.Enabled = cbAudioTracks.Items.Count > 2;

            cbSubtitles.Items.Clear();
            cbSubtitles.Items.Add("None");
            foreach (DVDSubtitle st in title.Subtitles)
                cbSubtitles.Items.Add(st);
            cbSubtitles.SelectedIndex = 0;
            cbSubtitles.Enabled = cbSubtitles.Items.Count > 1;

            txtStartChapters.Text = txtEndChapters.Text = string.Empty;
            txtStartChapters.Enabled = txtEndChapters.Enabled = title.Chapters.Count > 1;
            lblChapters.Text = title.Chapters.Count.ToString();
        }

        private void cbAudioTracks_SelectedIndexChanged(object sender, EventArgs e)
        {
            MediaSource.AudioStream = MediaSource.GetAudioSteam(this.cbAudioTracks.SelectedItem as DVDAudioTrack);
        }

        private void cbSubtitles_SelectedIndexChanged(object sender, EventArgs e)
        {
            MediaSource.Subtitle = MediaSource.GetSubTitle(this.cbSubtitles.SelectedItem as DVDSubtitle);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int val;
            MediaSource.StartChapter = int.TryParse(txtStartChapters.Text, out val) ? (int?)val : null;
            MediaSource.EndChapter = int.TryParse(txtEndChapters.Text, out val) ? (int?)val : null;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
