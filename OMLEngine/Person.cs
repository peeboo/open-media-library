using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OMLEngine
{
    [Serializable()]
    public class Person : ISerializable
    {
        private string _name;
        private Sex _sex;

        /// <summary>
        /// Name of person
        /// </summary>
        public string name
        {
            get { return _name; }
            set
            {
                if (value != null)
                    _name = value;
            }
        }

        /// <summary>
        /// Sex of Person (using the Sex enum)
        /// </summary>
        public Sex sex
        {
            get { return _sex; }
            set
            {
                if (value != null)
                    _sex = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Person()
        {
        }

        /// <summary>
        /// Constructor with a name
        /// </summary>
        /// <param name="name">name of person to create</param>
        public Person(string name)
        {
            _name = name;
        }

        public Person(SerializationInfo info, StreamingContext ctxt)
        {
            _name = (string)info.GetValue("name", typeof(string));
            _sex = (Sex)info.GetValue("sex", typeof(Sex));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("name", _name);
            info.AddValue("sex", _sex);
        }
    }
}
