using OMLSDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace OMLEngineTestSuite
{
    
    
    /// <summary>
    ///This is a test class for OMLPluginTest and is intended
    ///to contain all OMLPluginTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OMLPluginTest
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
        ///A test for ValidateRow
        ///</summary>
        /*
        [TestMethod()]
        public void ValidateRowTest()
        {
            OMLDataSet ds = null; // TODO: Initialize to an appropriate value
            DataRow row = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = OMLPlugin.ValidateRow(ds, row);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OMLPlugin Constructor
        ///</summary>
        [TestMethod()]
        public void OMLPluginConstructorTest()
        {
            OMLPlugin target = new OMLPlugin();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
         */
    }
}
