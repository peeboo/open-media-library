﻿using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.IO;

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
                return Directory.Exists(path) ? path : null;
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

        public Disk() { }

        public Disk(string name, string path, VideoFormat format)
        {
            Name = name;
            Path = path;
            Format = format;
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
            Format = GetSerializedVideoFormat(info, "format");
            if (info.MemberCount > 3)
                ExtraOptions = info.GetString("extraOptions");
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