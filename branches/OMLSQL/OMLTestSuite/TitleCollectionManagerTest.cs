using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DVDProfilerPlugin;

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

            IEnumerable<FilteredCollection> items = TitleCollectionManager.GetAllGenres();

            foreach (FilteredCollection item in items)
                Console.WriteLine(item.Name + " " + item.Count);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        items.Count()));
        }

        [Test]
        public void TEST_GET_ALL_PEOPLE()
        {
            Console.WriteLine("Starting to get all people");
            DateTime start = DateTime.Now;

            IEnumerable<FilteredCollection> items = TitleCollectionManager.GetAllPeople();

            foreach (FilteredCollection item in items)
                Console.WriteLine(item.Name + " " + item.Count);

            Console.WriteLine(string.Format("Done - Took: {0} milliseconds for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        items.Count()));            
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

        public void TEST_GET_ALL_MOVIES_FOR_ACTOR_WITH_GENRE()
        {
            Console.WriteLine("Starting to get all Tim Burton movies");
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
    }
}
