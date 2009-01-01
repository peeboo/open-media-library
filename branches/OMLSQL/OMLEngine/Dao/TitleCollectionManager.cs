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
    }

    public class FilteredCollection
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
