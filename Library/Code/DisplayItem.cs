using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.Collections;
using OMLEngine;
using System.Diagnostics;

namespace Library
{
    public class DisplayItem : Command
    {
        private Title _titleObj;
        private Image _front_boxart;
        private Image _back_boxart;

        public DisplayItem() { }
        public DisplayItem(Title title)
        {
            _titleObj = title;
            _front_boxart = Movie.LoadImage(_titleObj.front_boxart_path);
            _back_boxart = Movie.LoadImage(_titleObj.back_boxart_path);
        }

        public string GetMedia
        {
            get { return _titleObj.FileLocation; }
        }
        
        public Image GetImage
        {
            get { return _front_boxart; }
        }
        public int itemId
        {
            get { return _titleObj.itemId; }
        }
        public string GetTitle
        {
            get { return _titleObj.Name; }
        }
        public string GetRuntime
        {
            get { return _titleObj.Runtime.ToString(); }
        }
        public string GetMpaaRating
        {
            get { return (string)Enum.GetName(typeof(Rating), _titleObj.MPAARating); }
        }
        public string GetSummary
        {
            get { return _titleObj.Synopsis; }
        }
        public string GetDistributor
        {
            get { return _titleObj.Distributor; }
        }
        public string GetCountryOfOrigin
        {
            get { return _titleObj.Country_Of_Origin; }
        }
        public string GetOfficialMovieWebsite
        {
            get { return _titleObj.Official_Website_Url; }
        }
        public string GetReleaseDate
        {
            get { return _titleObj.ReleaseDate.ToShortDateString(); }
        }
        public IList GetActors
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
        }
        public IList GetCrew
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
        }
        public IList GetDirectors
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
        }
        public IList GetProducers
        {
            get { return _titleObj.Producers; }
        }
        public IList GetWriters
        {
            get { return _titleObj.Writers; }
        }
        public string DynamicPlayMedia()
        {
            string path_to_media = _titleObj.FileLocation;
            if (Application.Current.IsExtender && _titleObj.NeedToTranscodeToExtenders())
            {
                Trace.WriteLine("We are an extender");
                string new_path = string.Empty;
                if (_titleObj.PlayTranscodedMedia(ref new_path))
                    path_to_media = new_path;
            }
            Trace.WriteLine("Returning path: " + path_to_media);
            return path_to_media;
        }
    }
}
