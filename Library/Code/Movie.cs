using System;
using System.IO;
using System.Data;
using System.Collections;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;

namespace Library
{
    public class MovieGallery
    {
        private static TitleCollection _titleCollection;
        private static MovieItem[] _myTitles = null;

        public MovieGallery()
        {
            _titleCollection = new TitleCollection();
            LoadMovies();
            CreateGallery();
        }

        public void LoadMovies()
        {
            _titleCollection.loadTitleCollection();
        }

        public MovieItem[] CreateGallery()
        {
            if (_myTitles == null)
            {
                SortedArrayList list = new SortedArrayList();
                foreach (Title title in _titleCollection)
                {
                    list.Add(CreateGalleryItem(title));
                }
                //_titleCollection.saveTitleCollection();
                _myTitles = (MovieItem[])list.ToArray(typeof(MovieItem));
            }
            return _myTitles;
        }

        public MovieItem[] Movies
        {
            get
            {
                return _myTitles;
            }
        }
        private MovieItem CreateGalleryItem(Title title)
        {
            MovieItem item = new MovieItem(title);

            item.Invoked += delegate(object sender, EventArgs args)
            {
                MovieItem galleryItem = (MovieItem)sender;

                // Navigate to a details page for this item.
                MovieDetailsPage page = CreateDetailsPage(item);
                OMLApplication.Current.GoToDetails(page);
            };

            return item;
        }
        public MovieDetailsPage CreateDetailsPage(MovieItem item)
        {
            Trace.WriteLine("Creating a detailspage");
            MovieDetailsPage page = new MovieDetailsPage(item);
            Trace.WriteLine("adding the item");
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