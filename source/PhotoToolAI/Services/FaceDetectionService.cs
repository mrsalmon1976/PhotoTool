using FaceAiSharp;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using PhotoToolAI.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Services
{
	internal interface IFaceDetectionService
	{
		ImageFaceDetectionResult CopyImageWithFaceDetections(string imagePath);

		IEnumerable<FaceDetectionResult> DetectFaces(string imagePath);
	}

	internal class FaceDetectionService : IFaceDetectionService
	{
		private readonly IFileService _fileService;
        private readonly IImageService _imageService;

        private readonly SKColor[] colorPalette;

		//private readonly Colors[] brushColorPalette;

		public FaceDetectionService(IFileService fileService, IImageService imageService)
		{
			_fileService = fileService;
			_imageService = imageService;

            colorPalette = new SKColor[]
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
		}

		public ImageFaceDetectionResult CopyImageWithFaceDetections(string imagePath)
		{
			ImageFaceDetectionResult result = new ImageFaceDetectionResult();

			var faces = this.DetectFaces(imagePath).ToList();

			string dir = Path.GetDirectoryName(imagePath)!;
			string fileName = Path.GetFileNameWithoutExtension(imagePath);
			string extension = Path.GetExtension(imagePath);
			string newFileName = $"{fileName}_D{extension}";
			string newFilePath = Path.Combine(dir, newFileName);

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
				FaceDetectionResult f = faces[i];
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
			result.ImageSize = new Microsoft.Maui.Graphics.Size(inputImage.Width, inputImage.Height);

			return result;
		}

		public IEnumerable<FaceDetectionResult> DetectFaces(string imagePath)
		{
			var img = SixLabors.ImageSharp.Image.Load<Rgb24>(imagePath);
			var det = FaceAiSharpBundleFactory.CreateFaceDetectorWithLandmarks();
			var faces = det.DetectFaces(img);

			int colorIndex = 0;
			List<FaceDetectionResult> result = new List<FaceDetectionResult>();
			foreach (var face in faces)
			{
				result.Add(new FaceDetectionResult()
				{
					Box = face.Box,
					Color = colorPalette[colorIndex]
				});

				colorIndex++;

				if (colorIndex == colorPalette.Length)
				{
					colorIndex = 0;
				}
			}
			return result;
		}



	}
}
