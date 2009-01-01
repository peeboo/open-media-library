using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine
{
    public enum TitleFilterType
    {
        Person,
        Genre,
        VideoFormat,
        Watched,
        Tag
    }

    public class TitleFilter
    {
        private TitleFilterType filterType;
        private string filterText;

        public TitleFilterType FilterType { get { return filterType; } }
        public string FilterText { get { return filterText; } }

        public TitleFilter(TitleFilterType filterType, string filterText)
        {
            this.filterType = filterType;
            this.filterText = filterText;
        }
    }
}
