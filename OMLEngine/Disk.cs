using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.IO;

using OMLGetDVDInfo;

namespace OMLEngine
{
    [Serializable]
    public class Disk : ISerializable
    {
        private Dao.Disk _disk;

        public string Name 
        { 
            get { return _disk.Name; } 
            set 
            {
                if (!string.IsNullOrEmpty(value) && value.Length > 255)
                    throw new FormatException("Disk name must be 255 characters or less.");
                _disk.Name = value; 
            } 
        }

        public string Path 
        { 
            get { return _disk.Path; } 
            set 
            {
                if (!string.IsNullOrEmpty(value) && value.Length > 255)
                    throw new FormatException("Disk path must be 255 characters or less.");
                _disk.Path = value; 
            } 
        }

        public VideoFormat Format { get { return (VideoFormat)_disk.VideoFormat; } set { _disk.VideoFormat = (byte)value; } }

        public string ExtraOptions 
        { 
            get { return _disk.ExtraOptions; } 
            set 
            {
                if (!string.IsNullOrEmpty(value) && value.Length > 255)
                    throw new FormatException("Disk extra options must be 255 characters or less.");
                _disk.ExtraOptions = value; 
            } 
        }

        internal Dao.Disk DaoDisk { get { return _disk; } }

        public override bool Equals(object obj)
        {
            Disk other = obj as Disk;
            if (other == null)
                return false;
            return Name == other.Name && Path == other.Path && Format == other.Format;
        }

        public override int GetHashCode()
        {
            return (Name + ":" + Path + ":" + Format.ToString()).GetHashCode();
        }

        public VideoFormat GetFormatFromPath(string path)
        {
            // Validate the new path
            if (File.Exists(path))
            {
                return (VideoFormat)Enum.Parse(typeof(VideoFormat),
                    System.IO.Path.GetExtension(Path).Replace(".", "").Replace("-", ""), true);
            }
            else if (Directory.Exists(path))
            {
                if (MediaData.IsDVD(path))
                {
                    return VideoFormat.DVD;
                }
                else if (MediaData.IsBluRay(path))
                {
                    return VideoFormat.BLURAY;
                }
                else if (MediaData.IsHDDVD(path))
                {
                    return VideoFormat.HDDVD;
                }
                else
                {
                    return VideoFormat.UNKNOWN;
                }
            }
            else
            {
                return VideoFormat.UNKNOWN;
            }
        }

        #region -- DVD Members --
        [NonSerialized]
        DVDDiskInfo _dvdDiskInfo;

        public DVDDiskInfo DVDDiskInfo
        {
            get
            {
                if (this.Format != VideoFormat.DVD)
                    return null;
                if (this._dvdDiskInfo == null)
                    this._dvdDiskInfo = DVDDiskInfo.ParseDVD(this.VIDEO_TS);
                return this._dvdDiskInfo;
            }
        }

        public string VIDEO_TS
        {
            get
            {
                if (Format != VideoFormat.DVD)
                    return null;
                if (string.Compare(new DirectoryInfo(Path).Name, "VIDEO_TS", true) == 0)
                    return Path;
                string path = System.IO.Path.Combine(Path, "VIDEO_TS");
                return Directory.Exists(path) ? path : Path;
            }
        }
        public string VIDEO_TS_Parent
        {
            get
            {
                string videoTS = VIDEO_TS;
                if (videoTS == null)
                    return null;
                return new DirectoryInfo(videoTS).Parent.FullName;
            }
        }
        #endregion

        public Disk()         
        {
            _disk = new OMLEngine.Dao.Disk();
        }

        public Disk(string name, string path, VideoFormat format) 
            : this(name, path, format, null)
        { }

        public Disk(string name, string path, VideoFormat format, string extraOptions)
            : this()
        {
            this.Name = name;
            this.Path = path;
            this.Format = format;
            this.ExtraOptions = string.IsNullOrEmpty(extraOptions) ? null : extraOptions;
        }

        internal Disk(Dao.Disk disk)
        {
            _disk = disk;
        }

        public override string ToString()
        {
            return Name + ", " + Format + ", @ " + Path;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
        }

        public Disk(SerializationInfo info, StreamingContext ctxt)
        {
            _disk = new OMLEngine.Dao.Disk();
            Name = info.GetString("name");
            Path = info.GetString("path");
            FindPath();
            Format = GetFormatFromPath(Path);
            if (info.MemberCount > 3)
                ExtraOptions = info.GetString("extraOptions");
        }

        static string sBasePaths = System.Configuration.ConfigurationSettings.AppSettings["BasePaths"];
        void FindPath()
        {
            if (Directory.Exists(Path) || File.Exists(Path))
                return;

            if (sBasePaths == null)
            {
                Utilities.DebugLine("Disk.FindPath({0}), not found [BasePaths not set in app.config]", Path);
                return;
            }

            string relPath = null;
            foreach (string basePath in sBasePaths.Split(';'))
                if (Path.ToLower().StartsWith(basePath.ToLower()))
                {
                    relPath = Path.Substring(basePath.Length + 1);
                    break;
                }

            if (relPath == null)
            {
                Utilities.DebugLine("Disk.FindPath({0}), not found [no BasePaths='{1}' match]", Path, sBasePaths);
                return;
            }

            Utilities.DebugLine("Disk.FindPath({0}), relPath = {1}", Path, relPath);
            foreach (string basePath in sBasePaths.Split(';'))
            {
                string newPath = System.IO.Path.Combine(basePath, relPath);
                if (Directory.Exists(newPath) || File.Exists(newPath))
                {
                    Utilities.DebugLine("Disk.FindPath({0}), NewPath = {1}", Path, newPath);
                    Path = newPath;
                    return;
                }
            }
            Utilities.DebugLine("Disk.FindPath({0}), no new path match found in BasePaths='{1}'", Path, sBasePaths);
        }
    }
}
