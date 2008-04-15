using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace Library
{
    [Serializable()]
    public class Title : ISerializable
    {
        #region locals
        private string _name;
        private string _description;
        private int _itemId;
        private Image _boxart;
        private string _runtime;
        private string _mpaa_rating;
        private string _imdb_rating;
        List<string> _actors;
        List<string> _crew;
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
        public string description
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
                if (value != null)
                    _itemId = value;
            }
        }
        public Image boxart
        {
            get { return _boxart; }
            set
            {
                if (value != null)
                    _boxart = value;
            }
        }
        public string runtime
        {
            get { return _runtime; }
            set
            {
                if (value != null)
                    _runtime = value;
            }
        }
        public string mpaa_rating
        {
            get { return _mpaa_rating; }
            set
            {
                if (value != null)
                    _mpaa_rating = value;
            }
        }
        public string imdb_rating
        {
            get { return _imdb_rating; }
            set
            {
                if (value != null)
                    _imdb_rating = value;
            }
        }
        public IList actors
        {
            get { return _actors; }
        }
        public IList crew
        {
            get { return _crew; }
        }
        #endregion

        #region serialization methods
        public Title(SerializationInfo info, StreamingContext ctxt)
        {
            _name = (string)info.GetValue("name", typeof(string));
            _description = (string)info.GetValue("description", typeof(string));
            _itemId = (int)info.GetValue("itemid", typeof(int));
            _boxart = (Image)info.GetValue("boxart", typeof(Image));
            _runtime = (string)info.GetValue("runtime", typeof(string));
            _mpaa_rating = (string)info.GetValue("mpaa_rating", typeof(string));
            _imdb_rating = (string)info.GetValue("imdb_rating", typeof(string));
            _actors = (List<string>)info.GetValue("actors", typeof(List<string>));
            _crew = (List<string>)info.GetValue("crew", typeof(List<string>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("name", _name);
            info.AddValue("description", _description);
            info.AddValue("itemid", _itemId);
            info.AddValue("boxart", _boxart);
            info.AddValue("runtime", _runtime);
            info.AddValue("mpaa_rating", _mpaa_rating);
            info.AddValue("imdb_rating", _imdb_rating);
            info.AddValue("actors", _actors);
            info.AddValue("crew", _crew);
        }
        #endregion

        public Title()
        {
            _actors = new List<string>();
            _crew = new List<string>();
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

    }
}
