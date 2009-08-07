using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.MediaCenter.UI;
using OMLEngine.Settings;

namespace Library.Code.V3
{
    public class GenrePivot : BrowsePivot
    {
        public GenrePivot(IModelItemOwner owner, string stDescription, string stNoContentText)
            : base(owner, stDescription, stNoContentText, null)
        {
            this.m_listContent = new VirtualList(new ItemCountHandler(this.InitializeListCount));
            ((VirtualList)this.m_listContent).RequestItemHandler = new RequestItemHandler(this.GetItem);
        }

        private List<OMLEngine.TitleFilter> m_filters;
        public GenrePivot(IModelItemOwner owner, string stDescription, string stNoContentText, List<OMLEngine.TitleFilter> filters, OMLEngine.TitleFilterType filterType)
            : base(owner, stDescription, stNoContentText, null)
        {
            this.SetupContextMenu();
            //this.ContentLabel = "OML";
            this.SupportsJIL = true;
            this.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            this.ContentItemTemplate = "twoRowGalleryItemGenre";
            this.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;

            this.SupportedContentItemTemplates.Add("View Large", "twoRowGalleryItemGenre");
            //this.SupportedContentItemTemplates.Add("View Small", "twoRowGalleryItemPoster");
            this.SupportedContentItemTemplates.Add("View List", "ListViewItem");

            
            
            this.m_filters = filters;
            this.m_filterType = filterType;
            this.m_listContent = new VirtualList(new ItemCountHandler(this.InitializeListCount));
            ((VirtualList)this.m_listContent).RequestItemHandler = new RequestItemHandler(this.GetItem);
        }

        void addFavoritesCmd_Invoked(object sender, EventArgs e)
        {
            //this.m_filters
            //IList<UserFilter> oldFilters = OMLSettings.UserFilters;
            //UserFilter[] newFilters = new UserFilter[oldFilters.Count + 1];
            ////deal with the existing userfilters
            //for (int i = 0; i < oldFilters.Count; i++)
            //{
            //    newFilters[i] = oldFilters[i];
            //}

            OMLEngine.TitleFilter[] newFilter = new OMLEngine.TitleFilter[this.m_filters.Count];
            for (int i = 0; i < this.m_filters.Count; i++)
            {
                newFilter[i]=this.m_filters[i];
            }
            
            UserFilter filter = new UserFilter(this.ContentLabel.Replace("|","").Replace(" OML Home",""), newFilter);
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.FavoritesItemSettings page = new Library.Code.V3.FavoritesItemSettings(filter, true);
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.CatchMoreInfo();
            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_FavoritesItemSettings", properties);
            //newFilters[oldFilters.Count] = filter;
            //OMLSettings.UserFilters = newFilters;

            
        }

        void viewSearchCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.CatchMoreInfo();
            if (this.Owner is GalleryPage)
                ((GalleryPage)this.Owner).searchCmd_Invoked(sender, e);
        }

        void viewSettingsCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.CatchMoreInfo();
            if (this.Owner is GalleryPage)
                ((GalleryPage)this.Owner).settingsCmd_Invoked(sender, e);
        }

        public override void UpdateContext(string newTemplate)
        {
            //change the buttons based on which view was invoked
            ICommand ctx0 = (ICommand)this.ContextMenu.SharedItems[0];

            switch (newTemplate)
            {

                case "twoRowGalleryItemGenre":
                    this.ContentItemTemplate = "twoRowGalleryItemGenre";
                    //this.SubContentItemTemplate = "twoRowPoster";
                    this.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
                    ctx0.Description = "View List";
                    break;
                case "ListViewItem":
                    this.ContentItemTemplate = "ListViewItem";
                    this.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
                    ctx0.Description = "View Large";
                    break;
            }
        }

        void viewCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.CatchMoreInfo();

            ICommand invokedCmd = (ICommand)sender;
            string template = "twoRowGalleryItemGenre";
            switch (invokedCmd.Description)
            {
                case "View Large":
                    template = "twoRowGalleryItemGenre";
                    break;
                case "View List":
                    template = "ListViewItem";
                    break;
            }
            this.UpdateContext(template);
        }

        public override void SetupContextMenu()
        {
            #region ctx menu
            //create the context menu
            Library.Code.V3.ContextMenuData ctx = new Library.Code.V3.ContextMenuData();

            Library.Code.V3.ThumbnailCommand viewFirstCmd = new Library.Code.V3.ThumbnailCommand(this);
            //if (this.SubContentItemTemplate == "twoRowPoster")
            viewFirstCmd.Description = "View List";//hard for now-we default to two row
            //else
            //    viewFirstCmd.Description = "View Small";
            viewFirstCmd.Invoked += new EventHandler(viewCmd_Invoked);
            ctx.SharedItems.Add(viewFirstCmd);

            Library.Code.V3.ThumbnailCommand viewSettingsCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSettingsCmd.Invoked += new EventHandler(viewSettingsCmd_Invoked);
            viewSettingsCmd.Description = "Settings";

            Library.Code.V3.ThumbnailCommand viewSearchCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSearchCmd.Invoked += new EventHandler(this.viewSearchCmd_Invoked);
            viewSearchCmd.Description = "Search";

            Library.Code.V3.ThumbnailCommand addFavoritesCmd = new Library.Code.V3.ThumbnailCommand(this);
            addFavoritesCmd.Invoked += new EventHandler(this.addFavoritesCmd_Invoked);
            addFavoritesCmd.Description = "Add to Favorites";

            ctx.SharedItems.Add(viewSettingsCmd);
            ctx.SharedItems.Add(viewSearchCmd);
            ctx.SharedItems.Add(addFavoritesCmd);

            this.ContextMenu = ctx;
            #endregion ctx menu
        }
        private void InitializeListCount(VirtualList vlist)
        {
            this.IsBusy = true;

            Microsoft.MediaCenter.UI.Application.DeferredInvoke(new Microsoft.MediaCenter.UI.DeferredHandler(this.loadBackground), null, new TimeSpan(1));
        }
        private void loadBackground(object options)
        {
            //titles = new List<OMLEngine.Title>(OMLEngine.TitleCollectionManager.GetAllTitles());
            Filter f = new Filter(null, this.m_filterType, this.m_filters);
            titles = f.GetGalleryItems();

            if (this.m_filterType == OMLEngine.TitleFilterType.Genre || this.m_filterType == OMLEngine.TitleFilterType.Tag)
            {
                
            }
            
            //titles = new List<OMLEngine.Title>(OMLEngine.TitleCollectionManager.GetFilteredTitles(this.m_filters));
            ((VirtualList)this.m_listContent).Count = titles.Count;
            //if (this.m_listContent.Count > 25)
            //{
            //    this.ContentItemTemplate = "twoRowGalleryItemPoster";
            //    this.DetailTemplate = BrowsePivot.StandardDetailTemplate;
            //}
            //else
            //{
            //    this.ContentItemTemplate = "oneRowGalleryItemPoster";
            //    this.DetailTemplate = BrowsePivot.ExtendedDetailTemplate;
            //}
            this.IsBusy = false;
        }

        public override int DoSearch(string searchString)
        {
            int jump = 0;
            foreach (Library.GalleryItem item in titles)
            {
                //TODO: this string stripping is a hack for OML 
                //this needs to be spiffed up
                //see FindContentItem in basebrowsepivot
                string s = null;
                if (item != null)
                {
                    //a and the add a lot of noise
                    if (item.SortName.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
                        s = item.SortName.Substring(4);
                    else if (item.SortName.StartsWith("a ", StringComparison.OrdinalIgnoreCase))
                        s = item.SortName.Substring(2);
                }
                if (s == null)
                    s = item.SortName;

                int cmpVal = s.CompareTo(searchString);
                if (cmpVal == 0)
                { // the values are the same
                    jump = titles.IndexOf(item);
                    break;
                    //direct match
                }
                else if (cmpVal > 0)
                {
                    jump = titles.IndexOf(item);
                    break;
                }
            }
            return jump;
        }

        private void GetItem(VirtualList vlist, int idx, ItemRequestCallback callbackItem)
        {
            object commandForItem = null;
            if (((VirtualList)this.m_listContent).IsItemAvailable(idx))
            {
                commandForItem = this.m_listContent[idx];
            }
            else if (((this.titles != null) && (idx >= 0)) && (idx < this.titles.Count))
            {
                Library.GalleryItem item = this.titles[idx] as Library.GalleryItem;
                if (item != null)
                {
                    commandForItem = this.GetCommandForItem(item);
                }
            }
            callbackItem(vlist, idx, commandForItem);
        }

        private object GetCommandForItem(Library.GalleryItem item)
        {
            return new Library.Code.V3.GenreItem(item, this, this.m_filters);
        }

        private OMLEngine.TitleFilterType m_filterType;

        private IList m_listContent;
        public override IList Content
        {
            get
            {
                //if(this.contentLoaded==false)
                //{
                //    if (retrievingData == false)
                //    {
                //        retrievingData = true;
                //        Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread(this.StartDataRetrieval, this.EndDataRetrieval, null);
                //    }
                //    //return new VirtualList(this.Owner, null);
                    
                //}
                return this.m_listContent;
            }
            set
            {
                if (this.m_listContent != value)
                {
                    this.m_listContent = value;
                    base.FirePropertyChanged("Content");
                }
            }
        }

        IList<Library.GalleryItem> titles;
    }
}
