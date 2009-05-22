using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.MediaCenter.UI;
using OMLEngine.Settings;

namespace Library.Code.V3
{
    public class FavoritesPivot : BrowsePivot
    {
        private IList<UserFilter> m_filters;
        public FavoritesPivot(IModelItemOwner owner, string stDescription, string stNoContentText)
            : base(owner, stDescription, stNoContentText, null)
        {
            //this.ContentLabel = "OML";
            this.SupportsJIL = true;
            this.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            this.ContentItemTemplate = "ListViewItem";
            this.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;

            this.SetupContextMenu();

            
            //this.m_filterType = filterType;
            this.m_listContent = new VirtualList(new ItemCountHandler(this.InitializeListCount));
            ((VirtualList)this.m_listContent).RequestItemHandler = new RequestItemHandler(this.GetItem);
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

        private void SetupContextMenu()
        {
            #region ctx menu
            //create the context menu
            Library.Code.V3.ContextMenuData ctx = new Library.Code.V3.ContextMenuData();
            //some ctx items
            //Library.Code.V3.ThumbnailCommand viewFirstCmd = new Library.Code.V3.ThumbnailCommand(this);
            //viewFirstCmd.Description = "Some Desc Holder";
            ////viewFirstCmd.Invoked += new EventHandler(viewCmd_Invoked);

            //Library.Code.V3.ThumbnailCommand viewSecondCmd = new Library.Code.V3.ThumbnailCommand(this);
            ////viewSecondCmd.Invoked += new EventHandler(viewCmd_Invoked);
            //viewSecondCmd.Description = "View List";


            //Library.Code.V3.ThumbnailCommand viewSettingsCmd = new Library.Code.V3.ThumbnailCommand(this);
            ////viewSettingsCmd.Invoked += new EventHandler(this.settingsCmd_Invoked);
            //viewSettingsCmd.Description = "Settings";

            Library.Code.V3.ThumbnailCommand viewSettingsCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSettingsCmd.Invoked += new EventHandler(viewSettingsCmd_Invoked);
            viewSettingsCmd.Description = "Settings";

            Library.Code.V3.ThumbnailCommand viewSearchCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSearchCmd.Invoked += new EventHandler(this.viewSearchCmd_Invoked);
            viewSearchCmd.Description = "Search";
            ctx.SharedItems.Add(viewSettingsCmd);
            ctx.SharedItems.Add(viewSearchCmd);

            //ctx.SharedItems.Add(viewFirstCmd);
            //ctx.SharedItems.Add(viewSecondCmd);
            //ctx.SharedItems.Add(viewSettingsCmd);

            //Library.Code.V3.ThumbnailCommand viewMovieDetailsCmd = new Library.Code.V3.ThumbnailCommand(this);
            //viewMovieDetailsCmd.Description = "Movie Details";

            //Library.Code.V3.ThumbnailCommand viewPlayCmd = new Library.Code.V3.ThumbnailCommand(this);
            //viewPlayCmd.Description = "Play";

            //Library.Code.V3.ThumbnailCommand viewDeleteCmd = new Library.Code.V3.ThumbnailCommand(this);
            //viewDeleteCmd.Description = "Delete";

            //ctx.UniqueItems.Add(viewMovieDetailsCmd);
            //ctx.UniqueItems.Add(viewPlayCmd);
            //ctx.UniqueItems.Add(viewDeleteCmd);

            //Command CommandContextPopOverlay = new Command();
            //properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);

            //properties.Add("MenuData", ctx);
            this.ContextMenu = ctx;
            #endregion ctx menu
        }
        private void InitializeListCount(VirtualList vlist)
        {
            this.IsBusy = true;
            this.m_filters = OMLSettings.UserFilters;
            ((VirtualList)this.m_listContent).Count = this.m_filters.Count;
            this.IsBusy = false;
        }

        public override int DoSearch(string searchString)
        {
            int jump = 0;
            foreach (UserFilter item in m_filters)
            {
                //TODO: this string stripping is a hack for OML 
                //this needs to be spiffed up
                //see FindContentItem in basebrowsepivot
                string s = null;
                if (item != null)
                {
                    //a and the add a lot of noise
                    if (item.Name.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
                        s = item.Name.Substring(4);
                    else if (item.Name.StartsWith("a ", StringComparison.OrdinalIgnoreCase))
                        s = item.Name.Substring(2);
                }
                if (s == null)
                    s = item.Name;

                int cmpVal = s.CompareTo(searchString);
                if (cmpVal == 0)
                { // the values are the same
                    jump = m_filters.IndexOf(item);
                    break;
                    //direct match
                }
                else if (cmpVal > 0)
                {
                    jump = m_filters.IndexOf(item);
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
            else if (((this.m_filters != null) && (idx >= 0)) && (idx < this.m_filters.Count))
            {
                UserFilter item = this.m_filters[idx] as UserFilter;
                if (item != null)
                {
                    commandForItem = this.GetCommandForItem(item);
                }
            }
            callbackItem(vlist, idx, commandForItem);
        }

        private object GetCommandForItem(UserFilter item)
        {
            return new Library.Code.V3.FilterItem(item, this, item.Filters);
        }

        //private OMLEngine.TitleFilterType m_filterType;

        private IList m_listContent;
        public override IList Content
        {
            get
            {
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

        //IList<Library.GalleryItem> titles;
    }
}
