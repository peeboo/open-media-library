using System;
using System.Collections.Generic;
using Library;
using NUnit.Framework;

namespace OMLTestSuite
{
    [TestFixture]
    public class VirtualDirectoryTest
    {
        [Test]
        public void TEST_CREATE_VIRTUAL_FOLDER()
        {
            VirtualDirectory vdir = new VirtualDirectory(@"..\..\..\Sample Files\TestFolder");

            Assert.AreEqual(2, vdir.TotalChildDirectories);
            Assert.That(!vdir.HasChildFiles);
        }

        [Test]
        public void TEST_MULTIPLE_BASE_FOLDERS_WORK()
        {
            VirtualDirectory vdir;
            vdir = new VirtualDirectory(new string[] {@"..\..\..\Sample Files\TestFolder\TestFolder1",
                                                      @"..\..\..\Sample Files\TestFolder\TestFolder2"});

            Assert.AreEqual(2, vdir.TotalChildDirectories);
        }
    }
}
