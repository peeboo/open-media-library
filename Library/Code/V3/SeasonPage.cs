using System;
using System.Collections;
using Microsoft.MediaCenter.UI;
using Library.Code.V3;
using System.Collections.Generic;
using System.Text;
using OMLEngine;

namespace Library.Code.V3
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// gallery page UI.
    /// </summary>
    /// 
    [Microsoft.MediaCenter.UI.MarkupVisible]
    public class SeasonPage : GalleryPage
    {
        private SeasonItem seasonItem;
        public SeasonPage(SeasonItem item)
            : base()
        {
            this.seasonItem = item;
            //description
            this.Description = item.Description;
            //this.Filters = filter;
            this.Model = new Library.Code.V3.BrowseModel(this);
            this.Model.Pivots = new Choice(this, "desc", new ArrayListDataSet(this));
            base.CreateCommands();
            this.CreateTitleView();
        }

        private void CreateTitleView()
        {
            Library.Code.V3.EpisodesPivot titlePivot = new Library.Code.V3.EpisodesPivot(this, "title", "No titles were found.", base.Filters, this.seasonItem.TitleObject.Id);
            titlePivot.ContentLabel = this.Description;
            titlePivot.SupportsJIL = true;
            titlePivot.ContentTemplate = "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            titlePivot.ContentItemTemplate = "oneRowGalleryItem4x3";
            titlePivot.DetailTemplate = Library.Code.V3.BrowsePivot.ExtendedDetailTemplate;
            this.Model.Pivots.Options.Add(titlePivot);
            //setting this as the default chosen pivot
            this.Model.Pivots.Chosen = titlePivot;
        }
    }
}
