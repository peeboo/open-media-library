using System;
using OMLEngine;

namespace Library
{
    class MoviePlayerWPL : IPlayMovie
    {
        WindowsPlayListManager wplm;

        public MoviePlayerWPL(MovieItem title)
        {
        }

        public bool PlayMovie()
        {
            foreach (PlayListItem item in wplm.PlayListItems)
            {

            }
            // go through the WindowsPlayListManager and create appropriate players for each (calling PlayMovie() on each)
            return true;
        }
    }
}
