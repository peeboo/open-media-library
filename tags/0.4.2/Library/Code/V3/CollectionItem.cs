using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;
using System.IO;
using Microsoft.MediaCenter.UI;

namespace Library.Code.V3
{
    public class CollectionItem : GalleryItem
    {

        private void CacheImageDone(object nothing)
        {
            FirePropertyChanged("DefaultImage");
        }

        public void SetImage(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                this.m_defaultImage = new Image("file://" + imagePath);
            }
        }

        public CollectionItem(Library.GalleryItem title, IModelItem owner, List<TitleFilter> filter)
            : base(owner)
        {
            this.Description = title.Caption;

            this.Invoked += delegate(object sender, EventArgs args)
            {
                //am I being silly here copying this?
                List<TitleFilter> newFilter = new List<TitleFilter>();
                foreach (TitleFilter filt in filter)
                {
                    newFilter.Add(filt);
                }
                newFilter.Add(new TitleFilter(title.Category.FilterType, title.Name));
                OMLProperties properties = new OMLProperties();
                properties.Add("Application", OMLApplication.Current);
                properties.Add("I18n", I18n.Instance);
                Command CommandContextPopOverlay = new Command();
                properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);

                Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage(newFilter, title.Description);

                properties.Add("Page", gallery);
                OMLApplication.Current.Session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
            };
        }

        private bool isUnwatched = false;
        public bool IsUnwatched
        {
            get
            {
                return isUnwatched;
            }
            set
            {
                if (this.isUnwatched != value)
                {
                    this.isUnwatched = value;
                    FirePropertyChanged("IsUnwatched");
                }
            }
        }


        private OMLEngine.Title _titleObj;
        public OMLEngine.Title TitleObject { get { return _titleObj; } }

        public CollectionItem(Title title, IModelItem owner)
            : base(owner)
        {
            this._titleObj = title;
            //this.InternalMovieItem = new Library.MovieItem(title, null);

            this.ItemType = 0;
            if(OMLEngine.Settings.OMLSettings.ShowUnwatchedIcon)
                this.OverlayContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGalleryItem#UnwatchedOverlay";
            if (this._titleObj.WatchedCount == 0)
                this.isUnwatched = true;

            DateTime releaseDate = Convert.ToDateTime(_titleObj.ReleaseDate.ToString("MMMM dd, yyyy"));// new DateTime(2000, 1, 1);
            //invalid dates
            if (releaseDate.Year != 1 && releaseDate.Year != 1900)
                this.MetadataTop = releaseDate.Year.ToString();
            else
                this.MetadataTop = "";

            this.ItemId = 1;
            string starRating = Convert.ToString(Math.Round((Convert.ToDouble(_titleObj.UserStarRating.HasValue ? _titleObj.UserStarRating.Value : 0) * 0.8), MidpointRounding.AwayFromZero));
            this.StarRating = starRating;
            string extendedMetadata = string.Empty;

            this.SortName = title.SortName;

            //this.Metadata = this.InternalMovieItem.Rating.Replace("PG13", "PG-13").Replace("NC17", "NC-17");
            this.Metadata = _titleObj.ParentalRating;
            if (string.IsNullOrEmpty(_titleObj.ParentalRating))
                this.Metadata = "Not Rated";
            if (_titleObj.Runtime.ToString() != "0")
                this.Metadata += string.Format(", {0} minutes", _titleObj.Runtime.ToString());
            this.Tagline = _titleObj.Synopsis;

            this.Description = title.Name;

            //TODO: not sure how to read this yet...
            //temporarially disabled due to bug in eagerloading
            //this.SimpleVideoFormat = _titleObj.VideoFormat.ToString();


            this.Invoked += delegate(object sender, EventArgs args)
            {
                //stub details for CollectionItem  
                CollectionPage page = new CollectionPage(this);
                OMLProperties properties = new OMLProperties();
                properties.Add("Application", OMLApplication.Current);
                //properties.Add("UISettings", new UISettings());
                //properties.Add("Settings", new Settings());
                properties.Add("I18n", I18n.Instance);
                //v3 main gallery
                //Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage(new List<OMLEngine.TitleFilter>(), "OML");
                ////description
                //gallery.Description = "OML";
                Command CommandContextPopOverlay = new Command();
                properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);
                properties.Add("Page", page);
                OMLApplication.Current.Session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
            };
        }

        private bool imagechkd = false;
        private Image m_defaultImage;
        public override Image DefaultImage
        {
            get
            {
                if (this.m_defaultImage == null && this.imagechkd == false)
                {
                    this.imagechkd = true;
                    this.SetDefaultImage();
                }
                return this.m_defaultImage;
            }
            set
            {
                if (this.m_defaultImage != value)
                {
                    this.m_defaultImage = value;
                    base.FirePropertyChanged("DefaultImage");
                }
            }
        }        

        private void SetDefaultImage()
        {            
            this.imagechkd = true;

            if (this._titleObj != null)
            {
                string imgPath = this._titleObj.FrontCoverMenuPath;
                if (!string.IsNullOrEmpty(imgPath))
                {                    
                    this.DefaultImage = new Image("file://" + imgPath);                    
                }
                else
                {
                    this._titleObj.BeginGetFrontCoverMenuPath(delegate
                    {
                        string path = _titleObj.FrontCoverMenuPath;

                        if (!string.IsNullOrEmpty(path))
                        {
                            SetImage(path);
                            Microsoft.MediaCenter.UI.Application.DeferredInvoke(CacheImageDone, null);
                        }                        
                    });
                }

            }
        }

        private void SetFanArtImage(ExtendedDetails details, Title title)
        {
            if (details.FanArtImages != null)
                return;

            if (title.FanArtPaths == null || title.FanArtPaths.Count == 0)
                return;

            details.FanArtImages = new List<Image>();

            foreach (string path in title.FanArtPaths)
            {
                details.FanArtImages.Add(new Image("file://" + path));
            }

            // set the first image
            details.FanArt = details.FanArtImages[0];
        }            

        private static void AppendSeparatedValue(ref string text, string value, string seperator)
        {
            if (String.IsNullOrEmpty(text))
                text = value;
            else
                text = string.Format("{0}{1}{2}", text, seperator, value);
        }

    }
}
