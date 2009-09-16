using System;
using System.Data;
using OMLEngine;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class TitleCollectionTest
    {
        [Test]
        public void TEST_BASE_CASE()
        {
            TitleCollection tc = new TitleCollection();
            Title title1 = new Title();
            title1.Name = "B Movie";
            Title title2 = new Title();
            title2.Name = "A Movie";

            tc.Add(title1);
            tc.Add(title2);

            Assert.AreEqual(2, tc.Count);

            tc.Sort();
            Assert.AreEqual("A Movie", ((Title)tc[0]).Name);
        }

        [Test]
        public void TEST_SOURCE_DATABASE_TO_USE()
        {
            TitleCollection tc = new TitleCollection();
            tc.SourceDatabaseToUse = SourceDatabase.OML;

            Assert.AreEqual(SourceDatabase.OML, tc.SourceDatabaseToUse);
        }

        [Test]
        public void TEST_FIND_FOR_ID()
        {
            TitleCollection tc = new TitleCollection();
            Title t1 = new Title();
            Title t2 = new Title();
            Title t3 = new Title();
            Title t4 = new Title();

            tc.Add(t1);
            tc.Add(t2);
            tc.Add(t3);
            tc.Add(t4);

            Title t = tc.GetTitleById(t1.InternalItemID);
            Assert.AreEqual(t1, t);
        }
    }
}
