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
            CoverArt = NoCoverImage;
            Invoked += ItemSelected;
        }

        /// <summary>
        /// Gets or sets the item image.
        /// </summary>
        /// <value>The item image.</value>
        public Image CoverArt
        {
            get { return _itemImage; }
            set 
            { 
                _itemImage = value;
                FirePropertyChanged("CoverArt");
            }
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
        private Image _itemImage;
        private MovieGallery _owner;
        private Filter _category;

    }

    /// <summary>
    /// Represents a Movie item which can be displayed in the gallery
    /// </summary>
    public class MovieItem : GalleryItem
    {
        private Title _titleObj;
        private Image _backCoverArtImage;
        private List<string> _actingRoles;

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
            _backCoverArtImage = NoCoverImage;
            CoverArt = NoCoverImage;
            if( _titleObj.Directors.Count > 0 )
                SubCaption = "Directed by: " + ((Person)_titleObj.Directors[0]).full_name + "\r\n";

            int actorCount = 0;
            if (_titleObj.Actors.Count > 0)
            {
                SubCaption += "Actors: ";
            }
            foreach (Person actor in _titleObj.Actors)
            {
                if (actor.full_name.Trim().Length > 0)
                {
                    if (actorCount > 0)
                        SubCaption += ", ";

                    SubCaption += actor.full_name;
                    actorCount++;
                    if (actorCount == 6)
                    {
                        SubCaption += " ...";
                        break;
                    }
                }
            }

            if (_titleObj.Actors.Count > 0)
            {
                SubCaption += "\r\n";
            }

            SubCaption += _titleObj.MPAARating + "\r\n";
            
            if (_titleObj.Runtime > 0)
                SubCaption += "Runtime: " + Convert.ToString(_titleObj.Runtime) + " minutes\r\n";
            
            if( _titleObj.UserStarRating > 0 )
                SubCaption += "User Rating: " + ((double)(_titleObj.UserStarRating/10)).ToString("0.0");

            Details = _titleObj.Synopsis;
            _actingRoles = new List<string>();

            if (title.ActingRoles.Count > 0)
            {

                foreach (KeyValuePair<string, string> kvp in title.ActingRoles)
                {
                    if (kvp.Value.Trim().Length > 0)
                        _actingRoles.Add(kvp.Key + " - " + kvp.Value);
                    else
                        _actingRoles.Add(kvp.Key);
                }
            }
            else
            {
                foreach (Person actor in title.Actors)
                {
                    _actingRoles.Add(actor.full_name);
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
            MovieItem galleryItem = (MovieItem)sender;

            // Navigate to a details page for this item.
            MovieDetailsPage page = CreateDetailsPage(galleryItem);
            OMLApplication.Current.GoToDetails(page);
        }

        /// <summary>
        /// Creates the details page for this movie
        /// </summary>
        /// <param name="item">The movie item.</param>
        /// <returns></returns>
        public MovieDetailsPage CreateDetailsPage(MovieItem item)
        {
            OMLApplication.DebugLine("Creating a detailspage");
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
        public string FileLocation
        {
            get { return _titleObj.FileLocation; }
            set { _titleObj.FileLocation = value; }
        }

        /// <summary>
        /// Gets or sets the front cover.
        /// </summary>
        /// <value>The front cover.</value>
        public Image FrontCover
        {
            get { return CoverArt; }
            set { CoverArt = value; }
        }

        /// <summary>
        /// Gets or sets the back cover.
        /// </summary>
        /// <value>The back cover.</value>
        public Image BackCover
        {
            get { return _backCoverArtImage; }
            set { _backCoverArtImage = value; }
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
            get { return _titleObj.MPAARating; }
            set { _titleObj.MPAARating = value; }
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
            get { return _titleObj.Distributor; }
            set { _titleObj.Distributor = value; }
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
            get { return _titleObj.ReleaseDate.ToShortDateString(); }
            //TODO
            set { }
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
                foreach (Person p in _titleObj.Actors)
                {
                    actor_names.Add(p.full_name);
                }
                return actor_names;
            }
            //set { _titleObj.Actors = value; }
        }

        /// <summary>
        /// Gets the crew.
        /// </summary>
        /// <value>The crew.</value>
        public IList Crew
        {
            get
            {
                List<string> crew_names = new List<string>();
                foreach (Person p in _titleObj.Crew)
                {
                    crew_names.Add(p.full_name);
                }
                return crew_names;
            }
            //set { _titleObj.Crew = value; }
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
