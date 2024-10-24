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

	public async Task LoadFaces()
	{
		var faces = await _faceRepo.GetAllAsync();

        savedFaces.Children.Clear();

        foreach (var face in faces)
		{
			Label label = new Label();
			label.Text = face.Name;
			savedFaces.Children.Add(label);
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