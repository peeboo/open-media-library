/*******************************************************
 * This project heavily inspired by Transcode 360      *
 * written by Albert Griscti-Soler and Bernard Maltais *
 *******************************************************/

using System;
using System.Runtime.Serialization;

using OMLGetDVDInfo;

namespace OMLEngine
{
    [DataContract]
    public class SubtitleStream
    {
        public string Language { get { return MediaLanguage.LanguageNameForId(LanguageID); } }
        [DataMember]
        public string LanguageID { get; private set; }
        [DataMember]
        public int? SubtitleID { get; private set; }

        public SubtitleStream()
        {
            this.LanguageID = string.Empty;
            this.SubtitleID = null;
        }

        public SubtitleStream(DVDSubtitle subTitle)
        {
            this.SubtitleID = subTitle.TrackNumber;
            this.LanguageID = subTitle.LanguageID;
        }
    }
}
