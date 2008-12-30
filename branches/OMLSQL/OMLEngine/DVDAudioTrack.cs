using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using OMLEngine;

namespace OMLGetDVDInfo
{
    public class DVDAudioTrack
    {
        #region -- Members --
        /// <summary>
        /// The track number of this Audio Track
        /// </summary>
        [XmlAttribute]
        public int TrackNumber;
        /// <summary>
        /// The language (if detected) of this Audio Track
        /// </summary>
        public string Language { get { return MediaLanguage.LanguageNameForId(LanguageID); } }

        [XmlAttribute]
        public string LanguageID;

        [XmlAttribute]
        public int ID;

        /// <summary>
        /// The primary format of this Audio Track
        /// </summary>
        [XmlAttribute]
        public AudioEncoding Format;
        /// <summary>
        /// Additional format info for this Audio Track
        /// </summary>
        public string SubFormat { get { return GetSubFormat(Channels); } }

        [XmlAttribute]
        public int Channels;

        [XmlAttribute]
        public AudioExtension Extension;
        /// <summary>
        /// The frequency (in MHz) of this Audio Track
        /// </summary>
        [XmlAttribute]
        public int Frequency;
        /// <summary>
        /// The bitrate (in kbps) of this Audio Track
        /// </summary>
        [XmlAttribute]
        public int Bitrate;
        #endregion

        public static string GetExtension(AudioExtension extension)
        {
            switch (extension)
            {
                case AudioExtension.For_visually_impaired:
                    return "For visually impaired";
                case AudioExtension.Director_s_comments:
                    return "Director's comments";
                case AudioExtension.Alternate_director_s_comments:
                    return "Alternate director's comments";
                case AudioExtension.Unspecified:
                case AudioExtension.Normal:
                default:
                    return string.Empty;
            }
        }

        public static string GetSubFormat(int channels)
        {
            switch (channels)
            {
                case 1: return "mono";
                case 2: return "stereo";
                case 6: return "5.1 ch";
                case 7: return "6.1 ch";
                case 8: return "7.1 ch";
            }
            return null;
        }

        /// <summary>
        /// Override of the ToString method to make this object easier to use in the UI
        /// </summary>
        /// <returns>A string formatted as: {track #} {language} ({format}) ({sub-format})</returns>
        public override string ToString()
        {
            return string.Format("{0} # {1} in {2}, {3}", this.Language, 
                this.Format, this.SubFormat, GetExtension(this.Extension)).TrimEnd(' ', ',');
        }

        public override bool Equals(object obj)
        {
            if (obj != null && (obj is DVDAudioTrack) == false)
                return false;
            return this == (DVDAudioTrack)obj;
        }
        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(DVDAudioTrack a, DVDAudioTrack b)
        {
            if ((object)a == (object)b) return true;
            if ((object)b == null) return false;
            if (a.TrackNumber != b.TrackNumber)
                return false;
            if (a.Language != b.Language)
                return false;
            if (a.Frequency != b.Frequency)
                return false;
            if (a.Format != b.Format)
                return false;
            if (a.SubFormat != b.SubFormat)
                return false;
            return true;
        }

        public static bool operator !=(DVDAudioTrack a, DVDAudioTrack b) { return !(a == b); }

    }

}
