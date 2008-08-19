/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLTranscoder
{
    public class AudioStream
    {
        int audioId;
        bool surroundSound;
        string languageId;
        string audioChannel;

        public AudioStream()
        {
            audioId = -1;
            surroundSound = false;
            languageId = string.Empty;
            audioChannel = string.Empty;
        }
    }
}
