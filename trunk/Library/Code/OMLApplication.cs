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
        private Library.Code.V3.IsMouseActiveHooker mouseActiveHooker;

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
            this.primaryBackgroundImageAlpha = Properties.Settings.Default.MainPageBackDropAlpha;
            if (Properties.Settings.Default.EnableMainPageBackDrop)
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
                    if (mainBackgroundTimer == null && iTotalBackgroundImages>1)
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
        }

        void mainBackgroundTimer_Tick(object sender, EventArgs e)
        {
            SetPrimaryBackgroundImage();
        }

        private float primaryBackgroundImageAlpha = 1;
        public float PrimaryBackgroundImageAlpha
        {
            get { return primaryBackgroundImageAlpha; }
            set
            {
                if (this.primaryBackgroundImageAlpha != value)
                {
                    this.primaryBackgroundImageAlpha = value;
                    FirePropertyChanged("PrimaryBackgroundImageAlpha");
                }
            }
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
            { 
                if(this.mediaChangers==null)
                    this.mediaChangers = new Library.Code.V3.MediaChangeManager();
                return this.mediaChangers; 
            }
        }

        public OMLApplication(HistoryOrientedPageSession session, AddInHost host)
        {
            #if LAYOUT_V3
            //sets up the hooks for context menu
            this.hooker = new Library.Code.V3.MoreInfoHooker2();
            this.mouseActiveHooker = new Library.Code.V3.IsMouseActiveHooker();
            this.mouseActiveHooker.MouseActive += new Library.Code.V3.IsMouseActiveHooker.MouseActiveHandler(mouseActiveHooker_MouseActive);
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

        private Boolean isMouseActive = false;
        public Boolean IsMouseActive
        {
            get { return isMouseActive; }
            set
            {
                if (isMouseActive != value)
                {
                    isMouseActive = value;
                    FirePropertyChanged("IsMouseActive");
                }
            }
        }

        void mouseActiveHooker_MouseActive(Library.Code.V3.IsMouseActiveHooker m, Library.Code.V3.MouseActiveEventArgs e)
        {
            this.IsMouseActive = e.MouseActive;
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

            #region v3POC

            SetPrimaryBackgroundImage();

            switch (context)
            {
                case "Settings":
                    {
                        Dictionary<string, object> properties = new Dictionary<string, object>();

                        Library.Code.V3.SettingsManager page = new Library.Code.V3.SettingsManager();
                        properties["Page"] = page;
                        properties["Application"] = OMLApplication.Current;

                        OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_Settings_SettingsManager", properties);
                        return;
                    }
                case "Search":
                    {
                        Dictionary<string, object> properties = new Dictionary<string, object>();

                        Library.Code.V3.MoviesSearchPage page = new Library.Code.V3.MoviesSearchPage();
                        properties["Page"] = page;
                        properties["Application"] = OMLApplication.Current;

                        OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_MoviesSearchPage", properties);
                        return;
                    }
                case "Trailers":
                    {
                        OMLProperties properties = new OMLProperties();
                        properties.Add("Application", this);
                        properties.Add("I18n", I18n.Instance);
                        Library.Code.V3.TrailerPage gallery = new Library.Code.V3.TrailerPage("trailers");
                        Command CommandContextPopOverlay = new Command();
                        properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);
                        properties.Add("Page", gallery);
                        _session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
                        return;
                    }
                case "Custom1":
                    {
                        UserFilter filt = new UserFilter(OMLEngine.Properties.Settings.Default.StartMenuCustom1);
                        this.GoHome(filt.Filters, filt.Name);
                        return;
                    }
                case "Custom2":
                    {
                        UserFilter filt = new UserFilter(OMLEngine.Properties.Settings.Default.StartMenuCustom1);
                        this.GoHome(filt.Filters, filt.Name);
                        return;
                    }
                case "Custom3":
                    {
                        UserFilter filt = new UserFilter(OMLEngine.Properties.Settings.Default.StartMenuCustom1);
                        this.GoHome(filt.Filters, filt.Name);
                        return;
                    }
                case "Custom4":
                    {
                        UserFilter filt = new UserFilter(OMLEngine.Properties.Settings.Default.StartMenuCustom1);
                        this.GoHome(filt.Filters, filt.Name);
                        return;
                    }
                case "Custom5":
                    {
                        UserFilter filt = new UserFilter(OMLEngine.Properties.Settings.Default.StartMenuCustom1);
                        this.GoHome(filt.Filters, filt.Name);
                        return;
                    }
                case "FirstRun":
                    {
                        Dictionary<string, object> properties = new Dictionary<string, object>();

                        Library.Code.V3.FirstRun page = new Library.Code.V3.FirstRun();
                        properties["Page"] = page;

                        OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_FirstRunBackground", properties);
                        return;
                    }
                case "TV":
                    {
                        this.GoHome(new List<OMLEngine.TitleFilter>(), "TV");
                        return;
                    }
                case "Movies":
                    {
                        this.GoHome(new List<OMLEngine.TitleFilter>(), "Movies");
                        return;
                    }
                default:
                    {
                        ////everything else for now
                        //OMLProperties properties = new OMLProperties();
                        //properties.Add("Application", this);
                        ////properties.Add("UISettings", new UISettings());
                        ////properties.Add("Settings", new Settings());
                        //properties.Add("I18n", I18n.Instance);
                        ////v3 main gallery
                        //Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage(new List<OMLEngine.TitleFilter>(), "OML");
                        ////description
                        //gallery.Description = "OML";
                        //Command CommandContextPopOverlay = new Command();
                        //properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);
                        //properties.Add("Page", gallery);
                        //_session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
                        ////this.mediaChangers = new Library.Code.V3.MediaChangeManager();
                        if (Properties.Settings.Default.CompletedFirstRun == false)
                        {
                            Dictionary<string, object> properties = new Dictionary<string, object>();

                            Library.Code.V3.FirstRun page = new Library.Code.V3.FirstRun();
                            properties["Page"] = page;

                            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_FirstRunBackground", properties);
                        }
                        else
                            this.GoHome(new List<OMLEngine.TitleFilter>(), "OML");
                        return;
                    }
            }
            #endregion v3POC
            //return;
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

        private void GoHome(List<OMLEngine.TitleFilter> filters, string name)
        {
            OMLProperties properties = new OMLProperties();
            properties.Add("Application", this);
            //properties.Add("UISettings", new UISettings());
            //properties.Add("Settings", new Settings());
            properties.Add("I18n", I18n.Instance);
            //v3 main gallery
            Library.Code.V3.GalleryPage gallery = new Library.Code.V3.GalleryPage(filters, name);
            //description
            gallery.Description = name;
            Command CommandContextPopOverlay = new Command();
            properties.Add("CommandContextPopOverlay", CommandContextPopOverlay);
            properties.Add("Page", gallery);
            _session.GoToPage(@"resx://Library/Library.Resources/V3_GalleryPage", properties);
        }

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

        public void CatchMoreInfo()
        {
            if (this._moreInfo == true)
                this._moreInfo = false;
            else
                this._moreInfo = true;
            base.FirePropertyChanged("MoreInfo");
        }

        private bool _moreInfo = false;
        public bool MoreInfo
        {
            get { return this._moreInfo; }
            set { this._moreInfo = value; }
        }

        public void Uninitialize()
        {
            try
            {
                ExtenderDVDPlayer.Uninitialize();
            }
            catch (Exception err)
            {
                DebugLine("Unhandled Exception: {0}", err);
            }
            finally
            {
                // close the db connections
                TitleCollectionManager.CloseDBConnection();                
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
