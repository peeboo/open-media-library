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
using OMLEngine.Settings;

namespace Library
{
    /// A list of movies (MovieItems)
    /// </summary>
    public class MovieGallery: BaseModelItem
    {
        #region Public Properties

        /// <summary>
        /// A list of MovieItems used by the UI
        /// </summary>
        /// <value>The movies.</value>
        public VirtualList Movies
        {
            get { return _moviesVirtualList; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        #endregion

        #region Methods
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

        private void Initialize(IEnumerable<Title> titles)
        {
            DateTime start = DateTime.Now;

            _jumpInListText = new EditableText(this);
            _jumpInListText.Value = String.Empty;

            //FocusedItem = new GalleryItem(this, "", "", null);
            _categoryChoice = new Choice(this, "Categories");
            //CreateCategories();
            _moviesVirtualList = new VirtualList(this, null);

            //CreateSortLookup();

            if (_sortFunctionLookup.ContainsKey(OMLSettings.MovieSort))
                _currentSort = _sortFunctionLookup[OMLSettings.MovieSort];
            else
                _currentSort = SortByNameAscending;

            //SetupAlphaCharacters();

            LoadMovies(titles);
            OMLApplication.DebugLine("[MovieGallery] Initialize: time: {0}, items: {1}", DateTime.Now - start, this._movies.Count);
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
                string imageFile = OMLSettings.UseOriginalCoverArt
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
        //private List<LabeledList> labeledLists = null;
        private List<TitleFilter> filters = null;
        #endregion
    }
}
