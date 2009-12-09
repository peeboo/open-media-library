using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace OMLDatabaseEditor.Controls
{
    public class OMLListView : System.Windows.Forms.ListView
    {
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;

        private const int SB_LINELEFT = 0;
        private const int SB_LINERIGHT = 1;
        private const int SB_PAGELEFT = 2;
        private const int SB_PAGERIGHT = 3;
        private const int SB_THUMBPOSITION = 4;
        private const int SB_THUMBTRACK = 5;
        private const int SB_LEFT = 6;
        private const int SB_RIGHT = 7;
        private const int SB_ENDSCROLL = 8;

        private const int SIF_TRACKPOS = 0x10;
        private const int SIF_RANGE = 0x1;
        private const int SIF_POS = 0x4;
        private const int SIF_PAGE = 0x2;
        private const int SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS;

        public event EventHandler Scroll;
        public event EventHandler ScrollUp;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetScrollInfo(IntPtr hWnd, int n, ref ScrollInfoStruct lpScrollInfo);

        private struct ScrollInfoStruct
        {
            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        protected void OnScroll()
        {
            if (this.Scroll != null) this.Scroll(this, EventArgs.Empty);
        }

        protected void OnScrollUp()
        {
            if (this.ScrollUp != null) this.ScrollUp(this, EventArgs.Empty);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_VSCROLL)
            {

                ScrollInfoStruct si = new ScrollInfoStruct();
                si.fMask = SIF_ALL;
                si.cbSize = (int)Marshal.SizeOf(si);
                GetScrollInfo(m.HWnd, SB_VERT, ref si);
                if (m.WParam.ToInt32() == SB_ENDSCROLL)
                {
                    ScrollEventArgs sargs = new ScrollEventArgs(ScrollEventType.EndScroll, si.nPos);
                    this.OnScrollUp();
                }
                else this.OnScroll();
            }
            else if (m.Msg == WM_HSCROLL) this.OnScroll();
        }
    }
}