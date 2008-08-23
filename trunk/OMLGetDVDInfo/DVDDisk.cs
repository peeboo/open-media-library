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

        public override string ToString()
        {
            return string.Join(", ", new List<DVDTitle>(Titles).ConvertAll<string>(delegate (DVDTitle t) { return "[" + t + "]"; }).ToArray());
        }

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

        public static DVDDiskInfo ParseDVD(string inputFile)
        {
            if (handbrakePath == null)
                return null;
            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = Path.Combine(handbrakePath, "HandBrakeCLI.exe");
                proc.StartInfo.Arguments = string.Format(@"-i ""{0}"" -t0 -v", inputFile);
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
            list.Sort(delegate(DVDTitle a, DVDTitle b) { return -a.m_Duration.CompareTo(b.m_Duration); });
            return list[0];
        }
    }

}
