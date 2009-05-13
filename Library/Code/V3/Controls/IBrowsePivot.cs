using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Collections;

namespace Library.Code.V3
{
    public interface IBrowsePivot : IBrowseSearchList
    {
        // Properties
        //ContentMapping[] AdditionalContentItemMappings { get; }
        Boolean SupportsItemContext { get; set; }
        Boolean IsBusy { get; set; }
        IList Content { get; }
        string ContentItemTemplate { get; }
        string ContentLabel { get; }
        ISelectionPolicy ContentSelectionPolicy { get; }
        string ContentTemplate { get; }
        string Description { get; }
        string DetailTemplate { get; }
        string EmptyContentText { get; }
        Choice SortOptions { get; }
        ContextMenuData ContextMenu { get; }
    }
}
