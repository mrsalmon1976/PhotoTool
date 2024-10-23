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

		private readonly SKColor[] colorPalette;

		//private readonly Colors[] brushColorPalette;

		public FaceDetectionService(IFileService fileService)
		{
			_fileService = fileService;

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
			result.OriginalImagePath = imagePath;

			var faces = this.DetectFaces(imagePath).ToList();

			string dir = Path.GetDirectoryName(imagePath)!;
			string fileName = Path.GetFileNameWithoutExtension(imagePath);
			string extension = Path.GetExtension(imagePath);
			string newFileName = $"{fileName}_D{extension}";
			string newFilePath = Path.Combine(dir, newFileName);

            using var inputImage = SKBitmap.Decode(imagePath);
			using var surface = SKSurface.Create(inputImage.Info);
			var imageFormat = GetImageFormatFromPath(imagePath);          
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
                    string faceFileName = $"{fileName}_face{i}{extension}";
                    string faceFilePath = Path.Combine(dir, faceFileName);

                    using (var faceStream = File.OpenWrite(faceFilePath))
                    {
                        faceData.SaveTo(faceStream);
                        faceStream.Close();
                    }
					f.ImagePath = faceFilePath;
                }
            }

            using var image = surface.Snapshot();
			using var data = image.Encode(imageFormat, 100);
			using var stream = File.OpenWrite(newFilePath);
			data.SaveTo(stream);
			stream.Close();


			result.DecoratedImagePath = newFilePath;
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

		private SKEncodedImageFormat GetImageFormatFromPath(string filePath)
		{
			var extension = Path.GetExtension(filePath).ToLowerInvariant();

			return extension switch
			{
				".png" => SKEncodedImageFormat.Png,
				".jpeg" => SKEncodedImageFormat.Jpeg,
				".jpg" => SKEncodedImageFormat.Jpeg,
				".bmp" => SKEncodedImageFormat.Bmp,
				".gif" => SKEncodedImageFormat.Gif,
				".webp" => SKEncodedImageFormat.Webp,
				_ => SKEncodedImageFormat.Png // Default to PNG if unknown
			};
		}

	}
}
