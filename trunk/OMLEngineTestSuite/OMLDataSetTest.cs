using OMLSDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Collections;

namespace OMLEngineTestSuite
{
    
    
    /// <summary>
    ///This is a test class for OMLDataSetTest and is intended
    ///to contain all OMLDataSetTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OMLDataSetTest
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
        ///A test for OMLDataSet Constructor
        ///</summary>
        [TestMethod()]
        public void OMLDataSetConstructorTest()
        {
            OMLDataSet target = new OMLDataSet();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddRow
        ///</summary>
        [TestMethod()]
        public void AddRowTest()
        {
            OMLDataSet target = new OMLDataSet(); // TODO: Initialize to an appropriate value
            DataRow row = null; // TODO: Initialize to an appropriate value
            target.AddRow(row);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetColumnNames
        ///</summary>
        [TestMethod()]
        public void GetColumnNamesTest()
        {
            OMLDataSet target = new OMLDataSet(); // TODO: Initialize to an appropriate value
            ArrayList expected = null; // TODO: Initialize to an appropriate value
            ArrayList actual;
            actual = target.GetColumnNames();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for NewRow
        ///</summary>
        [TestMethod()]
        public void NewRowTest()
        {
            OMLDataSet target = new OMLDataSet(); // TODO: Initialize to an appropriate value
            DataRow expected = null; // TODO: Initialize to an appropriate value
            DataRow actual;
            actual = target.NewRow();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
