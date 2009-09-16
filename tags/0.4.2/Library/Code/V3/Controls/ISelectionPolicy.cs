using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Code.V3
{
    public interface ISelectionPolicy
    {
        // Methods
        bool AllowSetSelection(ISelectable item, bool select);
        void OnItemGainFocus(ISelectable item);
        void OnItemLostFocus(ISelectable item);
        void OnSetSelection(ISelectable item, bool select);

        // Properties
        ISelectable CurrentSelection { get; }
        int SelectionIndex { get; set; }
    }
}
