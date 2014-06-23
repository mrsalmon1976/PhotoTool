using ImageProcessor;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace SAFish.PhotoTool
{
	/// <summary>
	/// Provides common static methods.
	/// </summary>
	public class ImageUtils
	{
		private ImageUtils()
		{
		}

		/// <summary>
		/// Creates an "empty" bitmap with a message to display to the user.
		/// </summary>
		/// <param name="txt">Message to display on the bitmap.</param>
		/// <param name="w">Width of the image.</param>
		/// <param name="h">Height of the image.</param>
		/// <returns>A plain bitmap with a text message.</returns>
		public static Bitmap CreateBlankImage(string txt, int w, int h) 
		{
			Font font = FormMain.DefaultFont;//.FontFamily, FormMain.DefaultFont.Size);
			Bitmap b = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(b))
            {
                float x = (b.Width - g.MeasureString(txt, font).Width) / 2;
                float y = (b.Height - g.MeasureString(txt, font).Height) / 2;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString(txt, font, new SolidBrush(Color.Black), x, y);
                return b;
            }
		}

        public static Image ConvertBytesToImage(byte[] imgData)
        {
            using (MemoryStream ms = new MemoryStream(imgData))
            {
                return Image.FromStream(ms);
            }
        }

        public static byte[] ResizeImage(byte[] imgBytes, int length, long quality)
        {
            using (MemoryStream inStream = new MemoryStream(imgBytes))
            {
                Image sourceImage = Image.FromStream(inStream);

                // Read a file and resize it.
                ImageFormat format = sourceImage.RawFormat;

                int w = sourceImage.Width;
                int h = sourceImage.Height;
                double width = 0.0;
                double height = 0.0;
                if (w > h)
                {
                    width = Convert.ToDouble(length);
                    height = Convert.ToDouble(h) / Convert.ToDouble(w) * width;
                }
                else
                {
                    height = Convert.ToDouble(length);
                    width = Convert.ToDouble(w) / Convert.ToDouble(h) * height;
                }
                Size size = new Size(Convert.ToInt32(width), Convert.ToInt32(height));


                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                                    .Resize(size)
                                    .Format(format)
                                    .Quality(Convert.ToInt32(quality))
                                    .Save(outStream);
                    }

                    return outStream.ToArray();
                }
            }

        }
        /// <summary>
        /// This function can be used to resize images, while retaining image quality 
        /// that cannot be done when using the GetThumbnailImage method.
        /// </summary>
        /// <param name="sourceImgPath">Path to the source image</param>
        /// <param name="outputImgPath">Path to the output image</param>
        /// <param name="length">Length of the output image (longest side - other length will be calculated)</param>
        /// <param name="quality">Quality (valid values 0 - 100)</param>
        public static byte[] ResizeImage(string sourceImgPath, string outputImgPath, int length, long quality)
        {
            byte[] img = File.ReadAllBytes(sourceImgPath);
            byte[] resized = ResizeImage(img, length, quality);
            File.WriteAllBytes(outputImgPath, resized);
            return resized;
        }


		/// <summary>
		/// Resizes an image.
		/// </summary>
		/// <param name="bitmap">Image to resize</param>
		/// <param name="width">New width of the image</param>
		/// <param name="height">New height of the image</param>
		/// <returns>A resized version of the original image</returns>
		public static Bitmap ScaleImage(Bitmap bitmap, int width, int height) 
		{
			Bitmap bOut = new Bitmap(width, height);
			Graphics g = Graphics.FromImage(bOut);
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.DrawImage(bitmap, 0, 0, width, height); 
			g.Dispose();
			return bOut;

		}
		
	}
}
