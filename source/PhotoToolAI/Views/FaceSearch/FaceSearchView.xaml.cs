using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;
using PhotoToolAI.Comparers;
using PhotoToolAI.Constants;
using PhotoToolAI.Models;
using PhotoToolAI.Repositories;
using PhotoToolAI.Resources;
using PhotoToolAI.Services;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;

namespace PhotoToolAI.Views.FaceSearch;

public partial class FaceSearchView : ContentView
{
    private readonly ILogger<FaceSearchView> _logger;

    private ContentView _visibleComponent;
    private double _menuWidth = 200; // Default width
    private double _startX; // Track the last drag position

    private IFaceRepository _faceRepo;
    private readonly IImageService _imageService;
    private readonly IFaceDetectionService _faceDetectionService;
    private readonly IResourceProvider _resourceProvider;

    private bool _isLoading;
    private bool _isLoaded;
    private bool _isSearching;
    private FaceControl? _selectedFaceControl;
    private CancellationTokenSource? _cancellationTokenSource = null;


    public ICommand DirectorySelectClickCommand => new Command(OnDirectorySelectClick);



    public FaceSearchView()
	{
        _logger = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<ILogger<FaceSearchView>>()!;
        _faceRepo = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IFaceRepository>()!;
        _imageService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IImageService>()!;
        _faceDetectionService = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IFaceDetectionService>()!;
        _resourceProvider = Application.Current!.MainPage!.Handler!.MauiContext!.Services.GetService<IResourceProvider>()!;

        InitializeComponent();
        BindingContext = this;


        _visibleComponent = selectFaceComponent;
		addFaceComponent.FacesSaved += AddFaceComponent_FacesSavedAsync;


        // Handle when the view becomes visible
        this.HandlerChanged += OnHandlerChanged;

    }

    public ObservableCollection<FaceSearchResultItem> FaceSearchResults { get; set; } = new ObservableCollection<FaceSearchResultItem>();

    private async void OnDirectorySelectClick()
    {
        try
        {
            var cancellationToken = new CancellationTokenSource().Token;

            var folderPicker = await FolderPicker.PickAsync(cancellationToken);
            string path = SelectedPathLabel.Text;

            if (folderPicker != null && folderPicker.Folder != null)
            {
                path = folderPicker.Folder.Path;
                await MainThread.InvokeOnMainThreadAsync(() => {
                    SelectedPathLabel.Text = path;
                });
                //SearchFolderControl item = new SearchFolderControl();
                //sources.Children.Add(item);
                //await item.SearchFolderForFace(this.FaceModel!, folderPicker.Folder.Path);
            }
            SearchButton.IsEnabled = Directory.Exists(path);
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

    private async Task LoadFacesAsync()
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() => IsLoading = true);

            // Do the file I/O on a background thread
            var faces = await Task.Run(async () =>
            {
                // Your file loading code here
                return await _faceRepo.GetAllAsync();
            });
            //var listFaces = new ObservableCollection<ListFace>(faces.Select(x => new ListFace(x.Name, x.ImageData)));

            var faceControls = new List<FaceControl>();


            foreach (var face in faces)
            {
                FaceControl faceControl = new FaceControl();
                faceControl.SetFaceModel(face);
                faceControl.Clicked += OnFaceControlClicked; ;
                faceControls.Add(faceControl);
            }

            // Update the UI on the main thread
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                savedFaces.Children.Clear();

                foreach (var faceControl in faceControls)
                {
                    savedFaces.Children.Add(faceControl);
                }
            });
        }
        catch (Exception ex)
        {
            // Handle any errors, possibly show an alert
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                _logger.LogError(ex, "Failed to load faces");
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Faled to load faces: {ex.Message}", "OK");
            });
        }
        finally
        {
            await MainThread.InvokeOnMainThreadAsync(() => IsLoading = false);
        }
    }

    private void OnFaceControlClicked(object? sender, FaceModel e)
    {
        if (_selectedFaceControl != null)
        {
            _selectedFaceControl.ResetGrayscale();
        }
        _selectedFaceControl = sender as FaceControl;
        SearchLabel.Text = $"Search for {e.Name} in";
        SelectedPathLabel.IsVisible = true;
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
    }

    private async void OnHandlerChanged(object sender, EventArgs e)
    {
        if (Handler != null && !_isLoaded)
        {
            await LoadFacesAsync();//.FireAndForget();
            _isLoaded = true;
        }
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

	private void SelectFace_Click(object sender, FaceModel faceModel)
	{
		searchComponent.FaceModel = faceModel;
		SetVisibleComponent(searchComponent);
	}

	private void AddFace_CancelButtonClick(object sender, EventArgs e)
	{
        MainGrid.IsVisible = true;
        addFaceComponent.IsVisible = false;
    }

    private void Search_BackButtonClick(object sender, EventArgs e)
    {
        SetVisibleComponent(selectFaceComponent);
    }

    private void SetVisibleComponent(ContentView contentView)
	{
		if (_visibleComponent != null)
		{
			_visibleComponent.IsVisible = false;
		}
		_visibleComponent = contentView;
        _visibleComponent.IsVisible = true;
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _startX = _menuWidth; // Capture initial width
                break;

            case GestureStatus.Running:
                double newWidth = Math.Max(50, _startX + e.TotalX); // Prevent shrinking too much
                MainGrid.ColumnDefinitions[0].Width = new GridLength(newWidth, GridUnitType.Absolute);
                _menuWidth = newWidth;
                break;
        }
    }

    public async Task SearchFolderForFace()
    {
        if (_isSearching || _selectedFaceControl == null) return;
    
        uint totalFileCount = 0;
        uint imageCount = 0;
        uint faceMatchCount = 0;
        string path = SelectedPathLabel.Text;
        var faceModel = _selectedFaceControl.FaceModel;

        try
        {
            _isSearching = true;
            _cancellationTokenSource = new CancellationTokenSource();

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                ProgressPanel.IsVisible = true;
                ProgressLabel.TextColor = _resourceProvider.PrimaryDarkTextColor;
                ProgressLabel.Text = "Searching for images...";
                FaceSearchResults.Clear();
                CancelSearchButton.IsVisible = true;
                SearchButton.IsEnabled = false;

            });

            // set up progress feedback listener
            IProgress<string> progress = new Progress<string>(infoText =>
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    ProgressLabel.Text = infoText;
                });
            });

            // Perform background work
            await Task.Run(async () =>
            {
                Stopwatch stopwatchLoad = Stopwatch.StartNew();
                IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
                List<FileInfo> imageFiles = new List<FileInfo>();

                double progressPercentage = 0D;

                foreach (var file in files)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    FileInfo fileInfo = new FileInfo(file);

                    if (_imageService.IsImageExtension(fileInfo.Extension))
                    {
                        imageFiles.Add(fileInfo);
                        imageCount++;
                    }

                    totalFileCount++;
                    progress.Report($"Searching for images...{imageCount} images found of {totalFileCount} files.");
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
                int i = 0;
                foreach (FileInfo fileInfo in imageFiles)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    progress.Report($"Analysing file {fileInfo.FullName}.");

                    i++;
                    progressPercentage = (double)i / imageCount;
                    await ProgressBar.ProgressTo(progressPercentage, imageCount, Easing.Linear);

                    FaceSearchResult searchResult = _faceDetectionService.SearchForFace(faceEmbedding, fileInfo.FullName);
                    if (searchResult.FaceMatchProspect != FaceMatchProspect.None)
                    {
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            FaceSearchResults.Add(new FaceSearchResultItem()
                            {
                                Name = fileInfo.Name,
                                Path = fileInfo.FullName,
                                Source = ImageSource.FromFile(fileInfo.FullName),
                                MatchInfo = $"{searchResult.FaceMatchProspect.ToString()} match",
                                MatchColor = (searchResult.FaceMatchProspect == FaceMatchProspect.Probable) ? Colors.Green : Colors.Orange
                            });
                            faceMatchCount++;

                        });
                    }
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        ResultsLabel.Text = $"{totalFileCount} files, {imageCount} images, {faceMatchCount} facial matches.";
                    });
                }
                stopwatchSearch.Stop();
                _logger.LogInformation($"Searched {imageCount} image files for face matches in {stopwatchSearch.ElapsedMilliseconds}ms");


            }, _cancellationTokenSource.Token);

            // Update UI when complete
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ResultsLabel.Text = $"{totalFileCount} files, {imageCount} images, {faceMatchCount} facial matches.";
            });

        }
        catch (OperationCanceledException)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ProgressLabel.Text = "Image search cancelled.";
                ProgressLabel.TextColor = _resourceProvider.ErrorTextColor;
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ProgressLabel.Text = $"Unexpected error occurred: {ex.Message}";
                ProgressLabel.TextColor = _resourceProvider.ErrorTextColor;
                _logger.LogError(ex, "Unexpected error occurred: {message}", ex.Message);
            });
        }
        finally
        {
            //fileSystemItem.IsCancelButtonVisible = false;
            _isSearching = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            CancelSearchButton.IsVisible = false;
            SearchButton.IsEnabled = true;

        }


    }

    private async void SearchButton_Clicked(object sender, EventArgs e)
    {
        await SearchFolderForFace();
    }

    private void CancelSearchButton_Clicked(object sender, EventArgs e)
    {
        _cancellationTokenSource?.Cancel();
    }

    public async Task<FileResult?> PickImageFile()
    {
        FileResult? result = null;
        try
        {
            result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Select an Image"
            });
        }
        catch (Exception ex)
        {
            // Handle errors - often due to permission issues
            _logger.LogError($"Image selection failed: {ex.Message}");
        }
        return result;
    }

    private void AddFaceButton_Clicked(object sender, EventArgs e)
    {
        MainGrid.IsVisible = false;
        addFaceComponent.IsVisible = true;
    }

    private async void addFaceComponent_FacesSaved(object sender, EventArgs e)
    {
        await LoadFacesAsync();
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            MainGrid.IsVisible = true;
            addFaceComponent.IsVisible = false;
        });
    }
}

public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
        task.ContinueWith(t =>
        {
            if (t.IsFaulted && t.Exception != null)
            {
                Console.WriteLine($"Fire-and-forget task failed: {t.Exception}");
            }
        });
    }
}