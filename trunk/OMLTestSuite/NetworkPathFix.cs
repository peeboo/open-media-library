using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace OMLTestSuite
{
    [TestFixture]

    public class NetworkPathFix : TestBase
    {
        public void Test()
        {
            string t;
            CheckPath(@"C:\Media\MediaLocal\GameOn\105 - Matthew - A suitable case for treatment.avi");
            CheckPath(@"C:\Media\MediaShare\GameOn\105 - Matthew - A suitable case for treatment.avi");
            CheckPath(@"\\10.20.0.220\d\Media\My TV\Blakes 7 - 4x13 - Blake.avi");
            CheckPath(@"Z:\Media\My TV\Dr Who 2005\107 - The long game.avi");
        }

        private void CheckPath(string t)
        {            
            Console.WriteLine(t);
            Console.WriteLine(OMLEngine.FileSystem.NetworkScanner.FixPath(t));
            if (File.Exists(OMLEngine.FileSystem.NetworkScanner.FixPath(t)))
            {
                Console.WriteLine("OK!");
            }
            else
            {
                Console.WriteLine("Not Found");
            }
            Console.WriteLine("");
        }
    }
}
