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

        public SubtitleStream()
        {
            languageId = string.Empty;
            subtitleId = -1;
            subtitleChannel = string.Empty;
        }
    }
}
