using OMLEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace OMLEngineTestSuite
{
    
    
    /// <summary>
    ///This is a test class for UtilitiesTest and is intended
    ///to contain all UtilitiesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UtilitiesTest
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
        ///A test for ImportData
        ///</summary>
        [TestMethod()]
        public void ImportDataTest()
        {
            DataSet dataSet = null; // TODO: Initialize to an appropriate value
            DataSet dataSetExpected = null; // TODO: Initialize to an appropriate value
            Utilities.ImportData(ref dataSet);
            Assert.AreEqual(dataSetExpected, dataSet);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for getImporterClassType
        ///</summary>
        [TestMethod()]
        public void getImporterClassTypeTest()
        {
            Type expected = typeof(MoviesXmlImporter);
            Type actual;
            actual = Utilities.getImporterClassType("MoviesXmlImporter");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateImporterTest()
        {
            bool expected = true;
            bool actual = Utilities.ValidateImporter("MoviesXmlImporter");
            Assert.AreEqual(expected, actual);
        }
    }
}
