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

        public static IEnumerable<FilteredCollection> GetAllStudios(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllStudios(filters);
        }

        public static IEnumerable<FilteredCollection> GetAllAspectRatios(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllAspectRatios(filters);
        }

        public static IEnumerable<FilteredCollection> GetAllCountryOfOrigin(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllCountryOfOrigin(filters);
        }

        /// <summary>
        /// Returns all the valid parental ratings and their count
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllParentalRatings(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllParentalRatings(filters);            
        }

        /// <summary>
        /// Returns all the valid video formats and their counts
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllVideoFormats(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllVideoFormats(filters);            
        }

        /// <summary>
        /// Returns the date when titles were added and their counts
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllDateAdded(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllDateAdded(filters);
        }

        /// <summary>
        /// Returns all the valid runtimes for the collection and their counts
        /// 
        /// todo : solomon : runtimes doesn't use the fancy LINQ way because of the grouping
        /// although i bet some smarty can figure it out
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllRuntimes(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllRuntimes(filters);
        }

        public static IEnumerable<FilteredTitleCollection> GetAllAlphaIndex(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllAlphaIndex(filters);
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
        /// Returns the all the valid years for the given filter and their counts
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllYears(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllYears(filters);                
        }

        /// <summary>
        /// Gets all the user rated titles and their counts
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllUserRatings(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllUserRatings(filters);                
        }

        /// <summary>
        /// Returns all the countries and counts for the given filters
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllCountries(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllCountries(filters);            
        }

        /// <summary>
        /// Returns all the tags and their counts for a given filter
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllTags(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllTags(filters);            
        }               
        
        /// <summary>
        /// Returns all the genres and counts given a filter      
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllGenres(List<TitleFilter> filters)
        {
            foreach (FilteredCollection col in Dao.TitleCollectionDao.GetAllGenres(filters))
            {
                // todo : solomon : this way kinda sucks because it's a sql query per item returned
                // there's probably a better way
                col.ImagePath = (from t in Dao.TitleCollectionDao.GetFilteredTitlesWrapper(filters)
                                 from g in t.Genres
                                 where g.MetaData.Name == col.Name
                                 select t).First().FrontCoverMenuPath;

                yield return col;
            }
        }                

        /// <summary>
        /// Gets all the people and their count of movies
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllPeople(List<TitleFilter> filter, PeopleRole role)
        {
            return Dao.TitleCollectionDao.GetAllPersons(filter, role);            
        }

        /// <summary>
        /// Creates a list of title objects from the DAO title objects
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        internal static IEnumerable<Title> ConvertDaoTitlesToTitles(IEnumerable<Dao.Title> titles)
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
            Dao.Title title = Dao.TitleCollectionDao.GetTitleById(titleId);
            if (title != null)
                return new Title(title);

            return null;
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

        public static void AddPersonToTitle(Title title, string name, PeopleRole type)
        {
            if (string.IsNullOrEmpty(name))
                return;

            Dao.BioData bioData = Dao.TitleCollectionDao.GetPersonBioDataByName(name);

            if (bioData == null)
            {
                bioData = new OMLEngine.Dao.BioData();
                bioData.FullName = name;
                Dao.DBContext.Instance.BioDatas.InsertOnSubmit(bioData);
            }

            Dao.Person person = new OMLEngine.Dao.Person();
            person.MetaData = bioData;
            person.Role = (byte)type;
            title.DaoTitle.People.Add(person);
        }

        public static void AddActorToTitle(Title title, string actor, string role)
        {
            AddActorToTitle(title, actor, role, PeopleRole.Actor);
        }

        public static void AddActorToTitle(Title title, string actor, string role, PeopleRole type)
        {
            if (string.IsNullOrEmpty(actor))
                return;

            Dao.BioData bioData = Dao.TitleCollectionDao.GetPersonBioDataByName(actor);

            if (bioData == null)
            {
                bioData = new OMLEngine.Dao.BioData();
                bioData.FullName = actor;
            }

            Dao.Person person = new OMLEngine.Dao.Person();
            person.BioId = bioData.Id;
            person.CharacterName = role;
            person.Role = (byte)type;
            title.DaoTitle.People.Add(person);
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

        /// <summary>
        /// Closes any open db connections
        /// </summary>
        public static void CloseDBConnection()
        {
            Dao.DBContext.Instance.Connection.Close();
        }
    }

    public class FilteredCollection : IComparable
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public string ImagePath { get; set; }

        public int CompareTo(object other)
        {
            FilteredCollection otherT = other as FilteredCollection;
            if (otherT == null)
                return -1;
            return Name.CompareTo(otherT.Name);
        }
    }

    public class FilteredTitleCollection : IComparable
    {
        public string Name { get; set; }
        public IEnumerable<Title> Titles { get; set; }

        public int CompareTo(object other)
        {
            FilteredTitleCollection otherT = other as FilteredTitleCollection;
            if (otherT == null)
                return -1;
            return Name.CompareTo(otherT.Name);
        }
    }

    public enum PeopleRole : byte
    {
        Actor = 0,
        Director = 1,
        Writer = 2,
        Producers = 3,
        NonActing = 4 // this is a placeholder - we should flush this out to include all film roles
    }
}
