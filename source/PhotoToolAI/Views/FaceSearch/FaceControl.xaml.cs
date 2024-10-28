using PhotoToolAI.Models;

namespace PhotoToolAI.Views.FaceSearch;

public partial class FaceControl : ContentView
{
	public FaceControl()
	{
		InitializeComponent();
	}

	public event EventHandler<FaceModel>? Clicked;

	public FaceModel FaceModel { get; private set; }

	public void SetFaceModel(FaceModel model)
	{
		FaceModel = model;
        nameLabel.Text = model.Name;

		var data = model.GetImageDataAsBytes();
        faceImage.Source = ImageSource.FromStream(() => new MemoryStream(data));
    }

	private void faceImage_Clicked(object sender, EventArgs e)
	{
		if (Clicked != null)
		{
			Clicked(sender, FaceModel);
		}
	}
}