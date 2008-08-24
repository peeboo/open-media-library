using System;
using System.Collections.Generic;
using OMLEngine;
using NUnit.Framework;
using DVDProfilerPlugin;

namespace OMLTestSuite
{
    [TestFixture]
    public class DVDProfilerTest
    {
        [Test]
        public void TEST_SPACING_IS_CORRECT_FOR_ACTOR_NAMES()
        {
            DVDProfilerImporter importer = new DVDProfilerImporter();
            importer.DoWork(new string[] { @"..\..\..\Sample Files\DVDProfiler-SampleBymuskie.xml" });

            IList<Title> titles = importer.GetTitles();
            Assert.AreEqual(4, titles.Count);


            Dictionary<string, string> kvp = titles[0].ActingRoles;
            Assert.IsNotNull(kvp["Arnold Schwarzenegger"]);
        }

        [Test]
        public void TEST_SYNOPSIS_IS_CORRECTLY_SET_FROM_OVERVIEW()
        {
            DVDProfilerImporter importer = new DVDProfilerImporter();
            importer.DoWork(new string[] { @"..\..\..\Sample Files\dvd_profiler.xml" });

            IList<Title> titles = importer.GetTitles();
            Assert.AreEqual(373, titles.Count);

            Assert.IsNotNull((titles[0]).Synopsis);
        }
    }
}