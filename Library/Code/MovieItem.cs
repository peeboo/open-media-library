using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.Collections;
using OMLEngine;
using System.Diagnostics;

namespace Library
{
    public class MovieItem : Command
    {
        private WindowsPlayListManager _wplm;
        private Title _titleObj;
        private Image _frontCoverArtImage;
        private Image _backCoverArtImage;

        public MovieItem(Title title)
        {
            _titleObj = title;
            _frontCoverArtImage = MovieGallery.LoadImage(_titleObj.FrontCoverPath);
            _backCoverArtImage = MovieGallery.LoadImage(_titleObj.BackCoverPath);
        }

        public MovieItem(WindowsPlayListManager wplm)
        {
            _wplm = wplm;
        }

        public void PlayMovie()
        {
            IPlayMovie moviePlayer = MoviePlayerFactory.CreateMoviePlayer(this);
            moviePlayer.PlayMovie();
        }

        public Title TitleObject
        {
            get { return _titleObj; }
        }

        public string FileLocation
        {
            get { return _titleObj.FileLocation; }
            set { _titleObj.FileLocation = value; }
        }

        public Image FrontCover
        {
            get { return _frontCoverArtImage; }
            set { _frontCoverArtImage = value; }
        }

        public Image BackCover
        {
            get { return _backCoverArtImage; }
            set { _backCoverArtImage = value; }
        }

        public int itemId
        {
            get { return _titleObj.InternalItemID; }
        }

        public int UseStarRating
        {
            get { return _titleObj.UserStarRating; }
        }

        public string Name
        {
            get { return _titleObj.Name; }
            set { _titleObj.Name = value; }
        }
        public string Runtime
        {
            get { return _titleObj.Runtime.ToString(); }
            set { _titleObj.Runtime = Convert.ToInt32(value); }
        }
        public string Rating
        {
            get { return _titleObj.MPAARating; }
            set { }
        }
        public string Synopsis
        {
            get { return _titleObj.Synopsis; }
            set { _titleObj.Synopsis = value; }
        }
        public string Distributor
        {
            get { return _titleObj.Distributor; }
            set { _titleObj.Distributor = value; }
        }
        public string CountryOfOrigin
        {
            get { return _titleObj.CountryOfOrigin; }
            set { _titleObj.CountryOfOrigin = value; }
        }
        public string OfficialWebsite
        {
            get { return _titleObj.OfficialWebsiteURL; }
            set { _titleObj.OfficialWebsiteURL = value; }
        }
        public string ReleaseDate
        {
            get { return _titleObj.ReleaseDate.ToShortDateString(); }
            //TODO
            set { }
        }
        public IList Actors
        {
            get
            {
                List<string> actor_names = new List<string>();
                foreach (Person p in _titleObj.Actors)
                {
                    actor_names.Add(p.full_name);
                }
                return actor_names;
            }
            //set { _titleObj.Actors = value; }
        }
        public IList Crew
        {
            get
            {
                List<string> crew_names = new List<string>();
                foreach (Person p in _titleObj.Crew)
                {
                    crew_names.Add(p.full_name);
                }
                return crew_names;
            }
            //set { _titleObj.Crew = value; }
        }
        public IList Directors
        {
            get
            {
                List<string> director_names = new List<string>();
                foreach (Person p in _titleObj.Directors)
                {
                    director_names.Add(p.full_name);
                }
                return director_names;
            }
            //set { _titleObj.Directors = value; }
        }
        public IList Producers
        {
            get { return _titleObj.Producers; }
            //set { _titleObj.Producers = value; }
        }
        public IList Writers
        {
            get { return _titleObj.Writers; }
            //set { _titleObj.Writers = value; }
        }
    }
}
