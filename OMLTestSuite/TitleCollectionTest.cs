﻿using System;
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

            Title t = tc.find_for_id(t1.InternalItemID);
            Assert.AreEqual(t1, t);
        }

        [Test]
        public void TEST_TO_DATASET_TABLE_OBJECT()
        {
            TitleCollection tc = new TitleCollection();
            Title t1 = new Title();
            tc.Add(t1);

            DataTable dt = tc.ToDataTable();

            Assert.IsInstanceOfType(typeof(DataTable), dt);
            Assert.AreEqual(dt.Rows.Count, 1);
            Assert.AreEqual(dt.Columns.Count, 28);
        }

        [Test]
        public void TEST_REPLACE_METHOD()
        {
            TitleCollection tc = new TitleCollection("\\test.dat");
            Title t = new Title();
            t.Name = "Boo";
            tc.Add(t);
            tc.saveTitleCollection();

            TitleCollection tc2 = new TitleCollection("\\test.dat");
            tc2.loadTitleCollection();

            Assert.AreEqual(1, tc2.Count);

            Title t2 = (Title)tc2[0];

            Assert.IsInstanceOfType(typeof(Title), t2);

            t2.Runtime = 4;

            tc2.Replace(t2);

            Assert.AreEqual(4, ((Title)tc2[0]).Runtime);
        }
    }
}
