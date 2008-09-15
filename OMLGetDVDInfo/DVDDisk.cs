using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace OMLGetDVDInfo
{
    public class DVDDiskInfo
    {
        [XmlElement("Title")]
        public DVDTitle[] Titles;

        public override string ToString()
        {
            return string.Join(", ", new List<DVDTitle>(Titles).ConvertAll<string>(delegate(DVDTitle t) { return "[" + t + "]"; }).ToArray());
        }

        public override bool Equals(object obj) { return this == (DVDDiskInfo)obj; }
        public override int GetHashCode() { return base.GetHashCode(); }

        public static bool operator ==(DVDDiskInfo a, DVDDiskInfo b)
        {
            if ((object)a == (object)b) return true;
            if ((object)b == null) return false;
            if (a.Titles.Length != b.Titles.Length)
                return false;
            for (int i = 0; i < a.Titles.Length; ++i)
                if (a.Titles[i] != b.Titles[i])
                    return false;
            return true;
        }
        public static bool operator !=(DVDDiskInfo a, DVDDiskInfo b) { return !(a == b); }

#if HANDBRAKE
        internal static DVDDiskInfo Parse(StreamReader output)
        {
            DVDDiskInfo thisDVD = new DVDDiskInfo();
            while (output.EndOfStream == false)
            {
                if ((char)output.Peek() == '+')
                    thisDVD.Titles = DVDTitle.ParseList(output.ReadToEnd()).ToArray();
                else
                    output.ReadLine();
            }
            return thisDVD;
        }
#endif

#if HANDBRAKE
        static string handbrakePath = null;
        static DVDDiskInfo()
        {
            handbrakePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Handbrake");
            if (Directory.Exists(handbrakePath) == false)
            {
                handbrakePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + " (x86)", "Handbrake");
                if (Directory.Exists(handbrakePath) == false)
                    handbrakePath = null;
            }
        }
#endif

        public static DVDDiskInfo ParseDVD(string videoTSDir)
        {
            return FromIFO(videoTSDir);
#if HANDBRAKE
            if (handbrakePath == null)
                return null;
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = Path.Combine(handbrakePath, "HandBrakeCLI.exe");
                proc.StartInfo.Arguments = string.Format(@"-i ""{0}"" -t0 -v", videoTSDir);
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardError = true;

                proc.Start();

                ManualResetEvent wait = new ManualResetEvent(false);
                DVDDiskInfo ret = null;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    ret = DVDDiskInfo.Parse(proc.StandardError);
                    wait.Set();
                });

                proc.WaitForExit();
                wait.WaitOne();
                return ret;
            }
#endif
        }

        public static void Test(string videoTS_Folder)
        {
            DVDDiskInfo thisDVD = ParseDVD(videoTS_Folder);
            XmlSerializer ser = new XmlSerializer(typeof(DVDDiskInfo));
            StringWriter outStr = new StringWriter();
            ser.Serialize(outStr, thisDVD);
            Console.WriteLine(outStr.ToString());
        }

        public DVDTitle GetMainTitle()
        {
            if (this.Titles == null || this.Titles.Length == 0)
                return null;
            if (this.Titles.Length == 1)
                return this.Titles[0];
            List<DVDTitle> list = new List<DVDTitle>(this.Titles);
            list.Sort(delegate(DVDTitle a, DVDTitle b) { return -a.Duration.CompareTo(b.Duration); });
            return list[0];
        }

        public static DVDDiskInfo FromIFO(string videoTSDir)
        {
            if (string.Compare(new DirectoryInfo(videoTSDir).Name, "VIDEO_TS", true) != 0)
                videoTSDir = Path.Combine(videoTSDir, "VIDEO_TS");
            return new DVDDiskInfo() { Titles = IFOUtilities.Titles(videoTSDir).ToArray() };
        }
    }

}
