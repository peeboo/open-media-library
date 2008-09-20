/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using OMLGetDVDInfo;

namespace OMLEngine
{
    public class AudioStream
    {
        public bool IsSurroundSound { get { return (Channels ?? 0) > 2; } }
        public string Language { get { return MediaLanguage.LanguageNameForId(LanguageID); } }
        public string LanguageID { get; private set; }
        public int? AudioID { get; private set; }
        public int? Channels { get; private set; }
        public AudioEncoding Format { get; private set; }

        public AudioStream()
        {
            this.AudioID = null;
        }

        public AudioStream(DVDAudioTrack audioTrack)
        {
            this.AudioID = audioTrack.ID;
            this.LanguageID = audioTrack.LanguageID;
            this.Channels = audioTrack.Channels;
            this.Format = audioTrack.Format;
        }
    }
}
