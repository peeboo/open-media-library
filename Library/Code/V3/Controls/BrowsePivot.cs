using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Collections;
using System.Collections.Specialized;

namespace Library.Code.V3
{
    [MarkupVisible]
    public class BrowsePivot : BaseBrowsePivot, IBrowsePivot
    {
        private Boolean supportsItemContext = false;
        public Boolean SupportsItemContext
        {
            get { return supportsItemContext; }
            set
            {
                supportsItemContext = value;
                FirePropertyChanged("SupportsItemContext");
            }
        }

        public virtual int DoSearch(string searchString)
        {
            return 0;
        }
        public void beginLoadContent(object pivot)
        {
            //testing a slow load...
            //System.Threading.Thread.Sleep(5000);

            List<OMLEngine.Title> titles = new List<OMLEngine.Title>(OMLEngine.TitleCollectionManager.GetAllTitles());
            Application.DeferredInvoke(delegate
                {
                    foreach (OMLEngine.Title t in titles)
                    {
                        this.m_listContent.Add(new Library.Code.V3.MovieItem(t, this));
                    }
                });
        }

        public void endLoadContent(object pivot)
        {
            if (!IsDisposed)
            {
                if (this.Content.Count > 25)
                {
                    this.ContentItemTemplate = "twoRowGalleryItemPoster";
                    this.DetailTemplate = BrowsePivot.StandardDetailTemplate;
                }
                else
                {
                    this.ContentItemTemplate = "oneRowGalleryItemPoster";
                    this.DetailTemplate = BrowsePivot.ExtendedDetailTemplate;
                }
                this.FirePropertyChanged("Content");
                this.IsBusy = false;
            }
        }

        // Fields
        private Choice m_choiceSort;
        //private ContentMapping[] m_contentItemMappings;
        private IList m_listContent;
        private ISelectionPolicy m_policyContent;
        private string m_stContentItemTemplate;
        private string m_stContentLabel;
        private string m_stContentTemplate;
        private string m_stContentUnderlayTemplate;
        private string m_stDetailTemplate;
        private string m_stNoContentText;

        private Boolean isBusy = false;
        public Boolean IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                FirePropertyChanged("IsBusy");
            }
        }

        private void SetupContextMenu()
        {
            #region ctx menu
            //create the context menu
            Library.Code.V3.ContextMenuData ctx = new Library.Code.V3.ContextMenuData();

            Library.Code.V3.ThumbnailCommand viewSettingsCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSettingsCmd.Invoked += new EventHandler(viewSettingsCmd_Invoked);
            viewSettingsCmd.Description = "Settings";

            Library.Code.V3.ThumbnailCommand viewSearchCmd = new Library.Code.V3.ThumbnailCommand(this);
            viewSearchCmd.Invoked += new EventHandler(this.viewSearchCmd_Invoked);
            viewSearchCmd.Description = "Search";
            ctx.SharedItems.Add(viewSettingsCmd);
            ctx.SharedItems.Add(viewSearchCmd);

            this.ContextMenu = ctx;
            #endregion ctx menu
        }

        private void viewSearchCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.CatchMoreInfo();
            if (this.Owner is GalleryPage)
                ((GalleryPage)this.Owner).searchCmd_Invoked(sender, e);
        }

        private void viewSettingsCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.CatchMoreInfo();
            if (this.Owner is GalleryPage)
                ((GalleryPage)this.Owner).settingsCmd_Invoked(sender, e);
        }

        private ContextMenuData m_contextMenu;
        public ContextMenuData ContextMenu
        {
            get
            {
                return m_contextMenu;
            }
            set
            {
                this.m_contextMenu = value;
            }
        }

        private bool supportsJIL = false;
        public override bool SupportsJIL
        {
            get
            {
                return supportsJIL;
            }
            set
            {
                this.supportsJIL = value;
            }
        }

        // Methods
        public BrowsePivot()
            : this(null, null, null)
        {
        }

        public BrowsePivot(IModelItemOwner owner, string stDescription, string stNoContentText)
            : this(owner, stDescription, stNoContentText, new ArrayList())
        {
        }

        public BrowsePivot(IModelItemOwner owner, string stDescription, string stNoContentText, IList listContent)
            : base(owner, stDescription)
        {
            this.m_stNoContentText = stNoContentText;
            this.m_listContent = listContent;
            this.m_stContentTemplate = StandardGalleryTemplate;
            this.m_stDetailTemplate = StandardDetailTemplate;
            CustomSelectionPolicy policy = new CustomSelectionPolicy();
            policy.SelectOnGainFocus = true;
            policy.ClearSelectionOnLostFocus = true;
            this.m_policyContent = policy;
        }

        // Properties
        //public ContentMapping[] AdditionalContentItemMappings
        //{
        //    get
        //    {
        //        return this.m_contentItemMappings;
        //    }
        //    set
        //    {
        //        this.m_contentItemMappings = value;
        //    }
        //}

        public virtual IList Content
        {
            get
            {
                return this.m_listContent;
            }
            set
            {
                if (this.m_listContent != value)
                {
                    this.m_listContent = value;
                    base.FirePropertyChanged("Content");
                }
            }
        }

        public NameValueCollection SupportedContentItemTemplates = new NameValueCollection();

        public string ContentItemTemplate
        {
            get
            {
                return this.m_stContentItemTemplate;
            }
            set
            {
                if (this.m_stContentItemTemplate != value)
                {
                    this.m_stContentItemTemplate = value;
                    UpdateContext(this.m_stContentItemTemplate);
                    base.FirePropertyChanged("ContentItemTemplate");
                }
            }
        }

        public virtual void UpdateContext(string newTemplate)
        {
        }
        private string m_stSubContentItemTemplate;
        public string SubContentItemTemplate
        {
            get
            {
                return this.m_stSubContentItemTemplate;
            }
            set
            {
                if (this.m_stSubContentItemTemplate != value)
                {
                    this.m_stSubContentItemTemplate = value;
                    base.FirePropertyChanged("SubContentItemTemplate");
                }
            }
        }

        public string ContentLabel
        {
            get
            {
                return this.m_stContentLabel;
            }
            set
            {
                this.m_stContentLabel = value;
            }
        }

        public ISelectionPolicy ContentSelectionPolicy
        {
            get
            {
                return this.m_policyContent;
            }
        }

        public string ContentTemplate
        {
            get
            {
                return this.m_stContentTemplate;
            }
            set
            {
                this.m_stContentTemplate = value;
            }
        }

        public string ContentUnderlayTemplate
        {
            get
            {
                return this.m_stContentUnderlayTemplate;
            }
            set
            {
                this.m_stContentUnderlayTemplate = value;
            }
        }

        public string DetailTemplate
        {
            get
            {
                return this.m_stDetailTemplate;
            }
            set
            {
                if (this.m_stDetailTemplate != value)
                {
                    this.m_stDetailTemplate = value;
                    base.FirePropertyChanged("DetailTemplate");
                }
            }
        }

        public string EmptyContentText
        {
            get
            {
                return this.m_stNoContentText;
            }
            set
            {
                if (this.m_stNoContentText != value)
                {
                    this.m_stNoContentText = value;
                    base.FirePropertyChanged("EmptyContentText");
                }
            }
        }

        public Choice SortOptions
        {
            get
            {
                return this.m_choiceSort;
            }
            set
            {
                this.m_choiceSort = value;
            }
        }

        public static string Standard1RowGalleryItem4x3Template
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseGalleryItem#1RowGalleryItem4x3";
            }
        }

        public static string Standard1RowGalleryItemTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseGalleryItem#1RowGalleryItem";
            }
        }

        public static string Standard2RowGalleryItemTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseGalleryItem#2RowGalleryItem";
            }
        }

        public static string Standard3RowGalleryItem4x3Template
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseGalleryItem#3RowGalleryItem4x3";
            }
        }

        public static string Standard3RowGalleryItemTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseGalleryItem#3RowGalleryItem";
            }
        }

        public static string Standard5RowGalleryItemTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseGalleryItem#5RowGalleryItem";
            }
        }

        public static string StandardDetailTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseDetails#Details";
            }
        }

        public static string ExtendedDetailTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseDetails#ExtendedDetails";
            }
        }

        public static string StandardGalleryTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseGallery#Gallery";
            }
        }

        public static string StandardGroupedGalleryTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseGallery#GroupedGallery";
            }
        }

        public static string StandardGroupedListViewTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/BrowseList#GroupedListView";
            }
        }

        public static string StandardListViewTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/BrowseList#ListView";
            }
        }
    }

    public delegate int FindContentItemHandler(IBrowsePivot pivot, string strSearch);

    public abstract class BaseBrowsePivot : ModelItem, IBrowseSearchList
    {
        // Fields
        private FindContentItemHandler m_findDelegate;

        // Methods
        public BaseBrowsePivot(IModelItemOwner owner)
            : base(owner)
        {
        }

        public BaseBrowsePivot(IModelItemOwner owner, string stDescription)
            : base(owner, stDescription)
        {
        }

        public virtual int FindContentItem(string strSearch)
        {
            if (string.IsNullOrEmpty(strSearch))
            {
                return -1;
            }
            IBrowsePivot pivot = this as IBrowsePivot;
            if (pivot == null)
            {
                throw new InvalidOperationException("BaseBrowsePivot.FindContentItem requires the derived class be an IBrowsePivot.");
            }
            if (this.m_findDelegate != null)
            {
                return this.m_findDelegate(pivot, strSearch);
            }
            return FindContentItem(pivot, strSearch);
        }

        public static int FindContentItem(IBrowsePivot pivot, string strSearch)
        {
            if (pivot == null)
            {
                throw new ArgumentNullException("pivot");
            }
            return FindContentItem(pivot.Content, strSearch);
        }

        protected static int FindContentItem(IList listContent, string strSearch)
        {
            if (!ListUtility.IsNullOrEmpty(listContent))
            {
                for (int i = 0; i < listContent.Count; i++)
                {
                    object obj2 = listContent[i];
                    bool flag = false;
                    IBrowseGroup group = obj2 as IBrowseGroup;
                    if (group != null)
                    {
                        if (!string.IsNullOrEmpty(group.Description))
                        {
                            flag = IsMatchForSearchString(group.Description, strSearch);
                        }
                    }
                    else
                    {
                        string description;
                        ICommand command = obj2 as ICommand;
                        if (command != null)
                        {
                            description = command.Description;
                        }
                        else
                        {
                            description = string.Empty;
                        }
                        if (!string.IsNullOrEmpty(description))
                        {
                            flag = IsMatchForSearchString(description, strSearch);
                        }
                    }
                    if (flag)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static bool IsMatchForSearchString(string strItem, string strSearch)
        {
            if (string.IsNullOrEmpty(strItem))
            {
                return false;
            }
            if (string.IsNullOrEmpty(strSearch))
            {
                return false;
            }
            return strItem.StartsWith(strSearch, StringComparison.CurrentCultureIgnoreCase);
        }

        // Properties
        public FindContentItemHandler FindContentItemHandler
        {
            get
            {
                return this.m_findDelegate;
            }
            set
            {
                if (this.m_findDelegate != value)
                {
                    this.m_findDelegate = value;
                    base.FirePropertyChanged("FindContentItemHandler");
                }
            }
        }

        public virtual bool SupportsJIL
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
    }

    internal static class ListUtility
    {
        // Methods
        public static int GetUnwrappedIndex(int idxData, int nGeneration, int cItems)
        {
            return (idxData + (nGeneration * cItems));
        }

        public static void GetWrappedIndex(int idx, int cItems, out int idxData, out int nGeneration)
        {
            if (cItems == 0)
            {
                idxData = 0;
                nGeneration = 0;
            }
            else
            {
                idxData = idx % cItems;
                if (idxData < 0)
                {
                    idxData = cItems + idxData;
                }
                nGeneration = idx / cItems;
                if ((idx < 0) && (idxData != 0))
                {
                    nGeneration--;
                }
            }
        }

        public static bool IsNullOrEmpty(IList list)
        {
            if (list != null)
            {
                return (list.Count <= 0);
            }
            return true;
        }

        public static bool IsValidIndex(IList list, int idx)
        {
            if (list == null)
            {
                throw new ArgumentNullException("Specified a null list");
            }
            return IsValidIndex(idx, list.Count);
        }

        public static bool IsValidIndex(int idx, int cItems)
        {
            int num = cItems - 1;
            return ((idx >= 0) && (idx <= num));
        }
    }
}
