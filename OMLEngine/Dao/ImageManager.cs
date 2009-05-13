using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

using Dao = OMLEngine.Dao;

namespace OMLEngine
{
    public enum ImageSize : int
    {
        Original = 0,
        Medium = 1,
        Small = 2,
    }

    public enum ImageType : byte
    {
        FrontCoverImage = 0,
        BackCoverImage = 1,
        FanartImage = 2,
        BackdropImage = 3
    }

    public static class ImageManager
    {
        private const int MAX_IMAGE_HEIGHT = 800;

        // these prefix's need to stay 2 characters long
        private static readonly string[] imagePrefixes = new string[] { "or", "me", "sm" };

        public static string NoCoverPath
        {
            get { return OMLEngine.FileSystemWalker.ImageDirectory + "\\nocover.jpg"; }
        }        
        
        public static int? AddImageToDB(string imagePath)
        {
            if (!File.Exists(imagePath))
                return null;

            int? returnId = null;

            if (!string.IsNullOrEmpty(imagePath) &&
                    File.Exists(imagePath))
            {
                try
                {
                    using (Image image = Image.FromFile(imagePath))
                    {
                        Image scaledImage = null;

                        try
                        {                            
                            // if the image is too big - scale it down
                            if (image.Height > MAX_IMAGE_HEIGHT)
                            {
                                scaledImage = ScaleImageByHeight(image, MAX_IMAGE_HEIGHT);
                            }
                            
                            byte[] imageArray = (scaledImage != null )  ? ImageManager.ImageToByteArray(scaledImage)
                                                                        : ImageManager.ImageToByteArray(image);

                            if (imageArray != null)
                            {
                                // save it to the db and store the id
                                returnId = Dao.TitleCollectionDao.AddImage(imageArray);
                            }
                        }
                        finally
                        {
                            if (scaledImage != null)
                                scaledImage.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utilities.DebugLine("[ImageManager.AddImageToDB] Exception: " + ex.Message);
                    returnId = null;
                }
            }            

            return returnId;
        }

        public static Image GetImageById(int id)
        {
            Image image = null;

            Dao.DBImage dbImage = Dao.TitleCollectionDao.GetImageBydId(id);

            if (dbImage != null)
            {
                image = ByteArrayToImage(dbImage.Image.ToArray());
            }

            return image;
        }

        /// <summary>
        /// Will return the proper image path for a given id.  If the image doesn't exist you will
        /// get the path for the NoImage image.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetImagePathById(int id, ImageSize size)
        {
            return GetImagePathById((int?)id, size);
        }

        /// <summary>
        /// Returns the image id for the given cached image url
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static int? GetIdFromImagePath(string path)
        {
            if (!path.StartsWith(FileSystemWalker.ImageDirectory))
                return null;

            string idString = path.Substring(FileSystemWalker.ImageDirectory.Length + 3).Replace(".png", "");

            int returnId;

            return (int.TryParse(idString, out returnId)) ? (int?)returnId : null;
        }

        internal static string ConstructImagePathById(int id, ImageSize size)
        {
            string prefix = null;
            int resizeSize = 0;

            switch (size)
            {
                case ImageSize.Medium:
                    prefix = imagePrefixes[(int)ImageSize.Medium];                    
                    break;

                case ImageSize.Small:
                    prefix = imagePrefixes[(int)ImageSize.Small];                    
                    break;

                case ImageSize.Original:
                default:
                    prefix = imagePrefixes[(int)ImageSize.Original]; ;                    
                    break;
            }

            return Path.Combine(FileSystemWalker.ImageDirectory, prefix + id.ToString() + ".png");
        }

        /// <summary>
        /// So we can manage the load of these
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetImagePathByIdFast(int? id, ImageSize size)
        {
            if (id == null)
                return NoCoverPath;

            string returnPath = ConstructImagePathById(id.Value, size);

            return returnPath;
        }

        public static string GetImagePathByIdSlow(int? id, string returnPath, System.Data.Linq.Binary tmpImg, ImageSize size)
        {
            if (id == null)
                return NoCoverPath;

            try
            {
                int resizeSize = 0;

                switch (size)
                {
                    case ImageSize.Medium:
                        resizeSize = 400;
                        break;

                    case ImageSize.Small:
                        resizeSize = 200;
                        break;

                    case ImageSize.Original:
                    default:
                        resizeSize = -1;
                        break;
                }

                //string returnPath = ConstructImagePathById(id.Value, size);

                // if the file hasn't been cached yet - retrieve it and cache it
                if (!File.Exists(returnPath))
                {
                    //string originalPath = Path.Combine(FileSystemWalker.ImageDirectory, imagePrefixes[(int)ImageSize.Original] + id.ToString() + ".png"); ;

                    //// make sure the original is saved on the disk
                    //if (!File.Exists(originalPath))
                    //{
                        //// grab the image from the db
                        //Image tmpImg2 = ImageManager.GetImageById(id.Value);
                        //if (tmpImg2 != null)
                        //    ResizeImage(tmpImg2, returnPath, resizeSize);
                        ////tmpImg2.Save(originalPath);
                        //else
                        //    returnPath = null;
                    using(Image image=ByteArrayToImage(tmpImg.ToArray()))
                    {
                        // grab the image from the db
                        //Image tmpImg2 = ImageManager.GetImageById(id.Value);
                        if (image != null)
                            ResizeImage(image, returnPath, resizeSize);
                        else
                            returnPath = null;
                    }
                        //using (Image image = ByteArrayToImage(tmpImg.ToArray()))
                        //{
                        //    // save it out
                        //    if (image != null)
                        //        image.Save(originalPath);
                        //    else
                        //        returnPath = null;
                        //}
                    //}

                    // if we're not looking for the original image we need to save the resized version
                    //if (size != ImageSize.Original && returnPath != null)
                    //{
                    //    ResizeImage(originalPath, returnPath, resizeSize);
                    //}
                }

                return returnPath ?? NoCoverPath;
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[Title.GetImagePathById] Exception: " + ex.Message);
                return NoCoverPath;
            }
        }

        /// <summary>
        /// Will return the proper image path for a given id.  If the image doesn't exist you will
        /// get the path for the NoImage image.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetImagePathById(int? id, ImageSize size)
        {
            if (id == null)
                return NoCoverPath;

            try
            {                                
                int resizeSize = 0;

                switch (size)
                {
                    case ImageSize.Medium:                        
                        resizeSize = 400;
                        break;

                    case ImageSize.Small:                        
                        resizeSize = 200;
                        break;

                    case ImageSize.Original:
                    default:                    
                        resizeSize = -1;
                        break;
                }

                string returnPath = ConstructImagePathById(id.Value, size);

                // if the file hasn't been cached yet - retrieve it and cache it
                if (!File.Exists(returnPath))
                {
                    string originalPath = Path.Combine(FileSystemWalker.ImageDirectory, imagePrefixes[(int)ImageSize.Original] + id.ToString() + ".png"); ;

                    // make sure the original is saved on the disk
                    if (!File.Exists(originalPath))
                    {
                        // grab the image from the db
                        using (Image image = GetImageById(id.Value))
                        {
                            // save it out
                            if (image != null)
                                image.Save(originalPath);
                            else
                                returnPath = null;
                        }
                    }

                    // if we're not looking for the original image we need to save the resized version
                    if (size != ImageSize.Original && returnPath != null)
                    {
                        ResizeImage(originalPath, returnPath, resizeSize);
                    }
                }

                return returnPath ?? NoCoverPath;
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[Title.GetImagePathById] Exception: " + ex.Message);
                return NoCoverPath;
            }
        }

        /// <summary>
        /// resizes an image to the given path.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="resizedImagePath"></param>
        /// <param name="size"></param>
        /// <returns>true if the resize was successful</returns>
        public static bool ResizeImage(string imagePath, string resizedImagePath, int size)
        {
            if (!File.Exists(imagePath))
                return false;

            try
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    if (image != null)
                    {
                        using (Image menuCoverArtImage = ScaleImageByHeight(image, size))
                        {
                            menuCoverArtImage.Save(resizedImagePath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[Title.BuildResizedMenuImage] Exception: " + ex.Message);
                return false;
            }

            return true;
        }

        public static bool ResizeImage(Image image, string resizedImagePath, int size)
        {
            try
            {
                
                    if (image != null)
                    {
                        using (Image menuCoverArtImage = ScaleImageByHeight(image, size))
                        {
                            menuCoverArtImage.Save(resizedImagePath, System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[Title.BuildResizedMenuImage] Exception: " + ex.Message);
                return false;
            }

            return true;
        }

        public static Image ScaleImageByHeight(Image img, int height)
        {
            if (img != null)
            {
                double fractionalPercentage = ((double)height / (double)img.Height);
                int outputWidth = (int)(img.Width * fractionalPercentage);
                int outputHeight = height;

                return ScaleImage(img, outputWidth, outputHeight);
            }
            else
            {
                return img;
            }
        }

        public static Image ScaleImage(Image img, int outputWidth, int outputHeight)
        {
            Bitmap outputImage = new Bitmap(outputWidth, outputHeight, img.PixelFormat);
            outputImage.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(img,
                                   new Rectangle(0, 0, outputWidth, outputHeight),
                                   new Rectangle(0, 0, img.Width, img.Height),
                                   GraphicsUnit.Pixel);

                graphics.Dispose();
            }

            return outputImage;
        }        

        /// <summary>
        /// Given a path turns the image into a byte array.  Will return NULL if the operation cannot be done. 
        /// The image is expected to be a JPG
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(Image image)
        {
            byte[] returnArray = null;
            try
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    returnArray = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[FileHelper.ImageToByteArray] Exception: " + ex.Message);
                returnArray = null;
            }

            return returnArray;
        }

        /// <summary>
        /// Turns an image into a byte array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Image ByteArrayToImage(byte[] array)
        {
            Image image = null;

            try
            {
                image = Image.FromStream(new MemoryStream(array));
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[FileHelper.ByteArrayToImage] Exception: " + ex.Message);
                image = null;
            }

            return image;
        }

        /// <summary>
        /// Deletes the given image from the database and from the filesystem cache
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteImage(int id)
        {
            Dao.TitleCollectionDao.SetDeleteImage(id);
            Dao.DBContext.Instance.SubmitChanges();
        }

        /// <summary>
        /// Deletes the given image from the filesystem cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The bytes removed</returns>
        public static long DeleteCachedImage(int id)
        {
            long totalBytesRemoved = 0;

            try
            {
                foreach (string prefix in imagePrefixes)
                {                    
                    string imagePath = Path.Combine(FileSystemWalker.ImageDirectory, prefix + id.ToString() + ".png");

                    if (!File.Exists(imagePath))
                        continue;

                    FileInfo file = new FileInfo(imagePath);
                    long size = file.Length;

                    File.Delete(imagePath);

                    totalBytesRemoved += size;
                }

            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[FileHelper.DeleteCachedImage] Exception: " + ex.Message);
            }

            return totalBytesRemoved;
        }

        /// <summary>
        /// Cleans up all the unused cached images
        /// </summary>
        public static long CleanupCachedImages()
        {
            long totalBytesRemove = 0;

            Dictionary<int, int> ids = new Dictionary<int, int>();

            // get a list of all the unique ids
            foreach (string file in Directory.GetFiles(FileSystemWalker.ImageDirectory, "*.png"))
            {
                int index = file.LastIndexOf("\\");

                string idName = file.Substring(index + 3).Replace(".png", "");
                int id;

                if (int.TryParse(idName, out id))
                {
                    ids[id] = id;
                }
            }

            // turn it into a list
            IEnumerable<int> allIds = new List<int>(ids.Values);

            // select all the images
            IEnumerable<int> dbImages = from i in Dao.DBContext.Instance.DBImages
                                        select i.Id;

            // grab all the extra id's
            IEnumerable<int> deleteIds = allIds.Except(dbImages);

            foreach (int id in deleteIds)
            {
                totalBytesRemove += DeleteCachedImage(id);
            }

            string[] files = Directory.GetFiles(FileSystemWalker.ImageDownloadDirectory, "*.*");
            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);                

                try
                {
                    info.Delete();
                    totalBytesRemove += info.Length;
                }
                catch { }
            }

            return totalBytesRemove;
        }
    }
}
