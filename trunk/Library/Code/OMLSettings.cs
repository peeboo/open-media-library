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
        }
        public string MovieSort
        {
            get { return Properties.Settings.Default.MovieSort; }
        }
        public string ActorSort
        {
            get { return Properties.Settings.Default.ActorSort; }
        }
        public string DirectorSort
        {
            get { return Properties.Settings.Default.DirectorSort; }
        }
        public string YearSort
        {
            get { return Properties.Settings.Default.YearSort; }
        }
        public string DateAddedSort
        {
            get { return Properties.Settings.Default.DateAddedSort; }
        }
        public bool SortFullString
        {
            get { return Properties.Settings.Default.SortFullString; }
        }
        public string GenreSort
        {
            get { return Properties.Settings.Default.GenreSort; }
        }
        public float DetailsLeftAnchor
        {
            get { return Properties.Settings.Default.DetailsLeftAnchor; }
        }
        public float DetailsTopAnchor
        {
            get { return Properties.Settings.Default.DetailsTopAnchor; }
        }
        public int DetailsLeftOffset
        {
            get { return Properties.Settings.Default.DetailsLeftOffset; }
        }
        public int DetailsTopOffset
        {
            get { return Properties.Settings.Default.DetailsTopOffset; }
        }
        public float BottomAnchor
        {
            get { return Properties.Settings.Default.BottomAnchor; }
        }
        public int BottomOffset
        {
            get { return Properties.Settings.Default.BottomOffset; }
        }
        public int GalleryCoverArtRows
        {
            get { return Properties.Settings.Default.GalleryCoverArtRows; }
        }
        public int GalleryListRows
        {
            get { return Properties.Settings.Default.GalleryListRows; }
        }
        public int CoverArtWidth
        {
            get { return Properties.Settings.Default.CoverArtWidth; }
        }
        public int CoverArtHeight
        {
            get { return Properties.Settings.Default.CoverArtHeight; }
        }
        public int ListItemWidth
        {
            get { return Properties.Settings.Default.ListItemWidth; }
        }
        public int ListItemHeight
        {
            get { return Properties.Settings.Default.ListItemHeight; }
        }
        public string UserRatingSort
        {
            get { return Properties.Settings.Default.UserRatingSort; }
        }
        public float RightAnchor
        {
            get { return Properties.Settings.Default.RightAnchor; }
        }
        public int RightOffset
        {
            get { return Properties.Settings.Default.RightOffset; }
        }
        public int GallerySpacingHorizontal
        {
            get { return Properties.Settings.Default.GallerySpacingHorizontal; }
        }
        public int GallerySpacingVertical
        {
            get { return Properties.Settings.Default.GallerySpacingVertical; }
        }
        public string NameAscendingSort
        {
            get { return Properties.Settings.Default.NameAscendingSort; }
        }
        #endregion
    }
}
