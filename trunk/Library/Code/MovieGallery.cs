using System;
using System.IO;
using System.Data;
using System.Collections;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;

namespace Library
{
    public class MovieGallery : ModelItem
    {
        private static TitleCollection _titleCollection;
        private static MovieItem[] _myTitles = null;
        private static DataSet _dataSet;

        public MovieGallery()
        {
            _titleCollection = new TitleCollection();
            LoadMovies();
            CreateGallery();
        }

        private void LoadMovies()
        {
            _titleCollection.loadTitleCollection();
        }

        private MovieItem[] CreateGallery()
        {
            if (_myTitles == null)
            {
                SortedArrayList list = new SortedArrayList();
                foreach (Title title in _titleCollection)
                {
                    list.Add(CreateGalleryItem(title));
                }
                _myTitles = (MovieItem[])list.ToArray(typeof(MovieItem));
            }
            _dataSet = _titleCollection.GetDataSet();
            
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