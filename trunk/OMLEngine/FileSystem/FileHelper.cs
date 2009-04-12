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

        /// <summary>
        /// Given a path turns the image into a byte array.  Will return NULL if the operation cannot be done. 
        /// The image is expected to be a JPG
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(string imagePath)
        {
            byte[] returnArray = null;
            if (!string.IsNullOrEmpty(imagePath) &&
                    File.Exists(imagePath))
            {
                try
                {
                    using (Image image = Image.FromFile(imagePath))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            returnArray = ms.ToArray();
                        }
                    }
                }
                catch(Exception ex)
                {
                    Utilities.DebugLine("[FileHelper.ImageToByteArray] Exception: " + ex.Message);
                    returnArray = null;
                }
            }

            return returnArray;
        }

        public static Image ByteArrayToImage(byte[] array)
        {
            Image image = null;

            try
            {
                image = Image.FromStream(new MemoryStream(array));
            }
            catch(Exception ex)
            {
                Utilities.DebugLine("[FileHelper.ByteArrayToImage] Exception: " + ex.Message);
                image = null;
            }

            return image;
        }

        /// <summary>
        /// Given an id, will return the image path.  Will return NULL if it can't be found.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetImagePathFromId(int id)
        {
            try
            {
                // first check to see if the image exists in our cache folder
                string imagePath = Path.Combine(FileSystemWalker.ImageDirectory, "ch" + id.ToString() + ".jpg");

                if (File.Exists(imagePath))
                    return imagePath;

                // if it's not cached - check SQL
                using (Image image = ImageManager.GetImageById(id))
                {

                    if (image == null)
                        return null;

                    // save the image
                    image.Save(imagePath);

                    return imagePath;
                }
            }
            catch(Exception ex)
            {
                Utilities.DebugLine("[FileHelper.GetImagePathFromId] Exception: " + ex.Message);
                return null;
            }
        }

        public static void DeleteCachedImage(int id)
        {
            try
            {
                string imagePath = Path.Combine(FileSystemWalker.ImageDirectory, "ch" + id.ToString() + ".jpg");

                if (File.Exists(imagePath))
                    File.Delete(imagePath);

            }
            catch(Exception ex)
            {
                Utilities.DebugLine("[FileHelper.DeleteCachedImage] Exception: " + ex.Message);
            }
        }
    }
}
