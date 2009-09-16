using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Net;
using System.Linq;
using System.Collections;

namespace OMLEngine.Trailers
{
    public enum AppleTrailerRes
    {
        LowRes,
        HiRes
    }

    public class AppleTrailers
    {
        private const string LoFiUrl = @"http://www.apple.com/trailers/home/xml/current.xml";
        private const string HiFiUrl = @"http://www.apple.com/trailers/home/xml/current_720p.xml";

        private List<AppleTrailer> appleTrailers = new List<AppleTrailer>();

        /// <summary>
        /// Get all the trailers
        /// </summary>
        public IEnumerable<AppleTrailer> AllTrailers
        {
            get { return from a in appleTrailers orderby a.PostDate descending select a; }
        }

        /// <summary>
        /// Get all the genres
        /// </summary>
        public IEnumerable<string> AllGenres
        {
            get
            {
                return (from a in appleTrailers
                        from g in a.Genres
                        orderby g ascending
                        select g).Distinct();
            }
        }

        /// <summary>
        /// Constructor for creating AppleTrailers object.  The constructor will do the outbound query and load up a collection
        /// of titles you can query/sort against.
        /// </summary>
        /// <param name="res">The resolution of the trailers</param>
        public AppleTrailers(AppleTrailerRes res)
        {
            string url = (res == AppleTrailerRes.HiRes)
                            ? HiFiUrl
                            : LoFiUrl;
            try
            {
                // load the xml
                using (WebClient client = new WebClient())
                {
                    using (Stream stream = client.OpenRead(url))
                    {
                        XmlTextReader reader = new XmlTextReader(stream);

                        reader.Read();

                        AppleTrailer trailer = null;

                        DateTimeFormatInfo dateFormat = new DateTimeFormatInfo();
                        dateFormat.ShortDatePattern = "yyyy-MM-dd";

                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "movieinfo":
                                        if (trailer != null)
                                            appleTrailers.Add(trailer);

                                        trailer = new AppleTrailer();

                                        trailer.Id = reader.GetAttribute(0);
                                        break;                                        

                                    case "title":
                                        trailer.Title = ReadToValue(reader);
                                        break;

                                    case "runtime":
                                        trailer.RunTime = ReadToValue(reader);
                                        break;

                                    case "rating":
                                        trailer.Rating = ReadToValue(reader);
                                        break;

                                    case "studio":
                                        trailer.Studio = ReadToValue(reader);
                                        break;

                                    case "postdate":
                                        trailer.PostDate = DateTime.Parse(ReadToValue(reader), dateFormat);
                                        break;

                                    case "releasedate":
                                        trailer.ReleaseDate = DateTime.Parse(ReadToValue(reader), dateFormat);
                                        break;

                                    case "director":
                                        trailer.Director = ReadToValue(reader);
                                        break;

                                    case "description":
                                        trailer.Description = ReadToValue(reader);
                                        break;

                                    case "cast":
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.EndElement &&
                                                reader.Name == "cast")
                                                break;

                                            if (reader.Name == "name")
                                            {
                                                trailer.AddCast(ReadToValue(reader));
                                            }
                                        }
                                        break;

                                    case "genre":
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.EndElement &&
                                                reader.Name == "genre")
                                                break;

                                            if (reader.Name == "name")
                                            {
                                                trailer.AddGenre(ReadToValue(reader));
                                            }
                                        }
                                        break;

                                    case "poster":
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.EndElement &&
                                                reader.Name == "poster")
                                                break;

                                            if (reader.Name == "location")
                                            {
                                                trailer.SmallImageUrl = ReadToValue(reader);
                                            }
                                            else if (reader.Name == "xlarge")
                                            {
                                                trailer.LargeImageUrl = ReadToValue(reader);
                                            }
                                        }
                                        break;

                                    case "preview":
                                        while (reader.Read())
                                        {
                                            if (reader.NodeType == XmlNodeType.EndElement &&
                                                reader.Name == "preview")
                                                break;

                                            if (reader.Name == "large")
                                            {
                                                trailer.TrailerUrl = ReadToValue(reader);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {

            }
        }                

        private static string ReadToValue(XmlTextReader reader)
        {
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Text)
                    break;

            return reader.ReadContentAsString();
        }

        /// <summary>
        /// Get all the trailers added with the last N number of days
        /// </summary>
        /// <param name="daysAgo"></param>
        /// <returns></returns>
        public IEnumerable<AppleTrailer> GetTrailersByDateAdded(int daysAgo)
        {
            return from a in appleTrailers
                   where (DateTime.Now - a.PostDate).TotalDays < daysAgo
                   orderby a.PostDate descending
                   select a;
        }

        /// <summary>
        /// Get all the movies being released within the next N number of days
        /// </summary>
        /// <param name="daysToCome"></param>
        /// <returns></returns>
        public IEnumerable<AppleTrailer> GetTrailersByReleaseDate(int daysToCome)
        {
            return from a in appleTrailers
                   where a.ReleaseDate > DateTime.Now && (a.ReleaseDate - DateTime.Now).TotalDays <= daysToCome
                   orderby a.ReleaseDate descending
                   select a;
        }
    }
    
}
