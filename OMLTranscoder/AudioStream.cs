/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLTranscoder
{
    public class AudioStream
    {
        public bool IsSurroundSound { get; private set; }
        public string LanguageName { get; private set; }
        public string LanguageShortName { get; private set; }
        public int AudioID { get; private set; }

        public AudioStream()
        {
            this.AudioID = -1;
            this.IsSurroundSound = false;
        }

        public AudioStream(int audioID, bool isSurround, string languageShortName)
        {
            this.AudioID = audioID;
            this.IsSurroundSound = isSurround;
            this.LanguageName = MediaLanguage.LanguageNameForId(languageShortName);
            this.LanguageShortName = languageShortName;
        }
    }
}
