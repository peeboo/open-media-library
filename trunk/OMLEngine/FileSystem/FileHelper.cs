using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace OMLEngine.FileSystem
{
    public static class FileHelper
    {
        private static readonly string[] BAD_FOLDER_CHARS = new string[] { "\\", "/", ":", "*", "?", "/", "<", ">", "|" };

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
