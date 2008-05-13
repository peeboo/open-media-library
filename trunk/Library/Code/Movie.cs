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
        //private DataSet                 _dataSet;
        private static Boolean _NeedSetup = false;
        private static TitleCollection _titleCollection;
        private static MovieItem[] _myTitles = null;
        private static Boolean _initialized = false;

        public MovieGallery()
        {
            //            dataSet = new DataSet();
            _titleCollection = new TitleCollection();
            _titleCollection.loadTitleCollection();
            if (_titleCollection.Count == 0)
            {
                _NeedSetup = true;
                Title t = new Title();
                t.Name = "No Titles in Database";
                _titleCollection.AddTitle(t);
            }
            Initialize();
        }

        public String NeedSetup
        {
            get { return _NeedSetup.ToString(); }
        }

        public void Initialize()
        {
            CreateGallery();
            _initialized = true;
        }

        public DataTable MoviesDataSet
        {
            get
            {
                if (!_initialized) Initialize();
                return _titleCollection.ToDataTable();
            }
        }

        public MovieItem[] Movies
        {
            get
            {
                if (!_initialized) Initialize();
                return _myTitles;
            }
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
                _titleCollection.saveTitleCollection();
                _myTitles = (MovieItem[])list.ToArray(typeof(MovieItem));
            }
            return _myTitles;
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