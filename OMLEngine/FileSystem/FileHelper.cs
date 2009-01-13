using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OMLEngine.FileSystem
{
    public static class FileHelper
    {
        /// <summary>
        /// Returns a valid folder name string for the given string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFolderNameString(string name)
        {
            StringBuilder sb = new StringBuilder(name);

            sb.Replace("\\", "");
            sb.Replace("/", "");
            sb.Replace(":", "");
            sb.Replace("*", "");
            sb.Replace("?", "");
            sb.Replace("\"", "");
            sb.Replace("<", "");
            sb.Replace(">", "");
            sb.Replace("|", "");

            return sb.ToString();
        }        

        /// <summary>
        /// Returns the name to use for the file for the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetNameFromPath(string path)
        {
            // right now we'll use the parant directory name - 
            // later we can be smarter about it
            DirectoryInfo info = new DirectoryInfo(path);

            return info.Name.Replace("  ", " ");
        }
    }
}
