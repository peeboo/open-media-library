/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLEngine
{
    public class MediaSource
    {
        public MediaSource(Disk disk)
        {
            Disk = disk;
            Subtitle = new SubtitleStream();
            AudioStream = new AudioStream();
        }

        public Disk Disk { get; private set; }
        public string Name { get { return Disk.Name; } }
        public string MediaPath { get { return Disk.Path; } set { Disk.Path = value; } }
        public VideoFormat Format { get { return Disk.Format; } set { Disk.Format = value; } }
        public SubtitleStream Subtitle { get; set; }
        public AudioStream AudioStream { get; set; }

        public int? StartChapter { get; set; }
        public int? EndChapter { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public int VideoType { get; set; }
        public float Ratio { get; set; }
        public float Framerate { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public override string ToString()
        {
            return Disk.ToString();
        }
    }
}
