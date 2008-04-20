using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Collections;
using OMLEngine;

namespace Library
{
    public class DisplayItem : Command
    {
        private Title _titleObj;

        public DisplayItem() { }
        public DisplayItem(Title title)
        {
            _titleObj = title;
        }
        
        public Image GetImage
        {
            get { return Movie.LoadImage(_titleObj.front_boxart_path); }
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
            get { return _titleObj.MPAARating; }
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
    }
}
