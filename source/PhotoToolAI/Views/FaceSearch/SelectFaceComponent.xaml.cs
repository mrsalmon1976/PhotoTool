namespace PhotoToolAI.Views.FaceSearch;

public partial class SelectFaceComponent : ContentView
{
	public SelectFaceComponent()
	{
		InitializeComponent();
	}

	public event EventHandler? AddFaceButtonClick;

	private void BtnAddFace_Clicked(object sender, EventArgs e)
	{
		if (AddFaceButtonClick != null)
		{
			AddFaceButtonClick.Invoke(this, EventArgs.Empty);
		}
	}
}