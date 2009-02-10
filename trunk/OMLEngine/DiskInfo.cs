using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLGetDVDInfo;
using System.Drawing;
using System.IO;

namespace OMLEngine
{
    public class DIFeature
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<DIAudioStream> AudioStreams { get; set; }
        public List<DIVideoStream> VideoStreams { get; set; }
        public List<DIChapter> Chapters { get; set; }
        public int Filesize { get; set; }
        public TimeSpan Duration { get; set; }
        public int OverallBitRate { get; set; }
        public string Format { get; set; }
        public override string ToString()
        {
            return Name;
        }

        public DIFeature()
        {
            AudioStreams = new List<DIAudioStream>();
            VideoStreams = new List<DIVideoStream>();
            Chapters = new List<DIChapter>();
        }

    }

    public class DIAudioStream
    {
        public string Name { get; set; }
        public bool IsSurroundSound { get { return (Channels ?? 0) > 2; } }
        public string Language { get { return MediaLanguage.LanguageNameForId(LanguageID); }}
        public string LanguageID { get; set; }
        public int? AudioID { get; set; }
        public int? Channels { get; set; }
        public int? TrackNo { get; set; }
        public int Bitrate { get; set; }
        public string BitrateMode { get; set; }
        public string Format { get; set; }
        public string SubFormat { get; set; }
        public AudioExtension Extension { get; set; }
        public int SampleFreq { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public class DIVideoStream
    {
        public string Name { get; set; }
        public int Bitrate { get; set; }
        public float FrameRate { get; set; }
        public Size Resolution { get; set; }
        public string Format { get; set; }
        public string AspectRatio { get; set; }
        public string ScanType { get; set; }
        public int TitleID { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public class DIChapter
    {
        public int ChapterNumber { get; set; }
        public int FPS { get; set; }
        public TimeSpan StartTime;
        public TimeSpan Duration;
        public string _Duration { get { return Duration.ToString(); } set { Duration = TimeSpan.Parse(value); } }
        public long TotalFrames { get; set; }
        public int Frames { get; set; }
    }

    public class DiskInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public VideoFormat Format { get; set; }
        public List<DIFeature> DiskFeatures { get; set; }



        public DiskInfo(string name, string path, VideoFormat format)
        {
            Name = name;
            Path = path;
            Format = format;
            DiskFeatures = new List<DIFeature>();

            IdentifyMediaType(name, path, format);
        }

        private void IdentifyMediaType(string name, string path, VideoFormat format)
        {
            // Now identify file type.
            switch (format)
            {
                // Image Files
                case VideoFormat.B5T : // BlindWrite image
                case VideoFormat.B6T : // BlindWrite image
                case VideoFormat.BIN : // using an image loader lib and load/play this as a DVD
                case VideoFormat.BWT : // BlindWrite image
                case VideoFormat.CCD : // CloneCD image
                case VideoFormat.CDI : // DiscJuggler Image
                case VideoFormat.CUE : // cue sheet
                case VideoFormat.ISO : // Standard ISO image
                case VideoFormat.ISZ : // Compressed ISO image
                case VideoFormat.MDF : // using an image loader lib and load/play this as a DVD
                case VideoFormat.IMG : // using an image loader lib and load/play this as a DVD
                case VideoFormat.NRG : // Nero image
                case VideoFormat.PDI : // Instant CD/DVD image
                    // Try to mount image then find media
                    break;

                // Physical disks
                case VideoFormat.BLURAY : // detect which drive supports this and request the disc
                case VideoFormat.HDDVD : // detect which drive supports this and request the disc
                    break;

                case VideoFormat.DVD : // detect which drive supports this and request the disc
                    QueryDVD();
                    break;


                // Offline disks
                case VideoFormat.OFFLINEBLURAY : // detect which drive supports this and request the disc
                case VideoFormat.OFFLINEDVD : // detect which drive supports this and request the disc
                case VideoFormat.OFFLINEHDDVD : // detect which drive supports this and request the disc
                    break;

                // Playlists
                case VideoFormat.ASX : // like WPL
                case VideoFormat.WPL : // playlist file?
                    break;

                // Video files
                case VideoFormat.ASF : // WMV style
                case VideoFormat.AVC : // AVC H264
                case VideoFormat.AVI : // DivX, Xvid, etc
                case VideoFormat.DVRMS : // MPG
                case VideoFormat.H264 : // AVC OR MP4
                case VideoFormat.IFO : // Online DVD
                case VideoFormat.MDS : // Media Descriptor file
                case VideoFormat.MKV : // Likely h264
                case VideoFormat.MOV : // Quicktime
                case VideoFormat.MPG :
                case VideoFormat.MPEG :
                case VideoFormat.MP4 : // DivX, AVC, or H264
                case VideoFormat.OGM : // Similar to MKV
                case VideoFormat.TS : // MPEG2
                case VideoFormat.UIF :
                case VideoFormat.WMV :
                case VideoFormat.VOB : // MPEG2
                case VideoFormat.WVX : // wtf is this?
                case VideoFormat.WTV : // new dvr format in vista (introduced in the tv pack 2008)
                case VideoFormat.M2TS : // mpeg2 transport stream (moved, since it got inserted in the midd
                    QueryMediaFile(path);
                    break;

                case VideoFormat.URL : // this is used for online content (such as streaming trailers)
                case VideoFormat.UNKNOWN :
                    break;
            }
        }

        private void QueryDVD()
        {
            // Check to see if any IFO files exist
            if (Directory.GetFiles(Path,"*.ifo").Length == 0)
            {
                // No IFO Files Found
                // Try to see if there is a VIDEO_TS folder
                if (Directory.Exists(System.IO.Path.Combine(Path, "VIDEO_TS")))
                {
                    Path = System.IO.Path.Combine(Path, "VIDEO_TS"); 
                    if (Directory.GetFiles(Path, "*.ifo").Length == 0)
                    {
                        return;
                    }
                }
            }
            DVDDiskInfo ddi = DVDDiskInfo.ParseDVD(Path);
            foreach(DVDTitle dt in ddi.Titles)
            {
                DIFeature difeature = new DIFeature();

                DIVideoStream divideostream = new DIVideoStream();

                difeature.Duration = dt.Duration;
                difeature.Name = dt.ToString();

                divideostream.AspectRatio = dt.AspectRatio.ToString();
                divideostream.Name = dt.ToString();
                divideostream.TitleID = dt.TitleNumber;
                divideostream.FrameRate = (float) dt.FPS;
                divideostream.Resolution = dt.Resolution;
                

                TimeSpan starttime = new TimeSpan(0);

                foreach (DVDChapter ch in dt.Chapters)
                {
                    DIChapter dichapter = new DIChapter();
                    dichapter.ChapterNumber = ch.ChapterNumber;
                    dichapter.StartTime = starttime;
                    dichapter.Duration = ch.Duration;
                    dichapter.Frames = ch.Frames;
                    dichapter.TotalFrames = ch.TotalFrames;
                    difeature.Chapters.Add(dichapter);
                    starttime += ch.Duration;
                }

                foreach (DVDAudioTrack at in dt.AudioTracks)
                {
                    DIAudioStream diaudiostream = new DIAudioStream();
                    diaudiostream.Name = at.ToString();
                    diaudiostream.Format = at.Format.ToString();
                    diaudiostream.LanguageID = at.LanguageID;
                    diaudiostream.SampleFreq = at.Frequency;
                    diaudiostream.AudioID = at.ID;
                    diaudiostream.Bitrate = at.Bitrate;
                    diaudiostream.Channels = at.Channels;
                    diaudiostream.TrackNo = at.TrackNumber;
                    diaudiostream.SubFormat = at.SubFormat;
                    difeature.AudioStreams.Add(diaudiostream);
                }

                difeature.VideoStreams.Add(divideostream);
                DiskFeatures.Add(difeature);
            }

        }

        private void QueryBluRay()
        {
        }

        private void QueryMediaFile(string path)
        {
            MediaInfoLib.MediaInfo MI = new MediaInfoLib.MediaInfo();

            MI.Open(path);
            int i;

            DIFeature difeature = new DIFeature();

            difeature.Filesize = (int) (ToInt64(MI.Get(0, 0, "FileSize")) / 1024 / 1024); // MB
            try
            {
                difeature.Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(MI.Get(0, 0, "Duration"))); // Minutes
            }
            catch
            {
            }
            difeature.OverallBitRate = ToInt32(MI.Get(0, 0, "OverallBitRate")) / 1000; // kbps
            difeature.Format = MI.Get(0, 0, "Format");
            difeature.Name = path;

            int videostreamscount = ToInt32(MI.Get(MediaInfoLib.StreamKind.Video, 0, "StreamCount"));
            Console.WriteLine("\nVideo Stream Count : " + videostreamscount.ToString());

            try
            {
                for (i = 0; i < videostreamscount; i++)
                {
                    DIVideoStream divideostream = new DIVideoStream();
                    divideostream.FrameRate = (float)Convert.ToDouble(MI.Get(MediaInfoLib.StreamKind.Video, i, "FrameRate"));
                    divideostream.Bitrate = ToInt32(MI.Get(MediaInfoLib.StreamKind.Video, i, "BitRate")) / 1000;
                    divideostream.Resolution = new Size(
                        ToInt32(MI.Get(MediaInfoLib.StreamKind.Video, i, "Width")),
                        ToInt32(MI.Get(MediaInfoLib.StreamKind.Video, i, "Height")));
                    divideostream.Format = MI.Get(MediaInfoLib.StreamKind.Video, i, "Format");
                    divideostream.AspectRatio = MI.Get(MediaInfoLib.StreamKind.Video, i, "DisplayAspectRatio/String");
                    divideostream.ScanType = MI.Get(MediaInfoLib.StreamKind.Video, i, "ScanType");
                    divideostream.TitleID = ToInt32(MI.Get(MediaInfoLib.StreamKind.Video, i, "ID"));
                    divideostream.Name = MI.Get(MediaInfoLib.StreamKind.Video, i, "Title");

                    difeature.VideoStreams.Add(divideostream);
                }
            }
             catch (Exception ex)
            {
            }

            try
            {
                int audiostreamscount = ToInt32(MI.Get(MediaInfoLib.StreamKind.Audio, 0, "StreamCount"));
                Console.WriteLine("\nAudio Stream Count : " + audiostreamscount.ToString());

                for (i = 0; i < audiostreamscount; i++)
                {
                    DIAudioStream diaudiostream = new DIAudioStream();
                    diaudiostream.Format = MI.Get(MediaInfoLib.StreamKind.Audio, i, "Format");
                    diaudiostream.LanguageID = MI.Get(MediaInfoLib.StreamKind.Audio, i, "Language");
                    diaudiostream.BitrateMode = MI.Get(MediaInfoLib.StreamKind.Audio, i, "BitRate_Mode");
                    diaudiostream.Bitrate = ToInt32(MI.Get(MediaInfoLib.StreamKind.Audio, i, "BitRate")) / 1000;
                    diaudiostream.Channels = ToInt32(MI.Get(MediaInfoLib.StreamKind.Audio, i, "Channels"));
                    diaudiostream.AudioID = ToInt32(MI.Get(MediaInfoLib.StreamKind.Audio, i, "ID"));
                    diaudiostream.Name = diaudiostream.Language;
                    if (diaudiostream.Channels > 0) { diaudiostream.Name = diaudiostream.Name + " " + diaudiostream.Channels.ToString() + "ch"; }
                    difeature.AudioStreams.Add(diaudiostream);
                }
            }
            catch (Exception ex)
            {
            }

            Console.WriteLine(MI.Count_Get(MediaInfoLib.StreamKind.Audio));

            Console.WriteLine(MI.Get(MediaInfoLib.StreamKind.General, 0, "AudioCount"));

            Console.WriteLine(MI.Get(MediaInfoLib.StreamKind.Audio, 0, "StreamCount"));

            DiskFeatures.Add(difeature);

            MI.Close();
        }

        private int ToInt32(string s)
        {
            if (s == null) { return 0; }
            try
            {
                return Convert.ToInt32(s);
            }
            catch
            {
                return 0;
            }
        }

        private long ToInt64(string s)
        {
            if (s == null) { return 0; }
            try
            {
                return Convert.ToInt64(s);
            }
            catch
            {
                return 0;
            }
        }
    }
}
