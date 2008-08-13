using System;
using System.Xml.Xsl;

namespace OMLEngine
{
    public class XMLTransformer
    {
        public static bool QuickTransform(string source, string stylesheet, string output)
        {
            try
            {
                XslTransform xslt = new XslTransform();
                xslt.Load(stylesheet);
                xslt.Transform(source, output);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
