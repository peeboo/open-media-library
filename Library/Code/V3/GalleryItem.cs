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

        public MovieItem InternalMovieItem;
        // Fields
        private int itemId;
        private string metadata;

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
        public string Metadata
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
    }

}
