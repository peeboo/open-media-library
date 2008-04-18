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
        ///A test for Writers
        ///</summary>
        [TestMethod()]
        public void WritersTest()
        {
            Title target = new Title();
            target.Name = "My Movie";
            target.itemId = 1;
            Assert.IsInstanceOfType(target, typeof(Title));
        }

        /// <summary>
        ///A test for Summary
        ///</summary>
        [TestMethod()]
        public void SummaryTest()
        {
            Title target = new Title();
            string expected = string.Empty;
            string actual;
            target.Summary = expected;
            actual = target.Summary;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Runtime
        ///</summary>
        [TestMethod()]
        public void RuntimeTest()
        {
            Title target = new Title();
            string expected = string.Empty;
            string actual;
            target.Runtime = expected;
            actual = target.Runtime;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ReleaseDate
        ///</summary>
        [TestMethod()]
        public void ReleaseDateTest()
        {
            Title target = new Title();
            DateTime expected = new DateTime();
            DateTime actual;
            target.ReleaseDate = expected;
            actual = target.ReleaseDate;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Producers
        ///</summary>
        [TestMethod()]
        public void ProducersTest()
        {
            Title target = new Title();
            IList actual;
            actual = target.Producers;
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Count, 0);
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void NameTest()
        {
            Title target = new Title();
            string expected = "My Movie";
            string actual;
            target.Name = expected;
            actual = target.Name;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for MPAARating
        ///</summary>
        [TestMethod()]
        public void MPAARatingTest()
        {
            Title target = new Title();
            string expected = "Rated (R)";
            string actual;
            target.MPAARating = expected;
            actual = target.MPAARating;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for itemId
        ///</summary>
        [TestMethod()]
        public void itemIdTest()
        {
            Title target = new Title();
            int expected = 4;
            int actual;
            target.itemId = expected;
            actual = target.itemId;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for IMDBRating
        ///</summary>
        [TestMethod()]
        public void IMDBRatingTest()
        {
            Title target = new Title();
            string expected = "Rated (PG)";
            string actual;
            target.IMDBRating = expected;
            actual = target.IMDBRating;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Directors
        ///</summary>
        [TestMethod()]
        public void DirectorsTest()
        {
            Title target = new Title();
            IList actual;
            actual = target.Directors;
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Count, 0);
            target.Directors.Add("boo");
            actual = target.Directors;
            Assert.AreEqual(actual.Count, 1);
        }

        /// <summary>
        ///A test for Description
        ///</summary>
        [TestMethod()]
        public void DescriptionTest()
        {
            Title target = new Title();
            string expected = "This is the description of my movie that I like very, very much";
            string actual;
            target.Description = expected;
            actual = target.Description;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Crew
        ///</summary>
        [TestMethod()]
        public void CrewTest()
        {
            Title target = new Title();
            IList actual;
            actual = target.Crew;
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Count, 0);
        }

        /// <summary>
        ///A test for boxart_path
        ///</summary>
        [TestMethod()]
        public void boxart_pathTest()
        {
            Title target = new Title();
            string expected = "c:\\program files\\img.jpg";
            string actual;
            target.boxart_path = expected;
            actual = target.boxart_path;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Actors
        ///</summary>
        [TestMethod()]
        public void ActorsTest()
        {
            Title target = new Title();
            IList actual;
            actual = target.Actors;
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Count, 0);
        }

        /// <summary>
        ///A test for GetObjectData
        ///</summary>
        [TestMethod()]
        public void GetObjectDataTest()
        {
            Title target = new Title(); // TODO: Initialize to an appropriate value
            SerializationInfo info = null; // TODO: Initialize to an appropriate value
            StreamingContext ctxt = new StreamingContext(); // TODO: Initialize to an appropriate value
            target.GetObjectData(info, ctxt);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AddWriter
        ///</summary>
        [TestMethod()]
        public void AddWriterTest()
        {
            Title target = new Title();
            string writer = string.Empty;
            target.AddWriter(writer);
            Assert.IsTrue(target.Writers.Contains(writer));
        }

        /// <summary>
        ///A test for AddProducer
        ///</summary>
        [TestMethod()]
        public void AddProducerTest()
        {
            Title target = new Title();
            string producer = string.Empty;
            target.AddProducer(producer);
            Assert.IsTrue(target.Producers.Contains(producer));
        }

        /// <summary>
        ///A test for AddDirector
        ///</summary>
        [TestMethod()]
        public void AddDirectorTest()
        {
            Title target = new Title();
            string director = string.Empty;
            target.AddDirector(director);
            Assert.IsTrue(target.Directors.Contains(director));
        }

        /// <summary>
        ///A test for AddCrew
        ///</summary>
        [TestMethod()]
        public void AddCrewTest()
        {
            Title target = new Title();
            string crew_member = string.Empty;
            target.AddCrew(crew_member);
            Assert.IsTrue(target.Crew.Contains(crew_member));
        }

        /// <summary>
        ///A test for AddActor
        ///</summary>
        [TestMethod()]
        public void AddActorTest()
        {
            Title target = new Title();
            string actor = string.Empty;
            target.AddActor(actor);
            Assert.IsTrue(target.Actors.Contains(actor));
        }

        /// <summary>
        ///A test for Title Constructor
        ///</summary>
        [TestMethod()]
        public void TitleConstructorTest()
        {
            Title target = new Title();
            Assert.IsTrue(target.Actors != null);
        }
    }
}
