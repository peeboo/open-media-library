using System;
using System.IO;
using System.Data;
using System.Collections;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;

namespace Library
{
    public class Movie
    {
        private DataSet dataSet;
        private static TitleCollection titleCollection;

        private static DisplayItem[] myTitles = null;
        private static Boolean initialized = false;

        public Movie()
        {
            Trace.WriteLine("Movie:Movie()");
            dataSet = new DataSet();
            titleCollection = new TitleCollection();
            titleCollection.loadTitleCollection();
            initialize();
        }
        public void initialize()
        {
            Trace.WriteLine("Movie:initialize()");
            createGallery();
            initialized = true;
        }
        public DisplayItem[] GetMovies
        {
            get
            {
                if (!initialized)
                    initialize();

                if (myTitles != null)
                {
                    Trace.WriteLine("GetMovies: returning " + myTitles.Length + " titles");
                }
                else
                {
                    Trace.WriteLine("GetMovies: myTitles is null");
                }
                return myTitles;
            }
        }
        public DisplayItem[] createGallery()
        {
            Trace.WriteLine("Movie:createGallery()");
            if (myTitles == null)
            {
                ArrayList list = new ArrayList();
                foreach (Title title in titleCollection)
                {
                    list.Add(CreateGalleryItem(title));
                }
                titleCollection.saveTitleCollection();
                myTitles = (DisplayItem[])list.ToArray(typeof(DisplayItem));
            }
            return myTitles;
        }
        private DisplayItem CreateGalleryItem(Title title)
        {
            Trace.WriteLine("Movie:CreateGalleryItem(): Title");
            DisplayItem item = new DisplayItem(title);

            item.Invoked += delegate(object sender, EventArgs args)
            {
                DisplayItem galleryItem = (DisplayItem)sender;

                // Navigate to a details page for this item.
                DetailsPage page = CreateDetailsPage(item);
                Application.Current.GoToDetails(page);
            };

            return item;
        }
        public DetailsPage CreateDetailsPage(DisplayItem item)
        {
            Trace.WriteLine("Movie:CreateDetailsPage()");
            DetailsPage page = new DetailsPage();
            page.Title = item.GetTitle;
            page.Summary = item.GetSummary;
            page.Background = item.GetImage;
            page.Rating = item.GetMpaaRating;
            page.Length = item.GetRuntime;
            page.ReleaseDate = item.GetReleaseDate;
            page.Actors = item.GetActors;
            page.Directors = item.GetDirectors;
            page.Producers = item.GetProducers;
            page.Writers = item.GetWriters;
            page.LocalMedia = new System.IO.FileInfo("C:\\users\\dxs\\documents\\Downloads\\Good Eats - Season 6\\Good Eats - S06E16 - Beet It.avi");

            return page;
        }
        public static Image LoadImage(string imageName)
        {
            Trace.WriteLine("LoadImage (location) : " + "file://" + imageName);
            if (File.Exists(imageName))
            {
                return new Image("file://" + imageName);
            }
            else
            {
                return new Image("resx://Library/Library.Resources/nocover");
            }
        }
    }
}