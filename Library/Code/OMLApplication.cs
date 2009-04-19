//#define DEBUG_EXT
//#define LAYOUT_V2
#define LAYOUT_V3
//#define CAROUSEL

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Reflection;
using System.Threading;

using OMLEngine;
using OMLEngine.Settings;
using System;

namespace Library
{    
    /// <summary>
    /// Starting point for the OML
    /// </summary>
    public class OMLApplication : BaseModelItem
    {
        private bool isBusy = false;
        private bool isStartingTranscodingJob = false;
        private string transcodeStatus = string.Empty;
        private int currentFocusedItemIndex = 0;
        private int currentItemIndexPosition = 0;
        private int currentAngleDegrees;
        private Image primaryBackgroundImage;
        private int iCurrentBackgroundImage = 0;
        private int iTotalBackgroundImages = 0;
        public Microsoft.MediaCenter.UI.Timer mainBackgroundTimer;
        //v3 temp
        private Library.Code.V3.MoreInfoHooker2 hooker;

        //v3 temp
        public void FixRepeatRate(object scroller, uint val)
        {
            PropertyInfo pi = scroller.GetType().GetProperty("View", BindingFlags.Public | BindingFlags.Instance);
            object view = pi.GetValue(scroller, null);
            pi = view.GetType().GetProperty("Control", BindingFlags.Public | BindingFlags.Instance);
            object control = pi.GetValue(view, null);

            pi = control.GetType().GetProperty("KeyRepeatThreshold", BindingFlags.NonPublic | BindingFlags.Instance);
            pi.SetValue(control, val, null);

        }

        private void SetPrimaryBackgroundImage()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.MainPageBackDropFile))
            {
                if (primaryBackgroundImage == null)
                {
                    string lockedFileName = Properties.Settings.Default.MainPageBackDropFile;
                    if (File.Exists(Path.Combine(FileSystemWalker.MainBackDropDirectory, lockedFileName)))
                    {
                        primaryBackgroundImage = new Image(
                            string.Format("file://{0}", Path.Combine(FileSystemWalker.MainBackDropDirectory, lockedFileName))
                        );
                        return;
                    }
                }
            }

            // a specific file is NOT set for the main backdrop, lets see how many we find.
            DirectoryInfo dirInfo = new DirectoryInfo(FileSystemWalker.MainBackDropDirectory);
            FileInfo[] files = dirInfo.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);
            iTotalBackgroundImages = files.Length;
            if (iTotalBackgroundImages > 0)
            {
                if (iCurrentBackgroundImage >= iTotalBackgroundImages)
                    iCurrentBackgroundImage = 0;

                PrimaryBackgroundImage = new Image(string.Format("file://{0}",
                        Path.Combine(FileSystemWalker.MainBackDropDirectory,
                        files[iCurrentBackgroundImage].FullName)));
                iCurrentBackgroundImage++;
                if (mainBackgroundTimer == null)
                {
                    mainBackgroundTimer = new Microsoft.MediaCenter.UI.Timer();
                    mainBackgroundTimer.AutoRepeat = true;
                    mainBackgroundTimer.Tick += new EventHandler(mainBackgroundTimer_Tick);
                    int rotationInSeconds = Properties.Settings.Default.MainPageBackDropRotationInSeconds;
                    int rotationInMilliseconds = rotationInSeconds * 1000;
                    mainBackgroundTimer.Interval = rotationInMilliseconds;
                    mainBackgroundTimer.Enabled = true;
                    mainBackgroundTimer.Start();
                }

                return;
            }
        }

        void mainBackgroundTimer_Tick(object sender, EventArgs e)
        {
            SetPrimaryBackgroundImage();
        }

        public Image PrimaryBackgroundImage
        {
            get { return primaryBackgroundImage; }
            set
            {
                primaryBackgroundImage = value;
                FirePropertyChanged("PrimaryBackgroundImage");
            }
        }

        public string RevisionNumber
        {
            get
            {
                try
                {
                    Assembly _assembly = Assembly.GetExecutingAssembly();
                    Stream _txtStream = _assembly.GetManifestResourceStream("Library.Revision.txt");
                    StreamReader _txtStreamReader = new StreamReader(_txtStream);
                    string revisionNumber = _txtStreamReader.ReadToEnd();
                    _txtStreamReader.Close();
                    _txtStream.Close();
                    return revisionNumber;
                }
                catch (Exception)
                {
                    return @"Unknown";
                }
            }
        }

        public int DistanceToMoveCloserBasedOnAngleOrRotation
        {
            get
            {
                int originalHeight = Properties.Settings.Default.CarouselItemWidth;
                double newWidth = originalHeight * Math.Cos(Convert.ToDouble(currentAngleDegrees));
                int distanceToMove = originalHeight - Convert.ToInt32(newWidth);

                return distanceToMove;
            }
        }

        public Inset MoveToInset
        {
            get { return new Inset(0, 0, DistanceToMoveCloserBasedOnAngleOrRotation, 0); }
        }

        public int CurrentAngleDegrees
        {
            get { return currentAngleDegrees; }
            set { currentAngleDegrees = value; }
        }

        public int CurrentFocusedItemIndex
        {
            get { return currentFocusedItemIndex; }
            set { currentFocusedItemIndex = value; }
        }

        public int CurrentItemIndexPosition
        {
            get { return currentItemIndexPosition; }
            set { currentItemIndexPosition = value; }
        }

        public string TranscodeStatus
        {
            get { return transcodeStatus; }
            set
            {
                transcodeStatus = value;
                FirePropertyChanged("TranscodeStatus");
            }
        }

        public Boolean IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                FirePropertyChanged("IsBusy");
            }
        }

        public Boolean IsStartingTranscodingJob
        {
            get { return isStartingTranscodingJob; }
            set
            {
                isStartingTranscodingJob = value;
                //FirePropertyChanged("IsStartingTranscodingJob");
            }
        }       

        public OMLApplication()
            : this(null, null)
        {
            DebugLine("[OMLApplication] Empty Constructor called");
        }

        ~OMLApplication()
        {
            if (mainBackgroundTimer != null)
            {
                mainBackgroundTimer.Stop();
                mainBackgroundTimer.Enabled = false;
                mainBackgroundTimer.Dispose();
            }
        }


        private Library.Code.V3.MediaChangeManager mediaChangers;
        public Library.Code.V3.MediaChangeManager MediaChangers
        {
            get
            { return this.mediaChangers; }
        }

        public OMLApplication(HistoryOrientedPageSession session, AddInHost host)
        {
            #if LAYOUT_V3
            //sets up the hooks for context menu
            this.hooker = new Library.Code.V3.MoreInfoHooker2();
            #endif


            this._session = session;
            AddInHost.Current.MediaCenterEnvironment.PropertyChanged +=new PropertyChangedEventHandler(MediaCenterEnvironment_PropertyChanged);

            try { // borrowed from vmcNetFlix project on google-code
                bool isConsole = false;
                if (host.MediaCenterEnvironment.Capabilities.ContainsKey("Console"))
                {
                    isConsole = (bool)host.MediaCenterEnvironment.Capabilities["Console"];
                }
                bool isVideo = false;
                if (host.MediaCenterEnvironment.Capabilities.ContainsKey("Video"))
                {
                    isVideo = (bool)host.MediaCenterEnvironment.Capabilities["Video"];
                }
                if (isConsole == false)
                {
                    if (isVideo == true)
                        _isExtender = true;
                }
            } catch (Exception)
            {
                _isExtender = false;
            }
#if DEBUG_EXT
            System.Diagnostics.Debugger.Launch();
            _isExtender = true;
#endif
#if DEBUG
            OMLApplication.DebugLine("[OMLApplication] MediaCenterEnvironment.Capabilities:");
            foreach (KeyValuePair<string, object> cap in host.MediaCenterEnvironment.Capabilities)
                try { DebugLine("  ['{0}'] = '{1}'", cap.Key, cap.Value); }
                catch { }
#else
            OMLApplication.DebugLine("[OMLApplication] constructor called");
#endif
            this._host = host;
            _singleApplicationInstance = this;
            I18n.InitializeResourceManager();
            string uiCulture = OMLSettings.UILanguage;
            if (!string.IsNullOrEmpty(uiCulture)) I18n.SetCulture(new CultureInfo(uiCulture));
            _nowPlayingMovieName = "Playing an unknown movie";
        }

        void MediaExperience_PropertyChanged(IPropertyObject sender, string property)
        {
            DebugLine("[OMLApplication] MediaExperience Property {0} changed", property);
        }

        // this is the context from the Media Center menu
        public void Startup(string context)
        {            
            OMLApplication.DebugLine("[OMLApplication] Startup({0}) {1}", context, IsExtender ? "Extender" : "Native");
#if CAROUSEL
            _session.GoToPage(@"resx://Library/Library.Resources/Trailers");
            return;
#endif
#if LAYOUT_V2
            OMLProperties properties = new OMLProperties();
            properties.Add("Application", this);
            properties.Add("UISettings", new UISettings());
            properties.Add("Settings", new Settings());
            properties.Add("I18n", I18n.Instance);
            properties.Add("Gallery", new GalleryV2(properties, _titles));
            //properties.Add("Gallery", new BaseGallery(properties, _titles));
            _session.GoToPage(@"resx://Library/Library.Resources/NewMenu", properties);
            return;
#endif
#if LAYOUT_V3

            System.Diagnostics.Debug.Assert(false);
            
            this.mediaChangers = new Library.Code.V3.MediaChangeManager();
            
            //System.Collections.ObjectModel.Collection<MediaChanger> changers=MediaCenterEnvironment.MediaChangers;
            //System.Collections.ObjectModel.Collection<DiscData> discs = changers[0].GetSlotDiscData();
            //System.Collections.ObjectModel.Collection<DiscData> discs2 = changers[0].GetDriveDiscData();

            //foreach (DiscData disc in discs)
            //{
            //    if (disc.DiscType == DiscType.MovieDvd)
            //    {
            //        string s = disc.DiscId;
            //        //System.Xml.XmlDocument doc = this.GetDVDInfo(s);
            //        Console.WriteLine(string.Format("DISCNAME:{0}", disc.VolumeLabel));
            //    }
            //    if (disc.DiscType == DiscType.Unknown)
            //    {
            //        changers[0].UnloadDisc(0);
            //        changers[0].LoadDisc(disc.Address, 0);
            //        changers[0].RescanDisc(0);
            //    }
            //}
            //this is the POC testing code for the v3 UI.
            //it currently populates the basic structure with test data
            #region v3POC

            SetPrimaryBackgroundImage();
            List<Title> titles = new List<Title>(TitleCollectionManager.GetAllTitles());

            //primaryBackgroundImage
            //this is temp to test controls
            OMLProperties properties = new OMLProperties();
            properties.Add("Application", this);
            properties.Add("UISettings", new UISettings());
            properties.Add("Settings", new Settings());
            properties.Add("I18n", I18n.Instance);
            //v3 main gallery
            Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage();
            //description
            gallery.Description = "OML";
            //size of the galleryitems
            //DEPRECATED
            //gallery.ItemSize = Library.Code.V3.GalleryItemSize.Small;

            gallery.Model = new Library.Code.V3.BrowseModel(gallery);
            //commands at top of screen
            gallery.Model.Commands = new ArrayListDataSet(gallery);

            //create the context menu
            Library.Code.V3.ContextMenuData ctx = new Library.Code.V3.ContextMenuData();

            //create the settings cmd
            Library.Code.V3.ThumbnailCommand settingsCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            settingsCmd.Description = "settings";
            settingsCmd.DefaultImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_BrowseCmd_Settings");
            settingsCmd.DormantImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_BrowseCmd_Settings_Dormant");
            settingsCmd.FocusImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_BrowseCmd_Settings_Focus");
            //no invoke for now
            //settingsCmd.Invoked += new EventHandler(this.SettingsHandler);
            settingsCmd.Invoked += new EventHandler(settingsCmd_Invoked);
            gallery.Model.Commands.Add(settingsCmd);

            //create the Search cmd
            Library.Code.V3.ThumbnailCommand searchCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            searchCmd.Description = "search";
            searchCmd.DefaultImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Search");
            searchCmd.DormantImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Search_Dormant");
            searchCmd.FocusImage = new Image("resx://Library/Library.Resources/V3_Controls_Common_Browse_Cmd_Search_Focus");
            searchCmd.Invoked += new EventHandler(searchCmd_Invoked);
            gallery.Model.Commands.Add(searchCmd);

            //some ctx items
            Library.Code.V3.ThumbnailCommand viewListCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            viewListCmd.Description = "View List";
            viewListCmd.Invoked += new EventHandler(viewCmd_Invoked);

            Library.Code.V3.ThumbnailCommand viewSmallCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            viewSmallCmd.Invoked += new EventHandler(viewCmd_Invoked);
            viewSmallCmd.Description = "View Small";

            Library.Code.V3.ThumbnailCommand viewLargeCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            viewLargeCmd.Invoked += new EventHandler(viewCmd_Invoked);
            
            viewLargeCmd.Description = "View Large";

            Library.Code.V3.ThumbnailCommand viewSettingsCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            //viewSettingsCmd.Invoked += new EventHandler(this.SettingsHandler);
            viewSettingsCmd.Description = "Settings";
            

            //ctx.SharedItems.Add(viewLargeCmd);
            ctx.SharedItems.Add(viewSmallCmd);
            ctx.SharedItems.Add(viewListCmd);
            ctx.SharedItems.Add(viewSettingsCmd);

            Library.Code.V3.ThumbnailCommand viewMovieDetailsCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            viewMovieDetailsCmd.Description = "Movie Details";

            Library.Code.V3.ThumbnailCommand viewPlayCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            viewPlayCmd.Description = "Play";

            Library.Code.V3.ThumbnailCommand viewDeleteCmd = new Library.Code.V3.ThumbnailCommand(gallery);
            viewDeleteCmd.Description = "Delete";

            ctx.UniqueItems.Add(viewMovieDetailsCmd);
            ctx.UniqueItems.Add(viewPlayCmd);
            ctx.UniqueItems.Add(viewDeleteCmd);

            Command CommandContextPopOverlay = new Command();
            properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);

            //properties.Add("MenuData", ctx);
            gallery.ContextMenu = ctx;
            //the pivots
            gallery.Model.Pivots = new Choice(gallery, "desc", new ArrayListDataSet(gallery));



            //twoRowGalleryItemPoster
            #region oneRowGalleryItemPoster
            VirtualList galleryList = new VirtualList(gallery, null);
            foreach (Title t in titles)
            {
                //galleryList.Add(this.CreateGalleryItem(t));
                galleryList.Add(new Library.Code.V3.MovieItem(t, galleryList));
            }


            Library.Code.V3.BrowsePivot p = new Library.Code.V3.BrowsePivot(gallery, "one row", "No titles were found.", galleryList);
            p.ContentLabel = "OML";
            p.SupportsJIL = true;
            p.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            p.ContentItemTemplate = "oneRowGalleryItemPoster";
            p.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
            gallery.Model.Pivots.Options.Add(p);
            #endregion oneRowGalleryItemPoster



            //twoRowGalleryItemPoster
            #region twoRowGalleryItemPoster
            VirtualList galleryListGenres = new VirtualList(gallery, null);
            foreach (Title t in titles)
            {
                galleryListGenres.Add(new Library.Code.V3.MovieItem(t, galleryListGenres));
            }

            Library.Code.V3.BrowsePivot p2 = new Library.Code.V3.BrowsePivot(gallery, "two row", "No titles were found.", galleryListGenres);
            p2.ContentLabel = "OML";
            p2.SupportsJIL = true;
            p2.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            p2.ContentItemTemplate = "twoRowGalleryItemPoster";
            p2.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
            gallery.Model.Pivots.Options.Add(p2);
            #endregion twoRowGalleryItemPoster

            //ListViewItem
            #region ListViewItem
            VirtualList galleryListListViewItem = new VirtualList(gallery, null);
            foreach (Title t in titles)
            {
                galleryListListViewItem.Add(new Library.Code.V3.MovieItem(t, galleryListListViewItem));
            }

            Library.Code.V3.BrowsePivot p3 = new Library.Code.V3.BrowsePivot(gallery, "list", "No titles were found.", galleryListListViewItem);
            p3.ContentLabel = "OML";
            p3.SupportsJIL = true;
            p3.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            p3.ContentItemTemplate = "ListViewItem";
            p3.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
            gallery.Model.Pivots.Options.Add(p3);
            #endregion ListViewItem

            //Grouped
            #region Grouped
            
            VirtualList groupedGalleryListListViewItem = new VirtualList(gallery, null);
            int i = 1;
            foreach (Title t in titles)
            {
                i++;
                groupedGalleryListListViewItem.Add(new Library.Code.V3.MovieItem(t, groupedGalleryListListViewItem));
                if (i > 20)
                    break;
            }
            Library.Code.V3.GalleryItem testtgenre = new Library.Code.V3.GalleryItem();
            testtgenre.Description = "Comedy";
            testtgenre.Metadata = "12 titles";
            testtgenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_comedy");
            //groupedGalleryListListViewItem.Add(testtgenre);

            Library.Code.V3.BrowseGroup testGroup = new Library.Code.V3.BrowseGroup();//this, "Sample Group", groupedGalleryListListViewItem);
            testGroup.Owner = gallery;
            testGroup.Description = "Group 1";
            testGroup.Metadata = "12 titles";
            testGroup.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_mystery");
            testGroup.Content = groupedGalleryListListViewItem;
            testGroup.ContentLabelTemplate = Library.Code.V3.BrowseGroup.StandardContentLabelTemplate;
            
            VirtualList groupListView = new VirtualList(gallery, null);

            
            //groupListView.Add(testtgenre);
            groupListView.Add(testGroup);
            
            VirtualList groupedGalleryListListViewItem2 = new VirtualList(gallery, null);

            foreach (Title t in titles)
            {
                i++;
                groupedGalleryListListViewItem2.Add(new Library.Code.V3.MovieItem(t, groupedGalleryListListViewItem2));
                if (i > 50)
                    break;
            }

            Library.Code.V3.BrowseGroup testGroup2 = new Library.Code.V3.BrowseGroup();//this, "Sample Group", groupedGalleryListListViewItem);
            testGroup2.Owner = gallery;
            testGroup2.Description = "Group 2";
            testGroup2.Metadata = "12 titles";
            testGroup2.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_mystery");
            testGroup2.Content = groupedGalleryListListViewItem2;
            testGroup2.ContentLabelTemplate = Library.Code.V3.BrowseGroup.StandardContentLabelTemplate;


            groupListView.Add(testGroup2);

            Library.Code.V3.BrowsePivot group1 = new Library.Code.V3.BrowsePivot(gallery, "group", "No titles were found.", groupListView);
            group1.ContentLabel = "OML";
            group1.SupportsJIL = false;
            group1.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            group1.ContentItemTemplate = "GalleryGroup";
            group1.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;

            gallery.Model.Pivots.Options.Add(group1);
            #endregion grouped

            //Genres test
            #region Genres
            VirtualList galleryListthreeRowGalleryItemPoster = new VirtualList(gallery, null);
            Library.Code.V3.GalleryItem actionAdventureGenre = new Library.Code.V3.GalleryItem();
            actionAdventureGenre.Description = "Action/Adventure";
            actionAdventureGenre.Metadata = "12 titles";
            actionAdventureGenre.DefaultImage=new Image("resx://Library/Library.Resources/Genre_Sample_actionadventure");
            galleryListthreeRowGalleryItemPoster.Add(actionAdventureGenre);

            Library.Code.V3.GalleryItem comedyGenre = new Library.Code.V3.GalleryItem();
            comedyGenre.Description = "Comedy";
            comedyGenre.Metadata = "12 titles";
            comedyGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_comedy");
            galleryListthreeRowGalleryItemPoster.Add(comedyGenre);

            Library.Code.V3.GalleryItem documentaryGenre = new Library.Code.V3.GalleryItem();
            documentaryGenre.Description = "Documentary";
            documentaryGenre.Metadata = "12 titles";
            documentaryGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_documentary");
            galleryListthreeRowGalleryItemPoster.Add(documentaryGenre);

            Library.Code.V3.GalleryItem dramaGenre = new Library.Code.V3.GalleryItem();
            dramaGenre.Description = "Drama";
            dramaGenre.Metadata = "12 titles";
            dramaGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_drama");
            galleryListthreeRowGalleryItemPoster.Add(dramaGenre);

            Library.Code.V3.GalleryItem kidsFamilyGenre = new Library.Code.V3.GalleryItem();
            kidsFamilyGenre.Description = "Kids/Family";
            kidsFamilyGenre.Metadata = "12 titles";
            kidsFamilyGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_kidsfamily");
            galleryListthreeRowGalleryItemPoster.Add(kidsFamilyGenre);

            Library.Code.V3.GalleryItem musicalGenre = new Library.Code.V3.GalleryItem();
            musicalGenre.Description = "Musical";
            musicalGenre.Metadata = "12 titles";
            musicalGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_musical");
            galleryListthreeRowGalleryItemPoster.Add(musicalGenre);

            Library.Code.V3.GalleryItem mysteryGenre = new Library.Code.V3.GalleryItem();
            mysteryGenre.Description = "Mystery";
            mysteryGenre.Metadata = "12 titles";
            mysteryGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_mystery");
            galleryListthreeRowGalleryItemPoster.Add(mysteryGenre);

            Library.Code.V3.GalleryItem otherGenre = new Library.Code.V3.GalleryItem();
            otherGenre.Description = "Other";
            otherGenre.Metadata = "12 titles";
            otherGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_other");
            galleryListthreeRowGalleryItemPoster.Add(otherGenre);

            Library.Code.V3.GalleryItem romanceGenre = new Library.Code.V3.GalleryItem();
            romanceGenre.Description = "Romance";
            romanceGenre.Metadata = "12 titles";
            romanceGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_romance");
            galleryListthreeRowGalleryItemPoster.Add(romanceGenre);

            Library.Code.V3.GalleryItem scienceFictionGenre = new Library.Code.V3.GalleryItem();
            scienceFictionGenre.Description = "Science Fiction";
            scienceFictionGenre.Metadata = "12 titles";
            scienceFictionGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_sciencefiction");
            galleryListthreeRowGalleryItemPoster.Add(scienceFictionGenre);

            Library.Code.V3.GalleryItem westernGenre = new Library.Code.V3.GalleryItem();
            westernGenre.Description = "Western";
            westernGenre.Metadata = "12 titles";
            westernGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_western");
            galleryListthreeRowGalleryItemPoster.Add(westernGenre);

            Library.Code.V3.GalleryItem noimageGenre = new Library.Code.V3.GalleryItem();
            noimageGenre.Description = "No Image ergeg ergege egegeg";
            noimageGenre.Metadata = "12 titles";
            noimageGenre.DefaultImage = new Image("resx://Library/Library.Resources/Genre_Sample_noimagegenre");
            galleryListthreeRowGalleryItemPoster.Add(noimageGenre);
            
            //foreach (Title t in _titles)
            //{
            //    galleryListthreeRowGalleryItemPoster.Add(this.CreateGalleryItem(t));
            //}

            Library.Code.V3.BrowsePivot p4 = new Library.Code.V3.BrowsePivot(gallery, "genres", "No titles were found.", galleryListthreeRowGalleryItemPoster);
            p4.ContentLabel = "OML";
            p4.SupportsJIL = true;
            p4.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            p4.ContentItemTemplate = "twoRowGalleryItemGenre";
            p4.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
            gallery.Model.Pivots.Options.Add(p4);
            #endregion Genres

            // add OML filters
            //System.Collections.Specialized.StringCollection filtersToShow =
            //    Properties.Settings.Default.MainFiltersToShow;
            IList<string> filtersToShow = OMLSettings.MainFiltersToShow;

            foreach (string filterName in filtersToShow)
            {
                TitleFilterType filterType = Filter.FilterStringToTitleType(filterName);
                Filter f = new Filter(null, filterType, null);//new MovieGallery()
                
                if (Filter.ShowFilterType(filterType))
                {
                    
                    ////IEnumerable<Title> titles = TitleCollectionManager.GetAllTitles();
                    //IEnumerable<Title> titles;
                    //if (filterType == TitleFilterType.DateAdded || filterType == TitleFilterType.VideoFormat)
                    //{
                    //    titles = TitleCollectionManager.GetAllTitles(); 
                    //}
                    //else
                    //{
                    //    titles = TitleCollectionManager.GetFilteredTitles(filterType, filterName);
                    //}

                    //VirtualList filteredGalleryList = new VirtualList(gallery, null);
                    //foreach (Title t in titles)
                    //{
                    //    //galleryList.Add(this.CreateGalleryItem(t));
                    //    filteredGalleryList.Add(new Library.Code.V3.MovieItem(t, filteredGalleryList));
                    //}
                    VirtualList filteredGalleryList = new VirtualList(gallery, null);
                    IList<GalleryItem> filteredTitles = f.GetGalleryItems();
                    foreach (GalleryItem item in filteredTitles)
                    {
                        //this is a temp hack
                        if(item.Name!=" All ")
                            filteredGalleryList.Add(new Library.Code.V3.MovieItem(item, filteredGalleryList));
                    }



                    Library.Code.V3.BrowsePivot filteredPivot = new Library.Code.V3.BrowsePivot(gallery, Filter.FilterTypeToString(filterType).ToLower(), "No titles were found.", filteredGalleryList);
                    filteredPivot.ContentLabel = "OML";
                    filteredPivot.SupportsJIL = true;
                    filteredPivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
                    filteredPivot.ContentItemTemplate = "ListViewItem";
                    filteredPivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
                    gallery.Model.Pivots.Options.Add(filteredPivot);
                }
            }
            //end add OML filters

            //oneRowChangerData

            #region oneRowChangerData
            if (this.mediaChangers != null && this.mediaChangers.KnownDiscs.Count > 0)
            {
                VirtualList changerGalleryList = new VirtualList(gallery, null);
                foreach (Library.Code.V3.DiscDataEx t in this.mediaChangers.KnownDiscs)
                {
                    //galleryList.Add(this.CreateGalleryItem(t));
                    if (t.DiscType == DiscType.MovieDvd)
                        changerGalleryList.Add(new Library.Code.V3.DVDChangerItem(t, changerGalleryList));
                }


                Library.Code.V3.BrowsePivot changerPivot = new Library.Code.V3.BrowsePivot(gallery, "changer", "No titles were found.", changerGalleryList);
                changerPivot.ContentLabel = "OML";
                changerPivot.SupportsJIL = true;
                changerPivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
                changerPivot.ContentItemTemplate = "oneRowGalleryItemPoster";
                changerPivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
                gallery.Model.Pivots.Options.Add(changerPivot);
            }
            #endregion oneRowChangerData

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
            deck.CommandPopOverlay.Invoked += new EventHandler(CommandPopOverlay_Invoked);
            deck.CommandClearOverlays = new Command();
            deck.CommandClearOverlays.Invoked += new EventHandler(CommandClearOverlays_Invoked);
            deck.CommandPushOverlay = new Command();
            deck.CommandPushOverlay.Invoked += new EventHandler(CommandPushOverlay_Invoked);

            //deck.AdditionalCommands.Add(cmd);
            properties.Add("SlideDeck", deck);
            properties.Add("CommandPopOverlay", deck.CommandPopOverlay);
            properties.Add("CommandClearOverlays", deck.CommandClearOverlays);
            properties.Add("CommandPushOverlay", deck.CommandPushOverlay);

            deck.Context = "hi";
            //_session.GoToPage(@"resx://Library/Library.Resources/V3_SlideDeck_Movie_Details", properties);

            gallery.Model.Pivots.Chosen = p;
            gallery.Model.Pivots.ChosenChanged += new EventHandler(Pivots_ChosenChanged);
            _session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
            _page = gallery;
            _deck = deck;

            #endregion v3POC
            return;
#endif

            // DISABLE THIS UNTIL ITS READY -- DJShultz 01/13/2009
            //OMLUpdater updater = new OMLUpdater();
            //ThreadPool.QueueUserWorkItem(new WaitCallback(updater.checkUpdate));

            //TheMovieDbBackDropDownloader downloader = new TheMovieDbBackDropDownloader();
            //foreach (Title t in Titles)
            //{
            //    downloader.SearchForTitle(t);
            //}
            SetPrimaryBackgroundImage();

            switch (context)
            {
                case "Menu":
                    // Before running strait to the menu, check to see if we want to run the first-time setup
                    if (!Properties.Settings.Default.HasRunSetup)
                    {
                        OMLApplication.DebugLine("[OMLApplication] firstrun, going to setup");
                        GoToSetup(new MovieGallery());
                        Properties.Settings.Default.HasRunSetup = true;
                        Properties.Settings.Default.Save();
                        return;
                    }

                    // we want the movie library - check the startup page
                    if (OMLSettings.StartPage == Filter.Home)
                    {
                        OMLApplication.DebugLine("[OMLApplication] going to Menu Page");
                        GoToMenu(new MovieGallery());
                    }
                    else
                    {                                                
                        // see if they've selected a subfilter
                        // the unwatched is a special case until we add a user setting to determine the subfilter
                        if (!string.IsNullOrEmpty(OMLSettings.StartPageSubFilter)
                            || OMLSettings.StartPage == Filter.Unwatched)
                        {
                            // go to the subfilter
                            GoToMenu(
                                new MovieGallery(
                                    new TitleFilter(Filter.FilterStringToTitleType(OMLSettings.StartPage),
                                        OMLSettings.StartPageSubFilter)));
                        }
                        else
                        {
                            // go to the selection list
                            GoToSelectionList(new Filter(new MovieGallery(), Filter.FilterStringToTitleType(OMLSettings.StartPage), null));               
                        }
                    }
                    return;
                case "Settings":
                    OMLApplication.DebugLine("[OMLApplication] going to Settings Page");
                    GoToSettingsPage(new MovieGallery(TitleCollectionManager.GetAllTitles(), Filter.Settings));
                    return;
                case "Trailers":
                    OMLApplication.DebugLine("[OMLApplication] going to Trailers Page");
                    GoToTrailersPage();
                    return;
                case "About":
                    OMLApplication.DebugLine("[OMLApplication] going to About Page");
                    GoToAboutPage(new MovieGallery(TitleCollectionManager.GetAllTitles(), Filter.About));
                    return;
                default:
                    OMLApplication.DebugLine("[OMLApplication] going to Default (Menu) Page");
                    GoToMenu(new MovieGallery());
                    return;
            }
        }

        void settingsCmd_Invoked(object sender, EventArgs e)
        {
            DebugLine("[OMLApplication] GoToSettings_AppearancePage()");
            if (_session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Library.Code.V3.MediaChangerManagerPage page = new Library.Code.V3.MediaChangerManagerPage();
                properties["Page"] = page;
                properties["Application"] = this;

                _session.GoToPage("resx://Library/Library.Resources/V3_MediaChangerManagerSettings", properties);
            }
        }

        //v3 temp
        void searchCmd_Invoked(object sender, EventArgs e)
        {
            Library.Code.V3.MoviesSearchPage page = new Library.Code.V3.MoviesSearchPage();
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Page"] = page;
            properties["Application"] = this;
            this._session.GoToPage("resx://Library/Library.Resources/V3_MoviesSearchPage", properties);
        }

        //v3 temp
        void Pivots_ChosenChanged(object sender, EventArgs e)
        {
            Library.Code.V3.BrowsePivot p = (Library.Code.V3.BrowsePivot)this._page.Model.Pivots.Chosen;


            //change the buttons based on which view was invoked
            ICommand ctx0 = (ICommand)_page.ContextMenu.SharedItems[0];
            ICommand ctx1 = (ICommand)_page.ContextMenu.SharedItems[1];

            switch (p.ContentItemTemplate)
            {
                case "oneRowGalleryItemPoster":
                    ctx0.Description = "View Small";
                    ctx1.Description = "View List";
                    break;
                case "twoRowGalleryItemPoster":
                    ctx0.Description = "View Large";
                    ctx1.Description = "View List";
                    break;
                case "ListViewItem":
                    ctx0.Description = "View Large";
                    ctx1.Description = "View Small";
                    break;
            }
        }

        //v3 temp
        public void GoToBackPage()
        {
            DebugLine("[OMLApplication] GoToBackPage()");
            if (_session != null)
            {
                _session.BackPage();
            }
        }

        //v3 temp
        public void SettingsHandler(object sender, EventArgs e)
        {
            _session.GoToPage("resx://Library/Library.Resources/Settings_Main", CreateProperties(true, true, null));
        }

        public void GoToSettings_AppearancePage(SettingsUIWrapper classSettingsUIWrapper)
        {
            DebugLine("[OMLApplication] GoToSettings_AppearancePage()");
            if (_session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Settings settings = new Settings();
                classSettingsUIWrapper.Init(settings);
                properties["ClassSettingsUIWrapper"] = classSettingsUIWrapper;
                properties["Settings"] = settings;
                properties["Application"] = this;

                _session.GoToPage("resx://Library/Library.Resources/Settings_Appearance", properties);
            }
        }

        public void GoToSettings_Appearance_GeneralPage(SettingsUIWrapper classSettingsUIWrapper)
        {
            DebugLine("[OMLApplication] GoToSettings_Appearance_GeneralPage()");
            if (_session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Settings settings = new Settings();
                classSettingsUIWrapper.Init(settings);
                properties["ClassSettingsUIWrapper"] = classSettingsUIWrapper;
                properties["Settings"] = settings;
                properties["Application"] = this;

                _session.GoToPage("resx://Library/Library.Resources/Settings_Appearance_General", properties);
            }
        }

        public void GoToSettings_Appearance_DetailViewPage(SettingsUIWrapper classSettingsUIWrapper)
        {
            DebugLine("[OMLApplication] GoToSettings_Appearance_GeneralPage()");
            if (_session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Settings settings = new Settings();
                classSettingsUIWrapper.Init(settings);
                properties["ClassSettingsUIWrapper"] = classSettingsUIWrapper;
                properties["Settings"] = settings;
                properties["Application"] = this;

                _session.GoToPage("resx://Library/Library.Resources/Settings_Appearance_DetailView", properties);
            }
        }

        public void GoToSettings_Appearance_GalleryViewPage(SettingsUIWrapper classSettingsUIWrapper)
        {
            DebugLine("[OMLApplication] GoToSettings_Appearance_GalleryViewPage()");
            if (_session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Settings settings = new Settings();
                classSettingsUIWrapper.Init(settings);
                properties["ClassSettingsUIWrapper"] = classSettingsUIWrapper;
                properties["Settings"] = settings;
                properties["Application"] = this;

                _session.GoToPage("resx://Library/Library.Resources/Settings_Appearance_GalleryView", properties);
            }
        }

        public void GoToSettings_ExternalInterfacePage(SettingsUIWrapper classSettingsUIWrapper)
        {
            DebugLine("[OMLApplication] GoToSettings_ExternalInterfacePage()");
            if (_session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Settings settings = new Settings();
                classSettingsUIWrapper.Init(settings);
                properties["ClassSettingsUIWrapper"] = classSettingsUIWrapper;
                properties["Settings"] = settings;
                properties["Application"] = this;

                _session.GoToPage("resx://Library/Library.Resources/Settings_ExternalInterface", properties);
            }
        }

        public void GoToSettings_FiltersPage(SettingsUIWrapper classSettingsUIWrapper)
        {
            DebugLine("[OMLApplication] GoToSettings_FiltersPage()");
            if (_session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Settings settings = new Settings();
                classSettingsUIWrapper.Init(settings);
                properties["ClassSettingsUIWrapper"] = classSettingsUIWrapper;
                properties["Settings"] = settings;
                properties["Application"] = this;

                _session.GoToPage("resx://Library/Library.Resources/Settings_Filters", properties);
            }
        }

        public void GoToSettings_TrailersPage(SettingsUIWrapper classSettingsUIWrapper)
        {
            DebugLine("[OMLApplication] GoToSettings_TrailersPage()");
            if (_session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Settings settings = new Settings();
                classSettingsUIWrapper.Init(settings);
                properties["ClassSettingsUIWrapper"] = classSettingsUIWrapper;
                properties["Settings"] = settings;
                properties["Application"] = this;

                _session.GoToPage("resx://Library/Library.Resources/Settings_Trailers", properties);
            }
        }

        public void GoToSettings_AboutPage()
        {
            DebugLine("[OMLApplication] GoToSettings_AboutPage()");
            if (_session != null)
            {
                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties["Application"] = this;

                _session.GoToPage("resx://Library/Library.Resources/Settings_About", properties);
            }
        }

        //v3 temp
        void viewCmd_Invoked(object sender, EventArgs e)
        {

            Library.Code.V3.BrowsePivot p = (Library.Code.V3.BrowsePivot)this._page.Model.Pivots.Chosen;
            base.FirePropertyChanged("MoreInfo");

            ICommand invokedCmd = (ICommand)sender;

            //change the buttons based on which view was invoked
            ICommand ctx0 = (ICommand)_page.ContextMenu.SharedItems[0];
            ICommand ctx1 = (ICommand)_page.ContextMenu.SharedItems[1];

            switch (invokedCmd.Description)
            {
                case "View Large":
                    p.ContentItemTemplate = "oneRowGalleryItemPoster";
                    ctx0.Description = "View Small";
                    ctx1.Description = "View List";
                    break;
                case "View Small":
                    p.ContentItemTemplate = "twoRowGalleryItemPoster";
                    ctx0.Description = "View Large";
                    ctx1.Description = "View List";
                    break;
                case "View List":
                    p.ContentItemTemplate = "ListViewItem";
                    ctx0.Description = "View Large";
                    ctx1.Description = "View Small";
                    break;
            }

            //p.ContentItemTemplate = "ListViewItem";
            p.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
        }

        //v3 temp
        void viewListCmd_Invoked(object sender, EventArgs e)
        {
            Library.Code.V3.BrowsePivot p = (Library.Code.V3.BrowsePivot)this._page.Model.Pivots.Chosen;
            base.FirePropertyChanged("MoreInfo");
            p.ContentItemTemplate = "ListViewItem";
            p.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
        }

        //v3 temp
        void viewSmallCmd_Invoked(object sender, EventArgs e)
        {
            Library.Code.V3.BrowsePivot p = (Library.Code.V3.BrowsePivot)this._page.Model.Pivots.Chosen;
            base.FirePropertyChanged("MoreInfo");
            p.ContentItemTemplate = "twoRowGalleryItemPoster";
            p.DetailTemplate = Library.Code.V3.BrowsePivot.StandardDetailTemplate;
        }

        //v3 temp
        void viewLargeCmd_Invoked(object sender, EventArgs e)
        {
            Library.Code.V3.BrowsePivot p = (Library.Code.V3.BrowsePivot)this._page.Model.Pivots.Chosen;
            base.FirePropertyChanged("MoreInfo");
            p.ContentItemTemplate = "oneRowGalleryItemPoster";
            p.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
        }
        //void hooker_ButtonPressed(object sender, Library.Code.V3.RemoteControlEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    this.CatchMoreInfo();
        //}

        //v3 temp
        public void CatchMoreInfo()
        {
            if (this._moreInfo == true)
                this._moreInfo = false;
            else
                this._moreInfo = true;
            base.FirePropertyChanged("MoreInfo");
        }

        //v3 temp
        private bool _moreInfo = false;
        public bool MoreInfo
        {
            get { return this._moreInfo; }
            set { this._moreInfo = value; }
        }

        //v3 temp
        void CommandPushOverlay_Invoked(object sender, EventArgs e)
        {

        }

        //v3 temp
        void CommandClearOverlays_Invoked(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        //v3 temp
        Library.Code.V3.MovieDetailsSlideDeck _deck;
        void CommandPopOverlay_Invoked(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

        }

        //v3 temp
        Library.Code.V3.GalleryPage _page;

        //v3 temp
        public static void AppendSeparatedValue(ref string text, string value, string seperator)
        {
            if (String.IsNullOrEmpty(text))
                text = value;
            else
                text = string.Format("{0}{1}{2}", text, seperator, value);
        }

        public void Uninitialize()
        {
            try
            {
                ExtenderDVDPlayer.Uninitialize(TitleCollectionManager.GetAllTitles());
            }
            catch (Exception err)
            {
                DebugLine("Unhandled Exception: {0}", err);
            }
            finally
            {
                // close the db connections
                TitleCollectionManager.CloseDBConnection();
                WatcherSettingsManager.CloseDBConnection();
            }            
        }

        public static OMLApplication Current
        {
            get { return _singleApplicationInstance; }
        }

        public HistoryOrientedPageSession Session
        {
            get { return _session; }
        }

        public MediaCenterEnvironment MediaCenterEnvironment
        {
            get
            {
                if (_host == null) return null;
                return _host.MediaCenterEnvironment;
            }
        }

        public void GoToSetup(MovieGallery gallery)
        {
            DebugLine("[OMLApplication] GoToSetup()");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/FirstRunSetup", CreateProperties(true, false, gallery));
            }
        }

        public void GoToSettingsPage(MovieGallery gallery)
        {
            DebugLine("[OMLApplication] GoToSettingsPage()");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Settings", CreateProperties(true, true, gallery));
            }
        }

        public void GoToTrailersPage()
        {
            DebugLine("[OMLApplication] GoToTrailersPage()");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Trailers", CreateProperties(true, true, null));
            }
        }

        public void GoToAboutPage(MovieGallery gallery)
        {
            DebugLine("[OMLApplication] GotoAboutPage()");
            if (_session != null)
                _session.GoToPage("resx://Library/Library.Resources/About", CreateProperties(true, false, gallery));
        }        

        public void GoToMenu(MovieGallery gallery)
        {
            DebugLine("[OMLApplication] GoToMenu(Gallery, #{0} Movies)", gallery.Movies.Count);
            Dictionary<string, object> properties = CreateProperties(true, false, gallery);

            if (OMLSettings.MovieView == GalleryView.CoverArtWithAlpha &&
                gallery.Movies.Count < 30)
            {
                // alpha falls back to cover art view if there's not enough items
                properties["GalleryView"] = GalleryView.CoverArt;
            }
            else
            {
                properties["GalleryView"] = OMLSettings.MovieView;
            }

            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Menu", properties);
            }
            //IsBusy = true; why do this?
        }

        public void GoToSelectionList(Filter filter)
        {
            // currently the selection list page uses the same gallery object as the previous page
            // we shoud look into refactoring this so the selection logic is in a different object than
            // the movie list so we can deal with selections across pages better
            MovieGallery gallery = filter.Gallery;

            // reset the index
            gallery.FocusIndex.Value = 0;

            //DebugLine("[OMLApplication] GoToSelectionList(#{0} items, list name: {1}, gallery: {2})", list.Count, listName, galleryView);
            Dictionary<string, object> properties = CreateProperties(true, false, gallery);
            properties["MovieBrowser"] = gallery;
            properties["List"] = filter.GetGalleryItems();
            properties["ListName"] = filter.Title;
            properties["GalleryView"] = filter.GetViewForFilter();

            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/SelectionList", properties);
            }            
        }

        public void GoToDetails(MovieDetailsPage page)
        {
            DebugLine("[OMLApplication] GoToDetails({0})", page);
            if (page == null)
                throw new System.Exception("The method or operation is not implemented.");

            //
            // Construct the arguments dictionary and then navigate to the
            // details page template.
            //
            Dictionary<string, object> properties = CreateProperties(true, false, null);
            properties["DetailsPage"] = page;

            // If we have no page session, just spit out a trace statement.
            if (_session != null)
            {
                switch (OMLSettings.DetailsView)
                {
                    case "Background Boxes":
                        _session.GoToPage("resx://Library/Library.Resources/DetailsPage_Boxes", properties);
                        break;

                    case "Original":
                    default:
                        _session.GoToPage("resx://Library/Library.Resources/DetailsPage", properties);
                        break;
                }                
            }
        }

        public static void DebugLine(string msg, params object[] paramArray)
        {
            Utilities.DebugLine(msg, paramArray);
        }

        // properties
        public bool IsExtender
        {
            get { return _isExtender; }
        }

        public string NowPlayingMovieName
        {
            get { return _nowPlayingMovieName; }
            set
            {
                if (_nowPlayingMovieName == value)
                    return;
                Utilities.DebugLine("[OMLApplication] NowPlayingMovieName {0}", value);
                _nowPlayingMovieName = value;
                FirePropertyChanged("NowPlaying");
            }
        }

        public PlayState NowPlayingStatus
        {
            get { return _nowPlayingStatus; }
            set 
            {
                if (_nowPlayingStatus == value)
                    return;
                Utilities.DebugLine("[OMLApplication] NowPlayingStatus {0}", value);
                FirePropertyChanged("NowPlaying");  
                _nowPlayingStatus = value; 
            }
        }

        public string NowPlaying
        {
            get { return NowPlayingStatus.ToString() + ": " + NowPlayingMovieName; }
        }

        /*public TitleCollection ReloadTitleCollection()
        {
            DebugLine("[OMLApplication] ReloadTitleCollection()");
            _titles.loadTitleCollection();
            return _titles;
        }*/

        /*public void SaveTitles()
        {
            //_titles.saveTitleCollection();
        }*/

        public static void ExecuteSafe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                DebugLine("Unhandled Exception: {0}", ex);
                DialogResult res = Current.MediaCenterEnvironment.Dialog("To rethrow exception select yes\n" + ex.ToString(), 
                    "Unhandled Exception logged in debug.txt", DialogButtons.Yes | DialogButtons.No, 10, true);
                if (res == DialogResult.Yes)
                    throw;
            }
        }

        private Dictionary<string, object> CreateProperties(bool uiSettings, bool settings, MovieGallery gallery)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["I18n"] = I18n.Instance;
            if (uiSettings)
                properties["UISettings"] = new UISettings();
            if (settings)
                properties["Settings"] = new Settings();
            if (gallery != null)
                properties["MovieBrowser"] = gallery;
            return properties;
        }

        static public void MediaCenterEnvironment_PropertyChanged(IPropertyObject sender, string property)
        {
            DebugLine("[OMLApplication] Property {0} changed on the MediaCenterEnvironment", property);
        }

        static public void AddInHost_PropertyChanged(IPropertyObject sender, string property)
        {
            DebugLine("[OMLApplication] Property {0} changed on the AddInHost", property);
        }
        // private data
        private string _nowPlayingMovieName;
        private PlayState _nowPlayingStatus;

        private static OMLApplication _singleApplicationInstance;
        private AddInHost _host;
        private HistoryOrientedPageSession _session;

        private bool _isExtender;
    }
}
