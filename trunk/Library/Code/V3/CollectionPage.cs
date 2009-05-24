using System;
using System.Collections;
using Microsoft.MediaCenter.UI;
using Library.Code.V3;
using System.Collections.Generic;
using System.Text;
using OMLEngine;

namespace Library.Code.V3
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// gallery page UI.
    /// </summary>
    /// 
    [Microsoft.MediaCenter.UI.MarkupVisible]
    public class CollectionPage : ContentPage
    {
        private Boolean applicationIdle=false;
        public Boolean ApplicationIdle
        {
            get
            {
                return this.applicationIdle;
            }
            set
            {
                if (applicationIdle != value)
                {
                    applicationIdle = value;
                    FirePropertyChanged("ApplicationIdle");
                }
            }
        }

        public void StartApplicationIdle()
        {
            this.applicationIdle = false;
            Microsoft.MediaCenter.UI.Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            Microsoft.MediaCenter.UI.Application.Idle -= new EventHandler(Application_Idle);
            this.ApplicationIdle = true;
        }

        private CollectionItem collectionItem;
        public CollectionPage(CollectionItem item)
            : this()
        {
            this.collectionItem = item;
            //description
            this.Description = item.Description;
            //this.Filters = filter;
            this.Model = new Library.Code.V3.BrowseModel(this);
            this.Model.Pivots = new Choice(this, "desc", new ArrayListDataSet(this));
            this.CreateCommands();
            this.CreateViews();
        }

        /// <summary>
        /// generic context menu for gallery
        /// </summary>
        private void CreateContextMenu()
        {
            //this.contextMenu=new Library.Code.V3.ContextMenuData();
            //Library.Code.V3.ThumbnailCommand viewSettingsCmd = new Library.Code.V3.ThumbnailCommand(this);
            //viewSettingsCmd.Invoked += new EventHandler(this.settingsCmd_Invoked);
            //viewSettingsCmd.Description = "Settings";
            //this.contextMenu.SharedItems.Add(viewSettingsCmd);

            //Library.Code.V3.ThumbnailCommand viewSearchCmd = new Library.Code.V3.ThumbnailCommand(this);
            //viewSettingsCmd.Invoked += new EventHandler(this.searchCmd_Invoked);
            //viewSearchCmd.Description = "Search";
            //this.contextMenu.SharedItems.Add(viewSearchCmd);
        }

        private bool IsFilterDoubled(OMLEngine.TitleFilterType type)
        {
            if (this.Filters == null)
                return false;
            
            foreach (OMLEngine.TitleFilter f in this.Filters)
            {
                if (f.FilterType == type)
                    return true;
            }
            return false;
        }
        private void CreateFilters()
        {
            IList<string> filtersToShow = OMLEngine.Settings.OMLSettings.MainFiltersToShow;

            //filters with no dd rules
            //year
            //date added
            //user rating
            //parental rating
            //format
            //country

            foreach (string filterName in filtersToShow)
            {
                OMLEngine.TitleFilterType filterType = Filter.FilterStringToTitleType(filterName);
                Filter f = new Filter(null, filterType, this.Filters);//new MovieGallery()

                if (Filter.ShowFilterType(filterType))
                {
                    //should we show the year filter
                    Library.Code.V3.FilterPivot filteredPivot;
                    switch (filterType)
                    {
                        case TitleFilterType.Unwatched:
                            if (!this.IsFilterDoubled(filterType))
                            {
                                this.CreateUnwatchedFilter();
                            }
                            break;                            

                        case OMLEngine.TitleFilterType.Year:
                            if (!this.IsFilterDoubled(filterType))
                            {
                                this.CreateYearFilter();
                            }
                            break;
                        case OMLEngine.TitleFilterType.ParentalRating:
                            if (!this.IsFilterDoubled(filterType))
                            {
                                filteredPivot = new Library.Code.V3.FilterPivot(this, Filter.FilterTypeToString(filterType).ToLower(), "No titles were found.", this.Filters, filterType);
                                filteredPivot.ContentLabel = this.Description;
                                this.Model.Pivots.Options.Add(filteredPivot);
                            }
                            break;
                        case OMLEngine.TitleFilterType.VideoFormat:
                            if (!this.IsFilterDoubled(filterType))
                            {
                                filteredPivot = new Library.Code.V3.FilterPivot(this, Filter.FilterTypeToString(filterType).ToLower(), "No titles were found.", this.Filters, filterType);
                                filteredPivot.ContentLabel = this.Description;
                                this.Model.Pivots.Options.Add(filteredPivot);
                            }
                            break;
                        case OMLEngine.TitleFilterType.DateAdded:
                            if (!this.IsFilterDoubled(filterType))
                            {
                                this.CreateDateAddedFilter();
                            }
                            break;
                        case OMLEngine.TitleFilterType.UserRating:
                            if (!this.IsFilterDoubled(filterType))
                            {
                                filteredPivot = new Library.Code.V3.FilterPivot(this, Filter.FilterTypeToString(filterType).ToLower(), "No titles were found.", this.Filters, filterType);
                                filteredPivot.ContentLabel = this.Description;
                                this.Model.Pivots.Options.Add(filteredPivot);
                            }
                            break;
                        case OMLEngine.TitleFilterType.Country:
                            if (!this.IsFilterDoubled(filterType))
                            {
                                filteredPivot = new Library.Code.V3.FilterPivot(this, Filter.FilterTypeToString(filterType).ToLower(), "No titles were found.", this.Filters, filterType);
                                filteredPivot.ContentLabel = this.Description;
                                this.Model.Pivots.Options.Add(filteredPivot);
                            }
                            break;
                        default:
                            filteredPivot = new Library.Code.V3.FilterPivot(this, Filter.FilterTypeToString(filterType).ToLower(), "No titles were found.", this.Filters, filterType);
                            filteredPivot.ContentLabel = this.Description;
                            this.Model.Pivots.Options.Add(filteredPivot);
                            break;
                    }

                    //determine our default pivot
                    if (this.filters == null || this.filters.Count == 0)
                    {
                        foreach (Library.Code.V3.BrowsePivot pivot in this.Model.Pivots.Options)
                        {
                            if (pivot.Description == Properties.Settings.Default.GalleryPivotChosen)
                                this.Model.Pivots.Chosen = pivot;
                        }
                        this.Model.Pivots.ChosenChanged += new EventHandler(Pivots_ChosenChanged);
                    }
                }
            }
            //add favorites if we are root
            if (this.filters == null || this.filters.Count == 0)
            {
                //add the Favorites pivot
                this.CreateFavoritesFilter();
            }
        }

        private void CreateFavoritesFilter()
        {
            Library.Code.V3.FavoritesPivot favoritesPivot = new Library.Code.V3.FavoritesPivot(this, "favorites", "No titles were found.");
            favoritesPivot.ContentLabel = this.Description;
            this.Model.Pivots.Options.Add(favoritesPivot);
            if (favoritesPivot.Description == Properties.Settings.Default.GalleryPivotChosen)
                this.Model.Pivots.Chosen = favoritesPivot;
        }

        void Pivots_ChosenChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("PIV::" +this.Model.Pivots.ChosenIndex.ToString());
            //FirePropertyChanged("ContextMenu");
            Library.Code.V3.BrowsePivot p = (Library.Code.V3.BrowsePivot)this.Model.Pivots.Chosen;
            
            //set our last pivot
            //needs refactor
            Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread
                (
                delegate
                        {
                            Properties.Settings.Default.GalleryPivotChosen = p.Description;
                            Properties.Settings.Default.Save();
                        }, 
                        delegate 
                        {
                            //FirePropertyChanged("ContextMenu");
                        }, null);
        }

        //I'm sure these could go somewhere else!
        public IEnumerable<OMLEngine.Title> SortByDate(IEnumerable<OMLEngine.Title> titles)
        {
            List<OMLEngine.Title> sortedList = new List<OMLEngine.Title>(titles);
            sortedList.Sort(delegate(OMLEngine.Title x, OMLEngine.Title y) { return x.DateAdded.CompareTo(y.DateAdded); });
            return sortedList;
        }
        public IEnumerable<OMLEngine.Title> SortByName(IEnumerable<OMLEngine.Title> titles)
        {
            List<OMLEngine.Title> sortedList = new List<OMLEngine.Title>(titles);
            sortedList.Sort(delegate(OMLEngine.Title x, OMLEngine.Title y) { return x.SortName.CompareTo(y.SortName); });
            return sortedList;
        }

        private void CreateViews()
        {
            this.CreateTitleView();
        }

        private string TitleFromFilter()
        {
            string title = Filter.Home;
            if (filters != null && filters.Count != 0)
            {
                // create the title given the list of filters
                //StringBuilder sb = new StringBuilder(filters.Count * 10);
                //sb.Append(Filter.Home);
                foreach (OMLEngine.TitleFilter filter in this.filters)
                {
                    //sb.Append("|");
                    title = " | " + title;
                    if (!string.IsNullOrEmpty(filter.FilterText))
                    {
                        title = filter.FilterText + title;
                        //sb.Append(filter.FilterText);
                    }
                    else
                    {
                        title = filter.FilterType.ToString() + title;
                        //sb.Append(filter.FilterType.ToString());
                    }
                }
                //title = sb.ToString();
            }
            
            return title;
        }
        private void CreateTitleView()
        {
            Library.Code.V3.TitlesPivot titlePivot = new Library.Code.V3.TitlesPivot(this, "title", "No titles were found.", this.filters);
            titlePivot.ContentLabel = this.Description;
            titlePivot.SupportsJIL = true;
            titlePivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            titlePivot.ContentItemTemplate = "oneRowGalleryItemPoster";
            titlePivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
            this.Model.Pivots.Options.Add(titlePivot);
            //setting this as the default chosen pivot
            this.Model.Pivots.Chosen = titlePivot;
        }

        private void CreateYearFilter()
        {            
            Library.Code.V3.YearPivot yearGroup = new Library.Code.V3.YearPivot(this, "year", "No titles were found.", this.filters);
            yearGroup.ContentLabel = this.Description;            
            this.Model.Pivots.Options.Add(yearGroup);
        }

        private void CreateUnwatchedFilter()
        {
            List<TitleFilter> filters = new List<TitleFilter>(this.filters);
            filters.Add(new TitleFilter(TitleFilterType.Unwatched, false.ToString()));

            Library.Code.V3.TitlesPivot pivot = new TitlesPivot(this, "unwatched", "No titles were found.", filters);
            pivot.ContentLabel = this.Description;
            this.Model.Pivots.Options.Add(pivot);
        }

        private void CreateDateAddedFilter()
        {            
            Library.Code.V3.DateAddedPivot yearGroup = new DateAddedPivot(this, "date added", "No titles were found.", this.filters);
            yearGroup.ContentLabel = this.Description;
            this.Model.Pivots.Options.Add(yearGroup);
        }

        private List<OMLEngine.TitleFilter> filters = new List<OMLEngine.TitleFilter>();
        public List<OMLEngine.TitleFilter> Filters
        {
            get { return this.filters; }
            set { this.filters = value; }
        }

        private Command quickPlay;
        public Command QuickPlay
        {
            get { return this.quickPlay; }
        }
        /// <summary>
        /// Loads the default commands
        /// </summary>
        private void CreateCommands()
        {
            //create the quickplay
            this.quickPlay = new Command(this);
            this.quickPlay.Invoked += new EventHandler(quickPlay_Invoked);
            this.Model.Commands = new ArrayListDataSet(this);

            //create the settings cmd
            Library.Code.V3.ThumbnailCommand settingsCmd = new Library.Code.V3.ThumbnailCommand(this);
            settingsCmd.Description = "settings";
            settingsCmd.DefaultImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_BrowseCmd_Settings");
            settingsCmd.DormantImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_BrowseCmd_Settings_Dormant");
            settingsCmd.FocusImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_BrowseCmd_Settings_Focus");
            //no invoke for now
            //settingsCmd.Invoked += new EventHandler(this.SettingsHandler);
            settingsCmd.Invoked += new EventHandler(settingsCmd_Invoked);
            this.Model.Commands.Add(settingsCmd);

            //create the Search cmd
            Library.Code.V3.ThumbnailCommand searchCmd = new Library.Code.V3.ThumbnailCommand(this);
            searchCmd.Description = "search";
            searchCmd.DefaultImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Search");
            searchCmd.DormantImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Search_Dormant");
            searchCmd.FocusImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Search_Focus");
            searchCmd.Invoked += new EventHandler(searchCmd_Invoked);
            this.Model.Commands.Add(searchCmd);
        }

        void quickPlay_Invoked(object sender, EventArgs e)
        {
            if (this.SelectedItemCommand is Library.Code.V3.MovieItem)
            {
                ((Library.Code.V3.MovieItem)this.SelectedItemCommand).PlayAllDisks();
            }
        }

        /// <summary>
        /// Navigates to the settings page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void settingsCmd_Invoked(object sender, EventArgs e)
        {
            if (OMLApplication.Current.Session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Library.Code.V3.SettingsManager page = new Library.Code.V3.SettingsManager();
                properties["Page"] = page;
                properties["Application"] = OMLApplication.Current;

                OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_Settings_SettingsManager", properties);
            }
        }

        /// <summary>
        /// Navigates to the search page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void searchCmd_Invoked(object sender, EventArgs e)
        {
            if (OMLApplication.Current.Session != null)
            {
                Library.Code.V3.MoviesSearchPage page = new Library.Code.V3.MoviesSearchPage();
                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties["Page"] = page;
                properties["Application"] = OMLApplication.Current;
                OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_MoviesSearchPage", properties);
            }
        }



        public CollectionPage()
            : base()
        {
            this.PageState = new PageState(this);
            this.PageState.IsCurrentPage = true;

            this.cachedSelectedValue = new IntRangedValue(this);
            this.cachedSelectedValue.Value = 0;

            this.PageStateEx = new PageStateEx();

            this.title = "movies";
            _JILtext = new EditableText(null, "JIL");
            JILtext.Value = "";
            JILtext.Submitted += new EventHandler(JILtext_Submitted);
        }

        /// <summary>
        /// CacheSelectedValue handles the selection when navigating back to the page
        /// </summary>
        private IntRangedValue cachedSelectedValue;
        public IntRangedValue CachedSelectedValue
        {
            get
            {
                return this.cachedSelectedValue;
            }
            set
            {
                this.cachedSelectedValue = value;
            }
        }

        /// <summary>
        /// default context menu data for the gallery
        /// </summary>
        //private ContextMenuData contextMenu;
        //public ContextMenuData ContextMenu
        //{
        //    get
        //    {
        //        //return this.contextMenu;
        //        //if our pivot has a ctx menu-lets use that
        //        if (this.model.Pivots.Chosen!=null && ((BrowsePivot)this.model.Pivots.Chosen).ContextMenu != null)
        //            return ((BrowsePivot)this.model.Pivots.Chosen).ContextMenu;
        //        else
        //            return this.contextMenu;

        //    }
        //    set
        //    {
        //        this.contextMenu = value;
        //    }
        //}

        /// <summary>
        /// the PageState for the gallery
        /// </summary>
        private PageState pageState;
        public PageState PageState
        {
            get
            {
                return this.pageState;
            }
            set
            {
                this.pageState = value;
            }
        }

        /// <summary>
        /// the PageState for the gallery
        /// </summary>
        private PageStateEx pageStateEx;
        public PageStateEx PageStateEx
        {
            get
            {
                return this.pageStateEx;
            }
            set
            {
                this.pageStateEx = value;
            }
        }

        /// <summary>
        /// holds pivots and commands for the gallery
        /// </summary>
        private BrowseModel model;
        public BrowseModel Model
        {
            get { return model; }
            set
            {
                if (model != value)
                {
                    model = value;
                    FirePropertyChanged("Model");
                }
            }
        }

        private Image fanArt = null;
        public Image FanArt
        {
            get
            {
                return this.fanArt;
            }
            set
            {
                if (this.fanArt != value)
                {
                    this.fanArt = value;
                    base.FirePropertyChanged("FanArt");
                }
            }
        }

        private int itemID;
        public int ItemID
        {
            get
            {
                return itemID;
            }
            set
            {
                this.itemID = value;
            }
        }

        private GalleryItemSize itemSize;
        public GalleryItemSize ItemSize
        {
            get { return itemSize; }
            set { itemSize = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string galleryLayout;
        [Microsoft.MediaCenter.UI.MarkupVisible]
        public string GalleryLayout
        {
            get
            {
                if (galleryLayout != null)
                    return galleryLayout;
                else
                    return "twoRowGalleryItemPoster";
            }
            set { if (galleryLayout != value) { galleryLayout = value; base.FirePropertyChanged("GalleryLayout"); } }
        }

        private string typerDisplay;
        [Microsoft.MediaCenter.UI.MarkupVisible]
        public string TyperDisplay
        {
            get
            {
                return typerDisplay;

            }
            set
            {
                if (((BrowsePivot)this.model.Pivots.Chosen).SupportsJIL)
                {
                    typerDisplay = value;

                    if (value != "")
                    {
                        SlowSearch(value);
                    }
                }
                else
                {
                    JILtext.Value = "";
                }
            }
        }

        /// <summary>
        /// Used for JIL
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        #region SlowSearch

        private int doSearch(string searchString)
        {
            return ((BrowsePivot)this.model.Pivots.Chosen).DoSearch(searchString);
            //int jump = 0;
            //VirtualList pivotList = (VirtualList)((BrowsePivot)this.model.Pivots.Chosen).Content;
            //foreach (GalleryItem item in pivotList)
            //{
            //    //TODO: this string stripping is a hack for OML 
            //    //this needs to be pushed over to the actual pivot-sorta finished
            //    //see FindContentItem in basebrowsepivot
            //    //right now its against the vlist which is broken because of the slowload of the vlist
            //    string s = null;
            //    if (item != null)
            //    {
            //        if (item.Description.StartsWith("the ", StringComparison.OrdinalIgnoreCase))
            //            s = item.Description.Substring(4);
            //        else if (item.Description.StartsWith(" a", StringComparison.OrdinalIgnoreCase))
            //            s = item.Description.Substring(2);
            //    }
            //    if (s == null)
            //        s = item.SortName;
                
            //    int cmpVal = s.CompareTo(searchString);
            //    if (cmpVal == 0)
            //    { // the values are the same
            //        jump = pivotList.IndexOf(item);
            //        break;
            //        //direct match
            //    }
            //    else if (cmpVal > 0)
            //    {
            //        jump = pivotList.IndexOf(item);
            //        break;
            //    }
            //}
            //return jump;
        }
        public void SlowSearch(string searchString)
        {
            Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread
                                (
                // Delegate to be invoked on background thread
                                 startLoadSlowSearch,

                                 // Delegate to be invoked on app thread
                                 endLoadSlowSearch,

                                 // Parameter to be passed to both delegates
                                 (object)searchString
                                 );
        }

        private void startLoadSlowSearch(object objSearchString)
        {

            try
            {
                string searchString = (string)objSearchString;
                int jump = this.doSearch(searchString);
                if (jump != 0)
                    Microsoft.MediaCenter.UI.Application.DeferredInvoke(notifySlowSearch, jump);
            }
            finally
            {
                //
                // Reset our thread's priority back to its previous value
                //
                //Thread.CurrentThread.Priority = priority;
            }
        }

        private void notifySlowSearch(object obj)
        {
            JILindex = (int)obj - JILCurrentindex;
        }

        private void endLoadSlowSearch(object obj)
        {
            //nothing yet
        }
        #endregion SlowSearch

        private EditableText _JILtext;
        private Int32 _JILindex = new Int32();

        [Microsoft.MediaCenter.UI.MarkupVisible]
        public EditableText JILtext
        {
            get { return _JILtext; }
            set { if (_JILtext != value) { _JILtext = value; base.FirePropertyChanged("JILtext"); } }
        }

        [Microsoft.MediaCenter.UI.MarkupVisible]
        public Int32 JILindex
        {
            get { return _JILindex; }
            set { if (_JILindex != value) { _JILindex = value; base.FirePropertyChanged("JILindex"); } }
        }

        private Int32 _JILCurrentindex = new Int32();
        [Microsoft.MediaCenter.UI.MarkupVisible]
        public Int32 JILCurrentindex
        {
            get { return _JILCurrentindex; }
            set { if (_JILCurrentindex != value) { _JILCurrentindex = value; base.FirePropertyChanged("JILCurrentindex"); } }
        }

        void JILtext_Submitted(Object sender, EventArgs e)
        {
            if (JILtext.Value != "")
                JILtext.Value = "";
        }
    }
}
