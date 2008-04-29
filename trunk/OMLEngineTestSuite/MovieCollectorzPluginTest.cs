using MovieCollectorz;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OMLSDK;

namespace OMLEngineTestSuite
{
    
    
    /// <summary>
    ///This is a test class for Class1Test and is intended
    ///to contain all Class1Test Unit Tests
    ///</summary>
    [TestClass()]
    public class MovieCollectorzPluginTest
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
        ///A test for GetName
        ///</summary>
        [TestMethod()]
        public void GetNameTest()
        {
            MovieCollectorzPlugin target = new MovieCollectorzPlugin();
            string expected = @"MovieCollectorzPlugin";
            string actual;
            actual = target.GetName();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetOmlDataSet
        ///</summary>
        [TestMethod()]
        public void GetOmlDataSetTest()
        {
            MovieCollectorzPlugin target = new MovieCollectorzPlugin();
            target.Load("..\\..\\..\\Sample Files\\Movie_Collectorz.xml");
            Assert.AreEqual(target.TotalRowsAdded, 9);
        }
    }
}
