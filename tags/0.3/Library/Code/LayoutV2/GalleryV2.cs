using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using OMLEngine;

namespace Library
{
    public class GalleryV2 : OMLPage
    {
        private TitleCollection collection;
        private Dictionary<string, Filter> _filters = new Dictionary<string,Filter>();
        private VirtualList allMoviesVL;
        private VirtualList moviesByGenreVL;
        private Choice groupView;
        private List<Choice> groupList;
        const int GROUPS = 1;


        public Choice GroupViewModel
        {
            get
            {
                return groupView;
            }
            set
            {
                groupView = value;
                FirePropertyChanged("GroupViewModel");
            }
        }

        public VirtualList AllMovies
        {
            get { return allMoviesVL; }
            set
            {
                allMoviesVL = value;
                FirePropertyChanged("AllMovies");
            }
        }

        public VirtualList MoviesByGenre
        {
            get { return moviesByGenreVL; }
            set
            {
                moviesByGenreVL = value;
                FirePropertyChanged("MoviesByGenre");
            }
        }

        public GalleryV2(OMLProperties props, TitleCollection tc)
            : base(props)
        {
            collection = tc;
            buildVirtualLists();
            groupView = new Choice();

            groupList = new List<Choice>();
            for (int i = 0; i <= GROUPS; i++)
            {
                Choice ch = new Choice();
                ch.Description = string.Format("Choice {0}", i);
                ch.Options = allMoviesVL;
                groupList.Add(ch);
            }

            groupView.Options = groupList;
        }

        private void buildVirtualLists()
        {
            if (collection.Count == 0)
                return;

            allMoviesVL = new VirtualList(this, null);
            foreach (Title t in collection)
            {
                allMoviesVL.Add(new MovieItemV2(t));
            }
        }
    }
}
