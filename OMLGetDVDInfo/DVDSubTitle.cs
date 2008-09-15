using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

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
        [XmlAttribute]
        public string Language;
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

#if HANDBRAKE
        #region -- Parsing --
        internal static DVDSubtitle Parse(TextReader output)
        {
            string curLine = output.ReadLine();

            Match m = Regex.Match(curLine, @"^    \+ ([0-9]*), ([A-Za-z, ]*) \((.*)\)");
            if (m.Success && !curLine.Contains("HandBrake has exited."))
            {
                DVDSubtitle thisSubtitle = new DVDSubtitle();
                thisSubtitle.TrackNumber = int.Parse(m.Groups[1].Value.Trim().ToString());
                thisSubtitle.Language = m.Groups[2].Value;
                return thisSubtitle;
            }
            else
                return null;
        }

        internal static List<DVDSubtitle> ParseList(TextReader output)
        {
            List<DVDSubtitle> subtitles = new List<DVDSubtitle>();
            while ((char)output.Peek() != '+')
            {
                DVDSubtitle thisSubtitle = DVDSubtitle.Parse(output);

                if (thisSubtitle != null)
                    subtitles.Add(thisSubtitle);
                else
                    break;
            }
            return subtitles;
        }
        #endregion
#endif
    }

}
