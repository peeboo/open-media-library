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
    /// A list of movies (MovieItems)
    /// </summary>
    public class MovieGallery : Gallery
    {


        private const string AllGenres = "All";

        private TitleCollection _titleCollection;
        private VirtualList _movies = new VirtualList();
        private Hashtable _genres = new Hashtable();
        private Hashtable _actors = new Hashtable();
        private Hashtable _directors = new Hashtable();



        public Hashtable Genres
        {
            get { return _genres; }
        }

        public Hashtable Actors
        {
            get { return _actors; }
        }

        public Hashtable Directors
        {
            get { return _directors; }
        }

        public MovieGallery()
        {
            Trace.WriteLine("MovieGallery: constructor");

            _titleCollection = new TitleCollection();
            _movies = new VirtualList(this, null);

            _movies.EnableSlowDataRequests = true;
            _movies.RequestSlowDataHandler = new RequestSlowDataHandler(CompleteGalleryItem);

            LoadMovies();
            CreateGallery();
            CreateGalleryFilters();
        }

        /// <summary>
        /// Finishes any slow data for a gallery item.
        /// </summary>
        private void CompleteGalleryItem(VirtualList list, int index)
        {
            MovieItem item = (MovieItem)list[index];
            if (item.ItemImage == MovieItem.NoCoverImage)
            {
                Trace.WriteLine("CompleteGalleryItem: loaded index: " + Convert.ToString(index));
                item.ItemImage = GalleryItem.LoadImage(item.TitleObject.FrontCoverPath);
                item.BackCover = GalleryItem.LoadImage(item.TitleObject.BackCoverPath);
            }
        }

        private void LoadMovies()
        {
            _titleCollection.loadTitleCollection();
        }

        private void AddToPersonList(IList personList, Hashtable hashTable, MovieItem movie)
        {
            foreach (Person p in personList)
            {
                if (!hashTable.ContainsKey(p.full_name))
                {
                    VirtualList movies = new VirtualList(this, null);
                    movies.Add(movie);
                    hashTable.Add(p.full_name, movies);
                }
                else
                {
                    VirtualList movies = (VirtualList)hashTable[p.full_name];
                    movies.Add(movie);
                }
            }
        }

        private void AddToStringList(IList list, Hashtable hashTable, MovieItem movie)
        {
            foreach (string p in list)
            {
                if (!hashTable.ContainsKey(p))
                {
                    VirtualList movies = new VirtualList(this,null);
                    movies.Add(movie);
                    hashTable.Add(p, movies);
                }
                else
                {
                    VirtualList movies = (VirtualList)hashTable[p];
                    movies.Add(movie);
                }
            }
        }

        private void CreateGallery()
        {
            Trace.WriteLine("MovieGallery: CreateGallery: start");
            _genres = new Hashtable();
            _actors = new Hashtable();
            _directors = new Hashtable();
            foreach (Title title in _titleCollection)
            {
                MovieItem movie = CreateGalleryItem(title);
                _movies.Add(movie);
                AddToPersonList(title.Directors, _directors, movie);
                AddToPersonList(title.Actors, _actors, movie);
                AddToStringList(title.Genres, _genres, movie);
            }
            Trace.WriteLine("MovieGallery: CreateGallery: end");
        }

        /// <summary>
        /// A list of MovieItems used by the UI
        /// </summary>
        /// <value>The movies.</value>
        public override VirtualList Items
        {
            get { return _movies;}
        }

        private MovieItem CreateGalleryItem(Title title)
        {
            MovieItem item = new MovieItem(title);

            item.Invoked += delegate(object sender, EventArgs args)
            {
                MovieItem galleryItem = (MovieItem)sender;

                // Navigate to a details page for this item.
                MovieDetailsPage page = CreateDetailsPage(item);
                OMLApplication.Current.GoToDetails(page);
            };

            return item;
        }

        protected virtual void OnFilterChanged(object sender, EventArgs args)
        {
                Trace.WriteLine("MovieGallery: OnGenreChanged");
                //MovieGallery galleryPage = (MovieGallery)sender;
                Choice c = (Choice)sender;
                Filter activeFilter = c.Chosen as Filter;
                FilterContent(activeFilter);
        }

        public MovieDetailsPage CreateDetailsPage(MovieItem item)
        {
            Trace.WriteLine("Creating a detailspage");
            MovieDetailsPage page = new MovieDetailsPage(item);
            return page;
        }

        /// <summary>
        /// Creat the filters on the gallery.
        /// </summary>
        private void CreateGalleryFilters()
        {
            CreateFilters(Genres, CategoryFilter.CategoryGenres);
            CreateFilters(Directors , CategoryFilter.CategoryDirector);
            CreateFilters(Actors, CategoryFilter.CategoryActor);

            CurrentCategory = CategoryFilter.CategoryGenres;
        }

        /// <summary>
        /// Creat the filters on the gallery.
        /// </summary>
        private void CreateFilters( Hashtable sourceData, string category)
        {
            VirtualList list = new VirtualList(this, null);

            // Create the unfiltered "All" filter
            ModelItem filterAll = new Filter(this, Filter.AllFilter, category);
            list.Add(filterAll);

            if (sourceData != null)
            {
                foreach (DictionaryEntry d in sourceData)
                {
                    ModelItem filter = new Filter(this, (string)d.Key, category);
                    list.Add(filter);
                }
            }

            Choice filters = new Choice(this, category, list);
            filters.Chosen = filterAll;
            SetFilters(category, new CategoryFilter(filters, OnFilterChanged));
        }


        /// <summary>
        /// Populate the gallery's content given the current filer.
        /// </summary>
        private void FilterContent(Filter activeFilter)
        {
            Trace.WriteLine("MovieGallery: Filtering content: activeFilter: " + activeFilter.Category + ", chosen: " + activeFilter.Description);
            _movies.Clear();

            if (AllGenres == activeFilter.Description)
            {
                Trace.WriteLine("MovieGallery: Filtering content: Getting all");
                CreateGallery();
            }
            else
            {
                if (activeFilter.Category == CategoryFilter.CategoryGenres)
                {
                    SelectMovies(Genres, activeFilter.Description);
                }
                else if (activeFilter.Category == CategoryFilter.CategoryDirector)
                {
                    SelectMovies(Directors, activeFilter.Description);
                }
                else if (activeFilter.Category == CategoryFilter.CategoryActor)
                {
                    SelectMovies(Actors, activeFilter.Description);
                }
            }
        }

        private void SelectMovies(Hashtable source, string filter)
        {
            if (source.Contains(filter))
            {
                foreach (MovieItem movie in (VirtualList)source[filter])
                {
                    _movies.Add(movie);
                }
            }
        }
    }

}