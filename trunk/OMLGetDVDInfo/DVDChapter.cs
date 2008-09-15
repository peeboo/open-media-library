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

#if HANDBRAKE
        #region -- Parsing --
        internal static DVDChapter Parse(TextReader output)
        {
            Match m = Regex.Match(output.ReadLine(), @"^    \+ ([0-9]*): cells ([0-9]*)->([0-9]*), ([0-9]*) blocks, duration ([0-9]{2}:[0-9]{2}:[0-9]{2})");
            if (m.Success)
            {
                return new DVDChapter()
                {
                    ChapterNumber = int.Parse(m.Groups[1].Value.Trim().ToString()),
                    Duration = TimeSpan.Parse(m.Groups[5].Value),
                };
            }
            else
                return null;
        }

        internal static List<DVDChapter> ParseList(TextReader output)
        {
            List<DVDChapter> chapters = new List<DVDChapter>();

            // this is to read the "  + chapters:" line from the buffer
            // so we can start reading the chapters themselvs
            output.ReadLine();

            while (true)
            {
                // Start of the chapter list for this Title
                DVDChapter thisChapter = DVDChapter.Parse(output);

                if (thisChapter != null)
                    chapters.Add(thisChapter);
                else
                    break;
            }
            return chapters;
        }
        #endregion
#endif
    }
}
