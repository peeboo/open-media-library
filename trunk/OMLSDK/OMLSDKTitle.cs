using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace OMLSDK
{
    public class OMLSDKTitle : IComparable
    {
        public OMLSDKTitle()
        {
            _NonActingRoles = new List<OMLSDKRole>();
            _ActingRoles = new List<OMLSDKRole>();
            _Directors = new List<OMLSDKPerson>();
            _Writers = new List<OMLSDKPerson>();
            _Producers = new List<OMLSDKPerson>();

            _Genres = new List<string>();

            _Disks = new List<OMLSDKDisk>();

            _AudioTracks = new List<string>();
            _Subtitles = new List<string>();
            _Trailers = new List<string>();
        }

        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string SortName { get; set; }
        public string Synopsis { get; set; }
        public int ProductionYear { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime DateAdded { get; set; }
        public int Runtime { get; set; }
        public string Studio { get; set; }
        public string UPC { get; set; }
        public int WatchedCount { get; set; }
        public int? UserStarRating { get; set; }

        public short? EpisodeNumber { get; set; }
        public short? SeasonNumber { get; set; }

        public string AspectRatio { get; set; }
        public string VideoDetails { get; set; }
        public string VideoResolution { get; set; }
        public string VideoStandard { get; set; }
        public OMLSDKVideoFormat VideoFormat { get; set; }

        public string FrontCoverPath { get; set; }
        public string BackCoverPath { get; set; }

        public string CountryOfOrigin { get; set; }

        public string MetadataSourceName { get; set; }
        public string MetadataSourceID { get; set; }
        public string ImporterSource { get; set; }

        public string OfficialWebsiteURL { get; set; }
        public string ParentalRating { get; set; }
        public string ParentalRatingReason { get; set; }


        #region Disks
        private List<OMLSDKDisk> _Disks;
        public List<OMLSDKDisk> Disks
        {
            get
            {
                return _Disks;
            }
        }
        public void AddDisk(OMLSDKDisk Disk)
        {
            _Disks.Add(Disk);
        }
        #endregion


        #region Extra Features
        private List<string> _extraFeatures = new List<string>();
        public List<string> ExtraFeatures
        {
            get { return _extraFeatures; }
            set { _extraFeatures = value; }
        }
        #endregion


        #region Trailers
        private List<string> _Trailers = null;
        public List<string> Trailers
        {
            get
            {
                return _Trailers;
            }
        }
        public void AddTrailer(string Trailer)
        {
            _Trailers.Add(Trailer);
        }
        #endregion


        #region Genres
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
        #endregion


        #region Acting Roles
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
        #endregion


        #region Non Acting Roles
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
        #endregion


        #region Directors
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
        #endregion


        #region Writers
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
        #endregion


        #region Producers
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
        #endregion


        #region Tags
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


        #region AudioTracks
        private List<string> _AudioTracks;
        public List<string> AudioTracks
        {
            get
            {
                return _AudioTracks;
            }
        }
        public void AddAudioTrack(string audioTrack)
        {
            if (string.IsNullOrEmpty(audioTrack))
                return;

            if (!_AudioTracks.Contains(audioTrack))
            {
                _AudioTracks.Add(audioTrack);
            }
        }
        #endregion


        #region Subtitles
        private List<string> _Subtitles;
        public List<string> Subtitles
        {
            get
            {
                return _Subtitles;
            }
        }
        public void AddSubtitle(string subtitle)
        {
            if (string.IsNullOrEmpty(subtitle))
                return;

            if (!_Subtitles.Contains(subtitle))
            {
                _Subtitles.Add(subtitle);
            }
        }
        #endregion


        #region Fanart Paths
        private IList<string> _FanArtPaths;
        public IList<string> FanArtPaths
        { 
            get
            {
                return _FanArtPaths;
            }
            set
            {
                _FanArtPaths = value;
            }
        }
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
        #endregion


        public int CompareTo(object other)
        {
            OMLSDKTitle otherT = other as OMLSDKTitle;
            if (otherT == null)
                return -1;
            return Name.CompareTo(otherT.Name);
        }


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

    public enum OMLSDKSex
    {
        Male,
        Female
    };

    public class OMLSDKPerson
    {
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
        public OMLSDKPerson(string _full_name)
        {
            full_name = _full_name;
        }

        public override string ToString()
        {
            return full_name;
        }
    }


}


