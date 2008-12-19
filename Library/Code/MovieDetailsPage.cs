using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MediaCenter.UI;

using OMLEngine;
//using OMLGetDVDInfo;

namespace Library
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// details page UI.
    /// </summary>
    public class MovieDetailsPage : ModelItem
    {

        #region Private Variables
        /// <summary>The URI of the media at its locally cached location.</summary>
        //private FileInfo _localMedia = null;
        private MovieItem _movieDetails = null;
        //private bool _isShowingDisks = false;
        //private string _playMovieCommandText = "Show Discs";
        private IList _commands;
        private string _metadata;
        private Choice _diskChoice = new Choice();
        private BooleanChoice _watched = new BooleanChoice();
        private ContextMenu _dvdContextMenu;
        private bool _showDVDContextMenu = false;
        //private Command _hideDisks;
        //private Command _showDisks;
        private Image _FullCover;
        private bool _PlayClicked = false;
        #endregion

        #region Public Properties

        public BooleanChoice Watched
        {
            get { return _watched; }         
        }

        public bool ShowDVDContextMenu
        {
            get { return _showDVDContextMenu; }
            set
            {
                _showDVDContextMenu = value;
                FirePropertyChanged("ShowDVDContextMenu");
            }
        }

        public ContextMenu DVDContextMenu
        {
            get { return _dvdContextMenu; }
            set
            {
                _dvdContextMenu = value;
                FirePropertyChanged("DVDContextMenu");
            }
        }

        public bool PlayClicked
        {
            get { return _PlayClicked; }
            set
            {
                _PlayClicked = value;
                FirePropertyChanged("PlayClicked");
            }
        }
        //public string PlayMovieCommandText
        //{
        //    get
        //    {
        //        if (this.MovieDetails.Disks.Count == 1)
        //            return "Play Movie";
        //        else if (this.MovieDetails.Disks.Count == 0)
        //            return "No Discs!";
        //        else
        //            return _playMovieCommandText;
        //    }
        //    set
        //    {
        //        _playMovieCommandText = value;
        //    }
        //}

        public string DirectorsAsString
        {
            get
            {
                string dirs = "";
                foreach (Person p in _movieDetails.TitleObject.Directors)
                {
                    if (dirs.Length > 0) dirs += ", ";
                    dirs += p.full_name;
                }
                return dirs;
            }
        }        

        public string WritersAsString
        {
            get
            {
                string dirs = "";
                foreach (Person p in _movieDetails.TitleObject.Writers)
                {
                    if (dirs.Length > 0) dirs += ", ";
                    dirs += p.full_name;
                }
                return dirs;
            }
        }
        public MovieItem MovieDetails
        {
            get { return _movieDetails; }
            set { _movieDetails = value; }
        }
        /// <summary>Gets or sets the URI of the media at its locally cached location.</summary>
        //public FileInfo LocalMedia
        //{
        //    get { return _localMedia; }
        //    set { _localMedia = value; }
        //}

        public string UserRating
        {
            get { return ((double)_movieDetails.UseStarRating / 10).ToString(); }
        }

        /// <summary>
        /// The primary title of the object.
        /// </summary>
        public string Title
        {
            get { return _movieDetails.Name; }
            set
            {
                if (_movieDetails.Name != value)
                {
                    _movieDetails.Name = value;
                    FirePropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// A multiline summary of the object.
        /// </summary>
        public string Summary
        {
            get { return _movieDetails.Synopsis; }
            set
            {
                if (_movieDetails.Synopsis != value)
                {
                    _movieDetails.Synopsis = value;
                    FirePropertyChanged("Summary");
                }
            }
        }

        /// <summary>
        /// A list of actions that can be performed on this object.
        /// This list should only contain objects of type Command.
        /// </summary>
        public IList Commands
        {
            get { return _commands; }
            set
            {
                if (_commands != value)
                {
                    _commands = value;
                    FirePropertyChanged("Commands");
                }
            }
        }

        /// <summary>
        /// Additional minor metadata about this object.
        /// </summary>
        public string Metadata
        {
            get { return _metadata; }
            set
            {
                if (_metadata != value)
                {
                    _metadata = value;
                    FirePropertyChanged("Metadata");
                }
            }
        }

        public Image FullCover
        {
            get { return _FullCover; }
            set
            {
                if (_FullCover != value)
                {
                    _FullCover = value;
                    FirePropertyChanged("FullCover");
                }

            }
        }

        public string Rating
        {
            get
            {
                if (_movieDetails.Rating.Trim().Length > 0)
                    return _movieDetails.Rating;
                else
                    return "";
            }

            set
            {

                if (_movieDetails.Rating != value)
                {
                    _movieDetails.Rating = value;
                    FirePropertyChanged("Rating");
                }
            }
        }

        public string Length
        {
            get
            {
                if (_movieDetails.TitleObject.Runtime > 0)
                    return _movieDetails.Runtime + " min";
                else
                    return "";
            }
            set
            {
                if (_movieDetails.Runtime != value)
                {
                    _movieDetails.Runtime = value;
                    FirePropertyChanged("Length");
                }
            }
        }

        public string ReleaseDateYear
        {
            get
            {
                if (_movieDetails.TitleObject.ReleaseDate != DateTime.MinValue)
                    return _movieDetails.TitleObject.ReleaseDate.Year.ToString();
                else
                    return "";
            }
        }

        public string ReleaseDate
        {
            get
            {
                if (_movieDetails.TitleObject.ReleaseDate != DateTime.MinValue)
                    return _movieDetails.ReleaseDate;
                else
                    return "";
            }
        }

        public string ReleaseYear
        {
            get
            {
                if (_movieDetails.TitleObject.ReleaseDate != DateTime.MinValue)
                    return Convert.ToString(_movieDetails.TitleObject.ReleaseDate.Year);
                else
                    return "";
            }
        }

        public IList Actors
        {
            get { return _movieDetails.Actors; }
        }

        public IList ActingRoles
        {
            get { return _movieDetails.ActingRoles; }
        }

        public IList Directors
        {
            get { return _movieDetails.Directors; }
        }

        public IList Genres
        {
            get { return _movieDetails.TitleObject.Genres; }
        }

        public string GenresAsString
        {
            get
            {
                string res = "";
                foreach (string s in _movieDetails.TitleObject.Genres)
                {
                    if (res.Length > 0) res += ", ";
                    res += s;
                }
                return res;
            }
        }

        public string TagsAsString
        {
            get
            {
                string res = "";
                foreach (string s in _movieDetails.TitleObject.Tags)
                {
                    if (res.Length > 0) res += ", ";
                    res += s;
                }
                return res;
            }
        }

        public List<string> Languages
        {
            get { return _movieDetails.TitleObject.AudioTracks; }
        }

        public string LanguagesAsString
        {
            get
            {
                string res = "";
                foreach (string s in _movieDetails.TitleObject.AudioTracks)
                {
                    if (res.Length > 0) res += ", ";
                    res += s;
                }
                return res;
            }
        }

        public string VideoDetails
        {
            get
            {
                string details = "";
                if (_movieDetails.TitleObject.Disks.Count > 0)
                    details += _movieDetails.TitleObject.Disks[0].Format.ToString();  //again, should really do this on a per disk bases, but this should be ok for now
                return _movieDetails.TitleObject.VideoDetails;
            }
        }

        public string SubtitlesAsString
        {
            get
            {
                string res = "";
                foreach (string s in _movieDetails.TitleObject.Subtitles)
                {
                    if (res.Length > 0) res += ", ";
                    res += s;
                }
                return res;
            }
        }

        public IList Producers
        {
            get { return _movieDetails.Producers; }
        }

        public IList Writers
        {
            get { return _movieDetails.Writers; }
            //set
            //{
            //    if (_movieDetails.Writers != value)
            //    {
            //        _movieDetails.Writers = value;
            //        FirePropertyChanged("Writers");
            //    }
            //}
        }

        public Choice DiskChoice
        {
            get { return _diskChoice; }
        }

        //public Command ShowDisks
        //{
        //    get { return _showDisks; }
        //}

        //public Command HideDisks
        //{
        //    get { return _hideDisks; }
        //}

        #endregion

        public MovieDetailsPage(MovieItem item)
        {
            LoadDetails(item);
        }

        public override string ToString()
        {
            return "MovieDetailsPage:" + this._movieDetails;
        }

        private void LoadDetails(MovieItem item)
        {
            _movieDetails = item;
            //_localMedia = null;

            //DVDDiskInfo debug code
            //if (item.TitleObject.Disks.Count > 0)
            //{
            //    DVDDiskInfo info = item.TitleObject.Disks[0].DVDDiskInfo;
            //}

            if (!string.IsNullOrEmpty(item.TitleObject.FrontCoverPath))
            {
                if (File.Exists(item.TitleObject.FrontCoverPath))
                {
                    _FullCover = GalleryItem.LoadImage(item.TitleObject.FrontCoverPath);
                }
            }
            _diskChoice = new Choice();
            if (_movieDetails.Disks.Count > 0 )
            {
                _diskChoice.Options = _movieDetails.FriendlyNamedDisks;
            }
            else
            {
                Disk[] temp = { new Disk() };
                temp[0].Name = "Play Me";
                _diskChoice.Options = temp; // MCE barfs if Options is bound to empty List.
                OMLApplication.DebugLine("[MovieDetailsPage] Details Page.LoadMovies: no disks");
            }

            _watched.Chosen = (_movieDetails.TitleObject.WatchedCount != 0);
        }

        public void PlayDiskWithOptions()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                // Play the Selected Disk
                _movieDetails.PlayMovie();
                _PlayClicked = false; // I use the private variable because I don't want to send an event to the MCML page
            });
        }

        private void CreateDvdContextMenuIfNeeded()
        {
            if ( _dvdContextMenu == null )
                _dvdContextMenu = new ContextMenu();
        }

        public void PlayDisk(int SelectedDisk)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                // Play the Selected Disk
                PlayClicked = true;
                _movieDetails.TitleObject.SelectedDisk = _movieDetails.Disks[SelectedDisk];
                PlayMovieOrShowDVDContextMenu();
                _PlayClicked = false; // I use the private variable because I don't want to send an event to the MCML page
            });
        }

        // TODO: enable this once it is present on all detail pages 1, 2, 3, and working, but only for extender sessions
        private bool _useDVDContextMenu = false; // OMLApplication.Current.IsExtender;
        private void PlayMovieOrShowDVDContextMenu()
        {
            if (_useDVDContextMenu && _movieDetails.TitleObject.SelectedDisk.Format == VideoFormat.DVD)
            {
                _dvdContextMenu = null;

                MediaSource ms = new MediaSource(_movieDetails.TitleObject.SelectedDisk);
                if (ms.DVDTitle != null && ms.DVDTitle.AudioTracks.Count > 0)
                {
                    CreateDvdContextMenuIfNeeded();
                    ICommand cmd = new Command();
                    cmd.Description = "Change Audio";
                    _dvdContextMenu.AddAudioCommand(cmd);

                    IList audList = new List<string>();
                    foreach (var audio in ms.DVDTitle.AudioTracks)
                        audList.Add(audio.ToString());

                    Choice audChoice = new Choice();
                    audChoice.Options = audList;
                    _dvdContextMenu.AudioTracksChoice = audChoice;
                }

                if (ms.DVDTitle != null && ms.DVDTitle.Subtitles.Count > 0)
                {
                    CreateDvdContextMenuIfNeeded();
                    ICommand cmd = new Command();
                    cmd.Description = "Change Subtitle";
                    _dvdContextMenu.AddSubtitleCommand(cmd);

                    IList subList = new List<string>();
                    subList.Add("None");
                    foreach (var subtitle in ms.DVDTitle.Subtitles)
                        subList.Add(subtitle.Language);

                    Choice subChoice = new Choice();
                    subChoice.Options = subList;
                    _dvdContextMenu.SubtitleTracksChoice = subChoice;
                }

                if (ms.DVDTitle != null && ms.DVDTitle.Chapters.Count > 0)
                {
                    CreateDvdContextMenuIfNeeded();
                    ICommand cmd = new Command();
                    cmd.Description = "Select Chapter";
                    _dvdContextMenu.AddChapterCommand(cmd);

                    IList chapList = new List<string>();
                    chapList.Add("None");
                    foreach (var chapter in ms.DVDTitle.Chapters)
                        chapList.Add(string.Format("Chapter {0}", chapter.ChapterNumber));

                    Choice chapChoice = new Choice();
                    chapChoice.Options = chapList;
                    _dvdContextMenu.ChapterSelectionChoice = chapChoice;
                }

                if (_dvdContextMenu != null)
                {
                    ICommand playCmd = new Command();
                    playCmd.Description = "Play Now";
                    _dvdContextMenu.AddPlayCommand(playCmd);

                    ShowDVDContextMenu = true;
                }
                else
                {
                    _movieDetails.PlayMovie();
                }
            }
            else
            {
                _movieDetails.PlayMovie();
            }
        }

        public void UpdateWatched()
        {
            bool watched = (bool)_watched.Chosen;

            if (watched && _movieDetails.TitleObject.WatchedCount == 0)
            {                    
                _movieDetails.TitleObject.WatchedCount = 1;
                OMLApplication.Current.SaveTitles();
            }
            else if (!watched && _movieDetails.TitleObject.WatchedCount != 0)
            {
                _movieDetails.TitleObject.WatchedCount = 0;
                OMLApplication.Current.SaveTitles();
            }
        }
    }


    // since for some reason Media Center's MCML engine will not load the OMLEngine Assembly 
    // This prevents me from casting the Disks Repeater items to Disk type.
    // By creating this wrapper, I can do the cast successfully. 
    public class DiskWrapper : Disk
    {

        public DiskWrapper() : base()
        {
        }

        public DiskWrapper(string name, string path, VideoFormat format)
            : base(name, path, format)
            {}


    }
}




