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
            get { return Properties.Settings.Default.DetailsTopOffset + 30; }
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
        public Size HeightOfBrowser
        {
            get
            {
                if (Properties.Settings.Default.ShowMovieDetails)
                    return new Size(0, Properties.Settings.Default.BrowserHeightWhenShowingDetails);
                else
                    return new Size(0, Properties.Settings.Default.BrowserHeightWhenNotShowingDetails);
            }
        }
        public int CoverArtRows
        {
            get { return Properties.Settings.Default.GalleryCoverArtRows; }
        }

        public int CarouselRows
        {
            get { return Properties.Settings.Default.CarouselRows; }
        }

        public int ListRows
        {
            get { return Properties.Settings.Default.GalleryListRows; }
        }

        public Size CoverArtSize
        {
            //TODO: get the 600 out of here into a setting. just experimenting for now
            get
            {
                int rows = Properties.Settings.Default.GalleryCoverArtRows;
                int height = (520 - rows * 2 * Properties.Settings.Default.CoverArtSpacingVertical) / rows;
                int width = (int)(height * 0.705);

                Properties.Settings.Default.FocusCoverArtScale = 1.3f - 0.16f * (3-rows);

                return new Size(width, height);
            }
            //get { return new Size(Properties.Settings.Default.CoverArtWidth, Properties.Settings.Default.CoverArtHeight); }
        }

        public Size ListItemSize
        {
            get { return new Size(Properties.Settings.Default.ListItemWidth, Properties.Settings.Default.ListItemHeight); }
        }

        public Size CarouselArtSize
        {
            get { return new Size(Properties.Settings.Default.CarouselItemWidth, Properties.Settings.Default.CarouselItemHeight); }
        }

        public Size CoverArtSpacing
        {
            get { return new Size(Properties.Settings.Default.CoverArtSpacingHorizontal, Properties.Settings.Default.CoverArtSpacingVertical); }
        }

        public Size ListSpacing
        {
            get { return new Size(Properties.Settings.Default.ListSpacingHorizontal, Properties.Settings.Default.ListSpacingVertical); }
        }

        public Size CarouselArtSpacing
        {
            get { return new Size(Properties.Settings.Default.CarouselItemSpacingHorizontal, Properties.Settings.Default.CarouselItemSpacingVertical); }
        }

        public Single CoverArtAlpha
        {
            get { if (Properties.Settings.Default.DimUnselectedCovers) return 0.5F; else return 1F; }
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
   
        public bool DimUnselectedCovers
        {
            get { return Properties.Settings.Default.DimUnselectedCovers; }
        }

        public Vector3 FocusCoverArtScale
        {
            get { return new Vector3(Properties.Settings.Default.FocusCoverArtScale, Properties.Settings.Default.FocusCoverArtScale, Properties.Settings.Default.FocusCoverArtScale); }
        }

        public Vector3 FocusCarouselScale
        {
            get { return new Vector3(Properties.Settings.Default.FocusCarouselScale, Properties.Settings.Default.FocusCarouselScale, Properties.Settings.Default.FocusCarouselScale); }
        }

        public Vector3 FocusListScale
        {
            get { return new Vector3(Properties.Settings.Default.FocusListScale, Properties.Settings.Default.FocusListScale, Properties.Settings.Default.FocusListScale); }
        }

        public bool UseOnScreenAlphaFiltering
        {
            get { return Properties.Settings.Default.UseOnScreenAlphaJumper; }
        }
    }
}
