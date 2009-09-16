using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Settings
{
    public enum Context
    {
        Home,
        Trailers,
        Movies,
        TV,
        Search,
        Settings,
        Custom1,
        Custom2,
        Custom3,
        Custom4,
        Custom5
    }

    public class StartMenuItem
    {
        public string ItemId { get; set; }
        public string ExtendedContext { get; set; }
        public string Title { get; set; }
        public double TimeStamp { get; set; }
        public string ImageUrl { get; set; }
        public string InactiveImageUrl { get; set; }
        public string Description { get; set; }
        public Context Context { get; set; }
        public string AppId { get; set; }//currently {3f0850a7-0fd7-4cbf-b8dc-c7f7ea31534e}
        public string AddIn { get; set; }
        public string CapabilitiesRequired { get; set; }
        public double SortTimeStamp { get; set; }
        public int SortOrder { get; set; }
    }
}
