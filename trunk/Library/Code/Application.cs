using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;

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
            this._session = session;
            this._isExtender = host.MediaCenterEnvironment.Capabilities.ContainsKey("Console");
            this._host = host;
            _singleApplicationInstance = this;
            _movieGallery = new MovieGallery();
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

        public void GoToMenu()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["MovieBrowser"] = _movieGallery;

            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/Default", properties);
            }
        }

        public void GoToDetails(DetailsPage page)
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

            // If we have no page session, just spit out a trace statement.
            if (_session != null)
            {
                _session.GoToPage("resx://Library/Library.Resources/DetailsPage", properties);
            }
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
        private static MovieGallery _movieGallery;
        private bool _isExtender;

    }
}