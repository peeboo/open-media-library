using System;
using System.Collections.Generic;
using System.Drawing;
using OMLEngine;
using System.Text.RegularExpressions;
using AmazonMetaData2.Amazon.ECS;

namespace AmazonMetaData2
{
    class AmazonToOML
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

                if (amazonItem.ItemAttributes.AudienceRating != null)
                    t.ParentalRating = AmazonRatingToString(amazonItem.ItemAttributes.AudienceRating);

                if (amazonItem.ItemAttributes.RunningTime != null)
                    t.Runtime = Convert.ToInt32(amazonItem.ItemAttributes.RunningTime.Value);

                if (amazonItem.ItemAttributes.TheatricalReleaseDate != null)
                    t.ReleaseDate = AmazonYearToDateTime(amazonItem.ItemAttributes.TheatricalReleaseDate);

                if (amazonItem.DetailPageURL != null)
                    t.OfficialWebsiteURL = amazonItem.DetailPageURL;

                if (amazonItem.ItemAttributes.Studio != null)
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

                if (amazonItem.LargeImage.URL != null)
                {
                    t.FrontCoverPath = amazonItem.LargeImage.URL;// title.ImageUrl;
                }

                //DownloadImage(t, amazonItem);

                return t;
            }
            catch
            {
                return null;
            }
        }

        /*static private void DownloadImage( Title t, Item amazonItem )
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
        }*/

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
            catch
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
            catch
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
            catch
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
            catch
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
            catch
            {
                return "";
            }

        }
    }
}
