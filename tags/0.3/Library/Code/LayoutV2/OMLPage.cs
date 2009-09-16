using System;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class OMLPage : ModelItem
    {
        OMLChoice menuTabs;
        OMLProperties pageProperties;
        string pageName;

        public string PageName
        {
            get { return pageName; }
            set
            {
                pageName = value;
                FirePropertyChanged("PageName");
            }
        }

        public OMLChoice MenuTabs
        {
            get { return menuTabs; }
            set
            {
                menuTabs = value;
                FirePropertyChanged("MenuTabs");
            }
        }

        public OMLPage(OMLProperties props)
        {
            pageProperties = props;

            menuTabs = new OMLChoice();
            ArrayListDataSet tabs = new ArrayListDataSet();
            tabs.Add("All Movies");
            tabs.Add(Filter.Genres);
            tabs.Add(Filter.ParentRating);
            tabs.Add(Filter.Runtime);
            tabs.Add(Filter.UserRating);
            tabs.Add(Filter.Year);
            menuTabs.Options = tabs;
        }
    }
}
