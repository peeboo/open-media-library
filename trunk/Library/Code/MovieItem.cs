using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.MediaCenter.UI;

using OMLEngine;

namespace Library
{
    /// <summary>
    /// Represents an item in a Gallery (a movie item, actor, etc)
    /// </summary>
    public class GalleryItem : Command, IComparable
    {        
        /// <summary>
        /// Default Image for no cover
        /// </summary>
        public static Image NoCoverImage = new Image("resx://Library/Library.Resources/nocover");
        private bool _loadedCover = false;

        public bool LoadedCover
        {
            get { return _loadedCover; }
            set { _loadedCover = value; }
        }

        public Filter Category
        {
            get { return _category; }
            set { _category = value; }
        }

        virtual public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public bool HasCover
        {
            get 
            {
                bool hasCover = (_MenuCoverArt != NoCoverImage);
                return hasCover;
            }
        }

        public Image MenuCoverArt
        {
            get { return _MenuCoverArt; }
            set
            {
                _MenuCoverArt = value;
                FirePropertyChanged("MenuCoverArt");
            }
        }



        virtual public string SortName
        {
            get { return _sortName; }
            set { _sortName = value; }
        }

        public MovieGallery Gallery
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public Command QuickPlay
        {
            get { return _quickPlay; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GalleryItem"/> class.
        /// </summary>
        public GalleryItem(MovieGallery owner, string name, string caption, Filter browseCategory)
            : base(owner)
        {
            _name = name;
            _sortName = name;
            _caption = caption;
            _owner = owner;
            _category = browseCategory;
            MenuCoverArt = NoCoverImage; // default to NoCoverImage
            Invoked += ItemSelected;

            _quickPlay = new Command();
            _quickPlay.Invoked += new EventHandler(QuickPlayClicked);
        }

        /// <summary>
        /// Handles the play or plause/pause button when click from the top level menu to immediately
        /// play disk 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuickPlayClicked(object sender, EventArgs e)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                // quick play is only allowed if we're a movie item
                // this needs a cast here because in MCML movieitems are used
                // as gallery items
                MovieItem movie = this as MovieItem;

                // make sure we have a disk to quick play
                if (movie != null &&
                    movie.Disks != null &&
                    movie.Disks.Count > 0 &&
                    movie.TitleObject != null)
                {
                    // play the first disk
                    movie.TitleObject.SelectedDisk = movie.Disks[0];
                    movie.PlayMovie();
                }
            });
        }

        virtual public int ItemCount
        {
            get
            {
                if (Gallery != null && Category != null && Category.ItemMovieRelation.ContainsKey(Name))
                {
                    return Category.ItemMovieRelation[Name].Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        virtual public string Caption
        {
            get 
            {
                int itemCount = ItemCount;
                if (itemCount > 0 )
                {
                    return Name + " (" + Convert.ToString( itemCount ) + ")";
                }
                else
                {
                    return Name;
                }
                
            }
            set { _caption = value; }
        }

        /// <summary>
        /// Gets the item sub caption.
        /// </summary>
        /// <value>The item sub caption.</value>
        virtual public string SubCaption
        {
            get { return _subCaption; }
            set { _subCaption = value; }
        }

        /// <summary>
        /// Gets the item details.
        /// </summary>
        /// <value>The item details.</value>
        virtual public string Details
        {
            get { return _details; }
            set { _details = value; }
        }

        /// <summary>
        /// Loads the specified image.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <returns></returns>
        public static Image LoadImage(string imageName)
        {
            if (File.Exists(imageName))
            {
                return new Image("file://" + imageName);
            }
            else
            {
                return new Image("resx://Library/Library.Resources/nocover");
            }
        }

        /// <summary>
        /// A callback that gets called when the item is selected
        /// </summary>
        /// <param name="sender">The sender is a MovieItem.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public virtual void ItemSelected(object sender, EventArgs args)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                GalleryItem galleryItem = (GalleryItem)sender;

                if (Gallery != null)
                {
                    if (galleryItem.Name == Filter.AllItems)
                    {
                        OMLApplication.Current.GoToMenu(Gallery);
                    }
                    else
                    {
                        OMLApplication.Current.GoToMenu(Category.CreateGallery(galleryItem.Name));
                    }
                }
            });
        }

        public virtual GalleryItem Clone(MovieGallery newOwner)
        {
            return new GalleryItem(newOwner, Name, Caption, Category);
        }

        public int CompareTo(object obj)
        {
            if (obj is GalleryItem)
            {

                GalleryItem item = (GalleryItem)obj;
                return _name.CompareTo(item._name);
            }
            else
            {
                throw new ArgumentException("object is not a GalleryItem");
            }
        }

        private string _name;
        private string _sortName;
        private string _caption;
        private string _subCaption;
        private string _details;
        private Image _MenuCoverArt;
        private MovieGallery _owner;
        private Filter _category;
        private Command _quickPlay;
    }

    /// <summary>
    /// Represents a Movie item which can be displayed in the gallery
    /// </summary>
    public class MovieItem : GalleryItem
    {
        private Title _titleObj;
        private List<string> _actingRoles;
        private List<Disk> _friendlyNamedDisks;

        public List<string> ActingRoles
        {
            get { return _actingRoles; }
            set { _actingRoles = value; }
        }

        public override GalleryItem Clone(MovieGallery newOwner)
        {
            MovieItem m = new MovieItem(_titleObj, newOwner);
            return m;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieItem"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        public MovieItem(Title title, MovieGallery owner)
            : base(owner, title.Name, title.Name, null)
        {
            _titleObj = title;
            SortName = title.SortName;
            SubCaption = "";
            if (_titleObj.Runtime > 0)
                SubCaption += Convert.ToString(_titleObj.Runtime) + " minutes";

            if (_titleObj.UserStarRating > 0)
            {
                if (SubCaption.Length > 0) SubCaption += " / ";
                SubCaption += ((double)(_titleObj.UserStarRating / 10)).ToString("0.0");
            }

//            if (_titleObj.Directors.Count > 0 && ((Person)_titleObj.Directors[0]).full_name.Trim().Length > 0)
//            {
//                if (SubCaption.Length > 0) SubCaption += " / ";
//                SubCaption += "Directed by: " + ((Person)_titleObj.Directors[0]).full_name;
//            }

            if (_titleObj.ParentalRating.Trim().Length > 0)
            {
                if (SubCaption.Length > 0) SubCaption += " / ";
                SubCaption += _titleObj.ParentalRating;
            }

            int genreCount = 0;
            if (_titleObj.Genres.Count > 0)
            {
                foreach (string g in _titleObj.Genres)
                {
                    if (SubCaption.Length > 0) SubCaption += " / ";
                    SubCaption += g;
                    genreCount++;
                    if (genreCount > 3) break;
                }
            }

            //int actorCount = 0;
            //if (_titleObj.ActingRoles.Count > 0)
            //{
            //    SubCaption += "Actors: ";
            //}

            //foreach (KeyValuePair<string,string> actor in _titleObj.ActingRoles)
            //{
            //    if (actor.Key.Trim().Length > 0)
            //    {
            //        if (actorCount > 0)
            //            SubCaption += ", ";

            //        SubCaption += actor.Key.Trim();
            //        actorCount++;

            //        if (actorCount == 6)
            //        {
            //            SubCaption += "...";
            //            break;
            //        }
            //    }
            //}


            //if (_titleObj.ActingRoles.Count > 0)
            //{
            //    SubCaption += "\n";
            //}

            Details = _titleObj.Synopsis;
            _actingRoles = new List<string>();

            if (title.ActingRoles.Count > 0)
            {

                foreach (KeyValuePair<string, string> kvp in title.ActingRoles)
                {
                    if (kvp.Value.Trim().Length > 0)
                        _actingRoles.Add(kvp.Key + " as " + kvp.Value);
                    else
                        _actingRoles.Add(kvp.Key);
                }
            }
        }

        /// <summary>
        /// A callback that gets called when the movie is selected
        /// </summary>
        /// <param name="sender">The sender is a MovieItem.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void ItemSelected(object sender, EventArgs args)
        {            
            OMLApplication.ExecuteSafe(delegate
            {
                MovieItem galleryItem = (MovieItem)sender;

                // Navigate to a details page for this item.
                MovieDetailsPage page = CreateDetailsPage(galleryItem);
                Gallery.JumpInListText.Value = "";
                Gallery.ClearJumpValue = true;
                OMLApplication.Current.GoToDetails(page);
            });
        }

        /// <summary>
        /// Creates the details page for this movie
        /// </summary>
        /// <param name="item">The movie item.</param>
        /// <returns></returns>
        public MovieDetailsPage CreateDetailsPage(MovieItem item)
        {
            OMLApplication.DebugLine("[MovieItem] Creating a detailspage for {0}", item);
            return new MovieDetailsPage(item);
        }

        /// <summary>
        /// Plays the movie.
        /// </summary>
        public void PlayMovie()
        {
            var ms = new MediaSource(this.TitleObject.SelectedDisk);
            ms.OnSave += new Action<MediaSource>(ms_OnSave);
            IPlayMovie moviePlayer = MoviePlayerFactory.CreateMoviePlayer(ms);
            moviePlayer.PlayMovie();
        }

        void ms_OnSave(MediaSource ms)
        {
            foreach (Disk d in this.TitleObject.Disks)
                if (d == ms.Disk)
                {
                    d.ExtraOptions = ms.UpdateExtraOptions(d.ExtraOptions);
                    OMLApplication.Current.SaveTitles();
                    break;
                }
        }

        /// <summary>
        /// Gets the Title object.
        /// </summary>
        /// <value>The title object.</value>
        public Title TitleObject { get { return _titleObj; } }

        public List<Disk> Disks
        {
            get { return _titleObj.Disks; }
            set { _titleObj.Disks = value; }
        }

        public List<Disk> FriendlyNamedDisks
        {
            get
            {
                if (_friendlyNamedDisks == null)
                {
                    _friendlyNamedDisks = new List<Disk>();
                    List<Disk> disks = this._titleObj.Disks;
                    
                    if (OMLApplication.Current.IsExtender)
                        for (int i = 0; i < disks.Count; ++i)
                            foreach (MediaSource ms in (MediaSource.GetSourcesFromOptions(disks[i].Path, disks[i].ExtraOptions, true)))
                                if (disks.Contains(ms.Disk) == false)
                                    disks.Insert(i + 1, ms.Disk);

                    foreach (Disk d in disks)
                    {
                        var ms = new MediaSource(d);
                        string name;
                        if (disks.Count == 1)
                            name = ms.ResumeTime != null ? "Resume Movie" : "Play Movie";
                        else if (d.DVDDiskInfo != null && new MediaSource(d).DVDTitle.Main == false)
                            name = (ms.ResumeTime != null ? "~> " : "-> ") + d.Name;
                        else
                            name = (ms.ResumeTime != null ? "Resume " : "Play ") + d.Name;

                        _friendlyNamedDisks.Add(new Disk(name, d.Path, d.Format, d.ExtraOptions));
                    }
                }
                return _friendlyNamedDisks;
            }
        }

        public int itemId
        {
            get { return _titleObj.InternalItemID; }
        }

        /// <summary>
        /// Gets the use star rating.
        /// </summary>
        /// <value>The use star rating.</value>
        public int UseStarRating
        {
            get { return _titleObj.UserStarRating; }
        }

        /// <summary>
        /// Gets or sets the runtime.
        /// </summary>
        /// <value>The runtime.</value>
        public string Runtime
        {
            get { return _titleObj.Runtime.ToString(); }
            set { _titleObj.Runtime = Convert.ToInt32(value); }
        }

        /// <summary>
        /// Gets or sets the rating (parental).
        /// </summary>
        /// <value>The rating.</value>
        public string Rating
        {
            get { return _titleObj.ParentalRating; }
            set { _titleObj.ParentalRating = value; }
        }

        /// <summary>
        /// Gets or sets the synopsis.
        /// </summary>
        /// <value>The synopsis.</value>
        public string Synopsis
        {
            get { return _titleObj.Synopsis; }
            set { _titleObj.Synopsis = value; }
        }

        /// <summary>
        /// Gets or sets the distributor.
        /// </summary>
        /// <value>The distributor.</value>
        public string Distributor
        {
            get { return _titleObj.Studio; }
            set { _titleObj.Studio = value; }
        }

        /// <summary>
        /// Gets or sets the country of origin.
        /// </summary>
        /// <value>The country of origin.</value>
        public string CountryOfOrigin
        {
            get { return _titleObj.CountryOfOrigin; }
            set { _titleObj.CountryOfOrigin = value; }
        }

        /// <summary>
        /// Gets or sets the official website.
        /// </summary>
        /// <value>The official website.</value>
        public string OfficialWebsite
        {
            get { return _titleObj.OfficialWebsiteURL; }
            set { _titleObj.OfficialWebsiteURL = value; }
        }

        /// <summary>
        /// Gets or sets the release date.
        /// </summary>
        /// <value>The release date.</value>
        public string ReleaseDate
        {
            get { return _titleObj.ReleaseDate.ToString("MMMM dd, yyyy"); }
        }

        /// <summary>
        /// Gets the actors.
        /// </summary>
        /// <value>The actors.</value>
        public IList Actors
        {
            get
            {
                List<string> actor_names = new List<string>();
                foreach (KeyValuePair<string,string> kvp  in _titleObj.ActingRoles)
                {
                    actor_names.Add(kvp.Key);
                }
                return actor_names;
            }
            //set { _titleObj.Actors = value; }
        }

        /// <summary>
        /// Gets the directors.
        /// </summary>
        /// <value>The directors.</value>
        public IList Directors
        {
            get
            {
                List<string> director_names = new List<string>();
                foreach (Person p in _titleObj.Directors)
                {
                    director_names.Add(p.full_name);
                }
                return director_names;
            }
            //set { _titleObj.Directors = value; }
        }

        /// <summary>
        /// Gets the producers.
        /// </summary>
        /// <value>The producers.</value>
        public IList Producers
        {
            get { return _titleObj.Producers; }
            //set { _titleObj.Producers = value; }
        }

        /// <summary>
        /// Gets the writers.
        /// </summary>
        /// <value>The writers.</value>
        public IList Writers
        {
            get { return _titleObj.Writers; }
            //set { _titleObj.Writers = value; }
        }

        public override string ToString()
        {
            return "MovieItem:" + this._titleObj;
        }
    }
}
