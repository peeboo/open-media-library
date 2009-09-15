using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLGetDVDInfo;
using System.Drawing;
using System.IO;

namespace OMLEngine
{
    public enum DIAudioEncoding
    {
        AC3,
        MPEG1,
        MPEG2,
        LPCM,
        DTS,
        Undefined,
        AAC,
        VORBIS,
        WMA2
    }
    public enum DIAudioEncodingProfile
    {
        Undefined,
        Layer1,
        Layer2,
        Layer3,
        L1,
        L2
    }
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
        public DIAudioEncoding Encoding { get; set; }
        public DIAudioEncodingProfile EncodingProfile { get; set; }
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
        public string FrameRateString {
            get
            {
                // Convert the fps & Floor the last digit to avoid any rounding errors
                int fm = (int)Math.Floor((FrameRate * 100)) * 10;
                switch (fm)
                {
                    case 60000: return "60";
                    case 59940: return "60000/1001";
                    case 50000: return "50";
                    case 30000: return "30";
                    case 29970: return "30000/1001";
                    case 25000: return "25";
                    case 24000: return "24";
                    case 23970: return "24000/1001";
                    default: return "";
                }
            }
            set
            {
            }
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
        //public string Name { get; set; }
        //public string Path { get; set; }
        //public VideoFormat Format { get; set; }
        public List<DIFeature> DiskFeatures { get; set; }



        public DiskInfo(string name, string path, VideoFormat format)
        {
            //Name = name;
            //Path = path;
            //Format = format;
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
                    MountImage(name, path);
                    break;

                // Physical disks
                case VideoFormat.BLURAY : // detect which drive supports this and request the disc
                case VideoFormat.HDDVD : // detect which drive supports this and request the disc
                    break;

                case VideoFormat.DVD : // detect which drive supports this and request the disc
                    QueryDVD(path);
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

                case VideoFormat.URL: // this is used for online content (such as streaming trailers)
                case VideoFormat.UNKNOWN:
                    break;

                // Video files
                default : 
                    QueryMediaFile(path);
                    break;
            }
        }

        
        private void MountImage(string name, string path)
        {
            FileSystem.MountingTool mt = new FileSystem.MountingTool();
            string drive;
            mt.Mount(path, out drive);
            drive = drive + ":";

            if (FileSystem.FileScanner.IsDVD(drive)) { IdentifyMediaType(name, drive, VideoFormat.DVD); }
            if (FileSystem.FileScanner.IsHDDVD(drive)) { IdentifyMediaType(name, drive, VideoFormat.HDDVD); }
            if (FileSystem.FileScanner.IsBluRay(drive)) { IdentifyMediaType(name, drive, VideoFormat.BLURAY); }
        }


        private void QueryDVD(string path)
        {
            // Check to see if any IFO files exist
            if (Directory.GetFiles(path, "*.ifo").Length == 0)
            {
                // No IFO Files Found
                // Try to see if there is a VIDEO_TS folder
                if (Directory.Exists(System.IO.Path.Combine(path, "VIDEO_TS")))
                {
                    path = System.IO.Path.Combine(path, "VIDEO_TS");
                    if (Directory.GetFiles(path, "*.ifo").Length == 0)
                    {
                        return;
                    }
                }
            }
            DVDDiskInfo ddi = DVDDiskInfo.ParseDVD(path);
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
                    diaudiostream.Encoding = (DIAudioEncoding)at.Format;
                    diaudiostream.EncodingProfile = DIAudioEncodingProfile.Undefined;
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
                Utilities.DebugLine("[DiskInfo:QueryMediaFile] An error occured during file scan: {0}", ex.Message);
            }

            try
            {
                int audiostreamscount = ToInt32(MI.Get(MediaInfoLib.StreamKind.Audio, 0, "StreamCount"));
                Console.WriteLine("\nAudio Stream Count : " + audiostreamscount.ToString());

                for (i = 0; i < audiostreamscount; i++)
                {
                    DIAudioStream diaudiostream = new DIAudioStream();
                    diaudiostream.LanguageID = MI.Get(MediaInfoLib.StreamKind.Audio, i, "Language");
                    diaudiostream.BitrateMode = MI.Get(MediaInfoLib.StreamKind.Audio, i, "BitRate_Mode");
                    diaudiostream.Bitrate = ToInt32(MI.Get(MediaInfoLib.StreamKind.Audio, i, "BitRate")) / 1000;
                    diaudiostream.Channels = ToInt32(MI.Get(MediaInfoLib.StreamKind.Audio, i, "Channels"));
                    diaudiostream.AudioID = ToInt32(MI.Get(MediaInfoLib.StreamKind.Audio, i, "ID"));
                    diaudiostream.Name = diaudiostream.Language;
                    if (diaudiostream.Channels > 0) { diaudiostream.Name = diaudiostream.Name + " " + diaudiostream.Channels.ToString() + "ch"; }
                    
                    // Initialise fields
                    diaudiostream.EncodingProfile =  DIAudioEncodingProfile.Undefined;
                    diaudiostream.SubFormat = "";

                    switch (MI.Get(MediaInfoLib.StreamKind.Audio, i, "Format"))
                    {
                        case "AC-3": 
                            diaudiostream.Encoding = DIAudioEncoding.AC3;
                            diaudiostream.EncodingProfile = DIAudioEncodingProfile.Undefined;
                            break;

                        case "DTS":
                            diaudiostream.Encoding = DIAudioEncoding.DTS;
                            diaudiostream.EncodingProfile = DIAudioEncodingProfile.Undefined;
                            break;

                        case "AAC":
                            diaudiostream.Encoding = DIAudioEncoding.AAC;
                            diaudiostream.EncodingProfile = DIAudioEncodingProfile.Undefined;
                            break;

                        case "Vorbis":
                            diaudiostream.Encoding = DIAudioEncoding.VORBIS;
                            diaudiostream.EncodingProfile = DIAudioEncodingProfile.Undefined;
                            break;

                        case "MPEG Audio":
                            switch (MI.Get(MediaInfoLib.StreamKind.Audio, i, "Format_Version"))
                            {
                                case "Version 1": diaudiostream.Encoding = DIAudioEncoding.MPEG1; break;
                                case "Version 2": diaudiostream.Encoding = DIAudioEncoding.MPEG2; break;
                                default: diaudiostream.Encoding = DIAudioEncoding.Undefined; break;
                            }
                            switch (MI.Get(MediaInfoLib.StreamKind.Audio, i, "Format_Profile"))
                            {
                                case "Layer 2": diaudiostream.EncodingProfile = DIAudioEncodingProfile.Layer2; break;
                                case "Layer 3": diaudiostream.EncodingProfile = DIAudioEncodingProfile.Layer3; break;
                                default: diaudiostream.EncodingProfile = DIAudioEncodingProfile.Undefined; break;
                            }
                            break;

                        case "WMA2":
                            diaudiostream.Encoding = DIAudioEncoding.WMA2;
                            switch (MI.Get(MediaInfoLib.StreamKind.Audio, i, "Format_Profile"))
                            {
                                case "L1": diaudiostream.EncodingProfile =  DIAudioEncodingProfile.L1; break;
                                case "L2": diaudiostream.EncodingProfile = DIAudioEncodingProfile.L2; break;
                                default: diaudiostream.EncodingProfile = DIAudioEncodingProfile.Undefined; break;
                            }
                            break;

                        default :
                            diaudiostream.Encoding = DIAudioEncoding.Undefined;
                            diaudiostream.EncodingProfile = DIAudioEncodingProfile.Undefined;
                            break;
                    }

                    difeature.AudioStreams.Add(diaudiostream);
                }
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[DiskInfo:QueryMediaFile] An error occured during file scan: {0}", ex.Message);
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
