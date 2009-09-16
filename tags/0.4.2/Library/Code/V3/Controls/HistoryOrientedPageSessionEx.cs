using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.Hosting;

namespace Library.Code.V3
{
    public class HistoryOrientedPageSessionEx : HistoryOrientedPageSession
    {
        // Methods
        public void GoToPageWithoutHistory(string source, IDictionary<string, object> uiProperties)
        {
            base.LoadPage(source, null, uiProperties);
        }
    }
}
