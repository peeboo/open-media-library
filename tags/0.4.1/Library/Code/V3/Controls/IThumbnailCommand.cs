using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Collections;

namespace Library.Code.V3
{
    public interface IThumbnailCommand : ICommand, IModelItem, IPropertyObject, IModelItemOwner
    {
        // Properties
        Image DefaultImage { get; }
        Image FocusImage { get; }
        Image DormantImage { get; }

    }
}
