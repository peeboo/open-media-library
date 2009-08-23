﻿using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;

using OMLGetDVDInfo;
using OMLEngine.FileSystem;

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
                value = Utilities.GetUniversalPath(value);
                //value = NetworkScanner.FixPath(value);
                /*// if it's pointing to a physical drive do the network test
                if (!string.IsNullOrEmpty(value)
                    && !value.StartsWith("\\\\") 
                    && value.Length > 2
                    && value[1] == ':')
                {
                    string uncPath;
                    
                    // if it's an attached network drive convert it to a UNC path
                    if (NetworkScanner.IsNetworkDrive(value[0], out uncPath))
                    {
                        value = uncPath + "\\" + value.Remove(0, 2);
                    }
                }*/

                if (!string.IsNullOrEmpty(value) && value.Length > 255)
                    throw new FormatException("Disk path must be 255 characters or less.");

                _disk.Path = value;
                _diskFeatures = null;
            } 
        }

        public VideoFormat Format { get { return (VideoFormat)_disk.VideoFormat; } set { _disk.VideoFormat = (byte)value; } }

        public int? MainFeatureXRes { get { return _disk.MainFeatureXRes; } set { _disk.MainFeatureXRes = value; } }

        public int? MainFeatureYRes { get { return _disk.MainFeatureYRes; } set { _disk.MainFeatureYRes = value; } }

        public string MainFeatureAspectRatio { get { return _disk.MainFeatureAspectRatio; } set { _disk.MainFeatureAspectRatio = value; } }

        public double? MainFeatureFPS { get { return _disk.MainFeatureFPS; } set { _disk.MainFeatureFPS = value; } }

        public int? MainFeatureLength { get { return _disk.MainFeatureLength; } set { _disk.MainFeatureLength = value; } }

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
                if (FileScanner.IsDVD(path))
                    return VideoFormat.DVD;

                if (FileScanner.IsBluRay(path))
                    return VideoFormat.BLURAY;

                if (FileScanner.IsHDDVD(path))
                    return VideoFormat.HDDVD;                   
            }
                
            return VideoFormat.UNKNOWN;            
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
            //FindPath();
            Format = GetFormatFromPath(Path);
            if (info.MemberCount > 3)
                ExtraOptions = info.GetString("extraOptions");
        }         
    }
}
