using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;
using System.Threading;
using System.Linq;
using System.Text;

namespace Library
{
    /// A list of movies (MovieItems)
    /// </summary>
    public class MovieGallery : ModelItem
    {
        #region Public Properties

        public List<LabeledList> LabeledLists
        {
            get
            {
                if (labeledLists == null)
                {                    
                    SetupAlphaCharacters();

                    Dictionary<char, List<Title>> alphaTitles = new Dictionary<char, List<Title>>();

                    // get all the valid titles and iterate through them - this is much
                    // faster then doing 26 queries to sql
                    IEnumerable<Title> allTitles = TitleCollectionManager.GetFilteredTitles(filters);

                    // build the list of valid alpha characters
                    foreach (Title title in allTitles)
                    {
                        List<Title> alphaTitle;

                        char firstChar = title.Name.ToUpperInvariant()[0];

                        if (((int)firstChar) < 65 || ((int)firstChar) > 90)
                            firstChar = '#';

                        if (alphaTitles.TryGetValue(firstChar, out alphaTitle))
                        {
                            alphaTitle.Add(title);
                        }
                        else
                        {
                            alphaTitles.Add(firstChar, new List<Title>() { title });
                        }
                    }

                    labeledLists = new List<LabeledList>(alphaTitles.Keys.Count);

                    // based off the list create a bunch of gallery objects
                    foreach (string alpha in alphaCharacters)
                    {
                        List<Title> alphaTitle;

                        if (alphaTitles.TryGetValue(alpha[0], out alphaTitle))
                            labeledLists.Add(new LabeledList(alpha, new MovieGallery(alphaTitle, string.Empty).Movies));
                    }

                    int index = 0;

                    // index all the items
                    foreach (LabeledList list in labeledLists)
                    {
                        foreach (MovieItem movie in list.Movies)
                        {
                            movie.GlobalIndex = index++;
                        }
                    }
                }

                return labeledLists;
            }
        }

        public int NumberOfPages
        {
            get { return numberOfPages; }
            set
            {
                numberOfPages = value;
                FirePropertyChanged("NumberOfPages");
            }
        }

        public List<string> AlphaCharacters { get { return alphaCharacters; } }

        public string JumpLetter
        {
            get { return jumpLetter; }
            set
            {
                this.jumpLetter = value;
                FirePropertyChanged("JumpLetter");
            }
        }

        public Int32 AngleDelta
        {
            get
            {
                if (Movies.Count == 0)
                    return 0;

                return -(Movies.Count / 360);

            }
            set
            {
                FirePropertyChanged("ListCount");
            }
        }

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

        private IntRangedValue _focusIndex = new IntRangedValue();

        public IntRangedValue FocusIndex
        {
            get { return _focusIndex; }
            set { _focusIndex = value; }
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
            _sortFunctionLookup.Add("Name Ascending", SortByNameAscending);
            _sortFunctionLookup.Add("Name Descending", SortByNameDescending);
            _sortFunctionLookup.Add("Year Ascending", SortByYearAscending);
            _sortFunctionLookup.Add("Year Descending", SortByYearDescending);
            _sortFunctionLookup.Add("Date Added Ascending", SortByDateAddedAscending);
            _sortFunctionLookup.Add("Date Added Descending", SortByDateAddedDescending);
            _sortFunctionLookup.Add("Runtime Ascending", SortByRuntimeAscending);
            _sortFunctionLookup.Add("Runtime Descending", SortByRuntimeDescending);
            _sortFunctionLookup.Add("User Rating Ascending", SortByUserRatingAscending);
            _sortFunctionLookup.Add("User Rating Descending", SortByUserRatingDescending);
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
            _movies.Sort(_currentSort);
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

        /// <summary>
        /// // creates a new movie gallery for the homepage listing all available titles
        /// </summary>
        public MovieGallery()
            : this(TitleCollectionManager.GetAllTitles(), Filter.Home)
        {
        }

        public MovieGallery(IEnumerable<Title> titles, string title)
        {
            _title = title;
            OMLApplication.DebugLine("[MovieGallery] MovieGallery: Title [{0}]", Title);
            Initialize(titles);
        }

        public MovieGallery(List<TitleFilter> filters)
        {
            this.filters = filters;

            if (filters != null && filters.Count != 0)
            {
                // create the title given the list of filters
                StringBuilder sb = new StringBuilder(filters.Count * 10);
                sb.Append(Filter.Home);

                foreach (TitleFilter filter in filters)
                {
                    sb.Append(" > ");
                    if (!string.IsNullOrEmpty(filter.FilterText))
                        sb.Append(filter.FilterText);
                    else
                        sb.Append(filter.FilterType.ToString());
                }

                _title = sb.ToString();
            }
            else
            {
                _title = Filter.Home;
            }

            Initialize(TitleCollectionManager.GetFilteredTitles(this.filters));
        }

        public MovieGallery(TitleFilter filter)
            : this(new List<TitleFilter>() { filter })
        {
        }

        private void CreateCategories()
        {            
            // home is first
            _categories.Add(new NavigationCommand(Filter.Home, delegate() { OMLApplication.Current.GoToMenu(new MovieGallery()); }));

            // then settings
            _categories.Add(new NavigationCommand(Filter.Settings, delegate() { OMLApplication.Current.GoToSettingsPage(new MovieGallery()); }));

            // add the trailers if they've requested to show them
            if ( Properties.Settings.Default.ShowFilterTrailers )
                _categories.Add(new NavigationCommand(Filter.Trailers, delegate() { OMLApplication.Current.GoToTrailersPage(); }));

            // add unwatched filter at the top            
            if (Properties.Settings.Default.ShowFilterUnwatched)
                //_categories.Add(new UnwatchedCommand(Filter.Unwatched));
                _categories.Add(new FilterCommand(Filters[Filter.Unwatched]));

            System.Collections.Specialized.StringCollection filtersToShow =
                Properties.Settings.Default.MainFiltersToShow;

            foreach (string filterName in filtersToShow)
            {
                if (Filters.ContainsKey(filterName))
                {
                    _categories.Add(new FilterCommand(Filters[filterName]));
                }
            }
            _categoryChoice = new Choice(this, "Categories", _categories);
        }

        private void Initialize(IEnumerable<Title> titles)
        {
            DateTime start = DateTime.Now;

            /*/
            if (Properties.Settings.Default.ShowFilterTrailers) _filters.Add(Filter.Trailers, new Filter(Filter.Trailers, this, Properties.Settings.Default.ActorView, true, Properties.Settings.Default.ActorSort));            
             */

            if (Properties.Settings.Default.ShowFilterUnwatched) _filters.Add(Filter.Unwatched, new Filter(Filter.Unwatched, this, Properties.Settings.Default.GenreView, Properties.Settings.Default.NameAscendingSort, TitleFilterType.Unwatched, filters));
            if (Properties.Settings.Default.ShowFilterActors) _filters.Add(Filter.Actor, new Filter(Filter.Actor, this, Properties.Settings.Default.ActorView, Properties.Settings.Default.ActorSort, TitleFilterType.Actor, filters));
            if (Properties.Settings.Default.ShowFilterDirectors) _filters.Add(Filter.Director, new Filter(Filter.Director, this, Properties.Settings.Default.DirectorView, Properties.Settings.Default.DirectorSort, TitleFilterType.Director, filters));
            if (Properties.Settings.Default.ShowFilterGenres) _filters.Add(Filter.Genres, new Filter(Filter.Genres, this, Properties.Settings.Default.GenreView, Properties.Settings.Default.GenreSort, TitleFilterType.Genre, filters));
            if (Properties.Settings.Default.ShowFilterYear) _filters.Add(Filter.Year, new Filter(Filter.Year, this, Properties.Settings.Default.YearView, Properties.Settings.Default.YearSort, TitleFilterType.Year, filters));
            if (Properties.Settings.Default.ShowFilterDateAdded) _filters.Add(Filter.DateAdded, new Filter(Filter.DateAdded, this, Properties.Settings.Default.DateAddedView, Properties.Settings.Default.DateAddedSort, TitleFilterType.DateAdded, filters));
            if (Properties.Settings.Default.ShowFilterRuntime) _filters.Add(Filter.Runtime, new Filter(Filter.Runtime, this, GalleryView.List, String.Empty, TitleFilterType.Runtime, filters));
            if (Properties.Settings.Default.ShowFilterUserRating) _filters.Add(Filter.UserRating, new Filter(Filter.UserRating, this, Properties.Settings.Default.GenreView, Properties.Settings.Default.UserRatingSort, TitleFilterType.UserRating, filters));
            if (Properties.Settings.Default.ShowFilterFormat) _filters.Add(Filter.VideoFormat, new Filter(Filter.VideoFormat, this, Properties.Settings.Default.GenreView, Properties.Settings.Default.NameAscendingSort, TitleFilterType.VideoFormat, filters));
            if (Properties.Settings.Default.ShowFilterParentalRating) _filters.Add(Filter.ParentRating, new Filter(Filter.ParentRating, this, Properties.Settings.Default.GenreView, Properties.Settings.Default.NameAscendingSort, TitleFilterType.ParentalRating, filters));
            if (Properties.Settings.Default.ShowFilterTags) _filters.Add(Filter.Tags, new Filter(Filter.Tags, this, Properties.Settings.Default.GenreView, Properties.Settings.Default.NameAscendingSort, TitleFilterType.Tag, filters));
            if (Properties.Settings.Default.ShowFilterCountry) _filters.Add(Filter.Country, new Filter(Filter.Country, this, Properties.Settings.Default.GenreView, Properties.Settings.Default.NameAscendingSort, TitleFilterType.Country, filters));

            _jumpInListText = new EditableText(this);
            _jumpInListText.Value = String.Empty;

            FocusedItem = new GalleryItem(this, "", "", null);
            _categoryChoice = new Choice(this, "Categories");
            CreateCategories();
            _moviesVirtualList = new VirtualList(this, null);

            CreateSortLookup();

            if (_sortFunctionLookup.ContainsKey(Properties.Settings.Default.MovieSort))
                _currentSort = _sortFunctionLookup[Properties.Settings.Default.MovieSort];
            else
                _currentSort = SortByNameAscending;

            SetupAlphaCharacters();

            LoadMovies(titles);
            OMLApplication.DebugLine("[MovieGallery] Initialize: time: {0}, items: {1}", DateTime.Now - start, this._movies.Count);
        }

        public bool ClearJumpValue
        {
            get
            {
                return true;
            }
            set
            {
                FirePropertyChanged("ClearJumpValue");
            }
        }

        public int RelativeJumpToPosition
        {
            get { return _relativeJumpToPosition; }
        }

        public int JumpToPosition
        {
            get { return _jumpToPosition; }
        }

        public void JumpToLetter(string jumpString, IList list)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                if (jumpString.Length == 0)
                    return;

                List<LabeledList> labels = list as List<LabeledList>;

                if (labels == null)
                    return;

                Utilities.DebugLine("[MovieGallery] JumpToMovie: {0}", jumpString);
                foreach (LabeledList m in list)
                {
                    if (m.FilterLabel == jumpLetter)
                    {
                        int focusedItemIndex = -1;

                        for (int x = 0; x < labels.Count; x++)
                        {
                            if (labels[x].FilterLabel.Equals(FocusedItem.SortName[0].ToString(), StringComparison.OrdinalIgnoreCase))
                            {
                                focusedItemIndex = x;
                                break;
                            }
                        }

                        if (focusedItemIndex < 0)
                        {
                            focusedItemIndex = 0;
                        }

                        _jumpToPosition = labels.IndexOf(m);
                        _relativeJumpToPosition = _jumpToPosition - focusedItemIndex;
                        _focusIndex.Value = _jumpToPosition;

                        //Utilities.DebugLine("[MovieGallery] JumpToString: Found movie {0} pos {1} relpos {2}", m.Name, _jumpToPosition, _relativeJumpToPosition);
                        FirePropertyChanged("JumpToPosition");
                        break;
                    }
                }
            });
        }

        public void JumpToMovie(string jumpString, IList list)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                if (jumpString.Length == 0)
                    return;

                Utilities.DebugLine("[MovieGallery] JumpToMovie: {0}", jumpString);
                foreach (MovieItem m in _moviesVirtualList)
                {
                    if (m.SortName.StartsWith(jumpString, true, null))
                    {
                        int focusedItemIndex = _moviesVirtualList.IndexOf(FocusedItem);

                        if (focusedItemIndex < 0)
                        {
                            focusedItemIndex = 0;
                        }

                        _jumpToPosition = _moviesVirtualList.IndexOf(m);
                        _relativeJumpToPosition = _jumpToPosition - focusedItemIndex;
                        _focusIndex.Value = _jumpToPosition;

                        Utilities.DebugLine("[MovieGallery] JumpToString: Found movie {0} pos {1} relpos {2}", m.Name, _jumpToPosition, _relativeJumpToPosition);
                        FirePropertyChanged("JumpToPosition");
                        break;
                    }
                }
            });
        }

        private void LoadMovies(IEnumerable<Title> titles)
        {
            _movies = new List<MovieItem>();

            if (titles != null)
            {
                OMLApplication.DebugLine("[MovieGallery] LoadMovies: have TitleCollection");

                foreach (Title title in titles)
                {
                    //MovieItem movie = new MovieItem(title, this);
                    //AddMovie(movie);
                    _movies.Add(new MovieItem(title, this));
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
            MovieItem item = (MovieItem)list[index];

            if (item.LoadedCover)
                return;

            //if (item.MenuCoverArt != MovieItem.NoCoverImage)
            //  return;

            Image image = null;

            // too many threads started, kills the system
            //Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread(delegate
            //{
            try
            {
                // OMLApplication.DebugLine("[MovieGallery] CompleteGalleryItem [index: {0}, name: {1}], load menu art", index, item.Name);
                string imageFile = Properties.Settings.Default.UseOriginalCoverArt
                    ? item.TitleObject.FrontCoverPath
                    : item.TitleObject.FrontCoverMenuPath;
                if (!string.IsNullOrEmpty(imageFile) && File.Exists(imageFile))
                    image = GalleryItem.LoadImage(imageFile);
            }
            catch (Exception e)
            {
                OMLApplication.DebugLine("[MovieGallery] Error: {0}\n    {1}", e.Message, e.StackTrace);
            }
            //}, delegate
            //{
            if (image != null)
            {
                // OMLApplication.DebugLine("[MovieGallery] CompleteGalleryItem [index: {0}, name: {1}], set menu art", index, item.Name);
                item.MenuCoverArt = image;
                item.LoadedCover = true;
            }
            //}, null);
        }

        private void SetupAlphaCharacters()
        {
            if (alphaCharacters != null)
                return;

            alphaCharacters = new List<string>(27);

            alphaCharacters.Add("#");

            for (int x = 65; x < 91; x++)
                alphaCharacters.Add(((char)x).ToString());
        }

        #endregion

        #region Private Data

        private ArrayList _categories = new ArrayList();
        private Choice _categoryChoice;

        private Dictionary<string, Filter> _filters = new Dictionary<string, Filter>();
        private Dictionary<string, Comparison<MovieItem>> _sortFunctionLookup = new Dictionary<string, Comparison<MovieItem>>();

        private VirtualList _moviesVirtualList = new VirtualList(); // what we pass to the UI
        private List<MovieItem> _movies = new List<MovieItem>(); // what we use internally for sorting

        delegate int MovieSort(MovieItem m1, MovieItem m2);
        private Comparison<MovieItem> _currentSort;

        private GalleryItem _focusedItem;
        private string _title;

        EditableText _jumpInListText;
        private int _jumpToPosition = -1;           // the ScrollData jump is relative (Scroll # of items)
        private int _relativeJumpToPosition = -1;   // the Repeater jump (NavigateIntoIndex) is absolute

        private string jumpLetter = "a";

        private List<string> alphaCharacters;

        private int numberOfPages = 0;
        private List<LabeledList> labeledLists = null;
        private List<TitleFilter> filters = null;
        #endregion
    }

    /// <summary>
    /// Home command is based off the FilterCommands so it can sit as a top level menu
    /// For some reason MCML won't let this be it's own class and share in the array - by deriving
    /// from FilterCommand I have access to the "Caption" property and can update it through the Filtername variable
    /// </summary>
    public class NavigationCommand : FilterCommand
    {
        public delegate void NavigateCommand();

        NavigateCommand command;

        public NavigationCommand(string name, NavigateCommand command)
        {
            FilterName = name;
            Invoked += Navigate;
            this.command = command;
        }

        public override string ToString()
        {
            return FilterName;
        }

        public void Navigate(object sender, EventArgs args)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                Trace.TraceInformation("MovieGallery.Navigate " + FilterName);
                command.Invoke();
            });
        }
    }
}
