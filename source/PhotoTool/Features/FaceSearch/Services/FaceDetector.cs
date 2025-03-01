using FaceAiSharp;
using PhotoTool.Features.FaceSearch.Models;
using PhotoTool.Shared.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.FaceSearch.Services
{
    public interface IFaceDetector
    {
        FaceDetectionResult DecorateImageWithFaceDetections(string imagePath);

        IEnumerable<FaceDetectionResultItem> DetectFaces(string imagePath);

        IEnumerable<FaceDetectionResultItem> DetectFaces(Image<Rgb24> image);
    }

    public class FaceDetector : IFaceDetector
    {
        private readonly SKColor[] _colorPalette;
        private readonly IImageProcessor _imageService;

        public FaceDetector(IImageProcessor imageService)
        {
            _colorPalette = new SKColor[]
            {
                SKColors.LightGreen,
                SKColors.Red,
                SKColors.Orange,
                SKColors.Cyan,
                SKColors.Yellow,
                SKColors.Purple,
                SKColors.Silver,
                SKColors.Blue,
                SKColors.Pink,
                SKColors.White
            };
            _imageService = imageService;
        }

        public IEnumerable<FaceDetectionResultItem> DetectFaces(string imagePath)
        {
            var image = Image.Load<Rgb24>(imagePath);
            return DetectFaces(image);
        }

        public FaceDetectionResult DecorateImageWithFaceDetections(string imagePath)
        {
            FaceDetectionResult result = new FaceDetectionResult();

            var faces = DetectFaces(imagePath).ToList();

            //string dir = Path.GetDirectoryName(imagePath)!;
            //string fileName = Path.GetFileNameWithoutExtension(imagePath);
            //string extension = Path.GetExtension(imagePath);
            //string newFileName = $"{fileName}_D{extension}";
            //string newFilePath = Path.Combine(dir, newFileName);

            using var inputImage = SKBitmap.Decode(imagePath);
            var imageFormat = _imageService.GetImageFormatFromPath(imagePath);

            using (var inputImageData = inputImage.Encode(imageFormat, 100))
            {
                result.OriginalImageData = inputImageData.ToArray();
            }

            using var surface = SKSurface.Create(inputImage.Info);
            var canvas = surface.Canvas;

            // Draw the original image
            canvas.Clear(SKColors.Transparent);
            canvas.DrawBitmap(inputImage, 0, 0);

            for (int i = 0; i < faces.Count; i++)
            {
                FaceDetectionResultItem f = faces[i];
                // draw lines on the image to highlight the face
                var paint = new SKPaint()
                {
                    Color = f.Color,
                    StrokeWidth = 5
                };

                canvas.DrawLine(f.Box.X, f.Box.Y, f.Box.X + f.Box.Width, f.Box.Y, paint);
                canvas.DrawLine(f.Box.X, f.Box.Y, f.Box.X, f.Box.Y + f.Box.Height, paint);
                canvas.DrawLine(f.Box.X, f.Box.Y + f.Box.Height, f.Box.X + f.Box.Width, f.Box.Y + f.Box.Height, paint);
                canvas.DrawLine(f.Box.X + f.Box.Width, f.Box.Y, f.Box.X + f.Box.Width, f.Box.Y + f.Box.Height, paint);

                // extract the image out
                SKRectI bounds = new SKRectI((int)f.Box.X, (int)f.Box.Y, (int)f.Box.X + (int)f.Box.Width, (int)f.Box.Y + (int)f.Box.Height);
                using SKBitmap faceImage = new SKBitmap(bounds.Width, bounds.Height);
                using (var faceCanvas = new SKCanvas(faceImage))
                {
                    // Draw the portion of the original bitmap onto the new one
                    faceCanvas.DrawBitmap(inputImage, bounds, new SKRect(0, 0, bounds.Width, bounds.Height));
                }
                using (var faceData = faceImage.Encode(imageFormat, 100))
                {
                    f.ImageData = faceData.ToArray();
                }
            }

            using var image = surface.Snapshot();
            using var decoratedImageData = image.Encode(imageFormat, 100);


            //result.DecoratedImagePath = newFilePath;
            result.DecoratedImageData = decoratedImageData.ToArray();
            result.Faces.AddRange(faces);
            result.ImageSize = new Size(inputImage.Width, inputImage.Height);

            return result;
        }

        public IEnumerable<FaceDetectionResultItem> DetectFaces(Image<Rgb24> image)
        {
            var det = FaceAiSharpBundleFactory.CreateFaceDetectorWithLandmarks();
            var faces = det.DetectFaces(image);

            int colorIndex = 0;
            List<FaceDetectionResultItem> result = new List<FaceDetectionResultItem>();
            foreach (var face in faces)
            {
                result.Add(new FaceDetectionResultItem()
                {
                    Box = face.Box,
                    Color = _colorPalette[colorIndex]
                });

                colorIndex++;

                if (colorIndex == _colorPalette.Length)
                {
                    colorIndex = 0;
                }
            }
            return result;
        }
    }
}
