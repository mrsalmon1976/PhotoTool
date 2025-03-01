using ImageMagick;
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
	public class ImageService
	{

		/// <summary>
		/// Creates an "empty" bitmap with a message to display to the user.
		/// </summary>
		/// <param name="txt">Message to display on the bitmap.</param>
		/// <param name="w">Width of the image.</param>
		/// <param name="h">Height of the image.</param>
		/// <returns>A plain bitmap with a text message.</returns>
		public virtual Bitmap CreateBlankImage(string txt, int w, int h) 
		{
			Font font = SystemFonts.DefaultFont;//.FontFamily, FormMain.DefaultFont.Size);
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

        /// <summary>
        /// This function can be used to resize images, while retaining image quality 
        /// that cannot be done when using the GetThumbnailImage method.
        /// </summary>
        /// <param name="sourceImgPath">Path to the source image</param>
        /// <param name="length">Length of the output image (longest side - other length will be calculated)</param>
        /// <param name="quality">Quality (valid values 0 - 100)</param>
        public virtual Image ResizeImage(string sourceImgPath, int length, int quality)
        {
            return this.ResizeImage(sourceImgPath, length, quality, null);
        }

        /// <summary>
        /// This function can be used to resize images, while retaining image quality 
        /// that cannot be done when using the GetThumbnailImage method.
        /// </summary>
        /// <param name="sourceImgPath">Path to the source image</param>
        /// <param name="outputImgPath">Path to the output image</param>
        /// <param name="length">Length of the output image (longest side - other length will be calculated)</param>
        /// <param name="quality">Quality (valid values 0 - 100)</param>
        public virtual Image ResizeImage(string sourceImgPath, int length, int quality, string outputImgPath)
        {
            MagickImage sourceImage = new MagickImage(sourceImgPath);

            int w = sourceImage.Width;
            int h = sourceImage.Height;
            int width = length;
            int height = width;

            // if length is 0 or less, we just use the original size
            if (length > 0)
            {
                if (w > h)
                {
                    height = Convert.ToInt32(Convert.ToDouble(h) / Convert.ToDouble(w) * width);
                }
                else if (w != h)
                {
                    width = Convert.ToInt32(Convert.ToDouble(w) / Convert.ToDouble(h) * height);
                }
                if (w != width || h != height)
                {
                    sourceImage.Resize(width, height);
                }
            }

            sourceImage.Quality = quality;

            if (!String.IsNullOrWhiteSpace(outputImgPath))
            {
                if (sourceImage.Format == MagickFormat.Heic || sourceImage.Format == MagickFormat.Heif)
                {
                    sourceImage.Format = MagickFormat.Jpeg;
                    int extensionLength = Path.GetExtension(outputImgPath).Length;
                    outputImgPath = outputImgPath.Substring(0, outputImgPath.Length - extensionLength) + ".JPG";
                }
                sourceImage.Write(outputImgPath);
            }

            using (var memStream = new MemoryStream())
            {
                sourceImage.Format = MagickFormat.Jpeg;
                sourceImage.Write(memStream);
                return new System.Drawing.Bitmap(memStream);
            }
        }

    }
}
