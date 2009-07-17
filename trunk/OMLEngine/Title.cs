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
using System.Linq;
using Dao = OMLEngine.Dao;

namespace OMLEngine
{
    [Serializable()]
    [XmlRootAttribute("OMLTitle", Namespace = "http://www.openmedialibrary.org/", IsNullable = false)]
    public class Title : IComparable, ISerializable
    {
        #region locals
        private static string XmlNameSpace = "http://www.openmedialibrary.org/";
        //private bool _needsTranscode = false;        

        private Disk _selectedDisk = null;
        private Dao.Title _title;
        private bool _peopleProcesed = false;
        private string _frontCoverMenuPath = null;
        private string _frontCoverPath = null;
        private string _backCoverPath = null;
        private Title _parentTitle = null;

        private List<Disk> _disks = null;
        private List<string> _audioTracks = null;
        private List<string> _subtitles = null;
        private List<string> _trailers = null;

        private List<string> _extraFeatures = new List<string>();

        #endregion

        #region properties

        #region Unknown Properties

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

        public void ReloadTitle()
        {
            Dao.DBContext.Instance.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, _title);
           // _title.Disks.Clear();
           // _disks = null;
           // Dao.DBContext.Instance.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, _title);

            DaoTitle.UpdatedActors = null;
            DaoTitle.UpdatedBackCoverPath = null;
            DaoTitle.UpdatedDirectors = null;
            DaoTitle.UpdatedFanArtPaths = null;
            DaoTitle.UpdatedFrontCoverPath = null;
            DaoTitle.UpdatedGenres = null;
            DaoTitle.UpdatedNonActingRoles = null;
            DaoTitle.UpdatedProducers = null;
            DaoTitle.UpdatedTags = null;
            DaoTitle.UpdatedWriters = null;

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

        public int ProductionYear
        {
            get
            {
                if (_title.ProductionYear == null)
                {
                    return 0;
                }
                else
                {
                    return (int)_title.ProductionYear;
                }
            }
            set
            {
                _title.ProductionYear = value;
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
                return _title.SortName;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) 
                {
                    _title.SortName = _title.Name;
                    return;
                }
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
                if (DaoTitle.UpdatedTags == null)
                {
                    DaoTitle.UpdatedTags = new List<string>(_title.Tags.Count);

                    foreach (Dao.Tag tag in _title.Tags)
                        DaoTitle.UpdatedTags.Add(tag.Name);
                }

                return DaoTitle.UpdatedTags.AsReadOnly();
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
                _title.UserRating = (value == null)
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
                    return (VideoFormat)_title.Disks[0].VideoFormat;
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

        /// <summary>
        /// Internal id of the Title
        /// </summary>
        public int Id
        {
            get { return _title.Id; }
            private set { _title.Id = value; }
        }

        public int? ParentTitleId
        {
            get { return _title.ParentTitleId; }
            set { _title.ParentTitleId = value; }
        }

        public Title ParentTitle
        {
            get
            {
                if (_parentTitle != null)
                    return _parentTitle;

                if (ParentTitleId != null)
                {
                    _parentTitle = TitleCollectionManager.GetTitle((int)ParentTitleId);
                    return _parentTitle;
                }
                return null;
            }
            set
            {
                _parentTitle = value;
                ParentTitleId = value.Id;
            }
        }


        public TitleTypes TitleType
        {
            get { return (TitleTypes)_title.TitleType; }
            set { _title.TitleType = (int)value; }
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

        public DateTime? ModifiedDate
        {
            get { return _title.ModifiedDate; }
            set { _title.ModifiedDate = value; }
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
                if (_title.UpdatedFrontCoverPath != null)
                    return _title.UpdatedFrontCoverPath;

                if (_frontCoverPath == null)
                {
                    Dao.ImageMapping frontCover = _title.Images.FirstOrDefault(i => i.ImageType == (byte)ImageType.FrontCoverImage);
                    _frontCoverPath = ImageManager.GetImagePathById((frontCover == null) ? (int?)null : frontCover.ImageId, ImageSize.Original);
                }

                return _frontCoverPath;
            }
            set
            {
                if (value.Length > 255)
                    throw new FormatException("FrontCoverPath must be 255 characters or less.");

                if ((value == "") || (File.Exists(value)))
                {
                    _title.UpdatedFrontCoverPath = value;
                    _frontCoverPath = null;
                    DaoTitle.ModifiedDate = DateTime.Now;
                }
            }
        }


        #region Async Menu Cover Art

        public delegate void DelegateFrontCoverPath(Title title);        
        private static BackgroundProcessor<Title> imageProcessor;
        private static object threadLocker = new object();
        private object pathUpdateLocker = new object();
        private DelegateFrontCoverPath callback;        

        private static void ProcessorCallback(Title title)
        {
            if (title._frontCoverMenuPath == null)
            {
                lock (title.pathUpdateLocker)
                {
                    if (title._frontCoverMenuPath == null)
                    {
                        title.GetFrontCoverMenuPathSlow();
                    }
                }
            }

            if (title.callback != null)
                title.callback.Invoke(title);
        }        

        public void BeginGetFrontCoverMenuPath(DelegateFrontCoverPath callback)
        {
            if (imageProcessor == null)
            {
                lock (threadLocker)
                {
                    if (imageProcessor == null)
                    {
                        imageProcessor = new BackgroundProcessor<Title>(4, Title.ProcessorCallback, "ImageProcessor");
                    }
                }
            }

            imageProcessor.Enqueue(this);
            this.callback += callback;            
        }        

        #endregion


        /// <summary>
        /// Will returns the front cover menu path and roundtrip to the database if need be.
        /// Thread Safe.
        /// </summary>
        /// <returns></returns>
        public string GetFrontCoverMenuPathSlow()
        {
            if (_frontCoverMenuPath == null)
            {                                
                int? frontCoverId = ImageManager.GetImageIdForTitleThreadSafe(this.Id, ImageType.FrontCoverImage);
                _frontCoverMenuPath = ImageManager.GetImagePathById(frontCoverId, ImageSize.Small);                             
            }

            return _frontCoverMenuPath;
        }               

        /// <summary>
        /// The small version of the front cover image.  Will be NULL if the image hasn't been retrieved from the database yet.
        /// 
        /// </summary>
        public string FrontCoverMenuPath
        {
            get 
            {
                if (_frontCoverMenuPath == null)
                {
                    Dao.ImageMapping frontCover = _title.Images.FirstOrDefault(i => i.ImageType == (byte)ImageType.FrontCoverImage);
                    string path = ImageManager.ConstructImagePathById((frontCover == null) ? (int?)null : frontCover.ImageId, ImageSize.Small);

                    if (File.Exists(path))
                        _frontCoverMenuPath = path;
                }

                return _frontCoverMenuPath; 
            }
        }

        /// <summary>
        /// Full path to the rear cover art image
        /// </summary>
        public string BackCoverPath
        {
            get
            {
                if (_title.UpdatedBackCoverPath != null)
                    return _title.UpdatedBackCoverPath;

                if (_backCoverPath == null)
                {
                    Dao.ImageMapping backCover = _title.Images.FirstOrDefault(i => i.ImageType == (byte)ImageType.BackCoverImage);
                    _backCoverPath = ImageManager.GetImagePathById((backCover == null) ? (int?)null : backCover.ImageId, ImageSize.Original);
                }

                return _backCoverPath;
            }
            set
            {
                if (value.Length > 255)
                    throw new FormatException("BackCoverPath must be 255 characters or less.");

                if ((value == "") || (File.Exists(value)))
                {
                    _title.UpdatedBackCoverPath = value;
                    _backCoverPath = null;
                    DaoTitle.ModifiedDate = DateTime.Now;
                }
            }
        }

        public IList<string> FanArtPaths
        {
            get
            {
                if (DaoTitle.UpdatedFanArtPaths == null)
                {
                    DaoTitle.UpdatedFanArtPaths = new List<string>();

                    foreach (Dao.ImageMapping mapping in _title.Images.Where(t => t.ImageType == (byte)ImageType.FanartImage))
                    {
                        DaoTitle.UpdatedFanArtPaths.Add(ImageManager.GetImagePathById(mapping.ImageId, ImageSize.Original));
                    }
                }

                return DaoTitle.UpdatedFanArtPaths.AsReadOnly();
            }
        }

        /// <summary>
        /// Runtime in minutes of the title
        /// </summary>
        public int Runtime
        {
            get { return _title.Runtime ?? 0; }
            set { _title.Runtime = (short)value; }
        }

        /// <summary>
        /// Rating of the film 
        /// </summary>
        public string ParentalRating
        {
            get { return _title.ParentalRating ?? string.Empty; }
            set
            {
                if (value.Length > 80)
                    throw new FormatException("ParentalRating must be 80 characters or less.");
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
                if (DaoTitle.UpdatedGenres == null)
                {
                    DaoTitle.UpdatedGenres = new List<string>(_title.Genres.Count);
                    foreach (Dao.Genre genre in _title.Genres)
                        DaoTitle.UpdatedGenres.Add(genre.MetaData.Name);
                }

                return DaoTitle.UpdatedGenres.AsReadOnly();
            }
        }

        public IList<Role> NonActingRoles
        {
            get
            {
                SetupPeopleCollections();

                return DaoTitle.UpdatedNonActingRoles.AsReadOnly();
            }
        }

        public IList<Role> ActingRoles
        {
            get
            {
                SetupPeopleCollections();

                return DaoTitle.UpdatedActors.AsReadOnly();
            }
        }

        /// <summary>
        /// List of Person objects that directed the title (usually one Person)
        /// </summary>
        public IList<Person> Directors
        {
            get
            {
                SetupPeopleCollections();

                return DaoTitle.UpdatedDirectors.AsReadOnly();
            }
        }
        /// <summary>
        /// List of Person objects that wrote the title
        /// </summary>
        public IList<Person> Writers
        {
            get
            {
                SetupPeopleCollections();

                return DaoTitle.UpdatedWriters.AsReadOnly();
            }
        }

        /// <summary>
        /// List of people/companies that produced the title
        /// </summary>
        public IList<Person> Producers
        {
            get
            {
                SetupPeopleCollections();

                return DaoTitle.UpdatedProducers.AsReadOnly();
            }
        }

        public decimal PercentComplete
        {
            get { return _title.PercentComplete; }
        }

        #endregion

        private void SetupPeopleCollections()
        {
            if (_peopleProcesed)
                return;

            DaoTitle.UpdatedActors = new List<Role>();
            DaoTitle.UpdatedDirectors = new List<Person>();
            DaoTitle.UpdatedWriters = new List<Person>();
            DaoTitle.UpdatedProducers = new List<Person>();
            DaoTitle.UpdatedNonActingRoles = new List<Role>();

            foreach (Dao.Person person in _title.People)
            {
                switch ((PeopleRole)person.Role)
                {
                    case PeopleRole.Actor:
                        DaoTitle.UpdatedActors.Add(new Role(person.MetaData.FullName, person.CharacterName));
                        break;

                    case PeopleRole.Director:
                        DaoTitle.UpdatedDirectors.Add(new Person(person.MetaData.FullName));
                        break;

                    case PeopleRole.Producers:
                        DaoTitle.UpdatedProducers.Add(new Person(person.MetaData.FullName));
                        break;

                    case PeopleRole.Writer:
                        DaoTitle.UpdatedWriters.Add(new Person(person.MetaData.FullName));
                        break;

                    case PeopleRole.NonActing:
                        DaoTitle.UpdatedNonActingRoles.Add(new Role(person.MetaData.FullName, person.CharacterName));
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

            DaoTitle.ModifiedDate = DateTime.Now;
            // todo : solomon : this went wonky here 
            /*
            info.AddValue("backdrop_boxart_path", _backDropImage);
            info.AddValue("production_year", _productionYear);
            info.AddValue("fan_art_folder", _fanartfolder);
             */
        }

        /// <summary>
        /// Constructor to base a title object off a db title object
        /// </summary>
        /// <param name="title"></param>
        internal Title(OMLEngine.Dao.Title title)
        {
            _title = title;
        }

        #region People Management
        public void AddActingRole(string actor, string role)
        {
            SetupPeopleCollections();

            if (!DaoTitle.UpdatedActors.Exists(p => p.PersonName.Equals(actor, StringComparison.OrdinalIgnoreCase)))
            {
                DaoTitle.UpdatedActors.Add(new Role(actor, role));
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveActingRole(string actor)
        {
            SetupPeopleCollections();

            Role role = DaoTitle.UpdatedActors.Find(p => p.PersonName == actor);

            if (role != null)
            {
                DaoTitle.UpdatedActors.Remove(role);
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveAllActingRoles()
        {
            DaoTitle.UpdatedActors = new List<Role>();
            DaoTitle.ModifiedDate = DateTime.Now;
        }

        public void AddNonActingRole(string name, string role)
        {
            SetupPeopleCollections();

            if (!DaoTitle.UpdatedNonActingRoles.Exists(p => p.PersonName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                DaoTitle.UpdatedActors.Add(new Role(name, role));
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveNonActingRole(string actor)
        {
            SetupPeopleCollections();

            Role role = DaoTitle.UpdatedNonActingRoles.Find(p => p.PersonName == actor);

            if (role != null)
            {
                DaoTitle.UpdatedNonActingRoles.Remove(role);
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveAllNonActingRoles()
        {
            DaoTitle.UpdatedNonActingRoles = new List<Role>();
            DaoTitle.ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Add a Person object to the directors list
        /// </summary>
        /// <param name="director">Person object to add</param>
        public void AddDirector(Person director)
        {
            SetupPeopleCollections();

            if (!(DaoTitle.UpdatedDirectors.Exists(p => p.full_name.Equals(director.full_name, StringComparison.OrdinalIgnoreCase))))
            {
                DaoTitle.UpdatedDirectors.Add(director);
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveDirector(string name)
        {
            SetupPeopleCollections();

            Person person = DaoTitle.UpdatedDirectors.Find(p => p.full_name == name);

            if (person != null)
            {
                DaoTitle.UpdatedDirectors.Remove(person);
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveAllDirectors()
        {
            DaoTitle.UpdatedDirectors = new List<Person>();
            DaoTitle.ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Add a Person object to the writers list
        /// </summary>
        /// <param name="writer">Person object to add</param>
        public void AddWriter(Person writer)
        {
            SetupPeopleCollections();

            if (!(DaoTitle.UpdatedWriters.Exists(p => p.full_name.Equals(writer.full_name, StringComparison.OrdinalIgnoreCase))))
            {
                DaoTitle.UpdatedWriters.Add(writer);
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveWriter(string name)
        {
            SetupPeopleCollections();

            Person person = DaoTitle.UpdatedWriters.Find(p => p.full_name == name);

            if (person != null)
            {
                DaoTitle.UpdatedWriters.Remove(person);
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveAllWriters()
        {
            DaoTitle.UpdatedWriters = new List<Person>();
            DaoTitle.ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Add a string (person name or company name) to the producers list
        /// </summary>
        /// <param name="producer">string name to add</param>
        public void AddProducer(Person producer)
        {
            SetupPeopleCollections();

            if (!(DaoTitle.UpdatedProducers.Exists(p => p.full_name.Equals(producer.full_name, StringComparison.OrdinalIgnoreCase))))
            {
                DaoTitle.UpdatedProducers.Add(producer);
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveProducer(string name)
        {
            SetupPeopleCollections();

            Person person = DaoTitle.UpdatedProducers.Find(p => p.full_name == name);

            if (person != null)
            {
                DaoTitle.UpdatedProducers.Remove(person);
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        public void RemoveAllProducers()
        {
            DaoTitle.UpdatedProducers = new List<Person>();
            DaoTitle.ModifiedDate = DateTime.Now;
        }
        #endregion

        /// <summary>
        /// Adds a fanart image to the title
        /// </summary>
        /// <param name="path"></param>
        public void AddFanArtImage(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (FanArtPaths.Contains(path))
                return;

            if (File.Exists(path))
            {
                DaoTitle.UpdatedFanArtPaths.Add(path);
                DaoTitle.ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Removes a fanart image from the title
        /// </summary>
        /// <param name="path"></param>
        public void RemoveFanArtImage(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!FanArtPaths.Contains(path))
                return;

            DaoTitle.UpdatedFanArtPaths.Remove(path);
            DaoTitle.ModifiedDate = DateTime.Now;
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
            _title.Subtitles = subtitles;

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

            // add the genre to the local collection
            DaoTitle.UpdatedGenres.Add(genre);
            DaoTitle.ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Removes a genre from the title
        /// </summary>
        /// <param name="genre"></param>
        public void RemoveGenre(string genre)
        {
            if (string.IsNullOrEmpty(genre))
                return;

            if (!Genres.Contains(genre))
                return;

            DaoTitle.UpdatedGenres.Remove(genre);
            DaoTitle.ModifiedDate = DateTime.Now;
        }

        /// <summary>
        /// Removes all the genres from the title
        /// </summary>
        public void RemoveAllGenres()
        {
            DaoTitle.UpdatedGenres = new List<string>();
            DaoTitle.ModifiedDate = DateTime.Now;
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

        public void RemoveDisk(Disk disk)
        {
            if (disk == null)
                return;

            if (!Disks.Contains(disk))
                return;

            _title.Disks.Remove(disk.DaoDisk);

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

            DaoTitle.UpdatedTags.Add(tag);
            DaoTitle.ModifiedDate = DateTime.Now;
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

            DaoTitle.UpdatedTags.Remove(tag);
            DaoTitle.ModifiedDate = DateTime.Now;
        }

        public void RemoveAllTags()
        {
            DaoTitle.UpdatedTags = new List<string>();
            DaoTitle.ModifiedDate = DateTime.Now;
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

        static public Title CreateFromXML(string fileName)
        {
            //return new Title();
            
            Title t = new Title();

            t.DateAdded = DateTime.Now;

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

                        
                        /*if (navigator.MoveToChild("FrontCoverMenuPath", XmlNameSpace))
                        {
                            if (!String.IsNullOrEmpty(navigator.Value)) t.FrontCoverMenuPath = navigator.Value;
                            navigator.MoveToParent();
                        }*/

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

                                        t.AddDisk(new Disk(diskName, path, format));

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
                                    if (releaseDate >= DateTime.Parse("1 Jan 1900"))
                                    {
                                        t.ReleaseDate = releaseDate;
                                    }
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
                                        t.FanArtPaths.Add(navigator.Value);
                                        //t.Photos.Add(navigator.Value);
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
                                        Person producer = new Person();
                                        producer.full_name = navigator.Value;
                                        t.AddProducer(producer);
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

                        /*if (navigator.MoveToChild("CustomFields", XmlNameSpace))
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
                        }*/
                    }
                }
            }
            catch (Exception ex)
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
            /*t.FrontCoverPath = FixupImagePath(t.FrontCoverPath.ToUpper(), fileName.ToUpper(), ".JPG");
            t.BackCoverPath = FixupImagePath(t.BackCoverPath.ToUpper(), fileName.ToUpper(), ".BACK.JPG");

            if (copyImages)
            {
                t.CopyFrontCoverFromFile(t.FrontCoverPath, false);
                t.CopyBackCoverFromFile(t.BackCoverPath, false);
            }*/

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

        //<?xml version="1.0" encoding="utf-8"?>
        //<OMLTitle xmlns="http://www.openmedialibrary.org/">

        public bool SerializeToXMLFile(string fileName)
        {
            fileName = fileName.ToUpper();
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
                writer.WriteElementString("SortName", this.SortName);
                writer.WriteElementString("OriginalName", this.OriginalName);

                /*string newFrontCoverPath = fileName.Replace(".OML.XML", ".JPG");
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
                */

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
                //writer.WriteElementString("TranscodeToExtender", _needsTranscode.ToString());

                writer.WriteElementString("Synopsis", this.Synopsis);
                writer.WriteElementString("Studio", this.Studio);
                writer.WriteElementString("Country", this.CountryOfOrigin);
                writer.WriteElementString("OfficialWebSiteURL", this.OfficialWebsiteURL);

                writer.WriteElementString("Runtime", this.Runtime.ToString());
                writer.WriteElementString("ParentalRating", this.ParentalRating);
                writer.WriteElementString("ParentalRatingReason", this.ParentalRatingReason);
                writer.WriteElementString("ReleaseDate", this.ReleaseDate.ToString());
                writer.WriteElementString("ProductionYear", this.ProductionYear.ToString());



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


                writer.WriteElementString("UserRating", this.UserStarRating.ToString());
                writer.WriteElementString("AspectRatio", this.AspectRatio);
                writer.WriteElementString("VideoStandard", this.VideoStandard);


                writer.WriteStartElement("ActingRoles");
                foreach (Role actingRole in ActingRoles)
                {
                    writer.WriteStartElement("Actor");
                    writer.WriteAttributeString("Name", actingRole.PersonName);
                    writer.WriteAttributeString("Role", actingRole.RoleName);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("NonActingRoles");
                foreach (Role nonactingRole in NonActingRoles)
                {
                    writer.WriteStartElement("Person");
                    writer.WriteAttributeString("Name", nonactingRole.PersonName);
                    writer.WriteAttributeString("Role", nonactingRole.RoleName);
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


                /*writer.WriteStartElement("CustomFields");
                foreach (KeyValuePair<string, string> field in AdditionalFields)
                {
                    writer.WriteStartElement("CustomField");
                    writer.WriteAttributeString("Name", field.Key);
                    writer.WriteAttributeString("Value", field.Key);
                }
                writer.WriteEndElement();*/


                /*writer.WriteStartElement("Photos");
                foreach (string photo in Photos)
                {
                    writer.WriteElementString("Photo", photo);
                }
                writer.WriteEndElement();*/

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


                writer.WriteElementString("VideoDetails", this.VideoDetails);

                writer.WriteElementString("VideoResolution", this.VideoResolution);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string CopyStringValue(string src, string dest, bool overWrite)
        {
            if (overWrite)
            {
                // only copy data if available
                if (!String.IsNullOrEmpty(src))
                    return src;
                else
                    if (!String.IsNullOrEmpty(dest))
                        return dest;
                    else return "";
            }
            else
            {
                if (!String.IsNullOrEmpty(src) && String.IsNullOrEmpty(dest))
                    return src;
                else
                    return dest ?? "";
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
            ReleaseDate = CheckDateRange(t.ReleaseDate);

            if (t.Runtime > 0) Runtime = t.Runtime;
            if (t.UserStarRating > 0) UserStarRating = t.UserStarRating;
            if (t.ProductionYear > 0) ProductionYear = t.ProductionYear;

            if (t.Directors != null && t.Directors.Count > 0)
            {
                if (overWrite || Directors.Count == 0)
                {
                    RemoveAllDirectors();
                    foreach (Person p in t.Directors)
                    {
                        AddDirector(new Person(p.full_name));
                    }
                }
            }


            if (t.Writers != null && t.Writers.Count > 0)
            {
                if (overWrite || Writers.Count == 0)
                {
                    RemoveAllWriters();
                    foreach (Person p in t.Writers)
                    {
                        AddWriter(new Person(p.full_name));
                    }
                }
            }

            if (t.Producers != null && t.Producers.Count > 0)
            {
                if (overWrite || Producers.Count == 0)
                {
                    RemoveAllProducers();
                    foreach (Person p in t.Producers)
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

            if (t.Genres != null && t.Genres.Count > 0)
            {
                if (overWrite || Genres.Count == 0)
                {
                    RemoveAllGenres();
                    foreach (string p in t.Genres)
                    {
                        AddGenre(p);
                    }
                }
            }

            if (t.Tags != null && t.Tags.Count > 0)
            {
                if (null == DaoTitle.UpdatedTags) DaoTitle.UpdatedTags = new List<string>();

                if (overWrite || DaoTitle.UpdatedTags.Count == 0)
                {
                    RemoveAllTags();
                    foreach (string p in t.DaoTitle.UpdatedTags)
                    {
                        DaoTitle.UpdatedTags.Add(p);
                    }
                }
            }

            if (t.ActingRoles != null && t.ActingRoles.Count > 0)
            {
                if (overWrite || ActingRoles.Count == 0)
                {
                    RemoveAllActingRoles();
                    foreach (Role p in t.ActingRoles)
                    {
                        AddActingRole(p.PersonName, p.RoleName);
                    }
                }
            }

            if (t.NonActingRoles != null && t.NonActingRoles.Count > 0)
            {
                if (overWrite || NonActingRoles.Count == 0)
                {
                    RemoveAllNonActingRoles();
                    foreach (Role p in t.NonActingRoles)
                    {
                        AddNonActingRole(p.PersonName, p.RoleName);
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

            if (!String.IsNullOrEmpty(t.DaoTitle.UpdatedFrontCoverPath))
            {
                if (overWrite || String.IsNullOrEmpty(FrontCoverPath) || FrontCoverPath == ImageManager.NoCoverPath)
                {
                    FrontCoverPath = t.DaoTitle.UpdatedFrontCoverPath;
                }
            }

            if (!String.IsNullOrEmpty(t.DaoTitle.UpdatedBackCoverPath))
            {
                if (overWrite || String.IsNullOrEmpty(BackCoverPath) || BackCoverPath == ImageManager.NoCoverPath)
                {
                    BackCoverPath = t.DaoTitle.UpdatedBackCoverPath;
                }
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

        private T GetSerializedList<T>(SerializationInfo info, string id) where T : new()
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
            Name = GetSerializedString(info, "name");
            //Id = GetSerializedInt( info,"itemid");
            MetadataSourceID = GetSerializedString(info, "sourceid");
            MetadataSourceName = GetSerializedString(info, "sourcename");
            FrontCoverPath = GetSerializedString(info, "front_boxart_path");
            //FrontCoverMenuPath = GetSerializedString(info, "front_boxart_menu_path");
            BackCoverPath = GetSerializedString(info, "back_boxart_path");
            Synopsis = GetSerializedString(info, "synopsis");
            Studio = GetSerializedString(info, "distributor");
            CountryOfOrigin = GetSerializedString(info, "country_of_origin");
            OfficialWebsiteURL = GetSerializedString(info, "official_website_url");
            DateAdded = CheckDateRange(GetSerializedDateTime(info, "date_added"));
            ImporterSource = GetSerializedString(info, "importer_source");
            Runtime = GetSerializedInt(info, "runtime");
            ParentalRating = GetSerializedString(info, "mpaa_rating");


            ReleaseDate = CheckDateRange(GetSerializedDateTime(info, "release_date"));

            DaoTitle.UpdatedProducers = new List<Person>();
            List<string> producers = GetSerializedList<List<string>>(info, "producers");
            producers.ForEach(p => DaoTitle.UpdatedProducers.Add(new Person(p)));

            DaoTitle.UpdatedWriters = GetSerializedList<List<Person>>(info, "writers");

            DaoTitle.UpdatedDirectors = GetSerializedList<List<Person>>(info, "directors");

            _audioTracks = GetSerializedList<List<string>>(info, "language_formats");
            _title.AudioTracks = Dao.TitleDao.GetDelimitedStringFromCollection(_audioTracks);
            DaoTitle.UpdatedGenres = GetSerializedList<List<string>>(info, "genres");

            UserStarRating = GetSerializedInt(info, "user_star_rating");
            AspectRatio = GetSerializedString(info, "aspect_ratio");
            VideoStandard = GetSerializedString(info, "video_standard");
            UPC = GetSerializedString(info, "upc");
            OriginalName = GetSerializedString(info, "original_name");
            DaoTitle.UpdatedTags = GetSerializedList<List<string>>(info, "tags");

            Dictionary<string, string> actors = GetSerializedList<Dictionary<string, string>>(info, "acting_roles");

            DaoTitle.UpdatedActors = new List<Role>();
            foreach (string actor in actors.Keys)
                DaoTitle.UpdatedActors.Add(new Role(actor, actors[actor]));

            Dictionary<string, string> nonActingRoles = GetSerializedList<Dictionary<string, string>>(info, "nonacting_roles");

            DaoTitle.UpdatedNonActingRoles = new List<Role>();
            foreach (string key in nonActingRoles.Keys)
                DaoTitle.UpdatedNonActingRoles.Add(new Role(key, nonActingRoles[key]));

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

        public DateTime CheckDateRange(DateTime dt)
        {
            if ((dt >= DateTime.Parse("1 Jan 1900")) && (dt<= DateTime.Parse("6 June 2079")))
            {
                return dt;
            }
            return DateTime.Parse("1 Jan 1900");
        }

        void CleanDuplicateDisks()
        {
            foreach (Disk d in new List<Disk>(this._disks))
            {
                int i = _disks.IndexOf(d);
                if (i < 0)
                    continue;
                while (true)
                {
                    int oi = _disks.IndexOf(d, i + 1);
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
