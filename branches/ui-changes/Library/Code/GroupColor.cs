using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;

namespace Library
{
    public class GroupColor : ModelItem
    {
        Color _currentColor = new Color(255,0,0);

        public Color Color
        {
            get { return _currentColor; }
            set
            {
                _currentColor = value;
                FirePropertyChanged("Color");
            }
        }
    }
}
