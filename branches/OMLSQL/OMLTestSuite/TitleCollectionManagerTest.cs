using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DVDProfilerPlugin;
using System.Diagnostics;

using NUnit.Framework;

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
            IList<Title> titles = importer.GetTitles();

            Console.WriteLine("Adding items to the db");
            DateTime start = DateTime.Now;
            foreach (Title title in titles)
            {                
                // save the title - this also generates it an id
                TitleCollectionManager.AddTitle(title);

                // update the image
                OMLPlugin.BuildResizedMenuImage(title);
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
            Console.WriteLine("Starting to get all titles");
            DateTime start = DateTime.Now;

            IEnumerable<Title> titles = TitleCollectionManager.GetAllTitles();

            foreach (Title title in titles)
                Console.WriteLine(title.Name);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        titles.Count()));
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
    }
}
