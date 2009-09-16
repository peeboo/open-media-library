using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace OMLEngine
{
    class OMLThumbnail
    {
        /// <summary>
        /// private void imageThumbnail(string sFilename, string sNewFilename)
        /// {
        ///     byte[] bImage = File.ReadAllBytes(sFilename);
        ///     byte[] nImage = OMLThumbnail.ResizeImageFile(bImage, 100);
        ///     File.WriteAllBytes(sNewFilename, nImage);
        /// }
        /// </summary>
        /// <param name="imageFile">The image, as a byte stream, to be scaled</param>
        /// <param name="targetSize">Specifies the largest dimension of the target image</param>
        /// <returns>A scaled jpg as a byte stream</returns>
        public static byte[] ResizeImageFile(byte[] imageFile, int targetSize)
        {
            MemoryStream mstr = new MemoryStream(imageFile);
            Image oldImage = Image.FromStream(mstr);
            Size newSize = CalculateDimensions(oldImage.Size, targetSize);
            Bitmap newImage = new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format24bppRgb);
            Graphics canvas = Graphics.FromImage(newImage);
            canvas.SmoothingMode = SmoothingMode.AntiAlias;
            canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
            canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
            canvas.DrawImage(oldImage, new Rectangle(new Point(0, 0), newSize));
            MemoryStream m = new MemoryStream();
            newImage.Save(m, ImageFormat.Jpeg);
            return m.GetBuffer();
            canvas.Dispose();
            newImage.Dispose();
            oldImage.Dispose();
            mstr.Dispose();
        }

        /// <summary>
        /// Checks to see if the height > width, then scale the shorter dimension
        /// </summary>
        /// <param name="oldSize"></param>
        /// <param name="targetSize"></param>
        /// <returns></returns>
        private static Size CalculateDimensions(Size oldSize, int targetSize)
        {
            Size newSize = new Size();
            if (oldSize.Height > oldSize.Width)
            {
                newSize.Height = targetSize;
                newSize.Width = (int)(oldSize.Width * (Single)(targetSize / (Single)oldSize.Height));
            }
            else
            {
                newSize.Width = targetSize;
                newSize.Height = (int)(oldSize.Height * (Single)(targetSize / (Single)oldSize.Width));
            }
            return newSize;
        }
    }
}
