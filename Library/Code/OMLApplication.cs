using System.Collections;
using System.Collections.Generic;

using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

using OMLEngine;

namespace Library
{
    /// <summary>
    /// Starting point for the OML
    /// </summary>
    public class OMLApplication : ModelItem
    {
        public OMLApplication()
            : this(null, null)
        {
            DebugLine("[OMLApplication] Empty Constructor called");
        }

        public OMLApplication(HistoryOrientedPageSession session, AddInHost host)
        {
            OMLApplication.DebugLine("[OMLApplication] constructor called");
            this._session = session;
            this._isExtender = !host.MediaCenterEnvironment.Capabilities.ContainsKey("Console");
            this._host = host;
            _singleApplicationInstance = this;
            _titles = new TitleCollection();
            _titles.loadTitleCollection();
            _nowPlayingMovieName = "Playing an unknown movie";
        }

        public void Startup(string guid)
        {
            OMLApplication.DebugLine("[OMLApplication] Startup(" + guid + ")");

            switch (guid)
            {
                case "{7533724D-C7CB-4ac2-8AEE-1B0B91ADD393}":
                    OMLApplication.DebugLine("[OMLApplication] going to Menu Page");
                    GoToMenu(new MovieGallery(_titles, Filter.Home));
                    return;
                case "{543d0438-b10d-43d8-a20d-f0c96db4e6bd}":
                    OMLApplication.DebugLine("[OMLApplication] going to Settings Page");
                    GoToSettingsPage(new MovieGallery(_titles, Filter.Settings));
                    return;
                case "{4D5BE22A-27B1-49e3-BF5E-0CC75D32A787}":
                    OMLApplication.DebugLine("[OMLApplication] going to About Page");
                    GoToAboutPage(new MovieGallery(_titles, Filter.About));
                    return;
                default:
                    OMLApplication.DebugLine("[OMLApplication] going to Default (Menu) Page");
                    GoToMenu(new MovieGallery(_titles, Filter.Home));
                    return;
            }
        }

        public static OMLApplication Current
        {
            get { return _singleApplicationInstance; }
        }

        public MediaCenterEnvironment MediaCenterEnvironment
        {
            get
            {
                if (_host == null) return null;
                return _host.MediaCenterEnvironment;
            }
        }

        public void GoToSetup( MovieGallery gallery )
        {
            DebugLine("[OMLApplication] GoToSetup()");
            Dictionary<string, object> properties = CreateProperties(true, false);
            properties["MovieBrowser"] = gallery;

            OMLApplication.DebugLine("OMLApplication.GoToSetup");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Setup", properties);
            }
        }

        public void GoToSettingsPage(MovieGallery gallery)
        {
            DebugLine("[OMLApplication] GoToSettingsPage()");
            Dictionary<string, object> properties = CreateProperties(true, true);
            properties["MovieBrowser"] = gallery;

            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Settings", properties);
            }
        }

        public void GoToAboutPage(MovieGallery gallery)
        {
            DebugLine("[OMLApplication] GotoAboutPage()");
            Dictionary<string, object> properties = CreateProperties(true, false);
            properties["MovieBrowser"] = gallery;

            if (_session != null)
                _session.GoToPage("resx://Library/Library.Resources/About", properties);
        }

        public void GoToMenu(MovieGallery gallery)
        {
            DebugLine("[OMLApplication] GoToMenu(Gallery, #{0} Movies)", gallery.Movies.Count);
            Dictionary<string, object> properties = CreateProperties(true, false);
            properties["MovieBrowser"] = gallery;
            properties["GalleryView"] = Properties.Settings.Default.MovieView;

            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Menu", properties);
            }
        }

        public void GoToSelectionList(MovieGallery gallery, IList list, string listName, string galleryView)
        {
            DebugLine("[OMLApplication] GoToSelectionList(#{0} items, list name: {1}, gallery: {2})", list.Count, listName, galleryView);
            Dictionary<string, object> properties = CreateProperties(true, false);
            properties["MovieBrowser"] = gallery;
            properties["List"] = list;
            properties["ListName"] = listName;
            properties["GalleryView"] = galleryView;

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
            Dictionary<string, object> properties = CreateProperties(true, false);
            properties["DetailsPage"] = page;

            // If we have no page session, just spit out a trace statement.
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/DetailsPage", properties);
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

        public TitleCollection ReloadTitleCollection()
        {
            DebugLine("[OMLApplication] ReloadTitleCollection()");
            _titles.loadTitleCollection();
            return _titles;
        }


        private Dictionary<string, object> CreateProperties(bool uiSettings, bool settings)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            if (uiSettings)
                properties["UISettings"] = new UISettings();
            if (settings)
                properties["Settings"] = new Settings();
            return properties;
        }


        // private data
        private string _nowPlayingMovieName;
        private PlayState _nowPlayingStatus;

        private static OMLApplication _singleApplicationInstance;
        private AddInHost _host;
        private HistoryOrientedPageSession _session;
        private TitleCollection _titles;

        private bool _isExtender;

    }
}