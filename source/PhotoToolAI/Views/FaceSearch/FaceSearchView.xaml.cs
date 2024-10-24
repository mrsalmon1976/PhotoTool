using Microsoft.Extensions.Logging;

namespace PhotoToolAI.Views.FaceSearch;

public partial class FaceSearchView : ContentView
{
    private readonly ILogger<FaceSearchView> _logger;

    private ContentView _visibleComponent;

	public FaceSearchView()
	{
        _logger = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<ILogger<FaceSearchView>>()!;

		InitializeComponent();

		_visibleComponent = selectFaceComponent;
		addFaceComponent.FacesSaved += AddFaceComponent_FacesSavedAsync;


	}

    private async void AddFaceComponent_FacesSavedAsync(object? sender, EventArgs e)
    {
        SetVisibleComponent(selectFaceComponent);
		await selectFaceComponent.LoadFaces();
    }

    private void SelectFace_AddFaceButtonClick(object sender, EventArgs e)
	{
		SetVisibleComponent(addFaceComponent);
	}

	private void AddFace_BackButtonClick(object sender, EventArgs e)
	{
		SetVisibleComponent(selectFaceComponent);
	}

	private void SetVisibleComponent(ContentView contentView)
	{
		_visibleComponent.IsVisible = false;
		contentView.IsVisible = true;
		_visibleComponent = contentView;
	}
}