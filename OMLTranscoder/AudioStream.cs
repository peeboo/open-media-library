/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLTranscoder
{
    public class AudioStream
    {
        private int audioId;
        private bool isSurroundSound;
        private string languageId;
        private string audioChannel;

        public AudioStream()
        {
            audioId = -1;
            isSurroundSound = false;
            languageId = string.Empty;
            audioChannel = string.Empty;
        }
    }
}
