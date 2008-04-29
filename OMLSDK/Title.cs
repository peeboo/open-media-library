using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OMLSDK
{
    public enum Genre { Comedy, Drama, Action, Romance };

    [Serializable()]
    public class Title : ISerializable
    {
        #region locals
        private string _file_location;
        private string _name;
        private string _description;
        private int _itemId;
        private string _front_boxart_path;
        private string _back_boxart_path;
        private string _runtime;
        private string _mpaa_rating;
        private string _synopsis;
        private string _distributor;
        private string _country_of_origin;
        private string _official_website_url;
        private DateTime _release_date;
        private DateTime _date_added;
        private string _importer_source;
        private List<string> _actors;
        private List<string> _crew;
        private List<string> _directors;
        private List<string> _writers;
        private List<string> _producers;
        private List<string> _sound_formats;
        private List<string> _language_formats;
        private List<Genre> _genres;
        #endregion

        #region properties
        public string FileLocation
        {
            get { return _file_location; }
            set { _file_location = value; }
        }
        public string Name
        {
            get { return _name; }
            set {
                if (value != null)
                    _name = value;
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if (value != null)
                    _description = value;
            }
        }
        public int itemId
        {
            get { return _itemId; }
            set
            {
                if (value >= 0)
                    _itemId = value;
            }
        }
        public string front_boxart_path
        {
            get { return _front_boxart_path; }
            set { _front_boxart_path = value; }
        }
        public string back_boxart_path
        {
            get { return _back_boxart_path; }
            set { _back_boxart_path = value; }
        }
        public string Runtime
        {
            get { return _runtime; }
            set
            {
                if (value != null)
                    _runtime = value;
            }
        }
        public string MPAARating
        {
            get { return _mpaa_rating; }
            set
            {
                if (value != null)
                    _mpaa_rating = value;
            }
        }
        public string Synopsis
        {
            get { return _synopsis; }
            set { _synopsis = value; }
        }
        public string Distributor
        {
            get { return _distributor; }
            set { _distributor = value; }
        }
        public string Country_Of_Origin
        {
            get { return _country_of_origin; }
            set { _country_of_origin = value; }
        }
        public string Official_Website_Url
        {
            get { return _official_website_url; }
            set { _official_website_url = value; }
        }
        public DateTime ReleaseDate
        {
            get { return _release_date; }
            set
            {
                if (value != null)
                    _release_date = value;
            }
        }
        public DateTime DateAdded
        {
            get { return _date_added; }
            set { _date_added = value; }
        }
        public string Importer_Source
        {
            get { return _importer_source; }
            set { _importer_source = value; }
        }
        public IList SoundFormats
        {
            get { return _language_formats; }
        }
        public IList LanguageFormats
        {
            get { return _sound_formats; }
        }
        public IList Genres
        {
            get { return _genres; }
        }
        public IList Actors
        {
            get { return _actors; }
        }
        public IList Crew
        {
            get { return _crew; }
        }
        public IList Directors
        {
            get { return _directors; }
        }
        public IList Writers
        {
            get { return _writers; }
        }
        public IList Producers
        {
            get { return _producers; }
        }
        #endregion

        #region serialization methods
        public Title(SerializationInfo info, StreamingContext ctxt)
        {
            _file_location = (string)info.GetValue("file_location", typeof(string));
            _name = (string)info.GetValue("name", typeof(string));
            _description = (string)info.GetValue("description", typeof(string));
            _itemId = (int)info.GetValue("itemid", typeof(int));
            _front_boxart_path = (string)info.GetValue("front_boxart_path", typeof(string));
            _back_boxart_path = (string)info.GetValue("back_boxart_path", typeof(string));
            _synopsis = (string)info.GetValue("synopsis", typeof(string));
            _distributor = (string)info.GetValue("distributor", typeof(string));
            _country_of_origin = (string)info.GetValue("country_of_origin", typeof(string));
            _official_website_url = (string)info.GetValue("official_website_url", typeof(string));
            _date_added = (DateTime)info.GetValue("date_added", typeof(DateTime));
            _importer_source = (string)info.GetValue("importer_source", typeof(string));
            _runtime = (string)info.GetValue("runtime", typeof(string));
            _mpaa_rating = (string)info.GetValue("mpaa_rating", typeof(string));
            _release_date = (DateTime)info.GetValue("release_date", typeof(DateTime));
            _actors = (List<string>)info.GetValue("actors", typeof(List<string>));
            _crew = (List<string>)info.GetValue("crew", typeof(List<string>));
            _producers = (List<string>)info.GetValue("producers", typeof(List<string>));
            _writers = (List<string>)info.GetValue("writers", typeof(List<string>));
            _directors = (List<string>)info.GetValue("directors", typeof(List<string>));
            _sound_formats = (List<string>)info.GetValue("sound_formats", typeof(List<string>));
            _language_formats = (List<string>)info.GetValue("language_formats", typeof(List<string>));
            _genres = (List<Genre>)info.GetValue("genres", typeof(List<Genre>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("file_location", _file_location);
            info.AddValue("name", _name);
            info.AddValue("description", _description);
            info.AddValue("itemid", _itemId);
            info.AddValue("front_boxart_path", _front_boxart_path);
            info.AddValue("back_boxart_path", _back_boxart_path);
            info.AddValue("synopsis", _synopsis);
            info.AddValue("distributor", _distributor);
            info.AddValue("country_of_origin", _country_of_origin);
            info.AddValue("official_website_url", _official_website_url);
            info.AddValue("date_added", _date_added);
            info.AddValue("importer_source", _importer_source);
            info.AddValue("runtime", _runtime);
            info.AddValue("mpaa_rating", _mpaa_rating);
            info.AddValue("release_date", _release_date);
            info.AddValue("actors", _actors);
            info.AddValue("crew", _crew);
            info.AddValue("producers", _producers);
            info.AddValue("writers", _writers);
            info.AddValue("directors", _directors);
            info.AddValue("sound_formats", _sound_formats);
            info.AddValue("language_formats", _language_formats);
            info.AddValue("genres", _genres);
        }
        #endregion

        public Title()
        {
            _actors = new List<string>();
            _crew = new List<string>();
            _directors = new List<string>();
            _writers = new List<string>();
            _producers = new List<string>();
            _sound_formats = new List<string>();
            _language_formats = new List<string>();
            _genres = new List<Genre>();
        }
        ~Title()
        {
        }

        public void AddActor(string actor)
        {
            if (!_actors.Contains(actor))
                _actors.Add(actor);
        }
        public void AddCrew(string crew_member)
        {
            if (!_crew.Contains(crew_member))
                _crew.Add(crew_member);
        }
        public void AddDirector(string director)
        {
            _directors.Add(director);
        }
        public void AddWriter(string writer)
        {
            _writers.Add(writer);
        }
        public void AddProducer(string producer)
        {
            _producers.Add(producer);
        }
        public void AddGenre(Genre genre)
        {
            _genres.Add(genre);
        }
        public void AddSoundFormat(string sound_format)
        {
            _sound_formats.Add(sound_format);
        }
        public void AddLanguageFormat(string language_format)
        {
            _language_formats.Add(language_format);
        }
    }
}
