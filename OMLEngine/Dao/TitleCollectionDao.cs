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
        /// Gets all the titles given the list of filters
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<Title> GetFilteredTitles(List<TitleFilter> filters)
        {
            var titles = DBContext.Instance.Titles;
            IQueryable<Title> results = null;

            foreach (TitleFilter filter in filters)
            {
                IQueryable<Title> lastQuery = results ?? titles;

                switch (filter.FilterType)
                {
                    case TitleFilterType.Genre:
                        results = ApplyGenreFilter(lastQuery, filter.FilterText);
                        break;

                    case TitleFilterType.Person:
                        results = ApplyPersonFilter(lastQuery, filter.FilterText);
                        break;

                    case TitleFilterType.VideoFormat:
                        results = ApplyFormatFilter(lastQuery, (VideoFormat)Enum.Parse(typeof(VideoFormat), filter.FilterText));
                        break;

                    case TitleFilterType.Watched:
                        results = ApplyWatchedFilter(
                                        lastQuery,
                                        (string.IsNullOrEmpty(filter.FilterText) ? false : bool.Parse(filter.FilterText)));
                        break;

                    case TitleFilterType.Tag:
                        results = ApplyTagFilter(lastQuery, filter.FilterText);
                        break;
                }
            }

            return results;
        }

        private static IQueryable<Title> ApplyTagFilter(IQueryable<Title> titles, string tagName)
        {
            return (from t in titles
                    from p in t.Tags
                    where t.Id == p.Title.Id && p.Name == tagName
                    select t).Distinct();
        }

        /// <summary>
        /// Returns all the titles given the watched/unwatched state
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="watched"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyWatchedFilter(IQueryable<Title> titles, bool watched)
        {
            if (watched)
            {
                return from t in DBContext.Instance.Titles
                       where t.WatchedCount != 0 && t.WatchedCount != null
                       select t;
            }
            else
            {
                return from t in DBContext.Instance.Titles
                       where t.WatchedCount == 0 || t.WatchedCount == null
                       select t;
            }
        }

        /// <summary>
        /// Returns all the titles for the given name
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyPersonFilter(IQueryable<Title> titles, string name)
        {
            return (from t in titles
                    from p in t.People
                    where t.Id == p.Title.Id && p.MetaData.FullName == name
                    select t).Distinct();
        }

        /// <summary>
        /// Returns all the titles in the given genre
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="genreName"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyGenreFilter(IQueryable<Title> titles, string genreName)
        {
            return (from title in titles
                    from g in title.Genres
                    where g.MetaData.Name == genreName
                    select title).Distinct();
        }

        /// <summary>
        /// Returns all the movies that have a disk with the given format
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyFormatFilter(IQueryable<Title> titles, VideoFormat format)
        {
            return (from title in titles
                    from disk in title.Disks
                    where title.Id == disk.TitleId && disk.VideoFormat == (byte)format
                    select title).Distinct();
        }

        /// <summary>
        /// Applys the unwatched filter
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyUnwatchedFilter(IQueryable<Title> titles)
        {
            return from title in titles
                   where title.WatchedCount == 0 || title.WatchedCount == null
                   select title;
        }

        /// <summary>
        /// Gets all the genres
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GenreMetaData> GetAllGenres()
        {
            return from t in DBContext.Instance.GenreMetaDatas
                   select t;
        }

        /// <summary>
        /// Gets all the people
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<BioData> GetAllPeople()
        {
            return from t in DBContext.Instance.BioDatas
                   select t;
        }

        /// <summary>
        /// Returns the number of items in a given genre
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        public static int GetItemsPerGenre(GenreMetaData genre)
        {
            return (from g in DBContext.Instance.Genres
                    from m in DBContext.Instance.GenreMetaDatas
                    where g.GenreMetaDataId == m.Id && m.Id == genre.Id
                    select g).Count();
        }

        /// <summary>
        /// Gets the number of movies a given person is in
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static int GetItemsPerPerson(BioData person)
        {
            return (from p in DBContext.Instance.Persons
                    from b in DBContext.Instance.BioDatas
                    where p.BioId == b.Id && b.Id == person.Id
                    select p).Count();
        }
    }
}
