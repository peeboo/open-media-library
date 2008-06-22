using System;
using System.Collections.Generic;
using System.Data;
using OMLEngine;
using System.Drawing;
using System.IO;

namespace OMLSDK
{
    public class OMLPlugin :IOMLPlugin
    {
        List<Title> titles;
        private int totalRowsAdded = 0;

        public string Name
        {
            get { return GetName(); }
        }

        public string Version
        {
            get { return GetVersion(); }
        }

        public string Description
        {
            get { return GetDescription(); }
        }

        public string Menu
        {
            get { return GetMenu();  }
        }

        public virtual Boolean CopyImages()
        {
            return true;
        }
        public virtual Boolean FolderSelection()
        {
            return false;
        }
        public virtual string GetVersion()
        {
            throw new Exception("You must implement this method in your class.");
        }
        public virtual string GetMenu()
        {
            throw new Exception("You must implement this method in your class.");
        }
        public virtual string GetName()
        {
            throw new Exception("You must implement this method in your class.");
        }
        public virtual string GetDescription()
        {
            throw new Exception("You must implement this method in your class.");
        }
        public virtual string GetAuthor()
        {
            throw new Exception("You must implement this method in your class.");
        }
        public virtual bool Load(string filename, bool ShouldCopyImages)
        {
            throw new Exception("You must implement this method in your class.");
        }
        public int GetTotalTitlesAdded()
        {
            return totalRowsAdded;
        }
        public List<Title> GetTitles()
        {
            titles.Sort();
            return titles;
        }
        public OMLPlugin()
        {
            titles = new List<Title>();
        }
        public Title newTitle()
        {
            return new Title();
        }
        public void AddTitle(Title newTitle)
        {
            if (string.IsNullOrEmpty(newTitle.AspectRatio))
            {
                Utilities.DebugLine("Setting AspectRatio for Title: " + newTitle.Name);
                //SetAspectRatio(newTitle);
            }

            if (newTitle.DateAdded.CompareTo(new DateTime(0001, 1, 1)) == 0)
            {
                Utilities.DebugLine("Setting Date Added for title: " + newTitle.Name);
                newTitle.DateAdded = DateTime.Now;
            }

            titles.Add(newTitle);
            BuildResizedMenuImage(newTitle);
            totalRowsAdded++;
        }
        public bool ValidateTitle(Title title_to_validate)
        {
            return true;
        }
        public bool IsSupportedFormat(string file_extension)
        {
            string[] formats = Enum.GetNames(typeof(VideoFormat));
            foreach (string format in formats)
            {
                if (file_extension.ToUpper().CompareTo(format.ToUpper()) == 0)
                    return true;
            }
            return false;
        }
        public static void BuildResizedMenuImage(Title t)
        {
            if (t.FrontCoverPath.Length > 0)
            {
                using (Image coverArtImage = Image.FromFile(t.FrontCoverPath))
                {

                    if (coverArtImage != null)
                    {
                        using (Image menuCoverArtImage = Utilities.ScaleImageByHeight(coverArtImage, 200))
                        {
                            string img_path = FileSystemWalker.ImageDirectory +
                                          "\\MF" + t.InternalItemID + ".jpg";
                            menuCoverArtImage.Save(img_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                            t.FrontCoverMenuPath = img_path;
                        }
                    }
                }
            }
        }
        /*
        public void SetAspectRatio(Title t)
        {
            string fileName = string.Empty;
            if (t.VideoFormat == VideoFormat.DVD)
            {
                fileName = t.FileLocation + "\\vts_01_0.vob";
            }
            else
                fileName = t.FileLocation;

            try 
            {
                Utilities.DebugLine("Scanning Res for: " + fileName);
                Size size = Utilities.ResolutionOfVideoFile(fileName);
                if (size.Width > 0 && size.Height > 0)
                {
                    t.AspectRatio = size.Width.ToString() + "x" + size.Height.ToString();
                }
            }
            catch (Exception ex) 
            {
            }
             
        }
        */
        public static string CopyImage(string from_location, string to_location)
        {
            FileInfo fi = new FileInfo(from_location);
            fi.CopyTo(to_location, true);
            return fi.Name;
        }
    }
}
