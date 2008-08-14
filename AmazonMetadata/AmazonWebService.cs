using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using AmazonMetadata.com.amazon.webservices;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using OMLEngine;

namespace AmazonMetadata
{
        public class AmazonSearchResult
        {
            // constructor
            public AmazonSearchResult(Title[] dvdList, int totalPages, int totalItems)
            {
                m_DVDList = dvdList;
                m_TotalPages = totalPages;
                m_TotalItems = totalItems;
            }

            // public properties
            public Title[] DVDList { get { return m_DVDList; } }
            public int TotalPages { get { return m_TotalPages; } }
            public int TotalItems { get { return m_TotalItems; } }

            // private data
            private int m_TotalItems;
            private int m_TotalPages;
            Title[] m_DVDList;
        }

        public class AmazonLocale
        {
            private string m_URL;

            public string URL
            {
                get { return m_URL; }
            }

            private AmazonLocale()
            {
                m_URL = "";
            }
            
            private AmazonLocale( string url )
            {
                m_URL = url;
            }

            public static AmazonLocale US = new AmazonLocale(Properties.Settings.Default.AmazonUrlEN);
            public static AmazonLocale UK = new AmazonLocale(Properties.Settings.Default.AmazonUrlUK);
            public static AmazonLocale Canada = new AmazonLocale(Properties.Settings.Default.AmazonUrlCA);
            public static AmazonLocale Germany = new AmazonLocale(Properties.Settings.Default.AmazonUrlDE);
            public static AmazonLocale Japan = new AmazonLocale(Properties.Settings.Default.AmazonUrlJP);
            public static AmazonLocale France = new AmazonLocale(Properties.Settings.Default.AmazonUrlFR);
            public static AmazonLocale Default = AmazonLocale.US;

            public static AmazonLocale FromString(string locale)
            {
                switch (locale.ToUpper())
                {
                    case "EN":
                        return AmazonLocale.US;
                    case "UK":
                        return AmazonLocale.UK;
                    case "DE":
                        return AmazonLocale.Germany;
                    case "JP":
                        return AmazonLocale.Japan;
                    case "FR":
                        return AmazonLocale.France;
                    case "CA":
                        return AmazonLocale.Canada;
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
                        return AmazonLocale.Default;
                }
            }
        }

        /// <summary>
        /// Provides simple search functionality using Amazon.com Web Services v4.0
        /// </summary>
        /// <remarks>
        /// Wrapper over the AmazonWebService. See http://www.amazon.com/aws/
        /// for more information about the service and SDK.  
        /// </remarks>
        public class AmazonWebService
        {
            //Web service proxy object
            private AWSECommerceService amazonService = new AmazonMetadata.com.amazon.webservices.AWSECommerceService();


            /// <summary>
            /// Search for DVDs by Keywords
            /// </summary>
            /// <param name="searchString">dvd keyword to search for, e.g. name, actor, or UPC</param>
            /// <param name="pageNumber">page number to show, starts at 1</param>
            /// <remarks>Several search criterial values can be altered in the Settings designer</remarks>
            public AmazonSearchResult SearchDVDs(string searchString,  int pageNumber, AmazonLocale locale)
            {
                string searchType = "Title";
                try
                {
                    // objects needed to define search and search criteria
                    ItemSearchRequest itemSearchRequest = new ItemSearchRequest();
                    ItemSearch itemSearch = new ItemSearch();

                    ItemSearchRequest itemPageRequest = new ItemSearchRequest();

                    //initialize objects
                    itemSearchRequest.Keywords = searchString;

                    //set the size of the response, e.g. "Medium"
                    itemSearchRequest.ResponseGroup = new string[] { "Medium", "Subjects" };
                    if (searchType == "Title")
                        itemSearchRequest.Title = searchString;
                    else if (searchType == "Actor")
                        itemSearchRequest.Actor = searchString;
                    else if (searchType == "Director")
                        itemSearchRequest.Director = searchString;
                    else if (searchType == "Keywords")
                        itemSearchRequest.Keywords = searchString;

                    itemSearchRequest.ItemPage = Convert.ToString(pageNumber);
                    itemSearchRequest.BrowseNode = "130";

                    // set the SearchIndex or search mode, e.g. "DVD"
                    itemSearchRequest.SearchIndex = Properties.Settings.Default.AmazonSearchMode;
                    itemSearch.SubscriptionId = Properties.Settings.Default.AmazonSubscriptionId;
                    itemSearch.Request = new ItemSearchRequest[] { itemSearchRequest };

                    // objects to store the response of the Web service
                    ItemSearchResponse amazonResponse = null;
                    Item[] amazonItems = null;

                    // bind the Web service proxy to the appropriate service end-point URL for the current locale
                    this.amazonService.Url = locale.URL;

                    // call the Web service and assign the response
                    amazonResponse = this.amazonService.ItemSearch(itemSearch);

                    // access the array of returned items is the response is not Nothing
                    if (amazonResponse != null)
                    {
                        amazonItems = amazonResponse.Items[0].Item;
                    }


                    if (amazonItems != null)
                    {
                        // convert Amazon Items to generic collection of DVDs
                        Title[] searchResults = new Title[amazonItems.Length];


                        for (int i = 0; i < amazonItems.Length; i++)
                        {
                            searchResults[i] = AmazonToOML.TitleFromAmazonItem(amazonItems[i]);
                        }
                        int totalPages = 0;
                        int totalItems = 0;
                        if (amazonResponse.Items[0].TotalPages != null) totalPages = Convert.ToInt32(amazonResponse.Items[0].TotalPages);
                        if (amazonResponse.Items[0].TotalResults != null) totalItems = Convert.ToInt32(amazonResponse.Items[0].TotalResults);

                        return (new AmazonSearchResult(searchResults, totalPages, totalItems));
                    }
                    else
                    {
                        return (new AmazonSearchResult(null, 0, 0));
                    }
                }
                catch (Exception ex)
                {
                    return (new AmazonSearchResult(null, 0, 0));
                }
            }
        }

        public class AmazonToOML
        {
            public static Title TitleFromAmazonItem(Item amazonItem)
            {
                try
                {
                    Title t = new Title();

                    t.Name = amazonItem.ItemAttributes.Title;
                    if (!String.IsNullOrEmpty(amazonItem.ASIN.ToString())) t.UPC = amazonItem.ASIN.ToString();
                    t.Synopsis = AmazonItemDescriptionToString(amazonItem);

                    if (amazonItem.ItemAttributes.Actor != null)
                    {
                        foreach (string actor in amazonItem.ItemAttributes.Actor)
                        {
                            t.AddActingRole(actor, "");
                        }
                    }

                    if (amazonItem.ItemAttributes.Director != null)
                    {
                        foreach (string director in amazonItem.ItemAttributes.Director)
                        {
                            t.AddDirector(new Person(director));
                        }
                    }

                    if( amazonItem.ItemAttributes.AudienceRating != null )
                        t.ParentalRating = AmazonRatingToString(amazonItem.ItemAttributes.AudienceRating);

                    if( amazonItem.ItemAttributes.RunningTime != null)
                        t.Runtime = Convert.ToInt32(amazonItem.ItemAttributes.RunningTime.Value);

                    if( amazonItem.ItemAttributes.TheatricalReleaseDate != null )
                        t.ReleaseDate = AmazonYearToDateTime(amazonItem.ItemAttributes.TheatricalReleaseDate);

                    if( amazonItem.DetailPageURL != null)
                        t.OfficialWebsiteURL = amazonItem.DetailPageURL;

                    if( amazonItem.ItemAttributes.Studio != null )
                        t.Studio = amazonItem.ItemAttributes.Studio;

                    if (amazonItem.ItemAttributes.AspectRatio != null) t.AspectRatio = amazonItem.ItemAttributes.AspectRatio;

                    // add only the genres that we have in our list otherwise we get too much crap
                    if (amazonItem.Subjects != null)
                    {
                        foreach (string subject in amazonItem.Subjects)
                        {
                            if (Properties.Settings.Default.Genres.Contains(subject))
                            {
                                t.AddGenre(subject);
                            }
                        }
                    }

                    DownloadImage(t, amazonItem);

                    return t;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            static private void DownloadImage( Title t, Item amazonItem )
            {
                if (amazonItem.LargeImage != null)
                {
                    if (amazonItem.LargeImage != null && amazonItem.LargeImage.URL != null)
                    {
                        string tempFileName = Path.GetTempFileName();
                        t.FrontCoverPath = tempFileName;
                        WebClient web = new WebClient();
                        try
                        {
                            web.DownloadFile(amazonItem.LargeImage.URL, tempFileName);
                        }
                        catch
                        {
                            File.Delete(tempFileName);
                        }
                    }
                }
            }

            static private DateTime AmazonYearToDateTime(string amazonDate)
            {
                try
                {
                    amazonDate.Trim();
                    if (amazonDate.Length == 0) return new DateTime();
                    // Format to handle Amazon's convention for date strings; 0 means not set
                    string[] expectedFormats = { "d", "yyyy", "yyyy-mm", "yyyymmdd", "yyyy0000", "yyyymm00", "yyyy00dd", "yyyy-mm-dd", "yyyy/mm/dd", "yyyy mm dd" };
                    System.DateTime myDate;
                    if (amazonDate != null)
                    {
                        if (amazonDate.Length == 4)
                            amazonDate = amazonDate + " 01 01";
                        myDate = System.DateTime.ParseExact(amazonDate, expectedFormats, System.Windows.Forms.Application.CurrentCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                    }
                    else
                    {
                        return new DateTime();
                    }

                    return myDate;
                }
                catch (Exception ex)
                {
                    return new DateTime();
                }
            }
            
            private static string AmazonRatingToString(string amazonRating)
            {
                if (amazonRating != null)
                {
                    if (amazonRating.StartsWith("R ") || amazonRating.StartsWith("R("))
                        return "R";
                    else if (amazonRating.StartsWith("G ") || amazonRating.StartsWith("G("))
                        return "G";
                    else if (amazonRating.StartsWith("PG13"))
                        return "PG13";
                    else if (amazonRating.StartsWith("PG"))
                        return "PG";
                    else if (amazonRating.StartsWith("NC"))
                        return "NC17";
                    else
                        return amazonRating;
                }
                else
                {
                    return "";
                }
            }
            /// <summary>
            /// Utility function to return Amazon's custom date/timestamp formats into a .NET Date type
            /// </summary>
            /// <param name="amazonDate">Date string returned by Amazon.com's ProductInfo type</param>
            /// <returns>Date type converted from custom string, or empty string if parameter is nothing</returns>
            /// <remarks></remarks>
            private string AmazonDateFormatToString(string amazonDate)
            {
                try
                {
                    // Format to handle Amazon's convention for date strings; 0 means not set
                    string[] expectedFormats = { "d", "yyyy", "yyyy-mm", "yyyymmdd", "yyyy0000", "yyyymm00", "yyyy00dd", "yyyy-mm-dd" };
                    System.DateTime myDate;
                    if (amazonDate != null)
                    {
                        myDate = System.DateTime.ParseExact(amazonDate, expectedFormats, System.Windows.Forms.Application.CurrentCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                    }
                    else
                    {
                        return "";
                    }

                    return myDate.ToString("yyyy mm dd");
                }
                catch (Exception ex)
                {

                    return "";
                }
            }

            /// <summary>
            /// Utility function that strips HTML tags and whitespace from the input string.
            /// </summary>
            /// <param name="htmlText"></param>
            /// <remarks>Used by GetListOfDVDs method. </remarks>
            public static string FilterHTMLText(string htmlText)
            {
                string filteredText = "";

                if (htmlText != null)
                {
                    filteredText = htmlText;

                    //remove HTML tags using a regular expression
                    filteredText = Regex.Replace(filteredText, "(<[^>]+>)", "");

                    //remove whitespace characters using a regular expression
                    filteredText = Regex.Replace(filteredText, "&(nbsp|#160);", "");

                    //optional: format out additional characters 
                    //...
                }

                return filteredText;
            }

            private string AmazonItemRunningTimeToString(Item amazonItem)
            {
                try
                {
                    string runningTime = "";
                    {
                        {
                            ItemAttributes itemAttributes = amazonItem.ItemAttributes;
                            if ((itemAttributes != null) && (itemAttributes.RunningTime != null))
                            {
                                runningTime = Convert.ToString(itemAttributes.RunningTime.Value);
                            }

                        }
                    }
                    return runningTime;
                }
                catch (Exception ex)
                {

                    return "";
                }
            }

            /// <summary>
            /// Utility function that converts the Amazon editorial review field to a string
            /// </summary>
            /// <remarks>Used to provide a description for an item</remarks>
            private static string AmazonItemDescriptionToString(Item amazonItem)
            {
                try
                {
                    string description = "";
                    EditorialReview[] editorialReviews = amazonItem.EditorialReviews;
                    if ((editorialReviews != null) && (editorialReviews.Length > 0))
                    {
                        description = FilterHTMLText(editorialReviews[0].Content);
                    }
                    return description;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }

            /// <summary>
            /// Utility function that converts the Amazon ImageURL to a URL string
            /// </summary>
            /// <remarks>Used to load the item image</remarks>
            private string AmazonItemLargeImageURLToString(Item amazonItem)
            {
                try
                {
                    string largeImageUrl = "";
                    {
                        if (amazonItem.LargeImage != null)
                        {
                            largeImageUrl = amazonItem.LargeImage.URL;
                        }
                    }
                    return largeImageUrl;
                }
                catch (Exception ex)
                {

                    return "";
                }

           }
        }

}
