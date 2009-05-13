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
        private static BackgroundProcessor<MovieItem> imageProcessor = new BackgroundProcessor<MovieItem>(4, MovieItem.ProcessorCallback, "ImageCacher");

        private static void ProcessorCallback(MovieItem movieitem)
        {
            movieitem.SetImage(movieitem.TitleObject.GetFrontCoverMenuPathSlow());
            Microsoft.MediaCenter.UI.Application.DeferredInvoke(movieitem.CacheImageDone, null);
        }
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

        private OMLEngine.Title _titleObj;
        public OMLEngine.Title TitleObject { get { return _titleObj; } }

        public MovieItem(Title title, IModelItem owner)
            : base(owner)
        {

            //this needs to be backed out- should only reference the title object
            this._titleObj = title;
            //this.InternalMovieItem = new Library.MovieItem(title, null);
            this.ItemType = 0;

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
            this.SimpleVideoFormat = _titleObj.VideoFormat.ToString();


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

                //this.SetFanArtImage(page.Details, this.InternalMovieItem);

                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties["Page"] = page;
                properties["Application"] = Library.OMLApplication.Current;

                if (page.Details.FanArt!=null)
                    Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_FanArtDetailsPage", properties);
                else
                    Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_DetailsPage", properties);
            };
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
            //if (!string.IsNullOrEmpty(this.InternalMovieItem.TitleObject.FrontCoverMenuPath) && File.Exists(this.InternalMovieItem.TitleObject.FrontCoverMenuPath))
            //{
            //    this.DefaultImage = new Image("file://" + this.InternalMovieItem.TitleObject.FrontCoverMenuPath);
            //}
            //return;
            this.imagechkd = true;
            if (this._titleObj != null)
            {
                string imgPath = this._titleObj.GetFrontCoverMenuPathFast();
                if (!string.IsNullOrEmpty(imgPath))
                {
                    if (File.Exists(imgPath))
                    {
                        this.DefaultImage = new Image("file://" + imgPath);
                    }
                    else
                    {
                        imageProcessor.Enqueue(this);
                        //imageProcessor.Inject(this);
                        //need to queue it!
                        //Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread(
                        //    delegate
                        //    {
                        //        System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
                        //        imgPath=this.InternalMovieItem.TitleObject.GetFrontCoverMenuPathSlow();
                        //        if (File.Exists(imgPath))
                        //        {
                        //            this.m_defaultImage = new Image("file://" + imgPath);
                        //        }
                        //    },
                        //    delegate 
                        //    {
                        //        if (this.m_defaultImage!= null)
                        //        {
                        //            FirePropertyChanged("DefaultImage");
                        //        }
                        //    },
                        ////    null);

                    }
                }
            }
        }

        //private void SetFanArtImage(ExtendedDetails details, Library.MovieItem internalMovie)
        //{
            //// only setup the background images once
            //if (details.FanArtImages != null)
            //    return;

            //details.FanArtImages = new List<Image>(1);

            //if (!string.IsNullOrEmpty(internalMovie.TitleObject.BackDropImage))
            //{
            //    if (File.Exists(Path.GetFullPath(internalMovie.TitleObject.BackDropImage)))
            //    {
            //        details.FanArtImages.Add(new Image(
            //            string.Format("file://{0}", Path.GetFullPath(internalMovie.TitleObject.BackDropImage))));

            //        // set the background image
            //        details.FanArt = details.FanArtImages[0];
                    
            //        return;
            //    }                
            //}

            //// a specific file was NOT found, time to go hunting


            //// if the /FanArt folder doesn't exist - that means no background images
            //string fanArtSrcDir = internalMovie.TitleObject.BackDropFolder;
            //if (string.IsNullOrEmpty(fanArtSrcDir)) //|| !Directory.Exists(fanArtSrcDir))
            //        return;

            //foreach (string file in
            //    Directory.GetFiles(fanArtSrcDir, "*.jpg", SearchOption.TopDirectoryOnly))
            //{
            //    OMLApplication.DebugLine("[MovieDetailsPage] loading fanart image {0}", file);

            //    details.FanArtImages.Add(new Image(string.Format("file://{0}", file)));
            //}

            //// oops - no images found in the fanart folder
            //if (details.FanArtImages.Count == 0)
            //    return;

            //// set the background image
            //details.FanArt = details.FanArtImages[0];            
        //}

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
            this.PlayAllDisks();
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
            IncrementPlayCount();

            // create a disk out of the playlist of all items
            this.TitleObject.SelectedDisk = new Disk(this.Description, CreatePlayListFromAllDisks(), VideoFormat.WPL);

            PlayMovie();
        }

        private string CreatePlayListFromAllDisks()
        {
            if (!Directory.Exists(FileSystemWalker.TempPlayListDirectory))
                FileSystemWalker.createTempPlayListDirectory();

            WindowsPlayListManager playlist = new WindowsPlayListManager();

            string playlistFile = Path.Combine(FileSystemWalker.TempPlayListDirectory, "AllDisks_" + this.TitleObject.Id + ".WPL");

            foreach (Disk disk in this.TitleObject.Disks)
            {
                if (string.IsNullOrEmpty(disk.Path))
                    continue;

                playlist.AddItem(new PlayListItem(disk.Path));
            }

            playlist.WriteWPLFile(playlistFile);

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
