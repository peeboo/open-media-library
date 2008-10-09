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
            if (string.IsNullOrEmpty(extraOptions) || extraOptions.StartsWith("#\n"))
                return;

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

        public static IEnumerable<MediaSource> GetSourcesFromOptions(string mediaPath, string extraOptions)
        {
            return GetSourcesFromOptions(mediaPath, extraOptions, false);
        }

        public static IEnumerable<MediaSource> GetSourcesFromOptions(string mediaPath, string extraOptions, bool returnDefault)
        {
            List<MediaSource> ret = new List<MediaSource>();
            if (extraOptions == null && returnDefault)
            {
                var ms = new MediaSource(new Disk("Main", mediaPath, VideoFormat.DVD));
                DVDTitle mt = ms.DVDTitle;
                if (mt != null)
                    foreach (DVDTitle t in ms.DVDDiskInfo.Titles)
                        if (t.TitleNumber != mt.TitleNumber && t.AudioTracks.Count > 0)
                            ret.Add(new MediaSource(new Disk("Title " + t.TitleNumber, mediaPath, VideoFormat.DVD) { ExtraOptions = "T=" + t.TitleNumber }));
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
    }
}
