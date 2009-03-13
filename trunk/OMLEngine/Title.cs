using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;
using System.Drawing;

using Dao = OMLEngine.Dao;

namespace OMLEngine
{
    [Serializable()]
    [XmlRootAttribute("OMLTitle", Namespace = "http://www.openmedialibrary.org/", IsNullable = false)]
    public class Title : IComparable, ISerializable
    {
        #region locals        
        //private static string XmlNameSpace = "http://www.openmedialibrary.org/";
        //private bool _needsTranscode = false;        

        private Disk _selectedDisk = null;
        private Dao.Title _title;
        private bool _peopleProcesed = false;
        private string _backDropImage = string.Empty;
        private int _productionYear = 0;
        private string _fanartfolder = string.Empty;

        private Dictionary<string, string> _nonActingRoles = new Dictionary<string, string>(); // name, role (ie. Vangelis, Music)        
        private Dictionary<string, string> _actingRoles = null; // actor, role                                       

        private List<string> _tags = null;
        private List<Person> _directors = null;
        private List<Person> _writers = null;
        private List<string> _producers = null;
        private List<Disk> _disks = null;
        private List<string> _genres = null;
        private List<string> _audioTracks = null;
        private List<string> _subtitles = null;
        private List<string> _trailers = null;
           
        private List<string> _extraFeatures = new List<string>();

        #endregion

        #region properties       

        #region Unknown Properties
        public int ProductionYear
        {
            get { return _productionYear; }
            set { _productionYear = value; }
        }

        public string BackDropImage
        {
            get
            {
                if (_backDropImage == string.Empty)
                    return NoCoverPath;
                return _backDropImage;
            }
            set { _backDropImage = value; }
        }

        public Disk SelectedDisk
        {
            get { return _selectedDisk; }
            set { _selectedDisk = value; }
        }

        /// <summary>
        /// Does this still make sense ?
        /// </summary>
        public List<string> ExtraFeatures
        {
            get { return _extraFeatures; }
            set { _extraFeatures = value; }
        }       

        public List<Role> NonActingRolesBinding
        {
            get
            {
                SetupPeopleCollections();
                List<Role> roles = new List<Role>();
                foreach (string person in _nonActingRoles.Keys)
                {
                    roles.Add(new Role(person, _nonActingRoles[person]));
                }
                return roles;
            }
        }

        public Dictionary<string, string> NonActingRoles
        {
            get { return _nonActingRoles; }
            set { _nonActingRoles = value; }
        }

        public List<Role> ActingRolesBinding
        {
            get
            {
                SetupPeopleCollections();
                List<Role> roles = new List<Role>();
                foreach (string person in _actingRoles.Keys)
                {
                    roles.Add(new Role(person, _actingRoles[person]));
                }
                return roles;
            }
        }              

        #endregion        

        /// <summary>
        /// Unique id from the Source of our title info (MyMovies, DVD Profiler, etc).
        /// </summary>
        public string MetadataSourceID
        {
            get { return _title.MetaDataSourceItemId ?? string.Empty; }
            set { _title.MetaDataSourceItemId = value; }
        }

        internal Dao.Title DaoTitle
        {
            get { return _title; }
        }

        public string VideoResolution
        {
            get { return _title.VideoResolution; }
            set 
            {
                if (value.Length > 20)
                    throw new FormatException("VideoResolution must be 20 characters or less.");
                _title.VideoResolution = value; 
            }
        }               

        public string VideoDetails
        {
            get { return _title.VideoDetails; }
            set 
            {
                _title.VideoDetails = value; 
            }
        }

        public string ParentalRatingReason
        {
            get { return _title.ParentalRatingReason; }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("ParentalRatingReason must be 255 characters or less.");
                _title.ParentalRatingReason = value; 
            }
        }

        public string SortName
        {
            get 
            {
                if (String.IsNullOrEmpty(_title.SortName))
                    return _title.Name;
                else
                    return _title.SortName; 
            }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("SortName must be 255 characters or less.");
                _title.SortName = value; 
            }
        }

        /// <summary>
        /// Gets or sets the name of the original name (especially for foreign movies)
        /// </summary>
        /// <value>The name of the original name.</value>
        public string OriginalName
        {
            get { return _title.OriginalName; }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("OriginalName must be 255 characters or less.");
                _title.OriginalName = value; 
            }
        }

        /// <summary>
        /// A user can add tags to movies
        /// </summary>
        /// <value>The tags.</value>
        public IList<string> Tags
        {
            get 
            {
                // lazy load the tags
                if (_tags == null)
                {
                    _tags = new List<string>(_title.Tags.Count);
                    foreach (Dao.Tag tag in _title.Tags)
                        _tags.Add(tag.Name);
                }
                return _tags.AsReadOnly();
            }
        }        

        /// <summary>
        /// Gets or sets the video standard (NTSC, PAL).
        /// </summary>
        /// <value>The video standard.</value>
        public string VideoStandard
        {
            get { return _title.VideoStandard; }
            set 
            {
                if (value.Length > 10)
                    throw new FormatException("VideoStandard must be 10 characters or less.");
                _title.VideoStandard = value; 
            }
        }

        /// <summary>
        /// Gets or sets the aspect ratio (1.33:1, Widescreen, etc)
        /// </summary>
        /// <value>The aspect ratio.</value>
        public string AspectRatio
        {
            get { return _title.AspectRatio; }
            set 
            {
                if (value.Length > 10)
                    throw new FormatException("AspectRatio must be 10 characters or less.");
                _title.AspectRatio = value; 
            }
        }

        /// <summary>
        /// Gets or sets the UPC.
        /// </summary>
        /// <value>The UPC.</value>
        public string UPC
        {
            get { return _title.UPC; }
            set 
            {
                if (value.Length > 100)
                    throw new FormatException("UPC must be 100 characters or less.");
                _title.UPC = value; 
            }
        }

        /// <summary>
        /// Gets or sets the user star rating (0 to 100) - null will make the item unrated
        /// </summary>
        /// <value>The user star rating.</value>
        public int? UserStarRating
        {
            get { return _title.UserRating ?? 0; }
            set 
            { 
                _title.UserRating = ( value == null ) 
                                            ? (byte?)null 
                                            : (value > 100) ?
                                                (byte)100 
                                                : (value < 0) 
                                                    ? (byte)0 
                                                    : (byte)value; 
            }
        }

        /// <summary>
        /// Number of times video has been watched
        /// </summary>
        public int WatchedCount
        {
            get { return _title.WatchedCount ?? 0; }
            set { _title.WatchedCount = value; }
        }

        /// <summary>
        ///  Physical location of media
        /// </summary>
        public string FileLocation
        {
            get 
            {
                if (_title.Disks.Count == 1)
                    return (_title.Disks[0].Path);
                else
                    return (String.Empty);
            
            }
        }        

        /// <summary>
        /// disks for the title
        /// </summary>
        public IList<Disk> Disks
        {
            get
            {
                if (_disks == null)
                {
                    _disks = new List<Disk>(_title.Disks.Count);
                    foreach (Dao.Disk disk in _title.Disks)
                        _disks.Add(new Disk(disk));
                }

                return _disks.AsReadOnly();
            }
        }

        /// <summary>
        /// Video format of title (DVD, AVI, etc)
        /// </summary>
        public VideoFormat VideoFormat
        {
            get
            {
                if (_title.Disks.Count > 0)
                    return (VideoFormat) _title.Disks[0].VideoFormat;
                else
                    return VideoFormat.UNKNOWN;
            }
        }

        /// <summary>
        /// Display name of movie
        /// </summary>
        public string Name
        {
            get { return _title.Name; }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("Name must be 255 characters or less.");
                _title.Name = value; 
            }
        }

        public string PathSafeName
        {
            get
            {
                string chars = string.Empty;
                foreach (Char ch in Path.GetInvalidFileNameChars())
                {
                    int ach = (int)ch;
                    chars += String.Format(@"\x{0}|", ach.ToString(@"x").PadLeft(2, '0'));
                }
                if (chars.Length > 0)
                    chars = chars.Remove(chars.Length - 1);
                string rslt = System.Text.RegularExpressions.Regex.Replace(Name, chars, "");
                return rslt;
            }
        }

        /// <summary>
        /// Internal id of the Title
        /// </summary>
        public int Id
        {
            get { return _title.Id; }
            private set { _title.Id = value; }
        }

        /// <summary>
        /// The id that this title is a group member of
        /// </summary>
        public int GroupId
        {
            get { return _title.GroupId ?? _title.Id; }
            set { _title.GroupId = value; }
        }
        
        /// <summary>
        /// Name of the source for our info (MyMovies, DVD Profiler, etc)
        /// </summary>
        public string MetadataSourceName
        {
            get { return _title.MetaDataSource; }
            set 
            {
                if (value.Length > 200)
                    throw new FormatException("MetaDataSourceName must be 200 characters or less.");
                _title.MetaDataSource = value; 
            }
        }

        public string NoCoverPath
        {
            get { return OMLEngine.FileSystemWalker.ImageDirectory + "\\nocover.jpg"; }
        }
        /// <summary>
        /// Name of the source from which meta-data was gathered (MyMovies, DVD Profiler, etc.)
        /// </summary>
        public string ImporterSource
        {
            get { return _title.ImporterSource; }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("ImporterSource must be 255 characters or less.");
                _title.ImporterSource = value; 
            }
        }

        /// <summary>
        /// Pull path to the cover art image, 
        /// default the the front cover menu art image to this as well
        /// </summary>
        public string FrontCoverPath
        {
            get
            {
                if (string.IsNullOrEmpty(_title.FrontCoverPath))
                    return NoCoverPath;

                return _title.FrontCoverPath;
            }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("FrontCoverPath must be 255 characters or less.");
                _title.FrontCoverPath = value; 
            }
        }

        public string FrontCoverMenuPath
        {
            get { return _title.FrontCoverMenuPath; }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("FrontCoverMenuPath must be 255 characters or less.");
                _title.FrontCoverMenuPath = value; 
            }
        }

        /// <summary>
        /// Full path to the rear cover art image
        /// </summary>
        public string BackCoverPath
        {
            get 
            {
                if (string.IsNullOrEmpty(_title.BackCoverPath))
                    return NoCoverPath;

                return _title.BackCoverPath; 
            }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("BackCoverPath must be 255 characters or less.");
                _title.BackCoverPath = value; 
            }
        }

        /// <summary>
        /// Runtime in minutes of the title
        /// </summary>
        public int Runtime
        {
            get { return _title.Runtime ?? 0; }
            set { _title.Runtime = (short) value; }
        }

        /// <summary>
        /// Rating of the film 
        /// </summary>
        public string ParentalRating
        {
            get { return _title.ParentalRating ?? string.Empty; }
            set 
            {
                if (value.Length > 20)
                    throw new FormatException("ParentalRating must be 20 characters or less.");
                _title.ParentalRating = value; 
            }
        }

        /// <summary>
        /// Long description of title
        /// </summary>
        public string Synopsis
        {
            get { return _title.Synopsis ?? string.Empty; }
            set { _title.Synopsis = value; }
        }

        /// <summary>
        /// Name of studio company
        /// </summary>
        public string Studio
        {
            get { return _title.Studio ?? string.Empty; }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("Studio must be 255 characters or less.");
                _title.Studio = value; 
            }
        }
        /// <summary>
        /// Country where the title was created/first released
        /// </summary>
        public string CountryOfOrigin
        {
            get { return _title.CountryOfOrigin ?? string.Empty; }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("CountryOfOrigin must be 255 characters or less.");
                _title.CountryOfOrigin = value; 
            }
        }
        /// <summary>
        /// website for title (if it has one)
        /// </summary>
        public string OfficialWebsiteURL
        {
            get { return _title.WebsiteUrl ?? string.Empty; }
            set 
            {
                if (value.Length > 255)
                    throw new FormatException("OfficialWebsiteURL must be 255 characters or less.");
                _title.WebsiteUrl = value; 
            }
        }

        /// <summary>
        /// Original date of release (or re-release)
        /// </summary>
        public DateTime ReleaseDate
        {
            get { return _title.ReleaseDate ?? DateTime.MinValue; }
            set { _title.ReleaseDate = value; }
        }

        /// <summary>
        /// Date that this title was added to the database
        /// </summary>
        public DateTime DateAdded
        {
            get { return _title.DateAdded ?? DateTime.MinValue; }
            set { _title.DateAdded = value; }
        }        

        /// <summary>
        /// List of languages (English, Spanish, French, DTS, DD5.1, DD2.0, etc)
        /// </summary>
        public IList<string> AudioTracks
        {
            get 
            {
                // lazy load the audio tracks
                if (_audioTracks == null)
                    _audioTracks = Dao.TitleDao.DelimitedDBStringToCollection(_title.AudioTracks);

                return _audioTracks.AsReadOnly(); 
            }
        }

        public IList<string> Subtitles
        {
            get
            {
                // lazy load the subtitles
                if (_subtitles == null)
                    _subtitles = Dao.TitleDao.DelimitedDBStringToCollection(_title.Subtitles);

                return _subtitles.AsReadOnly();
            }
        }

        public IList<string> Trailers
        {
            get
            {
                // lazy load the trailers
                if (_trailers == null)
                    _trailers = Dao.TitleDao.DelimitedDBStringToCollection(_title.Trailers);

                return _trailers.AsReadOnly();
            }
        }

        /// <summary>
        /// List of Genres
        /// </summary>
        public IList<string> Genres
        {
            get 
            {
                // lazy load the genres
                if (_genres == null)
                {
                    _genres = new List<string>(_title.Genres.Count);
                    foreach (Dao.Genre genre in _title.Genres)
                        _genres.Add(genre.MetaData.Name);
                }

                return _genres.AsReadOnly(); 
            }
        }

        public Dictionary<string, string> ActingRoles
        {
            get 
            {
                if (!_peopleProcesed)
                    SetupPeopleCollections();

                return _actingRoles; 
            }            
        }        

        public void AddNonActingRole(string name, string role)
        {
            if (name == null || role == null) return;
            if (_nonActingRoles != null && !_nonActingRoles.ContainsKey(name))
            {

            }
            if (!_nonActingRoles.ContainsKey(name))
            {
                _nonActingRoles.Add(name, role);
            }
        }       
        
        /// <summary>
        /// List of Person objects that directed the title (usually one Person)
        /// </summary>
        public List<Person> Directors
        {
            get 
            {
                if (!_peopleProcesed)
                    SetupPeopleCollections();

                return _directors; 
            }
            set { _directors = value; }
        }
        /// <summary>
        /// List of Person objects that wrote the title
        /// </summary>
        public List<Person> Writers
        {
            get 
            {
                if (!_peopleProcesed)
                    SetupPeopleCollections();

                return _writers; 
            }
            set { _writers = value; }
        }

        /// <summary>
        /// List of people/companies that produced the title
        /// </summary>
        public List<string> Producers
        {
            get 
            {
                if (!_peopleProcesed)
                    SetupPeopleCollections();

                return _producers; 
            }
            set { _producers = value; }
            _backDropImage = GetSerializedString(info, "backdrop_boxart_path");
            _productionYear = GetSerializedInt(info, "production_year");
            _fanartfolder = GetSerializedString(info, "fan_art_folder");
        }

        public decimal PercentComplete
        {
            get { return _title.PercentComplete; }
        }        

        #endregion

        private void SetupPeopleCollections()
        {
            _actingRoles = new Dictionary<string, string>();
            _writers = new List<Person>();
            _producers = new List<string>();
            _directors = new List<Person>();

            foreach (Dao.Person person in _title.People)
            {
                switch ((PeopleRole)person.Role)
                {
                    case PeopleRole.Actor:                        
                        // todo : solomon : we need to move this off a dictionary 
                        // duplicates should be allowed
                        if (! _actingRoles.ContainsKey(person.MetaData.FullName) )
                            _actingRoles.Add(person.MetaData.FullName, person.CharacterName);
                        break;

                    case PeopleRole.Director:
                        _directors.Add(new Person(person.MetaData.FullName));
                        break;

                    case PeopleRole.Producers:
                        _producers.Add(person.MetaData.FullName);
                        break;

                    case PeopleRole.Writer:
                        _writers.Add(new Person(person.MetaData.FullName));
                        break;
                }
            }

            _peopleProcesed = true;
        }

        /// <summary>
        /// Generic Constructor - used to add a new title to the db
        /// </summary>
        public Title() 
        {
            _title = new OMLEngine.Dao.Title();
            info.AddValue("backdrop_boxart_path", _backDropImage);
            info.AddValue("production_year", _productionYear);
            info.AddValue("fan_art_folder", _fanartfolder);
        }

        /// <summary>
        /// Constructor to base a title object off a db title object
        /// </summary>
        /// <param name="title"></param>
        internal Title(OMLEngine.Dao.Title title)
        {
            _title = title;
        }

        public void AddActingRole(string actor, string role)
        {
            if (actor.Length > 255)
                throw new FormatException("Actor must be 255 characters or less.");
            if (role.Length > 255)
                throw new FormatException("Role must be 255 characters or less.");

            if (string.IsNullOrEmpty(actor) && string.IsNullOrEmpty(role))
                return;

            if (ActingRoles.ContainsKey(actor))
                return;

            TitleCollectionManager.AddActorToTitle(this, actor, role);
            
            // reset the internal collection
            _peopleProcesed = false;
        }

        /// <summary>
        /// Add a Person object to the directors list
        /// </summary>
        /// <param name="director">Person object to add</param>
        public void AddDirector(Person director)
        {
            /*if ( Directors.Contains(
            /*
            if (director == null) return;
            _directors.Add(director);
             */
        }
        /// <summary>
        /// Add a Person object to the writers list
        /// </summary>
        /// <param name="writer">Person object to add</param>
        public void AddWriter(Person writer)
        {
            /*
            if (writer == null) return;
            _writers.Add(writer);
             */
        }
        /// <summary>
        /// Add a string (person name or company name) to the producers list
        /// </summary>
        /// <param name="producer">string name to add</param>
        public void AddProducer(string producer)
        {
            /*
            if (producer == null) return;
            _producers.Add(producer);
             */
        }                       

        /// <summary>
        /// Add an audio track
        /// </summary>
        /// <param name="audioTrack"></param>
        public void AddAudioTrack(string audioTrack)
        {
            if (string.IsNullOrEmpty(audioTrack))
                return;

            if (_audioTracks == null)
                _audioTracks = Dao.TitleDao.DelimitedDBStringToCollection(_title.AudioTracks);

            _audioTracks.Add(audioTrack);

            string tracks = Dao.TitleDao.GetDelimitedStringFromCollection(_audioTracks);
            if (tracks.Length > 255)
            {
                _audioTracks = null;
                throw new FormatException("Too many audio tracks have been added.");
            }
            _title.AudioTracks = tracks;
            _audioTracks = null;
        }

        /// <summary>
        /// Adds a subtitle
        /// </summary>
        /// <param name="subtitle"></param>
        public void AddSubtitle(string subtitle)
        {
            if (string.IsNullOrEmpty(subtitle))
                return;

            if (_subtitles == null)
                _subtitles = Dao.TitleDao.DelimitedDBStringToCollection(_title.Subtitles);

            _subtitles.Add(subtitle);
            string subtitles = Dao.TitleDao.GetDelimitedStringFromCollection(_subtitles);
            if (subtitles.Length > 255)
            {
                _subtitles = null;
                throw new FormatException("Too many audio tracks have been added.");
            }
            _title.Subtitles = subtitle;

            _subtitles = null;
        }

        /// <summary>
        /// Adds a trailer to the title
        /// </summary>
        /// <param name="trailer"></param>
        public void AddTrailer(string trailer)
        {
            if (string.IsNullOrEmpty(trailer))
                return;

            if (_trailers == null)
                _trailers = Dao.TitleDao.DelimitedDBStringToCollection(_title.Trailers);

            _trailers.Add(trailer);

            string trailers = Dao.TitleDao.GetDelimitedStringFromCollection(_trailers);
            if (trailers.Length > 255)
            {
                _trailers = null;
                throw new FormatException("Too many audio tracks have been added.");
            }
            _title.Trailers = trailers;

            _trailers = null;
        }

        /// <summary>
        /// Add a Genre to the genres list
        /// </summary>
        /// <param name="genre">A Genre from the Genre enum</param>
        public void AddGenre(string genre)
        {
            if (genre.Length > 255)
                throw new FormatException("Genre must be 255 characters or less.");
            if (string.IsNullOrEmpty(genre))
                return;

            if (Genres.Contains(genre))
                return;

            TitleCollectionManager.AddGenreToTitle(this, genre);

            _genres = null;
        }

        /// <summary>
        /// Removes a genre from the title
        /// </summary>
        /// <param name="genre"></param>
        public void RemoveGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
                return;

            if (!Tags.Contains(genre))
                return;

            Dao.Genre foundGenre = null;

            foreach (Dao.Genre daoGenre in _title.Genres)
            {
                if (daoGenre.MetaData.Name == genre)
                {
                    foundGenre = daoGenre;
                    break;
                }
            }

            if (foundGenre != null)
                _title.Genres.Remove(foundGenre);

            _genres = null;
        }

        /// <summary>
        /// Adds a disk to the collection
        /// </summary>
        /// <param name="disk"></param>
        public void AddDisk(Disk disk)
        {
            if (disk == null)
                return;

            if (Disks.Contains(disk))
                return;

            _title.Disks.Add(disk.DaoDisk);

            _disks = null;
        }

        /// <summary>
        /// Add a disk that won't be persisted to the database
        /// </summary>
        /// <param name="disk"></param>
        public void AddTempDisk(Disk disk)
        {
            if (disk == null)
                return;

            if (Disks.Contains(disk))
                return;

            if (_disks == null)
                _disks = new List<Disk>(1);

            _disks.Add(disk);
        }

        /// <summary>
        /// Sets a tag to be added
        /// </summary>
        /// <param name="tag"></param>
        public void AddTag(string tag)
        {
            if (tag.Length > 255)
                throw new FormatException("Tag must be 255 characters or less.");

            if (string.IsNullOrEmpty(tag))
                return;

            if (Tags.Contains(tag))
                return;

            TitleCollectionManager.AddTagToTitle(this, tag);

            _tags = null;
        }

        /// <summary>
        /// Removes the tag from the collection
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return;

            if (!Tags.Contains(tag))
                return;

            Dao.Tag foundTag = null;

            foreach (Dao.Tag daoTag in _title.Tags)
            {
                if (daoTag.Name == tag)
                {
                    foundTag = daoTag;
                    break;
                }
            }

            if (foundTag != null)
                _title.Tags.Remove(foundTag);

            _tags = null;
        }

        public override bool Equals(object obj)
        {
            Title otherT = obj as Title;

            if (otherT == null)
                return false;
            if (otherT._title == null)
                return false;
            if (this.Id == otherT.Id)
                return true;
            if (this.Name == otherT.Name)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public int CompareTo(object other)
        {
            Title otherT = other as Title;
            if (otherT == null)
                return -1;
            return Name.CompareTo(otherT.Name);
        }

        static public Title CreateFromXML(string fileName, bool copyImages)
        {
            return new Title();
            /*Title t = new Title();

            try
            {
                if (File.Exists(fileName))
                {
                    XPathDocument document = new XPathDocument(fileName);
                    XPathNavigator navigator = document.CreateNavigator();

                    navigator.MoveToParent();

                    if (navigator.MoveToChild("OMLTitle", XmlNameSpace))
                    {
                        if (navigator.MoveToChild("Name", XmlNameSpace))
                        {
                            t.Name = navigator.Value;
                            navigator.MoveToParent();
                        }
                        else
                        {
                            return null;
                        }
                        
                        if (navigator.MoveToChild("OriginalName", XmlNameSpace))
                        {
                            if (String.IsNullOrEmpty(navigator.Value))
                                t.OriginalName = t.Name;
                            else
                                t.OriginalName = navigator.Value;
                            navigator.MoveToParent();
                        }
                        
                        if (navigator.MoveToChild("SortName", XmlNameSpace))
                        {
                            if (String.IsNullOrEmpty(navigator.Value))
                                t.SortName = t.Name;
                            else
                                t.SortName = navigator.Value;
                            navigator.MoveToParent();
                        }
                        
                        if (navigator.MoveToChild("FrontCoverPath", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value)) t.FrontCoverPath = navigator.Value;
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("FrontCoverMenuPath", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value)) t.FrontCoverMenuPath = navigator.Value;
                            navigator.MoveToParent();
                        }

                        if (navigator.MoveToChild("BackCoverPath", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value)) t.BackCoverPath = navigator.Value;
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("Disks", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Disk")
                                    {
                                        string diskName = "Movie";
                                        if (navigator.MoveToFirstAttribute() && navigator.Name == "Name")
                                        {
                                            diskName = navigator.Value;
                                            navigator.MoveToParent();
                                        }

                                        string path = "";
                                        VideoFormat format = VideoFormat.DVD;

                                        if (navigator.MoveToChild("Path", XmlNameSpace))
                                        {
                                            path = navigator.Value;
                                        }
                                        navigator.MoveToParent();
                                        if (navigator.MoveToChild("Format", XmlNameSpace))
                                        {
                                            try
                                            {
                                                format = (VideoFormat)Enum.Parse(typeof(VideoFormat), navigator.Value, true);
                                            }
                                            catch
                                            {
                                                format = VideoFormat.DVD;
                                            }

                                        }
                                        navigator.MoveToParent();

                                        t.Disks.Add(new Disk(diskName, path, format));

                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                navigator.MoveToParent(); // move up to disks
                            }
                            
                            navigator.MoveToParent(); // move up to root
                        }

                        if (navigator.MoveToChild("Synopsis", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.Synopsis = navigator.Value;
                            }
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("Studio", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.Studio = navigator.Value;
                            }
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("Country", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.CountryOfOrigin = navigator.Value;
                            }
                            navigator.MoveToParent();
                        }


                        
                        if (navigator.MoveToChild("OfficialWebSiteURL", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.OfficialWebsiteURL = navigator.Value;
                            }
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("Runtime", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                int runtime = 0;
                                if (Int32.TryParse(navigator.Value, out runtime))
                                {
                                    t.Runtime = runtime;
                                }
                            }
                            navigator.MoveToParent();
                        }
                        
                        if (navigator.MoveToChild("ParentalRating", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.ParentalRating = navigator.Value;
                            }
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("ParentalRatingReason", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.ParentalRatingReason = navigator.Value;
                            }
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("ReleaseDate", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                DateTime releaseDate;
                                if (DateTime.TryParse(navigator.Value, out releaseDate))
                                {
                                    t.ReleaseDate = releaseDate;
                                }
                            }
                            navigator.MoveToParent();
                        }

                        if (navigator.MoveToChild("ProductionYear", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                int productionYear;
                                if (int.TryParse(navigator.Value, out productionYear))
                                {
                                    t.ProductionYear = productionYear;
                                }
                            }
                            navigator.MoveToParent();
                        }
                        
                        if (navigator.MoveToChild("Genres", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Genre")
                                    {
                                        t.AddGenre(navigator.Value);
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("Tags", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Tag")
                                    {
                                        t.Tags.Add(navigator.Value);
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else
                                    {
                                        break;
                                    }

                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("Trailers", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Trailer")
                                    {
                                        t.Trailers.Add(navigator.Value);
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                navigator.MoveToParent();
                            }
                            
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("Photos", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Photo")
                                    {
                                        t.Photos.Add(navigator.Value);
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();
                            
                        }


                        if (navigator.MoveToChild("Subtitles", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Subtitle")
                                    {
                                        t.Subtitles.Add(navigator.Value);
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else break;
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();

                        }


                        if (navigator.MoveToChild("AudioTracks", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "AudioTrack")
                                    {
                                        t.AudioTracks.Add(navigator.Value);
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else break;
                                }
                                navigator.MoveToParent();

                            }
                            navigator.MoveToParent();

                        }

                        if (navigator.MoveToChild("ExtraFeatures", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "ExtraFeature")
                                    {
                                        t.ExtraFeatures.Add(navigator.Value);
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else break;
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();
                        }

                        if (navigator.MoveToChild("Producers", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Producer")
                                    {
                                        t.AddProducer(navigator.Value);
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else break;
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();

                        }


                        if (navigator.MoveToChild("Writers", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Writer")
                                    {
                                        t.AddWriter(new Person(navigator.Value));
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else break;
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();
                        }

                        if (navigator.MoveToChild("Directors", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Director")
                                    {
                                        t.AddDirector(new Person(navigator.Value));
                                        if (!navigator.MoveToNext()) break;
                                    }
                                    else break;
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("AspectRatio", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.AspectRatio = navigator.Value;
                            }
                            navigator.MoveToParent();
                        }

                        
                        if (navigator.MoveToChild("VideoStandard", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.VideoStandard = navigator.Value;
                            }
                            navigator.MoveToParent();
                        }


                        if (navigator.MoveToChild("VideoResolution", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.VideoResolution = navigator.Value;
                            }
                            navigator.MoveToParent();
                        }


                        if (navigator.MoveToChild("VideoDetails", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                t.VideoDetails = navigator.Value;
                            }
                            navigator.MoveToParent();

                        }


                        if (navigator.MoveToChild("UserRating", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value))
                            {
                                int rating = 0;
                                if (Int32.TryParse(navigator.Value, out rating))
                                {
                                    t.UserStarRating = rating;
                                }
                            }
                            navigator.MoveToParent();
                        }


                        if (navigator.MoveToChild("ActingRoles", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Actor")
                                    {
                                        string name = "";
                                        string role = "";

                                        if (navigator.MoveToFirstAttribute())
                                        {
                                            for (; ; )
                                            {
                                                if (navigator.Name == "Name")
                                                    name = navigator.Value;
                                                else if (navigator.Name == "Role")
                                                    role = navigator.Value;
                                                if (!navigator.MoveToNextAttribute()) break;
                                            }
                                            navigator.MoveToParent();
                                        }
                                        //navigator.MoveToParent();

                                        if( name.Length > 0 )
                                            t.AddActingRole(name, role);

                                        if (!navigator.MoveToNext())
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();

                        }


                        if (navigator.MoveToChild("NonActingRoles", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "Person")
                                    {
                                        string name = "";
                                        string role = "";

                                        if (navigator.MoveToFirstAttribute())
                                        {
                                            for (; ; )
                                            {
                                                if (navigator.Name == "Name")
                                                    name = navigator.Value;
                                                else if (navigator.Name == "Role")
                                                    role = navigator.Value;
                                                if (!navigator.MoveToNextAttribute()) break;
                                            }
                                            navigator.MoveToParent();
                                        }
                                        //navigator.MoveToParent();

                                        if (name.Length > 0)
                                            t.AddNonActingRole(name, role);

                                        if (!navigator.MoveToNext())
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();

                        }

                        if (navigator.MoveToChild("CustomFields", XmlNameSpace))
                        {
                            if (navigator.MoveToFirstChild())
                            {
                                for (; ; )
                                {
                                    if (navigator.Name == "CustomField")
                                    {
                                        string name = "";
                                        string value = "";

                                        if (navigator.MoveToFirstAttribute())
                                        {
                                            for (; ; )
                                            {
                                                if (navigator.Name == "Name")
                                                    name = navigator.Value;
                                                else if (navigator.Name == "Value")
                                                    value = navigator.Value;
                                                if (!navigator.MoveToNextAttribute()) break;
                                            }
                                            navigator.MoveToParent();
                                        }

                                        if (name.Length > 0)
                                            t.AdditionalFields.Add(name, value);

                                        if (!navigator.MoveToNext())
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                navigator.MoveToParent();
                            }
                            navigator.MoveToParent();

                        }

                    }
                }
            }
            catch
            {
                return null;
            }

            // oml.xml should be int the same folder as the media
            // for an avi file c:\movies\test.avi the oml file is c:\movies\test.avi.oml.xml
            // for a dvd c:\movies\300\video_ts is c:\movies\300\video_ts.oml.xml

            // if we import to a different computer try to look up the information in
            // the current folder where the oml.xml is found

            foreach (Disk d in t.Disks)
            {
                d.Path = FixupDiskPath(d.Path.ToUpper(), fileName.ToUpper());
            }

            // tries to guess the right file names if file not found. Try several alternatives
            t.FrontCoverPath = FixupImagePath(t.FrontCoverPath.ToUpper(), fileName.ToUpper(), ".JPG");
            t.BackCoverPath = FixupImagePath(t.BackCoverPath.ToUpper(), fileName.ToUpper(), ".BACK.JPG");

            if (copyImages)
            {
                t.CopyFrontCoverFromFile(t.FrontCoverPath, false);
                t.CopyBackCoverFromFile(t.BackCoverPath, false);
            }

            return t;*/
        }

//<?xml version="1.0" encoding="utf-8"?>
//<OMLTitle xmlns="http://www.openmedialibrary.org/">
        
        public bool SerializeToXMLFile(string fileName)
        {
            return true;
            /*fileName = fileName.ToUpper();
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Document;

                XmlWriter writer = XmlWriter.Create(fileName, settings);

                writer.WriteStartDocument();

                writer.WriteStartElement("OMLTitle", XmlNameSpace);

                writer.WriteElementString("Name", Name);
                writer.WriteElementString("SortName", _sortName);
                writer.WriteElementString("OriginalName", _originalName);

                string newFrontCoverPath = fileName.Replace(".OML.XML", ".JPG");
                string newBackCoverPath = fileName.Replace(".OML.XML", ".BACK.JPG");

                if (newFrontCoverPath.EndsWith(@"\VIDEO_TS.JPG"))
                {
                    newFrontCoverPath = newFrontCoverPath.Replace(@"VIDEO_TS.JPG", "FOLDER.JPG");
                    newBackCoverPath = newBackCoverPath.Replace("@VIDEO_TS.BACK.JPG", "FOLDER.BACK.JPG");
                }

                if (!SaveFrontCoverToFile(newFrontCoverPath))
                {
                    newFrontCoverPath = FrontCoverPath;
                }

                if (!SaveBackCoverToFile(newBackCoverPath))
                {
                    newBackCoverPath = BackCoverPath;
                }

                // the reader should verify that this exists and if doesn't it should
                // just use folder.jpg in the same folder
                writer.WriteElementString("FrontCoverPath", newFrontCoverPath);

                writer.WriteElementString("BackCoverPath", newBackCoverPath);

                // this will be created when imported
                //writer.WriteElementString("FrontCoverMenuPath", FrontCoverMenuPath);
                writer.WriteElementString("FrontCoverMenuPath", "");


                // should be relative path so it can be loaded on any computer
                writer.WriteStartElement("Disks");
                foreach (Disk disk in Disks)
                {
                    writer.WriteStartElement("Disk");
                    writer.WriteAttributeString("Name", disk.Name);

                    // the xml file should be in the same dir as the movie
                    writer.WriteElementString("Path", disk.Path);
                    writer.WriteElementString("Format", disk.Format.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                ////writer.WriteElementString("VideoFormat", _videoFormat.ToString());
                writer.WriteElementString("TranscodeToExtender", _needsTranscode.ToString());

                writer.WriteElementString("Synopsis", _synopsis);
                writer.WriteElementString("Studio", _studio);
                writer.WriteElementString("Country", _countryOfOrigin);
                writer.WriteElementString("OfficialWebSiteURL", _officialWebsiteURL);

                writer.WriteElementString("Runtime", _runtime.ToString());
                writer.WriteElementString("ParentalRating", _parentalRating);
                writer.WriteElementString("ParentalRatingReason", _parentalRatingReason);
                writer.WriteElementString("ReleaseDate", _releaseDate.ToString());
                writer.WriteElementString("ProductionYear", _productionYear.ToString());



                writer.WriteStartElement("Genres");
                foreach (string genre in Genres)
                {
                    writer.WriteElementString("Genre", genre);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Writers");
                foreach (Person w in Writers)
                {
                    writer.WriteElementString("Writer", w.full_name);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Directors");
                foreach (Person director in Directors)
                {
                    writer.WriteElementString("Director", director.full_name);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("AudioTracks");
                foreach (string languageFormat in AudioTracks)
                {
                    writer.WriteElementString("AudioTrack", languageFormat);
                }
                writer.WriteEndElement();


                writer.WriteElementString("UserRating", _userStarRating.ToString());
                writer.WriteElementString("AspectRatio", _aspectRatio.ToString());
                writer.WriteElementString("VideoStandard", _videoStandard);


                writer.WriteStartElement("ActingRoles");
                foreach (KeyValuePair<string, string> actingRole in ActingRoles)
                {
                    writer.WriteStartElement("Actor");
                    writer.WriteAttributeString("Name", actingRole.Key);
                    writer.WriteAttributeString("Role", actingRole.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("NonActingRoles");
                foreach (KeyValuePair<string, string> actingRole in NonActingRoles)
                {
                    writer.WriteStartElement("Person");
                    writer.WriteAttributeString("Name", actingRole.Key);
                    writer.WriteAttributeString("Role", actingRole.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();


                writer.WriteStartElement("Tags");
                foreach (string tag in Tags)
                {
                    writer.WriteElementString("Tag", tag);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Subtitles");
                foreach (string sub in Subtitles)
                {
                    writer.WriteElementString("Subtitle", sub);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Producers");
                foreach (string sub in Subtitles)
                {
                    writer.WriteElementString("Producer", sub);
                }
                writer.WriteEndElement();


                writer.WriteStartElement("CustomFields");
                foreach (KeyValuePair<string, string> field in AdditionalFields)
                {
                    writer.WriteStartElement("CustomField");
                    writer.WriteAttributeString("Name", field.Key);
                    writer.WriteAttributeString("Value", field.Key);
                }
                writer.WriteEndElement();


                writer.WriteStartElement("Photos");
                foreach (string photo in Photos)
                {
                    writer.WriteElementString("Photo", photo);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("Trailers");
                foreach (string trailer in Trailers)
                {
                    writer.WriteElementString("Trailer", trailer);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("ExtraFeatures");
                foreach (string feature in ExtraFeatures)
                {
                    writer.WriteElementString("ExtraFeature", feature);
                }
                writer.WriteEndElement();


                writer.WriteElementString("VideoDetails", _videoDetails);

                writer.WriteElementString("VideoResolution", _videoResolution);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                return true;
            }
            catch
            {
                return false;
            }*/
        }

        private string CopyStringValue(string src, string dest, bool overWrite)
        {
            if (overWrite)
            {
                // only copy data if available
                if (!String.IsNullOrEmpty(src))
                    return src;
                else
                    return dest;
            }
            else
            {
                if (!String.IsNullOrEmpty(src) && String.IsNullOrEmpty(dest))
                    return src;
                else
                    return dest;
            }
        }

        public void CopyMetadata(Title t, bool overWrite)
        {
            Name = CopyStringValue(t.Name, Name, overWrite);
            MetadataSourceID = CopyStringValue(t.MetadataSourceID, MetadataSourceID, overWrite);

            ParentalRating = CopyStringValue(t.ParentalRating, ParentalRating, overWrite);
            Synopsis = CopyStringValue(t.Synopsis, Synopsis, overWrite);
            Studio = CopyStringValue(t.Studio, Studio, overWrite);
            CountryOfOrigin = CopyStringValue(t.CountryOfOrigin, CountryOfOrigin, overWrite);
            OfficialWebsiteURL = CopyStringValue(t.OfficialWebsiteURL, OfficialWebsiteURL, overWrite);
            AspectRatio = CopyStringValue(t.AspectRatio, AspectRatio, overWrite);
            VideoStandard = CopyStringValue(t.VideoStandard, VideoStandard, overWrite);
            UPC = CopyStringValue(t.UPC, UPC, overWrite);
            OriginalName = CopyStringValue(t.OriginalName, OriginalName, overWrite);
            SortName = CopyStringValue(t.SortName, SortName, overWrite);
            ParentalRatingReason = CopyStringValue(t.ParentalRatingReason, ParentalRatingReason, overWrite);
            VideoDetails = CopyStringValue(t.VideoDetails, VideoDetails, overWrite);
            
            if ( t.Runtime > 0) Runtime = t.Runtime;
            if (t.ReleaseDate != null) ReleaseDate = t.ReleaseDate;
            if (t.UserStarRating > 0) UserStarRating = t.UserStarRating;
            if (t.ProductionYear > 0) ProductionYear = t.ProductionYear;

            if (t._directors != null && t._directors.Count > 0)
            {
                if (_directors == null) _directors = new List<Person>();
                if (overWrite || _directors.Count == 0)
                {
                    _directors.Clear();
                    foreach (Person p in t._directors)
                    {
                        AddDirector(new Person(p.full_name));
                    }
                }
            }


            if (t._writers != null && t._writers.Count > 0)
            {
                if (_writers == null) _writers = new List<Person>();
                if (overWrite || _writers.Count == 0)
                {
                    _writers.Clear();
                    foreach (Person p in t._writers)
                    {
                        AddWriter(new Person(p.full_name));
                    }
                }

            }

            if (t._producers != null && t._producers.Count > 0)
            {
                if (_producers == null) _producers = new List<string>();
                if (overWrite || _producers.Count == 0)
                {
                    _producers.Clear();
                    foreach (string p in t._producers)
                    {
                        AddProducer(p);
                    }
                }
            }

            if (t._audioTracks != null && t._audioTracks.Count > 0)
            {
                if (_audioTracks == null) _audioTracks = new List<string>();
                if (overWrite || _audioTracks.Count == 0)
                {
                    _audioTracks.Clear();
                    foreach (string p in t._audioTracks)
                    {
                        _audioTracks.Add(p);
                    }
                }
            }

            if (t._genres != null && t._genres.Count > 0)
            {
                if (_genres == null) _genres = new List<string>();
                if (overWrite || _genres.Count == 0)
                {

                    _genres.Clear();
                    foreach (string p in t._genres)
                    {
                        _genres.Add(p);
                    }
                }
            }

            if (t._tags != null && t._tags.Count > 0)
            {
                if (_tags == null) _tags = new List<string>();
                if (overWrite || _tags.Count == 0)
                {

                    _tags.Clear();
                    foreach (string p in t._tags)
                    {
                        _tags.Add(p);
                    }
                }
            }

            if (t._actingRoles != null && t._actingRoles.Count > 0)
            {
                if (_actingRoles == null) _actingRoles = new Dictionary<string, string>();
                if (overWrite || _actingRoles.Count == 0)
                {
                    _actingRoles.Clear();
                    foreach (KeyValuePair<string, string> p in t._actingRoles)
                    {
                        _actingRoles.Add(p.Key, p.Value);
                    }
                }
            }

            if (t._nonActingRoles != null && t._nonActingRoles.Count > 0)
            {
                if (_nonActingRoles == null) _nonActingRoles = new Dictionary<string, string>();
                if (overWrite || _nonActingRoles.Count == 0)
                {

                    _nonActingRoles.Clear();
                    foreach (KeyValuePair<string, string> p in t._nonActingRoles)
                    {
                        _nonActingRoles.Add(p.Key, p.Value);
                    }
                }
            }            

            if (t._trailers != null && t._trailers.Count > 0)
            {
                if (_trailers == null) _trailers = new List<string>();
                if (overWrite || _trailers.Count == 0)
                {

                    _trailers.Clear();
                    foreach (string p in t._trailers)
                    {
                        _trailers.Add(p);
                    }
                }
            }

            if (t._subtitles != null && t._subtitles.Count > 0)
            {
                if (_subtitles == null) _subtitles = new List<string>();
                if (overWrite || _subtitles.Count == 0)
                {

                    _subtitles.Clear();
                    foreach (string p in t._subtitles)
                    {
                        _subtitles.Add(p);
                    }
                }
            }

            if (t._extraFeatures != null && t._extraFeatures.Count > 0)
            {
                if (_extraFeatures == null) _extraFeatures = new List<string>();
                if (overWrite || _extraFeatures.Count == 0)
                {

                    _extraFeatures.Clear();
                    foreach (string p in t._extraFeatures)
                    {
                        _extraFeatures.Add(p);
                    }
                }
            }

            if (!String.IsNullOrEmpty(t.FrontCoverPath))
            {
                if (overWrite || String.IsNullOrEmpty(FrontCoverPath) || FrontCoverPath == GetDefaultNoCoverName())
                {
                    CopyFrontCoverFromFile(t.FrontCoverPath, true);
                }
            }

            if (!String.IsNullOrEmpty(t.BackCoverPath))
            {
                if (overWrite || String.IsNullOrEmpty(BackCoverPath) || BackCoverPath == GetDefaultNoCoverName())
                {
                    CopyBackCoverFromFile(t.BackCoverPath, true);
                }
            }

        }

        // copy front cover image and set the menu cover too (resized version)
        public bool CopyFrontCoverFromFile(string source, bool deleteSource)
        {
            if (!string.IsNullOrEmpty(source))
            {
                try
                {
                    File.Copy(source, GetDefaultFrontCoverName(), true);
                    if (deleteSource) File.Delete(source);
                    FrontCoverPath = GetDefaultFrontCoverName();
                    BuildResizedMenuImage();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public bool CopyBackCoverFromFile(string source, bool deleteSource)
        {
            if (!string.IsNullOrEmpty(source))
            {
                try
                {
                    File.Copy(source, GetDefaultBackCoverName(), true);
                    if (deleteSource) File.Delete(source);
                    BackCoverPath = GetDefaultBackCoverName();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public string GetDefaultNoCoverName()
        {
            return OMLEngine.FileSystemWalker.ImageDirectory + "\\nocover.jpg";
        }
        
        public string GetDefaultFrontCoverName()
        {
            return OMLEngine.FileSystemWalker.ImageDirectory + "\\F" + Id + ".jpg";
        }

        public string GetDefaultBackCoverName()
        {
            return OMLEngine.FileSystemWalker.ImageDirectory + "\\B" + Id + ".jpg";
        }

        public string GetDefaultFrontCoverMenuName()
        {
            return OMLEngine.FileSystemWalker.ImageDirectory + "\\MF" + Id + ".jpg";
        }

        public void BuildResizedMenuImage()
        {
            try
            {
                if ( !String.IsNullOrEmpty(FrontCoverPath) && File.Exists(FrontCoverPath) )
                {
                    using (Image coverArtImage = Utilities.ReadImageFromFile(FrontCoverPath))
                    {
                        if (coverArtImage != null)
                        {
                            using (Image menuCoverArtImage = Utilities.ScaleImageByHeight(coverArtImage, 200))
                            {
                                string img_path = GetDefaultFrontCoverMenuName();
                                menuCoverArtImage.Save(img_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                                FrontCoverMenuPath = img_path;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[Title.BuildResizedMenuImage] Exception: " + ex.Message);
            }
        }

        public override string ToString()
        {
            return "Title:" + this.Name + " (" + this.Id + ")";
        }

        #region serialization methods

        private DateTime GetSerializedDateTime(SerializationInfo info, string id)
        {
            try
            {
                return info.GetDateTime(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in GetSerializedDateTime: " + e.Message);
                return new DateTime(0);
            }
        }

        private VideoFormat GetSerializedVideoFormat(SerializationInfo info, string id)
        {
            try
            {
                return (VideoFormat)info.GetValue(id, typeof(VideoFormat));
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in GetSerializedVideoFormat: " + e.Message);
                return VideoFormat.DVD;
            }
        }

        private bool GetSerializedBoolean(SerializationInfo info, string id)
        {
            try
            {
                return info.GetBoolean(id);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in GetSerializedBoolean: " + e.Message);
                return false;
            }
        }

        private int GetSerializedInt(SerializationInfo info, string id)
        {
            try
            {
                return (int)info.GetValue(id, typeof(int));
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in GetSerializedInt: " + e.Message);
                return 0;
            }
        }

        private string GetSerializedString(SerializationInfo info, string id)
        {
            try
            {
                string result = info.GetString(id);
                return (result == null ? String.Empty : result.Trim());
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in GetSerializedString: " + e.Message);
                return String.Empty;
            }
        }

        private  T GetSerializedList<T>(SerializationInfo info, string id) where T : new()
        {
            try
            {
                T result = (T)info.GetValue(id, typeof(T));
                return (result == null ? new T() : result);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception in GetSerializedList: " + e.Message);
                return new T();
            }
        }

        /// <summary>
        /// Constructor used for loading from database file
        /// </summary>
        /// <param name="info">SerializationInfo object</param>
        /// <param name="ctxt">StreamingContext object</param>
        public Title(SerializationInfo info, StreamingContext ctxt)
        {
            _title = new OMLEngine.Dao.Title();
            //Utilities.DebugLine("[Title] Loading Title from Serialization");
            Name = GetSerializedString(info,"name");
            //Id = GetSerializedInt( info,"itemid");
            MetadataSourceID = GetSerializedString(info,"sourceid");
            MetadataSourceName = GetSerializedString(info,"sourcename");
            FrontCoverPath = GetSerializedString(info,"front_boxart_path");
            FrontCoverMenuPath = GetSerializedString(info, "front_boxart_menu_path");
            BackCoverPath = GetSerializedString(info,"back_boxart_path");
            Synopsis = GetSerializedString(info,"synopsis");
            Studio = GetSerializedString(info,"distributor");
            CountryOfOrigin = GetSerializedString(info,"country_of_origin");
            OfficialWebsiteURL = GetSerializedString(info,"official_website_url");
            DateAdded = GetSerializedDateTime(info, "date_added");
            ImporterSource = GetSerializedString(info,"importer_source");
            Runtime = GetSerializedInt(info, "runtime");
            ParentalRating = GetSerializedString(info, "mpaa_rating");
            

            ReleaseDate = GetSerializedDateTime(info, "release_date");
            _producers = GetSerializedList<List<string>>(info, "producers");
            foreach (string p in _producers)
                TitleCollectionManager.AddPersonToTitle(this, p, PeopleRole.Producers);
            _writers = GetSerializedList<List<Person>>(info, "writers");
            foreach (Person p in _writers)
                TitleCollectionManager.AddPersonToTitle(this, p.full_name, PeopleRole.Writer);
            _directors = GetSerializedList<List<Person>>(info, "directors");
            foreach (Person p in _directors)
                TitleCollectionManager.AddPersonToTitle(this, p.full_name, PeopleRole.Director);
            _audioTracks = GetSerializedList<List<string>>(info, "language_formats");
            _title.AudioTracks = Dao.TitleDao.GetDelimitedStringFromCollection(_audioTracks);
            _genres = GetSerializedList<List<string>>(info, "genres");
            foreach (string g in _genres)
                TitleCollectionManager.AddGenreToTitle(this, g);

            UserStarRating = GetSerializedInt(info,"user_star_rating");
            AspectRatio = GetSerializedString(info,"aspect_ratio");
            VideoStandard = GetSerializedString(info,"video_standard");
            UPC = GetSerializedString(info,"upc");
            OriginalName = GetSerializedString(info,"original_name");
            _tags = GetSerializedList<List<string>>(info, "tags");
            foreach (string t in _tags)
                TitleCollectionManager.AddTagToTitle(this, t);
            _actingRoles = GetSerializedList<Dictionary<string, string>>(info, "acting_roles");
            foreach (string key in _actingRoles.Keys)
                TitleCollectionManager.AddActorToTitle(this, key, _actingRoles[key]);
            _nonActingRoles = GetSerializedList<Dictionary<string, string>>(info, "nonacting_roles");
            foreach (string key in _nonActingRoles.Keys)
                TitleCollectionManager.AddActorToTitle(this, key, _nonActingRoles[key], PeopleRole.NonActing);
            _trailers = GetSerializedList<List<string>>(info, "trailers");
            _title.Trailers = Dao.TitleDao.GetDelimitedStringFromCollection(_trailers);
            SortName = GetSerializedString(info, "sort_name");
            ParentalRatingReason = GetSerializedString(info, "mpaa_rating_reason");
            VideoDetails = GetSerializedString(info, "video_details");
            _subtitles = GetSerializedList<List<string>>(info, "subtitles");
            _title.Subtitles = Dao.TitleDao.GetDelimitedStringFromCollection(_subtitles);
            _disks = GetSerializedList<List<Disk>>(info, "disks");
            foreach (Disk d in _disks)
                _title.Disks.Add(d.DaoDisk);
            
            CleanDuplicateDisks();
            if (VideoFormat == VideoFormat.DVD)
            {
                if (VideoStandard == "PAL")
                    VideoResolution = "720x576";
                else
                    VideoResolution = "720x480";
            }
            VideoResolution = GetSerializedString(info, "video_resolution");
            ExtraFeatures = GetSerializedList<List<string>>(info, "extra_features");
            WatchedCount = GetSerializedInt(info, "watched_count");
        }

        void CleanDuplicateDisks()
        {
            foreach (Disk d in new List<Disk>(this._disks))
            {
                int i = _disks.IndexOf(d);
                if (i < 0)
                    continue;
                while (true) {
                    int oi = _disks.IndexOf(d, i+1);
                    if (oi < 0)
                        break;
                    _disks.RemoveAt(oi);
                }
            }
        }

        /// <summary>
        /// Used for serializing the title object (required for the ISerializable interface)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {

        }
        #endregion

        public string BasePath()
        {
            string folder = string.Empty;
            if (!string.IsNullOrEmpty(FileLocation))
                if (Directory.Exists(FileLocation))
                    folder = FileLocation;
                else
                    folder = Path.GetDirectoryName(FileLocation);
            else
            {
                if (Disks.Count > 0)
                {
                    if (Disks[0].Path.Length > 0)
                        if (Directory.Exists(Disks[0].Path))
                            folder = Disks[0].Path;
                        else
                            folder = Path.GetDirectoryName(Disks[0].Path);
                }
            }

            if (folder.Length > 0)
            {
                try
                {
                    if (Directory.Exists(folder))
                    {
                        if (folder.ToUpperInvariant().EndsWith("VIDEO_TS"))
                            folder = Path.GetDirectoryName(folder);
                        return folder;
                    }

                    return null;
                }
                catch (Exception e)
                {
                    Utilities.DebugLine("[Title] Error Looking for folder: {0}", e.Message);
                }
            }
            return null;
        }

        /// <summary>
        /// This returns the fanartfolder if _fanartfolder is defined. If not it will try to see
        /// if there is a fanart folder allready in existance under the movie folder first then
        /// the centralised folder. _fanartfolder is then set to this ready for serialisation.
        /// 
        /// This routine uses a primitive token to represent basepath of the movie and this is
        /// substituted when required. This means if the movie is moved and the basepath changed,
        /// this will return the correct fanart folder.
        /// 
        /// </summary>
        public string BackDropFolder
        {
            get {
                // Is there allready a fan art folder defined
                if (!string.IsNullOrEmpty(_fanartfolder))
                {
                    if (!string.IsNullOrEmpty(this.BasePath()))
                    {
                        // Perform token substitution.
                        return _fanartfolder.Replace("{basepath}", this.BasePath());
                    }
                    else
                    {
                        return _fanartfolder;
                    }
                }

                // Check for an existing fanart folder under the movie/disk folder
                if (!string.IsNullOrEmpty(this.BasePath()))
                {
                    if (Directory.Exists(Path.Combine(this.BasePath(), @"FanArt")))
                    {
                        _fanartfolder = @"{basepath}\FanArt";
                        return _fanartfolder.Replace("{basepath}", this.BasePath());
                    }
                }

                // Check for an existing fanart folder under the centralised folder
                if (!string.IsNullOrEmpty(Properties.Settings.Default.gsTitledFanArtPath))
                {
                    string MainFanArtDir = System.IO.Path.Combine(Properties.Settings.Default.gsTitledFanArtPath, PathSafeName);
                    if (Directory.Exists(MainFanArtDir))
                    {
                        _fanartfolder = MainFanArtDir;
                        return _fanartfolder;
                    }
                }
                return null;

            }
            set { _fanartfolder = value; }
        }

        public string CreateFanArtFolder(string basepath)
        {
            if (string.IsNullOrEmpty(_fanartfolder))
            {
                if (Properties.Settings.Default.gbTitledFanArtFolder)
                {
                    // Centralised Fan Art
                    string MainFanArtDir = Properties.Settings.Default.gsTitledFanArtPath;
                    if (!Directory.Exists(MainFanArtDir)) Directory.CreateDirectory(MainFanArtDir);
                    _fanartfolder = System.IO.Path.Combine(MainFanArtDir, PathSafeName);
                }
                else
                {
                    // Fan art local to movie
                    if (string.IsNullOrEmpty(basepath))
                    {
                        return null;
                    }
                    _fanartfolder = Path.Combine(basepath, @"FanArt");
                }
            }
            if (!Directory.Exists(_fanartfolder)) Directory.CreateDirectory(_fanartfolder);
            return _fanartfolder;
        }       
    }

    public class Role
    {
        public string PersonName;
        public string RoleName;

        public Role(string personName, string roleName)
        {
            PersonName = personName;
            RoleName = roleName;
        }

        public override string ToString()
        {
            return PersonName + " as " + RoleName;
        }

        public string Display
        {
            get { return ToString(); }
        }
    }
}
