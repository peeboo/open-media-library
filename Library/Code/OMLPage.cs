using System;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class OMLPage : ModelItem
    {
        OMLChoice menuTabs;
        PropertySet pageProperties;
        OMLApplication app;
        AddInHost addin;
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

        public OMLPage(OMLApplication application, AddInHost addinHost)
        {
            app = application;
            addin = addinHost;

            menuTabs = new OMLChoice();
            ArrayListDataSet tabs = new ArrayListDataSet();
            tabs.Add(Filter.Home);
            tabs.Add(Filter.Genres);
            tabs.Add(Filter.ParentRating);
            tabs.Add(Filter.Runtime);
            tabs.Add(Filter.UserRating);
            tabs.Add(Filter.Year);
            menuTabs.Options = tabs;

            pageProperties = new PropertySet(this);
        }
    }
}
