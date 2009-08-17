using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;
using System.IO;
using Microsoft.MediaCenter.UI;

namespace Library.Code.V3
{
    public class MovieItem : GalleryItem
    {

        private void CacheImageDone(object nothing)
        {
            FirePropertyChanged("DefaultImage");
        }

        public void SetImage(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                this.m_defaultImage = new Image("file://" + imagePath);
            }
        }

        public MovieItem(Library.GalleryItem title, IModelItem owner, List<TitleFilter> filter)
            : base(owner)
        {
            this.Description = title.Caption;

            this.Invoked += delegate(object sender, EventArgs args)
            {
                //am I being silly here copying this?
                List<TitleFilter> newFilter = new List<TitleFilter>();
                foreach (TitleFilter filt in filter)
                {
                    newFilter.Add(filt);
                }
                newFilter.Add(new TitleFilter(title.Category.FilterType, title.Name));
                OMLProperties properties = new OMLProperties();
                properties.Add("Application", OMLApplication.Current);
                properties.Add("I18n", I18n.Instance);
                Command CommandContextPopOverlay = new Command();
                properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);

                Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage(newFilter, title.Description);

                properties.Add("Page", gallery);
                OMLApplication.Current.Session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
            };
        }

        private bool isUnwatched = false;
        public bool IsUnwatched
        {
            get
            {
                return isUnwatched;
            }
            set
            {
                if (this.isUnwatched != value)
                {
                    this.isUnwatched = value;
                    FirePropertyChanged("IsUnwatched");
                }
            }
        }


        private OMLEngine.Title _titleObj;
        public OMLEngine.Title TitleObject { get { return _titleObj; } }

        public MovieItem(Title title, IModelItem owner)
            : base(owner)
        {
            this._titleObj = title;
            //this.InternalMovieItem = new Library.MovieItem(title, null);

            this.ItemType = 0;
            if(OMLEngine.Settings.OMLSettings.ShowUnwatchedIcon)
                this.OverlayContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGalleryItem#UnwatchedOverlay";
            if (this._titleObj.WatchedCount == 0)
                this.isUnwatched = true;

            DateTime releaseDate = Convert.ToDateTime(_titleObj.ReleaseDate.ToString("MMMM dd, yyyy"));// new DateTime(2000, 1, 1);
            //invalid dates
            if (releaseDate.Year != 1 && releaseDate.Year != 1900)
                this.MetadataTop = releaseDate.Year.ToString();
            else
                this.MetadataTop = "";

            this.ItemId = 1;
            string starRating = Convert.ToString(Math.Round((Convert.ToDouble(_titleObj.UserStarRating.HasValue ? _titleObj.UserStarRating.Value : 0) * 0.8), MidpointRounding.AwayFromZero));
            this.StarRating = starRating;
            string extendedMetadata = string.Empty;

            this.SortName = title.SortName;

            //this.Metadata = this.InternalMovieItem.Rating.Replace("PG13", "PG-13").Replace("NC17", "NC-17");
            this.Metadata = _titleObj.ParentalRating;
            if (string.IsNullOrEmpty(_titleObj.ParentalRating))
                this.Metadata = "Not Rated";
            if (_titleObj.Runtime.ToString() != "0")
                this.Metadata += string.Format(", {0} minutes", _titleObj.Runtime.ToString());
            this.Tagline = _titleObj.Synopsis;

            this.Description = title.Name;

            //TODO: not sure how to read this yet...
            //temporarially disabled due to bug in eagerloading
            //this.SimpleVideoFormat = _titleObj.VideoFormat.ToString();


            this.Invoked += delegate(object sender, EventArgs args)
            {

                //to test v3
                Library.Code.V3.DetailsPage page = new Library.Code.V3.DetailsPage();

                page.Description = "movie details";
                page.Title = this.Description;
                page.Summary = _titleObj.Synopsis;
                page.Background = this.DefaultImage;

                page.Details = new Library.Code.V3.ExtendedDetails();

                page.Details.CastArray = new ArrayListDataSet();

                string directors = string.Empty;
                int directorCount = 0;
                foreach (Person director in _titleObj.Directors)
                {
                    Library.Code.V3.CastCommand directorCommand = new Library.Code.V3.CastCommand();
                    directorCommand.Role = " ";
                    directorCommand.Description = director.full_name;
                    directorCommand.Invoked += new EventHandler(actorCommand_Invoked);
                    if (directorCount == 0)
                    {
                        directorCommand.CastType = "TitleAndDesc";
                        directorCommand.GroupTitle = "DIRECTOR";
                        directorCommand.ActorId = 1;//not sure how this works in oml
                    }
                    else
                        directorCommand.CastType = "Desc";
                    page.Details.CastArray.Add(directorCommand);
                    directorCount++;
                    AppendSeparatedValue(ref directors, director.full_name, "; ");
                }
                if (!string.IsNullOrEmpty(directors))
                    page.Details.Director = string.Format("Directed By: {0}", directors);

                string cast = string.Empty;
                int actorCount = 0;
                foreach (Role kvp in _titleObj.ActingRoles)
                {
                    Library.Code.V3.CastCommand actorCommand = new Library.Code.V3.CastCommand();
                    actorCommand.Description = kvp.PersonName;
                    actorCommand.ActorId = 1;//not sure how this works in oml
                    actorCommand.Invoked += new EventHandler(actorCommand_Invoked);
                    actorCommand.Role = kvp.RoleName;
                    if (actorCount == 0)
                    {
                        //add the title "CAST"
                        actorCommand.CastType = "TitleAndDesc";
                        actorCommand.GroupTitle = "CAST";
                    }
                    else
                        actorCommand.CastType = "Desc";

                    page.Details.CastArray.Add(actorCommand);
                    actorCount++;
                    AppendSeparatedValue(ref cast, kvp.PersonName, "; ");
                }

                //foreach (string castmember in this.InternalMovieItem.Actors)
                //{
                //    AppendSeparatedValue(ref cast, castmember, "; ");
                //}
                if (!string.IsNullOrEmpty(cast))
                    page.Details.Cast = string.Format("Cast Info: {0}", cast);


                if (this.StarRating != "0")
                    page.Details.StarRating = new Image(string.Format("resx://Library/Library.Resources/V3_Controls_Common_Stars_{0}", starRating));

                //strip invalid dates
                if (releaseDate.Year != 1 && releaseDate.Year != 1900)
                    page.Details.YearString = releaseDate.Year.ToString();

                page.Details.Studio = _titleObj.Studio;

                string genres = string.Empty;
                if (_titleObj.Genres.Count > 0)
                {
                    foreach (string genre in _titleObj.Genres)
                    {
                        AppendSeparatedValue(ref genres, genre, ", ");
                    }
                }
                AppendSeparatedValue(ref genres, this.Metadata, ", ");
                page.Details.GenreRatingandRuntime = genres;

                //fixes double spacing issues
                page.Details.Summary = _titleObj.Synopsis.Replace("\r\n", "\n");
                page.Commands = new ArrayListDataSet(page);
                //default play command
                Command playCmd = new Command();
                playCmd.Description = "Play";
                playCmd.Invoked += new EventHandler(playCmd_Invoked);
                page.Commands.Add(playCmd);

                if (page.Details.CastArray.Count > 0)
                {
                    Command command = new Command();
                    command.Description = "Cast + More";

                    command.Invoked += delegate(object castSender, EventArgs castArgs)
                    {
                        Dictionary<string, object> castProperties = new Dictionary<string, object>();
                        castProperties["Page"] = page;
                        castProperties["Application"] = Library.OMLApplication.Current;
                        Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_DetailsPageCastCrew", castProperties);
                    };

                    page.Commands.Add(command);
                }

                page.Watched = new BooleanChoice(this, "Watched");

                //is this the right way?
                // yes :)
                if (_titleObj.WatchedCount > 0)
                    page.Watched.Value = true;
                else
                    page.Watched.Value = false;

                page.Watched.ChosenChanged += new EventHandler(Watched_ChosenChanged);

                this.SetFanArtImage(page.Details, _titleObj);

                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties["Page"] = page;
                properties["Application"] = Library.OMLApplication.Current;

                if (page.Details.FanArt!=null)
                    Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_FanArtDetailsPage", properties);
                else
                    Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_DetailsPage", properties);
            };
        }

        void Watched_ChosenChanged(object sender, EventArgs e)
        {
            if (((BooleanChoice)sender).Value == true)
                this.IncrementPlayCount();
            else
                this.ClearWatchedCount();
        }

        private bool imagechkd = false;
        private Image m_defaultImage;
        public override Image DefaultImage
        {
            get
            {
                if (this.m_defaultImage == null && this.imagechkd == false)
                {
                    this.imagechkd = true;
                    this.SetDefaultImage();
                }
                return this.m_defaultImage;
            }
            set
            {
                if (this.m_defaultImage != value)
                {
                    this.m_defaultImage = value;
                    base.FirePropertyChanged("DefaultImage");
                }
            }
        }        

        private void SetDefaultImage()
        {            
            this.imagechkd = true;

            if (this._titleObj != null)
            {
                string imgPath = this._titleObj.FrontCoverMenuPath;
                if (!string.IsNullOrEmpty(imgPath))
                {                    
                    this.DefaultImage = new Image("file://" + imgPath);                    
                }
                else
                {
                    this._titleObj.BeginGetFrontCoverMenuPath(delegate
                    {
                        string path = _titleObj.FrontCoverMenuPath;

                        if (!string.IsNullOrEmpty(path))
                        {
                            SetImage(path);
                            Microsoft.MediaCenter.UI.Application.DeferredInvoke(CacheImageDone, null);
                        }                        
                    });
                }

            }
        }

        private void SetFanArtImage(ExtendedDetails details, Title title)
        {
            if (details.FanArtImages != null)
                return;

            if (title.FanArtPaths == null || title.FanArtPaths.Count == 0)
                return;

            details.FanArtImages = new List<Image>();

            foreach (string path in title.FanArtPaths)
            {
                details.FanArtImages.Add(new Image("file://" + path));
            }

            // set the first image
            details.FanArt = details.FanArtImages[0];
        }            

        void actorCommand_Invoked(object sender, EventArgs e)
        {
            Library.Code.V3.CastCommand actor=(Library.Code.V3.CastCommand)sender;
            List<TitleFilter> filter = new List<TitleFilter>();
            filter.Add(new TitleFilter(TitleFilterType.Person, actor.Description));
            IEnumerable<Title> items = TitleCollectionManager.GetFilteredTitles(filter);
            GalleryPage page = new GalleryPage(items, actor.Description);
        }

        void playCmd_Invoked(object sender, EventArgs e)
        {
            if (NeedsMounting(this.TitleObject.Disks[0].Format))
            {
                this.TitleObject.SelectedDisk = this.TitleObject.Disks[0];
                OMLApplication.Current.parentalControlManager.PlayMovie(this);
                //this.PlayMovie();
            }
            else
            {
                OMLApplication.Current.parentalControlManager.PlayAllDisks(this);
                //this.PlayAllDisks();
            }
        }

        //this is a temp hack - we need to handle this alot better!
        static bool NeedsMounting(VideoFormat videoFormat)
        {
            switch (videoFormat)
            {
                case VideoFormat.BIN:
                case VideoFormat.CUE:
                case VideoFormat.IMG:
                case VideoFormat.ISO:
                case VideoFormat.MDF:
                case VideoFormat.NRG:
                case VideoFormat.VOB:
                case VideoFormat.DVD:
                case VideoFormat.DVRMS:
                case VideoFormat.WTV:
                    return true;
                default:
                    return false;
            }
        }

        private static void AppendSeparatedValue(ref string text, string value, string seperator)
        {
            if (String.IsNullOrEmpty(text))
                text = value;
            else
                text = string.Format("{0}{1}{2}", text, seperator, value);
        }

        private void IncrementPlayCount()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                TitleCollectionManager.IncrementWatchedCount(this.TitleObject);
            });
        }

        private void ClearWatchedCount()
        {
            OMLApplication.ExecuteSafe(delegate
            {
                TitleCollectionManager.ClearWatchedCount(this.TitleObject);
            });
        }

        /// <summary>
        /// Plays the movie.
        /// </summary>
        public void PlayMovie()
        {
            IncrementPlayCount();

            var ms = new MediaSource(this.TitleObject.SelectedDisk);
            ms.OnSave += new Action<MediaSource>(ms_OnSave);
            IPlayMovie moviePlayer = MoviePlayerFactory.CreateMoviePlayer(ms);
            moviePlayer.PlayMovie();
        }

        public void PlayAllDisks()
        {
            //IncrementPlayCount();

            // create a disk out of the playlist of all items
            if (OMLApplication.Current.IsExtender)
                this.TitleObject.SelectedDisk = new Disk(this.Description, CreatePlayListFromAllDisks(), VideoFormat.WPL);
            else
                this.TitleObject.SelectedDisk = new Disk(this.Description, CreatePlayListFromAllDisks(), VideoFormat.WVX);

            PlayMovie();
        }

        private string CreatePlayListFromAllDisks()
        {
            if (!Directory.Exists(FileSystemWalker.TempPlayListDirectory))
                FileSystemWalker.createTempPlayListDirectory();

            WVXManager playlist = new WVXManager();

            string playlistFile = Path.Combine(FileSystemWalker.TempPlayListDirectory, "AllDisks_" + this.TitleObject.Id + ".WVX");

            foreach (Disk disk in this.TitleObject.Disks)
            {
                if (string.IsNullOrEmpty(disk.Path))
                    continue;

                playlist.AddItem(new PlayListItem(disk.Path, disk.Name));
            }

            playlist.WriteWVXFile(playlistFile);

            return playlistFile;
        }

        void ms_OnSave(MediaSource ms)
        {
            foreach (Disk d in this.TitleObject.Disks)
                if (d == ms.Disk)
                {
                    d.ExtraOptions = ms.UpdateExtraOptions(d.ExtraOptions);
                    //OMLApplication.Current.SaveTitles();
                    TitleCollectionManager.SaveTitleUpdates();
                    break;
                }
        }
    }
}
