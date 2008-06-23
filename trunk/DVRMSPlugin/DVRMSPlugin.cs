using System;
using OMLSDK;
using OMLEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Toub.MediaCenter.Dvrms.Metadata;

namespace DVRMSPlugin
{
    public class DVRMSPlugin : OMLPlugin, IOMLPlugin
    {
        private static double VERSION = 0.1;

        public DVRMSPlugin() : base()
        {
        }

        public override bool CopyImages()
        {
            return false;
        }

        public override string GetVersion()
        {
            return "0.9.0.0";
        }

        public override string GetMenu()
        {
            return "DVRMS Movie Files";
        }
        public override string GetName()
        {
            System.Type tipe = this.GetType();
            return tipe.Name;
        }

        public override string GetDescription()
        {
            return "DVRMS File Extractor Plugin v" + Version;
        }

        public override string GetAuthor()
        {
            return "OML Development Team";
        }

        public override bool Load(string filename)
        {
            string fPath = System.IO.Path.GetDirectoryName(filename);
            ProcessDir(fPath);
            return true;
        }

        private void ProcessDir(string fPath)
        {
            ProcessFiles(fPath);

            string [] dirs = Directory.GetDirectories(fPath);
            foreach (string dir in dirs)
            {
                ProcessDir(dir);
            }

        }

        private void ProcessFiles(string fPath)
        {
            string[] files = Directory.GetFiles(fPath, @"*.dvr-ms");
            foreach (string file in files)
            {
                Title newTitle = new Title();
                newTitle.Name = Path.GetFileNameWithoutExtension(file);
                IDictionary meta;
                DvrmsMetadataEditor editor = new DvrmsMetadataEditor(file);
                meta = editor.GetAttributes();
                foreach (string item in meta.Keys)
                {
                    MetadataItem attr = (MetadataItem) meta[item];
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
                            newTitle.FileLocation = file;
                            if (!String.IsNullOrEmpty(newTitle.AspectRatio))
                            {
                                newTitle.AspectRatio = @"Widescreen";
                            }
                            string ext = Path.GetExtension(file).Substring(1).Replace(@"-", @"");
                            newTitle.VideoFormat = (VideoFormat) Enum.Parse(typeof(VideoFormat), ext, true);
                            string cover = fPath + @"\" + Path.GetFileNameWithoutExtension(file) + @".jpg";
                            if (File.Exists(cover))
                            {
                                SetFrontCoverImage(ref newTitle, cover);
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
                                string [] gen = sGenre.Split(',');
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
                            string [] credits = persona.Split(';');
                            string [] cast = credits[0].Split('/');
                            foreach (string nm in cast)
                            {
                                if (!String.IsNullOrEmpty(nm)) newTitle.AddActor(new Person(nm));
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
                            newTitle.FileLocation = file;
                            string ext = Path.GetExtension(file).Substring(1).Replace(@"-", @"");
                            newTitle.VideoFormat = (VideoFormat) Enum.Parse(typeof(VideoFormat), ext, true);
                            string cover = fPath + @"\" + Path.GetFileNameWithoutExtension(file) + @".jpg";
                            if (File.Exists(cover))
                            {
                                newTitle.FrontCoverPath = cover;
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
                        Trace.WriteLine("Error adding row: " + e.Message);
                    }
                }
                else
                    Trace.WriteLine("Error saving row");
                }
            }
    }
}
