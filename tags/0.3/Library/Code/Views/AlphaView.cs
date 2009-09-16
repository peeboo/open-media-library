using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;
using System.Threading;
using System.Linq;

namespace Library
{
    public class AlphaView : ModelItem
    {
        private IntRangedValue _focusSubIndex = new IntRangedValue();

        public IntRangedValue FocusSubIndex
        {
            get { return _focusSubIndex; }
            set { _focusSubIndex = value; }
        }

        private bool updateDefaultFocus = false;

        /// <summary>
        /// Used to fire that the default focus needs to be set
        /// </summary>
        public bool UpdateDefaultFocus
        {
            get { return updateDefaultFocus; }
            set
            {
                updateDefaultFocus = value;
                FirePropertyChanged("UpdateDefaultFocus");
            }
        }

        private IntRangedValue focusIndex = new IntRangedValue();

        public IntRangedValue FocusIndex
        {
            get { return focusIndex; }
            set
            {
                focusIndex = value;
                FirePropertyChanged("FocusIndex");
            }
        }

        public AlphaView()
        {
        }
    }
}
