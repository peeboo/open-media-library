using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.ServiceModel;
using AmazonMetaData2.Amazon.ECS;

using OMLEngine;        // need this for OML Title class
using OMLSDK;           // need this for the IOMLMetadataPlugin

namespace AmazonMetaData2
{
    public class AmazonPlugin : IOMLMetadataPlugin
    {
        AmazonLocale _locale = AmazonLocale.Default;
        Item[] searchResultItems;
        AmazonSearchResult _searchResult = null;
        AWSECommerceServicePortTypeClient client;

        public string PluginName { get { return "AmazonV2"; } }

        public AmazonPlugin() { }

        public override string ToString()
        {
            return PluginName;
        }

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

        public List<string> GetBackDropUrlsForTitle()
        {
            return null;
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

        private void DownloadImage(Title title, string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                string tempFileName = Path.GetTempFileName();
                WebClient web = new WebClient();
                try
                {
                    web.DownloadFile(imageUrl, tempFileName);
                    title.FrontCoverPath = tempFileName;
                }
                catch
                {
                    File.Delete(tempFileName);
                }
            }
        }

        public Title GetTitle(int index)
        {
            if (_searchResult != null & _searchResult.DVDList != null)
            {
                if (index >= 0 && index < _searchResult.DVDList.Length)
                {
                    // Check if image has been downloaded
                    if (string.Compare(_searchResult.DVDList[index].FrontCoverPath.Substring(0, 4), "http", true) == 0)
                    {
                        DownloadImage(_searchResult.DVDList[index], _searchResult.DVDList[index].FrontCoverPath);
                    }

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

        public Title[] GetAvailableTitles()
        {
            if (_searchResult != null)
                return _searchResult.DVDList;
            else
                return null;
        }

        public Title GetBestMatch()
        {
            return GetTitle(0);
        }

        public bool SearchForTVSeries(string SeriesName, string EpisodeName, int? SeriesNo, int? EpisodeNo, int maxResults)
        {
            return false;
        }
        public bool SearchForTVDrillDown(int id, string EpisodeName, int? SeasonNo, int? EpisodeNo, int maxResults)
        {
            return false;
        }
        public bool SearchForMovie(string movieName, int maxResults)
        {
            try
            {
                ItemSearchRequest req = new ItemSearchRequest();
                req.SearchIndex = "DVD";
                req.Title = movieName;
                req.ResponseGroup = new string[] { "Medium", "Subjects" };

                ItemSearch iSearch = new ItemSearch();
                iSearch.Request = new ItemSearchRequest[] { req };
                iSearch.AWSAccessKeyId = Properties.Settings.Default.AWEAccessKeyId;

                Console.WriteLine(iSearch.Validate);
                ItemSearchResponse res = client.ItemSearch(iSearch);
                if (res.Items[0].Item.Length > 0)
                {
                    Item[] amazonItems = res.Items[0].Item;
                    int itemsToProcess = Math.Min(amazonItems.Length, 20);

                    if (amazonItems != null)
                    {
                        // convert Amazon Items to generic collection of DVDs
                        Title[] searchResults = new Title[itemsToProcess];


                        for (int i = 0; i < itemsToProcess; i++)
                        {
                            searchResults[i] = AmazonToOML.TitleFromAmazonItem(amazonItems[i]);
                        }
                        int totalPages = 0;
                        int totalItems = 0;
                        if (res.Items[0].TotalPages != null) totalPages = Convert.ToInt32(res.Items[0].TotalPages);
                        if (res.Items[0].TotalResults != null) totalItems = Convert.ToInt32(res.Items[0].TotalResults);

                        _searchResult = (new AmazonSearchResult(searchResults, totalPages, totalItems));
                    }
                    else
                    {
                        _searchResult = (new AmazonSearchResult(null, 0, 0));
                    }

                    return true;
                }
            }
            catch
            {
                _searchResult = (new AmazonSearchResult(null, 0, 0));
            }

            return false;
        }

        public bool Initialize(string provider, Dictionary<string, string> parameters)
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.MaxReceivedMessageSize = int.MaxValue;

            client = new AWSECommerceServicePortTypeClient(
                binding, new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService")
            );

            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior());

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

    }
}
