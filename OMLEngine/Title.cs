using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OMLEngine
{
    [Serializable()]
    public class Title : ISerializable
    {
        #region locals
        private int _watched;
        private string _file_location;
        private VideoFormat _video_format;
        private bool _transcode_to_extender;
        private string _name;
        private string _description;
        private int _itemId;
        private string _sourceId;
        private string _source_name;
        private string _front_boxart_path;
        private string _back_boxart_path;
        private string _runtime;
        private Rating _mpaa_rating;
        private string _synopsis;
        private string _distributor;
        private string _country_of_origin;
        private string _official_website_url;
        private DateTime _release_date;
        private DateTime _date_added;
        private string _importer_source;
        private List<Person> _actors;
        private List<Person> _crew;
        private List<Person> _directors;
        private List<Person> _writers;
        private List<string> _producers;
        private List<string> _sound_formats;
        private List<string> _language_formats;
        private List<Genre> _genres;
        #endregion

        #region properties

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
            get { return _file_location; }
            set { _file_location = value; }
        }

        /// <summary>
        /// Video format of title
        /// </summary>
        public VideoFormat VideoFormat
        {
            get { return _video_format; }
            set { _video_format = value; }
        }

        /// <summary>
        /// Does title need to be transcoded to extender devices
        /// </summary>
        public bool TranscodeToExtender
        {
            get { return _transcode_to_extender; }
            set { _transcode_to_extender = value; }
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
        public int itemId
        {
            get { return _itemId; }
            set
            {
                if (value >= 0)
                    _itemId = value;
            }
        }
        /// <summary>
        /// Unique id from the Source of our title info (MyMovies, DVD Profiler, etc).
        /// </summary>
        public string sourceId
        {
            get { return _sourceId; }
            set
            {
                if (value != null)
                    _sourceId = value;
            }
        }
        /// <summary>
        /// Name of the source for our info (MyMovies, DVD Profiler, etc)
        /// </summary>
        public string sourceName
        {
            get { return _source_name; }
            set
            {
                if (value != null)
                    _source_name = value;
            }
        }
        /// <summary>
        /// Pull path to the cover art image
        /// </summary>
        public string front_boxart_path
        {
            get { return _front_boxart_path; }
            set { _front_boxart_path = value; }
        }
        /// <summary>
        /// Full path to the rear cover art image
        /// </summary>
        public string back_boxart_path
        {
            get { return _back_boxart_path; }
            set { _back_boxart_path = value; }
        }
        /// <summary>
        /// Runtime in minutes of the title
        /// </summary>
        public string Runtime
        {
            get { return _runtime; }
            set
            {
                if (value != null)
                    _runtime = value;
            }
        }
        /// <summary>
        /// Rating of the film acording to the MPAA
        /// </summary>
        public Rating MPAARating
        {
            get { return _mpaa_rating; }
            set
            {
                if (value != null)
                    _mpaa_rating = value;
            }
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
        public string Country_Of_Origin
        {
            get { return _country_of_origin; }
            set { _country_of_origin = value; }
        }
        /// <summary>
        /// website for title (if it has one)
        /// </summary>
        public string Official_Website_Url
        {
            get { return _official_website_url; }
            set { _official_website_url = value; }
        }
        /// <summary>
        /// Original date of release (or re-release)
        /// </summary>
        public DateTime ReleaseDate
        {
            get { return _release_date; }
            set
            {
                if (value != null)
                    _release_date = value;
            }
        }
        /// <summary>
        /// Date that this title was added to the database
        /// </summary>
        public DateTime DateAdded
        {
            get { return _date_added; }
            set { _date_added = value; }
        }
        /// <summary>
        /// Name of the source from which meta-data was gathered (MyMovies, DVD Profiler, etc.)
        /// </summary>
        public string Importer_Source
        {
            get { return _importer_source; }
            set { _importer_source = value; }
        }
        /// <summary>
        /// List of sound formats (DTS, DD5.1, DD2.0, etc)
        /// </summary>
        public IList SoundFormats
        {
            get { return _language_formats; }
        }
        /// <summary>
        /// List of languages (English, Spanish, French, etc)
        /// </summary>
        public IList LanguageFormats
        {
            get { return _sound_formats; }
        }
        /// <summary>
        /// List of Genres (see the Genre Enumerator)
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
        /// <summary>
        /// Constructor used for loading from database file
        /// </summary>
        /// <param name="info">SerializationInfo object</param>
        /// <param name="ctxt">StreamingContext object</param>
        public Title(SerializationInfo info, StreamingContext ctxt)
        {
            _file_location = info.GetString("file_location");
            _video_format = (VideoFormat)info.GetValue("video_format", typeof(VideoFormat));
            _transcode_to_extender = info.GetBoolean("transcode_to_extender");
            _name = info.GetString("name");
            _description = info.GetString("description");
            _itemId = (int)info.GetValue("itemid", typeof(int));
            _sourceId = info.GetString("sourceid");
            _source_name = info.GetString("sourcename");
            _front_boxart_path = info.GetString("front_boxart_path");
            _back_boxart_path = info.GetString("back_boxart_path");
            _synopsis = info.GetString("synopsis");
            _distributor = info.GetString("distributor");
            _country_of_origin = info.GetString("country_of_origin");
            _official_website_url = info.GetString("official_website_url");
            _date_added = info.GetDateTime("date_added");
            _importer_source = info.GetString("importer_source");
            _runtime = info.GetString("runtime");
            _mpaa_rating = (Rating)info.GetValue("mpaa_rating", typeof(Rating));
            _release_date = info.GetDateTime("release_date");
            _actors = (List<Person>)info.GetValue("actors", typeof(List<Person>));
            _crew = (List<Person>)info.GetValue("crew", typeof(List<Person>));
            _producers = (List<string>)info.GetValue("producers", typeof(List<string>));
            _writers = (List<Person>)info.GetValue("writers", typeof(List<Person>));
            _directors = (List<Person>)info.GetValue("directors", typeof(List<Person>));
            _sound_formats = (List<string>)info.GetValue("sound_formats", typeof(List<string>));
            _language_formats = (List<string>)info.GetValue("language_formats", typeof(List<string>));
            _genres = (List<Genre>)info.GetValue("genres", typeof(List<Genre>));
        }

        /// <summary>
        /// Used for serializing the title object (required for the ISerializable interface)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("file_location", _file_location);
            info.AddValue("video_format", _video_format);
            info.AddValue("transcode_to_extender", _transcode_to_extender);
            info.AddValue("name", _name);
            info.AddValue("description", _description);
            info.AddValue("itemid", _itemId);
            info.AddValue("sourceid", _sourceId);
            info.AddValue("sourcename", _source_name);
            info.AddValue("front_boxart_path", _front_boxart_path);
            info.AddValue("back_boxart_path", _back_boxart_path);
            info.AddValue("synopsis", _synopsis);
            info.AddValue("distributor", _distributor);
            info.AddValue("country_of_origin", _country_of_origin);
            info.AddValue("official_website_url", _official_website_url);
            info.AddValue("date_added", _date_added);
            info.AddValue("importer_source", _importer_source);
            info.AddValue("runtime", _runtime);
            info.AddValue("mpaa_rating", _mpaa_rating);
            info.AddValue("release_date", _release_date);
            info.AddValue("actors", _actors);
            info.AddValue("crew", _crew);
            info.AddValue("producers", _producers);
            info.AddValue("writers", _writers);
            info.AddValue("directors", _directors);
            info.AddValue("sound_formats", _sound_formats);
            info.AddValue("language_formats", _language_formats);
            info.AddValue("genres", _genres);
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
            _sound_formats = new List<string>();
            _language_formats = new List<string>();
            _genres = new List<Genre>();
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
        public void AddGenre(Genre genre)
        {
            _genres.Add(genre);
        }
        /// <summary>
        /// Add a string sound format to the sound formats list
        /// </summary>
        /// <param name="sound_format">string name to add</param>
        public void AddSoundFormat(string sound_format)
        {
            _sound_formats.Add(sound_format);
        }
        /// <summary>
        /// Add a string language to the language formats list
        /// </summary>
        /// <param name="language_format">string name to add</param>
        public void AddLanguageFormat(string language_format)
        {
            _language_formats.Add(language_format);
        }
    }
}
