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

namespace OMLEngine
{
    [Serializable()]
    [XmlRootAttribute("OMLTitle", Namespace = "http://www.openmedialibrary.org/", IsNullable = false)]
    public class Title : IComparable, ISerializable, IXmlSerializable
    {
        #region locals
        private int _watchedCount;
        private string _fileLocation = "";
        private VideoFormat _videoFormat = VideoFormat.DVD;
        private bool _needsTranscode = false;
        private string _name = "";
        private int _itemId = -1;
        private string _metadataSourceId = "";
        private string _sourceName = "";
        private string _frontCoverPath = "";
        private string _frontCoverMenuPath = "";
        private string _backCoverPath = "";
        private int _runtime = 0;
        private string _parentalRating = "";
        private string _synopsis = "";
        private string _studio = "";
        private string _countryOfOrigin = "";
        private string _officialWebsiteURL = "";
        private DateTime _releaseDate = DateTime.MinValue;
        private DateTime _dateAdded = DateTime.MinValue;
        private string _importerSource = "";
        private List<Person> _actors = new List<Person>();
        private List<Person> _directors = new List<Person>();
        private List<Person> _writers = new List<Person>();
        private List<string> _producers = new List<string>();
        private List<string> _audioTracks = new List<string>();
        private List<string> _genres = new List<string>();
        private int _userStarRating = 0;
        private string _aspectRatio = "";    // Widescreen, 1.33, 1.66, 
        private string _videoStandard = "";  // NTSC, PAL
        private string _UPC = "";
        private string _originalName = "";
        private List<string> _tags = new List<string>();
        private Dictionary<string, string> _actingRoles = new Dictionary<string, string>(); // actor, role
        private Dictionary<string, string> _nonActingRoles = new Dictionary<string, string>(); // name, role (ie. Vangelis, Music)
        private Dictionary<string, string> _additionalFields = new Dictionary<string, string>();
        private List<string> _photos = new List<string>();
        private List<string> _trailers = new List<string>();
        private List<int> _children = new List<int>();
        private int _parent = 0;
        private string _sortName = "";
        private string _parentalRatingReason = "";
        private string _videoDetails = "";
        private List<string> _subtitles = new List<string>();
        private string _videoResolution = "";
        private List<string> _extraFeatures = new List<string>();
        private List<Disk> _disks = new List<Disk>();
        private Disk _selectedDisk = null;

        private static string XmlNameSpace = "http://www.openmedialibrary.org/";

        #endregion

        #region properties

        public Disk SelectedDisk
        {
            get { return _selectedDisk; }
            set { _selectedDisk = value; }
        }

        public List<string> ExtraFeatures
        {
            get { return _extraFeatures; }
            set { _extraFeatures = value; }
        }

        public string VideoResolution
        {
            get { return _videoResolution; }
            set { _videoResolution = value; }
        }
        
        public List<string> Subtitles
        {
            get { return _subtitles; }
            set { _subtitles = value; }
        }

        public string VideoDetails
        {
            get { return _videoDetails; }
            set { _videoDetails = value; }
        }

        public string ParentalRatingReason
        {
            get { return _parentalRatingReason; }
            set { _parentalRatingReason = value; }
        }


        public string SortName
        {
            get 
            {
                if (String.IsNullOrEmpty(_sortName))
                    return Name;
                else
                    return _sortName; 
            }
            set { _sortName = value.Trim(); }
        }

        public Dictionary<string, string> NonActingRoles
        {
            get { return _nonActingRoles; }
            set { _nonActingRoles = value; }
        }

        public Dictionary<string, string> ActingRoles
        {
            get { return _actingRoles; }
            set { _actingRoles = value; }
        }

        public void AddActingRole(string actor, string role)
        {
            if (actor == null || role == null) return;
            if (!_actingRoles.ContainsKey(actor))
            {
                _actingRoles.Add(actor, role);
            }
        }

        public void AddNonActingRole(string name, string role)
        {
            if (name == null || role == null) return;
            if (!_nonActingRoles.ContainsKey(name))
            {
                _nonActingRoles.Add(name, role);
            }
        }


        public int Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<int> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        public List<string> Photos
        {
            get { return _photos; }
            set { _photos = value; }
        }

        public List<string> Trailers
        {
            get { return _trailers; }
            set { _trailers = value; }
        }

        /// <summary>
        /// Gets or sets the additional fields (for future expansion).
        /// </summary>
        /// <value>The additional fields.</value>
        public Dictionary<string, string> AdditionalFields
        {
            get { return _additionalFields; }
            set { _additionalFields = value; }
        }


        /// <summary>
        /// A user can add tags to movies
        /// </summary>
        /// <value>The tags.</value>
        public List<string> Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        /// <summary>
        /// Gets or sets the name of the original name (especially for foreign movies)
        /// </summary>
        /// <value>The name of the original name.</value>
        public string OriginalName
        {
            get { return _originalName; }
            set { _originalName = value; }
        }

        /// <summary>
        /// Gets or sets the video standard (NTSC, PAL).
        /// </summary>
        /// <value>The video standard.</value>
        public string VideoStandard
        {
            get { return _videoStandard; }
            set { _videoStandard = value; }
        }

        /// <summary>
        /// Gets or sets the aspect ratio (1.33:1, Widescreen, etc)
        /// </summary>
        /// <value>The aspect ratio.</value>
        public string AspectRatio
        {
            get { return _aspectRatio; }
            set { _aspectRatio = value; }
        }

        /// <summary>
        /// Gets or sets the UPC.
        /// </summary>
        /// <value>The UPC.</value>
        public string UPC
        {
            get { return _UPC; }
            set { _UPC = value; }
        }

        /// <summary>
        /// Gets or sets the user star rating (0 to 100)
        /// </summary>
        /// <value>The user star rating.</value>
        public int UserStarRating
        {
            get { return _userStarRating; }
            set { _userStarRating = value; }
        }

        /// <summary>
        /// Number of times video has been watched
        /// </summary>
        public int WatchedCount
        {
            get { return _watchedCount; }
            set
            {
                _watchedCount = value;
            }
        }

        /// <summary>
        ///  Physical location of media
        /// </summary>
        public string FileLocation
        {
            get 
            {
                if (this._disks.Count == 1)
                    return (this._disks[0].Path);
                else
                    return (String.Empty);
            
            }
        }

        // To Support Multi-Disk!
        public List<Disk> Disks
        {
            get { return _disks; }
            set { _disks = value; }
        }

        /// <summary>
        /// Video format of title (DVD, AVI, etc)
        /// </summary>
        public VideoFormat VideoFormat
        {
            get
            {
                if (this.Disks.Count > 0)
                    return _disks[0].Format;
                else
                    return VideoFormat.DVD;
            }
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
        /// Pull path to the cover art image, 
        /// default the the front cover menu art image to this as well
        /// </summary>
        public string FrontCoverPath
        {
            get { return _frontCoverPath; }
            set 
            { 
                _frontCoverPath = value;
                if (string.IsNullOrEmpty(_frontCoverMenuPath))
                    _frontCoverMenuPath = value;
            }
        }

        public string FrontCoverMenuPath
        {
            get { return _frontCoverMenuPath; }
            set { _frontCoverMenuPath = value; }
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
        /// Rating of the film 
        /// </summary>
        public string ParentalRating
        {
            get { return _parentalRating; }
            set { _parentalRating = value; }
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
        /// Name of studio company
        /// </summary>
        public string Studio
        {
            get { return _studio; }
            set { _studio = value; }
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
        /// List of languages (English, Spanish, French, DTS, DD5.1, DD2.0, etc)
        /// </summary>
        public List<string> AudioTracks
        {
            get { return _audioTracks; }
        }
        /// <summary>
        /// List of Genres
        /// </summary>
        public List<string> Genres
        {
            get { return _genres; }
        }
        ///// <summary>
        ///// List of actors (Person objects)
        ///// </summary>
        //public List<Person> Actors
        //{
        //    get { return _actors; }
        //}
        /// <summary>
        /// List of Person objects that directed the title (usually one Person)
        /// </summary>
        public List<Person> Directors
        {
            get { return _directors; }
        }
        /// <summary>
        /// List of Person objects that wrote the title
        /// </summary>
        public List<Person> Writers
        {
            get { return _writers; }
        }
        /// <summary>
        /// List of people/companies that produced the title
        /// </summary>
        public List<string> Producers
        {
            get { return _producers; }
        }
        #endregion

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
            //Utilities.DebugLine("[Title] Loading Title from Serialization");
            _fileLocation = GetSerializedString(info, "file_location");
            _videoFormat = GetSerializedVideoFormat(info, "video_format");
            _needsTranscode = GetSerializedBoolean(info, "transcode_to_extender");
            _name = GetSerializedString(info,"name");
            _itemId = GetSerializedInt( info,"itemid");
            _metadataSourceId = GetSerializedString(info,"sourceid");
            _sourceName = GetSerializedString(info,"sourcename");
            _frontCoverPath = GetSerializedString(info,"front_boxart_path");
            _frontCoverMenuPath = GetSerializedString(info, "front_boxart_menu_path");
            _backCoverPath = GetSerializedString(info,"back_boxart_path");
            _synopsis = GetSerializedString(info,"synopsis");
            _studio = GetSerializedString(info,"distributor");
            _countryOfOrigin = GetSerializedString(info,"country_of_origin");
            _officialWebsiteURL = GetSerializedString(info,"official_website_url");
            _dateAdded = GetSerializedDateTime(info, "date_added");
            _importerSource = GetSerializedString(info,"importer_source");
            _runtime = GetSerializedInt(info, "runtime");
            _parentalRating = GetSerializedString(info, "mpaa_rating");
            

            _releaseDate = GetSerializedDateTime(info, "release_date");
            _actors = GetSerializedList<List<Person>>(info, "actors");
            _producers = GetSerializedList<List<string>>(info, "producers");
            _writers = GetSerializedList<List<Person>>(info, "writers");
            _directors = GetSerializedList<List<Person>>(info, "directors");
            _audioTracks = GetSerializedList<List<string>>(info, "language_formats");
            _genres = GetSerializedList<List<string>>(info, "genres");

            _userStarRating = GetSerializedInt(info,"user_star_rating");
            _aspectRatio = GetSerializedString(info,"aspect_ratio");
            _videoStandard = GetSerializedString(info,"video_standard");
            _UPC = GetSerializedString(info,"upc");
            _originalName = GetSerializedString(info,"original_name");
            _tags = GetSerializedList<List<string>>(info, "tags");
            _additionalFields = GetSerializedList<Dictionary<string, string>>(info, "additional_fields");
            _actingRoles = GetSerializedList<Dictionary<string, string>>(info, "acting_roles");
            _nonActingRoles = GetSerializedList<Dictionary<string, string>>(info, "nonacting_roles");
            _photos = GetSerializedList<List<string>>(info, "photos");
            _trailers = GetSerializedList<List<string>>(info, "trailers");
            _children = GetSerializedList<List<int>>(info, "children");
            _parent = GetSerializedInt( info,"parent");
            _sortName = GetSerializedString(info, "sort_name");
            _parentalRatingReason = GetSerializedString(info, "mpaa_rating_reason");
            _videoDetails = GetSerializedString(info, "video_details");
            _subtitles = GetSerializedList<List<string>>(info, "subtitles");
            _disks = GetSerializedList<List<Disk>>(info, "disks");
            if (_videoFormat == VideoFormat.DVD)
            {
                if (_videoStandard == "PAL")
                    _videoResolution = "720x576";
                else
                    _videoResolution = "720x480";
            }
            _videoResolution = GetSerializedString(info, "video_resolution");
            _extraFeatures = GetSerializedList<List<string>>(info, "extra_features");
        }

        /// <summary>
        /// Used for serializing the title object (required for the ISerializable interface)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            //Utilities.DebugLine("[Title] Adding Title ("+_name+") to Serialization data");
            info.AddValue("file_location", _fileLocation);
            info.AddValue("disks", _disks);
            info.AddValue("video_format", _videoFormat);
            info.AddValue("transcode_to_extender", _needsTranscode);
            info.AddValue("name", _name);
            info.AddValue("itemid", _itemId);
            info.AddValue("sourceid", _metadataSourceId);
            info.AddValue("sourcename", _sourceName);
            info.AddValue("front_boxart_path", _frontCoverPath);
            info.AddValue("front_boxart_menu_path", _frontCoverMenuPath);
            info.AddValue("back_boxart_path", _backCoverPath);
            info.AddValue("synopsis", _synopsis);
            info.AddValue("distributor", _studio);
            info.AddValue("country_of_origin", _countryOfOrigin);
            info.AddValue("official_website_url", _officialWebsiteURL);
            info.AddValue("date_added", _dateAdded);
            info.AddValue("importer_source", _importerSource);
            info.AddValue("runtime", _runtime);
            info.AddValue("mpaa_rating", _parentalRating);
            info.AddValue("release_date", _releaseDate);
            info.AddValue("actors", _actors);
            info.AddValue("producers", _producers);
            info.AddValue("writers", _writers);
            info.AddValue("directors", _directors);
            info.AddValue("language_formats", _audioTracks);
            info.AddValue("genres", _genres);
            info.AddValue("user_star_rating", _userStarRating);
            info.AddValue("aspect_ratio", _aspectRatio);
            info.AddValue("video_standard", _videoStandard);
            info.AddValue("upc", _UPC);
            info.AddValue("original_name", _originalName);
            info.AddValue("tags", _tags);
            info.AddValue("additional_fields", _additionalFields);
            info.AddValue("acting_roles", _actingRoles);
            info.AddValue("nonacting_roles", _nonActingRoles);
            info.AddValue("photos", _photos);
            info.AddValue("trailers", _trailers);
            info.AddValue("children", _children);
            info.AddValue("parent", _parent);
            info.AddValue("sort_name", _sortName);
            info.AddValue("mpaa_rating_reason", _parentalRatingReason);
            info.AddValue("video_details", _videoDetails);
            info.AddValue("subtitles", _subtitles);
            info.AddValue("video_resolution", _videoResolution);
            info.AddValue("extra_features", _extraFeatures);
        }
        #endregion

        /// <summary>
        /// Generic Constructor, inits all the IList items
        /// </summary>
        public Title()
        {
            Utilities.DebugLine("[Title] Creating new Empty Title object");
            _actors = new List<Person>();
            _directors = new List<Person>();
            _writers = new List<Person>();
            _producers = new List<string>();
            _audioTracks = new List<string>();
            _genres = new List<string>();
            _itemId = Utilities.NewRandomNumber();
        }

        /// <summary>
        /// Default destructor
        /// </summary>
        ~Title()
        {
            // Unless this becomes a problem, it just clutters the log
            //Utilities.DebugLine("[Title] Title destroyed");
        }

        ///// <summary>
        ///// Add a Person object to the actors list
        ///// </summary>
        ///// <param name="actor">Person object to add</param>
        //public void AddActor(Person actor)
        //{
        //    if (actor == null) return;
        //    if (!_actors.Contains(actor))
        //        _actors.Add(actor);
        //}

        /// <summary>
        /// Add a Person object to the directors list
        /// </summary>
        /// <param name="director">Person object to add</param>
        public void AddDirector(Person director)
        {
            if (director == null) return;
            _directors.Add(director);
        }
        /// <summary>
        /// Add a Person object to the writers list
        /// </summary>
        /// <param name="writer">Person object to add</param>
        public void AddWriter(Person writer)
        {
            if (writer == null) return;
            _writers.Add(writer);
        }
        /// <summary>
        /// Add a string (person name or company name) to the producers list
        /// </summary>
        /// <param name="producer">string name to add</param>
        public void AddProducer(string producer)
        {
            if (producer == null) return;
            _producers.Add(producer);
        }
        /// <summary>
        /// Add a Genre to the genres list
        /// </summary>
        /// <param name="genre">A Genre from the Genre enum</param>
        public void AddGenre(string genre)
        {
            if (genre == null) return;
            _genres.Add(genre);
        }

        /// <summary>
        /// Add a subtitle
        /// </summary>
        /// <param name="language_format">string name to add</param>
        public void AddSubtitle(string subtitle)
        {
            if (subtitle == null) return;
            _subtitles.Add(subtitle);
        }
        /// <summary>
        /// Add a string language to the language formats list
        /// </summary>
        /// <param name="language_format">string name to add</param>
        public void AddLanguageFormat(string language_format)
        {
            if (language_format == null) return;
            _audioTracks.Add(language_format);
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

        public bool SerializeToXMLFile(string fileName)
        {
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(Title));
                StreamWriter myWriter = new StreamWriter(fileName);
                mySerializer.Serialize(myWriter, this);
                myWriter.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if(reader.MoveToContent() == XmlNodeType.Element && reader.Name == "OMLTitle") 
            {
                while(reader.Read())
                {
                    if(reader.MoveToContent() == XmlNodeType.Element)
                    {
                        if( !reader.IsEmptyElement )
                        {
                            if (reader.Name == "Name")
                            {
                                Name = reader.ReadString();
                            }
                            else if (reader.Name == "SortName")
                            {
                                SortName = reader.ReadString();
                            }
                            else if (reader.Name == "OriginalName")
                            {
                                OriginalName = reader.ReadString();
                            }

                        }
                    }

                    //if (reader.IsStartElement())
                    //{
                    //    if (reader.IsEmptyElement)
                    //        Console.WriteLine("<{0}/>", reader.Name);
                    //}
                    //else
                    //{
                    //    Console.Write("<{0}> ", reader.Name);
                    //    reader.Read(); // Read the start tag.
                    //    if (reader.IsStartElement())  // Handle nested elements.
                    //        Console.Write("\r\n<{0}>", reader.Name);
                    //    Console.WriteLine(reader.ReadString());  //Read the text content of the element.
                    //}
                } 

            } // we have an OMLTitle
        }


        public XmlSchema GetSchema()
        {
            return null;
        }

        static public Title CreateFromXML(string fileName)
        {
            Title t = new Title();

            try
            {
                if (File.Exists(fileName))
                {
                    XPathDocument document = new XPathDocument(fileName);
                    XPathNavigator navigator = document.CreateNavigator();

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

            t.FrontCoverPath = FixupImagePath(t.FrontCoverPath.ToUpper(), fileName.ToUpper(), ".JPG");
            t.BackCoverPath = FixupImagePath(t.BackCoverPath.ToUpper(), fileName.ToUpper(), ".BACK.JPG");

            return t;
        }

        // for paths that don't exist, try to guess
        // replace the folder for the path with the folder where oml.xml file was found
        private static string FixupDiskPath(string path, string omlXmlFile)
        {
            path = path.Trim();
            if (File.Exists(path) || Directory.Exists(path))
                return path;

            string omlxmlFileFolder = Path.GetDirectoryName(omlXmlFile);
            string omlxmlFileStripped = omlXmlFile.Replace(".OML.XML", "");

            if (path.Length > 0)
            {
                string pathFilename = Path.GetFileName(path);

                string fixedPath = omlxmlFileFolder + "\\" + pathFilename;
                if (File.Exists(fixedPath) || Directory.Exists(fixedPath))
                {
                    return fixedPath;
                }
            }

            return path;
        }

        // for paths that don't exist, try to guess
        // replace the folder for the path with the folder where oml.xml file was found
        private static string FixupImagePath(string path, string omlXmlFile, string suffix)
        {
            path = path.Trim();
            if (File.Exists(path)) return path;

            string omlxmlFileFolder = Path.GetDirectoryName(omlXmlFile);
            string omlxmlFileStripped = omlXmlFile.Replace(".OML.XML", "");
            string omlxmlFileStrippedTwice = omlxmlFileFolder + "\\" + Path.GetFileNameWithoutExtension(omlxmlFileStripped);

            // try just the filename + the folder of the oml.xml file
            if (path.Length > 0)
            {
                string pathFilename = Path.GetFileName(path);

                string fixedPath = omlxmlFileFolder + "\\" + pathFilename;
                if (File.Exists(fixedPath))
                {
                    return fixedPath;
                }
            }

            // now try the standard folder.jpg, moviename.jpg, moviename.extension.jpg
            // in the oml.xml file directory. The moviename is everything before .oml.xml
            if (File.Exists(omlxmlFileStripped + suffix))
                return omlxmlFileStripped + suffix;
            else if (File.Exists(omlxmlFileStrippedTwice + suffix))
                return omlxmlFileStrippedTwice + suffix;
            else if (File.Exists(omlxmlFileFolder + "\\folder" + suffix))
                return omlxmlFileFolder + "\\folder" + suffix;


            return path;
        }
        
        // <CustomFields />
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Name", Name);
            writer.WriteElementString("SortName", _sortName);
            writer.WriteElementString("OriginalName", _originalName);

            // the reader should verify that this exists and if doesn't it should
            // just use folder.jpg in the same folder
            writer.WriteElementString("FrontCoverPath", FrontCoverPath);

            writer.WriteElementString("BackCoverPath", BackCoverPath);

            // this will be created when imported
            writer.WriteElementString("FrontCoverMenuPath", FrontCoverMenuPath);

            // should be relative path so it can be loaded on any computer
            writer.WriteStartElement("Disks");
            foreach (Disk disk in Disks)
            {
                writer.WriteStartElement("Disk");
                writer.WriteAttributeString("Name", disk.Name);

                // the xml file should be in the same dir as the movie
                writer.WriteElementString("Path",disk.Path);
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
            foreach (KeyValuePair<string,string> actingRole in ActingRoles)
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
            foreach (KeyValuePair<string,string> field in AdditionalFields)
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
            _name = CopyStringValue(t._name, _name, overWrite);
            _metadataSourceId = CopyStringValue(t._metadataSourceId, _metadataSourceId, overWrite);

            _parentalRating = CopyStringValue(t._parentalRating, _parentalRating, overWrite);
            _synopsis = CopyStringValue(t._synopsis, _synopsis, overWrite);
            _studio = CopyStringValue(t._studio, _studio, overWrite);
            _countryOfOrigin = CopyStringValue(t._countryOfOrigin, _countryOfOrigin, overWrite);
            _officialWebsiteURL = CopyStringValue(t._officialWebsiteURL, _officialWebsiteURL, overWrite);
            _aspectRatio = CopyStringValue(t._aspectRatio, _aspectRatio, overWrite);
            _videoStandard = CopyStringValue(t._videoStandard, _videoStandard, overWrite);
            _UPC = CopyStringValue(t._UPC, _UPC, overWrite);
            _originalName = CopyStringValue(t._originalName, _originalName, overWrite);
            _sortName = CopyStringValue(t._sortName, _sortName, overWrite);
            _parentalRatingReason = CopyStringValue(t._parentalRatingReason, _parentalRatingReason, overWrite);
            _videoDetails = CopyStringValue(t._videoDetails, _videoDetails, overWrite);
            
            if ( t.Runtime > 0) Runtime = t.Runtime;
            if (t.ReleaseDate != null) ReleaseDate = t.ReleaseDate;
            if (t.UserStarRating > 0) UserStarRating = t.UserStarRating;

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

            if (t._additionalFields != null && t._additionalFields.Count > 0)
            {
                if (_additionalFields == null) _additionalFields = new Dictionary<string, string>();
                if (overWrite || _additionalFields.Count == 0)
                {

                    _additionalFields.Clear();
                    foreach (KeyValuePair<string, string> p in t._additionalFields)
                    {
                        _additionalFields.Add(p.Key, p.Value);
                    }
                }
            }

            if (t._photos != null && t._photos.Count > 0)
            {
                if (_photos == null) _photos = new List<string>();
                if (overWrite || _photos.Count == 0)
                {

                    _photos.Clear();
                    foreach (string p in t._photos)
                    {
                        _photos.Add(p);
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
                if (overWrite || String.IsNullOrEmpty(FrontCoverPath))
                {
                    CopyFrontCoverFromFile(t.FrontCoverPath, true);
                }
            }
        
        }

        // copy front cover image and set the menu cover too (resized version)
        public bool CopyFrontCoverFromFile(string source, bool deleteSource)
        {
            try
            {
                File.Copy(source, GetDefaultFrontCoverName());
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

        public bool CopyBackCoverFromFile(string source, bool deleteSource)
        {
            try
            {
                File.Copy(source, GetDefaultBackCoverName());
                if (deleteSource) File.Delete(source);
                BackCoverPath = GetDefaultBackCoverName();
                
                return true;
            }
            catch
            {
                return false;
            }
        }        
        
        public string GetDefaultFrontCoverName()
        {
            return OMLEngine.FileSystemWalker.ImageDirectory + "\\F" + InternalItemID + ".jpg";
        }

        public string GetDefaultBackCoverName()
        {
            return OMLEngine.FileSystemWalker.ImageDirectory + "\\B" + InternalItemID + ".jpg";
        }

        public string GetDefaultFrontCoverMenuName()
        {
            return OMLEngine.FileSystemWalker.ImageDirectory + "\\MF" + InternalItemID + ".jpg";
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
            return "Title:" + this._name + " (" + this._itemId + ")";
        }
    }
}
