using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OMLEngine
{
    [Serializable()]
    public class Title : ISerializable
    {
        #region locals
        private string _name;
        private string _description;
        private int _itemId;
        private string _boxart_path;
        private string _runtime;
        private string _mpaa_rating;
        private string _imdb_rating;
        private string _summary;
        private List<string> _actors;
        private List<string> _crew;
        private List<string> _directors;
        private List<string> _writers;
        private List<string> _producers;
        private DateTime _release_date;
        #endregion

        #region properties
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
        public string boxart_path
        {
            get { return _boxart_path; }
            set
            {
                if (value != null)
                    _boxart_path = value;
            }
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
        public string IMDBRating
        {
            get { return _imdb_rating; }
            set
            {
                if (value != null)
                    _imdb_rating = value;
            }
        }
        public string Summary
        {
            get { return _summary; }
            set
            {
                if (value != null)
                    _summary = value;
            }
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
/*
        private string _name;
        private string _description;
        private int _itemId;
        private string _boxart_path;
        private string _runtime;
        private string _mpaa_rating;
        private string _imdb_rating;
        private string _summary;
        private List<string> _actors;
        private List<string> _crew;
        private List<string> _directors;
        private List<string> _writers;
        private List<string> _producers;
        private DateTime _release_date;

*/
            _name = (string)info.GetValue("name", typeof(string));
            _description = (string)info.GetValue("description", typeof(string));
            _itemId = (int)info.GetValue("itemid", typeof(int));
            _boxart_path = (string)info.GetValue("boxart_path", typeof(string));
            _runtime = (string)info.GetValue("runtime", typeof(string));
            _mpaa_rating = (string)info.GetValue("mpaa_rating", typeof(string));
            _imdb_rating = (string)info.GetValue("imdb_rating", typeof(string));
            _actors = (List<string>)info.GetValue("actors", typeof(List<string>));
            _crew = (List<string>)info.GetValue("crew", typeof(List<string>));
            _producers = (List<string>)info.GetValue("producers", typeof(List<string>));
            _writers = (List<string>)info.GetValue("writers", typeof(List<string>));
            _directors = (List<string>)info.GetValue("directors", typeof(List<string>));
            _release_date = (DateTime)info.GetValue("release_date", typeof(DateTime));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("name", _name);
            info.AddValue("description", _description);
            info.AddValue("itemid", _itemId);
            info.AddValue("boxart_path", _boxart_path);
            info.AddValue("runtime", _runtime);
            info.AddValue("mpaa_rating", _mpaa_rating);
            info.AddValue("imdb_rating", _imdb_rating);
            info.AddValue("actors", _actors);
            info.AddValue("crew", _crew);
            info.AddValue("producers", _producers);
            info.AddValue("writers", _writers);
            info.AddValue("directors", _directors);
            info.AddValue("release_date", _release_date);
        }
        #endregion

        public Title()
        {
            _actors = new List<string>();
            _crew = new List<string>();
            _directors = new List<string>();
            _writers = new List<string>();
            _producers = new List<string>();
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
    }
}
