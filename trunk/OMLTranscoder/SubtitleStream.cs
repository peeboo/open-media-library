/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLTranscoder
{
    public class SubtitleStream
    {
        public string Language { get; set; }
        public int SubtitleId { get; set; }
        public string SubtitleChannel { get; set; }

        public SubtitleStream()
        {
            this.Language = string.Empty;
            this.SubtitleId = -1;
            this.SubtitleChannel = string.Empty;
        }
    }
}
