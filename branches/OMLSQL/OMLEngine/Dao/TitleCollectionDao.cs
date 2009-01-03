﻿using System;
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
        /// Checks to see if any of the given disk paths exist in the db already
        /// </summary>
        /// <param name="diskPaths"></param>
        /// <returns></returns>
        public static bool ContainsDisks(IList<string> diskPaths)
        {
            var allDisks = from d in DBContext.Instance.Disks
                           select d.Path.ToLower();

            return diskPaths.Intersect(allDisks).Count() > 0;            
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

                    case TitleFilterType.Unwatched:                       
                        results = ApplyWatchedFilter(lastQuery, filter.FilterText);
                        break;

                    case TitleFilterType.Tag:
                        results = ApplyTagFilter(lastQuery, filter.FilterText);
                        break;

                    case TitleFilterType.Alpha:
                        results = ApplyAlphaFilter(lastQuery, filter.FilterText);
                        break;

                    case TitleFilterType.ParentalRating:
                        results = ApplyParentalRatingFilter(lastQuery, filter.FilterText);
                        break;

                    case TitleFilterType.Runtime:
                        results = ApplyRuntimeFilter(lastQuery, filter.FilterText);
                        break;

                    case TitleFilterType.Year:
                        results = ApplyYearFilter(lastQuery, filter.FilterText);
                        break;

                    case TitleFilterType.Country:
                        results = ApplyCountryFilter(lastQuery, filter.FilterText);
                        break;           
         
                    case TitleFilterType.Actor:
                        results = ApplyPersonFilter(lastQuery, filter.FilterText, PeopleRole.Actor);
                        break;

                    case TitleFilterType.Director:
                        results = ApplyPersonFilter(lastQuery, filter.FilterText, PeopleRole.Director);
                        break;

                    case TitleFilterType.UserRating:
                        results = ApplyUserRatingFilter(lastQuery, filter.FilterText);
                        break;
                        
                }
            }

            return results;
        }

        /// <summary>
        /// Applies the rating filter
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyUserRatingFilter(IQueryable<Title> titles, string rating)
        {
            double userRating;

            if (double.TryParse(rating, out userRating))
            {
                userRating *= 10;
            }

            return from t in titles
                   where t.UserRating == (byte)userRating
                   select t;
        }

        /// <summary>
        /// Applies the country filter
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyCountryFilter(IQueryable<Title> titles, string country)
        {
            return from t in titles
                   where t.CountryOfOrigin.ToLower() == country.ToLower()
                   select t;
        }

        /// <summary>
        /// returns the titles for the given year
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyYearFilter(IQueryable<Title> titles, string year)
        {
            int filterYear;

            int.TryParse(year, out filterYear);

            return from t in titles
                   where t.ReleaseDate.HasValue && t.ReleaseDate.Value.Year == filterYear
                   select t;
        }

        /// <summary>
        /// Returns all the titles that have a specific runtime
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="runtime"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyRuntimeFilter(IQueryable<Title> titles, string runtime)
        {
            int maxTime = TitleConfig.RuntimeFilterStringToInt(runtime);            

            if (maxTime == 0)
            {
                return from t in titles
                       where t.Runtime <= maxTime
                       select t;
            }               
            else if ( maxTime == -1 )
            {
                return from t in titles
                       where t.Runtime > TitleConfig.MAX_RUNTIME
                       select t;
            }
            else
            {
                int minTime = 0;

                // the min time is the previous index
                for (int x = 0; x < TitleConfig.RUNTIME_FILTER_LENGTHS.Length; x++)
                {
                    if (TitleConfig.RUNTIME_FILTER_LENGTHS[x] == maxTime)
                    {
                        minTime = TitleConfig.RUNTIME_FILTER_LENGTHS[x - 1];
                        break;
                    }
                }
                
                return from t in titles
                       where t.Runtime <= maxTime && t.Runtime > minTime
                       select t;
            }            
        }

        /// <summary>
        /// Returns all the titles that have a specific rating
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="rating"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyParentalRatingFilter(IQueryable<Title> titles, string rating)
        {
            return from t in titles
                   where t.ParentalRating == rating
                   select t;
        }

        /// <summary>
        /// Returns all the titles that start with the specific string
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="startCharacter"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyAlphaFilter(IQueryable<Title> titles, string startCharacter)
        {
            return from t in titles
                   where t.Name.StartsWith(startCharacter)
                   select t;
        }

        /// <summary>
        /// Returns all the titles that start with the given tag
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyTagFilter(IQueryable<Title> titles, string tagName)
        {
            return (from t in titles
                    from p in t.Tags
                    where p.Name.ToLower() == tagName.ToLower()
                    select t).Distinct();
        }

        /// <summary>
        /// Returns all the titles given the watched/unwatched state
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="watched"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyWatchedFilter(IQueryable<Title> titles, string watched)
        {
            bool unwatchedState;

            if (!bool.TryParse(watched, out unwatchedState))
                unwatchedState = false;

            if (unwatchedState)
            {
                return from t in titles
                       where t.WatchedCount != 0 && t.WatchedCount != null
                       select t;
            }
            else
            {
                return from t in titles
                       where t.WatchedCount == 0 || t.WatchedCount == null
                       select t;
            }
        }

        /// <summary>
        /// Returns all the titles for the given name in the given role
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="name"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyPersonFilter(IQueryable<Title> titles, string name, PeopleRole role)
        {
            return (from t in titles
                    from p in t.People
                    where p.MetaData.FullName == name && p.Role == (byte)role
                    select t).Distinct();
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
                    where p.MetaData.FullName == name
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
                    where disk.VideoFormat == (byte)format
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
        /// Returns all the genres for the given titles
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        public static IEnumerable<Genre> GetAllGenres(IQueryable<Title> titles)
        {
            return from t in titles
                   from g in t.Genres
                   select g;
        }
        
        /// <summary>
        /// Returns all the people in the given titles
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllPersons(IQueryable<Title> titles, PeopleRole role)
        {
            return from t in titles
                   from p in t.People
                   join b in DBContext.Instance.BioDatas on p.BioId equals b.Id
                   group b by b.FullName into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = g.Key, Count = g.Count() } ;

            //return from t in titles
            //       from p in t.People
            //       where p.Role == (byte) role
            //       select p;
        }

        /// <summary>
        /// Returns all the valid parental ratings
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllParentalRatings(IQueryable<Title> titles)
        {
            return (from t in titles
                    select t.ParentalRating).Distinct();
        }

        /// <summary>
        /// Returns all the valid video formats
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        public static IEnumerable<byte> GetAllVideoFormats(IQueryable<Title> titles)
        {
            return (from t in titles
                    from d in t.Disks
                    select d.VideoFormat).Distinct();
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
        /// Deletes the given title
        /// </summary>
        /// <param name="title"></param>
        public static void DeleteTitle(Title title)
        {
            foreach (Disk disk in title.Disks)
                DBContext.Instance.Disks.DeleteOnSubmit(disk);

            foreach (Genre genre in title.Genres)
                DBContext.Instance.Genres.DeleteOnSubmit(genre);

            foreach (Tag tag in title.Tags)
                DBContext.Instance.Tags.DeleteOnSubmit(tag);

            foreach (Person person in title.People)
                DBContext.Instance.Persons.DeleteOnSubmit(person);

            DBContext.Instance.Titles.DeleteOnSubmit(title);
            DBContext.Instance.SubmitChanges();
        }
    }
}
