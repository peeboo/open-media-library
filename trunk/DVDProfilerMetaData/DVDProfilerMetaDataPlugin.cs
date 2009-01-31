using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OMLEngine;        // need this for OML Title class
using OMLSDK;
using System.Data;           // need this for the IOMLMetadataPlugin

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
namespace DVDProfilerMetaData
{
    public class DVDProfilerMetaDataPlugin : IOMLMetadataPlugin
    {
        #region IOMLMetadataPlugin Members

        String _xmlFile;
        String _imgPath;
        DVDProfilerSearchResult _searchResult = null;

        public string PluginName
        {
            get { return "DVDProfiler"; }
        }

        public bool Initialize(Dictionary<string, string> parameters)
        {
            _xmlFile = Properties.Settings.Default.xmlFile;
            _imgPath = Properties.Settings.Default.imgPath;
            if (parameters != null)
            {
                if (parameters.ContainsKey("xmlFile"))
                {
                    _xmlFile = parameters["xmlFile"];
                }
            }
            return true;
        }

        public bool SearchForMovie(string movieName)
        {
            if (_xmlFile == null) Initialize(null);
            if (_xmlFile == null) return false;

            if (!File.Exists(_xmlFile)) return false;
            DataSet ds = new DataSet();
            ds.ReadXml(_xmlFile);
            List<DataRow> dvds = (from dvd in ds.Tables["DVD"].AsEnumerable()
                                  where dvd.Field<String>("Title").Contains(movieName)
                                  select dvd).ToList<DataRow>();

            List<Title> dvdList = new List<Title>();
            foreach (DataRow dr in dvds)
            {
                Title t = new Title();
                t.Name = (String)dr["Title"];
                t.SortName = (String)dr["SortTitle"];
                t.Synopsis = (String)dr["Overview"];
                t.FrontCoverPath = Path.Combine(_imgPath, (String)dr["ID"] + @"f.jpg");
                t.BackCoverPath = Path.Combine(_imgPath, (String)dr["ID"] + @"b.jpg");
                String prodYr = (String)dr["ProductionYear"];
                t.ReleaseDate = new DateTime(int.Parse(prodYr), 1, 1);
                DataRow genres = dr.GetChildRows("DVD_Genres")[0];
                foreach (DataRow gen in genres.GetChildRows("Genres_Genre"))
                {
                    String genre = (String)gen[0];
                    t.Genres.Add(genre);
                }
                dvdList.Add(t);
            }
            _searchResult = new DVDProfilerSearchResult(dvdList, dvds.Count, dvds.Count);
            bool rslt = (dvds.Count > 0);
            ds.Dispose(); ds = null;
            return rslt;
        }

        public Title GetBestMatch()
        {
            if (_searchResult != null & _searchResult.Count > 0)
            {
                // for now get the first one but it may not be the best match
                return _searchResult.DVDList[0];
            }
            else
            {
                return null;
            }
        }

        public Title[] GetAvailableTitles()
        {
            return _searchResult.DVDList.ToArray();
        }

        public Title GetTitle(int index)
        {
            return _searchResult.DVDList[index];
        }

        public List<OMLMetadataOption> GetOptions()
        {
            List<OMLMetadataOption> settings = new List<OMLMetadataOption>();
            settings.Add(new OMLMetadataOption("xmlFile", _xmlFile, null, false));
            settings.Add(new OMLMetadataOption("imgPath", _imgPath, null, false));

            return settings;

        }

        public bool SetOptionValue(string option, string value)
        {
            if (option.Equals("xmlFile", StringComparison.CurrentCultureIgnoreCase))
            {
                _xmlFile = value;
                Properties.Settings.Default.xmlFile = _xmlFile;
                Properties.Settings.Default.Save();
                return true;
            }
            else
            {
                if (option.Equals("imgPath", StringComparison.CurrentCultureIgnoreCase))
                {
                    _imgPath = value;
                    Properties.Settings.Default.imgPath = _imgPath;
                    Properties.Settings.Default.Save();
                    return true;
                }
                else
                {
                return false;
                }
            }
        }

        public bool SupportsBackDrops()
        {
            return false;
        }

        public void DownloadBackDropsForTitle(Title t, int index)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DVDProfilerSearchResult
    {
        // constructor
        public DVDProfilerSearchResult(List<Title> dvdList, int totalPages, int totalItems)
        {
            m_DVDList = dvdList;
            m_TotalPages = totalPages;
            m_TotalItems = totalItems;
        }

        public int Count { get { return m_DVDList.Count(); } }
        // public properties
        public List<Title> DVDList { get { return m_DVDList; } }
        public int TotalPages { get { return m_TotalPages; } }
        public int TotalItems { get { return m_TotalItems; } }

        // private data
        private int m_TotalItems;
        private int m_TotalPages;
        List<Title> m_DVDList;
    }

}
