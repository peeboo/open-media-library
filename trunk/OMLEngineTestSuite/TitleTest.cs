using OMLEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.Collections;
using System;

namespace OMLEngineTestSuite
{
    /// <summary>
    ///This is a test class for TitleTest and is intended
    ///to contain all TitleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TitleTest
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
        ///A test for Title Constructor
        ///</summary>
        [TestMethod()]
        public void TitleConstructorTest()
        {
            Title target = new Title();
            Assert.IsTrue(target.Actors != null);
        }

        [TestMethod()]
        public void TestBaseCase()
        {
            Title title = new Title();
            title.sourceId = "001";
            title.Name = "My Title";
            title.back_boxart_path = "c:\\img.jpg";
            title.front_boxart_path = "c:\\img.jpg";
            title.Country_Of_Origin = "US";
            title.DateAdded = new DateTime(2008, 01, 01);
            title.Description = "My Description";
            title.Distributor = "Paramount";
            title.FileLocation = "c:\\video.mkv";
            title.Importer_Source = "DVD";
            title.MPAARating = Rating.R;
            title.Official_Website_Url = "www.mymovie.com";
            title.ReleaseDate = new DateTime(2008, 01, 01);
            title.Runtime = "110";
            title.Synopsis = "my synopsis is here";

            Assert.AreEqual("My Title", title.Name);
            Assert.AreEqual("c:\\img.jpg", title.back_boxart_path);
            Assert.AreEqual("c:\\img.jpg", title.front_boxart_path);
            Assert.AreEqual("US", title.Country_Of_Origin);
            Assert.AreEqual("1/1/2008", title.DateAdded.ToShortDateString());
            Assert.AreEqual("My Description", title.Description);
            Assert.AreEqual("Paramount", title.Distributor);
            Assert.AreEqual("c:\\video.mkv", title.FileLocation);
            Assert.AreEqual("DVD", title.Importer_Source);
            Assert.AreEqual("R", title.MPAARating);
            Assert.AreEqual("www.mymovie.com", title.Official_Website_Url);
            Assert.AreEqual("1/1/2008", title.ReleaseDate.ToShortDateString());
            Assert.AreEqual("110", title.Runtime);
            Assert.AreEqual("my synopsis is here", title.Synopsis);
        }
    }
}
