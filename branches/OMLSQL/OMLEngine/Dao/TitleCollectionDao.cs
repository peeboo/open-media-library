using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Dao
{
    internal static class TitleCollectionDao
    {
        public static GenreMetaData GetGenreMetaDataByName(string name)
        {
            return DBContext.Instance.GenreMetaDatas.SingleOrDefault(t => t.Name.ToLower() == name.ToLower());
        }

        public static BioData GetPersonBioDataByName(string name)
        {
            return DBContext.Instance.BioDatas.SingleOrDefault(t => t.FullName.ToLower() == name.ToLower());
        }

        public static Tag GetTagByTagName(string name)
        {
            return DBContext.Instance.Tags.SingleOrDefault(t => t.Name.ToLower() == name.ToLower());
        }

        public static void AddTitle(Title title)
        {
            DBContext.Instance.Titles.InsertOnSubmit(title);
            DBContext.Instance.SubmitChanges();
        }

        public static Title GetTitleById(int id)
        {
            return DBContext.Instance.Titles.SingleOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Returns all the titles but doesn't return the actors/directors/writers
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Title> GetAllTitles()
        {
            var titles = from t in DBContext.Instance.Titles
                         select t;

            return titles;
        }

        /// <summary>
        /// Returns all the unwatched titles
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Title> GetUnwatchedTitles()
        {
            var titles = from t in DBContext.Instance.Titles
                         where t.WatchedCount == 0 || t.WatchedCount == null
                         select t;

            return titles;
        }

        /// <summary>
        /// Returns all the titles for a given person
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<Title> GetFilteredTitlesPerson(string name)
        {
            var titles = (from t in DBContext.Instance.Titles
                          from p in DBContext.Instance.Persons
                          where t.Id == p.Title.Id && p.MetaData.FullName == name
                          select t).Distinct();

            return titles;
        }

        /// <summary>
        /// Gets all the genres
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GenreMetaData> GetAllGenres()
        {
            var genres = from t in DBContext.Instance.GenreMetaDatas
                         select t;

            return genres;
        }

        /// <summary>
        /// Gets all the people
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<BioData> GetAllPeople()
        {
            var people = from t in DBContext.Instance.BioDatas
                         select t;

            return people;
        }

        /// <summary>
        /// Returns the number of items in a given genre
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        public static int GetItemsPerGenre(GenreMetaData genre)
        {
            int count = (from g in DBContext.Instance.Genres
                         from m in DBContext.Instance.GenreMetaDatas
                         where g.GenreMetaDataId == m.Id && m.Id == genre.Id
                         select g).Count();

            return count;
        }

        /// <summary>
        /// Gets the number of movies a given person is in
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static int GetItemsPerPerson(BioData person)
        {
            int count = (from p in DBContext.Instance.Persons
                         from b in DBContext.Instance.BioDatas
                         where p.BioId == b.Id && b.Id == person.Id
                         select p).Count();

            return count;
        }
    }
}
