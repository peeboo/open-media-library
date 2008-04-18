using System;
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
//            Utilities.ImportData(ref dataSet);
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
            DisplayItem item = new DisplayItem();
            item.Description = title.Description;
            item.title = title.Name;
            item.itemId = title.itemId;
            item.image = LoadImage(title.front_boxart_path);
            item.runtime = title.Runtime;
            item.mpaaRating = title.MPAARating;

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
            page.Title = item.title;
//            page.Summary;
            page.Background = item.image;
            page.Rating = item.mpaaRating;
            page.Length = item.runtime;
//            page.ReleaseDate;
//            page.Actors;
//            page.Directors;
//            page.Producers;
//            page.Writers;
            //page.LocalMedia = new System.IO.FileInfo("C:\\users\\dxs\\documents\\Downloads\\Good Eats - Season 6\\Good Eats - S06E16 - Beet It.avi");

            return page;
        }
        public static Image LoadImage(string imageName)
        {
            Trace.WriteLine("Movie:LoadImage()");
            try
            {
                return new Image("file://" + imageName);
            }
            catch (Exception)
            {
                Trace.WriteLine("Error loading image: " + imageName);
            }
            return null;
        }
    }
}