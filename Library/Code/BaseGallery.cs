using System;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class BaseGallery : ModelItem
    {
        OMLChoice menuTabs;

        public OMLChoice MenuTabs
        {
            get { return menuTabs; }
            set
            {
                menuTabs = value;
                FirePropertyChanged("MenuTabs");
            }
        }

        public BaseGallery()
        {
            menuTabs = new OMLChoice();
            ArrayListDataSet tabs = new ArrayListDataSet();
            tabs.Add("By Genre");
            tabs.Add("By Year");
            menuTabs.Options = tabs;
        }
    }
}
