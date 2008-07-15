using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;

namespace Library
{

    public class UISettings
    {
        public UISettings()
        {
            _anchorSettings = new AnchorSettings();
            _gallerySettings = new GallerySettings();
        }

        public AnchorSettings Anchor
        {
            get { return _anchorSettings; }
        }

        public GallerySettings Gallery
        {
            get { return _gallerySettings; }
        }

        private AnchorSettings _anchorSettings;
        private GallerySettings _gallerySettings;

    }

    public class AnchorSettings
    {
        public float DetailsLeftAnchor
        {
            get { return Properties.Settings.Default.DetailsLeftAnchor; }
        }

        public float DetailsRightAnchor
        {
            get { return Properties.Settings.Default.DetailsLeftAnchor + Properties.Settings.Default.ScrollingLockPosition - 0.08f; }
        }

        public int DetailsLeftOffset
        {
            get { return Properties.Settings.Default.DetailsLeftOffset; }
        }

        public int DetailsLeftBackgroundOffset
        {
            get { return Properties.Settings.Default.DetailsLeftOffset-200; }
        }


        public float DetailsTopAnchor
        {
            get { return Properties.Settings.Default.DetailsTopAnchor; }
        }

        public int DetailsTopOffset
        {
            get { return Properties.Settings.Default.DetailsTopOffset; }
        }

        public int FiltersTopOffset
        {
            get { return Properties.Settings.Default.DetailsTopOffset + 50; }
        }

        public float BottomAnchor
        {
            get { return Properties.Settings.Default.BottomAnchor; }
        }

        public int BottomOffset
        {
            get { return Properties.Settings.Default.BottomOffset; }
        }


        public float GalleryBottomAnchor
        {
            get { return Properties.Settings.Default.BottomAnchor - 0.05f; }
        }

        public int GalleryBottomOffset
        {
            get { return Properties.Settings.Default.BottomOffset - 15; }
        }

        public float RightAnchor
        {
            get { return Properties.Settings.Default.RightAnchor; }
        }

        public int RightOffset
        {
            get { return Properties.Settings.Default.RightOffset; }
        }

        public int NowPlayingBottomOffset
        {
            get { return Properties.Settings.Default.NowPlayingBottomOffset; }
        }
    }

    public class GallerySettings
    {
        public int CoverArtRows
        {
            get { return Properties.Settings.Default.GalleryCoverArtRows; }
        }

        public int ListRows
        {
            get { return Properties.Settings.Default.GalleryListRows; }
        }

        public Size CoverArtSize
        {
            get { return new Size(Properties.Settings.Default.CoverArtWidth, Properties.Settings.Default.CoverArtHeight); }
        }

        public Size ListItemSize
        {
            get { return new Size(Properties.Settings.Default.ListItemWidth, Properties.Settings.Default.ListItemHeight); }
        }

        public Size CoverArtSpacing
        {
            get { return new Size(Properties.Settings.Default.CoverArtSpacingHorizontal, Properties.Settings.Default.CoverArtSpacingVertical); }
        }

        public Size ListSpacing
        {
            get { return new Size(Properties.Settings.Default.ListSpacingHorizontal, Properties.Settings.Default.ListSpacingVertical); }
        }


        public Single CoverArtAlpha
        {
            get { return  Convert.ToSingle(Properties.Settings.Default.CoverArtAlpha); }
        }

        public Single ScrollingLockPosition
        {
            get { return Convert.ToSingle(Properties.Settings.Default.ScrollingLockPosition); }
        }

        public Single LeftPanelDetailsAlpha
        {
            get { return Convert.ToSingle(Properties.Settings.Default.LeftPanelDetailsAlpha); }
        }

        public bool ShowMovieDetails
        {
            get { return Properties.Settings.Default.ShowMovieDetails; }
        }
    }
}