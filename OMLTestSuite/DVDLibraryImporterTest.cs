using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using OMLEngine;
using NUnit.Framework;
using VMCDVDLibraryPlugin;

namespace OMLTestSuite
{
    [TestFixture]
    public class DVDLibraryImporterTest : TestBase
    {
        [Test]
        public void TEST_HONORS_OMLENGINE_SETTING_FOLDER_IS_TITLE()
        {
            OMLEngine.Properties.Settings.Default.FoldersAreTitles = true;
            DVDLibraryImporter importer = new DVDLibraryImporter();
            importer.DoWork(new string[] { @"..\..\..\sample files\vmcdvdlibraryimporterfiles\" });

            IList<Title> titles = importer.GetTitles();
        }
    }
}