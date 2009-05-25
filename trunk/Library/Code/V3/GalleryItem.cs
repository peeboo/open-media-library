using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;

namespace Library.Code.V3
{
    /// <summary>
    /// A derived thumbnail command class that stores a unique Id for easy
    /// cross-reference into a table.
    /// </summary>
    public class GalleryItem : ThumbnailCommand
    {
        public GalleryItem()
            : this(null)
        {
        }
        public GalleryItem(IModelItemOwner owner)
            : base(owner)
        {
        }

        private string overlayContentTemplate=null;
        public string OverlayContentTemplate
        {
            get
            {
                return overlayContentTemplate;
            }
            set
            {
                if (this.overlayContentTemplate != value)
                {
                    this.overlayContentTemplate = value;
                    FirePropertyChanged("OverlayContentTemplate");
                }
            }
        }

        //public Library.MovieItem InternalMovieItem;

        //hd/dvd/blu-ray icon
        private string simpleVideoFormat;
        public string SimpleVideoFormat
        {
            get { return simpleVideoFormat; }
            set
            {
                if (simpleVideoFormat != value)
                {
                    simpleVideoFormat = value;
                    FirePropertyChanged("SimpleVideoFormat");
                }
            }
        }

        // Fields
        private int itemId;
        private string metadata;

        public string SortName { get; set; }

        // Properties
        public int ItemId
        {
            get
            {
                return this.itemId;
            }
            set
            {
                this.itemId = value;
            }
        }

        private int itemType;

        public int ItemType
        {
            get
            {
                return this.itemType;
            }
            set
            {
                this.itemType = value;
            }
        }

        //holds the standard metadata (rating, duration | PG-13, 102 minutes)
        public virtual string Metadata
        {
            get
            {
                return this.metadata;
            }
            set
            {
                if (this.metadata != value)
                {
                    this.metadata = value;
                    base.FirePropertyChanged("Metadata");
                }
            }
        }

        //simple star rating (0-8)
        //0=unrated
        //1=.5 stars
        //8=4 stars
        private string starRating;
        public string StarRating
        {
            get
            {
                return this.starRating;
            }
            set
            {
                if (this.starRating != value)
                {
                    this.starRating = value;
                    base.FirePropertyChanged("StarRating");
                }
            }
        }

        private string metadataTop;
        public string MetadataTop
        {
            get
            {
                return this.metadataTop;
            }
            set
            {
                if (this.metadataTop != value)
                {
                    this.metadataTop = value;
                    base.FirePropertyChanged("MetadataTop");
                }
            }
        }

        private string tagline;
        public string Tagline
        {
            get
            {
                return this.tagline;
            }
            set
            {
                if (this.tagline != value)
                {
                    this.tagline = value;
                    base.FirePropertyChanged("Tagline");
                }
            }
        }
    }

}
