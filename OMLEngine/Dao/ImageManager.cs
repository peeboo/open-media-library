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
    public static class ImageManager
    {
        public static void SaveImages(Title title)
        {
            if (title.DaoTitle.FrontCoverImageId == null)
            {
                byte[] image = FileSystem.FileHelper.ImageToByteArray(title.DaoTitle.FrontCoverPath);

                if (image != null)
                {
                    title.DaoTitle.FrontCoverImageId = Dao.TitleCollectionDao.AddImage(image);
                }
            }

            if (title.DaoTitle.BackCoverImageId == null)
            {
                byte[] image = FileSystem.FileHelper.ImageToByteArray(title.DaoTitle.BackCoverPath);

                if (image != null)
                {
                    title.DaoTitle.BackCoverImageId = Dao.TitleCollectionDao.AddImage(image);
                }
            }
        }

        public static Image GetImageById(int id)
        {
            Image image = null;

            Dao.DBImage dbImage = Dao.TitleCollectionDao.GetImageBydId(id);

            if (dbImage != null)
            {
                image = FileSystem.FileHelper.ByteArrayToImage(dbImage.Image.ToArray());
            }

            return image;
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
                            menuCoverArtImage.Save(resizedImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
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
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.DrawImage(img,
                                   new Rectangle(0, 0, outputWidth, outputHeight),
                                   new Rectangle(0, 0, img.Width, img.Height),
                                   GraphicsUnit.Pixel);

                graphics.Dispose();
            }

            return outputImage;
        }
    }
}
