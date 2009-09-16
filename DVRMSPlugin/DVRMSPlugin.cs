using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using OMLEngine;
using OMLSDK;

using Toub.MediaCenter.Dvrms.Metadata;

namespace DVRMSPlugin
{
    public class DVRMSPlugin : OMLPlugin, IOMLPlugin
    {
        private static double MajorVersion = 0.9;
        private static double MinorVersion = 0.0;

        public DVRMSPlugin()
            : base()
        {
        }

        public override string SetupDescription()
        {
            return GetName() + @" will search for and import all DVR-MS files in the path(s) selected.";
        }

        public override bool IsSingleFileImporter()
        {
            return false;
        }

        protected override bool GetCopyImages()
        {
            return false;
        }

        protected override double GetVersionMajor()
        {
            return MajorVersion;
        }

        protected override double GetVersionMinor()
        {
            return MinorVersion;
        }

        protected override string GetMenu()
        {
            return @"DVR-MS Movie Files";
        }

        protected override string GetName()
        {
            System.Type tipe = this.GetType();
            return tipe.Name;
        }

        protected override string GetDescription()
        {
            return @"DVR-MS File Extractor Plugin v" + Version;
        }

        protected override string GetAuthor()
        {
            return @"OML Development Team";
        }

        protected override string GetFilter()
        {
            return @"DVR-MS files (*.dvr-ms*)|*.dvr-ms|" + base.GetFilter();
        }

        protected override bool GetMultiselect()
        {
            return true;
        }

        public override bool Load(string filename, bool ShouldCopyImages)
        {
            string fPath = System.IO.Path.GetDirectoryName(filename);
            ProcessDir(fPath);
            return true;
        }

        public override void ProcessDir(string fPath)
        {
            string[] files = Directory.GetFiles(fPath, @"*.dvr-ms");
            ProcessFiles(files);

            string[] dirs = Directory.GetDirectories(fPath);
            foreach (string dir in dirs)
            {
                ProcessDir(dir);
            }

        }

        public override void ProcessFiles(string[] sFiles)
        {
            foreach (string file in sFiles)
            {
                ProcessFile(file);
            }
        }

        public override void ProcessFile(string file)
        {
            Title newTitle = new Title();
            String fPath = Path.GetDirectoryName(file);
            newTitle.Name = Path.GetFileNameWithoutExtension(file);
            IDictionary meta;
            DvrmsMetadataEditor editor = new DvrmsMetadataEditor(file);
            meta = editor.GetAttributes();
            foreach (string item in meta.Keys)
            {
                MetadataItem attr = (MetadataItem)meta[item];
                switch (item)
                {
                    case DvrmsMetadataEditor.Title:
                        string sTitle = (string)attr.Value;
                        sTitle = sTitle.Trim();
                        if (!String.IsNullOrEmpty(sTitle))
                        {
                            newTitle.Name = sTitle;
                        }
                        newTitle.ImporterSource = @"DVRMSImporter";
                        newTitle.MetadataSourceName = @"DVR-MS";
                        Disk disk = new Disk();
                        string ext = Path.GetExtension(file).Substring(1).Replace(@"-", @"");
                        disk.Format = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                        Utilities.DebugLine("[DVRMSPlugin] Adding file: " + Path.GetFullPath(file));
                        disk.Path = Path.GetFullPath(file);
                        disk.Name = @"Disk 1";
                        newTitle.Disks.Add(disk);
                        //newTitle.FileLocation = file;
                        if (!String.IsNullOrEmpty(newTitle.AspectRatio))
                        {
                            newTitle.AspectRatio = @"Widescreen";
                        }
                        //string ext = Path.GetExtension(file).Substring(1).Replace(@"-", @"");
                        //newTitle.VideoFormat = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                        string cover = fPath + @"\" + Path.GetFileNameWithoutExtension(file) + @".jpg";
                        if (File.Exists(cover))
                        {
                            Utilities.DebugLine("[DVRMSPlugin] Setting CoverArt: " + Path.GetFullPath(cover));
                            SetFrontCoverImage(ref newTitle, Path.GetFullPath(cover));
                            //newTitle.FrontCoverPath = cover;
                        }
                        else
                        {
                            Utilities.DebugLine("[DVRMSPlugin] No coverart found");
                        }
                        break;
                    case DvrmsMetadataEditor.MediaOriginalBroadcastDateTime:
                        string sDT = (string)attr.Value;
                        if (!String.IsNullOrEmpty(sDT))
                        {
                            DateTime dt;
                            if (DateTime.TryParse(sDT, out dt))
                            {
                                newTitle.ReleaseDate = dt;
                            }
                        }
                        break;
                    case DvrmsMetadataEditor.Genre:
                        if (!String.IsNullOrEmpty((string)attr.Value))
                        {
                            string sGenre = (string)attr.Value;
                            string[] gen = sGenre.Split(',');
                            newTitle.Genres.Clear();
                            foreach (string genre in gen)
                            {
                                string uGenre = genre.ToUpper().Trim();
                                if (String.IsNullOrEmpty(uGenre)) continue;
                                if (uGenre.StartsWith(@"MOVIE")) continue;
                                uGenre = genre.Trim();
                                newTitle.AddGenre(uGenre);
                            }
                        }
                        break;
                    case DvrmsMetadataEditor.Duration:
                        Int64 rTime = (long)attr.Value;
                        rTime = rTime / 600 / 1000000;
                        newTitle.Runtime = (int)rTime;
                        break;
                    case DvrmsMetadataEditor.ParentalRating:
                        if (!String.IsNullOrEmpty((string)attr.Value))
                        {
                            newTitle.ParentalRating = (string)attr.Value;
                        }
                        break;
                    case DvrmsMetadataEditor.Credits:
                        string persona = (string)attr.Value;
                        persona += @";;;;";
                        string[] credits = persona.Split(';');
                        string[] cast = credits[0].Split('/');
                        foreach (string nm in cast)
                        {
                            if (!String.IsNullOrEmpty(nm)) newTitle.AddActingRole(nm, "");
                        }
                        string[] dirs = credits[1].Split('/');
                        if (dirs.Length > 0)
                        {
                            if (!String.IsNullOrEmpty(dirs[0]))
                            {
                                string nm = dirs[0];
                                newTitle.AddDirector(new Person(nm));
                            }
                        }

                        break;
                    case DvrmsMetadataEditor.SubtitleDescription:
                        newTitle.Synopsis = (string)attr.Value;
                        break;
                }
                attr = null;
            }

            if (ValidateTitle(newTitle))
            {
                try
                {
                    if (String.IsNullOrEmpty(newTitle.Name))
                    {
                        newTitle.Name = Path.GetFileNameWithoutExtension(file);
                        newTitle.ImporterSource = @"DVRMSImporter";
                        newTitle.MetadataSourceName = @"DVR-MS";
                        Disk disk = new Disk();
                        disk.Name = @"Disk 1";
                        disk.Path = file;
                        //newTitle.FileLocation = file;
                        string ext = Path.GetExtension(file).Substring(1).Replace(@"-", @"");
                        //newTitle.VideoFormat = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                        disk.Format = (VideoFormat)Enum.Parse(typeof(VideoFormat), ext, true);
                        newTitle.Disks.Add(disk);
                        string cover = fPath + @"\" + Path.GetFileNameWithoutExtension(file) + @".jpg";
                        if (File.Exists(Path.GetFullPath(cover)))
                        {
                            Utilities.DebugLine("[DVRMSPlugin] Setting CoverArt: " + Path.GetFullPath(cover));
                            newTitle.FrontCoverPath = cover;
                        }
                        else
                        {
                            Utilities.DebugLine("[DVRMSPlugin] No coverart found");
                        }
                    }
                    if (String.IsNullOrEmpty(newTitle.AspectRatio))
                    {
                        newTitle.AspectRatio = @"Widescreen";
                    }
                    if (String.IsNullOrEmpty(newTitle.ParentalRating))
                    {
                        newTitle.ParentalRating = @"--";
                    }
                    AddTitle(newTitle);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("[DVRMSPlugin] Error adding row: " + e.Message);
                }
            }
            else
            {
                Trace.WriteLine("[DVRMSPlugin] Error saving row");
            }
        }
    }
}
