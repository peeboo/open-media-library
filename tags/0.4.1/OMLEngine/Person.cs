using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OMLEngine
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class Person : ISerializable
    {
        private string _full_name;
        private Sex _sex;
        private DateTime _birth_date;
        private string _photo_path;

        /// <summary>
        /// Full name
        /// </summary>
        public string full_name
        {
            get { return _full_name; }
            set
            {
                if (value != null)
                    _full_name = value.Trim();
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
        /// <param name="full_name">string name</param>
        public Person(string full_name)
        {
            _full_name = full_name;
        }

        public override string ToString()
        {
            return full_name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public Person(SerializationInfo info, StreamingContext ctxt)
        {
            _full_name = info.GetString("full_name");
            _sex = (Sex)info.GetValue("sex", typeof(Sex));
            _birth_date = info.GetDateTime("birth_date");
            _photo_path = info.GetString("photo_path");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("full_name", _full_name);
            info.AddValue("sex", _sex);
            info.AddValue("birth_date", _birth_date);
            info.AddValue("photo_path", _photo_path);
        }
    }
}
