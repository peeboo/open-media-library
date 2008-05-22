using System;
using System.Collections;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Xml;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using System.Diagnostics;

namespace Library
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// details page UI.
    /// </summary>
    public class MovieDetailsPage : ModelItem
    {
        /// <summary>The URI of the media at its locally cached location.</summary>
        private FileInfo _localMedia = null;
        private MovieItem _movieDetails = null;

        public MovieDetailsPage(MovieItem item)
        {
            LoadDetails(item);
        }

        private void LoadDetails(MovieItem item)
        {
            _movieDetails = item;

            if (item.FrontCover == null)
            {
                Trace.WriteLine("Details Page.LoadMovies: front cover is null");
            }

            _backgroundImage = item.FrontCover;

            try
            {
                _localMedia = new System.IO.FileInfo(item.FileLocation);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Details Page.LoadMovies exception: " + e.Message);
            }
        }

        public MovieItem MovieDetails
        {
            get { return _movieDetails; }
            set { _movieDetails = value; }
        }
        /// <summary>Gets or sets the URI of the media at its locally cached location.</summary>
        public FileInfo LocalMedia
        {
            get { return _localMedia; }
            set { _localMedia = value; }
        }

        /// <summary>
        /// The primary title of the object.
        /// </summary>
        public string Title
        {
            get { return _movieDetails.Name; }
            set
            {
                if (_movieDetails.Name != value)
                {
                    _movieDetails.Name = value;
                    FirePropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// A multiline summary of the object.
        /// </summary>
        public string Summary
        {
            get { return _movieDetails.Synopsis; }
            set
            {
                if (_movieDetails.Synopsis != value)
                {
                    _movieDetails.Synopsis = value;
                    FirePropertyChanged("Summary");
                }
            }
        }

        /// <summary>
        /// A list of actions that can be performed on this object.
        /// This list should only contain objects of type Command.
        /// </summary>
        public IList Commands
        {
            get { return _commands; }
            set
            {
                if (_commands != value)
                {
                    _commands = value;
                    FirePropertyChanged("Commands");
                }
            }
        }

        /// <summary>
        /// Additional minor metadata about this object.
        /// </summary>
        public string Metadata
        {
            get { return _metadata; }
            set
            {
                if (_metadata != value)
                {
                    _metadata = value;
                    FirePropertyChanged("Metadata");
                }
            }
        }

        /// <summary>
        /// A fullscreen image to display in the background.
        /// </summary>
        public Image Background
        {
            get { return _backgroundImage; }
            set
            {
                if (_backgroundImage != value)
                {
                    _backgroundImage = value;
                    FirePropertyChanged("Background");
                }
            }
        }

        public string Rating
        {
            // this is a bit ugly, we need a better way
            get 
            {
                if (_movieDetails.Rating.Trim().Length > 0)
                    return _movieDetails.Rating;
                else
                    return "";
            }

            set
            {

                if (_movieDetails.Rating != value)
                {
                    _movieDetails.Rating = value;
                    FirePropertyChanged("Rating");
                }
            }
        }

        public string Length
        {
            get 
            {
                if( _movieDetails.TitleObject.Runtime > 0 )
                    return _movieDetails.Runtime + " min"; 
                else
                    return ""; 
            }
            set
            {
                if (_movieDetails.Runtime != value)
                {
                    _movieDetails.Runtime = value;
                    FirePropertyChanged("Length");
                }
            }
        }

        public string ReleaseDate
        {
            get 
            {
                if (_movieDetails.TitleObject.ReleaseDate != DateTime.MinValue)
                    return _movieDetails.ReleaseDate;
                else
                    return "";
            }
        }

        public string ReleaseYear
        {
            get
            {
                if (_movieDetails.TitleObject.ReleaseDate != DateTime.MinValue)
                    return Convert.ToString(_movieDetails.TitleObject.ReleaseDate.Year);
                else
                    return "";
            }
        }

        public IList Actors
        {
            get { return _movieDetails.Actors; }
        }

        public IList Directors
        {
            get { return _movieDetails.Directors; }
        }

        public IList Genres
        {
            get { return _movieDetails.TitleObject.Genres; }
        }
  

        public IList Producers
        {
            get { return _movieDetails.Producers; }
        }

        public IList Writers
        {
            get { return _movieDetails.Writers; }
            //set
            //{
            //    if (_movieDetails.Writers != value)
            //    {
            //        _movieDetails.Writers = value;
            //        FirePropertyChanged("Writers");
            //    }
            //}
        }

        public void PlayMovie()
        {
            _movieDetails.PlayMovie();
        }

        //private DisplayItem _movieDetails;

        //////private IList writers;
        //////private IList producers;
        //////private IList directors;
        //////private IList actors;
        //////private string releaseDate;
        //////private string length;
        //////private string rating;
        //////private string title;
        //////private string summary;
        private IList _commands;
        private string _metadata;
        private Image _backgroundImage;
    }
}
