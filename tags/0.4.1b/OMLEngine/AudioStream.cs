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
    public class AudioStream
    {
        public bool IsSurroundSound { get { return (Channels ?? 0) > 2; } }
        public string Language { get { return MediaLanguage.LanguageNameForId(LanguageID); } }
        [DataMember]
        public string LanguageID { get; private set; }
        [DataMember]
        public int? AudioID { get; private set; }
        [DataMember]
        public int? Channels { get; private set; }
        [DataMember]
        public AudioEncoding Format { get; private set; }
        [DataMember]
        public AudioExtension Extension { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.LanguageID, 
                string.Format("{0}, {1}, {2}, {3}", this.AudioID, this.Format, 
                    DVDAudioTrack.GetSubFormat(this.Channels ?? 0), 
                    DVDAudioTrack.GetExtension(this.Extension)).TrimEnd(' ', ','));
        }

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
            this.Extension = audioTrack.Extension;
        }
    }
}
