using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.MediaCenter.Hosting;
using System.Diagnostics;

namespace Library.Code.V3
{
    public enum InputDevice
    {
        Key,
        Mouse,
        OEM
    }


    public enum RemoteControlButton
    {
        Clear,
        Down,
        Left,
        Digit0,
        Digit1,
        Digit2,
        Digit3,
        Digit4,
        Digit5,
        Digit6,
        Digit7,
        Digit8,
        Digit9,
        Enter,
        Right,
        Up,

        Back,
        ChannelDown,
        ChannelUp,
        FastForward,
        VolumeMute,
        Pause,
        Play,
        Record,
        PreviousTrack,
        Rewind,
        NextTrack,
        Stop,
        VolumeDown,
        VolumeUp,

        RecordedTV,
        Guide,
        LiveTV,
        Details,
        DVDMenu,
        DVDAngle,
        DVDAudio,
        DVDSubtitle,
        MyMusic,
        MyPictures,
        MyVideos,
        MyTV,
        OEM1,
        OEM2,
        StandBy,
        TVJump,

        Unknown
    }


    #region RemoteControlEventArgs

    public class RemoteControlEventArgs : EventArgs
    {
        RemoteControlButton _rcb;
        InputDevice _device;

        public RemoteControlEventArgs(RemoteControlButton rcb, InputDevice device)
        {
            _rcb = rcb;
            _device = device;
        }


        public RemoteControlEventArgs()
        {
            _rcb = RemoteControlButton.Unknown;
            _device = InputDevice.Key;
        }


        public RemoteControlButton Button
        {
            get { return _rcb; }
            set { _rcb = value; }
        }

        public InputDevice Device
        {
            get { return _device; }
            set { _device = value; }
        }
    }

    #endregion


    class MoreInfoHooker2 : NativeWindow, IDisposable
    {
        private static LowLevelProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static LowLevelProc kbProc = KBHookCallback;
        private static IntPtr kbHookID = IntPtr.Zero;

        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr SetHook(LowLevelProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr SetKBHook(LowLevelProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr KBHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine(vkCode);
                if (Keys.D == (Keys)vkCode && Keys.Control == Control.ModifierKeys)
                {
                    RemoteControlButton rcb = RemoteControlButton.Details;
                    //OMLApplication.Current.CatchMoreInfo();
                    OMLApplication.Current.CatchMoreInfo();
                    SendMoreInfo();
                    return new IntPtr(1);
                    //if (this.ButtonPressed != null && rcb != RemoteControlButton.Unknown)
                    //    this.ButtonPressed(this, new RemoteControlEventArgs(rcb, InputDevice.Key));
                }
                else if (vkCode == WM_CONTEXTMENU || vkCode == 93 || vkCode == VK_APPS)
                {
                    RemoteControlButton rcb = RemoteControlButton.Details;
                    OMLApplication.Current.CatchMoreInfo();
                    SendMoreInfo();
                    return new IntPtr(1);
                }
                //{
                //    RemoteControlButton rcb = RemoteControlButton.Details;
                //    //OMLApplication.Current.CatchMoreInfo();
                //    SendMoreInfo();
                //    //if (this.ButtonPressed != null && rcb != RemoteControlButton.Unknown)
                //    //    this.ButtonPressed(this, new RemoteControlEventArgs(rcb, InputDevice.Key));
                //}
                //if (Keys.Back == (Keys)vkCode || Keys.BrowserBack == (Keys)vkCode)
                //{
                //    RemoteControlButton rcb = RemoteControlButton.Back;

                //    //Application.Current.DialogTest("TestHarness");
                //    Application.Current.GoBack();

                //    //if (this.ButtonPressed != null && rcb != RemoteControlButton.Unknown)
                //    //    this.ButtonPressed(this, new RemoteControlEventArgs(rcb, InputDevice.Key));
                //}
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine(vkCode);
                if (Keys.D == (Keys)vkCode && Keys.Control == Control.ModifierKeys)
                {
                    return new IntPtr(1);
                }
                else if (vkCode == WM_CONTEXTMENU || vkCode == 93 || vkCode == VK_APPS)
                {
                    return new IntPtr(1);
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYPRESS)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine(vkCode);
                if (Keys.D == (Keys)vkCode && Keys.Control == Control.ModifierKeys)
                {
                    return new IntPtr(1);
                }
                else if (vkCode == WM_CONTEXTMENU || vkCode == 93 || vkCode == VK_APPS)
                {
                    return new IntPtr(1);
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine(vkCode);
                if (Keys.D == (Keys)vkCode && Keys.Control == Control.ModifierKeys)
                {
                    return new IntPtr(1);
                }
                else if (vkCode == WM_CONTEXTMENU || vkCode == 93 || vkCode == VK_APPS)
                {
                    return new IntPtr(1);
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_SYSKEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine(vkCode);
                if (Keys.D == (Keys)vkCode && Keys.Control == Control.ModifierKeys)
                {
                    return new IntPtr(1);
                }
                else if (vkCode == WM_CONTEXTMENU || vkCode == 93 || vkCode == VK_APPS)
                {
                    return new IntPtr(1);
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_SYSKEYPRESS)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Console.WriteLine(vkCode);
                if (Keys.D == (Keys)vkCode && Keys.Control == Control.ModifierKeys)
                {
                    return new IntPtr(1);
                }
                else if (vkCode == WM_CONTEXTMENU || vkCode == 93 || vkCode==VK_APPS)
                {
                    return new IntPtr(1);
                }
            }
            return CallNextHookEx(kbHookID, nCode, wParam, lParam);
            //return new IntPtr(1);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //IntPtr activeWindowHandle = GetForegroundWindow();
            //Process curProcess = Process.GetCurrentProcess();
            //if (activeWindowHandle == curProcess.MainWindowHandle)
            //{

                if (nCode >= 0 &&
                MouseMessages.WM_RBUTTONDOWN == (MouseMessages)wParam)
                {
                    return new IntPtr(1);
                }
                if (nCode >= 0 &&
                MouseMessages.WM_RBUTTONUP == (MouseMessages)wParam)
                {
                    MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    //Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
                    RemoteControlButton rcb = RemoteControlButton.Details;
                    OMLApplication.Current.CatchMoreInfo();
                    SendMoreInfo();
                    //if (this.ButtonPressed != null && rcb != RemoteControlButton.Unknown)
                    //    this.ButtonPressed(this, new RemoteControlEventArgs(rcb, InputDevice.Mouse));
                    return new IntPtr(1);
                }
            //}
            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        }

        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// //////////////////////////
        /// </summary>

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDEVICE
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsage;
            [MarshalAs(UnmanagedType.U4)]
            public RIDEV dwFlags;
            public IntPtr hwndTarget;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTHEADER
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSize;
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int wParam;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWHID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizHid;
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct BUTTONSSTR
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonFlags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonData;
        }


        [StructLayout(LayoutKind.Explicit)]
        internal struct RAWMOUSE
        {
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(0)]
            public ushort usFlags;
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            public uint ulButtons;
            [FieldOffset(4)]
            public BUTTONSSTR buttonsStr;
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(8)]
            public uint ulRawButtons;
            [FieldOffset(12)]
            public int lLastX;
            [FieldOffset(16)]
            public int lLastY;
            [MarshalAs(UnmanagedType.U2)]
            [FieldOffset(20)]
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWKEYBOARD
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort MakeCode;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Flags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort Reserved;
            [MarshalAs(UnmanagedType.U2)]
            public ushort VKey;
            [MarshalAs(UnmanagedType.U4)]
            public uint Message;
            [MarshalAs(UnmanagedType.U4)]
            public uint ExtraInformation;
        }


        [StructLayout(LayoutKind.Explicit)]
        internal struct RAWINPUT
        {
            [FieldOffset(0)]
            public RAWINPUTHEADER header;
            //[FieldOffset (16)] public RAWMOUSE mouse;
            //[FieldOffset (16)] public RAWKEYBOARD keyboard;
            [FieldOffset(16)]
            public RAWHID hid;
            //could be [FieldOffset(24)] public RAWHID hid;
            //http://discuss.mediacentersandbox.com/forums/thread/8566.aspx
        }


        [DllImport("User32.dll")]
        extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

        [DllImport("User32.dll")]
        extern static uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        public const int VK_APPS = 0x5d;
 
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x101;
        private const int WM_KEYPRESS = 0x102; 
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private const int WM_SYSKEYPRESS = 0x0106;
        private const int WM_APPCOMMAND = 0x0319;
        private const int WM_INPUT = 0x00FF;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_CONTEXTMENU = 0x7b;

        private const int APPCOMMAND_BROWSER_BACKWARD = 1;
        private const int APPCOMMAND_VOLUME_MUTE = 8;
        private const int APPCOMMAND_VOLUME_DOWN = 9;
        private const int APPCOMMAND_VOLUME_UP = 10;
        private const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
        private const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
        private const int APPCOMMAND_MEDIA_STOP = 13;
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
        private const int APPCOMMAND_MEDIA_PLAY = 46;
        private const int APPCOMMAND_MEDIA_PAUSE = 47;
        private const int APPCOMMAND_MEDIA_RECORD = 48;
        private const int APPCOMMAND_MEDIA_FAST_FORWARD = 49;
        private const int APPCOMMAND_MEDIA_REWIND = 50;
        private const int APPCOMMAND_MEDIA_CHANNEL_UP = 51;
        private const int APPCOMMAND_MEDIA_CHANNEL_DOWN = 52;

        private const int RAWINPUT_DETAILS = 0x209;
        private const int RAWINPUT_GUIDE = 0x8D;
        private const int RAWINPUT_TVJUMP = 0x25;
        private const int RAWINPUT_STANDBY = 0x82;
        private const int RAWINPUT_OEM1 = 0x80;
        private const int RAWINPUT_OEM2 = 0x81;
        private const int RAWINPUT_MYTV = 0x46;
        private const int RAWINPUT_MYVIDEOS = 0x4A;
        private const int RAWINPUT_MYPICTURES = 0x49;
        private const int RAWINPUT_MYMUSIC = 0x47;
        private const int RAWINPUT_RECORDEDTV = 0x48;
        private const int RAWINPUT_DVDANGLE = 0x4B;
        private const int RAWINPUT_DVDAUDIO = 0x4C;
        private const int RAWINPUT_DVDMENU = 0x24;
        private const int RAWINPUT_DVDSUBTITLE = 0x4D;

        private const int RIM_TYPEMOUSE = 0;
        private const int RIM_TYPEKEYBOARD = 1;
        private const int RIM_TYPEHID = 2;

        private const int RID_INPUT = 0x10000003;
        private const int RID_HEADER = 0x10000005;

        private const int FAPPCOMMAND_MASK = 0xF000;
        private const int FAPPCOMMAND_MOUSE = 0x8000;
        private const int FAPPCOMMAND_KEY = 0;
        private const int FAPPCOMMAND_OEM = 0x1000;

        public delegate void RemoteControlDeviceEventHandler(object sender, RemoteControlEventArgs e);
        public event RemoteControlDeviceEventHandler ButtonPressed;


        //-------------------------------------------------------------
        // constructors
        //-------------------------------------------------------------

        public MoreInfoHooker2()
        {
            this.CreateHandle(new CreateParams());
            _hookID = SetHook(_proc);
            kbHookID = SetKBHook(kbProc);
            // Register the input device to receive the commands from the 
            // remote device. See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnwmt/html/remote_control.asp
            // for the vendor defined usage page.

            //RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[3];

            //rid[0].usUsagePage = 0xFFBC;
            //rid[0].usUsage = 0x88;
            //rid[0].dwFlags = 0;

            //rid[1].usUsagePage = 0x0C;
            //rid[1].usUsage = 0x01;
            //rid[1].dwFlags = 0;

            //rid[2].usUsagePage = 0x0C;
            //rid[2].usUsage = 0x80;
            //rid[2].dwFlags = 0;

            //if (!RegisterRawInputDevices(rid, 
            //    (uint) rid.Length, 
            //    (uint) Marshal.SizeOf(rid[0]))
            //    )
            //{
            //    throw new ApplicationException("Failed to register raw input devices.");
            //}

            RAWINPUTDEVICE[] pRawInputDevice = new RAWINPUTDEVICE[4];
            //pRawInputDevice[0].usUsagePage = 0xffbc;
            //pRawInputDevice[0].usUsage = 0x88;
            //pRawInputDevice[0].dwFlags = RIDEV.INPUTSINK;
            //pRawInputDevice[0].hwndTarget = base.Handle;
            //pRawInputDevice[1].usUsagePage = 12;
            //pRawInputDevice[1].usUsage = 1;
            //pRawInputDevice[1].dwFlags = RIDEV.INPUTSINK;
            //pRawInputDevice[1].hwndTarget = base.Handle;
            //pRawInputDevice[2].usUsagePage = 12;
            //pRawInputDevice[2].usUsage = 0x80;
            //pRawInputDevice[2].dwFlags = RIDEV.INPUTSINK;
            //pRawInputDevice[2].hwndTarget = base.Handle;
            //pRawInputDevice[3].usUsagePage = 1;
            //pRawInputDevice[3].usUsage = 6;
            //pRawInputDevice[3].dwFlags = RIDEV.INPUTSINK;
            //pRawInputDevice[3].hwndTarget = base.Handle;

            pRawInputDevice[0].usUsagePage = 0xFFBC;      // adds HID remote control
            pRawInputDevice[0].usUsage = 0x88;
            pRawInputDevice[0].dwFlags = RIDEV.INPUTSINK;
            pRawInputDevice[0].hwndTarget = base.Handle;

            pRawInputDevice[1].usUsagePage = 0x0C;      // adds HID remote control
            pRawInputDevice[1].usUsage = 0x01;
            pRawInputDevice[1].dwFlags = RIDEV.INPUTSINK;
            pRawInputDevice[1].hwndTarget = base.Handle;

            pRawInputDevice[2].usUsagePage = 0x0C;      // adds HID remote control
            pRawInputDevice[2].usUsage = 0x80;
            pRawInputDevice[2].dwFlags = RIDEV.INPUTSINK;
            pRawInputDevice[2].hwndTarget = base.Handle;

            //pRawInputDevice[3].usUsagePage = 0x01;      // adds HID mouse with no legacy messages
            //pRawInputDevice[3].usUsage = 0x02;
            //pRawInputDevice[3].dwFlags = RIDEV.NOLEGACY;
            ////pRawInputDevice[3].hwndTarget = base.Handle;

            pRawInputDevice[3].usUsagePage = 0x01;      // adds HID keyboard with no legacy message
            pRawInputDevice[3].usUsage = 0x06;
            pRawInputDevice[3].dwFlags = RIDEV.NOLEGACY;

            if (!RegisterRawInputDevices(pRawInputDevice, (uint)pRawInputDevice.Length, (uint)Marshal.SizeOf(pRawInputDevice[0])))
            {
                //throw new ApplicationException("Failed to register raw input device(s).");
            }
        }

        public enum RIDEV
        {
            APPKEYS = 0x400,
            CAPTUREMOUSE = 0x200,
            EXMODEMASK = 240,
            INPUTSINK = 0x100,
            NOHOTKEYS = 0x200,
            NOLEGACY = 0x30,
            PAGEONLY = 0x20,
            REMOVE = 1
        }

        //-------------------------------------------------------------
        // methods
        //-------------------------------------------------------------

        protected override void WndProc(ref Message message)
        {
            try
            {
                AddInHost host = AddInHost.Current;
                if (host.ApplicationContext.IsForegroundApplication)
                {
                    int param;
                    Console.WriteLine(message.Msg.ToString());
                    switch (message.Msg)
                    {
                        case WM_KEYDOWN:
                            param = message.WParam.ToInt32();
                            ProcessKeyDown(param);
                            break;
                        case WM_APPCOMMAND:
                            param = message.LParam.ToInt32();
                            ProcessAppCommand(param);
                            break;
                        case WM_INPUT:
                            ProcessInputCommand(ref message);
                            break;
                        //case WM_RBUTTONUP:
                        //    ProcessMouseCommand();
                        //    break;
                    }
                }
            }
            finally { }
            base.WndProc(ref message);
        }

        private void ProcessMouseCommand()
        {
            RemoteControlButton rcb = RemoteControlButton.Details;

            if (this.ButtonPressed != null && rcb != RemoteControlButton.Unknown)
                this.ButtonPressed(this, new RemoteControlEventArgs(rcb, InputDevice.Mouse));
        }


        //-------------------------------------------------------------
        // methods (helpers)
        //-------------------------------------------------------------

        private void ProcessKeyDown(int param)
        {
            RemoteControlButton rcb = RemoteControlButton.Unknown;

            switch (param)
            {
                case (int)Keys.Escape:
                    rcb = RemoteControlButton.Clear;
                    break;
                case (int)Keys.Down:
                    rcb = RemoteControlButton.Down;
                    break;
                case (int)Keys.Left:
                    rcb = RemoteControlButton.Left;
                    break;
                case (int)Keys.D0:
                    rcb = RemoteControlButton.Digit0;
                    break;
                case (int)Keys.D1:
                    rcb = RemoteControlButton.Digit1;
                    break;
                case (int)Keys.D2:
                    rcb = RemoteControlButton.Digit2;
                    break;
                case (int)Keys.D3:
                    rcb = RemoteControlButton.Digit3;
                    break;
                case (int)Keys.D4:
                    rcb = RemoteControlButton.Digit4;
                    break;
                case (int)Keys.D5:
                    rcb = RemoteControlButton.Digit5;
                    break;
                case (int)Keys.D6:
                    rcb = RemoteControlButton.Digit6;
                    break;
                case (int)Keys.D7:
                    rcb = RemoteControlButton.Digit7;
                    break;
                case (int)Keys.D8:
                    rcb = RemoteControlButton.Digit8;
                    break;
                case (int)Keys.D9:
                    rcb = RemoteControlButton.Digit9;
                    break;
                case (int)Keys.Enter:
                    rcb = RemoteControlButton.Enter;
                    break;
                case (int)Keys.Right:
                    rcb = RemoteControlButton.Right;
                    break;
                case (int)Keys.Up:
                    rcb = RemoteControlButton.Up;
                    break;
                case (int)Keys.Back:
                    rcb = RemoteControlButton.Back;
                    break;
                case (int)Keys.BrowserBack:
                    rcb = RemoteControlButton.Back;
                    break;
            }

            if (this.ButtonPressed != null && rcb != RemoteControlButton.Unknown)
                this.ButtonPressed(this, new RemoteControlEventArgs(rcb, GetDevice(param)));
        }


        private void ProcessAppCommand(int param)
        {
            RemoteControlButton rcb = RemoteControlButton.Unknown;

            int cmd = (int)(((ushort)(param >> 16)) & ~FAPPCOMMAND_MASK);

            switch (cmd)
            {
                case APPCOMMAND_BROWSER_BACKWARD:
                    rcb = RemoteControlButton.Back;
                    break;
                case APPCOMMAND_MEDIA_CHANNEL_DOWN:
                    rcb = RemoteControlButton.ChannelDown;
                    break;
                case APPCOMMAND_MEDIA_CHANNEL_UP:
                    rcb = RemoteControlButton.ChannelUp;
                    break;
                case APPCOMMAND_MEDIA_FAST_FORWARD:
                    rcb = RemoteControlButton.FastForward;
                    break;
                case APPCOMMAND_VOLUME_MUTE:
                    rcb = RemoteControlButton.VolumeMute;
                    break;
                case APPCOMMAND_MEDIA_PAUSE:
                    rcb = RemoteControlButton.Pause;
                    break;
                case APPCOMMAND_MEDIA_PLAY:
                    rcb = RemoteControlButton.Play;
                    break;
                case APPCOMMAND_MEDIA_RECORD:
                    rcb = RemoteControlButton.Record;
                    break;
                case APPCOMMAND_MEDIA_PREVIOUSTRACK:
                    rcb = RemoteControlButton.PreviousTrack;
                    break;
                case APPCOMMAND_MEDIA_REWIND:
                    rcb = RemoteControlButton.Rewind;
                    break;
                case APPCOMMAND_MEDIA_NEXTTRACK:
                    rcb = RemoteControlButton.NextTrack;
                    break;
                case APPCOMMAND_MEDIA_STOP:
                    rcb = RemoteControlButton.Stop;
                    break;
                case APPCOMMAND_VOLUME_DOWN:
                    rcb = RemoteControlButton.VolumeDown;
                    break;
                case APPCOMMAND_VOLUME_UP:
                    rcb = RemoteControlButton.VolumeUp;
                    break;
            }

            if (this.ButtonPressed != null && rcb != RemoteControlButton.Unknown)
                this.ButtonPressed(this, new RemoteControlEventArgs(rcb, GetDevice(param)));
        }


        private void ProcessInputCommand(ref Message message)
        {
            RemoteControlButton rcb = RemoteControlButton.Unknown;
            uint dwSize = 0;

            GetRawInputData(message.LParam,
                RID_INPUT,
                IntPtr.Zero,
                ref dwSize,
                (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)
                ));

            IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
            try
            {
                if (buffer == IntPtr.Zero)
                    return;

                if (GetRawInputData(message.LParam,
                    RID_INPUT,
                    buffer,
                    ref dwSize,
                    (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) != dwSize
                    )
                {
                    return;
                }

                RAWINPUT raw = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));
                if (raw.header.dwType == RIM_TYPEHID)
                {
                    byte[] bRawData = new byte[raw.hid.dwSizHid];
                    int pRawData = buffer.ToInt32() + Marshal.SizeOf(typeof(RAWINPUT)) + 1;

                    Marshal.Copy(new IntPtr(pRawData), bRawData, 0, raw.hid.dwSizHid - 1);
                    int rawData = bRawData[0] | bRawData[1] << 8;

                    switch (rawData)
                    {
                        case RAWINPUT_DETAILS:
                            rcb = RemoteControlButton.Details;
                            OMLApplication.Current.CatchMoreInfo();
                            SendMoreInfo();
                            break;
                        case RAWINPUT_GUIDE:
                            rcb = RemoteControlButton.Guide;
                            break;
                        case RAWINPUT_TVJUMP:
                            rcb = RemoteControlButton.TVJump;
                            break;
                        case RAWINPUT_STANDBY:
                            rcb = RemoteControlButton.StandBy;
                            break;
                        case RAWINPUT_OEM1:
                            rcb = RemoteControlButton.OEM1;
                            break;
                        case RAWINPUT_OEM2:
                            rcb = RemoteControlButton.OEM2;
                            break;
                        case RAWINPUT_MYTV:
                            rcb = RemoteControlButton.MyTV;
                            break;
                        case RAWINPUT_MYVIDEOS:
                            rcb = RemoteControlButton.MyVideos;
                            break;
                        case RAWINPUT_MYPICTURES:
                            rcb = RemoteControlButton.MyPictures;
                            break;
                        case RAWINPUT_MYMUSIC:
                            rcb = RemoteControlButton.MyMusic;
                            break;
                        case RAWINPUT_RECORDEDTV:
                            rcb = RemoteControlButton.RecordedTV;
                            break;
                        case RAWINPUT_DVDANGLE:
                            rcb = RemoteControlButton.DVDAngle;
                            break;
                        case RAWINPUT_DVDAUDIO:
                            rcb = RemoteControlButton.DVDAudio;
                            break;
                        case RAWINPUT_DVDMENU:
                            rcb = RemoteControlButton.DVDMenu;
                            break;
                        case RAWINPUT_DVDSUBTITLE:
                            rcb = RemoteControlButton.DVDSubtitle;
                            break;
                    }

                    if (rcb != RemoteControlButton.Unknown && this.ButtonPressed != null)
                        this.ButtonPressed(this, new RemoteControlEventArgs(rcb, GetDevice(message.LParam.ToInt32())));
                }
                else if (raw.header.dwType == RIM_TYPEMOUSE)
                {
                    // do mouse handling...	
                    //MessageBox.Show("mouse");
                }
                else if (raw.header.dwType == RIM_TYPEKEYBOARD)
                {
                    // do keyboard handling...
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }


        private InputDevice GetDevice(int param)
        {
            InputDevice inputDevice;

            switch ((int)(((ushort)(param >> 16)) & FAPPCOMMAND_MASK))
            {
                case FAPPCOMMAND_OEM:
                    inputDevice = InputDevice.OEM;
                    break;
                case FAPPCOMMAND_MOUSE:
                    inputDevice = InputDevice.Mouse;
                    break;
                default:
                    inputDevice = InputDevice.Key;
                    break;
            }

            return inputDevice;
        }

        public static void SendMoreInfo()
        {
            //SendKeys.SendWait("A");
        }
        #region IDisposable Members

        public void Dispose()
        {
            if (base.Handle != IntPtr.Zero)
            {
                this.DestroyHandle();
                UnhookWindowsHookEx(_hookID);
                UnhookWindowsHookEx(kbHookID);
            }
        }

        #endregion
    }
}
