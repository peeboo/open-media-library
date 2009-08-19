using System;
using System.Collections.Generic;

namespace AmazonMetaData2
{
    class AmazonLocale
    {
        private string m_URL = "";
        private string m_FriendlyName = "";

        public string FriendlyName
        {
            get { return m_FriendlyName; }
        }

        public string URL
        {
            get { return m_URL; }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        private AmazonLocale()
        {
            m_URL = "";
        }

        private AmazonLocale(string friendlyName, string url)
        {
            m_FriendlyName = friendlyName;
            m_URL = url;
        }

        public static AmazonLocale US = new AmazonLocale("US", Properties.Settings.Default.AmazonUrlEN);
        public static AmazonLocale UK = new AmazonLocale("UK", Properties.Settings.Default.AmazonUrlUK);
        public static AmazonLocale Canada = new AmazonLocale("CANADA", Properties.Settings.Default.AmazonUrlCA);
        public static AmazonLocale Germany = new AmazonLocale("GERMANY", Properties.Settings.Default.AmazonUrlDE);
        public static AmazonLocale Japan = new AmazonLocale("JAPAN", Properties.Settings.Default.AmazonUrlJP);
        public static AmazonLocale France = new AmazonLocale("FRANCE", Properties.Settings.Default.AmazonUrlFR);
        public static AmazonLocale Default = AmazonLocale.FromString(Properties.Settings.Default.AmazonLocale);

        public static AmazonLocale FromString(string locale)
        {
            switch (locale.ToUpper())
            {
                case "UK":
                    return AmazonLocale.UK;
                case "US":
                    return AmazonLocale.US;
                case "GERMANY":
                    return AmazonLocale.Germany;
                case "JAPAN":
                    return AmazonLocale.Japan;
                case "FRANCE":
                    return AmazonLocale.France;
                case "CANADA":
                    return AmazonLocale.Canada;
                default:
                    return AmazonLocale.US;
            }
        }
    }
}
