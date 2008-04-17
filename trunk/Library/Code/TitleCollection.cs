using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Library
{
    [Serializable()]
    public class TitleCollection : ISerializable
    {
        private List<Title> _titles;
        private bool _NeedSetup = false;

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
            return _titles.GetEnumerator();
        }

        public TitleCollection()
        {
            Trace.WriteLine("TitleCollection:TitleCollection()");
            _titles = new List<Title>();
            loadTitleCollection();
        }

        ~TitleCollection()
        {
            Trace.WriteLine("TitleCollection:~TitleCollection(): Holding " + _titles.Count + " titles");
            saveTitleCollection();
        }

        public List<Title> GetTitles()
        {
            return _titles;
        }

        public void AddTitle(Title title)
        {
            if (! _titles.Contains(title))
                _titles.Add(title);
        }

        public void AddTitles(List<Title> titles)
        {
            foreach (Title title in titles)
            {
                if (!titles.Contains(title))
                    titles.Add(title);
            }
        }

        public void RemoveTitle(Title title)
        {
            if (_titles.Contains(title))
                _titles.Remove(title);
        }

        public bool saveTitleCollection()
        {
            Trace.WriteLine("saveTitleCollection()");
            Stream stream;
            try
            {
                stream = File.Open("C:\\oml.dat", FileMode.OpenOrCreate);
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
                stm = File.Open(@"c:\\oml.dat", FileMode.OpenOrCreate);
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
}
