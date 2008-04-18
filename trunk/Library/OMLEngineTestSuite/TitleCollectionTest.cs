using OMLEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System;
using System.Collections;

namespace OMLEngineTestSuite
{
    /// <summary>
    ///This is a test class for TitleCollectionTest and is intended
    ///to contain all TitleCollectionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TitleCollectionTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SyncRoot
        ///</summary>
        [TestMethod()]
        public void SyncRootTest()
        {
            TitleCollection target = new TitleCollection();
            object actual;
            actual = target.SyncRoot;
            Assert.IsInstanceOfType(actual, typeof(TitleCollection));
        }

        /// <summary>
        ///A test for NeedSetup
        ///</summary>
        [TestMethod()]
        public void NeedSetupTest()
        {
            TitleCollection target = new TitleCollection();
            bool expected = false;
            bool actual;
            target.NeedSetup = expected;
            actual = target.NeedSetup;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IsReadOnly
        ///</summary>
        [TestMethod()]
        public void IsReadOnlyTest()
        {
            TitleCollection target = new TitleCollection();
            bool expected = false;
            bool actual;
            target.IsReadOnly = expected;
            actual = target.IsReadOnly;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Count
        ///</summary>
        [TestMethod()]
        public void CountTest()
        {
            TitleCollection target = new TitleCollection();
            Assert.AreEqual(target.Count, 0);
            target.Add(new Title());
            Assert.AreEqual(target.Count, 1);
        }

        /// <test_serialization_functionality>
        /// A test to ensure that the saving and loading is working correctly
        /// </test_serialization_functionality>
        [TestMethod()]
        public void SaveAndLoadTest()
        {
            TitleCollection target = new TitleCollection("c:\\oml.dat");
            Title title1 = new Title();
            title1.Name = "My Movie";
            title1.ReleaseDate = new DateTime(2008, 01, 01);
            title1.Runtime = "110";
            title1.front_boxart_path = "c:\\2001ASpaceOdyssey1968237_f.jpg";
            title1.Description = "my desc";
            title1.itemId = 1;
            title1.MPAARating = "Rated R";
            title1.Synopsis = "this is a cool synopsis";
            target.Add(title1);

            target.saveTitleCollection();

            TitleCollection target2 = new TitleCollection("c:\\oml.dat");
            target2.loadTitleCollection();
            Assert.AreEqual(target2.Count, 1);

            foreach (Title mytitle in target2)
            {
                Assert.IsInstanceOfType(mytitle, typeof(Title));
                Assert.AreEqual("My Movie", mytitle.Name);
                Assert.AreEqual("110", mytitle.Runtime);
                Assert.AreEqual(new DateTime(2008, 01, 01).ToShortDateString(),
                                mytitle.ReleaseDate.ToShortDateString());
                Assert.AreEqual("c:\\2001ASpaceOdyssey1968237_f.jpg", mytitle.front_boxart_path);
            }
        }

        /// <summary>
        ///A test for Remove
        ///</summary>
        [TestMethod()]
        public void RemoveTest()
        {
            TitleCollection target = new TitleCollection();
            Title title = null;
            bool expected = false;
            bool actual;
            actual = target.Remove(title);
            Assert.AreEqual(expected, actual);

            Title myTitle = new Title();
            target.Add(myTitle);
            Assert.AreEqual(target.Count, 1);
            actual = target.Remove(myTitle);
            Assert.AreEqual(true, actual);
            Assert.AreEqual(target.Count, 0);
        }
        /// <summary>
        ///A test for Contains
        ///</summary>
        [TestMethod()]
        public void ContainsTest()
        {
            TitleCollection target = new TitleCollection();
            Title title = null;
            bool expected = false;
            bool actual;
            actual = target.Contains(title);
            Assert.AreEqual(expected, actual);
            title = new Title();
            target.Add(title);
            Assert.IsTrue(target.Contains(title));
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [TestMethod()]
        public void AddTest()
        {
            TitleCollection target = new TitleCollection();
            Title title = new Title();
            target.Add(title);
            Assert.AreEqual(true, target.Contains(title));
        }
    }
}
