/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLTranscoder
{
    public class AudioStream
    {
        private int audioId = -1;
        private bool isSurroundSound = false;
        private string languageName;
        private string languageShortName;

        public string LanguageName
        {
            get
            {
                return null;
            }
        }

        public string LanguageShortName
        {
            get
            {
                return null;
            }
        }

        public int AudioID
        {
            get { return audioId; }
        }

        public AudioStream()
        {
            audioId = -1;
            isSurroundSound = false;
        }

        public AudioStream(int AudioID, bool IsSurround, string languageShortName)
        {
            audioId = AudioID;
            isSurroundSound = IsSurround;
            MediaLanguage ml = new MediaLanguage();
            languageName = ml.LanguageNameForId(LanguageShortName);
            languageShortName = languageShortName;
        }
    }
}
