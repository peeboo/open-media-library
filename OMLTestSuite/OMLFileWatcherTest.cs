using System;
using System.IO;
using NUnit.Framework;

using OMLFileWatcher;

namespace OMLTestSuite
{

    [TestFixture]
    public class OMLFileWatcherTest
    {
        [Test]
        public void TEST_BASE_CASE()
        {
            OMLFileWatcher.OMLFileWatcher watcher = new OMLFileWatcher.OMLFileWatcher();
            watcher.AddWatch(@"..\..\..\Sample Files\FileWatcherTestData\", @"*.avi", true);
            watcher.Created += new OMLFileWatcher.OMLFileWatcher.eCreated(fsw_Created);
            watcher.EnableRaisingEvents = true;

            FileStream fstm = File.Create(@"..\..\..\Sample Files\FileWatcherTestData\myfile.avi");
            if (fstm.CanWrite)
            {
                fstm.Flush();
                fstm.Close();
            }
            File.Delete(@"..\..\..\Sample Files\FileWatcherTestData\myfile.avi");
        }

        private void fsw_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
