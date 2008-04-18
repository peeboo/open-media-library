using System;
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
            return _titles.Remove(title);
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
            _database_filename = database_filename;
            _titles = new List<Title>();
        }
        public TitleCollection()
        {
            Trace.WriteLine("TitleCollection:TitleCollection()");
            _database_filename = @"C:\\oml.dat";
            _titles = new List<Title>();
        }
        ~TitleCollection()
        {
            Trace.WriteLine("TitleCollection:~TitleCollection(): Holding " + _titles.Count + " titles");
        }

        public bool saveTitleCollection()
        {
            Trace.WriteLine("saveTitleCollection()");
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

            return true;
        }
        public bool loadTitleCollection()
        {
            Trace.WriteLine("loadTitleCollection()");
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
