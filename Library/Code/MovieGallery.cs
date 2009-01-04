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

namespace Library
{
    /// A list of movies (MovieItems)
    /// </summary>
    public class MovieGallery : ModelItem
    {       
        #region Public Properties        

        private List<LabeledList> labeledLists = null;


        private AlphaView alphaView = null;

        public AlphaView AlphaView
        {
            get
            {
                if (alphaView == null)
                    alphaView = new AlphaView();

                return alphaView;
            }
        }

        public List<LabeledList> LabeledLists
        {
            get
            {
                if (labeledLists == null)
                {
                    Filter alphaFilter = null;

                    if (!Filters.TryGetValue(Filter.Alpha, out alphaFilter))
                    {
                        OMLApplication.DebugLine("[MovieGallery][LabeledLists] Alpha filter hit when no alpha filters existed");
                        return new List<LabeledList>(0);
                    }

                    labeledLists = new List<LabeledList>(alphaFilter.ItemMovieRelation.Keys.Count);                                        
                    List<string> keys = new List<string>(alphaFilter.ItemMovieRelation.Keys.Count);
                    
                    foreach (string key in alphaFilter.ItemMovieRelation.Keys)
                        keys.Add(key);

                    keys.Sort();

                    foreach (string filterKey in keys)                    
                    {
                        MovieGallery gallery = alphaFilter.CreateGallery(filterKey);
                        if (gallery != null &&
                            gallery.Movies != null &&
                            gallery.Movies.Count != 0)
                        {
                            labeledLists.Add(new LabeledList(filterKey, gallery.Movies));
                        }                        
                    }

                    int index = 0;

                    // index all the times
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
            // home is first
            _categories.Add(new HomeCommand(Filter.Home));

            // then settings
            _categories.Add(new FilterCommand(Filters[Filter.Settings]));

            // add unwatched filter at the top            
            if ( Properties.Settings.Default.ShowFilterUnwatched )
                _categories.Add(new UnwatchedCommand(Filter.Unwatched));

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

        private void Initialize(TitleCollection col)
        {
            DateTime start = DateTime.Now;
            
            // if their start page is unwatched movies - auto-add the filter
            if (!Properties.Settings.Default.ShowFilterUnwatched && 
                Properties.Settings.Default.StartPage == Filter.Unwatched)
            {
                Properties.Settings.Default.ShowFilterUnwatched = true;
                Properties.Settings.Default.Save();
            }

            _filters.Add(Filter.Settings, new Filter(Filter.Settings, this, Properties.Settings.Default.ActorView, true, Properties.Settings.Default.ActorSort));
            if (Properties.Settings.Default.ShowFilterUnwatched) _filters.Add(Filter.Unwatched, new Filter(Filter.Unwatched, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.NameAscendingSort));

            if (Properties.Settings.Default.ShowFilterActors) _filters.Add(Filter.Actor, new Filter(Filter.Actor, this, Properties.Settings.Default.ActorView, true, Properties.Settings.Default.ActorSort));
            if (Properties.Settings.Default.ShowFilterDirectors) _filters.Add(Filter.Director, new Filter(Filter.Director, this, Properties.Settings.Default.DirectorView, true, Properties.Settings.Default.DirectorSort));
            if (Properties.Settings.Default.ShowFilterGenres) _filters.Add(Filter.Genres, new Filter(Filter.Genres, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.GenreSort));
            if (Properties.Settings.Default.ShowFilterYear) _filters.Add(Filter.Year, new Filter(Filter.Year, this, Properties.Settings.Default.YearView, true, Properties.Settings.Default.YearSort));
            if (Properties.Settings.Default.ShowFilterDateAdded) _filters.Add(Filter.DateAdded, new Filter(Filter.DateAdded, this, Properties.Settings.Default.DateAddedView, false, Properties.Settings.Default.DateAddedSort));
            if (Properties.Settings.Default.ShowFilterRuntime) _filters.Add(Filter.Runtime, new Filter(Filter.Runtime, this, GalleryView.List, false, String.Empty));
            if (Properties.Settings.Default.ShowFilterUserRating) _filters.Add(Filter.UserRating, new Filter(Filter.UserRating, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.UserRatingSort));
            if (Properties.Settings.Default.ShowFilterFormat) _filters.Add(Filter.VideoFormat, new Filter(Filter.VideoFormat, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.NameAscendingSort));

            if (Properties.Settings.Default.ShowFilterParentalRating) _filters.Add(Filter.ParentRating, new Filter(Filter.ParentRating, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.NameAscendingSort));
            if (Properties.Settings.Default.ShowFilterTags) _filters.Add(Filter.Tags, new Filter(Filter.Tags, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.NameAscendingSort));
            if (Properties.Settings.Default.ShowFilterCountry) _filters.Add(Filter.Country, new Filter(Filter.Country, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.NameAscendingSort));
            if (Properties.Settings.Default.ShowFilterTrailers) _filters.Add(Filter.Trailers, new Filter(Filter.Trailers, this, Properties.Settings.Default.ActorView, true, Properties.Settings.Default.ActorSort));            
            if ( Properties.Settings.Default.MovieView == GalleryView.CoverArtWithAlpha) _filters.Add(Filter.Alpha, new Filter(Filter.Alpha, this, Properties.Settings.Default.GenreView, true, Properties.Settings.Default.NameAscendingSort));
            
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

            LoadMovies(col);
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

        /// <summary>
        /// Creates a filter after the initial filter process - this will scan through all titles to recreate the filter
        /// </summary>
        /// <param name="filterName"></param>
        public Filter CreateFilter(string filterName)
        {
            // check it alreayd exists 
            if (Filters.ContainsKey(filterName))
                return Filters[filterName];

            // we'll only create the Participant filter right now
            if (filterName != Filter.Participant)
                return null;

            Filter filter = new Filter(Filter.Participant, this, Properties.Settings.Default.ActorView, true, Properties.Settings.Default.ActorSort);

            foreach (MovieItem movie in _movies)
            {
                AddToActorFilter(filter, movie, false);
                AddToWriterFilter(filter, movie, false);
                AddToDirectorFilter(filter, movie, false);
            }

            _filters.Add(Filter.Participant, filter);

            return filter;
        }

        private void AddToDirectorFilter(Filter directorFilter, MovieItem movie, bool allowDuplicates)
        {
            foreach (Person p in movie.TitleObject.Directors)
            {
                if (p.full_name.Trim().Length == 0)
                    directorFilter.AddMovie("Unknown director", movie, allowDuplicates);
                else
                {
                    directorFilter.AddMovie(p.full_name, movie, allowDuplicates);
                }
            }
        }

        private void AddToActorFilter(Filter actorFilter, MovieItem movie, bool allowDuplicates)
        {
            foreach (KeyValuePair<string, string> kvp in movie.TitleObject.ActingRoles)
            {
                if (kvp.Key.Trim().Length == 0)
                    actorFilter.AddMovie("Unknown actor", movie, allowDuplicates);
                else
                    actorFilter.AddMovie(kvp.Key, movie, allowDuplicates);
            }
        }

        private void AddToWriterFilter(Filter writerFilter, MovieItem movie, bool allowDuplicates)
        {
            foreach (Person p in movie.TitleObject.Writers)
            {
                if (p.full_name.Trim().Length == 0)
                    writerFilter.AddMovie("Unknown writer", movie, allowDuplicates);
                else
                    writerFilter.AddMovie(p.full_name, movie, allowDuplicates);
            }
        }

        public void AddMovie(MovieItem movie)
        {
            //_moviesVirtualList.Add(movie);
            _movies.Add(movie);

            // update the filters
            Title title = movie.TitleObject;

            if (Filters.ContainsKey(Filter.Director) )
            {
                // add the movie to the directors filter
                // since we know the movie is unique we don't need to check for duplicates
                AddToDirectorFilter(Filters[Filter.Director], movie, true);
            }

            if (Filters.ContainsKey(Filter.Actor))
            {
                // add the movie to the actor filter
                // since we know the movie is unique we don't need to check for duplicates
                AddToActorFilter(Filters[Filter.Actor], movie, true);            
            }

            if (Filters.ContainsKey(Filter.Genres))
            {
                foreach (string genre in title.Genres)
                {
                    if (genre.Trim().Length == 0)
                        Filters[Filter.Genres].AddMovie("Not Classified", movie);
                    else
                        Filters[Filter.Genres].AddMovie(genre, movie);
                }
            }

            if (Filters.ContainsKey(Filter.Year))
            {
                Filters[Filter.Year].AddMovie(Convert.ToString(title.ReleaseDate.Year), movie);
            }

            if (Filters.ContainsKey(Filter.DateAdded))
            {
                AddDateAddedFilter(movie);
            }

            if (Filters.ContainsKey(Filter.UserRating))
            {
                if (title.UserStarRating == 0)
                    Filters[Filter.UserRating].AddMovie("Not Rated", movie);
                else
                    Filters[Filter.UserRating].AddMovie(((double)title.UserStarRating / 10).ToString("0.0"), movie);
            }

            if (Filters.ContainsKey(Filter.VideoFormat))
            {
                if (title.Disks.Count > 0)
                    Filters[Filter.VideoFormat].AddMovie(title.Disks[0].Format.ToString(), movie);  //Should really do this independently for each disk, but for now, this should be fine
            }

            if (Filters.ContainsKey(Filter.Tags))
            {
                foreach (string tag in title.Tags)
                {
                    if (tag.Trim().Length == 0)
                        Filters[Filter.Tags].AddMovie("Not Tagged", movie);
                    else
                        Filters[Filter.Tags].AddMovie(tag, movie);
                }
            }

            if (Filters.ContainsKey(Filter.Country))
            {
                if( title.CountryOfOrigin.Trim().Length == 0 )
                    Filters[Filter.Country].AddMovie("Not Specified", movie);
                else
                    Filters[Filter.Country].AddMovie(title.CountryOfOrigin, movie);
            }

            if (Filters.ContainsKey(Filter.ParentRating))
            {
                if( title.ParentalRating.Trim().Length == 0 )
                    Filters[Filter.ParentRating].AddMovie("Unspecified", movie);
                else
                    Filters[Filter.ParentRating].AddMovie(title.ParentalRating, movie);
            }

            if (Filters.ContainsKey(Filter.Runtime))
            {
                AddRuntimeFilter(movie);
            }

            if (Filters.ContainsKey(Filter.Alpha))
            {
                string firstChar = title.SortName.Substring(0, 1).ToUpper();

                if (((int)firstChar[0]) < 65 || ((int)firstChar[0]) > 90)
                    firstChar = "#";

                Filters[Filter.Alpha].AddMovie(firstChar, movie);
            }

            if (Filters.ContainsKey(Filter.Unwatched))
            {
                if( title.WatchedCount == 0 )
                    Filters[Filter.Unwatched].AddMovie(Filter.Unwatched, movie);
            }
        }

        private void AddDateAddedFilter( MovieItem movie )
        {
            Filters[Filter.DateAdded].AddItem("Today");
            Filters[Filter.DateAdded].AddItem("Yesterday");
            Filters[Filter.DateAdded].AddItem("Within Last Week");
            Filters[Filter.DateAdded].AddItem("Within Last 2 Weeks");
            Filters[Filter.DateAdded].AddItem("Within Last Month");
            Filters[Filter.DateAdded].AddItem("Within Last 3 Months");
            Filters[Filter.DateAdded].AddItem("Within Last 6 Months");
            Filters[Filter.DateAdded].AddItem("Within Last Year");
            Filters[Filter.DateAdded].AddItem("More Than 1 Year");

            DateTime today = DateTime.Today;
            DateTime d = new DateTime(movie.TitleObject.DateAdded.Year, movie.TitleObject.DateAdded.Month, movie.TitleObject.DateAdded.Day);
            
            int days = (today - d).Days;

            if (days == 0) Filters[Filter.DateAdded].AddMovie("Today", movie);
            if( days == 1) Filters[Filter.DateAdded].AddMovie("Yesterday", movie);
            if (days <= 7) Filters[Filter.DateAdded].AddMovie("Within Last Week", movie);
            if (days <= 14) Filters[Filter.DateAdded].AddMovie("Within Last 2 Weeks", movie);
            if (days <= 31) Filters[Filter.DateAdded].AddMovie("Within Last Month", movie);
            if (days <= 92) Filters[Filter.DateAdded].AddMovie("Within Last 3 Months", movie);
            if (days <= 184) Filters[Filter.DateAdded].AddMovie("Within Last 6 Months", movie);
            if (days <= 365) Filters[Filter.DateAdded].AddMovie("Within Last Year", movie);
            if (days > 365) Filters[Filter.DateAdded].AddMovie("More Than 1 Year", movie);

        }

        private void AddRuntimeFilter( MovieItem movie)
        {
            //TODO: make this configurable
            Filters[Filter.Runtime].AddItem("30 minutes or less");
            Filters[Filter.Runtime].AddItem("1 hour or less");
            Filters[Filter.Runtime].AddItem("1.5 hours or less");
            Filters[Filter.Runtime].AddItem("2 hours or less");
            Filters[Filter.Runtime].AddItem("2.5 hours or less");
            Filters[Filter.Runtime].AddItem("3 hours or less");
            Filters[Filter.Runtime].AddItem("Over 3 hours");
            Filters[Filter.Runtime].AddItem("Unknown duration");

            Title title = movie.TitleObject;

            if (title.Runtime < 1)
            {
                Filters[Filter.Runtime].AddMovie("Unknown duration", movie);
                return;
            }

            if (title.Runtime <= 30)
            {
                Filters[Filter.Runtime].AddMovie("30 minutes or less", movie);
            }

            if (title.Runtime <= 60)
            {
                Filters[Filter.Runtime].AddMovie("1 hour or less", movie);
            }
            
            if (title.Runtime <= 90)
            {
                Filters[Filter.Runtime].AddMovie("1.5 hours or less", movie);
            }
            
            if (title.Runtime <= 120)
            {
                Filters[Filter.Runtime].AddMovie("2 hours or less", movie);
            }
            
            if (title.Runtime <= 150)
            {
                Filters[Filter.Runtime].AddMovie("2.5 hours or less", movie);
            }
            
            if (title.Runtime <= 180)
            {
                Filters[Filter.Runtime].AddMovie("3 hours or less", movie);
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
            alphaCharacters = new List<string>(26);

            for (int x = 65; x < 91; x++)
                alphaCharacters.Add(((char)x).ToString());
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

        EditableText _jumpInListText;
        private int _jumpToPosition = -1;           // the ScrollData jump is relative (Scroll # of items)
        private int _relativeJumpToPosition = -1;   // the Repeater jump (NavigateIntoIndex) is absolute

        private string jumpLetter = "a";

        private List<string> alphaCharacters;

        private int numberOfPages = 0;      
        #endregion
   } 

    /// <summary>
    /// Home command is based off the FilterCommands so it can sit as a top level menu
    /// For some reason MCML won't let this be it's own class and share in the array - by deriving
    /// from FilterCommand I have access to the "Caption" property and can update it through the Filtername variable
    /// </summary>
    public class HomeCommand : FilterCommand
    {      
        public HomeCommand(string name)
        {
            FilterName = name;
            Invoked += GoHome;
        }

        public override string ToString()
        {
            return FilterName;
        }

        public void GoHome(object sender, EventArgs args)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                Trace.TraceInformation("MovieGallery.GoHome");                
                OMLApplication.Current.GoToMenu(new MovieGallery(OMLApplication.Current.Titles, Filter.Home));
            });
        }
    }

    public class UnwatchedCommand : FilterCommand
    {
        public UnwatchedCommand(string name)
        {
            FilterName = name;
            Invoked += GoHome;
        }

        public override string ToString()
        {
            return FilterName;
        }

        public void GoHome(object sender, EventArgs args)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                Trace.TraceInformation("MovieGallery.GoHome");
                OMLApplication.Current.GoToSelectionList(new MovieGallery(OMLApplication.Current.Titles, Filter.Home), Filter.Unwatched, Filter.Unwatched);
            });
        }
    }
}
