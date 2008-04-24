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
            title.itemId = 1;
            title.Name = "My Title";
            title.back_boxart_path = "c:\\img.jpg";
            title.front_boxart_path = "c:\\img.jpg";
            title.Country_Of_Origin = "US";
            title.DateAdded = new DateTime(2008, 01, 01);
            title.Description = "My Description";
            title.Distributor = "Paramount";
            title.FileLocation = "c:\\video.mkv";
            title.Importer_Source = "DVD";
            title.MPAARating = "R";
            title.Official_Website_Url = "www.mymovie.com";
            title.ReleaseDate = new DateTime(2008, 01, 01);
            title.Runtime = "110";
            title.Synopsis = "my synopsis is here";

            Assert.AreEqual(1, title.itemId);
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

        [TestMethod()]
        public void TestIListMembers()
        {
            Title title = new Title();

            title.AddActor("Translucent");
            Assert.AreEqual(1, title.Actors.Count);
            Assert.AreEqual("Translucent", title.Actors[0]);

            title.AddCrew("Cast Member");
            Assert.AreEqual(1, title.Crew.Count);
            Assert.AreEqual("Cast Member", title.Crew[0]);

            title.AddDirector("cool director");
            Assert.AreEqual(1, title.Directors.Count);
            Assert.AreEqual("cool director", title.Directors[0]);

            title.AddProducer("sony");
            Assert.AreEqual(1, title.Producers.Count);
            Assert.AreEqual("sony", title.Producers[0]);

            title.AddWriter("terry pratchet");
            Assert.AreEqual(1, title.Writers.Count);
            Assert.AreEqual("terry pratchet", title.Writers[0]);

            title.AddGenre(Genre.Comedy);
            Assert.AreEqual(1, title.Genres.Count);
            Assert.AreEqual(Genre.Comedy, title.Genres[0]);
        }
    }
}
