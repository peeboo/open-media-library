//#define DEBUG_EXT
//#define LAYOUT_V2
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
    public class OMLApplication : ModelItem
    {
        private bool isBusy = false;
        private bool isStartingTranscodingJob = false;
        private string transcodeStatus = string.Empty;
        private int currentFocusedItemIndex = 0;
        private int currentItemIndexPosition = 0;
        private int currentAngleDegrees;

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
                FirePropertyChanged("IsStartingTranscodingJob");
            }
        }       

        public OMLApplication()
            : this(null, null)
        {
            DebugLine("[OMLApplication] Empty Constructor called");
        }

        public OMLApplication(HistoryOrientedPageSession session, AddInHost host)
        {           
            this._session = session;
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
            string uiCulture = Properties.Settings.Default.UILanguage;
            if (!string.IsNullOrEmpty(uiCulture)) I18n.SetCulture(new CultureInfo(uiCulture));
            _nowPlayingMovieName = "Playing an unknown movie";
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
            //OMLUpdater updater = new OMLUpdater();
            //ThreadPool.QueueUserWorkItem(new WaitCallback(updater.checkUpdate));

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
                    if (Properties.Settings.Default.StartPage == Filter.Home)
                    {
                        OMLApplication.DebugLine("[OMLApplication] going to Menu Page");
                        GoToMenu(new MovieGallery());
                    }
                    else
                    {                                                
                        // see if they've selected a subfilter
                        // the unwatched is a special case until we add a user setting to determine the subfilter
                        if (!string.IsNullOrEmpty(Properties.Settings.Default.StartPageSubFilter)
                            || Properties.Settings.Default.StartPage == Filter.Unwatched)
                        {
                            // go to the subfilter
                            GoToMenu(
                                new MovieGallery(
                                    new TitleFilter(Filter.FilterStringToTitleType(Properties.Settings.Default.StartPage),
                                        Properties.Settings.Default.StartPageSubFilter)));
                        }
                        else
                        {
                            // go to the selection list
                            GoToSelectionList(new Filter(new MovieGallery(), Filter.FilterStringToTitleType(Properties.Settings.Default.StartPage), null));               
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

            if (Properties.Settings.Default.MovieView == GalleryView.CoverArtWithAlpha &&
                gallery.Movies.Count < 30)
            {
                // alpha falls back to cover art view if there's not enough items
                properties["GalleryView"] = GalleryView.CoverArt;
            }
            else
            {
                properties["GalleryView"] = Properties.Settings.Default.MovieView;
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
                _session.GoToPage("resx://Library/Library.Resources/DetailsPage3", properties);
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

        // private data
        private string _nowPlayingMovieName;
        private PlayState _nowPlayingStatus;

        private static OMLApplication _singleApplicationInstance;
        private AddInHost _host;
        private HistoryOrientedPageSession _session;

        private bool _isExtender;
    }
}
