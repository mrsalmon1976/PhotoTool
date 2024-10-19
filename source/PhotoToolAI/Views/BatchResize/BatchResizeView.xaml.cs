using Microsoft.Extensions.Logging;

namespace PhotoToolAI.Views.BatchResize;

public partial class BatchResizeView : ContentView
{
    private readonly ILogger<BatchResizeView> _logger;

    public BatchResizeView()
	{
        _logger = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<ILogger<BatchResizeView>>()!;

        InitializeComponent();

    }
}