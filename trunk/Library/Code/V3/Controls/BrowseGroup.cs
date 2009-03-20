using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Library.Code.V3
{
    public class BrowseGroup: GalleryItem, IBrowseGroup
    {
        // Fields
        private IList m_list;
        private string m_stContentLabelTemplate;

        // Methods
        //public BrowseGroup(Microsoft.MediaCenter.UI.IModelItemOwner owner, string stDescription, IList list)
        //    : base(owner, stDescription)
        //{
        //    this.m_list = list;
        //    this.m_stContentLabelTemplate = StandardContentLabelTemplate;
        //}

        // Properties
        public IList Content
        {
            get
            {
                return this.m_list;
            }
            set
            {
                this.m_list = value;
            }
        }

        public string ContentLabelTemplate
        {
            get
            {
                return this.m_stContentLabelTemplate;
            }
            set
            {
                if (this.m_stContentLabelTemplate != value)
                {
                    this.m_stContentLabelTemplate = value;
                    base.FirePropertyChanged("ContentLabelTemplate");
                }
            }
        }

        public static string StandardContentLabelTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_Controls_BrowseGroupedGallery#TextContentLabel";
            }
        }

    }

 

}
