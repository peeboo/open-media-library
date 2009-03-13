using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Maximizer
{
    /// <summary>
    /// This class is used to handle Windows of external 
    /// applications.
    /// 
    /// Version 1.0.1
    /// </summary>
    public class WindowHandler
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WindowHandler()
        {
        }

        // constants
        private const int WM_CLOSE = 0x0010;

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;


        /// <summary>
        /// Send a Windows Message
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int SendMessage(int hWnd,
            int Msg,
            int wParam,
            int lParam);

        /// <summary>
        /// Get the foreground window
        /// </summary>
        /// <returns>windowhandle</returns>
        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        /// <summary>
        /// Show a Window
        /// </summary>
        /// <param name="hwnd">windowhandle</param>
        /// <param name="nCmdShow">parameter to set a special state</param>
        /// <returns></returns>
        [DllImport("user32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);


        /// <summary>
        /// Get the Window for a special process
        /// </summary>
        /// <param name="processName">Name of the process</param>
        /// <returns>Process-Handle</returns>
        public Int32 GetWnd(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
                return -1;

            return processes[0].MainWindowHandle.ToInt32();
        }

        /// <summary>
        /// Get the  windowhandle of the window in the foreground
        /// </summary>
        /// <returns>windowhandle</returns>
        public Int32 GetForegroundWnd()
        {
            return GetForegroundWindow();
        }

        /// <summary>
        /// Close the MainWindow of an a process
        /// </summary>
        public void Close(Int32 handle)
        {
            SendMessage(handle, WM_CLOSE, 0, 0);
        }

        /// <summary>
        /// Minimize the Window
        /// </summary>
        /// <param name="handle">windowhandle</param>
        public void Minimize(Int32 handle)
        {
            ShowWindow(handle, SW_SHOWMINIMIZED);
        }

        /// <summary>
        /// Maximize the Window
        /// </summary>
        /// <param name="handle">windowhandle</param>
        public void Maximize(Int32 handle)
        {
            ShowWindow(handle, SW_SHOWMAXIMIZED);
        }

        /// <summary>
        /// "Normalize" the Window
        /// </summary>
        /// <param name="handle">windowhandle</param>
        public void Normalize(Int32 handle)
        {
            ShowWindow(handle, SW_SHOWNORMAL);
        }
    }
}