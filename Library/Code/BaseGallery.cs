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
        TitleCollection titleCollection;

        public BaseGallery(OMLApplication app, AddInHost host, TitleCollection tc) : base(app, host)
        {
            PageName = "Main";
            titleCollection = tc;
            gView = new GroupingView(tc, Filter.Genres);
            FirePropertyChanged("View");
        }

        public OMLChoice View
        {
            get { return gView.myData; }
        }
    }
}
