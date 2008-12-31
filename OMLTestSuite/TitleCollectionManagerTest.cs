using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DVDProfilerPlugin;

using NUnit.Framework;

using OMLEngine;

namespace OMLTestSuite
{     
    [TestFixture]
    public class TitleCollectionManagerTest : TestBase
    {
        [Test]
        public void TEST_IMPORT_INTO_DATABASE()
        {
            Console.WriteLine("Cleaning the database");
            TitleCollectionManager.DeleteAllTitles();

            Console.WriteLine("Running dvd profiler importer");
            DVDProfilerImporter importer = new DVDProfilerImporter();
            importer.DoWork(new[] { @"..\..\..\Sample Files\DVDProfiler - Large.xml" });
            IList<Title> titles = importer.GetTitles();

            Console.WriteLine("Adding items to the db");
            DateTime start = DateTime.Now;
            foreach (Title title in titles)
                TitleCollectionManager.AddTitle(title);

            Console.WriteLine(string.Format("Done - Took: {0} for {1} titles",
                                        (DateTime.Now - start).TotalMilliseconds.ToString(),
                                        titles.Count));
        }
    }
}
