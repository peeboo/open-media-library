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
        TimeSpan m_Duration;
        [XmlAttribute]
        public string Duration { get { return m_Duration.ToString(); } set { m_Duration = TimeSpan.Parse(value); } }
        #endregion

        #region -- Parsing --
        internal static DVDChapter Parse(TextReader output)
        {
            Match m = Regex.Match(output.ReadLine(), @"^    \+ ([0-9]*): cells ([0-9]*)->([0-9]*), ([0-9]*) blocks, duration ([0-9]{2}:[0-9]{2}:[0-9]{2})");
            if (m.Success)
            {
                DVDChapter thisChapter = new DVDChapter();
                thisChapter.ChapterNumber = int.Parse(m.Groups[1].Value.Trim().ToString());
                thisChapter.m_Duration = TimeSpan.Parse(m.Groups[5].Value);
                return thisChapter;
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
    }
}
