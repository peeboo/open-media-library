using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Library.Code.V3
{
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
