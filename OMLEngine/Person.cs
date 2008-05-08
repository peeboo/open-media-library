using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OMLEngine
{
    [Serializable()]
    public class Person : ISerializable
    {
        private string _first_name;
        private string _last_name;
        private Sex _sex;
        private DateTime _birth_date;
        private string _photo_path;

        /// <summary>
        /// First name of person
        /// </summary>
        public string first_name
        {
            get { return _first_name; }
            set
            {
                if (value != null)
                    _first_name = value;
            }
        }

        /// <summary>
        /// Last name of person
        /// </summary>
        public string last_name
        {
            get { return _last_name; }
            set
            {
                _last_name = value;
            }
        }

        /// <summary>
        /// Sex of Person (using the Sex enum)
        /// </summary>
        public Sex sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        /// <summary>
        /// Date of Birth
        /// </summary>
        public DateTime BirthDate
        {
            get { return _birth_date; }
            set
            {
                _birth_date = value;
            }
        }

        /// <summary>
        /// Full path the photo image for person
        /// </summary>
        public string PhotoPath
        {
            get { return _photo_path; }
            set
            {
                _photo_path = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Person()
        {
        }

        /// <summary>
        /// Constructor with first and last name
        /// </summary>
        /// <param name="first_name">string name</param>
        /// <param name="last_name">string name</param>
        public Person(string first_name, string last_name)
        {
            _first_name = first_name;
            _last_name = last_name;
        }

        public Person(SerializationInfo info, StreamingContext ctxt)
        {
            _first_name = info.GetString("first_name");
            _last_name = info.GetString("last_name");
            _sex = (Sex)info.GetValue("sex", typeof(Sex));
            _birth_date = info.GetDateTime("birth_date");
            _photo_path = info.GetString("photo_path");
        }
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("first_name", _first_name);
            info.AddValue("last_name", _last_name);
            info.AddValue("sex", _sex);
            info.AddValue("birth_date", _birth_date);
            info.AddValue("photo_path", _photo_path);
        }
    }
}
