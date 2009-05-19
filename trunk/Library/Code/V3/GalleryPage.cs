using System;
using System.Collections;
using Microsoft.MediaCenter.UI;
using Library.Code.V3;
using System.Collections.Generic;
using System.Text;

namespace Library.Code.V3
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// gallery page UI.
    /// </summary>
    /// 
    [Microsoft.MediaCenter.UI.MarkupVisible]
    public class GalleryPage : ContentPage
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

        public GalleryPage(List<OMLEngine.TitleFilter> filter, string galleryName)
            : this()
        {
            
            //description
            //this.Description = galleryName;
            this.Filters = filter;
            this.Model = new Library.Code.V3.BrowseModel(this);
            this.Model.Pivots = new Choice(this, "desc", new ArrayListDataSet(this));
            this.Description = this.TitleFromFilter();
            //this.CreateContextMenu();
            this.CreateCommands();
            this.CreateViews();
            this.CreateFilters();
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
            /*OMLEngine.TitleFilterType filterType = Filter.FilterStringToTitleType("Year");
            Filter f = new Filter(null, filterType, null);
            VirtualList filteredGalleryList = new VirtualList(this, null);
            IList<Library.GalleryItem> filteredTitles = f.GetGalleryItems();
            foreach (Library.GalleryItem item in filteredTitles)
            {
                //am I being silly here copying this?
                List<OMLEngine.TitleFilter> newFilter = new List<OMLEngine.TitleFilter>();
                foreach (OMLEngine.TitleFilter filt in this.filters)
                {
                    newFilter.Add(filt);
                }
                newFilter.Add(new OMLEngine.TitleFilter(OMLEngine.TitleFilterType.Year, item.Name));

                Library.Code.V3.YearBrowseGroup testGroup2 = new Library.Code.V3.YearBrowseGroup(newFilter);
                testGroup2.Owner = this;
                //tmp hack for unknown dates
                if (item.Name == "1900")
                    testGroup2.Description = "UNKNOWN";
                else
                    testGroup2.Description = item.Name;
                testGroup2.DefaultImage = null;
                testGroup2.Invoked += delegate(object sender, EventArgs args)
                {
                    OMLProperties properties = new OMLProperties();
                    properties.Add("Application", OMLApplication.Current);
                    properties.Add("I18n", I18n.Instance);
                    Command CommandContextPopOverlay = new Command();
                    properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);

                    Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage(newFilter, testGroup2.Description);

                    properties.Add("Page", gallery);
                    OMLApplication.Current.Session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
                };
                testGroup2.ContentLabelTemplate = Library.Code.V3.BrowseGroup.StandardContentLabelTemplate;
                filteredGalleryList.Add(testGroup2);
            }*/

            Library.Code.V3.YearPivot yearGroup = new Library.Code.V3.YearPivot(this, "year", "No titles were found.", this.filters);
            yearGroup.ContentLabel = this.Description;
            /*yearGroup.SupportsJIL = false;
            yearGroup.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            yearGroup.ContentItemTemplate = "GalleryGroup";
            yearGroup.SubContentItemTemplate = "twoRowPoster";//needs to pull from the total count to decide...
            yearGroup.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
            yearGroup.ContentLabel = this.Description;*/
            this.Model.Pivots.Options.Add(yearGroup);
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
        /// <summary>
        /// Loads the default commands
        /// </summary>
        private void CreateCommands()
        {
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

        //gotta remove this-fired only from actor invoke on movieitem
        public GalleryPage(System.Collections.Generic.IEnumerable<OMLEngine.Title> _titles, string galleryName)
            : this()
        {
            #region v3POC

            //SetPrimaryBackgroundImage();
            //OMLEngine.TitleCollection _titles = new OMLEngine.TitleCollection();
            //_titles.loadTitleCollection();
            //primaryBackgroundImage
            //this is temp to test controls
            OMLProperties properties = new OMLProperties();
            properties.Add("Application", OMLApplication.Current);
            properties.Add("UISettings", new UISettings());
            properties.Add("Settings", new Library.Settings());
            properties.Add("I18n", I18n.Instance);
            //v3 main gallery
            Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage();
            //description
            gallery.Description = galleryName;
            //size of the galleryitems
            //DEPRECATED
            //gallery.ItemSize = Library.Code.V3.GalleryItemSize.Small;

            gallery.Model = new Library.Code.V3.BrowseModel(gallery);
            //commands at top of screen
            gallery.Model.Commands = new ArrayListDataSet(gallery);

            //create the context menu
            //Library.Code.V3.ContextMenuData ctx = new Library.Code.V3.ContextMenuData();

            //create the settings cmd
            Library.Code.V3.ThumbnailCommand settingsCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            settingsCmd.Description = "settings";
            settingsCmd.DefaultImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_BrowseCmd_Settings");
            settingsCmd.DormantImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_BrowseCmd_Settings_Dormant");
            settingsCmd.FocusImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_BrowseCmd_Settings_Focus");
            //no invoke for now
            //settingsCmd.Invoked += new EventHandler(settingsCmd_Invoked);
            gallery.Model.Commands.Add(settingsCmd);

            //create the Search cmd
            Library.Code.V3.ThumbnailCommand searchCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            searchCmd.Description = "search";
            searchCmd.DefaultImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Search");
            searchCmd.DormantImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Search_Dormant");
            searchCmd.FocusImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Search_Focus");
            //searchCmd.Invoked += new EventHandler(searchCmd_Invoked);
            gallery.Model.Commands.Add(searchCmd);

            ////some ctx items
            //Library.Code.V3.ThumbnailCommand viewListCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            //viewListCmd.Description = "View List";
            ////viewListCmd.Invoked += new EventHandler(viewCmd_Invoked);

            //Library.Code.V3.ThumbnailCommand viewSmallCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            ////viewSmallCmd.Invoked += new EventHandler(viewCmd_Invoked);
            //viewSmallCmd.Description = "View Small";

            //Library.Code.V3.ThumbnailCommand viewLargeCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            ////viewLargeCmd.Invoked += new EventHandler(viewCmd_Invoked);

            //viewLargeCmd.Description = "View Large";

            //Library.Code.V3.ThumbnailCommand viewSettingsCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            ////viewSettingsCmd.Invoked += new EventHandler(this.SettingsHandler);
            //viewSettingsCmd.Description = "Settings";


            ////ctx.SharedItems.Add(viewLargeCmd);
            //ctx.SharedItems.Add(viewSmallCmd);
            //ctx.SharedItems.Add(viewListCmd);
            //ctx.SharedItems.Add(viewSettingsCmd);

            //Library.Code.V3.ThumbnailCommand viewMovieDetailsCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            //viewMovieDetailsCmd.Description = "Movie Details";

            //Library.Code.V3.ThumbnailCommand viewPlayCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            //viewPlayCmd.Description = "Play";

            //Library.Code.V3.ThumbnailCommand viewDeleteCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            //viewDeleteCmd.Description = "Delete";

            //ctx.UniqueItems.Add(viewMovieDetailsCmd);
            //ctx.UniqueItems.Add(viewPlayCmd);
            //ctx.UniqueItems.Add(viewDeleteCmd);

            Command CommandContextPopOverlay = new Command();
            properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);

            //properties.Add("MenuData", ctx);
            //gallery.ContextMenu = ctx;
            //the pivots
            gallery.Model.Pivots = new Choice(gallery, "desc", new ArrayListDataSet(gallery));

            //twoRowChangerData

            //#region oneRowChangerData
            //if (this.mediaChangers != null && this.mediaChangers.KnownDiscs.Count > 0)
            //{
            //    VirtualList changerGalleryList = new VirtualList(gallery, null);
            //    foreach (Library.Code.V3.DiscDataEx t in this.mediaChangers.KnownDiscs)
            //    {
            //        //galleryList.Add(this.CreateGalleryItem(t));
            //        if (t.DiscType == DiscType.MovieDvd)
            //            changerGalleryList.Add(new Library.Code.V3.DVDChangerItem(t, changerGalleryList));
            //    }


            //    Library.Code.V3.BrowsePivot changerPivot = new Library.Code.V3.BrowsePivot(gallery, "changer", "loading titles...", changerGalleryList);
            //    changerPivot.ContentLabel = "OML";
            //    changerPivot.SupportsJIL = true;
            //    changerPivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            //    changerPivot.ContentItemTemplate = "oneRowGalleryItemPoster";
            //    changerPivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
            //    gallery.Model.Pivots.Options.Add(changerPivot);
            //}
            //#endregion oneRowChangerData

            //twoRowGalleryItemPoster
            #region oneRowGalleryItemPoster
            VirtualList galleryList = new VirtualList(gallery, null);
            foreach (OMLEngine.Title t in _titles)
            {
                //galleryList.Add(this.CreateGalleryItem(t));
                galleryList.Add(new Library.Code.V3.MovieItem(t, galleryList));
            }


            Library.Code.V3.BrowsePivot p = new Library.Code.V3.BrowsePivot(gallery, "one row", "loading titles...", galleryList);
            p.ContentLabel = galleryName;
            p.SupportsJIL = true;
            p.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            p.ContentItemTemplate = "oneRowGalleryItemPoster";
            p.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
            gallery.Model.Pivots.Options.Add(p);
            #endregion oneRowGalleryItemPoster



            ////twoRowGalleryItemPoster
            //#region twoRowGalleryItemPoster
            //VirtualList galleryListGenres = new VirtualList(gallery, null);
            //foreach (OMLEngine.Title t in _titles)
            //{
            //    galleryListGenres.Add(new Library.Code.V3.MovieItem(t, galleryListGenres));
            //}

            //Library.Code.V3.BrowsePivot p2 = new Library.Code.V3.BrowsePivot(gallery, "two row", "loading genres...", galleryListGenres);
            //p2.ContentLabel = galleryName;
            //p2.SupportsJIL = true;
            //p2.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            //p2.ContentItemTemplate = "twoRowGalleryItemPoster";
            //p2.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
            //gallery.Model.Pivots.Options.Add(p2);
            //#endregion twoRowGalleryItemPoster

            ////ListViewItem
            //#region ListViewItem
            //VirtualList galleryListListViewItem = new VirtualList(gallery, null);
            //foreach (OMLEngine.Title t in _titles)
            //{
            //    galleryListListViewItem.Add(new Library.Code.V3.MovieItem(t, galleryListListViewItem));
            //}

            //Library.Code.V3.BrowsePivot p3 = new Library.Code.V3.BrowsePivot(gallery, "list", "loading genres...", galleryListListViewItem);
            //p3.ContentLabel = galleryName;
            //p3.SupportsJIL = true;
            //p3.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            //p3.ContentItemTemplate = "ListViewItem";
            //p3.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
            //gallery.Model.Pivots.Options.Add(p3);
            //#endregion ListViewItem

            ////Grouped
            //#region Grouped

            //VirtualList groupedGalleryListListViewItem = new VirtualList(gallery, null);
            //int i = 1;
            //foreach (OMLEngine.Title t in _titles)
            //{
            //    i++;
            //    groupedGalleryListListViewItem.Add(new Library.Code.V3.MovieItem(t, groupedGalleryListListViewItem));
            //    if (i > 20)
            //        break;
            //}
            //Library.Code.V3.GalleryItem testtgenre = new Library.Code.V3.GalleryItem();
            //testtgenre.Description = "Comedy";
            //testtgenre.Metadata = "12 titles";
            //testtgenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_comedy");
            ////groupedGalleryListListViewItem.Add(testtgenre);

            //Library.Code.V3.BrowseGroup testGroup = new Library.Code.V3.BrowseGroup();//this, "Sample Group", groupedGalleryListListViewItem);
            //testGroup.Owner = gallery;
            //testGroup.Description = "Group 1";
            //testGroup.Metadata = "12 titles";
            //testGroup.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_mystery");
            //testGroup.Content = groupedGalleryListListViewItem;
            //testGroup.ContentLabelTemplate = Library.Code.V3.BrowseGroup.StandardContentLabelTemplate;

            //VirtualList groupListView = new VirtualList(gallery, null);


            ////groupListView.Add(testtgenre);
            //groupListView.Add(testGroup);

            //VirtualList groupedGalleryListListViewItem2 = new VirtualList(gallery, null);

            //foreach (OMLEngine.Title t in _titles)
            //{
            //    i++;
            //    groupedGalleryListListViewItem2.Add(new Library.Code.V3.MovieItem(t, groupedGalleryListListViewItem2));
            //    if (i > 50)
            //        break;
            //}

            //Library.Code.V3.BrowseGroup testGroup2 = new Library.Code.V3.BrowseGroup();//this, "Sample Group", groupedGalleryListListViewItem);
            //testGroup2.Owner = gallery;
            //testGroup2.Description = "Group 2";
            //testGroup2.Metadata = "12 titles";
            //testGroup2.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_mystery");
            //testGroup2.Content = groupedGalleryListListViewItem2;
            //testGroup2.ContentLabelTemplate = Library.Code.V3.BrowseGroup.StandardContentLabelTemplate;


            //groupListView.Add(testGroup2);

            //Library.Code.V3.BrowsePivot group1 = new Library.Code.V3.BrowsePivot(gallery, "group", "grouped...", groupListView);
            //group1.ContentLabel = galleryName;
            //group1.SupportsJIL = false;
            //group1.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            //group1.ContentItemTemplate = "GalleryGroup";
            //group1.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;

            //gallery.Model.Pivots.Options.Add(group1);
            //#endregion grouped

            ////Genres test
            //#region Genres
            //VirtualList galleryListthreeRowGalleryItemPoster = new VirtualList(gallery, null);
            //Library.Code.V3.GalleryItem actionAdventureGenre = new Library.Code.V3.GalleryItem();
            //actionAdventureGenre.Description = "Action/Adventure";
            //actionAdventureGenre.Metadata = "12 titles";
            //actionAdventureGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_actionadventure");
            //galleryListthreeRowGalleryItemPoster.Add(actionAdventureGenre);

            //Library.Code.V3.GalleryItem comedyGenre = new Library.Code.V3.GalleryItem();
            //comedyGenre.Description = "Comedy";
            //comedyGenre.Metadata = "12 titles";
            //comedyGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_comedy");
            //galleryListthreeRowGalleryItemPoster.Add(comedyGenre);

            //Library.Code.V3.GalleryItem documentaryGenre = new Library.Code.V3.GalleryItem();
            //documentaryGenre.Description = "Documentary";
            //documentaryGenre.Metadata = "12 titles";
            //documentaryGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_documentary");
            //galleryListthreeRowGalleryItemPoster.Add(documentaryGenre);

            //Library.Code.V3.GalleryItem dramaGenre = new Library.Code.V3.GalleryItem();
            //dramaGenre.Description = "Drama";
            //dramaGenre.Metadata = "12 titles";
            //dramaGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_drama");
            //galleryListthreeRowGalleryItemPoster.Add(dramaGenre);

            //Library.Code.V3.GalleryItem kidsFamilyGenre = new Library.Code.V3.GalleryItem();
            //kidsFamilyGenre.Description = "Kids/Family";
            //kidsFamilyGenre.Metadata = "12 titles";
            //kidsFamilyGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_kidsfamily");
            //galleryListthreeRowGalleryItemPoster.Add(kidsFamilyGenre);

            //Library.Code.V3.GalleryItem musicalGenre = new Library.Code.V3.GalleryItem();
            //musicalGenre.Description = "Musical";
            //musicalGenre.Metadata = "12 titles";
            //musicalGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_musical");
            //galleryListthreeRowGalleryItemPoster.Add(musicalGenre);

            //Library.Code.V3.GalleryItem mysteryGenre = new Library.Code.V3.GalleryItem();
            //mysteryGenre.Description = "Mystery";
            //mysteryGenre.Metadata = "12 titles";
            //mysteryGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_mystery");
            //galleryListthreeRowGalleryItemPoster.Add(mysteryGenre);

            //Library.Code.V3.GalleryItem otherGenre = new Library.Code.V3.GalleryItem();
            //otherGenre.Description = "Other";
            //otherGenre.Metadata = "12 titles";
            //otherGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_other");
            //galleryListthreeRowGalleryItemPoster.Add(otherGenre);

            //Library.Code.V3.GalleryItem romanceGenre = new Library.Code.V3.GalleryItem();
            //romanceGenre.Description = "Romance";
            //romanceGenre.Metadata = "12 titles";
            //romanceGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_romance");
            //galleryListthreeRowGalleryItemPoster.Add(romanceGenre);

            //Library.Code.V3.GalleryItem scienceFictionGenre = new Library.Code.V3.GalleryItem();
            //scienceFictionGenre.Description = "Science Fiction";
            //scienceFictionGenre.Metadata = "12 titles";
            //scienceFictionGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_sciencefiction");
            //galleryListthreeRowGalleryItemPoster.Add(scienceFictionGenre);

            //Library.Code.V3.GalleryItem westernGenre = new Library.Code.V3.GalleryItem();
            //westernGenre.Description = "Western";
            //westernGenre.Metadata = "12 titles";
            //westernGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_western");
            //galleryListthreeRowGalleryItemPoster.Add(westernGenre);

            //Library.Code.V3.GalleryItem noimageGenre = new Library.Code.V3.GalleryItem();
            //noimageGenre.Description = "No Image ergeg ergege egegeg";
            //noimageGenre.Metadata = "12 titles";
            //noimageGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_noimagegenre");
            //galleryListthreeRowGalleryItemPoster.Add(noimageGenre);

            ////foreach (Title t in _titles)
            ////{
            ////    galleryListthreeRowGalleryItemPoster.Add(this.CreateGalleryItem(t));
            ////}

            //Library.Code.V3.BrowsePivot p4 = new Library.Code.V3.BrowsePivot(gallery, "genres", "loading genres...", galleryListthreeRowGalleryItemPoster);
            //p4.ContentLabel = galleryName;
            //p4.SupportsJIL = true;
            //p4.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            //p4.ContentItemTemplate = "twoRowGalleryItemGenre";
            //p4.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
            //gallery.Model.Pivots.Options.Add(p4);
            //#endregion Genres

            // add OML filters
            //System.Collections.Specialized.StringCollection filtersToShow =
            //    Properties.Settings.Default.MainFiltersToShow;

            //foreach (string filterName in filtersToShow)
            //{
            //    OMLEngine.TitleFilterType filterType = Filter.FilterStringToTitleType(filterName);
            //    Filter f = new Filter(null, filterType, null);//new MovieGallery()

            //    if (Filter.ShowFilterType(filterType))
            //    {

            //        ////IEnumerable<Title> titles = TitleCollectionManager.GetAllTitles();
            //        //IEnumerable<Title> titles;
            //        //if (filterType == TitleFilterType.DateAdded || filterType == TitleFilterType.VideoFormat)
            //        //{
            //        //    titles = TitleCollectionManager.GetAllTitles(); 
            //        //}
            //        //else
            //        //{
            //        //    titles = TitleCollectionManager.GetFilteredTitles(filterType, filterName);
            //        //}

            //        //VirtualList filteredGalleryList = new VirtualList(gallery, null);
            //        //foreach (Title t in titles)
            //        //{
            //        //    //galleryList.Add(this.CreateGalleryItem(t));
            //        //    filteredGalleryList.Add(new Library.Code.V3.MovieItem(t, filteredGalleryList));
            //        //}
            //        VirtualList filteredGalleryList = new VirtualList(gallery, null);
            //        System.Collections.Generic.IList<Library.GalleryItem> filteredTitles = f.GetGalleryItems();
            //        foreach (Library.GalleryItem item in filteredTitles)
            //        {
            //            filteredGalleryList.Add(new Library.Code.V3.MovieItem(item, filteredGalleryList));
            //        }



            //        Library.Code.V3.BrowsePivot filteredPivot = new Library.Code.V3.BrowsePivot(gallery, Filter.FilterTypeToString(filterType).ToLower(), "loading titles...", filteredGalleryList);
            //        filteredPivot.ContentLabel = "OML";
            //        filteredPivot.SupportsJIL = true;
            //        filteredPivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            //        filteredPivot.ContentItemTemplate = "ListViewItem";
            //        filteredPivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
            //        gallery.Model.Pivots.Options.Add(filteredPivot);
            //    }
            //}
            //end add OML filters
            //properties.Add("Gallery", new GalleryV2(properties, _titles));
            properties.Add("Page", gallery);


            Library.Code.V3.MovieDetailsSlideDeck deck = new Library.Code.V3.MovieDetailsSlideDeck();
            //Choice c = new Choice();
            VirtualList Options = new VirtualList();
            Library.Code.V3.SlideBlueprint bp = new Library.Code.V3.SlideBlueprint(@"resx://Library/Library.Resources/V3_Slide_Movie_Details_Synopsis", "Synopsis", DateTime.MinValue, DateTime.Now);
            Library.Code.V3.SlideBlueprint bp2 = new Library.Code.V3.SlideBlueprint(@"resx://Library/Library.Resources/V3_Slide_Movie_Details_Actions", "Actions", DateTime.MinValue, DateTime.Now);
            Options.Add(bp);
            Options.Add(bp2);
            deck.Options = Options;
            deck.Commands = new ArrayListDataSet();

            //dummy up some cmds
            Library.Code.V3.ThumbnailCommand deleteCmd = new Library.Code.V3.ThumbnailCommand(deck);
            deleteCmd.Description = "Delete";
            deleteCmd.DefaultImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Remove");
            deleteCmd.DormantImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Remove_Dormant");
            deleteCmd.FocusImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Remove_Focus");
            deck.Commands.Add(deleteCmd);

            Library.Code.V3.ThumbnailCommand playCmd = new Library.Code.V3.ThumbnailCommand(deck);
            playCmd.Description = "Play";
            playCmd.DefaultImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Play");
            playCmd.DormantImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Play_Dormant");
            playCmd.FocusImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Play_Focus");
            deck.Commands.Add(playCmd);

            deck.Description = "descrip";
            deck.Synopsis = "this is a syn adfge rh rhyr yhyr hr hr ge ge gtwt rgwe tgew gr ewg weg ewg wetg wrt g rhtytjuhytgfr er gtwrt her  etju ktjy hgt efr erfgetw";
            deck.AdditionalCommands = new ArrayListDataSet();
            deck.CommandPopOverlay = new Command();
            //deck.CommandPopOverlay.Invoked += new EventHandler(CommandPopOverlay_Invoked);
            deck.CommandClearOverlays = new Command();
            //deck.CommandClearOverlays.Invoked += new EventHandler(CommandClearOverlays_Invoked);
            deck.CommandPushOverlay = new Command();
            //deck.CommandPushOverlay.Invoked += new EventHandler(CommandPushOverlay_Invoked);

            //deck.AdditionalCommands.Add(cmd);
            properties.Add("SlideDeck", deck);
            properties.Add("CommandPopOverlay", deck.CommandPopOverlay);
            properties.Add("CommandClearOverlays", deck.CommandClearOverlays);
            properties.Add("CommandPushOverlay", deck.CommandPushOverlay);

            deck.Context = "hi";
            //_session.GoToPage(@"resx://Library/Library.Resources/V3_SlideDeck_Movie_Details", properties);

            //gallery.Model.Pivots.Chosen = p2;
            //gallery.Model.Pivots.ChosenChanged += new EventHandler(Pivots_ChosenChanged);
            OMLApplication.Current.Session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
            //_page = gallery;
            //_deck = deck;

            #endregion v3POC
        }

        public GalleryPage()
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

    /// <summary>
    /// The possible size configurations supported by the gallery UI.
    /// </summary>
    public enum GalleryItemSize
    {
        Small,
        Large
    }
    public enum GalleryType
    {
        oneRowGalleryItemPoster,
        twoRowGalleryItemPoster,
        oneRowGalleryItem4x3
    }

    /// <summary>
    /// Maintains PageState for the object to handle navigation based transitions 
    /// </summary>
    /// 
    public class PageStateEx
    {
        public PageTransitionState TransitionState
        {
            get
            {
                if (this.transitionState == PageTransitionState.NavigatingToForward)
                {
                    this.transitionState = PageTransitionState.NavigatingToBackward;
                    return PageTransitionState.NavigatingToForward;
                }
                else
                {
                    return PageTransitionState.NavigatingToBackward;
                }
            }
        }
        private PageTransitionState transitionState = PageTransitionState.NavigatingToForward;
    }
}
