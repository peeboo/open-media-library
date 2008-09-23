/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
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
        }

        [DataMember]
        public Disk Disk { get; private set; }
        public string Name { get { return Disk.Name; } }
        public string MediaPath { get { return Disk.Path; } set { Disk.Path = value; } }

        public string Key { get { throw new NotImplementedException(); } }

        #region -- DVD Members --
        public string VIDEO_TS { get { return Disk.VIDEO_TS; } }
        public string VIDEO_TS_Parent { get { return Disk.VIDEO_TS_Parent; } }
        public DVDDiskInfo DVDDiskInfo { get { return Disk.DVDDiskInfo; } }

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
            return Disk.ToString();
        }
    }
}
