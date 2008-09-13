using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

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
        [XmlAttribute]
        public string Language;
        /// <summary>
        /// The primary format of this Audio Track
        /// </summary>
        [XmlAttribute]
        public string Format;
        /// <summary>
        /// Additional format info for this Audio Track
        /// </summary>
        [XmlAttribute]
        public string SubFormat;

        [XmlAttribute]
        public string Extension;
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

        /// <summary>
        /// Override of the ToString method to make this object easier to use in the UI
        /// </summary>
        /// <returns>A string formatted as: {track #} {language} ({format}) ({sub-format})</returns>
        public override string ToString()
        {
            return string.Format("{0} {1} ({2}) ({3})", this.TrackNumber, this.Language, this.Format, this.SubFormat);
        }

        public override bool Equals(object obj) { return this == (DVDAudioTrack)obj; }
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

        #region -- Parsing --
        internal static DVDAudioTrack Parse(TextReader output)
        {
            Match m = Regex.Match(output.ReadLine(), @"^    \+ ([0-9]*), ([A-Za-z0-9]*) \((.*)\) \((.*)\), ([0-9]*)Hz, ([0-9]*)bps");
            if (m.Success)
            {
                DVDAudioTrack thisTrack = new DVDAudioTrack();
                thisTrack.TrackNumber = int.Parse(m.Groups[1].Value.Trim().ToString());
                thisTrack.Language = m.Groups[2].Value;
                thisTrack.Format = m.Groups[3].Value;
                thisTrack.SubFormat = m.Groups[4].Value;
                thisTrack.Frequency = int.Parse(m.Groups[5].Value.Trim().ToString());
                thisTrack.Bitrate = int.Parse(m.Groups[6].Value.Trim().ToString());
                return thisTrack;
            }
            else
                return null;
        }

        internal static List<DVDAudioTrack> ParseList(TextReader output)
        {
            List<DVDAudioTrack> tracks = new List<DVDAudioTrack>();
            while (true)
            {
                DVDAudioTrack thisTrack = DVDAudioTrack.Parse(output);
                if (thisTrack != null)
                    tracks.Add(thisTrack);
                else
                    break;
            }
            return tracks;
        }
        #endregion
    }

}
