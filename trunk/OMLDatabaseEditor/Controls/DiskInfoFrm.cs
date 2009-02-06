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
        public DiskInfoFrm(Disk _disk)
        {
            InitializeComponent();
            OMLEngine.DiskInfo di = new OMLEngine.DiskInfo(_disk.Name, _disk.Path, _disk.Format);

            TreeNode topNode = new TreeNode();
            topNode.Name = di.Path;
            topNode.Text = di.Path;
            tvDiskInfo.Nodes.Add(topNode);
            
            foreach (DIFeature df in di.DiskFeatures)
            {
                TreeNode featureNode = new TreeNode();
                featureNode.Name = df.Name;
                featureNode.Text = df.Name;
                topNode.Nodes.Add(featureNode);

                if (df.Filesize != 0) { featureNode.Nodes.Add("Size : " + df.Filesize.ToString()); }

                if (df.Format != null)
                {
                    if (df.Format.ToString() != "") { featureNode.Nodes.Add("Format : " + df.Format); }
                }

                featureNode.Nodes.Add(string.Format("Duration : {0:00}:{1:00}:{2:00}",df.Duration.TotalHours,df.Duration.Minutes,df.Duration.Seconds));


                TreeNode video = new TreeNode();
                video.Name = "Video Streams";
                video.Text = "Video Streams";
                featureNode.Nodes.Add(video);
                foreach (DIVideoStream vd in df.VideoStreams)
                {
                    TreeNode videostream = new TreeNode();
                    videostream.Name = vd.Name;
                    videostream.Text = vd.Name;
                    video.Nodes.Add(videostream);
                    if (vd.TitleID != 0) { videostream.Nodes.Add("Title ID : " + vd.TitleID); }
                    if (vd.AspectRatio != "") { videostream.Nodes.Add("Aspect Ratio : " + vd.AspectRatio); }
                    if (vd.Bitrate != 0) { videostream.Nodes.Add("Bitrate : " + vd.Bitrate.ToString() + "kbps"); }
                    if (vd.Format != null)
                    {
                        if (vd.Format.ToString() != "") { videostream.Nodes.Add("Format : " + vd.Format); }
                    }
                    if (vd.FrameRate != 0) { videostream.Nodes.Add("Framerate : " + vd.FrameRate.ToString() + "fps"); }
                    if (vd.Resolution.Width != 0) { videostream.Nodes.Add("Picture Size : " + vd.Resolution.Width.ToString() + "x" + vd.Resolution.Height.ToString()); }
                    if (vd.ScanType != "") { videostream.Nodes.Add("Scan Type : " + vd.ScanType); }
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
                    if (ad.Format != "")
                    {
                        if (ad.SubFormat != "")
                        {
                            audiostream.Nodes.Add("Format : " + ad.Format + " in " + ad.SubFormat);
                        }
                        else
                        {
                            audiostream.Nodes.Add("Format : " + ad.Format);
                        }
                    }
                    if (ad.Language != "") { audiostream.Nodes.Add("Language : " + ad.Language.ToString()); }
                    if (ad.SampleFreq != 0) { audiostream.Nodes.Add("Sample Frequency : " + ad.SampleFreq.ToString()); }
                    if (ad.Channels != 0) { audiostream.Nodes.Add("Channels : " + ad.Channels.ToString()); }
                    if (ad.Bitrate != 0) { audiostream.Nodes.Add("Bitrate : " + ad.Bitrate.ToString() + "kbps (" + ad.BitrateMode + ")"); }
                 }
            }
        }
    }
}