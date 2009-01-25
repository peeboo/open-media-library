using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Microsoft.MediaCenter.UI;
using System.Text;

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
        private Choice _actors;
        private Choice _directors;
        private Choice _writers;
        private MovieItem _movieDetails = null;
        private Choice _diskChoice = new Choice();
        private BooleanChoice _watched = new BooleanChoice();
        private ContextMenu _dvdContextMenu;
        private bool _showDVDContextMenu = false;
        private Image _fullCover;
        private bool _playClicked = false;        
        private Image backgroundImage;
        private Single fanAlphaFadeOverride;
        
        private List<Image> backgroundImages = null;
        #endregion

        #region Public Properties

        public Choice Actors { get { return _actors; } }
        public Choice Directors { get { return _directors; } }
        public Choice Writers { get { return _writers; } }
        public BooleanChoice Watched { get { return _watched; } }
        public MovieItem MovieDetails { get { return _movieDetails; } }
        public string UserRating { get { return ((double)_movieDetails.UseStarRating / 10).ToString(); } }
        public string Title { get { return _movieDetails.Name; } }
        public Image FullCover { get { return _fullCover; } }
        public List<string> Languages { get { return _movieDetails.TitleObject.AudioTracks; } }
        public Choice DiskChoice { get { return _diskChoice; } }

        /// <summary>
        /// A multiline summary of the object.
        /// </summary>
        public string Summary { get { return _movieDetails.Synopsis; } }
        public bool HasFanArtImage { get { return backgroundImages != null && backgroundImages.Count != 0; } }
        public bool RotateFanArt { get { return backgroundImages != null && backgroundImages.Count > 1; } }

        public bool ShowDVDContextMenu
        {
            get { return _showDVDContextMenu; }
            set
            {
                _showDVDContextMenu = value;
                FirePropertyChanged("ShowDVDContextMenu");
            }
        }

        public Single FanAlphaFadeOverride
        {
            get { return fanAlphaFadeOverride; }
            set
            {
                fanAlphaFadeOverride = value;
                FirePropertyChanged("FanAlphaFadeOverride");
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
            get { return _playClicked; }
            set
            {
                _playClicked = value;
                FirePropertyChanged("PlayClicked");
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

        public string SubHeading
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                
                string year = ReleaseYear;
                string genres = GenresAsString;
                string rating = Rating;
                string length = Length;

                if (!string.IsNullOrEmpty(year))
                    sb.Append(year);

                if (!string.IsNullOrEmpty(genres))
                {
                    if ( sb.Length != 0)
                        sb.Append(", ");

                    sb.Append(genres);
                }

                if(!string.IsNullOrEmpty(rating))
                {
                    if ( sb.Length != 0)
                        sb.Append(", ");

                    sb.Append(rating);
                }

                if (!string.IsNullOrEmpty(length))
                {
                    if (sb.Length != 0)
                        sb.Append(", ");

                    sb.Append(length);
                }

                return sb.ToString();
            }
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
        
        #endregion

        public MovieDetailsPage(MovieItem item)
        {
            LoadDetails(item);
            SetBackgroundImage();
        }

        public override string ToString()
        {
            return "MovieDetailsPage:" + this._movieDetails;
        }

        public Image BackgroundImage
        {
            get { return backgroundImage; }
            set
            {
                backgroundImage = value;
                FirePropertyChanged("BackgroundImage");
            }
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
                    _fullCover = GalleryItem.LoadImage(item.TitleObject.FrontCoverPath);
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

            SetupCastObjects();                       
        }

        private void SetupCastObjects()
        {
            List<MovieCastCommand> actors = new List<MovieCastCommand>(_movieDetails.TitleObject.ActingRoles.Count);

            foreach (string actor in _movieDetails.TitleObject.ActingRoles.Keys)
            {
                actors.Add(new MovieCastCommand(actor, _movieDetails.TitleObject.ActingRoles[actor]));
            }

            _actors = new Choice(this, "Actors", actors);


            List<MovieCastCommand> directors = new List<MovieCastCommand>(_movieDetails.TitleObject.Directors.Count);

            foreach (Person director in _movieDetails.TitleObject.Directors)
            {
                directors.Add(new MovieCastCommand(director.full_name, null));
            }

            _directors = new Choice(this, "Directors", directors);

            List<MovieCastCommand> writers = new List<MovieCastCommand>(_movieDetails.TitleObject.Writers.Count);

            foreach (Person writer in _movieDetails.TitleObject.Writers)
            {
                writers.Add(new MovieCastCommand(writer.full_name, null));
            }

            _writers = new Choice(this, "Writers", writers);
        }

        public void PlayDiskWithOptions()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                // Play the Selected Disk
                _movieDetails.PlayMovie();
                _playClicked = false; // I use the private variable because I don't want to send an event to the MCML page
            });
        }

        private void CreateDvdContextMenuIfNeeded()
        {
            if ( _dvdContextMenu == null )
                _dvdContextMenu = new ContextMenu();
        }

        public void PlayMovie()
        {
            // if there is more than one disk show the context menu
            if (_movieDetails.Disks != null &&
                MovieDetails.Disks.Count > 1)
            {
                ShowDVDContextMenu = true;
            }
            else if ( _movieDetails.Disks.Count == 1 )
            {
                // just play the first disk
                PlayDisk(0);
            }

            // if there are no disks do nothing
        }

        public void PlayAllDisks()
        {
            if (!((bool)_watched.Chosen))
                _watched.Chosen = true;

            OMLApplication.ExecuteSafe(delegate
            {
                // Play the Selected Disk
                PlayClicked = true;
                _movieDetails.PlayAllDisks();
                _playClicked = false; // I use the private variable because I don't want to send an event to the MCML page
            });
        }

        public void PlayDisk(int SelectedDisk)
        {
            // check the checkbox - this is to make it checked when the user clicks the back button
            // this will trigger the save to happen again but since we've already updated the count 
            // the save will be a noop
            if (! ((bool)_watched.Chosen))
                _watched.Chosen = true;

            OMLApplication.ExecuteSafe(delegate
            {
                // Play the Selected Disk
                PlayClicked = true;
                _movieDetails.TitleObject.SelectedDisk = _movieDetails.Disks[SelectedDisk];
                PlayMovieOrShowDVDContextMenu();
                _playClicked = false; // I use the private variable because I don't want to send an event to the MCML page
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
            OMLApplication.ExecuteSafe(delegate
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
            });
        }

        private void SetBackgroundImage()
        {
            // only setup the background images once
            if (backgroundImages != null)
                return;

            backgroundImages = new List<Image>(1);

            if (!string.IsNullOrEmpty(_movieDetails.TitleObject.BackDropImage))
            {                
                if (File.Exists(Path.GetFullPath(_movieDetails.TitleObject.BackDropImage)))
                {
                    backgroundImages.Add(new Image(
                        string.Format("file://{0}", Path.GetFullPath(_movieDetails.TitleObject.BackDropImage))));
                    
                    return;
                }                
            }

            // a specific file was NOT found, time to go hunting
            
            // if the /FanArt folder doesn't exist - that means no background images
            if (!Directory.Exists(_movieDetails.TitleObject.BackDropFolder))
                return;                

            foreach (string file in
                Directory.GetFiles(_movieDetails.TitleObject.BackDropFolder, "*.jpg", SearchOption.TopDirectoryOnly))
            {
                OMLApplication.DebugLine("[MovieDetailsPage] loading fanart image {0}", file);

                backgroundImages.Add(new Image(string.Format("file://{0}", file)));
            }

            // oops - no images found in the fanart folder
            if (backgroundImages.Count == 0)
                return;

            // set the background image
            BackgroundImage = backgroundImages[0];            
        }

        public void RotateBackground()
        {
            if (backgroundImages == null || backgroundImages.Count == 0)
                return;

            int currentIndex = backgroundImages.IndexOf(BackgroundImage);

            if (currentIndex == -1 || currentIndex == backgroundImages.Count - 1)
                BackgroundImage = backgroundImages[0];
            else
                BackgroundImage = backgroundImages[currentIndex + 1];
        }
    }

    public class MovieCastCommand : Command
    {
        private string name;
        private string part;

        public override string ToString()
        {
            return name;
        }

        public string Name
        {
            get 
            {
                return (string.IsNullOrEmpty(part)) ? name : name + " as " + part;
            }
        }

        public MovieCastCommand(string name, string part)  
        {
            this.name = name;
            this.part = part;

            this.Invoked += new EventHandler(MovieCastCommand_Invoked);
        }

        private void MovieCastCommand_Invoked(object sender, EventArgs e)
        {
            MovieGallery gallery = new MovieGallery(OMLApplication.Current.Titles, "Home");
            OMLApplication.Current.GoToSelectionList(gallery, Filter.Participant, name);
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




