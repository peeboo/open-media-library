using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.IO;
using System.Xml;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
//            titleCollection.loadTitleCollection();
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

                return myTitles;
            }
        }
        public DisplayItem[] createGallery()
        {
            Trace.WriteLine("Movie:createGallery()");
            if (myTitles == null)
            {
                ArrayList list = new ArrayList();
                /*
                foreach (Title title in titleCollection)
                {
                    list.Add(CreateGalleryItem(title));
                }
                titleCollection.saveTitleCollection();
                myTitles = (DisplayItem[])list.ToArray(typeof(DisplayItem));
                */
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
            item.image = LoadImage(title.boxart_path);
            item.runtime = title.Runtime;
            item.mpaaRating = title.MPAARating;
            item.imdbRating = title.IMDBRating;

            item.Invoked += delegate(object sender, EventArgs args)
            {
                DisplayItem galleryItem = (DisplayItem)sender;

                // Navigate to a details page for this item.
                DetailsPage page = CreateDetailsPage(title);
                Application.Current.GoToDetails(page);
            };

            return item;
        }
        public DetailsPage CreateDetailsPage(Title title)
        {
            Trace.WriteLine("Movie:CreateDetailsPage()");
            DetailsPage page = new DetailsPage();
            page.Title = title.Name;
            page.Summary = title.Summary;
            page.Background = LoadImage(title.boxart_path);
            page.Rating = title.MPAARating;
            page.Length = title.Runtime;
            page.ReleaseDate = title.ReleaseDate.ToShortDateString();
            page.Actors = title.Actors;
            page.Directors = title.Directors;
            page.Producers = title.Producers;
            page.Writers = title.Writers;
            page.ImdbRating = title.IMDBRating;
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