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
            // todo : solomon : do your duplicate logic here
            Dao.TitleCollectionDao.AddTitle(OMLEngine.Dao.Title.CreateFromOMLEngineTitle(title));
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
    }
}
