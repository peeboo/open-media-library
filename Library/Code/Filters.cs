using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;
using OMLEngine.Settings;


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
        private string _name = null;

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

        public TitleFilterType FilterType { get { return this.filterType; } }

        public MovieGallery Gallery
        {
            get { return _gallery; }
        }

        public string Name
        {
            get 
            {
                if (_name == null)
                    _name = FilterTypeToString(filterType);

                return _name; 
            }
        }

        public string Title
        {
            get { return _gallery.Title + " > " + Name; }
        }

        public IList<GalleryItem> GetGalleryItems()
        {            
            List<GalleryItem> items = new List<GalleryItem>();
            //items.Add(new GalleryItem(_gallery, AllItems, AllItems, this));

            IEnumerable<FilteredCollection> filteredItems = null;

            switch (filterType)
            {
                case TitleFilterType.Genre:
                    filteredItems = TitleCollectionManager.GetAllGenres(existingFilters);
                    //IEnumerable<FilteredCollectionWithImages> fc = TitleCollectionManager.GetAllGenresWithImages(existingFilters);
                    //foreach (FilteredCollectionWithImages gen in fc)
                    //{
                        
                    //}
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

                default:
                    throw new ArgumentException("Unknown filter type");
            }

            if (filteredItems != null)
            {
                foreach (FilteredCollection item in filteredItems)
                {
                    GalleryItem galleryItem = new GalleryItem(_gallery, item.Name, item.Name, this, item.Count);
                    if (!string.IsNullOrEmpty(item.ImagePath))
                    {
                        //TODO:ASYNC this
                        galleryItem.MenuCoverArt = MovieItem.LoadImage(item.ImagePath);
                    }

                    items.Add(galleryItem);
                }
            }

            return items;
        }

        public string GetViewForFilter()
        {
            switch (filterType)
            {
                case TitleFilterType.Genre:
                    return "ListWithCover";

                default:
                    return "List";
            }
        }

        public static string FilterTypeToString(TitleFilterType filter)
        {
            switch (filter)
            {
                case TitleFilterType.Genre:
                    return Genres;
                case TitleFilterType.Director:
                    return Director;
                case TitleFilterType.Actor:
                    return Actor;
                case TitleFilterType.Runtime:
                    return Runtime;
                case TitleFilterType.Country:
                    return Country;
                case TitleFilterType.ParentalRating:
                    return ParentRating;
                case TitleFilterType.Tag:
                    return Tags;
                case TitleFilterType.UserRating:
                    return UserRating;
                case TitleFilterType.Year:
                    return Year;
                case TitleFilterType.DateAdded:
                    return DateAdded;
                case TitleFilterType.VideoFormat:
                    return VideoFormat;
                case TitleFilterType.Unwatched:
                    return Unwatched;
            }

            return AllItems;
        }

        /// <summary>
        /// Takes the given filter string and returns a filter type
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static TitleFilterType FilterStringToTitleType(string filter)
        {
            switch (filter)
            {
                case Genres:
                    return TitleFilterType.Genre;
                case Director:
                    return TitleFilterType.Director;
                case Actor:
                    return TitleFilterType.Actor;
                case Runtime:
                    return TitleFilterType.Runtime;
                case Country:
                    return TitleFilterType.Country;                    
                case ParentRating:
                    return TitleFilterType.ParentalRating;
                case Tags:
                    return TitleFilterType.Tag;
                case UserRating:
                    return TitleFilterType.UserRating;
                case Year:
                    return TitleFilterType.Year;
                case DateAdded:
                    return TitleFilterType.DateAdded;
                case VideoFormat:
                    return TitleFilterType.VideoFormat;
                case Unwatched:
                    return TitleFilterType.Unwatched;
            }

            return TitleFilterType.All;
        }

        public static bool ShowFilterType(TitleFilterType filter)
        {
            switch (filter)
            {
                case TitleFilterType.Actor:
                    return OMLSettings.ShowFilterActors;

                case TitleFilterType.Country:
                    return OMLSettings.ShowFilterCountry;

                case TitleFilterType.DateAdded:
                    return OMLSettings.ShowFilterDateAdded;

                case TitleFilterType.Director:
                    return OMLSettings.ShowFilterDirectors;

                case TitleFilterType.Genre:
                    return OMLSettings.ShowFilterGenres;

                case TitleFilterType.ParentalRating:
                    return OMLSettings.ShowFilterParentalRating;

                case TitleFilterType.Runtime:
                    return OMLSettings.ShowFilterRuntime;

                case TitleFilterType.Tag:
                    return OMLSettings.ShowFilterTags;

                case TitleFilterType.UserRating:
                    return OMLSettings.ShowFilterUserRating;

                case TitleFilterType.VideoFormat:
                    return OMLSettings.ShowFilterFormat;

                case TitleFilterType.Year:
                    return OMLSettings.ShowFilterYear;

                case TitleFilterType.Unwatched:
                    return OMLSettings.ShowFilterUnwatched;
            }

            return false;
        }

        public Filter(MovieGallery gallery, TitleFilterType filterType, List<TitleFilter> existingFilters)
        {
            this.filterType = filterType;
            this.existingFilters = existingFilters;

            _gallery = gallery;
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
                    OMLApplication.Current.GoToSelectionList(this);
                }
            });
        }
    }
}
