/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;

namespace OMLTranscoder
{
    public class SubtitleStream
    {
        public string LanguageName { get; private set; }
        public string LanguageShortName { get; private set; }
        public int? SubtitleID { get; private set; }

        public SubtitleStream()
        {
            this.LanguageName = this.LanguageShortName = string.Empty;
            this.SubtitleID = null;
        }

        public SubtitleStream(int subtitleID, string languageShortName)
        {
            this.SubtitleID = subtitleID;
            this.LanguageName = MediaLanguage.LanguageNameForId(languageShortName);
            this.LanguageShortName = languageShortName;
        }
    }
}
