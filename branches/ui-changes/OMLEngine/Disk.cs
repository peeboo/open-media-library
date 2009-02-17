﻿using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;

using OMLGetDVDInfo;

namespace OMLEngine
{
    [Serializable]
    public class Disk : ISerializable
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public VideoFormat Format { get; set; }
        public string ExtraOptions { get; set; }


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

        #region DiskInfo
        [NonSerialized]
        public List<DIFeature> _diskFeatures;
        public List<DIFeature> DiskFeatures
        {
            get
            {
                if (_diskFeatures == null)
                {
                //    DiskFeatures = new List<DIFeature>();
                //}
                DiskInfo di = new DiskInfo(Name, Path, Format);
                _diskFeatures = di.DiskFeatures;
                }
                return _diskFeatures;
            }
            set
            { 
            }
        }
        #endregion


        public Disk() { }

        public Disk(string name, string path, VideoFormat format) 
            : this(name, path, format, null)
        { }

        public Disk(string name, string path, VideoFormat format, string extraOptions)
        {
            Name = name;
            Path = path;
            Format = format;
            ExtraOptions = string.IsNullOrEmpty(extraOptions) ? null : extraOptions;
        }

        public override string ToString()
        {
            return Name + ", " + Format + ", @ " + Path;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("name", Name);
            info.AddValue("path", Path);
            info.AddValue("format", Format);
            info.AddValue("extraOptions", ExtraOptions);
        }

        public Disk(SerializationInfo info, StreamingContext ctxt)
        {
            Name = info.GetString("name");
            Path = info.GetString("path");
            //FindPath();
            Format = GetSerializedVideoFormat(info, "format");
            if (info.MemberCount > 3)
                ExtraOptions = info.GetString("extraOptions");
        }

        static string sBasePaths = System.Configuration.ConfigurationSettings.AppSettings["BasePaths"];
        public void FindPath()
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

        static VideoFormat GetSerializedVideoFormat(SerializationInfo info, string id)
        {
            try
            {
                return (VideoFormat)info.GetValue(id, typeof(VideoFormat));
            }
            catch (Exception)
            {
                return VideoFormat.DVD;
            }
        }

    }
}
