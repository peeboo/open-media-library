using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OMLSDK
{
    public class OMLSDKTitle
    {

/*using System;
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
using System.Drawing;
using System.Linq;
using Dao = OMLEngine.Dao;

namespace OMLEngine
{
    [Serializable()]
    [XmlRootAttribute("OMLTitle", Namespace = "http://www.openmedialibrary.org/", IsNullable = false)]
    public class Title : IComparable, ISerializable
    {
        // Database field sizes
        const int NameLength = 255;
        const int SortNameLength = 255;
        const int MetadataSourceLength = 200;
        const int ParentalRatingLength = 80;
        const int StudioLength = 255;
        const int CountryOfOriginLength = 255;
        const int WebsiteUrlLength = 255;
        const int AudioTracksLength = 255;
        const int AspectRatioLength = 10;
        const int VideoStandardLength = 10;
        const int UPCLength = 100;
        const int TrailersLength = 255;
        const int ParentalRatingReasonLength = 255;
        const int SubtitlesLength = 255;
        const int VideoResolutionLength = 20;
        const int OriginalNameLength = 255;
        const int ImporterSourceLength = 255;
        const int MetaDataSourceItemIdLength = 255;

        private static string XmlNameSpace = "http://www.openmedialibrary.org/";
        //private bool _needsTranscode = false; */       

        #region locals
        //private Disk _selectedDisk = null;
        //private Dao.Title _title;
        private bool _peopleProcesed = false;
        private string _frontCoverMenuPath = null;
        private string _frontCoverPath = null;
        private string _backCoverPath = null;
        //private Title _parentTitle = null;

        //private List<Disk> _disks = null;
        private List<string> _audioTracks = null;
        private List<string> _subtitles = null;
        private List<string> _trailers = null;

        private List<string> _extraFeatures = new List<string>();

        #endregion

        #region properties

        #region Unknown Properties


        public OMLSDKTitle()
        {
            _NonActingRoles = new List<OMLSDKRole>();
            _ActingRoles = new List<OMLSDKRole>();
            _Directors = new List<OMLSDKPerson>();
            _Writers = new List<OMLSDKPerson>();
            _Producers = new List<OMLSDKPerson>();

            _Genres = new List<string>();
        }


        //public Disk SelectedDisk
        //{
        //    get { return _selectedDisk; }
        //    set { _selectedDisk = value; }
        //}

        /// <summary>
        /// Does this still make sense ?
        /// </summary>
        public List<string> ExtraFeatures
        {
            get { return _extraFeatures; }
            set { _extraFeatures = value; }
        }

        #endregion

        public string VideoResolution { get; set; }

        public int ProductionYear { get; set; }

        public string VideoDetails { get; set; }

        public short? SeasonNumber { get; set; }

        public short? EpisodeNumber { get; set; }

        public string ParentalRatingReason { get; set; }

        public string SortName { get; set; }

        public string OriginalName { get; set; }

        /// <summary>
        /// Gets or sets the video standard (NTSC, PAL).
        /// </summary>
        /// <value>The video standard.</value>
        public string VideoStandard { get; set; }

        public string AspectRatio { get; set; }

        public string UPC { get; set; }

        /// <summary>
        /// Gets or sets the user star rating (0 to 100) - null will make the item unrated
        /// </summary>
        /// <value>The user star rating.</value>
        /*public int? UserStarRating
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
        }*/

        /// <summary>
        /// Video format of title (DVD, AVI, etc)
        /// </summary>
        //public VideoFormat VideoFormat { get; set; }

        public string Name { get; set; }

 /*        public int Id
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
        }*/


        /*public TitleTypes TitleType
        {
            get { return (TitleTypes)_title.TitleType; }
            set { _title.TitleType = (int)value; }
        }*/


        public string MetadataSourceName { get; set; }

        public string MetadataSourceID { get; set; }

/*        public string ImporterSource
        {
            get { return _title.ImporterSource; }
            set
            {
                if (value.Length > ImporterSourceLength)
                    throw new FormatException("ImporterSource must be " + ImporterSourceLength.ToString() + " characters or less.");
                _title.ImporterSource = value;
            }
        }
        public string ImporterSourceTrimmed
        {
            set
            {
                ImporterSource = value.Substring(0, Math.Min(value.Length, ImporterSourceLength));
            }
        }*/


        public string FrontCoverPath { get; set; }

        /*public string FrontCoverMenuPath { get; set; }
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
        }*/


        public string BackCoverPath { get; set; }

        public IList<string> FanArtPaths { get; set; }

        public int Runtime { get; set; }

        public string ParentalRating { get; set; }

        public string Synopsis { get; set; }

        public string Studio { get; set; }

        public string CountryOfOrigin { get; set; }

        public string OfficialWebsiteURL { get; set; }

        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// List of languages (English, Spanish, French, DTS, DD5.1, DD2.0, etc)
        /// </summary>
        /*public IList<string> AudioTracks
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
        }*/

        private IList<string> _Genres;
        public IList<string> Genres
        {
            get
            {
                return _Genres;
            }
        }
        public void AddGenre(string genre)
        {
            if (genre.Length > 255)
                throw new FormatException("Genre must be 255 characters or less.");
            if (string.IsNullOrEmpty(genre))
                return;

            if (_Genres.Contains(genre))
                return;

            // add the genre to the local collection
            _Genres.Add(genre);
        }


        private List<OMLSDKRole> _NonActingRoles;
        public List<OMLSDKRole> NonActingRoles
        {
            get
            {
                return _NonActingRoles;
            }
        }
        public void AddNonActingRole(string name, string role)
        {
            if (!_NonActingRoles.Exists(p => p.PersonName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                _NonActingRoles.Add(new OMLSDKRole(name, role));
            }
        }


        private List<OMLSDKRole> _ActingRoles;
        public List<OMLSDKRole> ActingRoles
        {
            get
            {
                return _ActingRoles;
            }
        }
        public void AddActingRole(string actor, string role)
        {
            if (!_ActingRoles.Exists(p => p.PersonName.Equals(actor, StringComparison.OrdinalIgnoreCase)))
            {
                _ActingRoles.Add(new OMLSDKRole(actor, role));
            }
        }


        private List<OMLSDKPerson> _Directors;
        public List<OMLSDKPerson> Directors
        {
            get
            {
                return _Directors;
            }
        }
        public void AddDirector(OMLSDKPerson director)
        {
            if (!(_Directors.Exists(p => p.full_name.Equals(director.full_name, StringComparison.OrdinalIgnoreCase))))
            {
                _Directors.Add(director);
            }
        }


        private List<OMLSDKPerson> _Writers;
        public List<OMLSDKPerson> Writers
        {
            get
            {
                return _Writers;
            }
        }
        public void AddWriter(OMLSDKPerson writer)
        {
            if (!(_Writers.Exists(p => p.full_name.Equals(writer.full_name, StringComparison.OrdinalIgnoreCase))))
            {
                _Writers.Add(writer);
            }
        }


        private List<OMLSDKPerson> _Producers;
        public List<OMLSDKPerson> Producers
        {
            get
            {
                return _Producers;
            }
        }
        public void AddProducer(OMLSDKPerson producer)
        {
            if (!(_Producers.Exists(p => p.full_name.Equals(producer.full_name, StringComparison.OrdinalIgnoreCase))))
            {
                _Producers.Add(producer);
            }
        }


        private List<string> _Tags;
        public List<string> Tags
        {
            get
            {
                return _Tags;
            }
        }
        public void AddTag(string tag)
        {
            if (tag.Length > 255)
                throw new FormatException("Tag must be 255 characters or less.");

            if (string.IsNullOrEmpty(tag))
                return;

            if (_Tags.Contains(tag))
                return;

            _Tags.Add(tag);
        }

        #endregion



        public void AddFanArtImage(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (FanArtPaths.Contains(path))
                return;

            if (File.Exists(path))
            {
                FanArtPaths.Add(path);
            }
        }



        /// <summary>
        /// Add an audio track
        /// </summary>
        /// <param name="audioTrack"></param>
        /*public void AddAudioTrack(string audioTrack)
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
        }*/

        /// <summary>
        /// Adds a subtitle
        /// </summary>
        /// <param name="subtitle"></param>
        /*public void AddSubtitle(string subtitle)
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
        }*/

        /// <summary>
        /// Adds a trailer to the title
        /// </summary>
        /// <param name="trailer"></param>
        /*public void AddTrailer(string trailer)
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
        }*/

        /// <summary>
        /// Add a Genre to the genres list
        /// </summary>
        /// <param name="genre">A Genre from the Genre enum</param>
        

        public override string ToString()
        {
            return "Title:" + this.Name; // +" (" + this.Id + ")";
        }

        
    }

    public class OMLSDKRole
    {
        public string PersonName;
        public string RoleName;

        public OMLSDKRole(string personName, string roleName)
        {
            PersonName = personName;
            RoleName = roleName;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(RoleName))
            {
                return PersonName;
            }
            else
            {
                return PersonName + " as " + RoleName;
            }
        }

        public string Display
        {
            get { return ToString(); }
        }
    }


    public class OMLSDKPerson
    {
        private string _full_name;
        private OMLSDKSex _sex;
        private DateTime _birth_date;
        private string _photo_path;

        /// <summary>
        /// Full name
        /// </summary>
        public string full_name { get; set; }

        /// <summary>
        /// Sex of Person (using the Sex enum)
        /// </summary>
        public OMLSDKSex sex { get; set; }


        /// <summary>
        /// Date of Birth
        /// </summary>
        public DateTime BirthDate { get; set; }


        /// <summary>
        /// Full path the photo image for person
        /// </summary>
        public string PhotoPath { get; set; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public OMLSDKPerson()
        {
        }

        /// <summary>
        /// Constructor with first and last name
        /// </summary>
        /// <param name="full_name">string name</param>
        public OMLSDKPerson(string full_name)
        {
            _full_name = full_name;
        }

        public override string ToString()
        {
            return full_name;
        }
    }


    public enum OMLSDKSex
    {
        Male,
        Female
    };

    public enum OMLSDKVideoFormat : int
    {
        // DO NOT MODIFY ORDER, INSERT IN THE MIDDLE, OR REMOVE ENTRIES, JUST ADD TO THE END!

        ASF = 1, // WMV style
        AVC = 2, // AVC H264
        AVI = 3, // DivX, Xvid, etc
        B5T = 4, // BlindWrite image
        B6T = 5, // BlindWrite image
        BIN = 6, // using an image loader lib and load/play this as a DVD
        BLURAY = 7, // detect which drive supports this and request the disc
        BWT = 8, // BlindWrite image
        CCD = 9, // CloneCD image
        CDI = 10, // DiscJuggler Image
        CUE = 11, // cue sheet
        DVD = 12, // detect which drive supports this and request the disc
        DVRMS = 13, // MPG
        H264 = 14, // AVC OR MP4
        HDDVD = 15, // detect which drive supports this and request the disc
        IFO = 16, // Online DVD
        IMG = 17, // using an image loader lib and load/play this as a DVD
        ISO = 18, // Standard ISO image
        ISZ = 19, // Compressed ISO image
        M2TS = 20, // mpeg2 transport stream
        MDF = 21, // using an image loader lib and load/play this as a DVD
        MDS = 22, // Media Descriptor file
        MKV = 23, // Likely h264
        MOV = 24, // Quicktime
        MPG = 25,
        MPEG = 26,
        MP4 = 27, // DivX, AVC, or H264
        NRG = 28, // Nero image
        OFFLINEBLURAY = 29, // detect which drive supports this and request the disc
        OFFLINEDVD = 30, // detect which drive supports this and request the disc
        OFFLINEHDDVD = 31, // detect which drive supports this and request the disc
        OGM = 32, // Similar to MKV
        PDI = 33, // Instant CD/DVD image
        TS = 34, // MPEG2
        UIF = 35,
        UNKNOWN = 36,
        URL = 37, // this is used for online content (such as streaming trailers)
        WMV = 38,
        VOB = 39, // MPEG2
        WVX = 40, // wtf is this?
        ASX = 41, // like WPL
        WPL = 42, // playlist file?
        WTV = 43, // new dvr format in vista (introduced in the tv pack 2008)
        DIVX = 44,

        ALL = 2147483647, // meaning all format types - used for setting video format to external player
    };
}


