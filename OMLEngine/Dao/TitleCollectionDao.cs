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

        public static IEnumerable<string> GetUniqueMetaIds(IEnumerable<string> metaDataIds, string metaDataSource)
        {
            var ids = from t in DBContext.Instance.Titles
                      where t.MetaDataSourceItemId != null && t.MetaDataSource.ToLower() == metaDataSource.ToLower()
                      select t.MetaDataSourceItemId;

            return metaDataIds.Except(ids);
        }

        public static int AddImage(byte[] imageStream)
        {
            DBImage dbImage = new OMLEngine.Dao.DBImage();
            dbImage.Image = imageStream;

            DBContext.Instance.DBImages.InsertOnSubmit(dbImage);
            DBContext.Instance.SubmitChanges();

            return dbImage.Id;
        }

        /// <summary>
        /// Returns all the media paths that aren't already being used in the given collection
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUniqueMediaPaths(IEnumerable<string> paths)
        {
            var allPaths = from d in DBContext.Instance.Disks
                           select d.Path;

            return paths.Except(allPaths, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns all the titles but doesn't return the actors/directors/writers
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Title> GetAllTitles()
        {
            var titles = from t in DBContext.Instance.Titles
                         orderby t.SortName
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
                         orderby t.SortName
                         select t;

            return titles;
        }

        /// <summary>
        /// Wrapper for returning the IQueryable object instead of the enumerator
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IQueryable<Title> GetFilteredTitlesWrapper(List<TitleFilter> filters)
        {
            return (IQueryable<Title>) GetFilteredTitles(filters);
        }

        /// <summary>
        /// Gets all the titles given the list of filters
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<Title> GetFilteredTitles(List<TitleFilter> filters)
        {
            var titles = DBContext.Instance.Titles;

            if (filters == null || filters.Count == 0)
                return titles;

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

                    case TitleFilterType.DateAdded:
                        results = ApplyDateAddedFilter(lastQuery, filter.FilterText);
                        break;
                }
            }

            // sort the results
            return from t in results
                   orderby t.SortName
                   select t;
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
        /// Applys the date added filter
        /// </summary>
        /// <param name="titles"></param>
        /// <param name="dateAdded"></param>
        /// <returns></returns>
        private static IQueryable<Title> ApplyDateAddedFilter(IQueryable<Title> titles, string dateAdded)
        {
            int days = TitleConfig.DateAddedFilterStringToInt(dateAdded);

            if (days == 0)
            {
                return from t in titles
                       where t.DateAdded != null && ((TimeSpan) (DateTime.Now - t.DateAdded)).Days <= days
                       select t;
            }
            else if (days == -1)
            {
                return from t in titles
                       where t.DateAdded != null && ((TimeSpan)(DateTime.Now - t.DateAdded)).Days > TitleConfig.MAX_DATE_ADDED
                       select t;
            }
            else
            {
                int minDate = 0;

                // the min time is the previous index
                for (int x = 0; x < TitleConfig.ADDED_FILTER_DATE.Length; x++)
                {
                    if (TitleConfig.ADDED_FILTER_DATE[x] == days)
                    {
                        minDate = TitleConfig.ADDED_FILTER_DATE[x - 1];
                        break;
                    }
                }

                return from t in titles
                       where t.DateAdded != null && ((TimeSpan)(DateTime.Now - t.DateAdded)).Days <= days && ((TimeSpan)(DateTime.Now - t.DateAdded)).Days > minDate
                       select t;
            }
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
                   where (startCharacter != "#" && t.Name.StartsWith(startCharacter)) || (startCharacter == "#" && t.SortName.ToUpper()[0] < 'A')
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

        // <summary>
        /// Returns all the people in the given titles
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllGenres(List<TitleFilter> filters)
        {
            var filteredTitles = from t in GetFilteredTitlesWrapper(filters)
                                                from g in t.Genres
                                                join b in DBContext.Instance.GenreMetaDatas on g.GenreMetaDataId equals b.Id
                                                join c in DBContext.Instance.ImageMappings on t.Id equals c.TitleId                                                 
                                                    where c.ImageType == ImageType.FrontCoverImage
                                                select new { GenreName = b.Name, Path = c.ImageId };

            return from t in filteredTitles
                   group t by t.GenreName into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = g.Key, Count = g.Count(), ImageId = 
                       (from title in filteredTitles
                        where title.GenreName == g.Key
                        select title.Path).First() };
        }       
        
        /// <summary>
        /// Returns all the people in the given titles
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllPersons(List<TitleFilter> filters, PeopleRole role)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   from p in t.People
                   where p.Role == (byte)role
                   join b in DBContext.Instance.BioDatas on p.BioId equals b.Id
                   group b by b.FullName into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = g.Key, Count = g.Count() } ;           
        }

        /// <summary>
        /// Returns all the valid parental ratings
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllParentalRatings(List<TitleFilter> filters)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   where t.ParentalRating != null && t.ParentalRating.Length != 0
                   group t by t.ParentalRating into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = g.Key, Count = g.Count() };
        }

        /// <summary>
        /// Returns all the valid countries for the collection and their counts
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllCountries(List<TitleFilter> filters)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   where t.CountryOfOrigin != null && t.CountryOfOrigin.Length != 0
                   group t by t.CountryOfOrigin into g                   
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = g.Key, Count = g.Count() };
        }

        /// <summary>
        /// Returns all the user ratings and their counts
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllUserRatings(List<TitleFilter> filters)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   where t.UserRating != null && t.UserRating != 0
                   group t by t.UserRating into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = UserRatingToString(g.Key), Count = g.Count() };
        }

        /// <summary>
        /// Converts a user rating to it's string
        /// </summary>
        /// <param name="byteUserRating"></param>
        /// <returns></returns>
        private static string UserRatingToString(byte? userRating)
        {
            if (userRating == null)
                userRating = 0;

            return ((double)userRating.Value / 10).ToString("0.0");
        }

        public static IEnumerable<FilteredCollection> GetAllStudios(List<TitleFilter> filters)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   where t.Studio != null
                   group t by t.Studio into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = g.Key.ToString(), Count = g.Count() };
        }

        public static IEnumerable<FilteredCollection> GetAllAspectRatios(List<TitleFilter> filters)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   where t.AspectRatio != null
                   group t by t.AspectRatio into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = g.Key.ToString(), Count = g.Count() };
        }

        public static IEnumerable<FilteredCollection> GetAllCountryOfOrigin(List<TitleFilter> filters)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   where t.CountryOfOrigin != null
                   group t by t.CountryOfOrigin into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = g.Key.ToString(), Count = g.Count() };
        }

        /// <summary>
        /// Returns all the movie release years and their counts
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllYears(List<TitleFilter> filters)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   where t.ReleaseDate != null
                   group t by t.ReleaseDate.Value.Year into g
                   orderby g.Key descending
                   select new FilteredCollection() { Name = g.Key.ToString(), Count = g.Count() };
        }        

        /// <summary>
        /// Returns all the valid video formats and their counts
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllVideoFormats(List<TitleFilter> filters)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   from d in t.Disks
                   group d by d.VideoFormat into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = GetVideoStringFromByte(g.Key), Count = g.Count() };
        }

        /// <summary>
        /// Returns all the tags for the given filter and their counts
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IEnumerable<FilteredCollection> GetAllTags(List<TitleFilter> filters)
        {
            return from t in GetFilteredTitlesWrapper(filters)
                   from ta in t.Tags
                   group ta by ta.Name into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = g.Key, Count = g.Count() };
        }

        public static IEnumerable<FilteredCollection> GetAllDateAdded(List<TitleFilter> filters)
        {
            IEnumerable<int> days = from t in GetFilteredTitlesWrapper(filters)
                       where t.DateAdded.HasValue
                       select (int)(DateTime.Now - t.DateAdded.Value).TotalDays;

            return from d in days
                   from r in TitleConfig.DATE_ADDED_RANGE
                   where d >= r.Start && d < r.End
                   group r by r.End into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = TitleConfig.DaysToFilterString(g.Key), Count = g.Count() };
        }

        public static IEnumerable<FilteredCollection> GetAllRuntimes(List<TitleFilter> filters)
        {
            IEnumerable<short> runtimes = from t in GetFilteredTitlesWrapper(filters)
                                        where t.Runtime.HasValue
                                        select t.Runtime.Value;

            return from d in runtimes
                   from r in TitleConfig.RUNTIME_RANGE
                   where d >= r.Start && d < r.End
                   group r by r.End into g
                   orderby g.Key ascending
                   select new FilteredCollection() { Name = TitleConfig.RuntimeToFilterString(g.Key), Count = g.Count() };
        }

        public static IEnumerable<FilteredTitleCollection> GetAllAlphaIndex(List<TitleFilter> filters)
        {
            IEnumerable<char> firstLetters = from t in GetFilteredTitlesWrapper(filters)
                                             select GetProperIndex(t.SortName.ToUpper()[0]);

            return from l in firstLetters
                   group l by l into g
                   orderby g.Key ascending
                   select new FilteredTitleCollection() { Name = g.Key.ToString(), Titles = TitleCollectionManager.ConvertDaoTitlesToTitles(from t in ApplyAlphaFilter(GetFilteredTitlesWrapper(filters), g.Key.ToString()) select t) };
        }

        private static char GetProperIndex(char letter)
        {
            if (letter < 'A')
                return '#';
            else
                return letter;
        }

        /// <summary>
        /// Creating a method for this since doing it inline makes SQL try to do it which fails
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetVideoStringFromByte(byte key)
        {
            return ((VideoFormat)key).ToString();
        }

        public static DBImage GetImageBydId(int id)
        {
            return Dao.DBContext.Instance.DBImages.SingleOrDefault(i => i.Id == id);
        }

        public static void SetDeleteImage(int imageId)
        {
            SetDeleteImage(GetImageBydId(imageId));
        }

        public static void SetDeleteImage(DBImage image)
        {            
            if (image != null)
            {
                // delete the image from cache
                ImageManager.DeleteCachedImage(image.Id);

                // set it to be deleted in SQL
                DBContext.Instance.DBImages.DeleteOnSubmit(image);
            }
        }

        /// <summary>
        /// Deletes the given title
        /// </summary>
        /// <param name="title"></param>
        public static void DeleteTitle(Title title)
        {
            // delete all the images            
            foreach (Dao.ImageMapping mapping in title.Images)
            {
                SetDeleteImage(mapping.DBImage);

                DBContext.Instance.ImageMappings.DeleteOnSubmit(mapping);
            }

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
