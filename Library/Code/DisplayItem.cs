using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.Collections;
using OMLEngine;

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
            get { return _titleObj.Runtime; }
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
            get { return _titleObj.Actors; }
        }
        public IList GetCrew
        {
            get { return _titleObj.Crew; }
        }
        public IList GetDirectors
        {
            get { return _titleObj.Directors; }
        }
        public IList GetProducers
        {
            get { return _titleObj.Producers; }
        }
        public IList GetWriters
        {
            get { return _titleObj.Writers; }
        }
        public void DynamicPlayMedia()
        {
            string path_to_media = _titleObj.FileLocation;
            if (Application.Current.IsExtender && _titleObj.TranscodeToExtender)
            {
                string new_path = string.Empty;
                if (_titleObj.PlayTranscodedMedia(ref new_path))
                    path_to_media = new_path;
            }
            Application.Current.MediaCenterEnvironment.PlayMedia(MediaType.Video,
                                                                 _titleObj.FileLocation,
                                                                 false);
            Application.Current.MediaCenterEnvironment.MediaExperience.GoToFullScreen();
        }
    }
}
