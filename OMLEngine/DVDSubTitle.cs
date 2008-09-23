using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using OMLEngine;

namespace OMLGetDVDInfo
{
    /// <summary>
    /// An object that represents a subtitle associated with a Title, in a DVD
    /// </summary>
    public class DVDSubtitle
    {
        #region -- Members --
        /// <summary>
        /// The track number of this Subtitle
        /// </summary>
        [XmlAttribute]
        public int TrackNumber;
        /// <summary>
        /// The language (if detected) of this Subtitle
        /// </summary>
        public string Language { get { return MediaLanguage.LanguageNameForId(LanguageID); } }
        [XmlAttribute]
        public string LanguageID;
        #endregion

        /// <summary>
        /// Override of the ToString method to make this object easier to use in the UI
        /// </summary>
        /// <returns>A string formatted as: {track #} {language}</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", this.TrackNumber, this.Language);
        }

        public override bool Equals(object obj) { return this == (DVDSubtitle)obj; }
        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(DVDSubtitle a, DVDSubtitle b)
        {
            if ((object)a == (object)b) return true;
            if ((object)b == null) return false;
            if (a.TrackNumber != b.TrackNumber)
                return false;
            if (a.Language != b.Language)
                return false;
            return true;
        }
        public static bool operator !=(DVDSubtitle a, DVDSubtitle b) { return !(a == b); }
    }

}
