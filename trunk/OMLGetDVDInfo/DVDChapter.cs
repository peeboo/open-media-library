using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OMLGetDVDInfo
{
    public class DVDChapter
    {
        #region -- Members --
        [XmlAttribute]
        public int ChapterNumber;

        [XmlAttribute]
        public int FPS;

        [XmlIgnore]
        public TimeSpan Duration;
        [XmlAttribute("Duration")]
        public string _Duration { get { return Duration.ToString(); } set { Duration = TimeSpan.Parse(value); } }
        #endregion

        public long TotalFrames { get { return IFOUtilities.GetTotalFrames(Duration, FPS); } }
        public int Frames { get { return IFOUtilities.GetFrames(Duration, FPS); } }

        public override bool Equals(object obj) { return this == (DVDChapter)obj; }
        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(DVDChapter a, DVDChapter b)
        {
            if ((object)a == (object)b) return true;
            if ((object)b == null) return false;
            if (a.ChapterNumber != b.ChapterNumber)
                return false;
            if (Math.Abs((a.Duration - b.Duration).TotalSeconds) > 1)
                return false;
            return true;
        }
        public static bool operator !=(DVDChapter a, DVDChapter b) { return !(a == b); }

        public override string ToString()
        {
            return string.Format("{0} - {1} / {2} fps", ChapterNumber, Duration, FPS);
        }

    }
}
