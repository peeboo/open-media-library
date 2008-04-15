using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using System.IO;
using System.Diagnostics;

namespace Library
{
    public class Application : ModelItem
    {
        private static Application singleApplicationInstance;
        private AddInHost host;
        private HistoryOrientedPageSession session;
        private static Movie movie;
        public bool IsExtender;

        public Application()
            : this(null, null)
        {
        }

        public Application(HistoryOrientedPageSession session, AddInHost host)
        {
            Debug.WriteLine("Application:Application()");
            this.session = session;
            IsExtender = host.MediaCenterEnvironment.Capabilities.ContainsKey("Console");
            this.host = host;
            singleApplicationInstance = this;
            movie = new Movie();
        }

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
            Trace.WriteLine("Application:GoToMenu()");
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties["Application"] = this;
            properties["Movie"] = movie;

            if (session != null)
            {
                session.GoToPage("resx://Library/Library.Resources/Menu", properties);
            }
        }

        public void GoToDetails(DetailsPage page)
        {
            Trace.WriteLine("Application:GoToDetails()");
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