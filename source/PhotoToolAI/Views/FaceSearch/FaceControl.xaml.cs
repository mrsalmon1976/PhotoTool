namespace PhotoToolAI.Views.FaceSearch;

public partial class FaceControl : ContentView
{
	public FaceControl()
	{
		InitializeComponent();
	}

	public event EventHandler? Clicked;

	public ImageSource FaceImageSource
	{
		get
		{
			return faceImage.Source;
		}
		set
		{
			faceImage.Source = value;
		}
	}

	public string FaceName
	{
		get
		{
			return nameLabel.Text;
		}
		set
		{
			nameLabel.Text = value;
		}
	}

	private void faceImage_Clicked(object sender, EventArgs e)
	{
		if (Clicked != null)
		{
			Clicked(sender, e);
		}
	}
}