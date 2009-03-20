using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Library.Code.V3
{
    public class ContextMenuData : Microsoft.MediaCenter.UI.ModelItem
    {
        private IList m_listContextualItems;
        private IList m_listGlobalItems;

        public ContextMenuData()
        {
            m_listContextualItems = new List<Microsoft.MediaCenter.UI.ICommand>();
            m_listGlobalItems = new List<Microsoft.MediaCenter.UI.ICommand>();
        }
        public IList SharedItems
        {
            get
            {
                return this.m_listGlobalItems;
            }
        }

        public IList UniqueItems
        {
            get
            {
                return this.m_listContextualItems;
            }
        }

    }
}
