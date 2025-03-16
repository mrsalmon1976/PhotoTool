using Avalonia.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoTool.Shared.Graphics
{
    public interface IImageProcessor
    {

        IReadOnlyDictionary<string, SKEncodedImageFormat> ImageExtensions { get; }

        SKEncodedImageFormat GetImageFormatFromExtension(string extension);

        byte[]? ConvertToByteArray(Bitmap? image);

        string? ConvertToBase64(Bitmap? image);

        byte[] ConvertToGrayscale(byte[] imageBytes);

        SKEncodedImageFormat GetImageFormatFromPath(string path);

        bool IsImageExtension(string extension);

        void ResizeImage(string path, uint length, string outPath);
    }

    public class ImageProcessor : IImageProcessor
    {
        private Dictionary<string, SKEncodedImageFormat> _imageExtensions = new Dictionary<string, SKEncodedImageFormat>();



        public ImageProcessor()
        {
            _imageExtensions.Add(".bmp", SKEncodedImageFormat.Bmp);
            //_imageExtensions.Add(".dng", SKEncodedImageFormat.Dng); // not supported by SixLabors.ImageSharp
            _imageExtensions.Add(".gif", SKEncodedImageFormat.Gif);
            //_imageExtensions.Add(".heif", SKEncodedImageFormat.Heif); // not supported by SixLabors.ImageSharp
            _imageExtensions.Add(".jpeg", SKEncodedImageFormat.Jpeg);
            _imageExtensions.Add(".jpg", SKEncodedImageFormat.Jpeg);
            // _imageExtensions.Add(".ktx", SKEncodedImageFormat.Ktx);  // not supported by SixLabors.ImageSharp
            // _imageExtensions.Add(".ico", SKEncodedImageFormat.Ico); // not supported by SixLabors.ImageSharp
            // _imageExtensions.Add(".pbm", SKEncodedImageFormat);  // not supported by SkiaSharp
            // _imageExtensions.Add(".pkm", SKEncodedImageFormat.Pkm); // not supported by SixLabors.ImageSharp
            _imageExtensions.Add(".png", SKEncodedImageFormat.Png);
            // _imageExtensions.Add(".qoi", SKEncodedImageFormat);  // not supported by SkiaSharp
            // _imageExtensions.Add(".tga", SKEncodedImageFormat);  // not supported by SkiaSharp
            // _imageExtensions.Add(".tiff", SKEncodedImageFormat);  // not supported by SkiaSharp
            // _imageExtensions.Add(".wbmp", SKEncodedImageFormat.Wbmp);    // not supported by SixLabors.ImageSharp
            _imageExtensions.Add(".webp", SKEncodedImageFormat.Webp);
        }

        public IReadOnlyDictionary<string, SKEncodedImageFormat> ImageExtensions => _imageExtensions;

        public byte[] ConvertToGrayscale(byte[] imageBytes)
        {
            using (Image image = Image.Load(imageBytes)) // Load image from byte array
            {
                image.Mutate(x => x.Grayscale()); // Apply grayscale transformation

                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, new PngEncoder()); // Save as PNG (or another format)
                    return outputStream.ToArray(); // Return byte array
                }
            }
        }

        public string? ConvertToBase64(Bitmap? image)
        {
            string? result = null;
            byte[]? data = ConvertToByteArray(image);
            if (data != null)
            {
                result = Convert.ToBase64String(data);
            }
            return result;
        }


        public byte[]? ConvertToByteArray(Bitmap? image)
        {
            if (image == null)
            {
                return null;
            }
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream);
            return memoryStream.ToArray();
        }

        public SKEncodedImageFormat GetImageFormatFromExtension(string extension)
        {
            var ext = extension.ToLowerInvariant();
            if (!_imageExtensions.ContainsKey(ext.ToLower()))
            {
                throw new NotSupportedException($"Extension {extension} is not a supported image extension");
            }
            return _imageExtensions[ext];
        }

        public SKEncodedImageFormat GetImageFormatFromPath(string path)
        {
            string extension = Path.GetExtension(path);
            return GetImageFormatFromExtension(extension);
        }


        public bool IsImageExtension(string extension)
        {
            var ext = extension.ToLowerInvariant();
            return _imageExtensions.ContainsKey(ext);
        }

        /// <summary>
        /// This function can be used to resize images, while retaining image quality.
        /// </summary>
        /// <param name="path">Source image path.</param>
        /// <param name="length">Length of the output image (longest side - other length will be calculated).</param>
        /// <param name="outPath">Where the file will be written to - note that if path = outPath, the original image will be overwritten</param>
        public virtual void ResizeImage(string path, uint length, string outPath)
        {
            using (Image image = Image.Load(path))
            {
                // SixLabors will do the calculation - we just need to know which side is smaller in the original image and set it to 0
                uint width = (image.Width >= image.Height ? length : 0);
                uint height = (width == 0 ? length : 0);
                image.Mutate(x => x.Resize((int)width, (int)height));
                image.Save(outPath);
            }
        }


    }
}
