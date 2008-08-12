using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace AppleTrailers
{
    public class TrailerData
    {
        public TrailerData()
        {
        }

        public List<AppleTitle> GetAppleTitles()
        {
            List<AppleTitle> appleTitles = new List<AppleTitle>();

            try
            {
                WebClient client = new WebClient();
                Stream strm = client.OpenRead(@"http://www.apple.com/trailers/home/feeds/just_hd.json");
                StreamReader sr = new StreamReader(strm);
                string json = sr.ReadToEnd();
                MemoryStream ms = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(json));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<AppleTitle>));
                appleTitles = ser.ReadObject(ms) as List<AppleTitle>;
                ms.Close();
            }
            catch (Exception e) { }

            return appleTitles;
        }

        [DataContract(Name="AppleTrailer", Namespace="")]
        public class AppleTitle
        {
            string _location;

            [DataMember]
            public string title;
            [DataMember]
            public string releasedate;
            [DataMember]
            public string studio;
            [DataMember]
            public string poster;
            [DataMember]
            public string moviesite;
            [DataMember]
            public string location
            {
                get { return _location; }
                set
                {
                    _location = @"www.apple.com" + value;
                }
            }
            [DataMember]
            public string director;
            [DataMember]
            public string rating;
            [DataMember]
            public List<string> genre;
            [DataMember]
            public List<string> actors;
            [DataMember]
            public List<Trailer> trailers;
        }

        [DataContract(Name="Trailer", Namespace="")]
        public class Trailer
        {
            [DataMember]
            public string postdate;
            [DataMember]
            public string url;
            [DataMember]
            public string type;
            [DataMember]
            public string exclusive;
            [DataMember]
            public bool hd;
        }
    }
}
