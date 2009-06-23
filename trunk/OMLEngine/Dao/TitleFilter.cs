using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine
{
    public enum TitleFilterType
    {
        All,
        Person,
        Genre,
        VideoFormat,
        Unwatched,
        Tag,
        Alpha,
        ParentalRating,
        Runtime,
        Year,
        Country,
        Director,
        Actor,
        UserRating,        
        PercentComplete,
        DateAdded,
        Name,
        Parent,
        TitleType
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

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            TitleFilter compareTo = (TitleFilter)obj;

            return (this.filterType == compareTo.filterType 
                        && this.filterText == compareTo.filterText);
        }

        public override int GetHashCode()
        {
            if (filterText != null)
                return filterType.GetHashCode() ^ filterText.GetHashCode();
            else
                return filterType.GetHashCode();
        }
    }

    public class TitleTypeFilter : TitleFilter
    {
        public TitleTypeFilter(TitleTypes titleTypes)
            : base(TitleFilterType.TitleType, ((int)titleTypes).ToString())
        {
        }
    }

    public class ParentFilter : TitleFilter
    {
        public ParentFilter(int parentId)
            : base(TitleFilterType.Parent, parentId.ToString())
        {
        }
    }
}
