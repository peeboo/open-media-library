using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.MediaCenter.UI;

namespace Library.Code.V3
{
    public class YearPivot : BrowsePivot
    {
        public YearPivot(IModelItemOwner owner, string stDescription, string stNoContentText)
            : base(owner, stDescription, stNoContentText, null)
        {
            this.m_listContent = new VirtualList(new ItemCountHandler(this.InitializeListCount));
            //((VirtualList)this.m_listContent).RequestItemHandler = new RequestItemHandler(this.GetItem);
        }

        private List<OMLEngine.TitleFilter> m_filters;
        public YearPivot(IModelItemOwner owner, string stDescription, string stNoContentText, List<OMLEngine.TitleFilter> filters)
            : base(owner, stDescription, stNoContentText, null)
        {
            this.SupportsJIL = false;
            this.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            this.ContentItemTemplate = "GalleryGroup";
            this.SubContentItemTemplate = "twoRowPoster";//needs to pull from the total count to decide...
            this.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;

            this.SupportedContentItemTemplates.Add("View Large", "oneRowGalleryItemPoster");
            this.SupportedContentItemTemplates.Add("View Small", "twoRowGalleryItemPoster");
            this.SupportedContentItemTemplates.Add("View List", "ListViewItemGrouped");

            this.m_filters = filters;
            this.m_listContent = new VirtualList(new ItemCountHandler(this.InitializeListCount));
            this.SupportsItemContext = true;
            this.SetupContextMenu();
            //((VirtualList)this.m_listContent).RequestItemHandler = new RequestItemHandler(this.GetItem);
        }

        private void SetupContextMenu()
        {
            #region ctx menu
            //create the context menu
            Library.Code.V3.ContextMenuData ctx = new Library.Code.V3.ContextMenuData();
            //some ctx items
            //Library.Code.V3.ThumbnailCommand viewFirstCmd = new Library.Code.V3.ThumbnailCommand(this);
            //if (this.ContentItemTemplate == "twoRowGalleryItemPoster")
            //    viewFirstCmd.Description = "View Large";
            //else
            //    viewFirstCmd.Description = "View Small";
            //viewFirstCmd.Invoked += new EventHandler(viewCmd_Invoked);

            //Library.Code.V3.ThumbnailCommand viewSecondCmd = new Library.Code.V3.ThumbnailCommand(this);
            //viewSecondCmd.Invoked += new EventHandler(viewCmd_Invoked);
            //viewSecondCmd.Description = "View List";

            Library.Code.V3.ThumbnailCommand viewSettingsCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSettingsCmd.Invoked += new EventHandler(viewSettingsCmd_Invoked);
            viewSettingsCmd.Description = "Settings";

            Library.Code.V3.ThumbnailCommand viewSearchCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSearchCmd.Invoked += new EventHandler(this.viewSearchCmd_Invoked);
            viewSearchCmd.Description = "Search";

            //ctx.SharedItems.Add(viewFirstCmd);
            //ctx.SharedItems.Add(viewSecondCmd);
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

            //titles = new List<OMLEngine.Title>(OMLEngine.TitleCollectionManager.GetFilteredTitles(this.m_filters));
            //((VirtualList)this.m_listContent).Count = titles.Count;



            OMLEngine.TitleFilterType filterType = Filter.FilterStringToTitleType("Date Added");
            Filter f = new Filter(null, filterType, null);//new MovieGallery()
            this.m_listContent = new VirtualList(this, null);
            IList<Library.GalleryItem> filteredTitles = f.GetGalleryItems();
            foreach (Library.GalleryItem item in filteredTitles)
            {
                //this is a temp hack
                if (item.Name != " All " && item.ForcedCount>1)//umm why is the forced count always wrong?
                {
                    //List<OMLEngine.TitleFilter> filter = new List<OMLEngine.TitleFilter>();
                    //am I being silly here copying this?
                    List<OMLEngine.TitleFilter> newFilter = new List<OMLEngine.TitleFilter>();
                    foreach (OMLEngine.TitleFilter filt in this.m_filters)
                    {
                        newFilter.Add(filt);
                    }
                    newFilter.Add(new OMLEngine.TitleFilter(OMLEngine.TitleFilterType.DateAdded, item.Name));

                    Library.Code.V3.YearBrowseGroup testGroup2 = new Library.Code.V3.YearBrowseGroup(newFilter);
                    testGroup2.Owner = this;
                    testGroup2.Description = item.Name;
                    testGroup2.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_mystery");
                    testGroup2.ContentLabelTemplate = Library.Code.V3.BrowseGroup.StandardContentLabelTemplate;
                    this.m_listContent.Add(testGroup2);
                }
            }


            this.IsBusy = false;
        }


        //private void GetItem(VirtualList vlist, int idx, ItemRequestCallback callbackItem)
        //{
        //    object commandForItem = null;
        //    if (((VirtualList)this.m_listContent).IsItemAvailable(idx))
        //    {
        //        commandForItem = this.m_listContent[idx];
        //    }
        //    else if (((this.titles != null) && (idx >= 0)) && (idx < this.titles.Count))
        //    {
        //        OMLEngine.Title item = this.titles[idx] as OMLEngine.Title;
        //        if (item != null)
        //        {
        //            commandForItem = this.GetCommandForItem(item);
        //        }
        //    }
        //    callbackItem(vlist, idx, commandForItem);
        //}

        //private object GetCommandForItem(OMLEngine.Title item)
        //{
        //    return new Library.Code.V3.MovieItem(item, this);
        //}



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

        private List<Library.GalleryItem> titles;
        private bool retrievingData = false;
        private bool contentLoaded = false;
    }
}
