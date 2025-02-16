using FaceAiSharp;
using Microsoft.Extensions.Logging;
using PhotoToolAI.Comparers;
using PhotoToolAI.Constants;
using PhotoToolAI.Models;
using PhotoToolAI.Resources;
using PhotoToolAI.Services;
using SixLabors.ImageSharp.PixelFormats;
using System.Diagnostics;

namespace PhotoToolAI.Views.FaceSearch;

public partial class SearchFolderControl : ContentView
{
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isBusy = false;

    private readonly ILogger<SearchFolderControl> _logger;
    private readonly IResourceProvider _resourceProvider;
    private readonly IImageService _imageService;
    private readonly IFileService _fileService;
    private readonly IFaceDetectionService _faceDetectionService;

    public SearchFolderControl()
	{
		InitializeComponent();
        BindingContext = this;

        _logger = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<ILogger<SearchFolderControl>>()!;
        _resourceProvider = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IResourceProvider>()!;
        _imageService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IImageService>()!;
        _fileService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IFileService>()!;
        _faceDetectionService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IFaceDetectionService>()!;

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

    public async Task SearchFolderForFace(FaceModel faceModel, string path)
    {
        if (IsBusy) return;

        this.Path = path;
        fileSystemItem.InfoText = "Searching for images...";
        fileSystemItem.InfoTextColor = _resourceProvider.PrimaryDarkTextColor;
        fileSystemItem.IsCancelButtonVisible = true;

        int totalFileCount = 0;
        int imageCount = 0;
        int faceMatchCount = 0;


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
                Stopwatch stopwatchLoad = Stopwatch.StartNew();
                IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
                List<FileInfo> imageFiles = new List<FileInfo>();

                foreach (var file in files)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    FileInfo fileInfo = new FileInfo(file);

                    if (_imageService.IsImageExtension(fileInfo.Extension))
                    {
                        imageFiles.Add(fileInfo);
                        imageCount++;
                    }

                    progress.Report($"Searching for images...{imageCount} found of {++totalFileCount} files.");
                }
                stopwatchLoad.Stop();
                _logger.LogInformation($"Processed {totalFileCount} in {stopwatchLoad.ElapsedMilliseconds}ms");

                // sort the objects
                Stopwatch stopwatchSort = Stopwatch.StartNew();
                progress.Report($"Sorting images by date...");
                imageFiles.Sort(new FileInfoCreateDateComparer());
                stopwatchSort.Stop();
                _logger.LogInformation($"Sorted {imageCount} images by CreateDate in {stopwatchSort.ElapsedMilliseconds}ms");

                // get the face embedding
                var faceEmbedding = _faceDetectionService.GetFaceEmbedding(faceModel);

                // now we start trying to search through the images
                Stopwatch stopwatchSearch = Stopwatch.StartNew();
                foreach (FileInfo fileInfo in imageFiles)
                {
                    progress.Report($"Searching file {fileInfo.FullName}.");
                    FaceSearchResult searchResult = _faceDetectionService.SearchForFace(faceEmbedding, fileInfo.FullName);
                    if (searchResult.FaceMatchProspect != FaceMatchProspect.None)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            Label lbl = new Label();
                            lbl.Text = fileInfo.FullName;
                            searchResults.Children.Add(lbl);

                            if (searchResult.FaceMatchProspect == FaceMatchProspect.Probable)
                            {
                                lbl.TextColor = Colors.Green;
                            }
                            else if (searchResult.FaceMatchProspect == FaceMatchProspect.Possible)
                            {
                                lbl.TextColor = Colors.Orange;
                            }
                        });
                    }
                }
                stopwatchSearch.Stop();
                _logger.LogInformation($"Searched {imageCount} image files for face matches in {stopwatchSearch.ElapsedMilliseconds}ms");


            }, _cancellationTokenSource.Token);

            // Update UI when complete
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                fileSystemItem.InfoText = $"{totalFileCount} found, {imageCount} images, {faceMatchCount} matches.";
                fileSystemItem.InfoTextColor = _resourceProvider.PrimaryDarkTextColor;
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
                fileSystemItem.InfoText = $"Unexpected error occurred: {ex.Message}";
                fileSystemItem.InfoTextColor = _resourceProvider.ErrorTextColor;
                _logger.LogError(ex, ex.Message);
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