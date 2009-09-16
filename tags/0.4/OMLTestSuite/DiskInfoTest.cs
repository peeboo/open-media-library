using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLTestSuite
{
    class DiskInfoTest : TestBase
    {
        public void TestLookup(string name, string path, OMLEngine.VideoFormat format)
        {
            OMLEngine.DiskInfo di = new OMLEngine.DiskInfo(name, path, format);
            Console.ReadKey();
        }
    } 
}
