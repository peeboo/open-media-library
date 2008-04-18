using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace OMLEngine
{
    public class MovieMetadata
    {
        public string Title;
        public string Summary;
        public string Genre;
        //public string CountryFullName;
        //public string CountryShortName;
        public string Rating;
        public string ImagePath;
        public string Length;
        public string ReleaseDate;
        public IList Actors;
        public IList Directors;
        public IList Producers;
        public IList Writers;
        public string ImdbRating;
        public string Url;

        public int Id;
    }
}
