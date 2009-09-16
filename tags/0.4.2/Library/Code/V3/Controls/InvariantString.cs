using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Globalization;
using System.Collections.Specialized;

namespace Library.Code.V3
{
    internal static class InvariantString
    {
        // Methods
        public static IDictionary CreateDictionary()
        {
            return new Hashtable(StringComparer.Ordinal);
        }

        public static IDictionary CreateDictionaryI()
        {
            return new Hashtable(StringComparer.OrdinalIgnoreCase);
        }

        public static IDictionary CreateListDictionary()
        {
            return new ListDictionary(StringComparer.Ordinal);
        }

        public static bool EndsWith(string stValue, string stSuffix)
        {
            return stValue.EndsWith(stSuffix, false, CultureInfo.InvariantCulture);
        }

        public static bool EndsWithI(string stValue, string stSuffix)
        {
            return stValue.EndsWith(stSuffix, true, CultureInfo.InvariantCulture);
        }

        public static bool Equals(string stLeft, string stRight)
        {
            return (string.Compare(stLeft, stRight, false, CultureInfo.InvariantCulture) == 0);
        }

        public static bool EqualsI(string stLeft, string stRight)
        {
            return (string.Compare(stLeft, stRight, true, CultureInfo.InvariantCulture) == 0);
        }

        public static string Format(string stFormat, params object[] arArgs)
        {
            string text = stFormat;
            if (arArgs != null)
            {
                text = string.Format(CultureInfo.InvariantCulture, stFormat, arArgs);
            }
            return text;
        }

        public static int LastIndexOf(string stValue, string stSearch)
        {
            return stValue.LastIndexOf(stSearch, StringComparison.Ordinal);
        }

        public static bool StartsWith(string stValue, string stPrefix)
        {
            return stValue.StartsWith(stPrefix, false, CultureInfo.InvariantCulture);
        }

        public static bool StartsWithI(string stValue, string stPrefix)
        {
            return stValue.StartsWith(stPrefix, true, CultureInfo.InvariantCulture);
        }

        public static string ValueToString(ushort v, string stFormat)
        {
            return v.ToString(stFormat, CultureInfo.InvariantCulture);
        }
    }
}
