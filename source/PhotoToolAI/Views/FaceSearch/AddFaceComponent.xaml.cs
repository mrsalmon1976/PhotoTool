using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Maui.Storage;
using SixLabors.ImageSharp.PixelFormats;

namespace PhotoToolAI.Views.FaceSearch;

public partial class AddFaceComponent : ContentView
{
	private readonly ILogger<AddFaceComponent> _logger;

	public AddFaceComponent()
	{
		_logger = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<ILogger<AddFaceComponent>>()!;

		InitializeComponent();
	}

	public event EventHandler BackButtonClick;


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

			// Example: Load into an Image control
			//var stream = await file.OpenReadAsync();
			//var img = SixLabors.ImageSharp.Image.Load<Rgb24>(stream);
			
			//imgFace.Source = ImageSource.FromStream(() => stream);

			//using (System.Drawing.Graphics graphics = System.Drawing.Graphics.Fr(b))
			//{

			//}

			// Or just get the path
			string filePath = file.FullPath;
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