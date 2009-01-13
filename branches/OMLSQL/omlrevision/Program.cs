using System;
using System.Collections.Generic;
using System.Xml;
using System.ServiceModel.Syndication;

namespace omlrevision
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlReader reader = XmlReader.Create("http://code.google.com/feeds/p/open-media-library/svnchanges/basic");
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            IEnumerator<SyndicationItem> items = feed.Items.GetEnumerator();
            if (items.Current == null)
                items.MoveNext();

            string revision_string = items.Current.Title.Text;
            revision_string = revision_string.Substring(9, (revision_string.IndexOf(':') - 9));
            Console.Write(revision_string);
        }
    }
}
