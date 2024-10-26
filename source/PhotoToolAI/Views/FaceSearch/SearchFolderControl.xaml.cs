using Microsoft.Extensions.Logging;
using PhotoToolAI.Comparers;
using PhotoToolAI.Resources;
using PhotoToolAI.Services;

namespace PhotoToolAI.Views.FaceSearch;

public partial class SearchFolderControl : ContentView
{
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isBusy = false;

    private readonly ILogger<SearchFolderControl> _logger;
    private readonly IResourceProvider _resourceProvider;
    private readonly IImageService _imageService;

    public SearchFolderControl()
	{
		InitializeComponent();
        BindingContext = this;

        _logger = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<ILogger<SearchFolderControl>>()!;
        _resourceProvider = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IResourceProvider>()!;
        _imageService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IImageService>()!;

    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
    }

    public string Path
    {
        get
        {
            return fileSystemItem.Path;
        }
        set
        {
            fileSystemItem.Path = value;
        }
    }

    public async Task SearchFolderForFace(string path)
    {
        if (IsBusy) return;

        this.Path = path;
        fileSystemItem.InfoText = "Searching for images...";
        fileSystemItem.InfoTextColor = _resourceProvider.PrimaryDarkTextColor;
        fileSystemItem.IsCancelButtonVisible = true;

        try
        {
            IsBusy = true;
            _cancellationTokenSource = new CancellationTokenSource();

            // Show loading indicator on UI
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                fileSystemItem.InfoText = $"Searching for images...";
            });

            // set up progress feedback listener
            IProgress<string> progress = new Progress<string>(infoText =>
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    fileSystemItem.InfoText = infoText;
                });
            });

            // Perform background work
            await Task.Run(async () =>
            {
                IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
                List<FileInfo> imageFiles = new List<FileInfo>();
                int processedFiles = 0;

                foreach (var file in files)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    FileInfo fileInfo = new FileInfo(file);

                    if (_imageService.IsImageExtension(fileInfo.Extension))
                    {
                        imageFiles.Add(fileInfo);
                    }

                    progress.Report($"Searching for images...{imageFiles.Count} found of {++processedFiles} files.");
                }

                // sort the objects
                progress.Report($"Sorting images by date...");
                imageFiles.Sort(new FileInfoCreateDateComparer());

                // now we start trying to search through the images
                foreach (FileInfo fileInfo in imageFiles)
                {
                }


            }, _cancellationTokenSource.Token);

            // Update UI when complete
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                //StatusLabel.Text = "Processing complete!";
                //LoadingIndicator.IsVisible = false;
            });
        }
        catch (OperationCanceledException)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                fileSystemItem.InfoText = "Image search cancelled.";
                fileSystemItem.InfoTextColor = _resourceProvider.ErrorTextColor;
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                //StatusLabel.Text = $"Error: {ex.Message}";
                //LoadingIndicator.IsVisible = false;
            });
        }
        finally
        {
            fileSystemItem.IsCancelButtonVisible = false;
            IsBusy = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }


    }

    private void fileSystemItemCancelClicked(object sender, EventArgs e)
    {
        _cancellationTokenSource?.Cancel();

    }
}