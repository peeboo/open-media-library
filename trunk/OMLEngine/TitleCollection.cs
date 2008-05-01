using System;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OMLEngine
{
    [Serializable()]
    public class TitleCollection : ArrayList, ISerializable
    {
        private bool _useDatabase;
        private List<Title> _titles;
        private bool _NeedSetup = false;
        private bool _IsReadOnly = false;
        private string _database_filename;

        #region ICollection properties and methods
        public bool IsReadOnly
        {
            get { return _IsReadOnly; }
            set { _IsReadOnly = value; }
        }
        public bool NeedSetup
        {
            get { return _NeedSetup; }
            set { _NeedSetup = value; }
        }
        public int Count
        {
            get { return _titles.Count; }
        }
        public IEnumerator GetEnumerator()
        {
            return new TitleEnum(_titles.ToArray());
        }
        /*
        public IEnumerable<Title> GetEnumerator()
        {
        }
        */
        public void Add(Title title)
        {
            if (!_titles.Contains(title))
                _titles.Add(title);

            if (_NeedSetup == true)
                _NeedSetup = false;
        }
        public void Clear()
        {
            _titles = null;
        }
        public bool Contains(Title title)
        {
            return _titles.Contains(title);
        }
        public bool Remove(Title title)
        {
            bool result = _titles.Remove(title);
            if (_titles.Count == 0)
                _NeedSetup = true;

            return result;
        }
        public void CopyTo(Array array, int index)
        {
            return;
            /*
            int i = index;
            foreach (Title title in _titles)
            {
                array[i] = title;
                i++;
            }
            */
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public Object SyncRoot {
            get { return this; }
        }
        #endregion

        public TitleCollection(string database_filename)
        {
            Trace.WriteLine("TitleCollection:TitleCollection(database_filename)");
            _useDatabase = true;
            _database_filename = database_filename;
            _titles = new List<Title>();
            if (_titles.Count == 0)
                _NeedSetup = true;
        }
        public TitleCollection()
        {
            Trace.WriteLine("TitleCollection:TitleCollection()");
            _useDatabase = true;
            _database_filename = FileSystemWalker.RootDirectory + "\\oml.dat";
            _titles = new List<Title>();
            if (_titles.Count == 0)
                _NeedSetup = true;
        }
        ~TitleCollection()
        {
            Trace.WriteLine("TitleCollection:~TitleCollection(): Holding " + _titles.Count + " titles");
        }

        public bool saveTitleCollection()
        {
            Trace.WriteLine("saveTitleCollection()");
            if (_useDatabase)
            {
                Stream stream;
                try
                {
                    stream = File.Open(_database_filename, FileMode.OpenOrCreate);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Error reading file: " + e.Message);
                    return false;
                }
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, _titles.Count);

                foreach (Title title in _titles)
                    bformatter.Serialize(stream, title);

                stream.Close();
            }

            return true;
        }
        public bool loadTitleCollection()
        {
            Trace.WriteLine("loadTitleCollection()");
            if (_useDatabase)
            {
                Stream stm;
                try
                {
                    stm = File.Open(_database_filename, FileMode.OpenOrCreate);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error loading title collection: " + ex.Message);
                    return false;
                }

                if (stm.Length > 0)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    int numTitles = (int)bf.Deserialize(stm);
                    for (int i = 0; i < numTitles; i++)
                    {
                        _titles.Add((Title)bf.Deserialize(stm));
                    }
                    stm.Close();
                    Trace.WriteLine("Loaded: " + numTitles + " titles");
                    return true;
                }
                return false;
            }
            return true;
        }

        public DataTable ToDataTable()
        {
            DataTable dt = new DataTable("Titles");
            dt.Columns.Add("Name").DataType = typeof(string);
            dt.Columns.Add("Description").DataType = typeof(string);
            dt.Columns.Add("FileLocation").DataType = typeof(string);
            dt.Columns.Add("id").DataType = typeof(int);
            dt.Columns.Add("front_boxart").DataType = typeof(string);
            dt.Columns.Add("back_boxart").DataType = typeof(string);
            dt.Columns.Add("Runtime");
            dt.Columns.Add("Rating");
            dt.Columns.Add("Synopsis");
            dt.Columns.Add("Distributor");
            dt.Columns.Add("Country");
            dt.Columns.Add("Website");
            dt.Columns.Add("ReleaseDate");
            dt.Columns.Add("DateAdded");
            dt.Columns.Add("Source");
            dt.Columns.Add("Actors");
            dt.Columns.Add("Crew");
            dt.Columns.Add("Directors");
            dt.Columns.Add("Writers");
            dt.Columns.Add("Producers");
            dt.Columns.Add("SoundFormats");
            dt.Columns.Add("LanguageFormats");
            dt.Columns.Add("Genres");

            foreach (Title ti in _titles)
            {
                DataRow row = dt.NewRow();
                row["Name"] = ti.Name;
                row["Description"] = ti.Description;
                row["FileLocation"] = ti.FileLocation;
                row["id"] = ti.itemId;
                row["front_boxart"] = ti.front_boxart_path;
                row["back_boxart"] = ti.back_boxart_path;
                row["Runtime"] = ti.Runtime;
                row["Rating"] = ti.MPAARating;
                row["Synopsis"] = ti.Synopsis;
                row["Distributor"] = ti.Distributor;
                row["Country"] = ti.Country_Of_Origin;
                row["Website"] = ti.Official_Website_Url;
                row["ReleaseDate"] = ti.ReleaseDate;
                row["DateAdded"] = ti.DateAdded;
                row["Source"] = ti.Importer_Source;
                row["Actors"] = ti.Actors;
                row["Crew"] = ti.Crew;
                row["Directors"] = ti.Directors;
                row["Writers"] = ti.Writers;
                row["Producers"] = ti.Producers;
                row["SoundFormats"] = ti.SoundFormats;
                row["LanguageFormats"] = ti.LanguageFormats;
                row["Genres"] = ti.Genres;

                dt.Rows.Add(row);
            }
            return dt;
        }

        #region serialization methods
        public TitleCollection(SerializationInfo info, StreamingContext ctxt)
        {
            Trace.WriteLine("TitleCollection:TitleCollection (Serialized)");
            _titles = (List<Title>)info.GetValue("titles", typeof(List<Title>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            Trace.WriteLine("TitleCollection:GetObjectData()");
            info.AddValue("titles", _titles);
        }
        #endregion
    }

    public class TitleEnum : IEnumerator
    {
        public Title[] _title;
        int position = -1;

        public TitleEnum(Title[] list)
        {
            _title = list;
        }
        public bool MoveNext()
        {
            position++;
            return (position < _title.Length);
        }
        public void Reset()
        {
            position = -1;
        }
        public object Current
        {
            get
            {
                try
                {
                    return _title[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
