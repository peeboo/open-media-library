using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;

namespace Library
{
    /// <summary>
    /// CategoryCommand - handle changing the browsing category
    /// </summary>
    public class FilterCommand : Command
    {
        public FilterCommand(Filter filter)
            : base()
        {
            _filter = filter;
            Invoked += filter.OnFilterSelected;
        }

        public override string ToString() 
        {
            return Caption;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        public string Caption
        {
            get { return _filter.Name; }
        }

        private Filter _filter;

        public Filter Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

    }

    /// A list of movies (MovieItems)
    /// </summary>
    public class MovieGallery : ModelItem
    {
        #region Public Properties

        public Choice Categories
        {
            get { return _categoryChoice; }
            set { _categoryChoice = value; }
        }


        public Comparison<MovieItem> CurrentSort
        {
            get { return _currentSort; }
            set { _currentSort = value; }
        }

        /// <summary>
        /// A list of MovieItems used by the UI
        /// </summary>
        /// <value>The movies.</value>
        public VirtualList Movies
        {
            get { return _moviesVirtualList; }
        }

        public Dictionary<string, Filter> Filters
        {
            get { return _filters; }
        }



        public Size MenuImageSize
        {
            get { return _MenuImageSize; }
            set
            {
                _MenuImageSize = value;
                FirePropertyChanged("MenuImageSize");
            }
        }

        public int NumberOfMenuRows
        {
            get { return _NumberOfMenuRows; }
            set
            {
                _NumberOfMenuRows = value;
                FirePropertyChanged("NumberOfMenuRows");
            }
        }


        public EditableText JumpInListText
        {
            get { return _jumpInListText; }
            set { _jumpInListText = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Gets or sets the focused item in the gallery
        /// </summary>
        /// <value>The focused item.</value>
        public GalleryItem FocusedItem
        {
            get { return _focusedItem; }
            set
            {
                // for now just do it for movie items but we may need it to work for all items
                if (value.GetType() == typeof(MovieItem))
                {
                    _focusedItem = value;
                    FirePropertyChanged("FocusedItem");
                }
            }
        }
        #endregion

        #region Methods

        private void CreateSortLookup()
        {
            _sortFunctionLookup.Add("NameAscending", SortByNameAscending);
            _sortFunctionLookup.Add("NameDescending", SortByNameDescending);
            _sortFunctionLookup.Add("YearAscending", SortByYearAscending);
            _sortFunctionLookup.Add("YearDescending", SortByYearDescending);
            _sortFunctionLookup.Add("RuntimeAscending", SortByRuntimeAscending);
            _sortFunctionLookup.Add("RuntimeDescending", SortByRuntimeDescending);
            _sortFunctionLookup.Add("UserRatingAscending", SortByUserRatingAscending);
            _sortFunctionLookup.Add("UserRatingDescending", SortByUserRatingDescending);
            _sortFunctionLookup.Add("DateAddedAscending", SortByDateAddedAscending);
            _sortFunctionLookup.Add("DateAddedAscending", SortByDateAddedDescending);
        }

        private void CreateVirtualList()
        {
            _moviesVirtualList = new VirtualList(this, null);
            
            foreach (MovieItem m in _movies)
            {
                _moviesVirtualList.Add(m);
            }
            _moviesVirtualList.EnableSlowDataRequests = true;
            _moviesVirtualList.RequestSlowDataHandler = new RequestSlowDataHandler(CompleteGalleryItem);
        }

        public void SortMovies()
        {
            _movies.Sort( _currentSort );
            CreateVirtualList();
        }

        private static int SortByNameAscending(MovieItem m1, MovieItem m2)
        {
            return m1.SortName.CompareTo(m2.SortName);
        }

        private static int SortByNameDescending(MovieItem m1, MovieItem m2)
        {
            return -SortByNameAscending(m1, m2);
        }

        private static int SortByYearAscending(MovieItem m1, MovieItem m2)
        {
            int compare = m1.TitleObject.ReleaseDate.CompareTo(m2.TitleObject.ReleaseDate);
            if (compare == 0)
                return SortByNameAscending(m1, m2);
            else
                return compare;
        }

        private static int SortByYearDescending(MovieItem m1, MovieItem m2)
        {
            int compare = m1.TitleObject.ReleaseDate.CompareTo(m2.TitleObject.ReleaseDate);
            if (compare == 0)
                return SortByNameAscending(m1, m2);
            else
                return -compare;
        }

        private static int SortByRuntimeAscending(MovieItem m1, MovieItem m2)
        {
            if (m1.TitleObject.Runtime > m2.TitleObject.Runtime)
                return 1;
            else if (m1.TitleObject.Runtime < m2.TitleObject.Runtime)
                return -1;
            else
                return SortByNameAscending(m1, m2);
        }

        private static int SortByRuntimeDescending(MovieItem m1, MovieItem m2)
        {
            if (m1.TitleObject.Runtime > m2.TitleObject.Runtime)
                return -1;
            else if (m1.TitleObject.Runtime < m2.TitleObject.Runtime)
                return 1;
            else
                return SortByNameAscending(m1, m2);
        }

        private static int SortByUserRatingAscending(MovieItem m1, MovieItem m2)
        {
            if (m1.TitleObject.UserStarRating > m2.TitleObject.UserStarRating)
                return 1;
            else if (m1.TitleObject.UserStarRating < m2.TitleObject.UserStarRating)
                return -1;
            else
                return SortByNameAscending(m1, m2);
        }

        private static int SortByUserRatingDescending(MovieItem m1, MovieItem m2)
        {
            if (m1.TitleObject.UserStarRating > m2.TitleObject.UserStarRating)
                return -1;
            else if (m1.TitleObject.UserStarRating < m2.TitleObject.UserStarRating)
                return 1;
            else
                return SortByNameAscending(m1, m2);
        }

        private static int SortByDateAddedAscending(MovieItem m1, MovieItem m2)
        {
            int compare = m1.TitleObject.DateAdded.CompareTo(m2.TitleObject.DateAdded);
            if (compare == 0)
                return SortByNameAscending(m1, m2);
            else
                return compare;
        }

        private static int SortByDateAddedDescending(MovieItem m1, MovieItem m2)
        {
            int compare = m1.TitleObject.DateAdded.CompareTo(m2.TitleObject.DateAdded);
            if (compare == 0)
                return SortByNameAscending(m1, m2);
            else
                return -compare;
        }

        #endregion

        #region Construction
        public MovieGallery(string title)
        {
            _title = title;
            OMLApplication.DebugLine("[MovieGallery] MovieGallery: Title [{0}]", Title);
            Initialize(null);
        }

        public MovieGallery(TitleCollection col, string title)
        {
            _title = title;
            OMLApplication.DebugLine("[MovieGallery] MovieGallery: Title [{0}]", Title);
            Initialize(col);
        }

        private void CreateCategories()
        {
            _categories.Add(new FilterCommand(Filters[Filter.Genres]));
            _categories.Add(new FilterCommand(Filters[Filter.Director]));
            _categories.Add(new FilterCommand(Filters[Filter.Actor]));
            _categories.Add(new FilterCommand(Filters[Filter.DateAdded]));
            _categories.Add(new FilterCommand(Filters[Filter.Year]));
            _categories.Add(new FilterCommand(Filters[Filter.Runtime]));
            _categories.Add(new FilterCommand(Filters[Filter.UserRating]));
            _categoryChoice = new Choice(this, "Categories", _categories);
        }

        private void Initialize(TitleCollection col)
        {
            _filters.Add(Filter.Actor, new Filter(Filter.Actor,this, Properties.Settings.Default.ActorView, true, Properties.Settings.Default.ActorSort));
            _filters.Add(Filter.Director, new Filter(Filter.Director, this, Properties.Settings.Default.DirectorView, true, Properties.Settings.Default.DirectorSort));
            _filters.Add(Filter.Genres, new Filter(Filter.Genres, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.GenreSort));
            _filters.Add(Filter.Year, new Filter(Filter.Year, this, Properties.Settings.Default.YearView, true, Properties.Settings.Default.YearSort));
            _filters.Add(Filter.DateAdded, new Filter(Filter.DateAdded, this, Properties.Settings.Default.DateAddedView, true, Properties.Settings.Default.DateAddedSort));
            _filters.Add(Filter.Runtime, new Filter(Filter.Runtime, this, GalleryView.List, false, String.Empty));
            _filters.Add(Filter.UserRating, new Filter(Filter.UserRating, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.UserRatingSort));

            _jumpInListText = new EditableText(this);
            _jumpInListText.Value = String.Empty;
            //_jumpInListText.Activity += new EventHandler(JumpInListTextActivity);
            //_jumpInListText.Submitted += new EventHandler(JumpInListTextSubmitted);
            FocusedItem = new GalleryItem(this, "", "", null);
            _categoryChoice = new Choice(this, "Categories");
            CreateCategories();
            _moviesVirtualList = new VirtualList(this, null);

            if (_sortFunctionLookup.ContainsKey(Properties.Settings.Default.MovieSort))
                _currentSort = _sortFunctionLookup[Properties.Settings.Default.MovieSort];
            else
                _currentSort = SortByNameAscending;
            LoadMovies(col);
        }

        //public string JumpInText
        //{
        //    get { return _jumpInListText.Value; }
        //    set { _jumpInListText.Value = value; }
        //}

        //void JumpInListTextSubmitted(object sender, EventArgs e)
        //{
        //    EditableText t = (EditableText)sender;
        //    Utilities.DebugLine("MovieGallery.JumpInListTextSubmitted: {0}", t.Value);
        //    JumpToMovie(t.Value);
        //}

        //void JumpInListTextActivity(object sender, EventArgs e)
        //{
        //    EditableText t = (EditableText)sender;
        //    Utilities.DebugLine("MovieGallery.JumpInListTextActivity: {0}", t.Value);
        //    JumpToMovie(t.Value);
        //}

        private int _jumpToPosition = -1;           // the ScrollData jump is relative (Scroll # of items)
        private int _relativeJumpToPosition = -1;   // the Repeater jump (NavigateIntoIndex) is absolute

        public int RelativeJumpToPosition
        {
            get { return _relativeJumpToPosition; }
        }

        public int JumpToPosition
        {
            get { return _jumpToPosition; }
        }

        public void JumpToMovie(string jumpString)
        {
            if (jumpString.Length == 0) return;

            Utilities.DebugLine("MovieGallery.JumpToString: {0}", jumpString);
            foreach (MovieItem m in _moviesVirtualList)
            {
                if (m.Name.StartsWith(_jumpInListText.Value, true, null))
                {
                    int focusedItemIndex = _moviesVirtualList.IndexOf(FocusedItem);
                    if (focusedItemIndex < 0)
                    {
                        focusedItemIndex = 0;
                    }

                    _jumpToPosition = _moviesVirtualList.IndexOf(m);
                    _relativeJumpToPosition = _jumpToPosition - focusedItemIndex;

                    Utilities.DebugLine("MovieGallery.JumpToString: Found movie {0} pos {1} relpos {2}", m.Name, _jumpToPosition, _relativeJumpToPosition);
                    FirePropertyChanged("JumpToPosition");
                    break;
                }
            }
        }

        public void AddMovie(MovieItem movie)
        {
            //_moviesVirtualList.Add(movie);
            _movies.Add(movie);

            // update the filters
            Title title = movie.TitleObject;
            foreach (Person p in title.Directors)
            {
                Filters[Filter.Director].AddMovie(p.full_name, movie);
            }

            foreach (Person p in title.Actors)
            {
                Filters[Filter.Actor].AddMovie(p.full_name, movie);
            }

            foreach (string genre in title.Genres)
            {
                Filters[Filter.Genres].AddMovie(genre, movie);
            }

            Filters[Filter.Year].AddMovie(Convert.ToString(title.ReleaseDate.Year), movie);
            Filters[Filter.DateAdded].AddMovie(title.DateAdded.ToShortDateString(), movie);
            Filters[Filter.UserRating].AddMovie(Convert.ToString(title.UserStarRating), movie);

            AddRuntimeFilter(movie);
        }

        private void AddRuntimeFilter( MovieItem movie)
        {
            //TODO: make this configurable
            Filters[Filter.Runtime].AddItem("1 to 15 minutes");
            Filters[Filter.Runtime].AddItem("15 to 30 minutes");
            Filters[Filter.Runtime].AddItem("30 to 45 minutes");
            Filters[Filter.Runtime].AddItem("45 minutes to 1 hour");
            Filters[Filter.Runtime].AddItem("1 hour to 1.5 hours");
            Filters[Filter.Runtime].AddItem("1.5 hours to 2 hours");
            Filters[Filter.Runtime].AddItem("2 hours to 2.5 hours");
            Filters[Filter.Runtime].AddItem("2.5 hours to 3 hours");
            Filters[Filter.Runtime].AddItem("Over 3 hours");
            Filters[Filter.Runtime].AddItem("Unavailable");

            Title title = movie.TitleObject;

            if (title.Runtime < 1)
            {
                Filters[Filter.Runtime].AddMovie("Unavailable", movie);
            }
            else if (title.Runtime <= 15)
            {
                Filters[Filter.Runtime].AddMovie("1 to 15 minutes", movie);
            }
            else if (title.Runtime <= 30)
            {
                Filters[Filter.Runtime].AddMovie("16 to 30 minutes", movie);
            }
            else if (title.Runtime <= 45)
            {
                Filters[Filter.Runtime].AddMovie("30 to 45 minutes", movie);
            }
            else if (title.Runtime <= 60)
            {
                Filters[Filter.Runtime].AddMovie("45 minutes to 1 hour", movie);
            }
            else if (title.Runtime <= 90)
            {
                Filters[Filter.Runtime].AddMovie("1 hour to 1.5 hours", movie);
            }
            else if (title.Runtime <= 120)
            {
                Filters[Filter.Runtime].AddMovie("1.5 hours to 2 hours", movie);
            }
            else if (title.Runtime <= 150)
            {
                Filters[Filter.Runtime].AddMovie("2 hours to 2.5 hours", movie);
            }
            else if (title.Runtime <= 180)
            {
                Filters[Filter.Runtime].AddMovie("2.5 hours to 3 hours", movie);
            }
            else
            {
                Filters[Filter.Runtime].AddMovie("Over 3 hours", movie);
            }
        }

        private void LoadMovies(TitleCollection col)
        {
            _movies = new List<MovieItem>();

            if (col != null)
            {
                OMLApplication.DebugLine("[MovieGallery] LoadMovies: have TitleCollection");

                foreach (Title title in col)
                {
                    MovieItem movie = new MovieItem(title, this);
                    AddMovie(movie);
                }
            }

            SortMovies();
            if (Movies.Count > 0)
            {
                foreach (KeyValuePair<string, Filter> kvp in Filters)
                {
                    kvp.Value.Sort();
                }
                FocusedItem = (GalleryItem)Movies[0];
            }
            //OMLApplication.DebugLine("[MovieGallery] LoadMovies: done: directors {0} actors {1} genres {2} movies {3}", _directors.Count, _actors.Count, _genres.Count, _movies.Count);
        }

        /// <summary>
        /// Finishes any slow data for a gallery item.
        /// </summary>
        private void CompleteGalleryItem(VirtualList list, int index)
        {
            try
            {
                MovieItem item = (MovieItem)list[index];
                if (item.CoverArt == MovieItem.NoCoverImage)
                {
                    item.CoverArt = GalleryItem.LoadImage(item.TitleObject.FrontCoverPath);
                    item.BackCover = GalleryItem.LoadImage(item.TitleObject.BackCoverPath);
                }
            }
            catch
            {
                UISettings x = new UISettings();
            }
        }
        
        #endregion  

        #region Private Data

        private ArrayList _categories = new ArrayList();
        private Choice _categoryChoice;

        private Dictionary<string, Filter> _filters = new Dictionary<string,Filter>();
        private Dictionary<string, Comparison<MovieItem>> _sortFunctionLookup = new Dictionary<string, Comparison<MovieItem>>();

        private VirtualList _moviesVirtualList = new VirtualList(); // what we pass to the UI
        private List<MovieItem> _movies = new List<MovieItem>() ; // what we use internally for sorting

        delegate int MovieSort(MovieItem m1, MovieItem m2);
        private Comparison<MovieItem> _currentSort;

        private GalleryItem _focusedItem;
        private string _title;

        private int _NumberOfMenuRows = 2;
        private Size _MenuImageSize = new Size(150, 200);

        EditableText _jumpInListText;
 

        #endregion
   }

    public class UISettings
    {
        public UISettings() 
        { 
            _anchorSettings = new AnchorSettings();
            _gallerySettings = new GallerySettings();
        }

        public AnchorSettings Anchor
        {
            get { return _anchorSettings; }
        }

        public GallerySettings Gallery
        {
            get { return _gallerySettings; }
        }

        private AnchorSettings _anchorSettings;
        private GallerySettings _gallerySettings;

   }

    public class AnchorSettings
    {
        public float DetailsLeftAnchor
        {
            get { return Properties.Settings.Default.DetailsLeftAnchor; }
        }

        public float DetailsRightAnchor
        {
            get { return Properties.Settings.Default.DetailsLeftAnchor + 0.30f; }
        }

        public int DetailsLeftOffset
        {
            get { return Properties.Settings.Default.DetailsLeftOffset; }
        }

        public float DetailsTopAnchor
        {
            get { return Properties.Settings.Default.DetailsTopAnchor; }
        }

        public int DetailsTopOffset
        {
            get { return Properties.Settings.Default.DetailsTopOffset; }
        }

        public int FiltersTopOffset
        {
            get { return Properties.Settings.Default.DetailsTopOffset + 50; }
        }

        public float BottomAnchor
        {
            get { return Properties.Settings.Default.BottomAnchor; }
        }

        public int BottomOffset
        {
            get { return Properties.Settings.Default.BottomOffset; }
        }


        public float GalleryBottomAnchor
        {
            get { return Properties.Settings.Default.BottomAnchor - 0.05f; }
        }

        public int GalleryBottomOffset
        {
            get { return Properties.Settings.Default.BottomOffset - 15; }
        }
    }

    public class GallerySettings
    {
        public int CoverArtRows
        {
            get { return Properties.Settings.Default.GalleryCoverArtRows; }
        }

        public int ListRows
        {
            get { return Properties.Settings.Default.GalleryListRows; }
        }

        public Size CoverArtSize
        {
            get { return new Size( Properties.Settings.Default.CoverArtWidth, Properties.Settings.Default.CoverArtHeight); }
        }

        public Size ListItemSize
        {
            get { return new Size(Properties.Settings.Default.ListItemWidth, Properties.Settings.Default.ListItemHeight); }
        }

    }

    public class Filter
    {

        public const string Genres = "Genres";
        public const string Director = "Directors";
        public const string Actor = "Actors";
        public const string Runtime = "Runtime";
        //public const string Country = "Country";
        //public const string ParentRating = "Parental Rating";
        public const string UserRating = "User Rating";
        public const string Year = "Year";
        public const string DateAdded = "Date Added";
        public const string Home = "OML Home";
        //public const string Movies = "Movies";


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public List<GalleryItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public Dictionary<string,VirtualList> ItemMovieRelation
        {
            get { return _itemMovieRelation; }
            set { _itemMovieRelation = value; }
        }

        public string GalleryView
        {
            get { return _galleryView; }
            set { _galleryView = value; }
        }

        public Filter(string name, MovieGallery gallery, string galleryView, bool bSort, string sortFunction)
        {
            _allowSort = bSort;
            _name = name;
            _gallery = gallery;
            _galleryView = galleryView;
            _currentSort = SortByNameAscending;
            Initialize(sortFunction);
        }

        private void Initialize(string sortFunction)
        {
            _sortFunctionLookup.Add("NameAscending", SortByNameAscending);
            _sortFunctionLookup.Add("NameDescending", SortByNameDescending);
            _sortFunctionLookup.Add("CountAscending", SortByItemCountAscending);
            _sortFunctionLookup.Add("CountDescending", SortByItemCountDescending);
            if (sortFunction.Trim() != String.Empty)
            {
                if (_sortFunctionLookup.ContainsKey(sortFunction))
                    _currentSort = _sortFunctionLookup[sortFunction];
            }
        }

        /// <summary>
        /// Adds an empty item to our lists. Use this to create the items first in a certain order before adding the movies
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddItem(string item)
        {
            if (!_itemMovieRelation.ContainsKey(item))
            {
                VirtualList movies = new VirtualList(_gallery, null);
                _itemMovieRelation.Add(item, movies);
                _items.Add(new GalleryItem(_gallery, item, item, this));
            }
        }

        /// <summary>
        /// Adds the movie corresponding to the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="movie">The movie.</param>
        public void AddMovie(string item, MovieItem movie)
        {
            if (_itemMovieRelation.ContainsKey(item))
            {
                VirtualList movies = (VirtualList)_itemMovieRelation[item];
                movies.Add(movie);
            }
            else
            {
                VirtualList movies = new VirtualList(_gallery, null);
                movies.Add(movie);
                _itemMovieRelation.Add(item, movies);
                _items.Add( new GalleryItem(_gallery, item, item, this ));
            }
        }

        public void Sort()
        {
            if (_allowSort && _currentSort != null) _items.Sort(_currentSort);
        }

        public MovieGallery CreateGallery(string filter)
        {
            Trace.TraceInformation("MovieGallery.CreateFilteredCollection");
            MovieGallery movies = new MovieGallery(_gallery.Title + " > " + filter);
            if (_itemMovieRelation.ContainsKey(filter))
            {
                foreach (MovieItem movie in _itemMovieRelation[filter])
                {
                    MovieItem newMovie = (MovieItem)movie.Clone(movies);
                    movies.AddMovie(newMovie);
                }
            }
            movies.SortMovies();
            //Trace.TraceInformation("MovieGallery.CreateFilteredCollection: done: directors {0} actors {1} genres {2} movies {3}", movies._directors.Count, movies._actors.Count, movies._genres.Count, movies._movies.Count);
            foreach( KeyValuePair<string, Filter> kvp in movies.Filters )
            {
                kvp.Value.Sort();
            }
            return movies;
        }


        public void OnFilterSelected(object sender, EventArgs args)
        {
            Trace.TraceInformation("MovieGallery.OnFilterSelected");
            FilterCommand cmd = (FilterCommand)sender;
            OMLApplication.Current.GoToSelectionList(_gallery, Items, _gallery.Title + " > " + Name, _galleryView);
        }

        private static int SortByItemCountAscending(GalleryItem m1, GalleryItem m2)
        {
            if (m1.ItemCount > m2.ItemCount)
                return 1;
            else if (m1.ItemCount < m2.ItemCount)
                return -1;
            else
                return SortByNameAscending(m1, m2);
        }

        private static int SortByItemCountDescending(GalleryItem m1, GalleryItem m2)
        {
            if (m1.ItemCount > m2.ItemCount)
                return -1;
            else if (m1.ItemCount < m2.ItemCount)
                return 1;
            else
                return SortByNameAscending(m1, m2);
        }


        private static int SortByNameAscending(GalleryItem m1, GalleryItem m2)
        {
            return m1.Name.CompareTo(m2.Name);
        }

        private static int SortByNameDescending(GalleryItem m1, GalleryItem m2)
        {
            return m2.Name.CompareTo(m1.Name);
        }

        private MovieGallery _gallery;
        private string _name;

        private Dictionary<string, VirtualList> _itemMovieRelation = new Dictionary<string, VirtualList>();
        private List<GalleryItem> _items = new List<GalleryItem>();
        private string _galleryView;
        private Comparison<GalleryItem> _currentSort;
        private Dictionary<string, Comparison<GalleryItem>> _sortFunctionLookup = new Dictionary<string, Comparison<GalleryItem>>();
        private bool _allowSort = false;

    }


    public class GalleryView
    {
        public const string List = "List";
        public const string CoverArt = "Cover Art";
    }
}