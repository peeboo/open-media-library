using System;
using System.Collections;
using Microsoft.MediaCenter.UI;
using Library.Code.V3;

namespace Library.Code.V3
{
    public class PageStateEx
    {
        public PageTransitionState TransitionState
        {
            get
            {
                if (this.transitionState == PageTransitionState.NavigatingToForward)
                {
                    this.transitionState = PageTransitionState.NavigatingToBackward;
                    return PageTransitionState.NavigatingToForward;
                }
                else
                {
                    return PageTransitionState.NavigatingToBackward;
                }
            }
        }
        private PageTransitionState transitionState = PageTransitionState.NavigatingToForward;
    }
    [Microsoft.MediaCenter.UI.MarkupVisible]
    public class GalleryPageSeries : GalleryPage
    {

    }

    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// gallery page UI.
    /// </summary>
    /// 
    [Microsoft.MediaCenter.UI.MarkupVisible]
    public class GalleryPage : ContentPage
    {
        /// <summary>
        /// The desired size for a galley item.  This will be interpreted by the 
        /// UI to control what size to allow for the thumbnails as well as how 
        /// many rows to display.
        /// </summary>
        /// 

        public GalleryPage()
            : base()
        {
            this.PageState = new PageState(this);
            this.PageState.IsCurrentPage = true;

            this.cachedSelectedValue = new IntRangedValue(this);
            this.cachedSelectedValue.Value = 0;

            this.PageStateEx = new PageStateEx();

            this.title = "movies";
            _JILtext = new EditableText(this.Owner, "JIL");
            JILtext.Value = "";
            JILtext.Submitted += new EventHandler(JILtext_Submitted);
        }

        private IntRangedValue cachedSelectedValue;
        public IntRangedValue CachedSelectedValue
        {
            get
            {
                return this.cachedSelectedValue;
            }
            set
            {
                this.cachedSelectedValue = value;
            }
        }
        public void BackHandlerEvent()
        {
            //TODO: implement in OML
            //Application.Current.Session.BackPage();
        }

        private PageState pageState;
        public PageState PageState
        {
            get
            {
                return this.pageState;
            }
            set
            {
                this.pageState = value;
            }
        }

        private PageStateEx pageStateEx;
        public PageStateEx PageStateEx
        {
            get
            {
                return this.pageStateEx;
            }
            set
            {
                this.pageStateEx = value;
            }
        }

        private BrowseModel model;
        public BrowseModel Model
        {
            get { return model; }
            set
            {
                if (model != value)
                {
                    model = value;
                    FirePropertyChanged("Model");
                }
            }
        }

        private VirtualList content2;
        public VirtualList Content2
        {
            get { return content2; }
            set
            {
                if (content2 != value)
                {
                    content2 = value;
                    FirePropertyChanged("Content2");
                    if (content2 != null && content2.Count > 0 && content2[0] is ThumbnailCommand)
                    {
                        this.SelectedItem = (ThumbnailCommand)content2[0];
                    }
                }
            }
        }

        private Image fanArt = null;
        public Image FanArt
        {
            get
            {
                return this.fanArt;
            }
            set
            {
                if (this.fanArt != value)
                {
                    this.fanArt = value;
                    base.FirePropertyChanged("FanArt");
                }
            }
        }

        private int itemID;
        public int ItemID
        {
            get
            {
                return itemID;
            }
            set
            {
                this.itemID = value;
            }
        }

        private GalleryItemSize itemSize;
        public GalleryItemSize ItemSize
        {
            get { return itemSize; }
            set { itemSize = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string galleryLayout;
        [Microsoft.MediaCenter.UI.MarkupVisible]
        public string GalleryLayout
        {
            get
            {
                if (galleryLayout != null)
                    return galleryLayout;
                else
                    return "twoRowGalleryItemPoster";
            }
            set { if (galleryLayout != value) { galleryLayout = value; base.FirePropertyChanged("GalleryLayout"); } }
        }

        private string typerDisplay;
        [Microsoft.MediaCenter.UI.MarkupVisible]
        public string TyperDisplay
        {
            get
            {
                return typerDisplay;

            }
            set
            {
                if (((BrowsePivot)this.model.Pivots.Chosen).SupportsJIL)
                {
                    typerDisplay = value;

                    if (value != "")
                    {
                        SlowSearch(value);
                    }
                }
                else
                {
                    JILtext.Value = "";
                }
            }
        }

        #region SlowSearch

        private int doSearch(string searchString)
        {
            int jump = 0;
            VirtualList pivotList = (VirtualList)((BrowsePivot)this.model.Pivots.Chosen).Content;
            foreach (GalleryItem item in pivotList)
            {
                string s = item.Description.ToLower().Replace("the ", "").Replace("a ", "");
                int cmpVal = s.CompareTo(searchString);
                if (cmpVal == 0)
                { // the values are the same
                    jump = pivotList.IndexOf(item);
                    //need to invoke on UI thread
                    //JILindex = jump - JILCurrentindex;
                    break;
                    //direct match
                }
                else if (cmpVal > 0)
                {
                    //jump = pivotList.IndexOf(item) + 1;
                    jump = pivotList.IndexOf(item);
                    //need to invoke on UI thread
                    //JILindex = jump - JILCurrentindex - 1;
                    break;
                }
            }
            return jump;
        }
        public void SlowSearch(string searchString)
        {
            Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread
                                (
                // Delegate to be invoked on background thread
                                 startLoadSlowSearch,

                                 // Delegate to be invoked on app thread
                                 endLoadSlowSearch,

                                 // Parameter to be passed to both delegates
                                 (object)searchString
                                 );
        }

        private void startLoadSlowSearch(object objSearchString)
        {

            try
            {
                string searchString = (string)objSearchString;
                int jump = this.doSearch(searchString);
                if (jump != 0)
                    Microsoft.MediaCenter.UI.Application.DeferredInvoke(notifySlowSearch, jump);
            }
            finally
            {
                //
                // Reset our thread's priority back to its previous value
                //
                //Thread.CurrentThread.Priority = priority;
            }
        }

        private void notifySlowSearch(object obj)
        {
            if (!IsDisposed)
            {
                JILindex = (int)obj - JILCurrentindex;
            }
        }


        private void endLoadSlowSearch(object obj)
        {
            //nothing yet
        }
        #endregion SlowSearch

        private EditableText _JILtext;
        private Int32 _JILindex = new Int32();

        [Microsoft.MediaCenter.UI.MarkupVisible]
        public EditableText JILtext
        {
            get { return _JILtext; }
            set { if (_JILtext != value) { _JILtext = value; base.FirePropertyChanged("JILtext"); } }
        }

        [Microsoft.MediaCenter.UI.MarkupVisible]
        public Int32 JILindex
        {
            get { return _JILindex; }
            set { if (_JILindex != value) { _JILindex = value; base.FirePropertyChanged("JILindex"); } }
        }

        private Int32 _JILCurrentindex = new Int32();
        [Microsoft.MediaCenter.UI.MarkupVisible]
        public Int32 JILCurrentindex
        {
            get { return _JILCurrentindex; }
            set { if (_JILCurrentindex != value) { _JILCurrentindex = value; base.FirePropertyChanged("JILCurrentindex"); } }
        }

        void JILtext_Submitted(Object sender, EventArgs e)
        {
            if (JILtext.Value != "")
                JILtext.Value = "";
        }
    }

    /// <summary>
    /// The possible size configurations supported by the gallery UI.
    /// </summary>
    public enum GalleryItemSize
    {
        Small,
        Large
    }
    public enum GalleryType
    {
        oneRowGalleryItemPoster,
        twoRowGalleryItemPoster,
        oneRowGalleryItem4x3
    }

}
