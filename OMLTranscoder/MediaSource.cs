/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using System.Collections.Generic;

namespace OMLTranscoder
{
    public class MediaSource
    {
        public MediaSource()
        {
            AudioID = -1;
            MediaPath = string.Empty;
            SubtitleID = -1;
            Subtitles = string.Empty;
            Volume = string.Empty;
        }

        public string Volume { get; set; }
        public int VideoType { get; set; }
        public long TimestampOffset { get; set; }
        public string Subtitles { get; set; }
        public int SubtitleID { get; set; }
        public float Ratio { get; set; }
        public string MediaPath { get; set; }
        public int NumberOfAudioChannels { get; set; }
        public int AudioID { get; set; }
        public int AudioType { get; set; }
        public float Framerate { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
