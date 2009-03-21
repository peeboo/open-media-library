using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Library.Code.V3
{
    [MarkupVisible]
    public class ScrollingDataHelper : BaseModelItem
    {
        private bool _loaded = false;

        public bool Loaded
        {
            get { return _loaded; }
            set { _loaded = value; }
        }

        public void SetScrollIntoViewDisposition(object ScrollingData, object ScrollIntoViewDisposition)
        {
//            Type type = ScrollingData.GetType();
//            PropertyInfo pi = null;
//            while (type != null && pi != null)
//            {
//                pi = type.GetProperty("ScrollIntoViewDisposition", BindingFlags.FlattenHierarchy |
//BindingFlags.IgnoreCase |
//BindingFlags.Instance |
//BindingFlags.NonPublic |
//BindingFlags.Public); // params omitted for brevity, using DeclaredOnly 
//                type = type.BaseType;
//            }


            if (_loaded == false)
            {
                _loaded = true;
                PrivateObjectWeasel weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.ScrollIntoViewDisposition, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                float f = 0;
                weasel.SetProperty("LockedPosition", f);
                weasel.SetProperty("LockedAlignment", f);

                MethodInfo mi = ScrollingData.GetType().GetMethod("set_ScrollIntoViewDisposition",
    BindingFlags.FlattenHierarchy |
    BindingFlags.IgnoreCase |
    BindingFlags.Instance |
    BindingFlags.NonPublic |
    BindingFlags.Public);
                mi.Invoke(ScrollingData, new object[] { weasel.Instance });

                //Type myType = ScrollingData.GetType();
                //PropertyInfo pinfo = myType.GetProperty("ScrollIntoViewDisposition", weasel.Instance.GetType());

                //pinfo.SetValue(ScrollingData, weasel.Instance, null);
                //pinfo.SetValue(ScrollingData, PipVisualState.Inactive, null);
            }
        }
    }

    public class ScrollIntoViewDisposition
    {
        // Fields
        private const float k_flUnLockedFocusPosition = -1f;
        private bool m_fEnabled;
        private float m_flLockedAlignment;
        private float m_flLockedPosition;
        private int m_nBeginPadding;
        private int m_nEndPadding;
        private RelativeEdge m_relativeBeginPadding;
        private RelativeEdge m_relativeEndPadding;

        // Methods
        public ScrollIntoViewDisposition()
            : this(0)
        {
        }

        public ScrollIntoViewDisposition(int nPadding)
        {
            this.m_nBeginPadding = this.m_nEndPadding = nPadding;
            this.m_flLockedPosition = -1f;
            this.m_flLockedAlignment = 0.5f;
            this.m_fEnabled = true;
            this.m_relativeBeginPadding = RelativeEdge.Near;
            this.m_relativeEndPadding = RelativeEdge.Far;
        }

        public override string ToString()
        {
            string text = InvariantString.Format("{0}(", new object[] { base.GetType().Name });
            if (!this.m_fEnabled)
            {
                text = InvariantString.Format("{0}Disabled", new object[] { text });
            }
            else
            {
                text = InvariantString.Format("{0}(BeginPadding={1}({2}), EndPadding={3}({4})", new object[] { text, this.m_nBeginPadding, this.m_relativeBeginPadding, this.m_nEndPadding, this.m_relativeEndPadding });
                if (this.Locked)
                {
                    text = InvariantString.Format("{0}, LockedPosition={1}, LockedAlignment={2}", new object[] { text, this.m_flLockedPosition, this.m_flLockedAlignment });
                }
            }
            return (text + ")");
        }

        // Properties
        [DefaultValue(0)]
        public int BeginPadding
        {
            get
            {
                return this.m_nBeginPadding;
            }
            set
            {
                this.m_nBeginPadding = value;
            }
        }

        [DefaultValue(0)]
        public RelativeEdge BeginPaddingRelativeTo
        {
            get
            {
                return this.m_relativeBeginPadding;
            }
            set
            {
                this.m_relativeBeginPadding = value;
            }
        }

        [DefaultValue(true)]
        public bool Enabled
        {
            get
            {
                return this.m_fEnabled;
            }
            set
            {
                this.m_fEnabled = value;
            }
        }

        [DefaultValue(0)]
        public int EndPadding
        {
            get
            {
                return this.m_nEndPadding;
            }
            set
            {
                this.m_nEndPadding = value;
            }
        }

        [DefaultValue(1)]
        public RelativeEdge EndPaddingRelativeTo
        {
            get
            {
                return this.m_relativeEndPadding;
            }
            set
            {
                this.m_relativeEndPadding = value;
            }
        }

        public bool Locked
        {
            get
            {
                return (this.m_flLockedPosition != -1f);
            }
        }

        [DefaultValue((float)0.5f)]
        public float LockedAlignment
        {
            get
            {
                return this.m_flLockedAlignment;
            }
            set
            {
                this.m_flLockedAlignment = value;
            }
        }

        [DefaultValue((float)-1f)]
        public float LockedPosition
        {
            get
            {
                return this.m_flLockedPosition;
            }
            set
            {
                this.m_flLockedPosition = value;
            }
        }

        public int Padding
        {
            get
            {
                return this.BeginPadding;
            }
            set
            {
                this.BeginPadding = this.EndPadding = value;
            }
        }

        public static float UnLockedFocusPosition
        {
            get
            {
                return -1f;
            }
        }
    }
}
