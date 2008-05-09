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
    public class TitleCollection : ISerializable
    {
        private SourceDatabase _source_database_to_use;
        private SortedArrayList _titles;
        private string _database_filename;

        #region SortedArrayList properties and methods
        public int Count
        {
            get { return _titles.Count; }
        }
        public IEnumerator GetEnumerator()
        {
            return _titles.GetEnumerator();
        }
        private void Add(Title title)
        {
            _titles.AddSorted(title);
        }
        public void Clear()
        {
            _titles = null;
        }
        public bool Contains(Title title)
        {
            return find_for_id(title.itemId);
        }
        private void Remove(Title title)
        {
            _titles.Remove(title);
        }
        #endregion

        /// <summary>
        /// Adds a new Title to the Collection
        /// </summary>
        /// <param name="title">A new Title Object</param>
        /// <returns>TITLE_COLLECTION_STATUS (usually TC_OK)</returns>
        public TITLE_COLLECTION_STATUS AddTitle(Title title)
        {
            if (! find_for_id(title.itemId))
            {
                _titles.Add(title);
                return TITLE_COLLECTION_STATUS.TC_OK;
            }
            return TITLE_COLLECTION_STATUS.TC_TITLE_ALREADY_EXISTS;
        }

        /// <summary>
        /// Removes an existing Title from the Collection
        /// </summary>
        /// <param name="title">A Title Object</param>
        /// <returns>TITLE_COLLECTION_STATUS (usually TC_OK)</returns>
        public TITLE_COLLECTION_STATUS RemoveTitle(Title title)
        {
            if (find_for_id(title.itemId))
            {
                _titles.Remove(title);
                return TITLE_COLLECTION_STATUS.TC_OK;
            }
            return TITLE_COLLECTION_STATUS.TC_TITLE_DOES_NOT_EXIST;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool find_for_id(int id)
        {
            foreach (Title title in _titles)
            {
                if (title.itemId == id)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Get/Set the Source Database type to use
        /// </summary>
        public SourceDatabase SourceDatabaseToUse
        {
            get { return _source_database_to_use; }
            set
            {
                _source_database_to_use = value;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="database_filename">full path of filename to use as the database</param>
        public TitleCollection(string database_filename)
        {
            Trace.WriteLine("TitleCollection:TitleCollection(database_filename)");
            _source_database_to_use = SourceDatabase.OML;
            _database_filename = database_filename;
            _titles = new SortedArrayList();
        }
        /// <summary>
        /// Generic constructor
        /// (Uses the default database file)
        /// </summary>
        public TitleCollection()
        {
            Trace.WriteLine("TitleCollection:TitleCollection()");
            _source_database_to_use = SourceDatabase.OML;
            _database_filename = FileSystemWalker.RootDirectory + "\\oml.dat";
            _titles = new SortedArrayList();
        }
        /// <summary>
        /// Default destructor
        /// </summary>
        ~TitleCollection()
        {
            Trace.WriteLine("TitleCollection:~TitleCollection(): Holding " + _titles.Count + " titles");
        }

        /// <summary>
        /// Saves the current list of titles to the db file
        /// </summary>
        /// <returns>True on success</returns>
        public bool saveTitleCollection()
        {
            Trace.WriteLine("saveTitleCollection()");

            IComparer myComparer = new TitleComparer();
            _titles.Sort(myComparer);

            switch (_source_database_to_use)
            {
                case SourceDatabase.OML:
                    return _saveTitleCollectionForOML();
                case SourceDatabase.DVDProfiler:
                    return true;
                case SourceDatabase.MovieCollectorz:
                    return true;
                case SourceDatabase.MyMovies:
                    return true;
                default:
                    return true;
            }
        }

        private bool _saveTitleCollectionForOML()
        {
            Stream stream;
            try
            {
                stream = File.OpenWrite(_database_filename);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Error reading file: " + e.Message);
                return false;
            }

            try
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, _titles.Count);

                foreach (Title title in _titles)
                    bformatter.Serialize(stream, title);

                stream.Close();
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Error writing file: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Determines which db type to use and loads data from that source
        /// </summary>
        /// <returns>True on success</returns>
        public bool loadTitleCollection()
        {
            Trace.WriteLine("loadTitleCollection()");
            switch (_source_database_to_use)
            {
                case SourceDatabase.OML:
                    return _loadTitleCollectionFromOML();
                case SourceDatabase.DVDProfiler:
                    return _loadTitleCollectionFromDVDProfiler();
                case SourceDatabase.MovieCollectorz:
                    return _loadTitleCollectionFromMovieCollectorz();
                case SourceDatabase.MyMovies:
                    return _loadTitleCollectionFromMyMovies();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Loads data from MyMovies xml file
        /// </summary>
        /// <returns>True on successful load</returns>
        private bool _loadTitleCollectionFromMyMovies()
        {
            return false;
        }

        /// <summary>
        /// Loads data from MovieCollectorz xml file
        /// </summary>
        /// <returns>True on successful load</returns>
        private bool _loadTitleCollectionFromMovieCollectorz()
        {
            return false;
        }

        /// <summary>
        /// Loads data form DVDProfiler xml file
        /// </summary>
        /// <returns>True on successful load</returns>
        private bool _loadTitleCollectionFromDVDProfiler()
        {
            return false;
        }

        /// <summary>
        /// Loads data from OML Database
        /// </summary>
        /// <returns>True on successful load</returns>
        private bool _loadTitleCollectionFromOML()
        {
            Trace.WriteLine("Using OML database");
            Stream stm;
            try
            {
                stm = File.OpenRead(_database_filename);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error loading title collection: " + ex.Message);
                return false;
            }

            if (stm.Length > 0)
            {
                try
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
                catch (Exception e)
                {
                    Trace.WriteLine("Failed to load db file: " + e.Message);
                }
            }
            return false;
        }
        /// <summary>
        /// Takes all titles in the collection and creates a DataTable object for use
        /// by the Media Center UI
        /// </summary>
        /// <returns>DataTable object contains all the titles in the collection.</returns>
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
            _titles = (SortedArrayList)info.GetValue("titles", typeof(SortedArrayList));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            Trace.WriteLine("TitleCollection:GetObjectData()");
            info.AddValue("titles", _titles);
        }
        #endregion
    }
    /// <summary>
    /// Provides Enumerator functionality on the TitleCollection object
    /// </summary>
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

    public class TitleComparer : IComparer
    {
        int IComparer.Compare(Object a, Object b)
        {
            return ((new CaseInsensitiveComparer()).Compare(((Title)a).Name, ((Title)b).Name));
        }
    }
}
