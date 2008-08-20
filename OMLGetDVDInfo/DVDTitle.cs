using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OMLGetDVDInfo
{
    public class DVDTitle
    {
        #region -- Members --
        [XmlElement("Chapter")]
        public DVDChapter[] Chapters;
        [XmlElement("AudioTrack")]
        public DVDAudioTrack[] AudioTracks;
        [XmlElement("SubTitle")]
        public DVDSubtitle[] Subtitles;
        [XmlAttribute]
        public string File;
        [XmlAttribute]
        public int TitleNumber;

        TimeSpan m_Duration;
        [XmlAttribute]
        public string Duration { get { return m_Duration.ToString(); } set { m_Duration = TimeSpan.Parse(value); } }

        [XmlElement]
        public Size Resolution;
        [XmlAttribute]
        public float AspectRatio;
        [XmlElement("AutoCrop")]
        public int[] AutoCrop;
        #endregion

        public override string ToString()
        {
            return string.Format("{0} ({1:00}:{2:00}:{3:00})", this.TitleNumber, this.m_Duration.Hours,
             this.m_Duration.Minutes, this.m_Duration.Seconds);
        }

        #region -- Parsing --
        internal static DVDTitle Parse(TextReader output)
        {
            DVDTitle thisTitle = new DVDTitle();
            Match m = Regex.Match(output.ReadLine(), @"^\+ title ([0-9]*):");
            // Match track number for this title
            if (m.Success)
                thisTitle.TitleNumber = int.Parse(m.Groups[1].Value.Trim().ToString());

            m = Regex.Match(output.ReadLine(), @"^  \+ (vts [0-9]+), ttn [0-9]+, cells ([0-9]+)->([0-9]+) \(([0-9]+) blocks\)");
            // Match track number for this title
            if (m.Success)
                thisTitle.File = m.Groups[1].Value.Trim().ToString();

            // Get duration for this title

            m = Regex.Match(output.ReadLine(), @"^  \+ duration: ([0-9]{2}:[0-9]{2}:[0-9]{2})");
            if (m.Success)
                thisTitle.m_Duration = TimeSpan.Parse(m.Groups[1].Value);

            // Get resolution, aspect ratio and FPS for this title
            m = Regex.Match(output.ReadLine(), @"^  \+ size: ([0-9]*)x([0-9]*), aspect: ([0-9]*\.[0-9]*), ([0-9]*\.[0-9]*) fps");
            if (m.Success)
            {
                thisTitle.Resolution = new Size(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
                thisTitle.AspectRatio = float.Parse(m.Groups[3].Value);
            }

            // Get autocrop region for this title
            m = Regex.Match(output.ReadLine(), @"^  \+ autocrop: ([0-9]*)/([0-9]*)/([0-9]*)/([0-9]*)");
            if (m.Success)
                thisTitle.AutoCrop = new int[4] { int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value) };

            thisTitle.Chapters = DVDChapter.ParseList(output).ToArray();
            thisTitle.AudioTracks = DVDAudioTrack.ParseList(output).ToArray();
            thisTitle.Subtitles = DVDSubtitle.ParseList(output).ToArray();

            return thisTitle;
        }

        internal static List<DVDTitle> ParseList(string output)
        {
            List<DVDTitle> titles = new List<DVDTitle>();
            using (StringReader sr = new StringReader(output))
            {
                while ((char)sr.Peek() == '+')
                    titles.Add(DVDTitle.Parse(sr));
                return titles;
            }
        }
        #endregion
    }

}
