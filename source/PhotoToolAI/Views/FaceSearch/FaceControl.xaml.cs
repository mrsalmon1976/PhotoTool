using PhotoToolAI.Models;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace PhotoToolAI.Views.FaceSearch;

public partial class FaceControl : ContentView
{
    private ImageSource? _grayImageSource;
    private ImageSource? _colorImageSource;

    public FaceControl()
	{
		InitializeComponent();
	}

	public event EventHandler<FaceModel>? Clicked;

	public FaceModel FaceModel { get; private set; }

    public void ResetGrayscale()
    {
        faceImage.Source = _grayImageSource;
    }

	public void SetFaceModel(FaceModel model)
	{
		FaceModel = model;
        nameLabel.Text = model.Name;

		var data = model.GetImageDataAsBytes();
        byte[] greyData;

        using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(data)) // Load image from byte array
        {
            image.Mutate(x => x.Grayscale()); // Apply grayscale transformation

            using (var outputStream = new MemoryStream())
            {
                image.Save(outputStream, new PngEncoder()); // Save as PNG (or another format)
                greyData = outputStream.ToArray(); // Return byte array
            }
        }

        _colorImageSource = ImageSource.FromStream(() => new MemoryStream(data));
        _grayImageSource = ImageSource.FromStream(() => new MemoryStream(greyData));

        faceImage.Source = _grayImageSource;
        faceImage.Clicked += faceImage_Clicked;
    }

	private void faceImage_Clicked(object? sender, EventArgs e)
	{
        if (Clicked != null)
        {
            Clicked(this, FaceModel);
        }
        faceImage.Source = _colorImageSource;
	}
}