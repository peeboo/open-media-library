using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace OMLEngine
{
    [Serializable()]
    public class Person : ISerializable
    {
        public enum Sex { Male, Female };

        private string _name;
        private Sex _sex;

        public string name
        {
            get { return _name; }
            set
            {
                if (value != null)
                    _name = value;
            }
        }
        public Sex sex
        {
            get { return _sex; }
            set
            {
                if (value != null)
                    _sex = value;
            }
        }

        public Person()
        {
        }

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
