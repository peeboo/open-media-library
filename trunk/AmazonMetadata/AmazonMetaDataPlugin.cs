
// Sample Metadata plugin - uses AMazon web service

using System;
using System.Collections.Generic;
using System.Text;
using OMLEngine;        // need this for OML Title class
using OMLSDK;           // need this for the IOMLMetadataPlugin

// Step 1: Make sure that you add OMLEngine and OMLSDK to your References
// Step 2: Make sure you add "using OMLENgine;" and "using OMSDK;" to your plugin code
// Step 3: Create a class that impelments interface IOMLMetadataPlugin
// Step 4: Implement the following methods from IOMLMetadatapluing:
//  Step 5: Implement PluginName property. This name is used in Menus
//  Step 6: Impelment Initialize method. You can do any initialization your plugin may need. If you don't need anything just return true;
//  Step 7: Implement SearchForMovies. OML will pass you the movie name, your code goes and searches for possible matches.
//          Use private data to keep whatever data you need around. Return true if the search was successful. You can keep an array or list of Title
//          objects to be retrieved later by OML.
//  Step 8: Impelment GetBestMatch. NOrmally returns the first Title on the list that was found using SearchForMovies.
//  Step 9: Implement GetAvailableTitles. Return an array of Title objects that were found in SearchMovies. This is to give the user
//          a chance to select the right movie. Note that you don't have to fill in all the fields in the Title object. For exapmle IMDB
//          may return only the name and possibly the year when responding to the search, while Amazon usually retrieves everything.
//  Step 10:  Implement GetTitle. OML asks for a Title at the index in the array from Step 9. At this point you may need to go back
//            to the metadata provider and dig deeper to get the full details of the movie. In IMDB, another HTTP request
//            has to be done (or possibly more, depending how much data you want). In Amazon it's easier, since the search gets the full
//            details for all the movies it finds.
//  Step 11: Implement GetOptions. Return null or an empty list if you plugin doesn't have any user settable options. The Amazon plugin
//           has one option for the locale (website to use, UK, Canada, etc). If you have options, you need to pass all the possible values to it.
//  Step 12: Implement SetOptionValue. Return true or false if you don't have options.
//  The options can be set in DBEditor.
// Step 13: compile and make sure you copy the plugin to the OML plugins directory.
namespace AmazonMetadata
{
    public class AmazonMetadataPlugin : IOMLMetadataPlugin
    {
        // Amazon specific data
        // use whatever data you need for your search engine
        AmazonWebService _amazon = null;
        AmazonLocale _locale = AmazonLocale.Default;
        AmazonSearchResult _searchResult = null;

        public string PluginName { get { return "Amazon"; } }

        public List<MetaDataPluginDescriptor> GetProviders
        {
            get
            {
                List<MetaDataPluginDescriptor> descriptors = new List<MetaDataPluginDescriptor>();

                MetaDataPluginDescriptor descriptor = new MetaDataPluginDescriptor();
                descriptor.DataProviderName = PluginName;
                descriptor.DataProviderMessage = "Data provided by Amazon";
                descriptor.DataProviderLink = "http://www.amazon.com";
                descriptor.DataProviderCapabilities = MetadataPluginCapabilities.SupportsMovieSearch;
                descriptor.PluginDLL = null;
                descriptors.Add(descriptor);
                return descriptors;
            }
        }


        // Must call this before any other plugin method
        // parameters is optional -use it if you want to get parameters passed in
        // here we can accept the Locale parameter but we'll use the default if not
        public bool Initialize(string provider, Dictionary<string, string> parameters)
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

        // this is the main method used to actually search for movies
        // returns true if it succeeded
        // Note: save the results in your private variables, we'll ask later for the found items
        public bool SearchForMovie(string movieName, int maxResults)
        {
            try
            {
                if (_amazon == null) Initialize(PluginName, null);
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

       
        // get the best match after doing a SearchFOrMovies
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

        // or choose among all the titles after doing a SearchFOrMovies
        public Title[] GetAvailableTitles()
        {
            if (_searchResult != null)
                return _searchResult.DVDList;
            else
                return null;
        }

        // gets a Title based on the index in GetAvailableTitles() above
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

        // if your plugin has any options than can be set send them here
        // otherwise return null
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

        // sets an option. if you don't handle this just ignore it
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

        public void DownloadBackDropsForTitle(Title t, int index)
        {
        }

        public bool SearchForTVSeries(string SeriesName, string EpisodeName, int? SeriesNo, int? EpisodeNo)
        {
            return false;
        }
        public bool SearchForTVDrillDown(int id, string EpisodeName, int? SeasonNo, int? EpisodeNo)
        {
            return false;
        }
    }
}
