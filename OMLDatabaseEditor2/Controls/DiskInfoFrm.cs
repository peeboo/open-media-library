using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OMLEngine;
using DevExpress.XtraEditors;

namespace OMLDatabaseEditor.Controls
{
    public partial class DiskInfoFrm : DevExpress.XtraEditors.XtraForm
    {
        Disk _disk;
        Title _title;

        public DiskInfoFrm(Title title, Disk disk)
        {
            _disk = disk;
            _title = title;
            InitializeComponent();
            //OMLEngine.DiskInfo di = new OMLEngine.DiskInfo(_disk.Name, _disk.Path, _disk.Format);

            TreeNode topNode = new TreeNode();
            topNode.Name = _disk.Path;
            topNode.Text = _disk.Path;
            tvDiskInfo.Nodes.Add(topNode);

            int FeatureNo = 0;
            int VideoStreamNo = 0;
            int AudioStreamNo = 0;

            foreach (DIFeature df in _disk.DiskFeatures)
            {
                TreeNode featureNode = new TreeNode();
                featureNode.Name = "Feature," + FeatureNo.ToString() + "," + df.Name;
                featureNode.Text = df.Name;
                topNode.Nodes.Add(featureNode);

                if (df.Filesize != 0) { featureNode.Nodes.Add("Size : " + df.Filesize.ToString()); }

                if (df.Format != null)
                {
                    if (df.Format.ToString() != "") { featureNode.Nodes.Add("Format : " + df.Format); }
                }

                featureNode.Nodes.Add(string.Format("Duration : {0:00}:{1:00}:{2:00}",Math.Floor(df.Duration.TotalHours),df.Duration.Minutes,df.Duration.Seconds));

                if (df.Chapters.Count > 0)
                {
                    TreeNode chapters = new TreeNode();
                    chapters.Name = "Chapters";
                    chapters.Text = "Chapters";
                    featureNode.Nodes.Add(chapters);

                    foreach (DIChapter ch in df.Chapters)
                    {
                        chapters.Nodes.Add("Chapter " + ch.ChapterNumber.ToString() +
                            string.Format("  :  Start Time {0:00}:{1:00}:{2:00}", Math.Floor(ch.StartTime.TotalHours), ch.StartTime.Minutes, ch.StartTime.Seconds) +
                            string.Format("  :  Duration {0:00}:{1:00}:{2:00}", Math.Floor(ch.Duration.TotalHours), ch.Duration.Minutes, ch.Duration.Seconds));
                    }
                }

                TreeNode video = new TreeNode();
                video.Name = "Video Streams";
                video.Text = "Video Streams";
                featureNode.Nodes.Add(video);
                foreach (DIVideoStream vd in df.VideoStreams)
                {
                    TreeNode videostream = new TreeNode();
                    videostream.Name = "VideoStream," + FeatureNo.ToString() + "," + VideoStreamNo.ToString() + "," + vd.Name;
                    videostream.Text = vd.Name;
                    video.Nodes.Add(videostream);

                    if (vd.TitleID != 0) { videostream.Nodes.Add(videostream.Name, "Title ID : " + vd.TitleID); }
                    if (vd.AspectRatio != "") { videostream.Nodes.Add(videostream.Name, "Aspect Ratio : " + vd.AspectRatio); }
                    if (vd.Bitrate != 0) { videostream.Nodes.Add(videostream.Name, "Bitrate : " + vd.Bitrate.ToString() + "kbps"); }
                    if (vd.Format != null)
                    {
                        if (vd.Format.ToString() != "") { videostream.Nodes.Add(videostream.Name, "Format : " + vd.Format); }
                    }
                    if (vd.FrameRate != 0) { videostream.Nodes.Add(videostream.Name, "Framerate : " + vd.FrameRate.ToString() + "fps"); }
                    if (vd.Resolution.Width != 0) { videostream.Nodes.Add(videostream.Name, "Picture Size : " + vd.Resolution.Width.ToString() + "x" + vd.Resolution.Height.ToString()); }
                    if (vd.ScanType != "") { videostream.Nodes.Add(videostream.Name, "Scan Type : " + vd.ScanType); }
                    VideoStreamNo++;
                }

                TreeNode audio = new TreeNode();
                audio.Name = "Audio Streams";
                audio.Text = "Audio Streams";
                featureNode.Nodes.Add(audio);
                foreach (DIAudioStream ad in df.AudioStreams)
                {
                    TreeNode audiostream = new TreeNode();
                    audiostream.Name = ad.Name;
                    audiostream.Text = ad.Name;
                    audio.Nodes.Add(audiostream);
                    if (ad.TrackNo != 0) { audiostream.Nodes.Add("Track No : " + ad.TrackNo.ToString()); }
                    if (ad.AudioID != 0) { audiostream.Nodes.Add("Audio ID : " + ad.AudioID.ToString()); }

                    // Build enocding string
                    StringBuilder str = new StringBuilder();
                    str.Append("Encoding : " + ad.Encoding.ToString());
                    if (ad.EncodingProfile != DIAudioEncodingProfile.Undefined)
                        str.Append(" " + ad.EncodingProfile.ToString());

                    if (ad.SubFormat != "")
                    {
                        str.Append(" in " + ad.SubFormat);
                    }
                    audiostream.Nodes.Add(str.ToString());

                    if (ad.Language != "") { audiostream.Nodes.Add("Language : " + ad.Language.ToString()); }
                    if (ad.SampleFreq != 0) { audiostream.Nodes.Add("Sample Frequency : " + ad.SampleFreq.ToString()); }
                    if (ad.Channels != 0) { audiostream.Nodes.Add("Channels : " + ad.Channels.ToString()); }
                    if (ad.Bitrate != 0) { audiostream.Nodes.Add("Bitrate : " + ad.Bitrate.ToString() + "kbps (" + ad.BitrateMode + ")"); }
                    AudioStreamNo++;
                 }

                FeatureNo++;
            }
        }

        private void sbSetAsMainFeature_Click(object sender, EventArgs e)
        {
            try
            {
                int Feature;
                int VideoStream;
                int AudioStream;

                string selectedNodeName = tvDiskInfo.SelectedNode.Name;
                string[] pars = selectedNodeName.Split(',');
                switch (pars[0])
                {
                    case "Feature":
                        Feature = Convert.ToInt32(pars[1]);
                        VideoStream = 0;
                        AudioStream = 0;
                        break;
                    case "VideoStream":
                        Feature = Convert.ToInt32(pars[1]);
                        VideoStream = Convert.ToInt32(pars[2]);
                        AudioStream = 0;
                        break;
                    case "AudioStream":
                        Feature = Convert.ToInt32(pars[1]);
                        VideoStream = 0;
                        AudioStream = Convert.ToInt32(pars[2]);
                        break;
                    default:
                        Feature = 0;
                        VideoStream = 0;
                        AudioStream = 0;
                        break;
                }


                _disk.MainFeatureXRes = _disk.DiskFeatures[Feature].VideoStreams[VideoStream].Resolution.Width;
                _disk.MainFeatureYRes = _disk.DiskFeatures[Feature].VideoStreams[VideoStream].Resolution.Height;
                _disk.MainFeatureAspectRatio = _disk.DiskFeatures[Feature].VideoStreams[VideoStream].AspectRatio;
                _disk.MainFeatureFPS = _disk.DiskFeatures[Feature].VideoStreams[VideoStream].FrameRate;
                _disk.MainFeatureLength = _disk.DiskFeatures[Feature].Duration.Hours * 60 + _disk.DiskFeatures[Feature].Duration.Minutes;

                if (OMLEngine.Settings.OMLSettings.ScanDiskRollInfoToTitle)
                {
                    _title.VideoResolution = _disk.MainFeatureXRes.ToString() + "x" + _disk.MainFeatureYRes.ToString();
                    _title.AspectRatio = _disk.MainFeatureAspectRatio;
                    //_disk.MainFeatureFPS = _disk.DiskFeatures[Feature].VideoStreams[VideoStream].FrameRate;
                    _title.Runtime = _disk.MainFeatureLength ?? 0;
                }
            }
            catch
            {
            }
        }
    }
}
