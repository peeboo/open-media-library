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
        /// <summary>
        /// Implemented has an accessor so a derived class can have access to update it
        /// </summary>
        protected string FilterName { get; set; }

        public FilterCommand(Filter filter)
            : base()
        {            
            _filter = filter;
            FilterName = _filter.Name;
            Invoked += filter.OnFilterSelected;
        }

        public override string ToString()
        {
            return Caption;
        }

        protected FilterCommand()
        {
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        public string Caption
        {
            get { return FilterName; }
        }

        private Filter _filter;

        public Filter Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }
    }

    public class Filter
    {
        private MovieGallery _gallery;
        private string _name;

        private Dictionary<string, VirtualList> _itemMovieRelation = new Dictionary<string, VirtualList>();
        private string _galleryView;
        private Comparison<GalleryItem> _currentSort;
        private Dictionary<string, Comparison<GalleryItem>> _sortFunctionLookup = new Dictionary<string, Comparison<GalleryItem>>();

        public const string About = "About";
        public const string Settings = "Settings";
        public const string Genres = "Genres";
        public const string Director = "Directors";
        public const string Actor = "Actors";
        public const string Runtime = "Runtime";
        public const string Country = "Country";
        public const string ParentRating = "Parental Rating";
        public const string Tags = "Tags";
        public const string UserRating = "User Rating";
        public const string Year = "Year";
        public const string DateAdded = "Date Added";
        public const string Home = "OML Home";
        public const string VideoFormat = "Format";
        public const string Trailers = "Trailers";
        public const string Unwatched = "Unwatched";
        public const string Alpha = "Alpha";
        public const string Participant = "Participant";

        public const string AllItems = " All ";

        TitleFilterType filterType = TitleFilterType.All;
        List<TitleFilter> existingFilters;        

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }        

        public List<GalleryItem> Items
        {
            get
            {
                // todo : solomon : look at storing this locally again - i think 
                // the problem i saw with it was unrelated
                //if (_items == null)
                //{
                List<GalleryItem> items = new List<GalleryItem>();
                items.Add(new GalleryItem(_gallery, AllItems, AllItems, this));

                IEnumerable<FilteredCollection> filteredItems = null;
                switch (filterType)
                {
                    case TitleFilterType.Genre:
                        filteredItems = TitleCollectionManager.GetAllGenres(existingFilters);                        
                        break;

                    case TitleFilterType.ParentalRating:
                        filteredItems = TitleCollectionManager.GetAllParentalRatings(existingFilters);
                        break;

                    case TitleFilterType.VideoFormat:
                        filteredItems = TitleCollectionManager.GetAllVideoFormats(existingFilters);
                        break;                        

                    case TitleFilterType.Runtime:
                        filteredItems = TitleCollectionManager.GetAllRuntimes(existingFilters);
                        break;                                                                   

                    case TitleFilterType.Year:
                        filteredItems = TitleCollectionManager.GetAllYears(existingFilters);
                        break;

                    case TitleFilterType.Country:
                        filteredItems = TitleCollectionManager.GetAllCountries(existingFilters);                            
                        break;

                    case TitleFilterType.Tag:
                        filteredItems = TitleCollectionManager.GetAllTags(existingFilters);
                        break;

                    case TitleFilterType.Director:
                        filteredItems = TitleCollectionManager.GetAllPeople(existingFilters, PeopleRole.Director);
                        break;

                    case TitleFilterType.Actor:
                        filteredItems = TitleCollectionManager.GetAllPeople(existingFilters, PeopleRole.Actor);
                        break;

                    case TitleFilterType.UserRating:
                        filteredItems = TitleCollectionManager.GetAllUserRatings(existingFilters);
                        break;

                    case TitleFilterType.DateAdded:
                        filteredItems = TitleCollectionManager.GetAllDateAdded(existingFilters);
                        break;
                }

                if (filteredItems != null)
                {
                    foreach (FilteredCollection item in filteredItems)
                        items.Add(new GalleryItem(_gallery, item.Name, item.Name, this, item.Count));
                }

                //}
                return items;
            }
        }

        public Dictionary<string, VirtualList> ItemMovieRelation
        {
            get { return _itemMovieRelation; }
            set { _itemMovieRelation = value; }
        }

        public string GalleryView
        {
            get { return _galleryView; }
            set { _galleryView = value; }
        }

        /// <summary>
        /// Takes the given filter string and returns a filter type
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static TitleFilterType FilterStringToTitleFilter(string filter)
        {
            switch (filter)
            {
                case "Genres":
                    return TitleFilterType.Genre;
                case "Directors":
                    return TitleFilterType.Director;
                case "Actors":
                    return TitleFilterType.Actor;
                case "Runtime":
                    return TitleFilterType.Runtime;
                case "Country":
                    return TitleFilterType.Country;                    
                case "Parental Rating":
                    return TitleFilterType.ParentalRating;
                case "Tags":
                    return TitleFilterType.Tag;
                case "User Rating":
                    return TitleFilterType.UserRating;
                case "Year":
                    return TitleFilterType.Year;
                case "Date Added":
                    return TitleFilterType.DateAdded;
                case "Format":
                    return TitleFilterType.VideoFormat;
                case "Unwatched":
                    return TitleFilterType.Unwatched;
            }

            return TitleFilterType.All;
        }

        public Filter(string name, MovieGallery gallery, string galleryView, string sortFunction, TitleFilterType filterType, List<TitleFilter> existingFilters)
        {
            this.filterType = filterType;
            this.existingFilters = existingFilters;

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
        /// todo : solomon : does this still make sense ?
        /// </summary>
        public void Sort()
        {
            //if (_allowSort && _currentSort != null) _items.Sort(_currentSort);
        }

        public MovieGallery CreateGallery(string filter)
        {
            Trace.TraceInformation("MovieGallery.CreateFilteredCollection");      
                  
            List<TitleFilter> filters;

            if (existingFilters != null)
                filters = new List<TitleFilter>(existingFilters);
            else
                filters = new List<TitleFilter>(1);

            TitleFilter newFilter = new TitleFilter(filterType, filter);

            if (!filters.Contains(newFilter))
                filters.Add(newFilter);

            MovieGallery movies = new MovieGallery(filters);
            
            movies.SortMovies();
            
            return movies;
        }


        public void OnFilterSelected(object sender, EventArgs args)
        {
            OMLApplication.ExecuteSafe(delegate
            {
                Trace.TraceInformation("MovieGallery.OnFilterSelected");
                FilterCommand cmd = (FilterCommand)sender;
                if (cmd.Filter.Name == Filter.Settings)
                {
                    OMLApplication.Current.GoToSettingsPage(_gallery);
                }
                else if (cmd.Filter.Name == Filter.Trailers)
                {
                    OMLApplication.Current.GoToTrailersPage();
                }
                else if (cmd.Filter.Name == Filter.Unwatched)
                {
                    if ( existingFilters == null )
                        existingFilters = new List<TitleFilter>(1);

                    TitleFilter newFilter = new TitleFilter(TitleFilterType.Unwatched, null);

                    if (!existingFilters.Contains(newFilter))
                        existingFilters.Add(newFilter);

                    OMLApplication.Current.GoToMenu(new MovieGallery(existingFilters));
                }
                else
                {
                    OMLApplication.Current.GoToSelectionList(_gallery, Name);
                    //OMLApplication.Current.GoToSelectionList(_gallery, Items, _gallery.Title + " > " + Name, _galleryView);
                }
            });
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
    }
}
