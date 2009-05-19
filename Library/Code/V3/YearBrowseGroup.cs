using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.MediaCenter.UI;
using OMLEngine;

namespace Library.Code.V3
{
    public class YearBrowseGroup: BrowseGroup
    {
        // Fields
        private IList m_listContent;
        private List<Title> titles;        
        
        public YearBrowseGroup(List<Title> titles)
            :base()
        {
            this.titles = titles;
            this.m_listContent = new VirtualList(new ItemCountHandler(this.InitializeListCount));
            ((VirtualList)this.m_listContent).RequestItemHandler = new RequestItemHandler(this.GetItem);
        }

        private void InitializeListCount(VirtualList vlist)
        {            
            //temp hack for perf issue: see http://discuss.mediacentersandbox.com/forums/thread/9205.aspx
            if (titles.Count > 20)
                ((VirtualList)this.m_listContent).Count = 20;
            else
                ((VirtualList)this.m_listContent).Count = titles.Count;
        }

        private void GetItem(VirtualList vlist, int idx, ItemRequestCallback callbackItem)
        {
            object commandForItem = null;
            if (((VirtualList)this.m_listContent).IsItemAvailable(idx))
            {
                commandForItem = this.m_listContent[idx];
            }
            else if (((this.titles != null) && (idx >= 0)) && (idx < this.titles.Count))
            {
                OMLEngine.Title item = this.titles[idx] as OMLEngine.Title;
                if (item != null)
                {
                    commandForItem = this.GetCommandForItem(item);
                }
            }
            callbackItem(vlist, idx, commandForItem);
        }

        private object GetCommandForItem(OMLEngine.Title item)
        {
            this.pulleditemCount++;
            Console.WriteLine(this.pulleditemCount.ToString());
            return new Library.Code.V3.MovieItem(item, this);
        }

        int pulleditemCount = 0;

        public override IList Content
        {
            get
            {
                return this.m_listContent;
            }
            set
            {
                if (this.m_listContent != value)
                {
                    this.m_listContent = value;
                    base.FirePropertyChanged("Content");
                }
            }
        }

    }

 

}
