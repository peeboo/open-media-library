using System;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class BaseGallery : ModelItem
    {
        Choice menuTabs;
        IList<Filter> tabFilters;

        public BaseGallery()
        {
            tabFilters = new List<Filter>();
        }
    }
}
