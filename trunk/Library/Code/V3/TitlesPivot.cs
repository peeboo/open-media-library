using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.MediaCenter.UI;

namespace Library.Code.V3
{
    public class TitlesPivot : BrowsePivot
    {
        public TitlesPivot(IModelItemOwner owner, string stDescription, string stNoContentText)
            : base(owner, stDescription, stNoContentText, null)
        {
            this.m_listContent = new VirtualList(new ItemCountHandler(this.InitializeListCount));
            ((VirtualList)this.m_listContent).RequestItemHandler = new RequestItemHandler(this.GetItem);
        }

        private int? parentId;

        private List<OMLEngine.TitleFilter> m_filters;
        public TitlesPivot(IModelItemOwner owner, string stDescription, string stNoContentText, List<OMLEngine.TitleFilter> filters, int? parentId)
            : base(owner, stDescription, stNoContentText, null)
        {
            this.parentId = parentId;
            this.SupportsItemContext = true;
            this.SupportsJIL = true;
            this.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            this.SupportedContentItemTemplates.Add("View Large", "oneRowGalleryItemPoster");
            this.SupportedContentItemTemplates.Add("View Small", "twoRowGalleryItemPoster");
            this.SupportedContentItemTemplates.Add("View List", "ListViewItem");

            this.SetupContextMenu();
            this.m_filters = filters;
            this.m_listContent = new VirtualList(new ItemCountHandler(this.InitializeListCount));
            ((VirtualList)this.m_listContent).RequestItemHandler = new RequestItemHandler(this.GetItem);           
        }

        public override void UpdateContext(string newTemplate)
        {
            //change the buttons based on which view was invoked
            ICommand ctx0 = (ICommand)this.ContextMenu.SharedItems[0];
            ICommand ctx1 = (ICommand)this.ContextMenu.SharedItems[1];

            switch (newTemplate)
            {
                case "oneRowGalleryItemPoster":
                    this.ContentItemTemplate = "oneRowGalleryItemPoster";
                    this.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
                    ctx0.Description = "View Small";
                    ctx1.Description = "View List";
                    break;
                case "twoRowGalleryItemPoster":
                    this.ContentItemTemplate = "twoRowGalleryItemPoster";
                    this.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
                    ctx0.Description = "View Large";
                    ctx1.Description = "View List";
                    break;
                case "ListViewItem":
                    this.ContentItemTemplate = "ListViewItem";
                    this.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
                    ctx0.Description = "View Large";
                    ctx1.Description = "View Small";
                    break;
            }
        }

        void viewCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.CatchMoreInfo();

            ICommand invokedCmd = (ICommand)sender;
            string template = "oneRowGalleryItemPoster";
            switch (invokedCmd.Description)
            {
                case "View Large":
                    template = "oneRowGalleryItemPoster";
                    break;
                case "View Small":
                    template = "twoRowGalleryItemPoster";
                    break;
                case "View List":
                    template = "ListViewItem";
                    break;
            }
            this.UpdateContext(template);   
        }

        private void SetupContextMenu()
        {
            #region ctx menu
            //create the context menu
            Library.Code.V3.ContextMenuData ctx = new Library.Code.V3.ContextMenuData();
            //some ctx items
            Library.Code.V3.ThumbnailCommand viewFirstCmd = new Library.Code.V3.ThumbnailCommand(this);
            if (this.ContentItemTemplate == "twoRowGalleryItemPoster")
                viewFirstCmd.Description = "View Large";
            else
                viewFirstCmd.Description = "View Small";
            viewFirstCmd.Invoked += new EventHandler(viewCmd_Invoked);

            Library.Code.V3.ThumbnailCommand viewSecondCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSecondCmd.Invoked += new EventHandler(viewCmd_Invoked);
            viewSecondCmd.Description = "View List";

            Library.Code.V3.ThumbnailCommand viewSettingsCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSettingsCmd.Invoked += new EventHandler(viewSettingsCmd_Invoked);
            viewSettingsCmd.Description = "Settings";

            Library.Code.V3.ThumbnailCommand viewSearchCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSearchCmd.Invoked += new EventHandler(this.viewSearchCmd_Invoked);
            viewSearchCmd.Description = "Search";

            ctx.SharedItems.Add(viewFirstCmd);
            ctx.SharedItems.Add(viewSecondCmd);
            ctx.SharedItems.Add(viewSettingsCmd);
            ctx.SharedItems.Add(viewSearchCmd);

            Library.Code.V3.ThumbnailCommand viewMovieDetailsCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewMovieDetailsCmd.Description = "Movie Details";
            viewMovieDetailsCmd.Invoked += new EventHandler(viewMovieDetailsCmd_Invoked);

            Library.Code.V3.ThumbnailCommand viewPlayCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewPlayCmd.Description = "Play";
            viewPlayCmd.Invoked += new EventHandler(viewPlayCmd_Invoked);

            //Library.Code.V3.ThumbnailCommand viewDeleteCmd = new Library.Code.V3.ThumbnailCommand(this);
            //viewDeleteCmd.Description = "Delete";

            ctx.UniqueItems.Add(viewMovieDetailsCmd);
            ctx.UniqueItems.Add(viewPlayCmd);
            //ctx.UniqueItems.Add(viewDeleteCmd);

            //Command CommandContextPopOverlay = new Command();
            //properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);

            //properties.Add("MenuData", ctx);
            this.ContextMenu = ctx;
            #endregion ctx menu
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

        void viewPlayCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.CatchMoreInfo();
            if (this.Owner is GalleryPage && ((GalleryPage)this.Owner).SelectedItemCommand is MovieItem)
            {
                GalleryPage page = this.Owner as GalleryPage;
                MovieItem movie = page.SelectedItemCommand as MovieItem;
                movie.PlayAllDisks();
            }
        }

        void viewMovieDetailsCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.CatchMoreInfo();
            if (this.Owner is GalleryPage)
                ((GalleryPage)this.Owner).SelectedItemCommand.Invoke();
        }
        private void InitializeListCount(VirtualList vlist)
        {
            this.IsBusy = true;

            List<OMLEngine.TitleFilter> filters = new List<OMLEngine.TitleFilter>(m_filters); ;

            if (this.parentId.HasValue)
                filters.Add(new OMLEngine.TitleFilter(OMLEngine.TitleFilterType.Parent, this.parentId.ToString()));

            // hack hack hack
            // todo : solomon : get a better way of doing this
            // if there are no filters - enforce that all items must be root items
            if (filters.Count == 0)
            {
                filters.Add(new OMLEngine.TitleTypeFilter(OMLEngine.TitleTypes.Root));
            }            
            
            filters.Add(new OMLEngine.TitleTypeFilter(OMLEngine.TitleTypes.AllFolders | OMLEngine.TitleTypes.AllMedia));
            
            titles = new List<OMLEngine.Title>(OMLEngine.TitleCollectionManager.GetFilteredTitles(filters));

            ((VirtualList)this.m_listContent).Count = titles.Count;

            if (Properties.Settings.Default.GallerySelectedView == 0)
            {
                this.ContentItemTemplate = "ListViewItem";
                this.DetailTemplate = BrowsePivot.StandardDetailTemplate;
            }
            else
            {
                this.ContentItemTemplate = "oneRowGalleryItemPoster";
                this.DetailTemplate = BrowsePivot.ExtendedDetailTemplate;

                if (Properties.Settings.Default.GalleryEnableTwoRow && this.m_listContent.Count > Properties.Settings.Default.GalleryTwoRowMin)
                {
                    this.ContentItemTemplate = "twoRowGalleryItemPoster";
                    this.DetailTemplate = BrowsePivot.StandardDetailTemplate;
                }
                if (Properties.Settings.Default.GalleryEnableThreeRow && this.m_listContent.Count > Properties.Settings.Default.GalleryThreeRowMin)
                {
                    this.ContentItemTemplate = "threeRowGalleryItemPoster";
                    this.DetailTemplate = BrowsePivot.StandardDetailTemplate;
                }
            }
            
            this.IsBusy = false;
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
                OMLEngine.Title item = this.titles[idx] as OMLEngine.Title;
                if (item != null)
                {
                    commandForItem = this.GetCommandForItem(item);
                }
            }
            callbackItem(vlist, idx, commandForItem);
        }

        private object GetCommandForItem(OMLEngine.Title item)
        {
            if ((item.TitleType & OMLEngine.TitleTypes.AllMedia) != 0)
            {
                return new Library.Code.V3.MovieItem(item, this);
            }

            if ((item.TitleType & OMLEngine.TitleTypes.Collection) != 0)
            {
                return new Library.Code.V3.CollectionItem(item, this);
            }

            if ((item.TitleType & OMLEngine.TitleTypes.TVShow) != 0)
            {
                return new Library.Code.V3.CollectionItem(item, this);
            }

            if ((item.TitleType & OMLEngine.TitleTypes.Season) != 0)
            {
                return new Library.Code.V3.SeasonItem(item, this);
            }

            return new Library.Code.V3.MovieItem(item, this);


            /*switch (item.TitleType)
            {
                case OMLEngine.TitleTypes.Movie:
                    {
                    }
                case OMLEngine.TitleTypes.Collection:
                    {
                        return new Library.Code.V3.CollectionItem(item, this);
                    }
                default:
                    {
                        return new Library.Code.V3.MovieItem(item, this);
                    }

            }*/
            
        }



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

        public override int DoSearch(string searchString)
        {
            int jump = 0;
            foreach (OMLEngine.Title item in titles)
            {
                //TODO: this string stripping is a hack for OML 
                //this needs to be spiffed up
                //see FindContentItem in basebrowsepivot
                string s = null;
                if (item != null)
                {
                    if (item.SortName.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
                        s = item.SortName.Substring(4);
                    else if (item.SortName.StartsWith(" a", StringComparison.OrdinalIgnoreCase))
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
        private List<OMLEngine.Title> titles;
        private bool retrievingData = false;
        private bool contentLoaded = false;
        public void StartDataRetrieval(object pivot)
        {
            //testing a slow load...
            //System.Threading.Thread.Sleep(5000);
            this.retrievingData = true;
            this.IsBusy = true;

            Application.DeferredInvoke(delegate
            {
                this.m_listContent = new VirtualList(this.Owner, null);
            });

            titles = new List<OMLEngine.Title>(OMLEngine.TitleCollectionManager.GetAllTitles());
            ((VirtualList)this.m_listContent).Count = titles.Count;
            //Application.DeferredInvoke(delegate
            //{
                //foreach (OMLEngine.Title t in titles)
                //{
                //    this.m_listContent.Add(new Library.Code.V3.MovieItem(t, this));
                //}
            //});
            //this.EndDataRetrieval();
        }

        public void EndDataRetrieval(object pivot)
        {
            if (!IsDisposed)
            {
                if (this.Content.Count > 25)
                {
                    this.ContentItemTemplate = "twoRowGalleryItemPoster";
                    this.DetailTemplate = BrowsePivot.StandardDetailTemplate;
                }
                else
                {
                    this.ContentItemTemplate = "oneRowGalleryItemPoster";
                    this.DetailTemplate = BrowsePivot.ExtendedDetailTemplate;
                }
                this.contentLoaded = true;
                this.retrievingData = false;
                this.FirePropertyChanged("Content");
                this.IsBusy = false;
            }
        }
    }
}
