using OMLEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Collections;

namespace OMLEngineTestSuite
{
    
    
    /// <summary>
    ///This is a test class for MoviesXmlImporterTest and is intended
    ///to contain all MoviesXmlImporterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MoviesXmlImporterTest
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
        ///A test for StripRating
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OMLEngine.dll")]
        public void StripRatingTest()
        {
            MoviesXmlImporter_Accessor target = new MoviesXmlImporter_Accessor(); // TODO: Initialize to an appropriate value
            string inputString = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.StripRating(inputString);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for StripHTML
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OMLEngine.dll")]
        public void StripHTMLTest()
        {
            MoviesXmlImporter_Accessor target = new MoviesXmlImporter_Accessor(); // TODO: Initialize to an appropriate value
            string inputString = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.StripHTML(inputString);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OMLEngine.dll")]
        public void getUrlTest()
        {
            MoviesXmlImporter_Accessor target = new MoviesXmlImporter_Accessor(); // TODO: Initialize to an appropriate value
            DataRow dataRow = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.getUrl(dataRow);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSingleDataRow
        ///</summary>
        [TestMethod()]
        public void GetSingleDataRowTest()
        {
            DataTable table = null; // TODO: Initialize to an appropriate value
            string column = string.Empty; // TODO: Initialize to an appropriate value
            object value = null; // TODO: Initialize to an appropriate value
            DataRow expected = null; // TODO: Initialize to an appropriate value
            DataRow actual;
            actual = MoviesXmlImporter.GetSingleDataRow(table, column, value);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetMovieData
        ///</summary>
        [TestMethod()]
        public void GetMovieDataTest()
        {
            MoviesXmlImporter target = new MoviesXmlImporter(); // TODO: Initialize to an appropriate value
            int itemId = 0; // TODO: Initialize to an appropriate value
            DataRow expected = null; // TODO: Initialize to an appropriate value
            DataRow actual;
            actual = target.GetMovieData(itemId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getDataSet
        ///</summary>
        [TestMethod()]
        public void getDataSetTest()
        {
            MoviesXmlImporter target = new MoviesXmlImporter(); // TODO: Initialize to an appropriate value
            DataSet expected = null; // TODO: Initialize to an appropriate value
            DataSet actual;
            actual = target.getDataSet();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getCrew
        ///</summary>
        [TestMethod()]
        public void getCrewTest1()
        {
            MoviesXmlImporter target = new MoviesXmlImporter(); // TODO: Initialize to an appropriate value
            IList expected = null; // TODO: Initialize to an appropriate value
            IList actual;
            actual = target.getCrew();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getCrew
        ///</summary>
        [TestMethod()]
        public void getCrewTest()
        {
            MoviesXmlImporter target = new MoviesXmlImporter(); // TODO: Initialize to an appropriate value
            DataRow dataRow = null; // TODO: Initialize to an appropriate value
            string something = string.Empty; // TODO: Initialize to an appropriate value
            IList expected = null; // TODO: Initialize to an appropriate value
            IList actual;
            actual = target.getCrew(dataRow, something);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getChildColumn
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OMLEngine.dll")]
        public void getChildColumnTest()
        {
            MoviesXmlImporter_Accessor target = new MoviesXmlImporter_Accessor(); // TODO: Initialize to an appropriate value
            DataRow dataRow = null; // TODO: Initialize to an appropriate value
            string relatedTable = string.Empty; // TODO: Initialize to an appropriate value
            string column = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.getChildColumn(dataRow, relatedTable, column);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for getActors
        ///</summary>
        [TestMethod()]
        public void getActorsTest()
        {
            MoviesXmlImporter target = new MoviesXmlImporter(); // TODO: Initialize to an appropriate value
            DataRow dataRow = null; // TODO: Initialize to an appropriate value
            IList expected = null; // TODO: Initialize to an appropriate value
            IList actual;
            actual = target.getActors(dataRow);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Finalize
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OMLEngine.dll")]
        public void FinalizeTest()
        {
            MoviesXmlImporter_Accessor target = new MoviesXmlImporter_Accessor(); // TODO: Initialize to an appropriate value
            target.Finalize();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ExtractMetadata
        ///</summary>
        [TestMethod()]
        [DeploymentItem("OMLEngine.dll")]
        public void ExtractMetadataTest()
        {
            MoviesXmlImporter_Accessor target = new MoviesXmlImporter_Accessor(); // TODO: Initialize to an appropriate value
            DataRow movieData = null; // TODO: Initialize to an appropriate value
            int movieId = 0; // TODO: Initialize to an appropriate value
            MovieMetadata expected = null; // TODO: Initialize to an appropriate value
            MovieMetadata actual;
            actual = target.ExtractMetadata(movieData, movieId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MoviesXmlImporter Constructor
        ///</summary>
        [TestMethod()]
        public void MoviesXmlImporterConstructorTest()
        {
            MoviesXmlImporter target = new MoviesXmlImporter();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
