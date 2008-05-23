using System;
using OMLEngine;

namespace Library
{
    class MoviePlayerWPL : IPlayMovie
    {
        MovieItem title;

        public MoviePlayerWPL(MovieItem mItem)
        {
            title = mItem;
        }

        public bool PlayMovie()
        {
            foreach (PlayListItem item in title.PlayList.PlayListItems)
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
