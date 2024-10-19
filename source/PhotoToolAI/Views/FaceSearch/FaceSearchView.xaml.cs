using Microsoft.Extensions.Logging;

namespace PhotoToolAI.Views.FaceSearch;

public partial class FaceSearchView : ContentView
{
    private readonly ILogger<FaceSearchView> _logger;

    public FaceSearchView()
	{
        _logger = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<ILogger<FaceSearchView>>()!;

        InitializeComponent();
	}

    private void CounterBtn_Clicked(object sender, EventArgs e)
    {
        _logger.LogInformation("Button clicked");
    }
}