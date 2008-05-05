using MyMoviesPlugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace OMLEngineTestSuite
{
    
    
    /// <summary>
    ///This is a test class for MyMoviesImporterTest and is intended
    ///to contain all MyMoviesImporterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MyMoviesImporterTest
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
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest()
        {
            MyMoviesImporter target = new MyMoviesImporter(); // TODO: Initialize to an appropriate value
            string filename = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
           actual = target.Load("C:\\mymovies.xml");
            OMLEngine.TitleCollection tc = new OMLEngine.TitleCollection();
            foreach (OMLEngine.Title t in target.GetTitles)
                tc.Add(t);

            tc.saveTitleCollection();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetName
        ///</summary>
        [TestMethod()]
        public void GetNameTest()
        {
            MyMoviesImporter target = new MyMoviesImporter(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetName();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CopyImage
        ///</summary>
        [TestMethod()]
        public void CopyImageTest()
        {
            MyMoviesImporter target = new MyMoviesImporter(); // TODO: Initialize to an appropriate value
            string from_location = string.Empty; // TODO: Initialize to an appropriate value
            string to_location = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.CopyImage(from_location, to_location);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MyMoviesImporter Constructor
        ///</summary>
        [TestMethod()]
        public void MyMoviesImporterConstructorTest()
        {
            MyMoviesImporter target = new MyMoviesImporter();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
