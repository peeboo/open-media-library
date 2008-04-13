using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using System.IO;

namespace Library
{
    public class Application : ModelItem
    {
        private static Application singleApplicationInstance;
        private AddInHost host;
        private HistoryOrientedPageSession session;
        private static Movie movie;

        public Application()
            : this(null, null)
        {
        }

        public Application(HistoryOrientedPageSession session, AddInHost host)
        {
            this.session = session;
            this.host = host;
            singleApplicationInstance = this;
            movie = new Movie();
        }

        /// <summary>
        /// The one and only instance of Application running in this Z process
        /// </summary>
        public static Application Current
        {
            get
            {
                return singleApplicationInstance;
            }
        }

        public MediaCenterEnvironment MediaCenterEnvironment
        {
            get
            {
                if (host == null) return null;
                return host.MediaCenterEnvironment;
            }
        }

        public void GoToMenu()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["Movie"] = movie;

            if (session != null)
            {
                session.GoToPage("resx://Library/Library.Resources/Menu", properties);
            }
            else
            {
                Debug.WriteLine("GoToMenu");
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
            if (session != null)
            {
                session.GoToPage("resx://Library/Library.Resources/DetailsPage", properties);
            }
        }
    }
}