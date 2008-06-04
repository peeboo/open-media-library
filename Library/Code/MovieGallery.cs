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

            Utilities.DebugLine("MovieGallery.JumpToMovie: {0}", jumpString);
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
            Filters[Filter.UserRating].AddMovie(((double)title.UserStarRating/10).ToString("0.0"), movie);
            
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
                    item.MenuCoverArt = GalleryItem.LoadImage(item.TitleObject.FrontCoverMenuPath);
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

    public class GalleryView
    {
        public const string List = "List";
        public const string MenuCoverArt = "Menu Cover Art";
        public const string CoverArt = "Cover Art";
    }
}