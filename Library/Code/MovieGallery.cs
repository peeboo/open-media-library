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
    public class CategoryCommand : Command
    {
        public CategoryCommand(string s, EventHandler selectedHandler )
            : base()
        {
            _caption = s;
            _selectedHandler = selectedHandler;
            Invoked += _selectedHandler;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <returns></returns>
        public string Caption
        {
            get { return _caption; }
        }

        private string _caption;
        private EventHandler _selectedHandler;
    }

    /// A list of movies (MovieItems)
    /// </summary>
    public class MovieGallery : ModelItem
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the genres for all the movies in this gallery
        /// </summary>
        /// <value>The genres.</value>
        public ArrayList Genres
        {
            get { return _genres; }
            set { _genres = value; }
        }

        /// <summary>
        /// Gets or sets the actors  for all the movies in this gallery
        /// </summary>
        /// <value>The actors.</value>
        public ArrayList Actors
        {
            get { return _actors; }
            set { _actors = value; }
        }

        /// <summary>
        /// Gets or sets the directors for all the movies in this gallery
        /// </summary>
        /// <value>The directors.</value>
        public ArrayList Directors
        {
            get { return _directors; }
            set { _directors = value; }
        }

        public Choice Categories
        {
            get { return _categoryChoice; }
            set { _categoryChoice = value; }
        }


        /// <summary>
        /// A list of MovieItems used by the UI
        /// </summary>
        /// <value>The movies.</value>
        public VirtualList Movies
        {
            get { return _movies; }
        }

        /// <summary>
        /// Genre is the key, a list of movies for that genre are the value
        /// </summary>
        /// <value>The genres movies.</value>
        public Hashtable GenresMovies
        {
            get { return _genresMovies; }
        }

        /// <summary>
        /// Actor is the key, a list of movies for the actor are the value
        /// </summary>
        /// <value>The actors movies.</value>
        public Hashtable ActorsMovies
        {
            get { return _actorsMovies; }
        }

        /// <summary>
        /// Director is the key, a list of movies for the dictore are the value
        /// </summary>
        /// <value>The directors movies.</value>
        public Hashtable DirectorsMovies
        {
            get { return _directorsMovies; }
        }
        
        #endregion

        private int _NumberOfMenuRows = 2;
        private Size _MenuImageSize = new Size(150, 200);

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
                }
            }
        }

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
            _categories.Add(new CategoryCommand(Category.Genres, GenreCategorySelected));
            _categories.Add(new CategoryCommand(Category.Director, DirectorsCategorySelected));
            _categories.Add(new CategoryCommand(Category.Actor, ActorsCategorySelected));
            _categoryChoice = new Choice(this, "Categories", _categories);
        }

        private void Initialize(TitleCollection col)
        {
            FocusedItem = new GalleryItem(this, "", "");
            _categoryChoice = new Choice(this, "Categories");
            CreateCategories();
            _movies = new VirtualList(this, null);
            LoadMovies(col);
        }

        /// <summary>
        /// Creates a new gallery from this gallery but based on the supplied filters
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public MovieGallery CreateFilteredGallery(string category, string filter)
        {
            OMLApplication.DebugLine("[MovieGallery] CreateFilteredGallery: Category [{0}] Filter [{1}]", category, filter);

            if (category == Category.Genres)
            {
                return CreateFilteredGalleryHelper(GenresMovies, category, filter);
            }
            else if (category == Category.Director)
            {
                return CreateFilteredGalleryHelper(DirectorsMovies, category, filter);
            }
            else if (category == Category.Actor)
            {
                return CreateFilteredGalleryHelper(ActorsMovies, category, filter);
            }
            else
            {
                return new MovieGallery(new TitleCollection(), "");
            }
        }

        private MovieGallery CreateFilteredGalleryHelper(Hashtable dataSource, string category, string filter)
        {
            OMLApplication.DebugLine("[MovieGallery] CreateFilteredCollection");
            MovieGallery movies = new MovieGallery( _title + " > "  + filter);
            if (dataSource.Contains(filter))
            {
                foreach (MovieItem movie in (VirtualList)dataSource[filter])
                {
                    MovieItem newMovie = (MovieItem)movie.Clone(movies);
                    movies.AddToDirectorsList(newMovie.TitleObject.Directors, movies._directorsMovies, newMovie, movies._directors);
                    movies.AddToActorsList(newMovie.TitleObject.Actors, movies._actorsMovies, newMovie, movies._actors);
                    movies.AddToGenreList(newMovie.TitleObject.Genres, movies._genresMovies, newMovie, movies._genres);
                    movies.Add(newMovie);
                }
            }

            OMLApplication.DebugLine("[MovieGallery] CreateFilteredCollection: done: directors {0} actors {1} genres {2} movies {3}", movies._directors.Count, movies._actors.Count, movies._genres.Count, movies._movies.Count);
            movies._genres.Sort();
            movies._actors.Sort();
            movies._directors.Sort();
            return movies;
        }

        private void LoadMovies(TitleCollection col)
        {
            _movies = new VirtualList(this, null);
            _movies.EnableSlowDataRequests = true;
            _movies.RequestSlowDataHandler = new RequestSlowDataHandler(CompleteGalleryItem);

            _genresMovies = new Hashtable();
            _actorsMovies = new Hashtable();
            _directorsMovies = new Hashtable();

            if (col != null)
            {
                OMLApplication.DebugLine("[MovieGallery] LoadMovies: have TitleCollection");
                //col.loadTitleCollection();
                col.Sort();

                foreach (Title title in col)
                {
                    MovieItem movie = new MovieItem(title, this);
                    _movies.Add(movie);
                    AddToDirectorsList(title.Directors, _directorsMovies, movie, _directors);
                    AddToActorsList(title.Actors, _actorsMovies, movie, _actors);
                    AddToGenreList(title.Genres, _genresMovies, movie, _genres);
                }
            }

            if (Movies.Count > 0)
            {
                _directors.Sort();
                _genres.Sort();
                _actors.Sort();
                FocusedItem = (GalleryItem)Movies[0];
            }
            OMLApplication.DebugLine("[MovieGallery] LoadMovies: done: directors {0} actors {1} genres {2} movies {3}", _directors.Count, _actors.Count, _genres.Count, _movies.Count);
        }

        #endregion

        #region Callbacks
        public void MovieCategorySelected(object sender, EventArgs args)
        {
            OMLApplication.DebugLine("[MovieGallery] MovieCategorySelected");
        }

        public void GenreCategorySelected(object sender, EventArgs args)
        {
            OMLApplication.DebugLine("[MovieGallery] GenreCategorySelected");
            OMLApplication.Current.GoToSelectionList(this, Genres, Title + " > Genres", "List");
        }

        public void DirectorsCategorySelected(object sender, EventArgs args)
        {
            OMLApplication.DebugLine("[MovieGallery] DirectorsCategorySelected");
            OMLApplication.Current.GoToSelectionList(this, Directors, Title + " > Directors", Properties.Settings.Default.DirectorView);
        }

        public void ActorsCategorySelected(object sender, EventArgs args)
        {
            OMLApplication.DebugLine("[MovieGallery] ActorsCategorySelected");
            OMLApplication.Current.GoToSelectionList(this, Actors, Title + " > Actors", Properties.Settings.Default.ActorView);

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
            }
        }
        
        #endregion  
        
        #region Private utility methods
        private void Add(MovieItem m)
        {
            _movies.Add(m);
        }

        private void AddToDirectorsList(IList sourceList, Hashtable categoryMoviesRelation, MovieItem sourceMovie, IList category)
        {
            foreach (Person p in sourceList)
            {
                if (!categoryMoviesRelation.ContainsKey(p.full_name))
                {
                    VirtualList movies = new VirtualList(this, null);
                    movies.Add(sourceMovie);
                    categoryMoviesRelation.Add(p.full_name, movies);
                    category.Add(new DirectorItem(p, this));
                }
                else
                {
                    VirtualList movies = (VirtualList)categoryMoviesRelation[p.full_name];
                    movies.Add(sourceMovie);
                }
            }
        }

        private void AddToActorsList(IList sourceList, Hashtable categoryMoviesRelation, MovieItem sourceMovie, IList category)
        {
            foreach (Person p in sourceList)
            {
                if (!categoryMoviesRelation.ContainsKey(p.full_name))
                {
                    VirtualList movies = new VirtualList(this, null);
                    movies.Add(sourceMovie);
                    categoryMoviesRelation.Add(p.full_name, movies);
                    category.Add(new ActorItem(p, this));
                }
                else
                {
                    VirtualList movies = (VirtualList)categoryMoviesRelation[p.full_name];
                    movies.Add(sourceMovie);
                }
            }
        }

        private void AddToGenreList(IList sourceList, Hashtable categoryMovieRelation, MovieItem sourceMovie, IList category)
        {
            foreach (string p in sourceList)
            {
                if (!categoryMovieRelation.ContainsKey(p))
                {
                    VirtualList movies = new VirtualList(this, null);
                    movies.Add(sourceMovie);
                    categoryMovieRelation.Add(p, movies);
                    category.Add(new GenreItem(p, this));
                }
                else
                {
                    VirtualList movies = (VirtualList)categoryMovieRelation[p];
                    movies.Add(sourceMovie);
                }
            }
        }
        
        #endregion

        #region Private Data

        private ArrayList _categories = new ArrayList();
        private Choice _categoryChoice;

        private ArrayList _directors = new ArrayList();
        private ArrayList _actors = new ArrayList();
        private ArrayList _genres = new ArrayList();

        private VirtualList _movies = new VirtualList();
        private Hashtable _genresMovies = new Hashtable();
        private Hashtable _actorsMovies = new Hashtable();
        private Hashtable _directorsMovies = new Hashtable();

        private string _currentlyBrowsingCategory;
        private string _currentView;
        private GalleryItem _focusedItem;
        private string _title;

        #endregion
   }

    public class GalleryView
    {
        public const string List = "List";
        public const string CoverArt = "Cover Art";
    }

    /// <summary>
    /// Category class - just has some statics for possible Categories
    /// Right now it's just strings but we can make it type safe in the future
    /// </summary>
    public class Category
    {
        public const string Genres = "Genre";
        public const string Director = "Directors";
        public const string Actor = "Actors";
        public const string Runtime = "Runtime";
        public const string Country = "Country";
        public const string ParentRating = "Parent Rating";
        public const string UserRating = "User Rating";
        public const string Year = "Year";
        public const string List = "Filters";
        public const string Home = "OML Home";
    }



}