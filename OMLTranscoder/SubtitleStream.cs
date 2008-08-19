/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLTranscoder
{
    public class SubtitleStream
    {
        string languageId;
        int subtitleId;
        string subtitleChannel;


        public string Language
        {
            get { return languageId; }
            set { languageId = value; }
        }

        public int SubtitleId
        {
            get { return subtitleId; }
            set { subtitleId = value; }
        }

        public string SubtitleChannel
        {
            get { return subtitleChannel; }
            set { subtitleChannel = value; }
        }

        public SubtitleStream()
        {
            languageId = string.Empty;
            subtitleId = -1;
            subtitleChannel = string.Empty;
        }
    }
}
