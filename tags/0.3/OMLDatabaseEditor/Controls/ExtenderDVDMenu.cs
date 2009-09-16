using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using OMLEngine;
using OMLGetDVDInfo;
using DirectShowLib;
using DirectShowLib.Dvd;
using System.Diagnostics;

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
                if (_disk.DVDDiskInfo == null)
                    return;

                _menu = new Dictionary<DVDTitle, MediaSource>();
                foreach (MediaSource s in MediaSource.GetSourcesFromOptions(_disk.Path, _disk.ExtraOptions))
                    _menu[s.DVDTitle] = s;

                this.lblDVDName.Text = value.Name + " @ " + new DirectoryInfo(value.VIDEO_TS_Parent).Name;

                foreach (DVDTitle title in _disk.DVDDiskInfo.Titles)
                    if (title.Duration > TimeSpan.FromSeconds(5) && title.AudioTracks.Count > 0)
                        this.cbTitle.Items.Add(title);
                if (this.cbTitle.Items.Count > 0)
                    this.cbTitle.SelectedIndex = 0;

                UpdateMenu();
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
            PlayTitle(title);
            int chapterCount;
            m_dvdInfo.GetNumberOfChapters(title.TitleNumber, out chapterCount);
            Debug.Assert(chapterCount == title.Chapters.Count);

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

        #region -- UI Handlers --
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this._disk.ExtraOptions = MediaSource.ExtraOptionsFromMenu(_menu.Values); 
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
        #endregion

        void FillSubtitle(DVDTitle title)
        {
            txtSubtitles.Text = string.Empty;
            foreach (DVDSubtitle st in title.Subtitles)
                txtSubtitles.Text += ", " + st.LanguageID;
            txtSubtitles.Text = txtSubtitles.Text.TrimStart(',', ' ');
        }

        void FillAudioTrack(DVDTitle title)
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

        void UpdateMenu()
        {
            lblMenu.Text = _disk.Name + "\n";
            foreach (KeyValuePair<DVDTitle, MediaSource> kv in _menu)
                lblMenu.Text += kv.Value.Description + "\n";
        }

        #region -- DVD Preview --
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (m_dvdCtrl == null)
                PreviewInit();
            IDvdCmd cmd;
            //m_dvdCtrl.PlayPrevChapter(DvdCmdFlags.None, out cmd);
            m_dvdCtrl.ShowMenu(DvdMenuId.Title, DvdCmdFlags.SendEvents, out cmd);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (m_dvdCtrl == null)
                PreviewInit();
            IDvdCmd cmd;
            m_dvdCtrl.PlayNextChapter(DvdCmdFlags.None, out cmd);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //m_dvdCtrl.SelectRelativeButton(DvdRelativeButton.Lower);
            m_dvdCtrl.ActivateButton();
        }
        
        void PlayTitle(DVDTitle title)
        {
            if (m_dvdCtrl == null)
                PreviewInit();
            IDvdCmd cmd;
            DvdHMSFTimeCode time = new DvdHMSFTimeCode() { bSeconds = 10 };
            m_dvdCtrl.PlayAtTimeInTitle(title.TitleNumber, time, DvdCmdFlags.SendEvents, out cmd);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DvdPlaybackLocation2 loc;
            m_dvdInfo.GetCurrentLocation(out loc);
            lblInfo.Text = "T:" + loc.TitleNum + ",C:" + loc.ChapterNum + ",T:" + (int)loc.TimeCode.bSeconds;
        }

        IDvdControl2 m_dvdCtrl;
        IDvdInfo2 m_dvdInfo;
        IGraphBuilder m_filterGraph;
        IBaseFilter m_dvdNav;
        IBaseFilter m_renderer;
        void PreviewInit()
        {
            m_dvdNav = (IBaseFilter)new DVDNavigator();
            m_dvdCtrl = m_dvdNav as IDvdControl2;
            int hr = m_dvdCtrl.SetDVDDirectory(Disk.VIDEO_TS);
            DsError.ThrowExceptionForHR(hr);

            m_dvdInfo = m_dvdCtrl as IDvdInfo2;

            m_filterGraph = (IGraphBuilder)new FilterGraph();
            hr = m_filterGraph.AddFilter(m_dvdNav, "DVD Navigator");
            DsError.ThrowExceptionForHR(hr);

            m_renderer = (IBaseFilter)new VideoMixingRenderer9();
            IVMRFilterConfig9 filterConfig = (IVMRFilterConfig9)m_renderer;

            hr = filterConfig.SetRenderingMode(VMR9Mode.Renderless);
            DsError.ThrowExceptionForHR(hr);

            hr = filterConfig.SetNumberOfStreams(1);
            DsError.ThrowExceptionForHR(hr);

            hr = m_filterGraph.AddFilter(m_renderer, "Video Mix 9");
            DsError.ThrowExceptionForHR(hr);

            IPin videoPin;
            hr = m_dvdNav.FindPin("Video", out videoPin);
            DsError.ThrowExceptionForHR(hr);

            IPin audioPin;
            hr = m_dvdNav.FindPin("AC3", out audioPin);
            DsError.ThrowExceptionForHR(hr);

            //hr = m_filterGraph.Render(videoPin);
            //DsError.ThrowExceptionForHR(hr);
            //hr = m_filterGraph.Render(audioPin);
            //DsError.ThrowExceptionForHR(hr);

            //IMediaControl mediaCtrl = (IMediaControl)m_filterGraph;

            //hr = mediaCtrl.Run();
            //DsError.ThrowExceptionForHR(hr);

            //hr = m_dvdCtrl.SetOption(DvdOptionFlag.EnableNonblockingAPIs, true);
            //DsError.ThrowExceptionForHR(hr);
            //m_dvdCtrl.SetOption(DvdOptionFlag.ResetOnStop, true);
        }
        #endregion
    }
}
