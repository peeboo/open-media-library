using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Code.V3
{
    public interface ISelectable
    {
        // Events
        event EventHandler FocusRequested;

        // Methods
        void RequestFocus();

        // Properties
        bool Selected { get; set; }
        ISelectionPolicy SelectionPolicy { get; set; }
    }
}
