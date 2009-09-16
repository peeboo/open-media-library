using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;
using OMLEngine;
using System.Collections;
using System;
using System.Data;

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

        public void Startup()
        {
            DebugLine("[OMLApplication] Startup()");
            //if ( _titles.Count > 0 )
                GoToMenu(new MovieGallery(_titles, Filter.Home));
            //else
            //    GoToSetup(null);
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
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["MovieBrowser"] = gallery;
            properties["UISettings"] = new UISettings();

            OMLApplication.DebugLine("OMLApplication.GoToSetup");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Setup", properties);
            }
        }

        public void GoToSettingsPage(MovieGallery gallery)
        {
            DebugLine("[OMLApplication] GoToSettingsPage()");
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["MovieBrowser"] = gallery;
            properties["UISettings"] = new UISettings();
            properties["Settings"] = new Settings();

            OMLApplication.DebugLine("OMLApplication.GoToSettingsPage");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Settings", properties);
            }
        }    

        public void GoToMenu(MovieGallery gallery)
        {
            DebugLine("[OMLApplication] GoToMenu()");
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["MovieBrowser"] = gallery;
            properties["GalleryView"] = Properties.Settings.Default.MovieView;
            properties["UISettings"] = new UISettings();

            OMLApplication.DebugLine("OMLApplication.GoToMenu");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Menu", properties);
            }
        }

        public void GoToSelectionList(MovieGallery gallery, IList list, string listName, string galleryView)
        {
            DebugLine("[OMLApplication] GoToSelectionList()");
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["MovieBrowser"] = gallery;
            properties["List"] = list;
            properties["ListName"] = listName;
            properties["GalleryView"] = galleryView;
            properties["UISettings"] = new UISettings();

            OMLApplication.DebugLine("OMLApplication.GoToMenu");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/SelectionList", properties);
            }
        }


        public void GoToDetails(MovieDetailsPage page)
        {
            DebugLine("[OMLApplication] GoToDetails()");
            if (page == null)
                throw new System.Exception("The method or operation is not implemented.");

            //
            // Construct the arguments dictionary and then navigate to the
            // details page template.
            //
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["DetailsPage"] = page;
            properties["Application"] = this;
            properties["UISettings"] = new UISettings();

            OMLApplication.DebugLine("OMLApplication.GoToDetails");
            // If we have no page session, just spit out a trace statement.
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/DetailsPage", properties);
            }
        }

        public static void DebugLine(string msg, params object[] paramArray)
        {
            Trace.TraceInformation(DateTime.Now.ToString() +" "+ msg, paramArray);
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


        // private data
        private  string _nowPlayingMovieName;
        private  PlayState _nowPlayingStatus;

        private static OMLApplication _singleApplicationInstance;
        private AddInHost _host;
        private HistoryOrientedPageSession _session;
        private TitleCollection _titles;

        private bool _isExtender;

    }
}