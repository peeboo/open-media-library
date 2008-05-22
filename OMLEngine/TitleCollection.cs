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
    public class TitleCollection : SortedArrayList, ISerializable
    {
        private SourceDatabase _source_database_to_use;
        private string _database_filename;

        public void Replace(Title title)
        {
            Title t = find_for_id(title.InternalItemID);
            if (t != null)
            {
                int index = this.IndexOf(t);
                this[index] = title;
            }
            else
            {
                Add(title);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Title find_for_id(int id)
        {
            foreach (Title title in this)
            {
                if (title.InternalItemID == id)
                    return title;
            }
            return null;
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
        }
        /// <summary>
        /// Default destructor
        /// </summary>
        ~TitleCollection()
        {
            Trace.WriteLine("TitleCollection:~TitleCollection(): Holding " + Count + " titles");
        }

        /// <summary>
        /// Saves the current list of titles to the db file
        /// </summary>
        /// <returns>True on success</returns>
        public bool saveTitleCollection()
        {
            Trace.WriteLine("saveTitleCollection()");

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
                bformatter.Serialize(stream, Count);

                foreach (Title title in this)
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
            Sort();
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
                        Add((Title)bf.Deserialize(stm));
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
            
            DataColumn idColumn = dt.Columns.Add("id");
            idColumn.DataType = typeof(int);
            dt.PrimaryKey = new DataColumn[] { idColumn };
            dt.Columns.Add("FrontCoverPath").DataType = typeof(string);
            dt.Columns.Add("BackCoverPath").DataType = typeof(string);
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
            dt.Columns.Add("UserStarRating");
            dt.Columns.Add("AspectRatio");
            dt.Columns.Add("VideoStandard");
            dt.Columns.Add("UPC");
            dt.Columns.Add("OriginalName");
            
            foreach (Title ti in this)
            {
                DataRow row = dt.NewRow();
                row["Name"] = ti.Name;
                row["Description"] = ti.Description;
                row["FileLocation"] = ti.FileLocation;
                row["id"] = ti.InternalItemID;
                row["FrontCoverPath"] = ti.FrontCoverPath;
                row["BackCoverPath"] = ti.BackCoverPath;
                row["Runtime"] = ti.Runtime;
                row["Rating"] = ti.MPAARating;
                row["Synopsis"] = ti.Synopsis;
                row["Distributor"] = ti.Distributor;
                row["Country"] = ti.CountryOfOrigin;
                row["Website"] = ti.OfficialWebsiteURL;
                row["ReleaseDate"] = ti.ReleaseDate;
                row["DateAdded"] = ti.DateAdded;
                row["Source"] = ti.ImporterSource;
                row["Actors"] = ti.Actors;
                row["Crew"] = ti.Crew;
                row["Directors"] = ti.Directors;
                row["Writers"] = ti.Writers;
                row["Producers"] = ti.Producers;
                row["SoundFormats"] = ti.SoundFormats;
                row["LanguageFormats"] = ti.LanguageFormats;
                row["Genres"] = ti.Genres;
                row["UserStarRating"] = ti.UserStarRating;
                row["AspectRatio"] = ti.AspectRatio;
                row["VideoStandard"] = ti.VideoStandard;
                row["UPC"] = ti.UPC;
                row["OriginalName"] = ti.OriginalName;
                dt.Rows.Add(row);
            }
            return dt;
        }

        #region serialization methods
        public TitleCollection(SerializationInfo info, StreamingContext ctxt)
        {
            Trace.WriteLine("TitleCollection:TitleCollection (Serialized)");
            TitleCollection tc = (TitleCollection)info.GetValue("TitleCollection", typeof(TitleCollection));
            foreach (Title title in tc)
                Add(title);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            Trace.WriteLine("TitleCollection:GetObjectData()");
            info.AddValue("TitleCollection", this);
        }
        #endregion

    }
}
