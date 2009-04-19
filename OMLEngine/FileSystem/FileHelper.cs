using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OMLEngine.FileSystem
{
    public static class FileHelper
    {
        private static readonly string[] BAD_FOLDER_CHARS = new string[] { "\\", "/", ":", "*", "?", "/", "<", ">", "|" };

        #region File Interop
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
        const int FO_DELETE = 3;
        const int FOF_ALLOWUNDO = 0x40;
        const int FOF_NOCONFIRMATION = 0x10; //Don't prompt the user.; 

        public static void DeleteFileToRecycleBin(string filename)
        {
            SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT();
            shf.wFunc = FO_DELETE;
            shf.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;
            shf.pFrom = filename + "\0";
            int result = SHFileOperation(ref shf);

            if (result != 0)
                Utilities.DebugLine("error: {0} while moving file {1} to recycle bin", result, filename);
        }
        #endregion

        /// <summary>
        /// Returns a valid folder name string for the given string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFolderNameString(string name)
        {
            StringBuilder sb = new StringBuilder(name);

            foreach(string badChar in BAD_FOLDER_CHARS)
                sb.Replace(badChar, " ");

            sb.Replace("  ", " ");

            return sb.ToString();
        }        

        /// <summary>
        /// Returns the name to use for the title for the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetNameFromPath(string path)
        {            
            // right now we'll use the parant directory name - 
            // later we can be smarter about it
            DirectoryInfo info = new DirectoryInfo(path);

            // if we're a file grab the parent folder
            string name = (File.Exists(path)) ? info.Parent.Name : info.Name;                                   

            return name.Replace("  ", " ");
        }

        /*
        public string PathSafeName
        {
            get
            {
                string chars = string.Empty;
                foreach (Char ch in Path.GetInvalidFileNameChars())
                {
                    int ach = (int)ch;
                    chars += String.Format(@"\x{0}|", ach.ToString(@"x").PadLeft(2, '0'));
                }
                if (chars.Length > 0)
                    chars = chars.Remove(chars.Length - 1);
                string rslt = System.Text.RegularExpressions.Regex.Replace(Name, chars, "");
                return rslt;
            }
        }*/        
    }
}
