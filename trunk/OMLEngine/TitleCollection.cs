using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.ServiceModel;
using OMLEngineService;
using System.Security.AccessControl;
using System.Runtime.InteropServices;

namespace OMLEngine
{
    [ServiceContract]
    public interface ITitleCollectionAPI
    {
        [OperationContract]
        void Add(byte[] t);

        [OperationContract]
        void Replace(byte[] t);

        [OperationContract]
        void Remove(int ID);

        [OperationContract]
        bool ContainsDisks(IEnumerable<Disk> disks);

        [OperationContract]
        byte[] FindByID(int ID);

        [OperationContract]
        byte[] FindByDisks(IEnumerable<Disk> disks);

        [OperationContract]
        IDictionary<int, string> List();

        [OperationContract]
        void Clean();
    }

    #region -- ITitleCollectionAPI : Proxy Implementation --
    public class TitleCollectionAPIProxy
    {
        static MyClientBase<ITitleCollectionAPI> ServiceProxy()
        {
            return new MyClientBase<ITitleCollectionAPI>(WCFUtilites.NetTcpBinding(), 
                new EndpointAddress("net.tcp://localhost:8321/OMLTC"));
        }

        public void Add(Title t)
        {
            using (var host = ServiceProxy())
                host.Channel.Add(WCFUtilites.Serialize(t));
        }

        public void Replace(Title t)
        {
            using (var host = ServiceProxy())
                host.Channel.Replace(WCFUtilites.Serialize(t));
        }

        public void Remove(int ID)
        {
            using (var host = ServiceProxy())
                host.Channel.Remove(ID);
        }

        public bool ContainsDisks(IEnumerable<Disk> disks)
        {
            using (var host = ServiceProxy())
                return host.Channel.ContainsDisks(disks);
        }

        public Title FindByID(int ID)
        {
            using (var host = ServiceProxy())
                return WCFUtilites.Deserialize<Title>(host.Channel.FindByID(ID));
        }

        public Title FindByDisks(IEnumerable<Disk> disks)
        {
            using (var host = ServiceProxy())
                return WCFUtilites.Deserialize<Title>(host.Channel.FindByDisks(disks));
        }

        public IDictionary<int, string> List()
        {
            using (var host = ServiceProxy())
                return host.Channel.List();
        }

        public void Clean()
        {
            using (var host = ServiceProxy())
                host.Channel.Clean();
        }
    }
    #endregion

    #region -- ITitleCollectionAPI : Service Host Implementation --
    public class TitleCollectionAPI : ITitleCollectionAPI
    {
        static TitleCollection _list;

        static TitleCollectionAPI()
        {
            _list = new TitleCollection();
            _list.loadTitleCollection();
            _list.Sort();
        }

        static void _Save()
        {
            _list.saveTitleCollection();
            _list = new TitleCollection();
            _list.loadTitleCollection();
            _list.Sort();
        }

        public void Add(byte[] _t)
        {
            Title t = WCFUtilites.Deserialize<Title>(_t);
            Utilities.DebugLine("[TitleCollectionAPI] Add({0})", t);
            _list.Add(t);
            _Save();
        }

        public void Replace(byte[] _t)
        {
            Title t = WCFUtilites.Deserialize<Title>(_t);
            Utilities.DebugLine("[TitleCollectionAPI] Replace({0})", t);
            _list.Replace(t);
            _Save();
        }

        public void Remove(int ID)
        {
            Utilities.DebugLine("[TitleCollectionAPI] Remove({0})", ID);
            Title title = WCFUtilites.Deserialize<Title>(FindByID(ID));
            if (title == null)
                return;

            _list.Remove(title);
            _Save();
        }

        public bool ContainsDisks(IEnumerable<Disk> disks)
        {
            Utilities.DebugLine("[TitleCollectionAPI] ContainsDisks({0})", disks);
            return _list.ContainsDisks(disks);
        }

        public byte[] FindByID(int ID)
        {
            Utilities.DebugLine("[TitleCollectionAPI] FindByID({0}, Found:{1})", ID, _list.MoviesByItemId.ContainsKey(ID));
            if (_list.MoviesByItemId.ContainsKey(ID) == false)
                return null;

            return WCFUtilites.Serialize(_list.MoviesByItemId[ID]);
        }

        public byte[] FindByDisks(IEnumerable<Disk> disks)
        {
            Utilities.DebugLine("[TitleCollectionAPI] FindByDisks({0})", disks);
            return WCFUtilites.Serialize(_list.FindByDisks(disks));
        }

        public IDictionary<int, string> List()
        {
            Utilities.DebugLine("[TitleCollectionAPI] List() -> #{0}", _list.Count);
            IDictionary<int, string> ret = new Dictionary<int, string>();
            foreach (Title t in _list)
                ret[t.InternalItemID] = t.Name;
            return ret;
        }

        public void Clean()
        {
            Utilities.DebugLine("[TitleCollectionAPI] Clean()");
            _list = new TitleCollection();
            _list.saveTitleCollection();
        }
    }
    #endregion

    [Serializable()]
    public class TitleCollection : ISerializable
    {
        private List<Title> _list = new List<Title>();
        static SourceDatabase _source_database_to_use;
        static string _database_filename;
        private Dictionary<string, Title> _moviesByFilename = new Dictionary<string, Title>();
        private Dictionary<int, Title> _moviesByItemId = new Dictionary<int, Title>();
        private Dictionary<string, string> _genreMap = new Dictionary<string, string>();

        public string DBFilename
        {
            get { return _database_filename; }
        }

        public Dictionary<string, string> GenreMap
        {
            get { return _genreMap; }
        }

        public Dictionary<int, Title> MoviesByItemId
        {
            get { return _moviesByItemId; }
        }

        public Title FindByDisks(IEnumerable<Disk> disks)
        {
            string hash = GetDiskHash(disks);
            if (hash != null && _moviesByFilename.ContainsKey(hash))
                return _moviesByFilename[hash];
            return null;
        }

        public bool ContainsDisks(IEnumerable<Disk> disks)
        {
            string hash = GetDiskHash(disks);
            if (hash == null)
                return false;
            return _moviesByFilename.ContainsKey(hash);
        }

        public List<Title>.Enumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public List<Title> Source
        {
            get { return _list; }
        }

        public List<string> GetAllActors
        {
           get {
               List<string> actors = (from title in _list 
                                   from actor in title.ActingRoles 
                                   orderby actor.Key ascending
                                   select actor.Key).Distinct().ToList<string>();
                return actors;
            }
        }

        public List<string> GetAllAspectRatios
        {
            get
            {
                List<string> aspects = (from title in _list
                                        orderby title.AspectRatio
                                        select title.AspectRatio).Distinct().ToList<string>();
                return aspects;
            }
        }

        public List<string> GetAllCountryofOrigin
        {
            get
            {
                List<string> countries = (from title in _list
                                          orderby title.CountryOfOrigin
                                          select title.CountryOfOrigin).Distinct().ToList<string>();
                return countries;
            }
        }

        public List<string> GetAllDirectors
        {
            get
            {
                List<string> directors = (from title in _list
                                          from director in title.Directors
                                          orderby director.full_name
                                          select director.full_name).Distinct().ToList<string>();
                return directors;
            }
        }

        public List<string> GetAllGenres
        {
            get
            {
                List<string> genres = (from title in _list
                                       from genre in title.Genres
                                       orderby genre ascending
                                       select genre).Distinct().ToList<string>();
                return genres;
            }
        }

        public List<string> GetAllProducers
        {
            get
            {
                List<string> producers = (from title in _list
                                          from producer in title.Producers
                                          orderby producer ascending
                                          select producer).Distinct().ToList<string>();
                return producers;
            }
        }

        public List<string> GetAllStudios
        {
            get
            {
                List<string> studios = (from title in _list
                                        orderby title.Studio ascending
                                        select title.Studio).Distinct().ToList<string>();
                return studios;
            }
        }

        public List<string> GetAllTags
        {
            get
            {
                List<string> tags = (from title in _list
                                     from tag in title.Tags
                                     orderby tag ascending
                                     select tag).Distinct().ToList<string>();
                return tags;
            }
        }

        public List<string> GetAllWriters
        {
            get
            {
                List<string> writers = (from title in _list
                                        from writer in title.Writers
                                        orderby writer.full_name ascending
                                        select writer.full_name).Distinct().ToList<string>();
                return writers;
            }
        }

        public List<string> GetAllParentalRatings
        {
            get
            {
                List<string> ratings = (from title in _list
                                        orderby title.ParentalRating ascending
                                        select title.ParentalRating).Distinct().ToList<string>();
                return ratings;
            }
        }

        public List<string> GetFolders
        {
            get
            {
                List<string> folders = (from title in _list
                                        from disk in title.Disks
                                        orderby System.IO.Path.GetDirectoryName(disk.Path) ascending
                                        select System.IO.Path.GetDirectoryName(disk.Path)).Distinct().ToList<string>();
                return folders;
            }
        }

        public Title this[int index]
        {
            get 
            {
                if (index >= 0 && index < _list.Count)
                {
                    return _list[index];
                }
                else
                {
                    return null;
                }
            }
        }

        public void Sort()
        {
            _list.Sort();
        }

        public void SortBy(string propertyName, bool sortAscending)
        {
            PropertyInfo propInfo = typeof(Title).GetProperty(propertyName);
            Comparison<Title> compare = delegate(Title a, Title b)
            {
                object valueA = sortAscending ? propInfo.GetValue(a, null) : propInfo.GetValue(b, null);
                object valueB = sortAscending ? propInfo.GetValue(b, null) : propInfo.GetValue(a, null);

                return valueA is IComparable ? ((IComparable)valueA).CompareTo(valueB) : 0;
            };
            _list.Sort(compare);
        }

        public int Count
        {
            get { return _list.Count; }
        }


        public void Add(Title newTitle)
        {
            _list.Add(newTitle);

            string key = GetDiskHash(newTitle.Disks);
            if ( newTitle.Disks.Count > 0 && !_moviesByFilename.ContainsKey(key))
            {
                _moviesByFilename.Add(key, newTitle);
            }
            if (!_moviesByItemId.ContainsKey(newTitle.InternalItemID))
            {
                _moviesByItemId.Add(newTitle.InternalItemID, newTitle);
            }
        }

        public void Remove(Title newTitle)
        {
            _moviesByItemId.Remove(newTitle.InternalItemID);
            if (newTitle.Disks.Count > 0)
                _moviesByFilename.Remove(GetDiskHash(newTitle.Disks));
            _list.Remove(newTitle);
            DeleteImageNoException(newTitle.FrontCoverPath);
            DeleteImageNoException(newTitle.FrontCoverMenuPath);
            DeleteImageNoException(newTitle.BackCoverPath);
            DeleteImageNoException(newTitle.BackDropImage);
        }

        public static void DeleteImageNoException(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                OMLEngine.Utilities.DebugLine("[TitleCollection] DeleteImageNoException(" + path + ") : failed deleting image because " + e.Message);
            }
        }

        public static void ClearMirrorDataFiles()
        {
            int i = 1;
            while (File.Exists(String.Format(@"C:\Users\Mcx{0}\AppData\Local\VirtualStore\ProgramData\OpenMediaLibrary\oml.dat", i)))
            {
                File.Delete(String.Format(@"C:\Users\Mcx{0}\AppData\Local\VirtualStore\ProgramData\OpenMediaLibrary\oml.dat", i));
            }
        }

        private string GetDiskHash(IEnumerable<Disk> disks)
        {
            string temp = "";
            foreach (Disk d in disks)
            {
                // we will concatonate the path + name for each disk and take the MD5 hash of that to determine if
                // the disk collection changed
                //temp += d.Name + d.Path;

                // don't care about the disk name
                temp += d.Path.ToUpper();
            }
            if (temp == "")
                return null;
            return CalculateMD5Hash(temp);
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
                int index = _list.IndexOf(t);
                _list[index] = title;
            }
            else
            {
                Add(title);
            }
        }

        public void Clear()
        {
            _list.Clear();
            _moviesByFilename.Clear();
            _moviesByItemId.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Title GetTitleById(int id)
        {
            Utilities.DebugLine("[TitleCollection] Title Lookup by Id: "+id);
            if (_moviesByItemId.ContainsKey(id))
            {
                return _moviesByItemId[id];
            }

            return null;
        }

        public List<Title> FindByGenre(string genre)
        {
            List<Title> titles = (from title in _list
                                  from titleGenre in title.Genres
                                  where titleGenre == genre
                                  orderby title.SortName ascending
                                  select title).ToList<Title>();
            return titles;
        }

        public List<Title> FindByParentalRating(string rating)
        {
            List<Title> titles = (from title in _list
                                  where title.ParentalRating == rating
                                  orderby title.SortName ascending
                                  select title).ToList<Title>();
            return titles;
        }

        public List<Title> FindByTag(string tag)
        {
            List<Title> titles = (from title in _list
                                  from tags in title.Tags
                                  where tags == tag
                                  orderby title.SortName ascending
                                  select title).ToList<Title>();
            return titles;
        }

        public List<Title> FindByCompleteness(decimal percentComplete)
        {
            List<Title> titles = (from title in _list
                                  where title.PercentComplete <= percentComplete
                                  orderby title.SortName ascending
                                  select title).ToList<Title>();
            return titles;
        }

        public List<Title> FindByFolder(string folder)
        {
            List<Title> titles = (from title in _list
                                  from disk in title.Disks
                                  where disk.Path.StartsWith(folder)
                                  orderby title.SortName ascending
                                  select title).ToList<Title>();
            return titles;
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
        }

        static TitleCollection()
        {
            _source_database_to_use = SourceDatabase.OML;
            Utilities.DebugLine("[TitleCollection] set _database_filename");
            _database_filename = Path.Combine(FileSystemWalker.PublicRootDirectory, @"oml.dat");
            Utilities.DebugLine("[TitleCollection] _database_filename is " + _database_filename);
        }

        /// <summary>
        /// Default destructor
        /// </summary>
        ~TitleCollection()
        {
            Utilities.DebugLine("[TitleCollection] ~TitleCollection(): Holding " + _list.Count + " titles");
        }

        /// <summary>
        /// Saves the current list of titles to the db file
        /// </summary>
        /// <returns>True on success</returns>
        public bool saveTitleCollection()
        {
            Utilities.DebugLine("[TitleCollection] saveTitleCollection()");
            return _saveTitleCollectionForOML();
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
                Utilities.DebugLine("[TitleCollection] Error reading file: " + e.Message);
                return false;
            }

            try
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, _list.Count);

                foreach (Title title in _list)
                {
                    //REMOVE DUPLICATES HERE FROM THE PRIMARY COLLECTION!!!!
                    bformatter.Serialize(stream, title);
                }
                bformatter.Serialize(stream, _genreMap);
                stream.Close();
                return true;
            }
            catch (Exception e)
            {
                Utilities.DebugLine("[TitleCollection] Error writing file: " + e.Message);
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
            Clear();
            return _loadTitleCollectionFromOML();
        }

        public static void checkACL()
        {
            if (File.Exists(_database_filename) == false)
                return;

            var fSecurity = File.GetAccessControl(_database_filename);
            fSecurity.AddAccessRule(new FileSystemAccessRule("Network Service", FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(_database_filename, fSecurity);
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
                if (File.Exists(_database_filename) == false)
                    return false;
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
                    }
                    _genreMap = (Dictionary<string, string>)bf.Deserialize(stm);
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

        private string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            return (Convert.ToBase64String(hash));            
        }

        public static long CleanUnusedImages()
        {
            long bytesRemoved = 0;
            string[] imageFiles = Directory.GetFiles(FileSystemWalker.ImageDirectory);
            TitleCollection tc = new TitleCollection();
            tc.loadTitleCollection();

            foreach (string image in imageFiles)
            {
                int id;
                string strId = image.Substring(1, image.IndexOf('.') - 1);
                try
                {
                    Int32.TryParse(strId, out id);
                    if (tc.GetTitleById(id) == null)
                    {
                        try
                        {
                            FileInfo fInfo = new FileInfo(Path.Combine(FileSystemWalker.ImageDirectory, image));
                            long size = fInfo.Length;

                            //File.Delete(Path.Combine(FileSystemWalker.ImageDirectory, image));
                            Utilities.DebugLine("Trying to delete file: {0}", image);
                            Win32APIFunctions.DeleteFilesToRecycleBin(image);
                            bytesRemoved += size;
                        }
                        catch (Exception e)
                        {
                            Utilities.DebugLine("Unable to delete file: {0} {1}", image, e.Message);
                        }
                    }
                }
                catch (Exception e) {
                    Utilities.DebugLine("Unable to parse Title id from image name: {0}", e.Message);
                }
            }
            return bytesRemoved;
        }

        #region serialization methods
        public TitleCollection(SerializationInfo info, StreamingContext ctxt)
        {
            Utilities.DebugLine("[TitleCollection] TitleCollection (Serialized)");
            List<Title> tc = (List <Title>)info.GetValue("TitleCollection", typeof(TitleCollection));
            foreach (Title title in tc)
                Add(title);
            _genreMap = info.GetValue("GenreMap", typeof(Dictionary<string, string>)) as Dictionary<string, string>;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            Utilities.DebugLine("[TitleCollection] GetObjectData()");
            info.AddValue("TitleCollection", _list);
            info.AddValue("GenreMap", _genreMap);
        }
        #endregion
    }

    class Win32APIFunctions
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
        const int FO_DELETE = 3;
        const int FOF_ALLOWUNDO = 0x40;
        const int FOF_NOCONFIRMATION = 0x10; //Don't prompt the user.; 

        public static void DeleteFilesToRecycleBin(string filename)
        {
            SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT();
            shf.wFunc = FO_DELETE;
            shf.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;
            shf.pFrom = filename + "\0";
            int result = SHFileOperation(ref shf);

            if (result != 0)
                Utilities.DebugLine("error: {0} while moving file {1} to recycle bin", result, filename);
        }
    }
}
