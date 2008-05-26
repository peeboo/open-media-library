using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Reflection;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace OMLEngine
{
    [Serializable()]
    public class Title : IComparable, ISerializable
    {
        #region locals
        private int _watched;
        private string _fileLocation = "";
        private VideoFormat _videoFormat = VideoFormat.DVD;
        private bool _needsTranscode = false;
        private string _name = "";
        private string _description = "";
        private int _itemId = -1;
        private string _metadataSourceId = "";
        private string _sourceName = "";
        private string _frontCoverPath = "";
        private string _backCoverPath = "";
        private int _runtime = 0;
        private string _MPAARating = "";
        private string _synopsis = "";
        private string _distributor = "";
        private string _countryOfOrigin = "";
        private string _officialWebsiteURL = "";
        private DateTime _releaseDate;
        private DateTime _dateAdded;
        private string _importerSource = "";
        private List<Person> _actors = new List<Person>();
        private List<Person> _crew = new List<Person>();
        private List<Person> _directors = new List<Person>();
        private List<Person> _writers = new List<Person>();
        private List<string> _producers = new List<string>();
        private List<string> _soundFormats = new List<string>();
        private List<string> _languageFormats = new List<string>();
        private List<string> _genres = new List<string>();
        private int _userStarRating = 0;
        private string _aspectRatio = "";    // Widescreen, 1.33, 1.66, 
        private string _videoStandard = "";  // NTSC, PAL
        private string _UPC = "";
        private string _originalName = "";


        #endregion

        #region properties

        public string OriginalName
        {
            get { return _originalName; }
            set { _originalName = value; }
        } 

        public string VideoStandard
        {
            get { return _videoStandard; }
            set { _videoStandard = value; }
        }

        public string AspectRatio
        {
            get { return _aspectRatio; }
            set { _aspectRatio = value; }
        }

        public string UPC
        {
            get { return _UPC; }
            set { _UPC = value; }
        }

        public int UserStarRating
        {
            get { return _userStarRating; }
            set { _userStarRating = value; }
        }

        /// <summary>
        /// Has this title been watched before or not
        /// </summary>
        public int HasWatched
        {
            get { return _watched; }
            set
            {
                _watched = value;
            }
        }

        /// <summary>
        ///  Physical location of media
        /// </summary>
        public string FileLocation
        {
            get { return _fileLocation; }
            set { _fileLocation = value; }
        }

        /// <summary>
        /// Video format of title
        /// </summary>
        public VideoFormat VideoFormat
        {
            get { return _videoFormat; }
            set { _videoFormat = value; }
        }

        /// <summary>
        /// Display name of movie
        /// </summary>
        public string Name
        {
            get { return _name; }
            set {
                if (value != null)
                    _name = value;
            }
        }
        /// <summary>
        /// Short description of the Title
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (value != null)
                    _description = value;
            }
        }
        /// <summary>
        /// Internal id of the Title
        /// </summary>
        public int InternalItemID
        {
            get { return _itemId; }
        }
        /// <summary>
        /// Unique id from the Source of our title info (MyMovies, DVD Profiler, etc).
        /// </summary>
        public string MetadataSourceID
        {
            get { return _metadataSourceId; }
            set
            {
                if (value != null)
                    _metadataSourceId = value;
            }
        }
        /// <summary>
        /// Name of the source for our info (MyMovies, DVD Profiler, etc)
        /// </summary>
        public string MetadataSourceName
        {
            get { return _sourceName; }
            set
            {
                if (value != null)
                    _sourceName = value;
            }
        }
        /// <summary>
        /// Pull path to the cover art image
        /// </summary>
        public string FrontCoverPath
        {
            get { return _frontCoverPath; }
            set { _frontCoverPath = value; }
        }
        /// <summary>
        /// Full path to the rear cover art image
        /// </summary>
        public string BackCoverPath
        {
            get { return _backCoverPath; }
            set { _backCoverPath = value; }
        }
        /// <summary>
        /// Runtime in minutes of the title
        /// </summary>
        public int Runtime
        {
            get { return _runtime; }
            set { _runtime = value; }
        }
        /// <summary>
        /// Rating of the film acording to the MPAA
        /// </summary>
        public string MPAARating
        {
            get { return _MPAARating; }
            set { _MPAARating = value; }
        }
        /// <summary>
        /// Long description of title
        /// </summary>
        public string Synopsis
        {
            get { return _synopsis; }
            set { _synopsis = value; }
        }
        /// <summary>
        /// Name of distribution company
        /// </summary>
        public string Distributor
        {
            get { return _distributor; }
            set { _distributor = value; }
        }
        /// <summary>
        /// Country where the title was created/first released
        /// </summary>
        public string CountryOfOrigin
        {
            get { return _countryOfOrigin; }
            set { _countryOfOrigin = value; }
        }
        /// <summary>
        /// website for title (if it has one)
        /// </summary>
        public string OfficialWebsiteURL
        {
            get { return _officialWebsiteURL; }
            set { _officialWebsiteURL = value; }
        }
        /// <summary>
        /// Original date of release (or re-release)
        /// </summary>
        public DateTime ReleaseDate
        {
            get { return _releaseDate; }
            set
            {
                if (value != null)
                    _releaseDate = value;
            }
        }
        /// <summary>
        /// Date that this title was added to the database
        /// </summary>
        public DateTime DateAdded
        {
            get { return _dateAdded; }
            set { _dateAdded = value; }
        }
        /// <summary>
        /// Name of the source from which meta-data was gathered (MyMovies, DVD Profiler, etc.)
        /// </summary>
        public string ImporterSource
        {
            get { return _importerSource; }
            set { _importerSource = value; }
        }
        /// <summary>
        /// List of sound formats (DTS, DD5.1, DD2.0, etc)
        /// </summary>
        public IList SoundFormats
        {
            get { return _soundFormats; }
        }
        /// <summary>
        /// List of languages (English, Spanish, French, etc)
        /// </summary>
        public IList LanguageFormats
        {
            get { return _languageFormats; }
        }
        /// <summary>
        /// List of Genres
        /// </summary>
        public IList Genres
        {
            get { return _genres; }
        }
        /// <summary>
        /// List of actors (Person objects)
        /// </summary>
        public IList Actors
        {
            get { return _actors; }
        }
        /// <summary>
        /// List of Person objects that worked on the film
        /// </summary>
        public IList Crew
        {
            get { return _crew; }
        }
        /// <summary>
        /// List of Person objects that directed the title (usually one Person)
        /// </summary>
        public IList Directors
        {
            get { return _directors; }
        }
        /// <summary>
        /// List of Person objects that wrote the title
        /// </summary>
        public IList Writers
        {
            get { return _writers; }
        }
        /// <summary>
        /// List of people/companies that produced the title
        /// </summary>
        public IList Producers
        {
            get { return _producers; }
        }
        #endregion

        #region serialization methods

        private string GetSerializedString(SerializationInfo info, string id)
        {
            string result = info.GetString(id);
            return (result == null ? String.Empty : result);
        }

        private  T GetSerializedList<T>(SerializationInfo info, string id) where T : new()
        {
            T result = (T)info.GetValue(id, typeof(T));
            return (result == null ? new T() : result);
        }

        /// <summary>
        /// Constructor used for loading from database file
        /// </summary>
        /// <param name="info">SerializationInfo object</param>
        /// <param name="ctxt">StreamingContext object</param>
        public Title(SerializationInfo info, StreamingContext ctxt)
        {
            _fileLocation = GetSerializedString(info, "file_location");
            _videoFormat = (VideoFormat)info.GetValue("video_format", typeof(VideoFormat));
            _needsTranscode = info.GetBoolean("transcode_to_extender");
            _name = GetSerializedString(info,"name");
            _description = GetSerializedString(info,"description");
            _itemId = (int)info.GetValue("itemid", typeof(int));
            _metadataSourceId = GetSerializedString(info,"sourceid");
            _sourceName = GetSerializedString(info,"sourcename");
            _frontCoverPath = GetSerializedString(info,"front_boxart_path");
            _backCoverPath = GetSerializedString(info,"back_boxart_path");
            _synopsis = GetSerializedString(info,"synopsis");
            _distributor = GetSerializedString(info,"distributor");
            _countryOfOrigin = GetSerializedString(info,"country_of_origin");
            _officialWebsiteURL = GetSerializedString(info,"official_website_url");
            _dateAdded = info.GetDateTime("date_added");
            _importerSource = GetSerializedString(info,"importer_source");
            _runtime = (int)info.GetValue("runtime", typeof(int));
            _MPAARating = GetSerializedString(info,"mpaa_rating");

            _releaseDate = info.GetDateTime("release_date");
            _actors = GetSerializedList<List<Person>>(info, "actors");
            _crew = GetSerializedList<List<Person>>(info, "crew");
            _producers = GetSerializedList<List<string>>(info, "producers");
            _writers = GetSerializedList<List<Person>>(info, "writers");
            _soundFormats = GetSerializedList<List<string>>(info, "sound_formats");
            _directors = GetSerializedList<List<Person>>(info, "directors");
            _languageFormats = GetSerializedList<List<string>>(info, "language_formats");
            _genres = GetSerializedList<List<string>>(info, "genres");

            _userStarRating = (int)info.GetValue("user_star_rating", typeof(int));
            _aspectRatio = GetSerializedString(info,"aspect_ratio");
            _videoStandard = GetSerializedString(info,"video_standard");
            _UPC = GetSerializedString(info,"upc");
            _originalName = GetSerializedString(info,"original_name");

            if (_itemId == 0)
                _itemId = Utilities.NewRandomNumber();
        }

        /// <summary>
        /// Used for serializing the title object (required for the ISerializable interface)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("file_location", _fileLocation);
            info.AddValue("video_format", _videoFormat);
            info.AddValue("transcode_to_extender", _needsTranscode);
            info.AddValue("name", _name);
            info.AddValue("description", _description);
            info.AddValue("itemid", _itemId);
            info.AddValue("sourceid", _metadataSourceId);
            info.AddValue("sourcename", _sourceName);
            info.AddValue("front_boxart_path", _frontCoverPath);
            info.AddValue("back_boxart_path", _backCoverPath);
            info.AddValue("synopsis", _synopsis);
            info.AddValue("distributor", _distributor);
            info.AddValue("country_of_origin", _countryOfOrigin);
            info.AddValue("official_website_url", _officialWebsiteURL);
            info.AddValue("date_added", _dateAdded);
            info.AddValue("importer_source", _importerSource);
            info.AddValue("runtime", _runtime);
            info.AddValue("mpaa_rating", _MPAARating);
            info.AddValue("release_date", _releaseDate);
            info.AddValue("actors", _actors);
            info.AddValue("crew", _crew);
            info.AddValue("producers", _producers);
            info.AddValue("writers", _writers);
            info.AddValue("directors", _directors);
            info.AddValue("sound_formats", _soundFormats);
            info.AddValue("language_formats", _languageFormats);
            info.AddValue("genres", _genres);
            info.AddValue("user_star_rating", _userStarRating);
            info.AddValue("aspect_ratio", _aspectRatio);
            info.AddValue("video_standard", _videoStandard);
            info.AddValue("upc", _UPC);
            info.AddValue("original_name", _originalName);

        }
        #endregion

        /// <summary>
        /// Generic Constructor, inits all the IList items
        /// </summary>
        public Title()
        {
            _actors = new List<Person>();
            _crew = new List<Person>();
            _directors = new List<Person>();
            _writers = new List<Person>();
            _producers = new List<string>();
            _soundFormats = new List<string>();
            _languageFormats = new List<string>();
            _genres = new List<string>();
            Random random = new Random(new DateTime().Millisecond);
            _itemId = Utilities.NewRandomNumber();
        }

        /// <summary>
        /// Default destructor
        /// </summary>
        ~Title()
        {
        }

        /// <summary>
        /// Add a Person object to the actors list
        /// </summary>
        /// <param name="actor">Person object to add</param>
        public void AddActor(Person actor)
        {
            if (!_actors.Contains(actor))
                _actors.Add(actor);
        }
        /// <summary>
        /// Add a Person object to the crew list
        /// </summary>
        /// <param name="crew_member">Person object to add</param>
        public void AddCrew(Person crew_member)
        {
            if (!_crew.Contains(crew_member))
                _crew.Add(crew_member);
        }
        /// <summary>
        /// Add a Person object to the directors list
        /// </summary>
        /// <param name="director">Person object to add</param>
        public void AddDirector(Person director)
        {
            _directors.Add(director);
        }
        /// <summary>
        /// Add a Person object to the writers list
        /// </summary>
        /// <param name="writer">Person object to add</param>
        public void AddWriter(Person writer)
        {
            _writers.Add(writer);
        }
        /// <summary>
        /// Add a string (person name or company name) to the producers list
        /// </summary>
        /// <param name="producer">string name to add</param>
        public void AddProducer(string producer)
        {
            _producers.Add(producer);
        }
        /// <summary>
        /// Add a Genre to the genres list
        /// </summary>
        /// <param name="genre">A Genre from the Genre enum</param>
        public void AddGenre(string genre)
        {
            _genres.Add(genre);
        }
        /// <summary>
        /// Add a string sound format to the sound formats list
        /// </summary>
        /// <param name="sound_format">string name to add</param>
        public void AddSoundFormat(string sound_format)
        {
            _soundFormats.Add(sound_format);
        }
        /// <summary>
        /// Add a string language to the language formats list
        /// </summary>
        /// <param name="language_format">string name to add</param>
        public void AddLanguageFormat(string language_format)
        {
            _languageFormats.Add(language_format);
        }

        /*
        public int CompareTo(object other)
        {
            return InternalItemID.CompareTo(
                ((Title)other).InternalItemID
                );
        }
        */
        public int CompareTo(object other)
        {
            return Name.CompareTo(
                ((Title)other).Name
                );
        }
    }
}
