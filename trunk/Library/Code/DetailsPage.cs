using System;
using System.Collections;

using Microsoft.MediaCenter.UI;
using System.IO;
using System.Xml;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;


namespace Library
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// details page UI.
    /// </summary>
    public class DetailsPage : ModelItem
    {

        /// <summary>The URI of the media at its locally cached location.</summary>
        private FileInfo _localMedia = null;
        /// <summary>Gets or sets the URI of the media at its locally cached location.</summary>
        public FileInfo LocalMedia
        {
            get { return _localMedia; }
            set
            {
                _localMedia = value; 
            }
        }

        public string getUrl
        {
            get
            {
                return _localMedia.FullName;
            }
        }

        /// <summary>
        /// The primary title of the object.
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    FirePropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// A multiline summary of the object.
        /// </summary>
        public string Summary
        {
            get { return summary; }
            set
            {
                if (summary != value)
                {
                    summary = value;
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
            get { return commands; }
            set
            {
                if (commands != value)
                {
                    commands = value;
                    FirePropertyChanged("Commands");
                }
            }
        }

        /// <summary>
        /// Additional minor metadata about this object.
        /// </summary>
        public string Metadata
        {
            get { return metadata; }
            set
            {
                if (metadata != value)
                {
                    metadata = value;
                    FirePropertyChanged("Metadata");
                }
            }
        }

        /// <summary>
        /// A fullscreen image to display in the background.
        /// </summary>
        public Image Background
        {
            get { return backgroundImage; }
            set
            {
                if (backgroundImage != value)
                {
                    backgroundImage = value;
                    FirePropertyChanged("Background");
                }
            }
        }

        public string Rating
        {
            get { return rating; }
            set
            {
                if (rating != value)
                {
                    rating = value;
                    FirePropertyChanged("Rating");
                }
            }
        }

        public string Length
        {
            get { return length; }
            set
            {
                if (length != value)
                {
                    length = value;
                    FirePropertyChanged("Length");
                }
            }
        }

        public string ReleaseDate
        {
            get { return releaseDate; }
            set
            {
                if (releaseDate != value)
                {
                    releaseDate = value;
                    FirePropertyChanged("ReleaseDate");
                }
            }
        }

        public string ImdbRating
        {
            get { return imdbRating; }
            set
            {
                if (imdbRating != value)
                {
                    imdbRating = value;
                    FirePropertyChanged("ImdbRating");
                }
            }
        }

        public IList Actors
        {
            get { return actors; }
            set
            {
                if (actors != value)
                {
                    actors = value;
                    FirePropertyChanged("Actors");
                }
            }
        }

        public IList Directors
        {
            get { return directors; }
            set
            {
                if (directors != value)
                {
                    directors = value;
                    FirePropertyChanged("Directors");
                }
            }
        }

        public IList Producers
        {
            get { return producers; }
            set
            {
                if (producers != value)
                {
                    producers = value;
                    FirePropertyChanged("Producers");
                }
            }
        }

        public IList Writers
        {
            get { return writers; }
            set
            {
                if (writers != value)
                {
                    writers = value;
                    FirePropertyChanged("Writers");
                }
            }
        }

        private IList writers;
        private IList producers;
        private IList directors;
        private IList actors;
        private string imdbRating;
        private string releaseDate;
        private string length;
        private string rating;
        private string Genre; //to do
        private string title;
        private string summary;
        private IList commands;
        private string metadata;
        private Image backgroundImage;
    }
}
