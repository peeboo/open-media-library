using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dao = OMLEngine.Dao;

namespace OMLEngine
{
    /*public enum TitleTypes : int
    {
        Movie, // (uses media logic)
        Episode, // (uses media logic)
        Collection, // (uses folder logic)
        TVShow, // (uses folder logic)
        Season // (uses folder logic)
    }*/
    public enum TitleTypes : int
    {
        Root = 0x0001,

        // Folders / containers
        Collection = 0x0002,
        TVShow = 0x0004,
        Season = 0x0008,
        AllFolders = 0x00FE,

        // Media
        Unknown = 0x0100,
        Movie = 0x0200, 
        Episode = 0x0400,
        AllMedia = 0xFF00
    }


    public static class TitleCollectionManager
    {
        /// <summary>
        /// Adds a title to the db
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool AddTitle(Title title)
        {
            using (OMLEngine.Dao.LocalDataContext db = new OMLEngine.Dao.LocalDataContext())
            {                
                // Set default titletype if none specified
                if ((title.TitleType == null) || (title.TitleType == 0)) { title.TitleType = TitleTypes.Root | TitleTypes.Unknown; }

                // setup all the collections objects
                // todo : solomon : this should go away once it's understood how people 
                // will be added
                Dao.TitleDao.SetupCollectionsToBeAdded(db.Context, title);

                // setup all the images
                UpdatesImagesForTitle(title.DaoTitle);

                // reset the % complete
                title.DaoTitle.ResetPercentComplete();

                // todo : solomon : do your duplicate logic here
                Dao.TitleCollectionDao.AddTitle(db.Context, title.DaoTitle);

                return true;
            }                        
        }

        private static void UpdatesImagesForTitle(Dao.Title title)
        {
            // if there's a new front cover image path
            if (title.UpdatedFrontCoverPath != null)
            {                                
                // delete the old one if it exists
                for (int i = title.Images.Count - 1; i >= 0; i--)
                {
                    if (title.Images[i].ImageType == (byte) ImageType.FrontCoverImage)
                    {
                        Dao.TitleCollectionDao.SetDeleteImage(title.Images[i].ImageId);
                        Dao.DBContext.Instance.ImageMappings.DeleteOnSubmit(title.Images[i]);
                        title.Images.RemoveAt(i);
                    }
                } 

                // add the new one
                int? id = ImageManager.AddImageToDB(title.UpdatedFrontCoverPath);

                // if we got an id back let's associate it
                if (id != null)
                {
                    Dao.ImageMapping image = new OMLEngine.Dao.ImageMapping();
                    image.ImageId = id.Value;
                    image.ImageType = (byte)ImageType.FrontCoverImage;                    

                    title.Images.Add(image);
                }

                // clear it out
                title.UpdatedFrontCoverPath = null;
            }

            // if there's a new front cover image path
            if (title.UpdatedBackCoverPath != null)
            {
                // delete the old one if it exists
                for (int i = title.Images.Count - 1; i >= 0; i--)
                {
                    if (title.Images[i].ImageType == (byte)ImageType.BackCoverImage)
                    {
                        Dao.TitleCollectionDao.SetDeleteImage(title.Images[i].ImageId);
                        Dao.DBContext.Instance.ImageMappings.DeleteOnSubmit(title.Images[i]);
                        title.Images.RemoveAt(i);
                    }
                }                

                // add the new one
                int? id = ImageManager.AddImageToDB(title.UpdatedBackCoverPath);

                // if we got an id back let's associate it
                if (id != null)
                {
                    Dao.ImageMapping image = new OMLEngine.Dao.ImageMapping();
                    image.ImageId = id.Value;
                    image.ImageType = (byte)ImageType.BackCoverImage;                    

                    title.Images.Add(image);
                }                

                // clear it out
                title.UpdatedBackCoverPath = null;
            }

            if (title.UpdatedFanArtPaths != null)
            {
                IEnumerable<string> originalCoverArt = from t in title.Images
                                                       where t.ImageType == (byte)ImageType.FanartImage
                                                       select ImageManager.ConstructImagePathById(t.ImageId, ImageSize.Original);

                List<string> added = new List<string>(title.UpdatedFanArtPaths.Where(t => !originalCoverArt.Contains(t)));
                List<string> removed = new List<string>(originalCoverArt.Where(t => !title.UpdatedFanArtPaths.Contains(t)));

                // remove ones no longer used
                foreach (string remove in removed)
                {
                    int? id = ImageManager.GetIdFromImagePath(remove);
                    if (id != null)
                    {
                        Dao.ImageMapping mapping = title.Images.FirstOrDefault(t => t.ImageId == id.Value);

                        if (mapping != null)
                        {
                            Dao.TitleCollectionDao.SetDeleteImage(mapping.DBImage);
                            Dao.DBContext.Instance.ImageMappings.DeleteOnSubmit(mapping);
                            title.Images.Remove(mapping);
                        }
                    }                    
                }                

                // add the new ones
                foreach (string add in added)
                {
                    int? id = ImageManager.AddImageToDB(add);

                    if (id != null)
                    {
                        Dao.ImageMapping mapping = new Dao.ImageMapping();
                        mapping.ImageId = id.Value;
                        mapping.ImageType = (byte)ImageType.FanartImage;

                        title.Images.Add(mapping);
                    }
                }

                title.UpdatedFanArtPaths = null;
            }
        }        

        /// <summary>
        /// Persists any pending updates to the database
        /// </summary>
        /// <returns></returns>
        public static bool SaveTitleUpdates()
        {
            System.Data.Linq.ChangeSet changeset = Dao.DBContext.Instance.GetChangeSet();
            
            // Find actual SQL run to process the update - debugging purposes
            string s = Dao.DBContext.Instance.GetType().GetMethod("GetChangeText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(Dao.DBContext.Instance, null) as string;
            // updates all the pending image changes
            foreach (object title in changeset.Updates)
            {
                Dao.Title daoTitle = title as Dao.Title;

                if (daoTitle != null)
                {
                    UpdatesImagesForTitle(daoTitle);
                    Dao.TitleDao.UpdateCollectionsForTitle(daoTitle);

                    daoTitle.ResetPercentComplete();
                }
            }                         

            // todo : solomon : add error handing and logging here
            Dao.DBContext.Instance.SubmitChanges();            
            return true;
        }

                
        /// <summary>
        /// deletes all the images
        /// </summary>
        public static void DeleteAllImages()
        {
            // delete all the Image mappings
            Dao.DBContext.Instance.ExecuteCommand("delete from ImageMappings");

            // delete all the images
            Dao.DBContext.Instance.ExecuteCommand("delete from DBImages");
        }

        /// <summary>
        /// Deletes all the titles from the db - leaves the genre and people meta data
        /// </summary>
        public static void DeleteAllTitles()
        {
            // delete all the people
            Dao.DBContext.Instance.ExecuteCommand("delete from People");

            // delete all the disks
            Dao.DBContext.Instance.ExecuteCommand("delete from Disks");

            // delete all the genres
            Dao.DBContext.Instance.ExecuteCommand("delete from Genres");

            // delete all the tags
            Dao.DBContext.Instance.ExecuteCommand("delete from Tags");            

            // delete all the Genre Mappings
            Dao.DBContext.Instance.ExecuteCommand("delete from GenreMappings");            

            // delete all the meta data mappings
            Dao.DBContext.Instance.ExecuteCommand("delete from MataDataMappings");

            // delete all the tiles
            Dao.DBContext.Instance.ExecuteCommand("delete from Titles");            
        }

        #region GenreMetaData functions
        /// <summary>
        /// Deletes the genre meta data
        /// </summary>
        public static void DeleteAllGenreMetaData()
        {
            // delete all the meta data mappings
            Dao.DBContext.Instance.ExecuteCommand("delete from GenreMetaData");
        }

        public static IEnumerable<GenreMetaData> GetAllGenreMetaDatas()
        {
            foreach (Dao.GenreMetaData gm in Dao.TitleCollectionDao.GetAllGenreMetaDatas())
            {
                yield return new GenreMetaData(gm);

            }
        }

        public static void AddGenreMetaData(GenreMetaData gm)
        {
            Dao.DBContext.Instance.GenreMetaDatas.InsertOnSubmit(gm.DaoGenreMetaData);
            Dao.DBContext.Instance.SubmitChanges();
        }

        public static void RemoveGenreMetaData(GenreMetaData gm)
        {
            Dao.DBContext.Instance.GenreMetaDatas.DeleteOnSubmit(gm.DaoGenreMetaData);
            Dao.DBContext.Instance.SubmitChanges();
        }

        public static void SaveGenreMetaDataChanges()
        { 
            Dao.DBContext.Instance.SubmitChanges();
        }
        #endregion

        #region BioData functions
        /// <summary>
        /// Deletes the people meta data
        /// </summary>
        public static void DeleteAllPeopleData()
        {
            // delete all the people
            Dao.DBContext.Instance.ExecuteCommand("delete from BioData");            
        }

        public static IEnumerable<BioData> GetAllBioDatas()
        {
            foreach (Dao.BioData bd in Dao.TitleCollectionDao.GetAllBioDatas())
            {
                yield return new BioData(bd);

            }
        }

        public static void AddBioData(BioData bd)
        {
            UpdatesImagesForBioData(bd.DaoBioData);
            Dao.DBContext.Instance.BioDatas.InsertOnSubmit(bd.DaoBioData);
            Dao.DBContext.Instance.SubmitChanges();
        }

        public static void RemoveBioData(BioData bd)
        {
            // delete the image if it exists
            if (bd.DaoBioData.PhotoID != null)
            {
                Dao.TitleCollectionDao.SetDeleteImage((int)bd.DaoBioData.PhotoID);
            }

            Dao.DBContext.Instance.BioDatas.DeleteOnSubmit(bd.DaoBioData);
            Dao.DBContext.Instance.SubmitChanges();
        }

        public static void SaveBioMetaDataChanges()
        {
            System.Data.Linq.ChangeSet changeset = Dao.DBContext.Instance.GetChangeSet();

            // Find actual SQL run to process the update - debugging purposes
            string s = Dao.DBContext.Instance.GetType().GetMethod("GetChangeText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(Dao.DBContext.Instance, null) as string;
            // updates all the pending image changes
            foreach (object bio in changeset.Updates)
            {
                Dao.BioData daoBio = bio as Dao.BioData;

                if (daoBio != null)
                {
                    UpdatesImagesForBioData(daoBio);
                }
            }

            Dao.DBContext.Instance.SubmitChanges();
        }

        private static void UpdatesImagesForBioData(Dao.BioData bd)
        {
            // if there's a new front cover image path
            if (bd.UpdatedImagePath != null)
            {
                // delete the old one if it exists
                if (bd.PhotoID != null)
                {
                    Dao.TitleCollectionDao.SetDeleteImage((int)bd.PhotoID);
                }

                // add the new one
                bd.PhotoID = ImageManager.AddImageToDB(bd.UpdatedImagePath);

                // clear it out
                bd.UpdatedImagePath = null;
            }
        }        
        #endregion

        /// <summary>
        /// Returns a distinct list of all tags allocated to titles
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllTagsList()
        {
            return Dao.TitleCollectionDao.GetAllTagsList();
        }


        /// <summary>
        /// Deletes all the user information from the database
        /// </summary>
        public static void DeleteAllDBData()
        {
            TitleCollectionManager.DeleteAllImages();

            TitleCollectionManager.DeleteAllTitles();
            TitleCollectionManager.DeleteAllPeopleData();
            TitleCollectionManager.DeleteAllGenreMetaData();            

            ImageManager.CleanupCachedImages();
        }


        /// <summary>
        /// Gets all titles with data needed for browsing (this only give root titles ie parenttitleid = id)
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Title> GetAllTitles()
        {
            // get all the titles
            return ConvertDaoTitlesToTitles(Dao.TitleCollectionDao.GetAllTitles());
        }
        public static IEnumerable<Title> GetAllTitles(TitleTypes type)
        {
            // get all the titles
            return ConvertDaoTitlesToTitles(Dao.TitleCollectionDao.GetAllTitles(type));
        }
        public static IEnumerable<Title> GetAllTitles(TitleTypes type, int? parentId)
        {
            // get all the titles
            return ConvertDaoTitlesToTitles(Dao.TitleCollectionDao.GetAllTitles(type, parentId));
        }

        public static IEnumerable<Title> GetTitlesByPercentComplete(decimal completeness)
        {
            IEnumerable<Title> titles = GetAllTitles();
            return from title in titles
                   where title.PercentComplete <= completeness
                   select title;
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
        /// Returns the titles for the given filter
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<Title> GetFilteredTitles(List<TitleFilter> filters, TitleTypes type, int? parentId)
        {
            return (filters == null || filters.Count == 0)
                    ? GetAllTitles(type, parentId)
                    : ConvertDaoTitlesToTitles(Dao.TitleCollectionDao.GetFilteredTitles(filters, type, parentId));
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
        /// Returns the date when titles were added grouped into the logical groups with access to the titles
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredTitleCollection> GetAllDateAddedGrouped(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllDateAddedGrouped(filters);
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

        public static IEnumerable<FilteredTitleCollection> GetAllYearsGrouped(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllYearsGrouped(filters);
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
            return Dao.TitleCollectionDao.GetAllGenres(filters);
        }

        public static IEnumerable<FilteredCollectionWithImages> GetAllGenresWithImages(List<TitleFilter> filters)
        {
            return Dao.TitleCollectionDao.GetAllGenresWithImages(filters);
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
        /// Will return the meta id's that aren't being used already
        /// </summary>
        /// <param name="metaIds"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUniqueMetaIds(IEnumerable<string> metaIds, string metaDataSource)
        {
            return Dao.TitleCollectionDao.GetUniqueMetaIds(metaIds, metaDataSource);
        }

        /// <summary>
        /// Returns all the unused paths for the given collection of paths checked against existing Disks
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUniquePaths(IEnumerable<string> paths)
        {
            return Dao.TitleCollectionDao.GetUniqueMediaPaths(paths);         
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

        public static void RemoveAllPersons(Title title, PeopleRole type)
        {
            var deletePersons = from d in Dao.DBContext.Instance.Persons
                                where d.Role == (byte)type
                                select d;

            foreach (Dao.Person daoperson in deletePersons)
            {
                title.DaoTitle.People.Remove(daoperson);
            }
        }                          

        /// <summary>
        /// Closes any open db connections
        /// </summary>
        public static void CloseDBConnection()
        {
            if ( Dao.DBContext.InstanceOrNull != null  &&
                Dao.DBContext.Instance.Connection != null &&
                Dao.DBContext.Instance.Connection.State != System.Data.ConnectionState.Closed)
            {
                Dao.DBContext.Instance.Connection.Close();
                Dao.DBContext.Instance.Connection.Dispose();
            }
            else if (Dao.DBContext.InstanceOrNull != null &&
                Dao.DBContext.Instance.Connection != null)
            {
                Dao.DBContext.Instance.Dispose();
            }            
        }
    }

    public class FilteredCollectionWithImages : FilteredCollection
    {
        public IEnumerable<int> ImageIds { get; internal set; }
    }

    public class FilteredCollection : IComparable
    {
        private string imagePath = string.Empty;        

        public string Name { get; internal set; }
        public int Count { get; internal set; }
        public int? ImageId { get; internal set; }

        public string ImagePath
        {
            get
            {
                if (imagePath == string.Empty)
                {
                    if (ImageId != null)
                    {
                        imagePath = ImageManager.GetImagePathById(ImageId.Value, ImageSize.Small);
                    }
                }

                return (string.IsNullOrEmpty(imagePath)) ? null : imagePath;
            }
        }

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
