using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using StSana;

namespace OMLEngine
{
    public class StSanaServices
    {
        public delegate void SSEventHandler(string message);
        public event SSEventHandler Log;

        public List<Title> CreateTitlesFromPathArray(int? parentid, string[] path, string Tag)
        {
            // Create a list of all newly added titles
            List<Title> NewTitles = new List<Title>();
            bool titleCreated = false;
            Title folderTitle;
            foreach (string file in path)
            {
                try
                {
                    if (Directory.Exists(file))
                    {
                        // Folder passed in. This is where St Sana kicks in
                        Servant stsana = new Servant();
                        stsana.Log += new Servant.SSEventHandler(StSana_Log);
                        stsana.BasePaths.Add(file);
                        stsana.Scan();

                        int? a_parent;

                        if (OMLEngine.Settings.OMLSettings.StSanaCreateTLFolder)
                        {
                            titleCreated = false;
                            folderTitle = TitleCollectionManager.CreateFolderNonDuplicate(parentid, Path.GetFileName(file), TitleTypes.Collection, null, out titleCreated);
                            a_parent = folderTitle.Id;
                            if (titleCreated) NewTitles.Add(folderTitle);
                        }
                        else
                        {
                            a_parent = parentid;
                        }

                        if (stsana.Entities != null)
                        {
                            foreach (Entity e in stsana.Entities)
                            {
                                int? e_parent = a_parent;
                                if (e.Name != file)
                                {
                                    switch (e.EntityType)
                                    {
                                        case Serf.EntityType.COLLECTION:
                                        case Serf.EntityType.MOVIE:
                                            if ((e.Series.Count() > 1) || (OMLEngine.Settings.OMLSettings.StSanaAlwaysCreateMovieFolder))
                                            {
                                                titleCreated = false;
                                                folderTitle = TitleCollectionManager.CreateFolderNonDuplicate(a_parent, e.Name, TitleTypes.Collection, null, out titleCreated);
                                                e_parent = folderTitle.Id;
                                                if (titleCreated) NewTitles.Add(folderTitle);
                                            }
                                            else
                                            {
                                                e_parent = a_parent;
                                            }
                                            break;
                                        case Serf.EntityType.TV_SHOW:
                                            titleCreated = false;
                                            folderTitle = TitleCollectionManager.CreateFolderNonDuplicate(a_parent, e.Name, TitleTypes.TVShow, null, out titleCreated);
                                            e_parent = folderTitle.Id;
                                            if (titleCreated) NewTitles.Add(folderTitle);
                                            break;
                                    }
                                }

                                foreach (Series s in e.Series)
                                {
                                    int? s_parent = e_parent;
                                    // if the s.name and e.name are the same, its a movie, to be sure though lets check s.number, it should be
                                    // -1 for non tv shows.
                                    if (s.Name.ToUpperInvariant().CompareTo(e.Name.ToUpperInvariant()) != 0 || s.Number > -1)
                                    {
                                        //s_parent = CreateFolderNonDuplicate(e_parent, s.Name, TitleTypes.Collection, false);
                                        if (s.Name != e.Name)
                                        {
                                            switch (e.EntityType)
                                            {
                                                case Serf.EntityType.COLLECTION:
                                                case Serf.EntityType.MOVIE:
                                                    if ((e_parent == a_parent) || (OMLEngine.Settings.OMLSettings.StSanaAlwaysCreateMovieFolder))
                                                    {
                                                        titleCreated = false;
                                                        folderTitle = TitleCollectionManager.CreateFolderNonDuplicate(e_parent, s.Name, TitleTypes.Collection, null, out titleCreated);
                                                        s_parent = folderTitle.Id;
                                                        if (titleCreated) NewTitles.Add(folderTitle);
                                                    }
                                                    else
                                                    {
                                                        s_parent = e_parent;
                                                    }
                                                    break;
                                                case Serf.EntityType.TV_SHOW:
                                                    titleCreated = false;
                                                    folderTitle = TitleCollectionManager.CreateFolderNonDuplicate(e_parent, s.Name, TitleTypes.Season, (short)s.Number, out titleCreated);
                                                    s_parent = folderTitle.Id;
                                                    if (titleCreated) NewTitles.Add(folderTitle);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            s_parent = e_parent;
                                        }
                                    }

                                    foreach (Video v in s.Videos)
                                    {
                                        StSana_Log("Processing " + v.Name);
                                        //int v_parent = CreateFolder(s_parent, Path.GetFileNameWithoutExtension(v.Name), TitleTypes.Collection, false);

                                        List<Disk> disks = new List<Disk>();

                                        if ((e.EntityType == Serf.EntityType.COLLECTION) ||
                                            (e.EntityType == Serf.EntityType.MOVIE))
                                        {
                                            // Collection or movie mode. Create one title per folder with multiple disks
                                            foreach (string f in v.Files)
                                            {
                                                if (!TitleCollectionManager.ContainsDisks(OMLEngine.FileSystem.NetworkScanner.FixPath(f)))
                                                {
                                                    Disk disk = new Disk();
                                                    disk.Path = f;
                                                    disk.Format = disk.GetFormatFromPath(f); // (VideoFormat)Enum.Parse(typeof(VideoFormat), fileExtension.ToUpperInvariant());
                                                    disk.Name = string.Format("Disk {0}", 0);
                                                    if (disk.Format != VideoFormat.UNKNOWN)
                                                    {
                                                        disks.Add(disk);
                                                    }
                                                }
                                            }
                                            if (disks.Count != 0)
                                            {
                                                Title newTitle = TitleCollectionManager.CreateTitle(s_parent,
                                                    Path.GetFileNameWithoutExtension(v.Name),
                                                    TitleTypes.Unknown,
                                                    Tag,
                                                    disks.ToArray());

                                                CheckDiskPathForImages(newTitle, disks[0]);
                                                NewTitles.Add(newTitle);
                                                TitleCollectionManager.SaveTitleUpdates();
                                            }
                                        }
                                        else
                                        {
                                            // TV mode. Create one title per file, each with single disks
                                            foreach (string f in v.Files)
                                            {
                                                if (!TitleCollectionManager.ContainsDisks(OMLEngine.FileSystem.NetworkScanner.FixPath(f)))
                                                {
                                                    Disk disk = new Disk();
                                                    disk.Path = f;
                                                    disk.Format = disk.GetFormatFromPath(f); //(VideoFormat)Enum.Parse(typeof(VideoFormat), fileExtension.ToUpperInvariant());
                                                    disk.Name = string.Format("Disk {0}", 0);
                                                    if (disk.Format != VideoFormat.UNKNOWN)
                                                    {
                                                        disks.Add(disk);
                                                    }
                                                }
                                                if (disks.Count != 0)
                                                {
                                                    short episodeno = 0;
                                                    if (v.EpisodeNumbers.Count > 0) episodeno = (short)v.EpisodeNumbers[0];

                                                    Title newTitle = TitleCollectionManager.CreateTitle(s_parent,
                                                        Path.GetFileNameWithoutExtension(f),
                                                        TitleTypes.Episode,
                                                        Tag,
                                                        (short)s.Number,
                                                        episodeno,
                                                        disks.ToArray());

                                                    CheckDiskPathForImages(newTitle, disks[0]);
                                                    NewTitles.Add(newTitle);
                                                    TitleCollectionManager.SaveTitleUpdates();

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (File.Exists(file))
                    {
                        StSana_Log("Processing " + file);
                        string extension = Path.GetExtension(file).ToUpper().Replace(".", "");
                        extension = extension.Replace("-", "");

                        Disk disk = new Disk();
                        disk.Path = file;
                        disk.Format = disk.GetFormatFromPath(file); // (VideoFormat)Enum.Parse(typeof(VideoFormat), extension.ToUpperInvariant());
                        disk.Name = string.Format("Disk {0}", 0);

                        if (disk.Format != VideoFormat.UNKNOWN)
                        {
                            Title newTitle = TitleCollectionManager.CreateTitle(parentid,
                                Path.GetFileNameWithoutExtension(file),
                                TitleTypes.Unknown,
                                Tag,
                                new Disk[1] { disk });

                            CheckDiskPathForImages(newTitle, disk);
                            NewTitles.Add(newTitle);
                            TitleCollectionManager.SaveTitleUpdates();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.DebugLine("[OMLEngine] CreateTitlesFromPathArray exception" + ex.Message);
                }
            }
            return NewTitles;
        }

        private void CheckDiskPathForImages(Title title, Disk disk)
        {
            if ((disk == null) || (string.IsNullOrEmpty(disk.Path))) return;

            string diskFolder = disk.GetDiskFolder;
            string diskPathWithExtension = null;
            string diskPathWithoutExtension = null;

            if (!string.IsNullOrEmpty(disk.GetDiskFile))
            {
                diskPathWithExtension = disk.Path;
                diskPathWithoutExtension = disk.GetDiskFolder + "\\" + Path.GetFileNameWithoutExtension(disk.GetDiskFile);
            }

            string image = null;

            // If the Disk is a media file, look for an image in the disk 
            // folder with the same name as the media file.
            if (!string.IsNullOrEmpty(diskPathWithExtension))
            {
                if (File.Exists(diskPathWithExtension + ".jpg"))
                {
                    image = diskPathWithExtension + ".jpg";
                }
                else if (File.Exists(diskPathWithExtension + ".png"))
                {
                    image = diskPathWithExtension + ".png";
                }
                else if (File.Exists(diskPathWithoutExtension + ".jpg"))
                {
                    image = diskPathWithoutExtension + ".jpg";
                }
                else if (File.Exists(diskPathWithoutExtension + ".png"))
                {
                    image = diskPathWithoutExtension + ".png";
                }
            }

            // Look for a generic folder.xxx image
            if (string.IsNullOrEmpty(image))
            {
                if (File.Exists(Path.Combine(diskFolder, "folder.jpg")))
                {
                    image = Path.Combine(diskFolder, "folder.jpg");
                }
                else if (File.Exists(Path.Combine(diskFolder, "folder.png")))
                {
                    image = Path.Combine(diskFolder, "folder.png");
                }
            }

            // Look for any jpg image
            if (string.IsNullOrEmpty(image))
            {
                string[] imagefiles = Directory.GetFiles(diskFolder, "*.jpg");
                if (imagefiles.Count() > 0)
                {
                    image = imagefiles[0];
                }
            }

            // Look for any jpg image
            if (string.IsNullOrEmpty(image))
            {
                string[] imagefiles = Directory.GetFiles(diskFolder, "*.png");
                if (imagefiles.Count() > 0)
                {
                    image = imagefiles[0];
                }
            }


            if (!string.IsNullOrEmpty(image))
            {
                title.FrontCoverPath = image;
            }

            // Check for fanart
            string fanartfolder = Path.Combine(diskFolder, "Fanart");
            if (Directory.Exists(fanartfolder))
            {
                foreach (string imagefile in Directory.GetFiles(fanartfolder))
                {
                    string extension = Path.GetExtension(imagefile);
                    if (!string.IsNullOrEmpty(extension))
                    {
                        if ((string.Compare(extension, ".jpg", true) == 0) ||
                            (string.Compare(extension, ".png", true) == 0) ||
                            (string.Compare(extension, ".bmp", true) == 0))
                        {
                            title.AddFanArtImage(imagefile);
                        }
                    }
                }
            }
        }

        void StSana_Log(string message)
        {
            if (Log != null)
                Log(message);
        }

    }
}
