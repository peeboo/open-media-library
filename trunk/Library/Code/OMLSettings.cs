using System;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using OMLEngine;

namespace Library
{
    public class OMLSettings : ModelItem
    {
        public OMLSettings() { }

        #region properties
        public string DaemonTools
        {
            get { return OMLEngine.Properties.Settings.Default.DaemonTools; }
            set
            {
                OMLEngine.Properties.Settings.Default.DaemonTools = value;
                OMLEngine.Properties.Settings.Default.Save();
                FirePropertyChanged("DaemonTools");
            }
        }
        public string VirtualDiscDrive
        {
            get { return OMLEngine.Properties.Settings.Default.VirtualDiscDrive; }
            set
            {
                OMLEngine.Properties.Settings.Default.VirtualDiscDrive = value;
                OMLEngine.Properties.Settings.Default.Save();
                FirePropertyChanged("VirtualDiscDrive");
            }
        }
        public int VirtualDiscDriveNumber
        {
            get { return OMLEngine.Properties.Settings.Default.VirtualDiscDriveNumber; }
            set
            {
                OMLEngine.Properties.Settings.Default.VirtualDiscDriveNumber = value;
                OMLEngine.Properties.Settings.Default.Save();
                FirePropertyChanged("VirtualDiscDriveNumber");
            }
        }
        public bool CopyImages
        {
            get { return OMLEngine.Properties.Settings.Default.CopyImages; }
            set
            {
                OMLEngine.Properties.Settings.Default.CopyImages = value;
                OMLEngine.Properties.Settings.Default.Save();
                FirePropertyChanged("CopyImages");
            }
        }
        public bool ShowFileLocation
        {
            get { return Properties.Settings.Default.ShowFileLocation; }
            set
            {
                Properties.Settings.Default.ShowFileLocation = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("ShowFileLocation");
            }
        }
        public bool ShowMovieDetails
        {
            get { return Properties.Settings.Default.ShowMovieDetails; }
            set
            {
                Properties.Settings.Default.ShowMovieDetails = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("ShowMovieDetails");
            }
        }
        public string MovieView
        {
            get { return Properties.Settings.Default.MovieView; }
            set
            {
                Properties.Settings.Default.MovieView = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("MovieView");
            }
        }
        public string ActorView
        {
            get { return Properties.Settings.Default.ActorView; }
            set
            {
                Properties.Settings.Default.ActorView = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("ActorView");
            }
        }
        public string DirectorView
        {
            get { return Properties.Settings.Default.DirectorView; }
            set
            {
                Properties.Settings.Default.DirectorView = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("DirectorView");
            }
        }
        public string GenreView
        {
            get { return Properties.Settings.Default.GenreView; }
            set
            {
                Properties.Settings.Default.GenreView = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("GenreView");
            }
        }
        public string DateAddedView
        {
            get { return Properties.Settings.Default.DateAddedView; }
            set
            {
                Properties.Settings.Default.DateAddedView = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("DateAddedView");
            }
        }
        public string YearView
        {
            get { return Properties.Settings.Default.YearView; }
            set
            {
                Properties.Settings.Default.YearView = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("YearView");
            }
        }
        public string StartPage
        {
            get { return Properties.Settings.Default.StartPage; }
            set
            {
                Properties.Settings.Default.StartPage = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("StartPage");
            }
        }
        public string MovieSort
        {
            get { return Properties.Settings.Default.MovieSort; }
            set
            {
                Properties.Settings.Default.MovieSort = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("MovieSort");
            }
        }
        public string ActorSort
        {
            get { return Properties.Settings.Default.ActorSort; }
            set
            {
                Properties.Settings.Default.ActorSort = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("ActorSort");
            }
        }
        public string DirectorSort
        {
            get { return Properties.Settings.Default.DirectorSort; }
            set
            {
                Properties.Settings.Default.DirectorSort = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("DirectorSort");
            }
        }
        public string YearSort
        {
            get { return Properties.Settings.Default.YearSort; }
            set
            {
                Properties.Settings.Default.YearSort = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("YearSort");
            }
        }
        public string DateAddedSort
        {
            get { return Properties.Settings.Default.DateAddedSort; }
            set
            {
                Properties.Settings.Default.DateAddedSort = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("DateAddedSort");
            }
        }
        public bool SortFullString
        {
            get { return Properties.Settings.Default.SortFullString; }
            set
            {
                Properties.Settings.Default.SortFullString = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("SortFullString");
            }
        }
        public string GenreSort
        {
            get { return Properties.Settings.Default.GenreSort; }
            set
            {
                Properties.Settings.Default.GenreSort = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("GenreSort");
            }
        }
        public float DetailsLeftAnchor
        {
            get { return Properties.Settings.Default.DetailsLeftAnchor; }
            set
            {
                Properties.Settings.Default.DetailsLeftAnchor = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("DetailsLeftAnchor");
            }
        }
        public float DetailsTopAnchor
        {
            get { return Properties.Settings.Default.DetailsTopAnchor; }
            set
            {
                Properties.Settings.Default.DetailsTopAnchor = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("DetailsTopAnchor");
            }
        }
        public int DetailsLeftOffset
        {
            get { return Properties.Settings.Default.DetailsLeftOffset; }
            set
            {
                Properties.Settings.Default.DetailsLeftOffset = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("DetailsLeftOffset");
            }
        }
        public int DetailsTopOffset
        {
            get { return Properties.Settings.Default.DetailsTopOffset; }
            set
            {
                Properties.Settings.Default.DetailsTopOffset = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("DetailsTopOffset");
            }
        }
        public float BottomAnchor
        {
            get { return Properties.Settings.Default.BottomAnchor; }
            set
            {
                Properties.Settings.Default.BottomAnchor = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("BottomAnchor");
            }
        }
        public int BottomOffset
        {
            get { return Properties.Settings.Default.BottomOffset; }
            set
            {
                Properties.Settings.Default.BottomOffset = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("BottomOffset");
            }
        }
        public int GalleryCoverArtRows
        {
            get { return Properties.Settings.Default.GalleryCoverArtRows; }
            set
            {
                Properties.Settings.Default.GalleryCoverArtRows = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("GalleryCoverArtRows");
            }
        }
        public int GalleryListRows
        {
            get { return Properties.Settings.Default.GalleryListRows; }
            set
            {
                Properties.Settings.Default.GalleryListRows = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("GalleryListRows");
            }
        }
        public int CoverArtWidth
        {
            get { return Properties.Settings.Default.CoverArtWidth; }
            set
            {
                Properties.Settings.Default.CoverArtWidth = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("CoverArtWidth");
            }
        }
        public int CoverArtHeight
        {
            get { return Properties.Settings.Default.CoverArtHeight; }
            set
            {
                Properties.Settings.Default.CoverArtHeight = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("CoverArtHeight");
            }
        }
        public int ListItemWidth
        {
            get { return Properties.Settings.Default.ListItemWidth; }
            set
            {
                Properties.Settings.Default.ListItemWidth = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("ListItemWidth");
            }
        }
        public int ListItemHeight
        {
            get { return Properties.Settings.Default.ListItemHeight; }
            set
            {
                Properties.Settings.Default.ListItemHeight = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("ListItemHeight");
            }
        }
        public string UserRatingSort
        {
            get { return Properties.Settings.Default.UserRatingSort; }
            set
            {
                Properties.Settings.Default.UserRatingSort = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("UserRatingSort");
            }
        }
        public float RightAnchor
        {
            get { return Properties.Settings.Default.RightAnchor; }
            set
            {
                Properties.Settings.Default.RightAnchor = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("RightAnchor");
            }
        }
        public int RightOffset
        {
            get { return Properties.Settings.Default.RightOffset; }
            set
            {
                Properties.Settings.Default.RightOffset = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("RightOffset");
            }
        }
        public int CoverArtSpacingHorizontal
        {
            get { return Properties.Settings.Default.CoverArtSpacingHorizontal; }
        set
        {
            Properties.Settings.Default.CoverArtSpacingHorizontal = value;
            Properties.Settings.Default.Save();
            FirePropertyChanged("GallerySpacingHorizontal");
        }
        }
        public int CoverArtSpacingVertical
        {
            get { return Properties.Settings.Default.CoverArtSpacingVertical; }
            set
            {
                Properties.Settings.Default.CoverArtSpacingVertical = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("GallerySpacingVertical");
            }
        }
        public string NameAscendingSort
        {
            get { return Properties.Settings.Default.NameAscendingSort; }
            set
            {
                Properties.Settings.Default.NameAscendingSort = value;
                Properties.Settings.Default.Save();
                FirePropertyChanged("NameAscendingSort");
            }
        }
        #endregion
    }
}
