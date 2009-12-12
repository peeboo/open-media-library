using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DVDProfilerPlugin;
using System.Diagnostics;

using NUnit.Framework;

using OMLEngine.FileSystem;
using OMLEngine.Settings;
using OMLEngine;
using System.Drawing;
using OMLSDK;

namespace OMLTestSuite
{     
    [TestFixture]
    public class TitleCollectionManagerTest : TestBase
    {
        [Test]
        public void TEST_DELETE_ALL_DATA()
        {
            Console.WriteLine("Cleaning the database");
            TitleCollectionManager.DeleteAllTitles();
        }

        [Test]
        public void TEST_IMPORT_INTO_DATABASE()
        {            
            Console.WriteLine("Running dvd profiler importer");
            DVDProfilerImporter importer = new DVDProfilerImporter();
            importer.DoWork(new[] { @"..\..\..\Sample Files\DVDProfiler - Large.xml" });
            IList<OMLSDKTitle> titles = importer.GetTitles();

            Console.WriteLine("Adding items to the db");
            DateTime start = DateTime.Now;
            foreach (OMLSDKTitle title in titles)
            {                
                // save the title - this also generates it an id
                TitleCollectionManager.AddTitle(OMLSDK.SDKUtilities.ConvertOMLSDKTitleToTitle(title));
            }

            // save all the image updates
            TitleCollectionManager.SaveTitleUpdates();

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        titles.Count));
        }

        [Test]
        public void TEST_GETTING_ALL_TITLES()
        {
            List<TitleFilter> filters = new List<TitleFilter>();

            filters.Add(new TitleFilter(TitleFilterType.Genre, "Drama", true));
            filters.Add(new TitleFilter(TitleFilterType.Genre, "Comedy", true));

            List<Title> titles = new List<Title>(TitleCollectionManager.GetFilteredTitles(filters));

            foreach (Title title in titles)
            {
                Console.WriteLine(title.Name);
            }            

            return;


            OMLEngine.Trailers.AppleTrailers trailer = new OMLEngine.Trailers.AppleTrailers(OMLEngine.Trailers.AppleTrailerRes.HiRes);

            foreach (OMLEngine.Trailers.AppleTrailer trail in trailer.AllTrailers)
            {
                Console.WriteLine(trail.Title);
            }

            return;

            Console.WriteLine("Starting to get all titles");
            DateTime start = DateTime.Now;

            IEnumerable<FilteredCollectionWithImages> items = TitleCollectionManager.GetAllGenresWithImages(null);


            //IEnumerable<FilteredTitleCollection> items = TitleCollectionManager.GetAllYearsGrouped(new List<TitleFilter>() { new TitleFilter(TitleFilterType.Genre, "Horror") });

            /*            

            IEnumerable<Title> titles = TitleCollectionManager.GetAllTitles();

            foreach (Title title in titles)
            {
                Console.WriteLine(title.Name + title.Disks.Count + title.FrontCoverMenuPath);
            }*/

            /*foreach (FilteredTitleCollection col in items)
            {
                Console.WriteLine(col.Name);

                foreach (Title title in col.Titles)
                {
                    Console.WriteLine(title.Name);
                }
            }*/

            foreach (FilteredCollectionWithImages col in items)
            {
                Console.WriteLine(col.Name + " " + col.Count);

                int x = 0;

                foreach (int id in col.ImageIds)
                {
                    if (x++ == 6)
                        break;

                    Console.Write(id);
                }
            }

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        items
                                        .Count()));
        }

        [Test]
        public void TEST_GET_ALL_GENRES()
        {
            Console.WriteLine("Starting to get all genres");
            DateTime start = DateTime.Now;

            IEnumerable<FilteredCollection> items = TitleCollectionManager.GetAllGenres(null);

            foreach (FilteredCollection item in items)
                Console.WriteLine(item.Name + " " + item.Count + " " + item.ImagePath);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        items.Count()));
        }

        [Test]
        public void TEST_GET_ALL_PEOPLE()
        {
            Console.WriteLine("Starting to get all people");
            DateTime start = DateTime.Now;

            IEnumerable<FilteredCollection> items = TitleCollectionManager.GetAllPeople(null, PeopleRole.Actor);

            List<FilteredCollection> allItems = new List<FilteredCollection>(items);            

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        allItems.Count));            
        }

        [Test]
        public void TEST_GET_ALL_RUNTIMES()
        {
            Console.WriteLine("Starting to get all runtime values");
            DateTime start = DateTime.Now;

            IEnumerable<FilteredCollection> items = TitleCollectionManager.GetAllRuntimes(null);

            List<FilteredCollection> allItems = new List<FilteredCollection>(items);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        allItems.Count));
            foreach (FilteredCollection item in allItems)
            {
                Console.WriteLine(String.Format("{0}-{1}", item.Name, item.Count));
            }
        }

        [Test]
        public void TEST_GET_ALL_DATEADDED()
        {
            Console.WriteLine("Starting to get all date added values");
            DateTime start = DateTime.Now;

            IEnumerable<FilteredCollection> items = TitleCollectionManager.GetAllDateAdded(null);

            List<FilteredCollection> allItems = new List<FilteredCollection>(items);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        allItems.Count));
            foreach (FilteredCollection item in allItems)
            {
                Console.WriteLine(String.Format("{0}-{1}", item.Name, item.Count));
            }
        }

        [Test]
        public void TEST_GET_ALL_ALPHAINDEX()
        {
            Console.WriteLine("Starting to get all date added values");
            DateTime start = DateTime.Now;

            List<List<Title>> titlesCollection = new List<List<Title>>();

            IEnumerable<FilteredTitleCollection> items = TitleCollectionManager.GetAllAlphaIndex(null);

            List<FilteredTitleCollection> allItems = new List<FilteredTitleCollection>(items);            

            foreach (FilteredTitleCollection item in allItems)
            {
                List<Title> list = new List<Title>();

                foreach (Title title in item.Titles)
                    list.Add(title);

                titlesCollection.Add(list);
            }

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        titlesCollection.Count));
        }       

        [Test]
        public void TEST_GET_ALL_MOVIES_FOR_ACTOR()
        {
            Console.WriteLine("Starting to get all Tim Burton movies");
            DateTime start = DateTime.Now;

            IEnumerable<Title> items = TitleCollectionManager.GetFilteredTitles(TitleFilterType.Person, "Tim Burton");

            foreach (Title title in items)
                Console.WriteLine(title.Name);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        items.Count()));
        }

        [Test]
        public void TEST_GET_ALL_MOVIES_FOR_ACTOR_WITH_GENRE()
        {
            Console.WriteLine("Starting to get all Tim Burton movies that are dramas");
            DateTime start = DateTime.Now;

            List<TitleFilter> filter = new List<TitleFilter>();
            filter.Add(new TitleFilter(TitleFilterType.Person, "Tim Burton"));
            filter.Add(new TitleFilter(TitleFilterType.Genre, "Drama"));

            IEnumerable<Title> items = TitleCollectionManager.GetFilteredTitles(filter);
            
            foreach (Title title in items)
                Console.WriteLine(title.Name);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        items.Count()));
        }

        [Test]
        public void TEST_GET_ALL_GENRES_GIVEN_FILTER()
        {
            Console.WriteLine("Starting to get all Tim Burton movie genres");
            DateTime start = DateTime.Now;

            List<TitleFilter> filter = new List<TitleFilter>();
            filter.Add(new TitleFilter(TitleFilterType.Person, "Tim Burton"));

            IEnumerable<FilteredCollection> items = TitleCollectionManager.GetAllGenres(filter);

            foreach (FilteredCollection item in items)
                Console.WriteLine(item.Name + " " + item.Count);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        items.Count()));
        }

        [Test]
        public void TEST_GET_ACTORS_UNWATCHED()
        {
            Console.WriteLine("Starting to get all unwatched Harrison Ford Movies");
            DateTime start = DateTime.Now;

            List<TitleFilter> filter = new List<TitleFilter>();
            filter.Add(new TitleFilter(TitleFilterType.Person, "Harrison Ford (1942)"));
            filter.Add(new TitleFilter(TitleFilterType.Unwatched, null));

            IEnumerable<Title> items = TitleCollectionManager.GetFilteredTitles(filter);

            foreach (Title item in items)
                Console.WriteLine(item.Name);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        items.Count()));
        }

        [Test]
        public void TEST_SAVE_SETTINGS()
        {
            Console.WriteLine(OMLSettings.MovieView.ToString());

            //OMLSettings.MovieView = MovieView.List;

            Console.WriteLine(OMLSettings.MovieView.ToString());
        }

        [Test]
        public void TEST_DISK_ALREADY_EXISTS()
        {
            Console.WriteLine("Starting to check if disk already exists");

            string existingPath = null;

            foreach (Title title in TitleCollectionManager.GetAllTitles())
            {
                existingPath = title.Disks[0].Path;
                break;
            }
               
            DateTime start = DateTime.Now;

            bool found = false;

            Assert.AreEqual(true, found = TitleCollectionManager.ContainsDisks(existingPath), "Disk path should have been found");

            Console.WriteLine("Result : " + found);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds",
                                        (DateTime.Now - start).TotalMilliseconds.ToString()));

            Console.WriteLine("Starting to check to verify disk is not found");

            Assert.AreEqual(false, found = TitleCollectionManager.ContainsDisks(existingPath + "test"), "Disk path should not have been found");

            Console.WriteLine("Result : " + found);
            
        }

        [Test]
        public void TEST_DELETE_DISK()
        {
            Console.WriteLine("Starting to delete disks");

            Title deleteTitle = null;

            foreach (Title title in TitleCollectionManager.GetAllTitles())
            {
                deleteTitle = title;
                break;
            }

            int titleId = deleteTitle.Id;

            TitleCollectionManager.DeleteTitle(deleteTitle);

            Console.WriteLine("Done deleting " + titleId.ToString());
        }

        public void TEST_GET_NEW_FILES()
        {
            DateTime start = DateTime.Now;

            List<string> directories = new List<string>() { @"\\percy\movies", @"c:\" };

            IEnumerable<string> filePaths = FileScanner.GetAllMediaFromPath(directories);

            IEnumerable<string> newMedia = TitleCollectionManager.GetUniquePaths(filePaths);

            foreach (string media in newMedia)
                Console.WriteLine(media);

            Console.WriteLine("Took: " + (DateTime.Now - start).TotalMilliseconds + " milliseconds");
        }

        public void TEST_GET_NETWORKSHARES()
        {
            DateTime start = DateTime.Now;

            foreach (string share in NetworkScanner.GetAllAvailableNetworkShares())
                Console.WriteLine(share);

            Console.WriteLine("Took: " + (DateTime.Now - start).TotalMilliseconds + " milliseconds");
        }

        public void TEST_UNIQUE_DISKS()
        {
            DateTime start = DateTime.Now;
            List<string> titles = new List<string>();

            titles.Add(@"\\percy\movies\Bully");
            titles.Add(@"\\percy\movies\Fake\1.mpg");
            titles.Add(@"\\percy\movies\Fake\13.mpg");            

            bool contains = TitleCollectionManager.ContainsDisks(@"\\percy\movies\Bully");

            foreach (string disk in TitleCollectionManager.GetUniquePaths(titles))
                Console.WriteLine(disk);

            Console.WriteLine("Took: " + (DateTime.Now - start).TotalMilliseconds + " milliseconds");
        }

        public void GET_ALL_GENRES_WITH_COVERS()
        {
            DateTime start = DateTime.Now;

            IEnumerable<FilteredCollection> allGenres = TitleCollectionManager.GetAllGenres(null);

            foreach (FilteredCollection item in allGenres)
                Console.WriteLine(item.Name + " " + item.ImagePath + " " + item.Count);


            Console.WriteLine("Took: " + (DateTime.Now - start).TotalMilliseconds + " milliseconds");
        }

        public void SETUP_WATCH_FOLER()
        {
            List<string> list = new List<string>();

            list.Add(@"C:\Users\Public\Recorded TV");

            //OMLSettings.ScannerWatchedFolders = list;
        }

        /*public void UPDATE_SETTINGS()
        {
            WatcherSettings settings = WatcherSettingsManager.GetSettings();

            foreach (MetaDataSettings meta in settings.MetaDataPlugins)
            {
                Console.WriteLine(meta.Name);

                foreach (KeyValuePair<string, string> options in meta.Options)
                {
                    Console.WriteLine(options.Key + " " + options.Value);
                }
            }

            List<MetaDataSettings> list = new List<MetaDataSettings>();

            List<KeyValuePair<string, string>> keys = new List<KeyValuePair<string,string>>();
            keys.Add(new KeyValuePair<string,string>("Collection Path", "c:\\Collection.xml"));           

            settings.SetMetaDataPlugins(new MetaDataSettings[] { new MetaDataSettings("DVDProfiler", keys) });
        }*/

        public void TEST_CLEANUP_IMAGES()
        {
            ImageManager.CleanupCachedImages();
        }

        public void TEST_USER_FILTERS()
        {
            UserFilter filter = new UserFilter("Horror Comedies", new TitleFilter[] { new TitleFilter(TitleFilterType.Genre, "Horror"), new TitleFilter(TitleFilterType.Genre, "Comedy") });            
            
            OMLSettings.UserFilters = new UserFilter[] { filter };

            /*foreach (UserFilter filter in OMLSettings.UserFilters)
            {
                foreach (Title title in filter.GetFilteredTitles())
                {
                    Console.WriteLine(title.Name);
                }
            }*/
        }
    }
}
