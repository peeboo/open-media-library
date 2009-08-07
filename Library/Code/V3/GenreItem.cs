using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine;
using System.IO;
using Microsoft.MediaCenter.UI;
using OMLEngine.Settings;

namespace Library.Code.V3
{
    public class GenreItem : GalleryItem
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

        public GenreItem(UserFilter title, IModelItem owner, List<TitleFilter> filter)
            : base(owner)
        {
            this.Description = title.Name;

            this.Invoked += delegate(object sender, EventArgs args)
            {
                OMLProperties properties = new OMLProperties();
                properties.Add("Application", OMLApplication.Current);
                properties.Add("I18n", I18n.Instance);
                Command CommandContextPopOverlay = new Command();
                properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);

                Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage(filter, title.Name);

                properties.Add("Page", gallery);
                OMLApplication.Current.Session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
            };
        }

        public GenreItem(Library.GalleryItem title, IModelItem owner, List<TitleFilter> filter)
            : base(owner)
        {
            this.Description = title.Name;
            this.DefaultImage=title.MenuCoverArt;
            this.Metadata = string.Format("{0} titles", title.ForcedCount);
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

        private Library.GalleryItem _title;
        private OMLEngine.Title _titleObj;
        public OMLEngine.Title TitleObject { get { return _titleObj; } }

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
    }
}
