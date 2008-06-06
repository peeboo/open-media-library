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

namespace DVRMS
{
    public class DVRMSPlugin : OMLPlugin, IOMLPlugin
    {
        private static double VERSION = 0.1;

        public DVRMSPlugin() : base()
        {
        }

        public override string GetName()
        {
            return "DVRMSPlugin";
        }

        public override string GetDescription()
        {
            return "DVRMS File Extractor Plugin";
        }

        public override string GetAuthor()
        {
            return "Thom Lamb";
        }

        public override bool Load(string filename, bool ShouldCopyImages)
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
                            } else {
                                newTitle.Name = Path.GetFileNameWithoutExtension(file);
                            }
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
                                foreach (string genre in gen)
                                {
                                    string uGenre = genre.ToUpper();
                                    if (uGenre.StartsWith("MOVIE")) continue;
                                    newTitle.AddGenre(genre.Trim());
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
                                newTitle.MPAARating = (string)attr.Value;
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
                        if (String.IsNullOrEmpty(newTitle.MPAARating))
                        {
                            newTitle.MPAARating = @"--";
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
