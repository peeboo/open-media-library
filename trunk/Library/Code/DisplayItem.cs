using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Collections;

namespace Library
{
    public class DisplayItem : Command
    {
        public Image image;
        public string title;
        public string runtime;
        private IList content;
        private int id;
        public string mpaaRating;
        public string imdbRating;

        public DisplayItem() { }
        
        public Image GetImage
        {
            get { return image; }
            set
            {
                image = value;
            }
        }

        public int itemId
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                }
            }
        }

        /// <summary>
        /// The list of items in the page.
        /// This list should only contain objects of type ThumbnailCommand.
        /// </summary>
        public IList Content
        {
            get { return content; }
            set
            {
                if (content != value)
                {
                    content = value;
                    FirePropertyChanged("Content");
                }
            }
        }

        public string GetTitle
        {
            get { return title; }
            set
            {
                title = value;
            }
        }

        public string GetRuntime
        {
            get { return runtime; }
            set
            {
                title = value;
            }
        }

        public string GetMpaaRating
        {
            get { return mpaaRating; }
            set
            {
                title = value;
            }
        }

        public string GetImdbRating
        {
            get { return imdbRating; }
            set
            {
                title = value;
            }
        }
    }
}
