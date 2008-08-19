using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using OMLEngine;

namespace OMLSDK
{
    public class OMLPlugin :IOMLPlugin
    {
        List<Title> titles;
        private int totalRowsAdded = 0;
        private Boolean _copyImages = false;
        public static string MyMoviesXslTransform =
            Path.Combine(FileSystemWalker.PluginsDirectory, @"MyMoviesToOML.xsl");

        public enum PluginTypes { ImportPlugin, MetadataPlugin };
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public Boolean CanCopyImages
        {
            get { return GetCanCopyImages(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean CanProcessDir
        {
            get { return GetProcessDir(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean CanProcessFiles
        {
            get { return GetProcessFiles(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean CanProcessFile
        {
            get { return GetProcessFile(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean CopyImages
        {
            get { return GetCopyImages(); }
            set { _copyImages = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return GetDescription(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Filter
        {
            get { return GetFilter(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 FilterIndex
        {
            get { return GetFilterIndex(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean FolderSelect
        {
            get { return GetFolderSelect(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Menu
        {
            get { return GetMenu();  }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean MultiSelect
        {
            get { return GetMultiselect(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return GetName(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int TotalTitlesAdded
        {
            get { return totalRowsAdded; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Version
        {
            get { return GetVersion(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double VersionMajor
        {
            get { return GetVersionMajor(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double VersionMinor
        {
            get { return GetVersionMinor(); }
        }

        #endregion

        public virtual string DefaultFileToImport()
        {
            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string[] GetWork()
        {
            if (this.FolderSelect)
            {
                return GetFolder();
            }
            else
            {
                return GetFiles();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string[] GetFolder()
        {
            FolderBrowserDialog ofDiag = new FolderBrowserDialog();
            DialogResult res = ofDiag.ShowDialog();
            if (res == DialogResult.OK)
            {
                return new string[] { ofDiag.SelectedPath };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string[] GetFiles()
        {
            OpenFileDialog ofDiag = new OpenFileDialog();
            ofDiag.InitialDirectory = Properties.Settings.Default.InitialDirectory; //"c:\\";
            ofDiag.Filter = this.Filter;
            ofDiag.FilterIndex = this.FilterIndex;
            ofDiag.RestoreDirectory = true;
            ofDiag.AutoUpgradeEnabled = true;
            ofDiag.CheckFileExists = true;
            ofDiag.CheckPathExists = true;
            ofDiag.Multiselect = this.MultiSelect;
            ofDiag.Title = String.Format(@"Select {0} file to import", Name);
            DialogResult dlgRslt = ofDiag.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                string sPath = string.Empty;
                Utilities.DebugLine("[OMLPlugin] " + String.Format(@"[OMLImporter] Valid file found ({0})", ofDiag.FileName));
                if (this.MultiSelect)
                {
                    sPath = Path.GetDirectoryName(ofDiag.FileNames[0]);
                }
                else
                {
                    sPath = Path.GetDirectoryName(ofDiag.FileName);
                }
                if (!string.IsNullOrEmpty(sPath))
                {
                    Properties.Settings.Default.InitialDirectory = sPath;
                    Properties.Settings.Default.Save();
                }
                if (this.MultiSelect)
                {
                    return ofDiag.FileNames;
                }
                else
                {
                    return new string[] { ofDiag.FileName };
                }
            }
            else
            {
                return null;
            }
        }

        public delegate void FileFoundEventHandler(object sender, PlugInFileEventArgs e);
        public delegate void FileNotFoundEventHandler(object sender, System.ArgumentException e);
        public event FileFoundEventHandler FileFound;
        public event FileNotFoundEventHandler FileNotFound;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnFileFound(PlugInFileEventArgs e)
        {
            if (FileFound != null)
                FileFound(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnFileNotFound(ArgumentException e)
        {
            if (FileNotFound != null)
                FileNotFound(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thework"></param>
        public virtual void DoWork(string[] thework)
        {
            if (thework != null)
            {
                foreach (string file in thework)
                {
                    if (File.Exists(file) || Directory.Exists(file))
                    {
                        OnFileFound(new PlugInFileEventArgs(file));
                        ProcessFile(file);
                    }
                    else
                    {
                        OnFileNotFound(new ArgumentException("File not found", file));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public virtual bool Load(string filename)
        {
            return Load(filename, CopyImages);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSingleFileImporter()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            return String.Format("{0:00.00}.{1:00.00}", VersionMajor, VersionMinor);
        }

        #region Need to be implemented in descendant class
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string SetupDescription()
        {
            throw new Exception(@"You must implement this method in your class.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAuthor()
        {
            throw new Exception(@"You must implement this method in your class.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDescription()
        {
            throw new Exception(@"You must implement this method in your class.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetMenu()
        {
            throw new Exception(@"You must implement this method in your class.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetName()
        {
            throw new Exception(@"You must implement this method in your class.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual double GetVersionMajor()
        {
            return 0.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual double GetVersionMinor()
        {
            return 0.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="ShouldCopyImages"></param>
        /// <returns></returns>
        public virtual bool Load(string filename, bool ShouldCopyImages)
        {
            throw new Exception(@"You must implement this method in your class.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fPath"></param>
        public virtual void ProcessDir(string fPath)
        {
            throw new Exception(@"You must implement this method in your class.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sFiles"></param>
        public virtual void ProcessFiles(string[] sFiles)
        {
            throw new Exception(@"You must implement this method in your class.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public virtual void ProcessFile(string file)
        {
            throw new Exception(@"You must implement this method in your class.");
        }

        #endregion

        #region Overridable Stubs for readonly properties

        /// <summary>
        /// Returns a boolean if the plugin can copy images
        /// </summary>
        /// <returns>Default: false</returns>
        protected virtual Boolean GetCanCopyImages() { return false; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean GetCopyImages() { return _copyImages; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFilter()
        {
            return @"All files (*.*)|*.*";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Int32 GetFilterIndex() { return 1; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean GetMultiselect() { return false; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean GetFolderSelect() { return false; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean GetProcessDir() { return false; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean GetProcessFiles() { return false; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Boolean GetProcessFile() { return false; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<Title> GetTitles()
        {
            titles.Sort();
            return titles;
        }

        /// <summary>
        /// 
        /// </summary>
        public OMLPlugin()
        {
            titles = new List<Title>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Title newTitle()
        {
            return new Title();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newTitle"></param>
        public void AddTitle(Title newTitle)
        {
            if (string.IsNullOrEmpty(newTitle.AspectRatio))
            {
                Utilities.DebugLine("[OMLPlugin] Setting AspectRatio for Title: " + newTitle.Name);
                //SetAspectRatio(newTitle);
            }

            if (newTitle.DateAdded.CompareTo(new DateTime(0001, 1, 1)) == 0)
            {
                Utilities.DebugLine("[OMLPlugin] Setting Date Added for title: " + newTitle.Name);
                newTitle.DateAdded = DateTime.Now;
            }

            titles.Add(newTitle);
            totalRowsAdded++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title_to_validate"></param>
        /// <returns></returns>
        public bool ValidateTitle(Title title_to_validate) { return true; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_extension"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        public static void BuildResizedMenuImage(Title t)
        {
            try
            {
                if (t.FrontCoverPath.Length > 0)
                {
                    using (Image coverArtImage = Utilities.ReadImageFromFile(t.FrontCoverPath))
                    {

                        if (coverArtImage != null)
                        {
                            using (Image menuCoverArtImage = Utilities.ScaleImageByHeight(coverArtImage, 200))
                            {
                                string img_path = FileSystemWalker.ImageDirectory +
                                              @"\MF" + t.InternalItemID + ".jpg";
                                menuCoverArtImage.Save(img_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                                t.FrontCoverMenuPath = img_path;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[OMLPlugin] Exception: " + ex.Message);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from_location"></param>
        /// <param name="to_location"></param>
        /// <returns></returns>
        public static string CopyImage(string from_location, string to_location)
        {
            FileInfo fi = new FileInfo(from_location);
            fi.CopyTo(to_location, true);
            return fi.Name;
        }
        public void SetFrontCoverImage(ref Title newTitle, string imagePath)
        {
            if (!File.Exists(imagePath)) return;

            FileInfo fi;
            try {
                fi = new FileInfo(imagePath);
                string new_full_name = OMLEngine.FileSystemWalker.ImageDirectory +
                                                   "\\F" + newTitle.InternalItemID +
                                                   fi.Extension;
                if (CopyImages)
                {
                    CopyImage(imagePath, new_full_name);
                    imagePath = new_full_name;
                }

                newTitle.FrontCoverPath = imagePath;
            }
            catch (Exception e) { Utilities.DebugLine("[OMLPlugin] " + e.Message); }
        }
    }

    public class PlugInFileEventArgs : System.EventArgs
    {
        public string FileName;

        public PlugInFileEventArgs(string filename)
        {
            this.FileName = filename;
        }
    }
}

