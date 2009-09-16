using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;
using System.Threading;
using System.Linq;

namespace Library
{
    public class LabeledList : ModelItem
    {
        private VirtualList moviesVirtualList;
        private string filterLabel;

        public string FilterLabel { get { return filterLabel; } }

        public VirtualList Movies
        {
            get { return moviesVirtualList; }
        }

        public LabeledList(string filterLabel, VirtualList filteredMovies)
        {
            this.filterLabel = filterLabel;
            this.moviesVirtualList = filteredMovies;
        }        
    }
}
