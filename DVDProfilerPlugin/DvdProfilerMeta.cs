using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using OMLEngine;        // need this for OML Title class
using OMLEngine.FileSystem;
using OMLSDK;           // need this for the IOMLMetadataPlugin
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DVDProfilerPlugin
{
    public class DvdProfilerMeta : IOMLMetadataPlugin
    {
        private string collectionPath;
        private Dictionary<string, string> dvdsByName;
        private Dictionary<string, string> dvdsById;
        private Dictionary<string, string> dvdsBySortName;
        XPathDocument document;
        private Title foundTitle = null;

        public void DownloadBackDropsForTitle(Title t, int index)
        {
        }

        public string PluginName { get { return "DVDProfiler (DVDProfilerPlugin)"; } }

        public List<MetaDataPluginDescriptor> GetProviders
        {
            get
            {
                List<MetaDataPluginDescriptor> descriptors = new List<MetaDataPluginDescriptor>();

                MetaDataPluginDescriptor descriptor = new MetaDataPluginDescriptor();
                descriptor.DataProviderName = PluginName;
                descriptor.DataProviderMessage = "";
                descriptor.DataProviderLink = "";
                descriptor.DataProviderCapabilities = MetadataPluginCapabilities.SupportsMovieSearch;
                descriptor.PluginDLL = null;
                descriptors.Add(descriptor);
                return descriptors;
            }
        }


        // these 2 methods must be called in sequence
        public bool Initialize(string provider, Dictionary<string, string> parameters)
        {
            dvdsByName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            dvdsById = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            dvdsBySortName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(collectionPath) && File.Exists(collectionPath))
            {
                using (XmlTextReader reader = new XmlTextReader(collectionPath))
                {
                    document = new XPathDocument(reader);

                    var navigator = document.CreateNavigator();

                    foreach (XPathNavigator dvd in navigator.Select("/Collection/DVD|/DVD")) // Allow both collection file and single profile export
                    {
                        string name = null;
                        string id = null;
                        string sortName = null;

                        foreach (XPathNavigator dvdElement in dvd.SelectChildren(XPathNodeType.Element))
                        {                          
                            if (dvdElement.Name == "ID")
                            {
                                id = dvdElement.Value;
                            }
                            else if (dvdElement.Name == "Title")
                            {
                                name = dvdElement.Value;
                            }
                            else if (dvdElement.Name == "SortTitle")
                            {
                                sortName = dvdElement.Value;
                            }

                            if (name != null && id != null && sortName != null)
                            {
                                name = CleanString(FileHelper.GetFolderNameString(name));

                                if (dvdsByName.ContainsKey(name))
                                {
                                    name += " " + Guid.NewGuid().ToString();
                                }

                                dvdsByName.Add(name, id);

                                sortName = CleanString(FileHelper.GetFolderNameString(sortName));

                                if (dvdsBySortName.ContainsKey(sortName))
                                {
                                    sortName += " " + Guid.NewGuid().ToString();
                                }

                                dvdsBySortName.Add(sortName, id);

                                if (!dvdsById.ContainsKey(id))
                                {
                                    dvdsById.Add(id, name);
                                }

                                break;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Destroys all non-alpha characters for making title comparison easier
        /// </summary>
        /// <param name="dirtyString"></param>
        /// <returns></returns>
        private string CleanString(string dirtyString)
        {
            StringBuilder sb = new StringBuilder(dirtyString);
            foreach( char c in dirtyString)
            {
                int num = (int)c;

                if ( num < 48 ||
                    ( num > 57 && num < 65 ) ||
                    ( num > 90 && num < 97 )||
                    ( num > 122 ))
                {
                    sb.Replace(c.ToString(), string.Empty);
                }
            }

            return sb.ToString();
        }

        public bool SearchForMovie(string movieName, int maxResults)
        {
            movieName = CleanString(movieName);

            foundTitle = null;
            List<string> possibleIds = new List<string>();

            if (dvdsByName.ContainsKey(movieName))
            {
                possibleIds.Add(dvdsByName[movieName]);
            }
            else if ( dvdsBySortName.ContainsKey(movieName))
            {
                possibleIds.Add(dvdsBySortName[movieName]);
            }
            else
            {
                foreach (string key in dvdsByName.Keys)
                {
                    if (key.StartsWith(movieName, StringComparison.OrdinalIgnoreCase))
                    {
                        possibleIds.Add(dvdsByName[key]);
                    }
                }
            }

            // try the sort names if none of the other names panned out
            if (possibleIds.Count == 0)
            {
                foreach (string key in dvdsBySortName.Keys)
                {
                    if (key.StartsWith(movieName, StringComparison.OrdinalIgnoreCase))
                    {
                        possibleIds.Add(dvdsBySortName[key]);
                    }
                }
            }

            IEnumerable<string> unusedPossibleIds = TitleCollectionManager.GetUniqueMetaIds(possibleIds, this.PluginName);

            foreach (string possibleId in unusedPossibleIds)
            {               
                var navigator = document.CreateNavigator();

                foreach (XPathNavigator dvd in navigator.Select("/Collection/DVD|/DVD")) // Allow both collection file and single profile export
                {                    
                    foreach (XPathNavigator child in dvd.SelectChildren(XPathNodeType.Element))
                    {
                        if (child.Name == "ID")
                        {
                            if (child.Value == possibleId)
                            {
                                child.MoveToParent();

                                DVDProfilerImporter importer = new DVDProfilerImporter();
                                foundTitle = importer.LoadTitle(child, false);
                                importer.GetImagesForNewTitle(foundTitle);
                            }
                        }

                        if (foundTitle != null)
                            break;
                    }

                    if (foundTitle != null)
                        break;
                }

                // we only care about the first one for now
                break;
            }                      

            return foundTitle != null;
        }        

        // these methods are to be called after the 2 methods above

        // get the best match
        public Title GetBestMatch()
        {
            return foundTitle;            
        }

        // or choose among all the titles
        public Title[] GetAvailableTitles()
        {
            throw new NotImplementedException();
        }

        public Title GetTitle(int index)
        {
            throw new NotImplementedException();
        }

        public List<OMLMetadataOption> GetOptions()
        {
            return null;
        }

        public bool SetOptionValue(string option, string value)
        {
            if (string.IsNullOrEmpty(option) || string.IsNullOrEmpty(value))
                return false;

            if (option.Equals("Collection Path", StringComparison.OrdinalIgnoreCase))
            {
                collectionPath = value;                
            }

            return true;
        }

        public bool SearchForTVSeries(string SeriesName, string EpisodeName, int? SeriesNo, int? EpisodeNo)
        {
            return false;
        }
        public bool SearchForTVDrillDown(int id)
        {
            return false;
        }
    }
}
