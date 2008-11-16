/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;

using OMLGetDVDInfo;

namespace OMLEngine
{
    [DataContract]
    [KnownType(typeof(VideoFormat))]
    public class MediaSource
    {
        public MediaSource(Disk disk)
        {
            Disk = disk;
            Subtitle = null;
            AudioStream = null;
            SetPropertiesFromExtraOptions(disk.ExtraOptions);
        }

        #region -- ExtraOptions --
        void SetPropertiesFromExtraOptions(string extraOptions)
        {
            if (string.IsNullOrEmpty(extraOptions))
                return;

            if (extraOptions.StartsWith("#"))
            {
                foreach (string kv in extraOptions.TrimStart('#').Split('\n'))
                {
                    if (string.IsNullOrEmpty(kv) == false)
                    {
                        var kvA = kv.Split('=');
                        var kvRec = new { key = kvA[0], val = kvA[1] };
                        switch (kvRec.key)
                        {
                            case "RT": this.ResumeTime = TimeSpan.Parse(kvRec.val); break;
                        }
                    }
                    break;
                }
                return;
            }

            foreach (string kv in extraOptions.Split(';'))
            {
                var kvA = kv.Split('=');
                var kvRec = new { key = kvA[0], val = kvA[1] };
                switch (kvRec.key)
                {
                    case "T": this.Title = int.Parse(kvRec.val); break;
                    case "CS": this.StartChapter = int.Parse(kvRec.val); break;
                    case "CE": this.EndChapter = int.Parse(kvRec.val); break;
                    case "S": this.Subtitle = GetSubTitle(this.DVDTitle.GetSubTitle(int.Parse(kvRec.val))); break;
                    case "A": this.AudioStream = GetAudioSteam(this.DVDTitle.GetAudio(int.Parse(kvRec.val))); break;
                    case "TS": this.StartTime = TimeSpan.Parse(kvRec.val); break;
                    case "TE": this.EndTime = TimeSpan.Parse(kvRec.val); break;
                    case "RT": this.ResumeTime = TimeSpan.Parse(kvRec.val); break;
                }
            }
        }

        public string ExtraOptions
        {
            get
            {
                string key = string.Empty;
                if (this.Title != null)
                    key += ";T=" + this.Title;
                if (this.StartChapter != null)
                    key += ";CS=" + this.StartChapter;
                if (this.EndChapter != null)
                    key += ";CE=" + this.EndChapter;
                if (this.Subtitle != null && this.Subtitle.SubtitleID != null)
                    key += ";S=" + this.Subtitle.SubtitleID;
                if (this.AudioStream != null && this.AudioStream.AudioID != null)
                    key += ";A=" + this.AudioStream.AudioID;
                if (this.StartTime != null)
                    key += ";TS=" + this.StartTime;
                if (this.EndTime != null)
                    key += ";TE=" + this.EndTime;
                if (this.ResumeTime != null)
                    key += ";RT=" + this.ResumeTime;
                return key.TrimStart(';');
            }
        }

        public static string ExtraOptionsFromMenu(ICollection<MediaSource> menu)
        {
            string ret = "#\n";
            foreach (MediaSource source in menu)
                ret += source.ExtraOptions + ";N=" + source.Name + "\n";
            return ret.TrimEnd();
        }

        public string UpdateExtraOptions(string extraOptions)
        {
            List<string> lines = new List<string>(string.IsNullOrEmpty(extraOptions) ? new string[0] : extraOptions.Split('\n'));
            string ret;
            if (this.Title == null)
            {
                if (lines.Count > 0)
                    lines.RemoveAt(0);
                ret = "#" + this.ExtraOptions + "\n" + string.Join("\n", lines.ToArray());
                Utilities.DebugLine("UpdateExtraOptions: {0}, with {1} -> {2}", extraOptions, this, ret);
                return ret;
            }

            for (int i = 1; i < lines.Count; ++i)
            {
                string line = lines[i];
                MediaSource tmp = new MediaSource(this.Disk);
                tmp.SetPropertiesFromExtraOptions(line);
                if (this.Title == tmp.Title)
                {
                    lines[i] = this.ExtraOptions;
                    break;
                }
            }

            ret = "#" + string.Join("\n", lines.ToArray());
            Utilities.DebugLine("UpdateExtraOptions: {0}, with {1} -> {2}", extraOptions, this, ret);
            return ret;
        }

        public static IEnumerable<MediaSource> GetSourcesFromOptions(string mediaPath, string extraOptions)
        {
            return GetSourcesFromOptions(mediaPath, extraOptions, false);
        }

        static string FormatTime(TimeSpan time)
        {
            if (time.TotalHours > 1)
                return string.Format("{0}h {1}m", time.Hours, time.Minutes + (time.Seconds > 30 ? 1 : 0));
            if (time.TotalMinutes > 1)
                return string.Format("{0}m {1}s", time.Minutes, time.Seconds);
            return string.Format("{0}s", time.Seconds);
        }

        public static IEnumerable<MediaSource> GetSourcesFromOptions(string mediaPath, string extraOptions, bool returnDefault)
        {
            List<MediaSource> ret = new List<MediaSource>();
            if (extraOptions == null && returnDefault)
            {
                var ms = new MediaSource(new Disk("Main", mediaPath, VideoFormat.DVD));
                DVDTitle mt = ms.DVDTitle;
                if (mt != null)
                {
                    TimeSpan minDuration = TimeSpan.FromSeconds(30);
                    var list = from t in ms.DVDDiskInfo.Titles
                               where t.TitleNumber != mt.TitleNumber && t.AudioTracks.Count > 0 && t.Duration > minDuration
                               group t by t.File.Substring(4).ToLower() into t
                               select new
                               {
                                   TitleNumber = t.Last().TitleNumber,
                                   File = t.Last().File.Substring(4),
                                   Duration = TimeSpan.FromSeconds(t.Sum(a => a.Duration.TotalSeconds))
                               };

                    ret.AddRange(from t in list.Take(7)
                                 orderby t.Duration.TotalSeconds
                                 select
                                     new MediaSource(new Disk("" + FormatTime(t.Duration) + ": " + t.File, mediaPath, VideoFormat.DVD) { ExtraOptions = "T=" + t.TitleNumber }));
                }
                return ret;
            }
            if (extraOptions == null || extraOptions.StartsWith("#\n") == false)
                return ret;

            foreach (string sourceLine in extraOptions.TrimStart('#', '\n').Split('\n'))
            {
                int pos = sourceLine.IndexOf(";N=");
                string name = sourceLine.Substring(pos + ";N=".Length);
                ret.Add(new MediaSource(new Disk(name, mediaPath, VideoFormat.DVD) { ExtraOptions = sourceLine.Substring(0, pos) }));
            }
            return ret;
        }
        #endregion

        public bool HasAudioSteam(AudioExtension extension) { return GetAudioSteam(extension) != null; }
        public AudioStream GetAudioSteam(AudioExtension extension)
        {
            DVDTitle title = DVDTitle;
            if (title == null)
                return null;
            return GetAudioSteam((from a in title.AudioTracks where a.Extension == extension select a).FirstOrDefault());
        }

        public static AudioStream GetAudioSteam(DVDAudioTrack at) { return at == null ? null : new AudioStream(at); }
        public static SubtitleStream GetSubTitle(DVDSubtitle st) { return st == null ? null : new SubtitleStream(st); }
        public SubtitleStream GetSubTitle(string languageID)
        {
            DVDTitle title = DVDTitle;
            if (title == null)
                return null;
            return GetSubTitle(title.GetSubTitle(languageID));
        }

        [DataMember]
        public Disk Disk { get; private set; }
        public string Name { get { return Disk.Name; } }
        public string MediaPath { get { return Disk.Path; } set { Disk.Path = value; } }

        public string Key
        {
            get
            {
                string key = TranscodingBaseName;
                if (key == null)
                    return null;
                key += "-" + ExtraOptions.Replace(";", "-").Replace("=", "").Replace(":", "-");
                return key.TrimEnd('-');
            }
        }

        #region -- DVD Members --
        public string VIDEO_TS { get { return Disk.VIDEO_TS; } }
        public string VIDEO_TS_Parent { get { return Disk.VIDEO_TS_Parent; } }
        public DVDDiskInfo DVDDiskInfo { get { return Disk.DVDDiskInfo; } }
        public DVDTitle DVDTitle
        {
            get
            {
                if (Disk.DVDDiskInfo == null)
                    return null;
                return Disk.DVDDiskInfo.GetTitle(this.Title);
            }
        }

        // DVD playback options
        [DataMember]
        public SubtitleStream Subtitle { get; set; }
        [DataMember]
        public AudioStream AudioStream { get; set; }
        [DataMember]
        public int? Title { get; set; }
        [DataMember]
        public int? StartChapter { get; set; }
        [DataMember]
        public int? EndChapter { get; set; }
        [DataMember]
        public TimeSpan? ResumeTime { get; set; }
        #endregion

        public VideoFormat Format { get { return Disk.Format; } set { Disk.Format = value; } }

        [DataMember]
        public TimeSpan? StartTime { get; set; }
        [DataMember]
        public TimeSpan? EndTime { get; set; }

        /* not used
        public int VideoType { get; private set; }
        public float Ratio { get; private set; }
        public float Framerate { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        */

        public override string ToString()
        {
            string ret = Disk.ToString();
            if (Format == VideoFormat.DVD)
            {
                if (this.Title != null)
                    ret += " T:" + this.Title;
                if (this.StartChapter != null)
                {
                    ret += " C:" + this.StartChapter;
                    if (this.EndChapter != null)
                        ret += "-" + this.EndChapter;
                }
                if (this.AudioStream != null)
                    ret += " A:" + this.AudioStream;
                if (this.Subtitle != null)
                    ret += " Sub:" + this.Subtitle;
            }
            else
            {
                if (this.StartTime != null)
                    ret += " ST:" + this.StartTime;
                if (this.EndTime != null)
                    ret += " ET:" + this.EndTime;
            }
            return ret;
        }

        public string Description
        {
            get
            {
                string ret = this.Name;

                if (this.StartChapter != null)
                {
                    ret += ", Chapter " + this.StartChapter;
                    if (this.EndChapter != null)
                        ret += "-" + this.EndChapter;
                }

                if (this.AudioStream != null)
                    ret += ", Audio " + this.AudioStream;
                if (this.Subtitle != null)
                    ret += ", Subtitle " + this.Subtitle.LanguageID;

                return ret;
            }
        }

        string TranscodingBaseName
        {
            get
            {
                if (this.Format != VideoFormat.DVD)
                    return Path.GetFileNameWithoutExtension(this.MediaPath);

                string root = this.VIDEO_TS_Parent;
                if (root != Path.GetPathRoot(root))
                    return new DirectoryInfo(root).Name;

                DriveInfo inputDrive = new DriveInfo(root.Substring(0, 1));
                try
                {
                    if (inputDrive.IsReady)
                        return inputDrive.VolumeLabel;
                }
                catch { }
                return null;
            }
        }

        public string GetTranscodingFileName()
        {
            string transcodingName = Key;
            if (transcodingName == null)
                return null;

            if (this.Format != VideoFormat.DVD)
                return Path.ChangeExtension(Path.Combine(FileSystemWalker.TranscodeBufferDirectory, transcodingName), ".wmv");

            return Path.ChangeExtension(Path.Combine(FileSystemWalker.TranscodeBufferDirectory, transcodingName), ".mpg");
        }

        public void ClearResumeTime()
        {
            if (this.ResumeTime == null)
                return;
            this.ResumeTime = null;
            if (this.OnSave != null)
                this.OnSave(this);
        }

        public void SetResumeTime(TimeSpan timeSpan)
        {
            this.ResumeTime = timeSpan;
            if (this.OnSave != null)
                this.OnSave(this);
        }

        public event Action<MediaSource> OnSave;
    }
}
