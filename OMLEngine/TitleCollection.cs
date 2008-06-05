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
    public class TitleCollection : List<Title>, ISerializable
    {
        private SourceDatabase _source_database_to_use;
        private string _database_filename;
        private Hashtable _moviesByFilename = new Hashtable();
        private Hashtable _moviesByItemId = new Hashtable();

        public Hashtable MoviesByItemId
        {
            get { return _moviesByItemId; }
        }

        public Hashtable MoviesByFilename
        {
            get
            {
                Utilities.DebugLine("[TitleCollection] MoviesByFilename called");
                return _moviesByFilename;
            }
        }

        public void Replace(Title newTitle, Title oldTitle)
        {
            Remove(oldTitle);
            Add(newTitle);
        }

        public void Replace(Title title)
        {


            Utilities.DebugLine("[TitleCollection] Title ("+title.Name+") has been replaced");
            Title t = GetTitleById(title.InternalItemID);

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
        public Title GetTitleById(int id)
        {
            Utilities.DebugLine("[TitleCollection] Title Lookup by Id: "+id);
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
            Utilities.DebugLine("[TitleCollection] TitleCollection(database_filename)");
            _source_database_to_use = SourceDatabase.OML;
            _database_filename = database_filename;
        }
        /// <summary>
        /// Generic constructor
        /// (Uses the default database file)
        /// </summary>
        public TitleCollection()
        {
            Utilities.DebugLine("[TitleCollection] TitleCollection()");
            _source_database_to_use = SourceDatabase.OML;
            _database_filename = FileSystemWalker.RootDirectory + "\\oml.dat";
        }
        /// <summary>
        /// Default destructor
        /// </summary>
        ~TitleCollection()
        {
            Utilities.DebugLine("[TitleCollection] ~TitleCollection(): Holding " + Count + " titles");
        }

        /// <summary>
        /// Saves the current list of titles to the db file
        /// </summary>
        /// <returns>True on success</returns>
        public bool saveTitleCollection()
        {
            Utilities.DebugLine("saveTitleCollection()");

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
                Utilities.DebugLine("Error reading file: " + e.Message);
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
                Utilities.DebugLine("Error writing file: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Determines which db type to use and loads data from that source
        /// </summary>
        /// <returns>True on success</returns>
        public bool loadTitleCollection()
        {
            Utilities.DebugLine("[TitleCollection] :loadTitleCollection()");
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
            Utilities.DebugLine("[TitleCollection] Using OML database");
            Stream stm;
            try
            {
                stm = File.OpenRead(_database_filename);
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[TitleCollection] Error loading title collection: " + ex.Message);
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
                        Title t = (Title)bf.Deserialize(stm);
                        //Utilities.DebugLine("[TitleCollection] Adding Title: "+t.Name);
                        Add(t);
                        try
                        {
                            _moviesByFilename.Add(t.FileLocation, t);
                        }
                        catch (Exception e)
                        {
                            //Utilities.DebugLine("Failed to add Title to _moviesByFilename (" + t.Name + "): " + e.Message);
                        }
                        _moviesByItemId.Add(t.InternalItemID, t);
                    }
                    stm.Close();
                    Utilities.DebugLine("[TitleCollection] Loaded: " + numTitles + " titles");
                    return true;
                }
                catch (Exception e)
                {
                    Utilities.DebugLine("[TitleCollection] Failed to load db file: " + e.Message);
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
            Utilities.DebugLine("[TitleCollection] Converting TitleCollection to a DataTable");
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
            Utilities.DebugLine("[TitleCollection] TitleCollection (Serialized)");
            TitleCollection tc = (TitleCollection)info.GetValue("TitleCollection", typeof(TitleCollection));
            foreach (Title title in tc)
                Add(title);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            Utilities.DebugLine("[TitleCollection] GetObjectData()");
            info.AddValue("TitleCollection", this);
        }
        #endregion
    }
}
