using System;
using OMLEngine;

namespace Valkyrie
{
    class MoviePlayerWPL : IPlayMovie
    {
        WindowsPlayListManager wplm;

        public bool IsExtender()
        {
            return false;
        }

        public MoviePlayerWPL(MovieItem title)
        {
        }

        public bool PlayMovie()
        {
            foreach (PlayListItem item in wplm.PlayListItems)
            {
                MovieItem mItem = new MovieItem(item.title);

                try
                {
                    IPlayMovie movie = MoviePlayerFactory.CreateMoviePlayer(mItem);
                    movie.PlayMovie();
                }
                catch (Exception e)
                {
                }
            }

            return true;
        }
    }
}
