using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Maui.Storage;
using SixLabors.ImageSharp.PixelFormats;
using PhotoToolAI.Services;
using SkiaSharp;

namespace PhotoToolAI.Views.FaceSearch;

public partial class AddFaceComponent : ContentView
{
	private readonly ILogger<AddFaceComponent> _logger;
	private IImageService _imageService;
	private IFaceDetectionService _faceDetectionService;

	public AddFaceComponent()
	{
		_logger = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<ILogger<AddFaceComponent>>()!;
		_imageService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IImageService>()!;
		_faceDetectionService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IFaceDetectionService>()!;

		InitializeComponent();
	}

	public event EventHandler? BackButtonClick;


	// Simple version to pick a single image file
	public async Task<FileResult?> PickImageFile()
	{
		try
		{
			var result = await FilePicker.PickAsync(new PickOptions
			{
				FileTypes = FilePickerFileType.Images,
				PickerTitle = "Select an Image"
			});

			if (result != null)
			{
				// Return the file result which contains:
				// result.FileName
				// result.FullPath
				// result.ContentType
				return result;
			}

			return null;
		}
		catch (Exception ex)
		{
			// Handle errors - often due to permission issues
			_logger.LogError($"Image selection failed: {ex.Message}");
			return null;
		}
	}

	// Version that allows multiple images
	private async void BtnSelect_Clicked(object sender, EventArgs e)
	{
		var file = await PickImageFile();
		if (file != null)
		{
			// copy the file into the working folder
			string workingFilePath = _imageService.CopyToWorkingDirectory(file.FullPath);

			// copy the image and add face squares
			var faceDetectionResult = _faceDetectionService.CopyImageWithFaceDetections(workingFilePath);

			// load the image into the image component
			imgFace.Source = ImageSource.FromFile(faceDetectionResult.DecoratedImagePath);
			imgFace.MaximumHeightRequest = faceDetectionResult.ImageSize!.Value.Height;
			if (imgFace.MaximumHeightRequest > 325)
			{
				imgFace.MaximumHeightRequest = 325;
			}

			nameCapturePanel.Children.Clear();

            foreach (var face in faceDetectionResult.Faces)
			{
				
				Color borderColor = _imageService.ConvertColor(face.Color);

                //SKBitmap originalBitmap = SKBitmap.Decode(faceDetectionResult.OriginalImagePath);
				//SKRectI bounds = new SKRectI((int)face.Box.X, (int)face.Box.Y, (int)face.Box.X + (int)face.Box.Width, (int)face.Box.Y + (int)face.Box.Height);
                //SKBitmap extractedBitmap = new SKBitmap(bounds.Width, bounds.Height);
                //using (var canvas = new SKCanvas(extractedBitmap))
                //{
                //    // Draw the portion of the original bitmap onto the new one
                //    canvas.DrawBitmap(originalBitmap, bounds, new SKRect(0, 0, bounds.Width, bounds.Height));
                //}

                NameEntry entry = new NameEntry();
				entry.BorderColor = borderColor;
				entry.Margin = new Thickness(2);
				//entry.HorizontalOptions = LayoutOptions.Start;
                //entry.VerticalOptions = LayoutOptions.Start;

                ImageSource imageSource = ImageSource.FromFile(face.ImagePath);
                entry.FaceImage = imageSource;


                nameCapturePanel.Children.Add(entry);


			}

		}
	}

	private void BtnBack_Clicked(object sender, EventArgs e)
	{
		if (BackButtonClick != null)
		{
			BackButtonClick(this, EventArgs.Empty);
		}

	}
}