using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;
using OMLSDK;

namespace AmazonMetadata
{
    public class AmazonMetadataPlugin : IOMLMetadataPlugin
    {
        // don't initialize it yet since there's a small time penalty
        // to initialize the web service.
        AmazonWebService _amazon = null;
        AmazonLocale _locale = AmazonLocale.Default;
        AmazonSearchResult _searchResult = null;

        public string PluginName
        {
            get { return "Amazon"; }
        }

        // these 2 methods must be called in sequence
        public bool Initialize(Dictionary<string, string> parameters)
        {
            try
            {
                _amazon = new AmazonWebService();
                _locale = AmazonLocale.FromString(Properties.Settings.Default.AmazonLocale);

                if (parameters != null)
                {
                    if (parameters.ContainsKey("Locale"))
                    {
                        string localeParam = parameters["Locale"];
                        _locale = AmazonLocale.FromString(localeParam);
                    }
                }
                return true;

            }
            catch
            {
                _amazon = null;
                return false;
            }
        }

        public bool SearchForMovie(string movieName)
        {
            try
            {
                if (_amazon == null) Initialize(null);
                if (_amazon == null) return false;

                _searchResult = _amazon.SearchDVDs(movieName, 1, _locale);
                if (_searchResult != null && _searchResult.DVDList != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool AdditonalSearchForMovie(string movieName)
        {
            // dont' do any more (we could do all pages)
            return false;
        }

        // these methods are to be called after the 2 methods above

        // get the best match
        public Title GetBestMatch()
        {
            if (_searchResult != null & _searchResult.DVDList != null)
            {
                // for now get the first one but it may not be the best match
                return _searchResult.DVDList[0];
            }
            else
            {
                return null;
            }
        }

        // or choose among all the titles
        public Title[] GetAvailableTitles()
        {
            if (_searchResult != null)
                return _searchResult.DVDList;
            else
                return null;
        }

        public Title GetTitle(int index)
        {
            if (_searchResult != null & _searchResult.DVDList != null)
            {
                if (index >= 0 && index < _searchResult.DVDList.Length)
                {
                    return _searchResult.DVDList[index];
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }

        public List<OMLMetadataOption> GetOptions()
        {
            List<OMLMetadataOption> settings = new List<OMLMetadataOption>();
            Dictionary<string, string> possibleValues = new Dictionary<string, string>();
            possibleValues.Add(AmazonLocale.US.FriendlyName, "Connect to Amazon.com (US)");
            possibleValues.Add(AmazonLocale.Canada.FriendlyName, "Connect to Amazon.ca (Canada)");
            possibleValues.Add(AmazonLocale.UK.FriendlyName, "Connect to Amazon.co.uk (UK)");
            possibleValues.Add(AmazonLocale.France.FriendlyName, "Connect to Amazon.fr (France)");
            possibleValues.Add(AmazonLocale.Germany.FriendlyName, "Connect to Amazon.de (Germany)");
            possibleValues.Add(AmazonLocale.Japan.FriendlyName, "Connect to Amazon.co.jp (Japan)");
            settings.Add(new OMLMetadataOption("Locale", _locale.FriendlyName, possibleValues, true));

            return settings;
        }

        public bool SetOptionValue(string option, string value)
        {
            if (option.Equals("Locale", StringComparison.CurrentCultureIgnoreCase))
            {
                _locale = AmazonLocale.FromString(value.ToUpper());
                Properties.Settings.Default.AmazonLocale = _locale.FriendlyName;
                Properties.Settings.Default.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return PluginName;
        }

        
    }
}
