using System;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;
using OMLEngine;

namespace Library
{
    public class GroupingView : ModelItem
    {
        const int GROUPS = 20;
        const int ITEMSINGROUP = 50;

        private List<OMLChoice> groupList;
        private VirtualList movieItems;

        public GroupingView(TitleCollection tc, string filterString)
        {
            groupList = new List<OMLChoice>();

            for (int i = 0; i <= GROUPS; i++)
            {
                OMLChoice ch = new OMLChoice();
                ch.Description = string.Format("Choice {0}", i);
                List<GalleryItem> items = new List<GalleryItem>();
                foreach (Title title in tc)
                {
                    items.Add(new MovieItem(title, null));
                }
                ch.Options = items;
                groupList.Add(ch);
            }
        }

        public OMLChoice Data
        {
            get
            {
                OMLChoice chReturn = new OMLChoice();
                chReturn.Options = groupList;
                return chReturn;
            }
        }
    }
}
