using System;
using System.Collections.Generic;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using OMLEngine;

namespace Library
{
    public class BaseGallery : OMLPage
    {
        GroupingView gView;        

        public BaseGallery(OMLProperties props) : base(props)
        {
            PageName = "Main";           
            gView = new GroupingView(Filter.Genres);
            FirePropertyChanged("View");
        }

        public OMLChoice View
        {
            get { return gView.myData; }
        }
    }
}
