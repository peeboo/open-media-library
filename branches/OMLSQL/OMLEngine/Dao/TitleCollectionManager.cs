using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dao = OMLEngine.Dao;

namespace OMLEngine
{
    public static class TitleCollectionManager
    {
        /// <summary>
        /// Adds a title to the db
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool AddTitle(Title title)
        {
            // setup all the collections objects
            // todo : solomon : this should go away once it's understood how people 
            // will be added
            Dao.TitleDao.SetupCollectionsToBeAdded(title);

            // todo : solomon : do your duplicate logic here
            Dao.TitleCollectionDao.AddTitle(title.DaoTitle);
            return true;
        }

        /// <summary>
        /// Persists any pending updates to the database
        /// </summary>
        /// <returns></returns>
        public static bool SaveTitleUpdates()
        {            
            // todo : solomon : add error handing and logging here
            Dao.DBContext.Instance.SubmitChanges();            
            return true;
        }

        /// <summary>
        /// Deletes all the titles from the db - leaves the genre and people meta data
        /// </summary>
        public static void DeleteAllTitles()
        {
            // this needs to be optimized - i believe this approach will use a select first
            var deletePeople = from d in Dao.DBContext.Instance.Persons
                               select d;

            Dao.DBContext.Instance.Persons.DeleteAllOnSubmit(deletePeople);

            var deleteDisks = from d in Dao.DBContext.Instance.Disks
                               select d;

            Dao.DBContext.Instance.Disks.DeleteAllOnSubmit(deleteDisks);

            var deleteGenres = from d in Dao.DBContext.Instance.Genres
                              select d;

            Dao.DBContext.Instance.Genres.DeleteAllOnSubmit(deleteGenres);

            var deleteTags = from d in Dao.DBContext.Instance.Tags
                               select d;

            Dao.DBContext.Instance.Tags.DeleteAllOnSubmit(deleteTags);

            var deleteTitles = from d in Dao.DBContext.Instance.Titles
                             select d;

            Dao.DBContext.Instance.Titles.DeleteAllOnSubmit(deleteTitles);            

            Dao.DBContext.Instance.SubmitChanges();
        }

        /// <summary>
        /// Deletes the genre meta data
        /// </summary>
        public static void DeleteAllGenreMetaData()
        {
            var deleteGenreMeta = from d in Dao.DBContext.Instance.GenreMetaDatas
                                  select d;

            Dao.DBContext.Instance.GenreMetaDatas.DeleteAllOnSubmit(deleteGenreMeta);

            Dao.DBContext.Instance.SubmitChanges();
        }

        /// <summary>
        /// Deletes the people meta data
        /// </summary>
        public static void DeleteAllPeopleData()
        {
            var deleteBio = from d in Dao.DBContext.Instance.BioDatas
                             select d;

            Dao.DBContext.Instance.BioDatas.DeleteAllOnSubmit(deleteBio);

            Dao.DBContext.Instance.SubmitChanges();
        }

        /// <summary>
        /// Deletes all the user information from the database
        /// </summary>
        public static void DeleteAllDBData()
        {
            TitleCollectionManager.DeleteAllTitles();
            TitleCollectionManager.DeleteAllPeopleData();
            TitleCollectionManager.DeleteAllGenreMetaData();
        }

        /// <summary>
        /// Gets all titles with data needed for browsing
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Title> GetAllTitles()
        {
            // get all the titles
            return ConvertDaoTitlesToTitles(Dao.TitleCollectionDao.GetAllTitles());            
        }

        /// <summary>
        /// Returns the title for the given filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterText"></param>
        /// <returns></returns>
        public static IEnumerable<Title> GetFilteredTitles(TitleFilterType filter, string filterText)
        {
            return ConvertDaoTitlesToTitles(
                            Dao.TitleCollectionDao.GetFilteredTitles(new List<TitleFilter> { new TitleFilter(filter, filterText) })
                        );
        }        

        /// <summary>
        /// Returns the titles for the given filter
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<Title> GetFilteredTitles(List<TitleFilter> filters)
        {            
            return (filters == null || filters.Count == 0 ) 
                    ? GetAllTitles()
                    : ConvertDaoTitlesToTitles(Dao.TitleCollectionDao.GetFilteredTitles(filters));
        }

        /// <summary>
        /// Returns all the valid parental ratings and their count
        /// todo : solomon : optimize this
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllParentalRatings(List<TitleFilter> filter)
        {
            IQueryable<Dao.Title> titles = ( filter == null || filter.Count == 0 )
                                                ? Dao.DBContext.Instance.Titles
                                                : (IQueryable<Dao.Title>)Dao.TitleCollectionDao.GetFilteredTitles(filter);

            IEnumerable<string> ratings = Dao.TitleCollectionDao.GetAllParentalRatings(titles);

            foreach (string rating in ratings)
            {
                int count = Dao.TitleCollectionDao.GetItemsPerParentalRating(titles, rating);
                if (count == 0)
                    continue;

                yield return new FilteredCollection() { Name = rating, Count = count };
            }
        }

        /// <summary>
        /// Returns all the valid video formats and their counts
        /// todo : solomon : optimize this
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllVideoFormats(List<TitleFilter> filter)
        {
            IQueryable<Dao.Title> titles = (filter == null || filter.Count == 0)
                                                ? Dao.DBContext.Instance.Titles
                                                : (IQueryable<Dao.Title>)Dao.TitleCollectionDao.GetFilteredTitles(filter);

            IEnumerable<byte> formats = Dao.TitleCollectionDao.GetAllVideoFormats(titles);

            foreach (byte format in formats)
            {
                int count = Dao.TitleCollectionDao.GetItemsPerVideoFormat(titles, format);
                if (count == 0)
                    continue;

                yield return new FilteredCollection() { Name = ((VideoFormat) format).ToString(), Count = count };
            }
        }

        /// <summary>
        /// Returns all the valid runtimes for the collection and their counts
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllRuntimes(List<TitleFilter> filter)
        {
            // for now i think it's more effecient to just grab the whole list and filter
            // it.  some smart sql person may find a faster way to do this in sql
            Dictionary<int, int> timeToCount = new Dictionary<int, int>();

            foreach (Title title in GetFilteredTitles(filter))
            {
                foreach (int time in TitleConfig.RUNTIME_FILTER_LENGTHS)
                {
                    if (title.Runtime <= time)
                    {
                        IncrementCount(timeToCount, time);
                        break;
                    }
                }
            }

            // add filtercollection for every count
            foreach (int length in TitleConfig.RUNTIME_FILTER_LENGTHS)
            {
                if (timeToCount.ContainsKey(length))
                    yield return new FilteredCollection() { Name = TitleConfig.RuntimeToFilterString(length), Count = timeToCount[length] };
            }            
        }

        /// <summary>
        /// Returns the all the valid years for the given filter and their counts
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllYears(List<TitleFilter> filter)
        {
            // again i'm gonna run through the list and build the filter instead of doing it in sql
            // hopefully we can figure out a sql way of doing it
            Dictionary<int, int> yearToCount = new Dictionary<int, int>();

            foreach (Title title in GetFilteredTitles(filter))
            {
                IncrementCount(yearToCount, title.ReleaseDate.Year);
            }

            List<FilteredCollection> yearList = new List<FilteredCollection>(yearToCount.Count);

            foreach (int year in yearToCount.Keys)
            {
                yearList.Add(new FilteredCollection() { Name = year.ToString(), Count = yearToCount[year] });
            }

            yearList.Sort();

            return yearList;
        }

        /// <summary>
        /// Returns all the countries and counts for the given filters
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllCountries(List<TitleFilter> filter)
        {
            // again i'm gonna run through the list and build the filter instead of doing it in sql
            // hopefully we can figure out a sql way of doing it
            Dictionary<string, int> countryToCount = new Dictionary<string, int>();

            foreach (Title title in GetFilteredTitles(filter))
            {
                if (string.IsNullOrEmpty(title.CountryOfOrigin))
                    continue;

                IncrementCount(countryToCount, title.CountryOfOrigin);
            }

            List<FilteredCollection> countryList = new List<FilteredCollection>(countryToCount.Count);

            foreach (string country in countryToCount.Keys)
            {
                countryList.Add(new FilteredCollection() { Name = country, Count = countryToCount[country] });
            }

            countryList.Sort();

            return countryList;
        }

        /// <summary>
        /// Returns all the tags and their counts for a given filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllTags(List<TitleFilter> filter)
        {
            // again i'm gonna run through the list and build the filter instead of doing it in sql
            // hopefully we can figure out a sql way of doing it
            Dictionary<string, int> tagToCount = new Dictionary<string, int>();

            foreach (Title title in GetFilteredTitles(filter))
            {
                if (title.Tags != null && title.Tags.Count != 0)
                {
                    foreach (string tag in title.Tags)
                    {
                        if (!string.IsNullOrEmpty(tag))
                            IncrementCount(tagToCount, tag);
                    }
                }
            }

            List<FilteredCollection> tagList = new List<FilteredCollection>(tagToCount.Count);

            foreach (string tag in tagToCount.Keys)
            {
                tagList.Add(new FilteredCollection() { Name = tag, Count = tagToCount[tag] });
            }

            tagList.Sort();

            return tagList;
        }

        /// <summary>
        /// Increments the count for a given value in the dictionary
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        private static void IncrementCount(Dictionary<int, int> collection, int item)
        {
            if (collection.ContainsKey(item))
                collection[item]++;
            else
                collection.Add(item, 1);
        }

        /// <summary>
        /// Increments the count for a given value in the dictionary
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        private static void IncrementCount(Dictionary<string, int> collection, string item)
        {
            if (collection.ContainsKey(item))
                collection[item]++;
            else
                collection.Add(item, 1);
        }

        /// <summary>
        /// Returns all the genres and their counts
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllGenres()
        {
            return GetAllGenres(null);
        }

        /// <summary>
        /// Returns all the genres and counts given a filter
        /// todo : solomon : even though this method returns pretty quick it could probably be done
        /// more effeciently
        /// </summary>
        /// <param name="givenFilter"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllGenres(List<TitleFilter> filter)
        {        
            IQueryable<Dao.Title> titles = (filter == null || filter.Count == 0)
                                            ? Dao.DBContext.Instance.Titles
                                            : (IQueryable<Dao.Title>)Dao.TitleCollectionDao.GetFilteredTitles(filter);

            IEnumerable<Dao.GenreMetaData> genres = Dao.TitleCollectionDao.GetAllGenres(titles);

            foreach (Dao.GenreMetaData genre in genres)
            {
                int count = Dao.TitleCollectionDao.GetItemsPerGenre(titles, genre);
                if (count == 0)
                    continue;

                yield return new FilteredCollection() { Name = genre.Name, Count = count };
            }            
        }        

        /// <summary>
        /// Gets all the people and their count of movies
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllPeople()
        {
            IEnumerable<Dao.BioData> people = Dao.TitleCollectionDao.GetAllPeople();

            foreach (Dao.BioData bio in people)
            {
                int count = Dao.TitleCollectionDao.GetItemsPerPerson(bio);
                if (count == 0)
                    continue;

                yield return new FilteredCollection() { Name = bio.FullName, Count = count };
            }
        }

        /// <summary>
        /// Creates a list of title objects from the DAO title objects
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        private static IEnumerable<Title> ConvertDaoTitlesToTitles(IEnumerable<Dao.Title> titles)
        {
            foreach (Dao.Title title in titles)
                yield return new Title(title);
        }

        /// <summary>
        /// Gets a title by it's id - will include all the actor information
        /// </summary>
        /// <param name="titleId"></param>
        /// <returns></returns>
        public static Title GetTitle(int titleId)
        {
            return new Title(Dao.TitleCollectionDao.GetTitleById(titleId));
        }

        /// <summary>
        /// Removes the given title and all it's meta data
        /// </summary>
        /// <param name="titleId"></param>
        public static void DeleteTitle(Title title)
        {
            Dao.TitleCollectionDao.DeleteTitle(title.DaoTitle);
        }

        /// <summary>
        /// Increments the watch count on the title object
        /// </summary>
        /// <param name="title"></param>
        public static void IncrementWatchedCount(Title title)
        {
            Dao.Title daoTitle = Dao.TitleCollectionDao.GetTitleById(title.Id);
            daoTitle.WatchedCount = (daoTitle.WatchedCount == null) ? 1 : daoTitle.WatchedCount.Value + 1;
            
            Dao.DBContext.Instance.SubmitChanges();

            title.WatchedCount = daoTitle.WatchedCount.Value;
        }

        /// <summary>
        /// Clears the watched count on the title
        /// </summary>
        /// <param name="title"></param>
        public static void ClearWatchedCount(Title title)
        {
            Dao.Title daoTitle = Dao.TitleCollectionDao.GetTitleById(title.Id);
            daoTitle.WatchedCount = null;

            Dao.DBContext.Instance.SubmitChanges();

            title.WatchedCount = 0;
        }
        
        /// <summary>
        /// Checks to see if disk is already using the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ContainsDisks(string path)
        {
            return Dao.TitleCollectionDao.ContainsDisks(new string[] { path.ToLowerInvariant() });
        }

        /// <summary>
        /// returns true if any of the disks returned already exist in the database
        /// </summary>
        /// <param name="disks"></param>
        public static bool ContainsDisks(IList<Disk> disks)
        {
            List<string> paths = new List<string>(disks.Count);
            foreach (Disk disk in disks)
            {
                if (string.IsNullOrEmpty(disk.Path))
                    continue;

                paths.Add(disk.Path.ToLowerInvariant());
            }

            return Dao.TitleCollectionDao.ContainsDisks(paths);
        }

        /// <summary>
        /// Adds a genre to a title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="genre"></param>
        public static void AddGenreToTitle(Title title, string genre)
        {
            if (string.IsNullOrEmpty(genre))
                return;

            // see if the genre exists
            Dao.GenreMetaData meta = Dao.TitleCollectionDao.GetGenreMetaDataByName(genre);

            if (meta == null)
            {
                meta = new OMLEngine.Dao.GenreMetaData();
                meta.Name = genre;

                // save the genre
                Dao.DBContext.Instance.GenreMetaDatas.InsertOnSubmit(meta);
                Dao.DBContext.Instance.SubmitChanges();
            }

            title.DaoTitle.Genres.Add(new Dao.Genre { MetaData = meta });
        }

        /// <summary>
        /// Sets a tag to be added to a title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="tag"></param>
        public static void AddTagToTitle(Title title, string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return;

            // see if the tag exists
            Dao.Tag daoTag = Dao.TitleCollectionDao.GetTagByTagName(tag);

            if (daoTag == null)
            {
                daoTag = new OMLEngine.Dao.Tag();
                daoTag.Name = tag;

                // save the genre
                Dao.DBContext.Instance.Tags.InsertOnSubmit(daoTag);
                Dao.DBContext.Instance.SubmitChanges();
            }

            title.DaoTitle.Tags.Add(daoTag);
        }
    }

    public class FilteredCollection : IComparable
    {
        public string Name { get; set; }
        public int Count { get; set; }       

        public int CompareTo(object other)
        {
            FilteredCollection otherT = other as FilteredCollection;
            if (otherT == null)
                return -1;
            return Name.CompareTo(otherT.Name);
        }
    }
}
