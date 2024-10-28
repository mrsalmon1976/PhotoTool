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
	public event EventHandler<FaceModel>? FaceControlClick;

	public async Task LoadFaces()
	{
		var faces = await _faceRepo.GetAllAsync();

        savedFaces.Children.Clear();

        foreach (var face in faces)
		{
			FaceControl faceControl = new FaceControl();
			faceControl.SetFaceModel(face);
			faceControl.Clicked += FaceControl_Clicked;
			savedFaces.Children.Add(faceControl);
		}
	}

	private void FaceControl_Clicked(object? sender, FaceModel faceModel)
	{
		if (FaceControlClick != null)
		{
			FaceControlClick.Invoke(this, faceModel);
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