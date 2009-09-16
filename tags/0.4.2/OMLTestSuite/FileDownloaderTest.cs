using System;
using System.Collections.Generic;
using FileDownloader;
using System.IO;
using NUnit.Framework;

namespace OMLTestSuite {
    [TestFixture]
    public class FileDownloaderTest : TestBase {

        [Test]
        public void TestDownload() {
            DownloadEngine downloader = new DownloadEngine(@"http://open-media-library.googlecode.com/files/Open_Media_Library_User_Manual%280.3b%29.pdf");
            downloader.Log += new DownloadEngine.DownloadEventsHandler(downloader_Log);
            if (downloader.Download()) {
                Assert.IsTrue(File.Exists(downloader.DownloadedFile));
                FileInfo fi = new FileInfo(downloader.DownloadedFile);
                Assert.AreEqual(fi.Length, downloader.TotalBytes);
            }
        }

        void downloader_Log(string status) {
            Assert.IsNotEmpty(status);
            Console.WriteLine("GOT: " + status);
        }
    }
}
