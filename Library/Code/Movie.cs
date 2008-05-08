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
//            dataSet = new DataSet();
            titleCollection = new TitleCollection();
            titleCollection.loadTitleCollection();
            if (titleCollection.Count == 0)
            {
                Title t = new Title();
                t.Name = "No Titles in Database";
                titleCollection.AddTitle(t);
            }
            initialize();
        }
        public void initialize()
        {
            createGallery();
            initialized = true;
        }
        public DisplayItem[] GetMovies
        {
            get
            {
                if (!initialized)
                    initialize();
                return myTitles;
            }
        }
        public DisplayItem[] createGallery()
        {
            if (myTitles == null)
            {
                SortedArrayList list = new SortedArrayList();
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