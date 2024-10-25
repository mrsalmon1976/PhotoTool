using PhotoToolAI.Models;
using PhotoToolAI.Repositories;

namespace PhotoToolAI.Views.FaceSearch;

public partial class SelectFaceComponent : ContentView
{
    private IFaceRepository _faceRepo;
    
	public SelectFaceComponent()
	{
        _faceRepo = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IFaceRepository>()!;


        InitializeComponent();

        this.Loaded += SelectFaceComponent_Loaded;
	}

    private async void SelectFaceComponent_Loaded(object? sender, EventArgs e)
    {
		await LoadFaces();
    }

    public event EventHandler? AddFaceButtonClick;
	public event EventHandler? FaceButtonClick;

	public async Task LoadFaces()
	{
		var faces = await _faceRepo.GetAllAsync();

        savedFaces.Children.Clear();

        foreach (var face in faces)
		{
			byte[] data = Convert.FromBase64String(face.ImageData);
			FaceControl faceControl = new FaceControl();
			faceControl.FaceName = face.Name;
			faceControl.FaceImageSource = ImageSource.FromStream(() => new MemoryStream(data));
			faceControl.Clicked += FaceControl_Clicked;
			savedFaces.Children.Add(faceControl);
		}
	}

	private void FaceControl_Clicked(object? sender, EventArgs e)
	{
		if (FaceButtonClick != null)
		{
			FaceButtonClick.Invoke(this, EventArgs.Empty);
		}
	}

	private void BtnAddFace_Clicked(object sender, EventArgs e)
	{
		if (AddFaceButtonClick != null)
		{
			AddFaceButtonClick.Invoke(this, EventArgs.Empty);
		}
	}
}