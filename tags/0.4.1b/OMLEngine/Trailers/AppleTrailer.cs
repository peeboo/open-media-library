using System;
using System.Collections.Generic;
using System.Text;

namespace OMLEngine.Trailers
{
    public class AppleTrailer
    {
        private List<string> cast = new List<string>();
        private List<string> genres = new List<string>();

        public string Title { get; internal set; }
        public string RunTime { get; internal set; }
        public string Rating { get; internal set; }
        public string Studio { get; internal set; }
        public DateTime PostDate { get; internal set; }
        public DateTime ReleaseDate { get; internal set; }
        public string Description { get; internal set; }
        public string Id { get; internal set; }

        public string Director { get; internal set; }
        public IList<string> Cast { get { return cast.AsReadOnly(); } }
        
        public IList<string> Genres { get { return genres.AsReadOnly(); } }

        public string LargeImageUrl { get; internal set; }
        public string SmallImageUrl { get; internal set; }        

        public string TrailerUrl { get; internal set; }

        public void AddCast(string castMember)
        {
            cast.Add(castMember);
        }

        public void AddGenre(string genre)
        {
            genres.Add(genre);
        }
    }
}
