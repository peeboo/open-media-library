using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Settings
{
    public class UserFilter
    {
        List<TitleFilter> filters = null;

        public List<TitleFilter> Filters
        { get { return this.filters; } }

        private string customName;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(customName))
                {
                    for(int x = filters.Count -1 ; x >= 0; x--)
                    {
                        if (! string.IsNullOrEmpty(filters[x].FilterText))
                        {
                            customName = filters[x].FilterText;
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(customName))
                {
                    for (int x = filters.Count - 1; x >= 0; x--)
                    {
                        if (!string.IsNullOrEmpty(filters[x].FilterText))
                        {
                            customName = filters[x].FilterType.ToString();
                            break;
                        }
                    }
                }

                return customName;
            }
        }

        public UserFilter(string serializedFilter)
        {
            filters = new List<TitleFilter>();

            string[] parts = serializedFilter.Split('|');

            if (parts == null || parts.Length < 3)
                return;

            this.customName = parts[0];

            for (int x = 1; x < parts.Length; x++)
            {                
                string filterTypeString = parts[x];
                string filterQuery = parts[++x];                

                if (! Enum.IsDefined(typeof(TitleFilterType), filterTypeString))
                    continue;

                filters.Add(new TitleFilter((TitleFilterType)Enum.Parse(typeof(TitleFilterType), filterTypeString), filterQuery));         
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The custom name of the filter to appear in the menu</param>
        /// <param name="filters">All the filters that should be run</param>
        public UserFilter(string name, IList<TitleFilter> filters)
        {
            customName = name;
            this.filters = new List<TitleFilter>(filters);
        }

        public IEnumerable<Title> GetFilteredTitles()
        {
            return TitleCollectionManager.GetFilteredTitles(filters);
        }

        public override string ToString()
        {
            if (filters == null || filters.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            sb.Append(Name);

            foreach (TitleFilter filter in filters)
            {
                sb.Append('|')
                    .Append(filter.FilterType.ToString())
                    .Append('|')
                    .Append(filter.FilterText);
            }

            return sb.ToString();
        }
    }
}
