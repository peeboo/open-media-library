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


    public class Filter
    {

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

        public const string AllItems = " All ";

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

        public Filter(string name, MovieGallery gallery, string galleryView, bool bSort, string sortFunction)
        {
            _allowSort = bSort;
            _name = name;
            _gallery = gallery;
            _galleryView = galleryView;
            _currentSort = SortByNameAscending;
            Initialize(sortFunction);
            AddItem(AllItems);
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

        public void AddMovie(string item, MovieItem movie)
        {
            AddMovieToFilter(item, movie);
        }
        /// <summary>
        /// Adds the movie corresponding to the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="movie">The movie.</param>
        private void AddMovieToFilter(string item, MovieItem movie)
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
                _items.Add(new GalleryItem(_gallery, item, item, this));
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
            foreach (KeyValuePair<string, Filter> kvp in movies.Filters)
            {
                kvp.Value.Sort();
            }
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

        private MovieGallery _gallery;
        private string _name;

        private Dictionary<string, VirtualList> _itemMovieRelation = new Dictionary<string, VirtualList>();
        private List<GalleryItem> _items = new List<GalleryItem>();
        private string _galleryView;
        private Comparison<GalleryItem> _currentSort;
        private Dictionary<string, Comparison<GalleryItem>> _sortFunctionLookup = new Dictionary<string, Comparison<GalleryItem>>();
        private bool _allowSort = false;

    }


}
