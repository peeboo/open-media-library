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
    public class WizardGalleryPage : GalleryPage
    {
        public WizardGalleryPage(List<OMLEngine.TitleFilter> filter, string galleryName)
            : base(filter, galleryName)
        {
        }
    }
}
