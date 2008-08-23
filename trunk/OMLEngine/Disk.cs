using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using OMLGetDVDInfo;

namespace OMLEngine
{
    [Serializable]
    public class Disk : ISerializable
    {
        private string _name = "";
        private string _path = "";
        private VideoFormat _format;
        [NonSerialized]
        private DVDDiskInfo _dvdDiskInfo;

        public Disk() { }

        public Disk(string name, string path, VideoFormat format)
        {
            _name = name;
            _path = path;
            _format = format;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public VideoFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }

        public override string ToString()
        {
            return _name + ", " + _format + ", @ " + _path;
        }

        public DVDDiskInfo DVDDiskInfo
        {
            get
            {
                if (this._format != VideoFormat.DVD)
                    return null;
                if (this._dvdDiskInfo == null)
                    this._dvdDiskInfo = DVDDiskInfo.ParseDVD(this._path);
                return this._dvdDiskInfo;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("name", _name);
            info.AddValue("path", _path);
            info.AddValue("format", _format);
        }

        public Disk(SerializationInfo info, StreamingContext ctxt)
        {
            _name = info.GetString("name");
            _path = info.GetString("path");
            _format = GetSerializedVideoFormat(info, "format");
        }

        private VideoFormat GetSerializedVideoFormat(SerializationInfo info, string id)
        {
            try
            {
                return (VideoFormat)info.GetValue(id, typeof(VideoFormat));
            }
            catch (Exception e)
            {
                Utilities.DebugLine("Exception in GetSerializedVideoFormat: " + e.Message);
                return VideoFormat.DVD;
            }
        }
    }
}
