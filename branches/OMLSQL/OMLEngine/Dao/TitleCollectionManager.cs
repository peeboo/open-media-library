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
        public static IList<Title> GetAllTitles()
        {
            // get all the titles
            return ConvertDaoTitlesToTitles(Dao.TitleCollectionDao.GetAllTitles());            
        }

        /// <summary>
        /// Returns the titles in the unwatched filter
        /// todo : solomon : this is a just a proof of concept - the final version will need to be
        /// much more robust with multiple levels of filters
        /// </summary>
        /// <returns></returns>
        public static IList<Title> GetUnwatchedTitles()
        {
            // get all the unwatched titles
            return ConvertDaoTitlesToTitles(Dao.TitleCollectionDao.GetUnwatchedTitles());
        }

        /// <summary>
        /// Returns all the titles for a given person
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static IList<Title> GetFilteredTitlesActor(string name)
        {
            return ConvertDaoTitlesToTitles(Dao.TitleCollectionDao.GetFilteredTitlesPerson(name));
        }

        /// <summary>
        /// Gets all the genres and their item count
        /// </summary>
        /// <returns></returns>
        public static IList<FilteredCollection> GetAllGenres()
        {
            IEnumerable<Dao.GenreMetaData> genres = Dao.TitleCollectionDao.GetAllGenres();

            List<FilteredCollection> returnItems = new List<FilteredCollection>(genres.Count());

            foreach (Dao.GenreMetaData genre in genres)
            {
                int count = Dao.TitleCollectionDao.GetItemsPerGenre(genre);
                if (count == 0)
                    continue;

                returnItems.Add(new FilteredCollection() { Name = genre.Name, Count = count });
            }

            return returnItems;
        }

        /// <summary>
        /// Gets all the people and their count of movies
        /// </summary>
        /// <returns></returns>
        public static IList<FilteredCollection> GetAllPeople()
        {
            IEnumerable<Dao.BioData> people = Dao.TitleCollectionDao.GetAllPeople();

            List<FilteredCollection> returnItems = new List<FilteredCollection>(people.Count());

            foreach (Dao.BioData bio in people)
            {
                int count = Dao.TitleCollectionDao.GetItemsPerPerson(bio);
                if (count == 0)
                    continue;

                returnItems.Add(new FilteredCollection() { Name = bio.FullName, Count = count });
            }

            return returnItems;
        }

        /// <summary>
        /// Creates a list of title objects from the DAO title objects
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        private static IList<Title> ConvertDaoTitlesToTitles(IEnumerable<Dao.Title> titles)
        {
            List<Title> returnTitles = new List<Title>(titles.Count());

            foreach (Dao.Title title in titles)
                returnTitles.Add(new Title(title));
                //returnTitles.Add(Dao.Title.CreateOMLEngineTitleFromTitle(title, false));

            return returnTitles;
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
    }

    public class FilteredCollection
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
