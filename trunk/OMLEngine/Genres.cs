using System;
using Dao = OMLEngine.Dao;
namespace OMLEngine
{
    public class Genre
    {
        private Dao.GenreMetaData _genreMetaData;

        public Genre()
        {
            _genreMetaData = new OMLEngine.Dao.GenreMetaData();
        }

        internal Genre(OMLEngine.Dao.GenreMetaData genreMetaData)
        {
            _genreMetaData = genreMetaData;
        }

        public long Id
        {
            get { return _genreMetaData.Id; }
            set { _genreMetaData.Id = value; }
        }

        public string Name
        {
            get { return _genreMetaData.Name ?? string.Empty; }
            set { _genreMetaData.Name = value; }
        }

        public override string ToString()
        {
            return _genreMetaData.Name;
        }

        /*public byte[] Photo
        {
            get { return _genreMetaData.Photo; }
            set { _genreMetaData.Photo = value; }
        }*/

    }
    /*public class Genres
    {
        public static string Action = @"Action";
        public static string Adventure = @"Adventure";
        public static string Animated = @"Animated";
        public static string Animation = @"Animation";
        public static string Biography = @"Biography";
        public static string Comedy = @"Comedy";
        public static string Crime = @"Crime";
        public static string Documentary = @"Documentary";
        public static string Drama = @"Drama";
        public static string Family = @"Family";
        public static string Fantasy = @"Fantasy";
        public static string FilmNoir = @"Film-Noir";
        public static string GameShow = @"Game-Show";
        public static string History = @"History";
        public static string Horror = @"Horror";
        public static string Music = @"Music";
        public static string Musical = @"Musical";
        public static string Mystery = @"Mystery";
        public static string News = @"News";
        public static string RealityTV = @"Reality-TV";
        public static string Romance = @"Romance";
        public static string SciFi = @"Sci-Fi";
        public static string Short = @"Short";
        public static string Sport = @"Sport";
        public static string TalkShow = @"Talk-Show";
        public static string Thriller = @"Thriller";
        public static string War = @"War";
        public static string Western = @"Western";
    }*/
}
