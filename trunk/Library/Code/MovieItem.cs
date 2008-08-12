using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.Collections;
using OMLEngine;
using System.Diagnostics;

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
            GalleryItem galleryItem = (GalleryItem)sender;
            if (Gallery != null) OMLApplication.Current.GoToMenu( Category.CreateGallery(galleryItem.Name) );
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

    }

    /// <summary>
    /// Represents a Movie item which can be displayed in the gallery
    /// </summary>
    public class MovieItem : GalleryItem
    {
        private Title _titleObj;
        private List<string> _actingRoles;
        private List<Disk> _friendlyNamedDisks = new List<Disk>();

        public Disk SelectedDisk
        {
            get { return _titleObj.SelectedDisk; }
            set { _titleObj.SelectedDisk = value; }
        }

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

            if (_titleObj.Runtime > 0)
                SubCaption += "Runtime: " + Convert.ToString(_titleObj.Runtime) + " minutes\n";

            if (_titleObj.UserStarRating > 0)
                SubCaption += "User Rating: " + ((double)(_titleObj.UserStarRating / 10)).ToString("0.0") + "\n";

            SubCaption += _titleObj.ParentalRating + "\n";


            if( _titleObj.Directors.Count > 0 )
                SubCaption += "Directed by: " + ((Person)_titleObj.Directors[0]).full_name + "\n";

            int actorCount = 0;
            if (_titleObj.ActingRoles.Count > 0)
            {
                SubCaption += "Actors: ";
            }

            foreach (KeyValuePair<string,string> actor in _titleObj.ActingRoles)
            {
                if (actor.Key.Trim().Length > 0)
                {
                    if (actorCount > 0)
                        SubCaption += ", ";


                    SubCaption += actor.Key.Trim();
                    actorCount++;

                    if (actorCount == 6)
                    {
                        SubCaption += "...";
                        break;
                    }
                }
            }


            if (_titleObj.ActingRoles.Count > 0)
            {
                SubCaption += "\n";
            }


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

            foreach (Disk d in title.Disks)
            {
                if (title.Disks.Count == 1)
                    _friendlyNamedDisks.Add(new Disk("Play Movie", d.Path, d.Format));
                else
                    _friendlyNamedDisks.Add(new Disk("Play " + d.Name, d.Path, d.Format));
            }
        }

        /// <summary>
        /// A callback that gets called when the movie is selected
        /// </summary>
        /// <param name="sender">The sender is a MovieItem.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void ItemSelected(object sender, EventArgs args)
        {
            MovieItem galleryItem = (MovieItem)sender;

            // Navigate to a details page for this item.
            MovieDetailsPage page = CreateDetailsPage(galleryItem);
            Gallery.JumpInListText.Value = "";
            Gallery.ClearJumpValue = true;
            OMLApplication.Current.GoToDetails(page);
        }

        /// <summary>
        /// Creates the details page for this movie
        /// </summary>
        /// <param name="item">The movie item.</param>
        /// <returns></returns>
        public MovieDetailsPage CreateDetailsPage(MovieItem item)
        {
            OMLApplication.DebugLine("[MovieItem] Creating a detailspage");
            MovieDetailsPage page = new MovieDetailsPage(item);
            return page;
        }
     
        

        /// <summary>
        /// Plays the movie.
        /// </summary>
        public void PlayMovie()
        {
            IPlayMovie moviePlayer = MoviePlayerFactory.CreateMoviePlayer(this);
            moviePlayer.PlayMovie();
        }

        /// <summary>
        /// Gets the Title object.
        /// </summary>
        /// <value>The title object.</value>
        public Title TitleObject
        {
            get { return _titleObj; }
        }

        

        /// <summary>
        /// Gets or sets the file location.
        /// </summary>
        /// <value>The file location.</value>
        //public string FileLocation
        //{
        //    get { return _titleObj.SelectedDisk.Path; }
        //    set { _titleObj.SelectedDisk.Path = value; }
        //}

        public List<Disk> Disks
        {
            get { return _titleObj.Disks; }
            set { _titleObj.Disks = value; }
        }

        public List<Disk> FriendlyNamedDisks
        {
            get { return _friendlyNamedDisks; }
            set { _friendlyNamedDisks = value; }
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

        ///// <summary>
        ///// Gets or sets the name.
        ///// </summary>
        ///// <value>The name.</value>
        //override public string Name
        //{
        //    get { return _titleObj.Name; }
        //    set { _titleObj.Name = value; }
        //}

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
    }



}
