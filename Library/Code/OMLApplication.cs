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
        }

        public OMLApplication(HistoryOrientedPageSession session, AddInHost host)
        {
            OMLApplication.DebugLine("OMLApplication.OMLApplication");
            this._session = session;
            this._isExtender = !host.MediaCenterEnvironment.Capabilities.ContainsKey("Console");
            this._host = host;
            _singleApplicationInstance = this;
            _titles = new TitleCollection();
            _titles.loadTitleCollection();
        }

        public void Startup()
        {
            if ( _titles.Count > 0 )
                GoToMenu( new MovieGallery(_titles, Category.Home ) );
            else
                GoToSetup(null);
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
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["MovieBrowser"] = gallery;

            OMLApplication.DebugLine("OMLApplication.GoToSetup");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Setup", properties);
            }
        }

    
        public void GoToMenu(MovieGallery gallery)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["MovieBrowser"] = gallery;
            properties["GalleryView"] = Properties.Settings.Default.MovieView;

            OMLApplication.DebugLine("OMLApplication.GoToMenu");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Menu", properties);
            }
        }

        public void GoToSelectionList(MovieGallery gallery, IList list, string listName, string galleryView)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["MovieBrowser"] = gallery;
            properties["List"] = list;
            properties["ListName"] = listName;
            properties["GalleryView"] = galleryView;

            OMLApplication.DebugLine("OMLApplication.GoToMenu");
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/SelectionList", properties);
            }
        }


        public void GoToDetails(MovieDetailsPage page)
        {
            if (page == null)
                throw new System.Exception("The method or operation is not implemented.");

            //
            // Construct the arguments dictionary and then navigate to the
            // details page template.
            //
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["DetailsPage"] = page;
            properties["Application"] = this;

            OMLApplication.DebugLine("OMLApplication.GoToDetails");
            // If we have no page session, just spit out a trace statement.
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/DetailsPage", properties);
            }
        }

        public static void DebugLine(string msg, params object[] paramArray)
        {
            Trace.TraceInformation(msg, paramArray);
            Trace.Flush();
        }

        // properties
        public bool IsExtender
        {
            get { return _isExtender; }
        }

        // private data
        private static OMLApplication _singleApplicationInstance;
        private AddInHost _host;
        private HistoryOrientedPageSession _session;
        private TitleCollection _titles;

        private bool _isExtender;

    }
}