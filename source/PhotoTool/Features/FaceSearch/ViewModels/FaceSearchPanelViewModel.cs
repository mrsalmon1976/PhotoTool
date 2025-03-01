using Avalonia.Controls;
using Avalonia.Media.Imaging;
using PhotoTool.Shared.Configuration;
using PhotoTool.Features.FaceSearch.Views;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using PhotoTool.Shared.ViewModels;
using PhotoTool.Features.FaceSearch.Repositories;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.UI;
using System.Threading;
using System;
using PhotoTool.Shared.Logging;
using Avalonia.Threading;

namespace PhotoTool.Features.FaceSearch.ViewModels
{
    public partial class FaceSearchPanelViewModel : ReactiveObject
    {
        private static IAppLogger _logger = AppLogger.Create<FaceSearchPanelViewModel>();

        private readonly IViewModelProvider _viewModelProvider;
        private readonly IFaceRepository _faceRepo;
        private readonly IImageProcessor _imageService;
        private bool _IsFaceListVisible;
        private bool _isAddFaceButtonEnabled = true;
        private bool _isSearchButtonEnabled = true;
        private CancellationTokenSource? _cancellationTokenSource = null;

        public FaceSearchPanelViewModel(IViewModelProvider viewModelProvider, IFaceRepository faceRepo, IImageProcessor imageService)
        {
            _viewModelProvider = viewModelProvider;
            _faceRepo = faceRepo;
            _imageService = imageService;
            AddFaceButtonClickCommand = ReactiveCommand.Create(OnAddFaceButtonClick);
            SearchButtonClickCommand = ReactiveCommand.Create(OnSearchButtonClick);
        }

        #region Control Properties

        public bool IsFaceListVisible
        {
            get => _IsFaceListVisible;
            private set => this.RaiseAndSetIfChanged(ref _IsFaceListVisible, value);
        }

        public bool IsAddFaceButtonEnabled
        {
            get => _isAddFaceButtonEnabled;
            private set => this.RaiseAndSetIfChanged(ref _isAddFaceButtonEnabled, value);
        }

        public bool IsSearchButtonEnabled
        {
            get => _isSearchButtonEnabled;
            private set => this.RaiseAndSetIfChanged(ref _isSearchButtonEnabled, value);
        }


        public ObservableCollection<FaceAddViewModel> SavedFaces { get; set; } = new ObservableCollection<FaceAddViewModel>();

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> AddFaceButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> SearchButtonClickCommand { get; }

        private async void OnAddFaceButtonClick()
        {
            var faceAddDialog = new FaceAddDialog();
            faceAddDialog.DataContext = _viewModelProvider.GetViewModel<FaceAddDialogViewModel>();
            faceAddDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = await faceAddDialog.ShowDialog<FaceAddDialogViewModel?>(WindowUtils.GetMainWindow());
            await LoadFaces();
        }

        private async void OnSearchButtonClick()
        {
            string? result = await SearchFaces();
            if (!String.IsNullOrEmpty(result))
            {
                await WindowUtils.ShowErrorDialog("Search Error", $"An unexpected error occurred: {result}");
            }
        }

        #endregion



        public async Task LoadFaces()
        {
            SavedFaces.Clear();
            var faces = await _faceRepo.GetAllAsync();
            if (faces.Count() > 0)
            {
                foreach (var f in faces)
                {
                    var imageData = f.GetImageDataAsBytes();
                    var image = new Bitmap(new MemoryStream(imageData));
                    var grayscaleImage = new Bitmap(new MemoryStream(_imageService.ConvertToGrayscale(imageData)));

                    SavedFaces.Add(new FaceAddViewModel()
                    {
                        ImageGrayscale = grayscaleImage,
                        Image = image,
                        Name = f.Name
                    });
                }
            }
            IsFaceListVisible = faces.Any();
        }

        private async Task<string?> SearchFaces()
        {
            ToggleBusyStatus(true);

            //uint totalFileCount = 0;
            //uint imageCount = 0;
            //uint faceMatchCount = 0;
            //string path = SelectedPathLabel.Text;
            //var faceModel = _selectedFaceControl.FaceModel;

            try
            {
                _cancellationTokenSource = new CancellationTokenSource();

                //    await MainThread.InvokeOnMainThreadAsync(async () =>
                //    {
                //        ProgressPanel.IsVisible = true;
                //        ProgressLabel.TextColor = _resourceProvider.PrimaryDarkTextColor;
                //        ProgressLabel.Text = "Searching for images...";
                //        FaceSearchResults.Clear();
                //        CancelSearchButton.IsVisible = true;
                //        SearchButton.IsEnabled = false;

                //    });

                //    // set up progress feedback listener
                //    IProgress<string> progress = new Progress<string>(infoText =>
                //    {
                //        MainThread.InvokeOnMainThreadAsync(() =>
                //        {
                //            ProgressLabel.Text = infoText;
                //        });
                //    });

                //    // Perform background work
                //    await Task.Run(async () =>
                //    {
                //        Stopwatch stopwatchLoad = Stopwatch.StartNew();
                //        IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
                //        List<FileInfo> imageFiles = new List<FileInfo>();
                //        double progressPercentage = 0D;

                //        foreach (var file in files)
                //        {
                //            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                //            FileInfo fileInfo = new FileInfo(file);

                //            if (_imageService.IsImageExtension(fileInfo.Extension))
                //            {
                //                imageFiles.Add(fileInfo);
                //                imageCount++;
                //            }

                //            totalFileCount++;
                //            progress.Report($"Searching for images...{imageCount} images found of {totalFileCount} files.");
                //        }
                //        stopwatchLoad.Stop();
                //        _logger.LogInformation($"Processed {totalFileCount} in {stopwatchLoad.ElapsedMilliseconds}ms");

                //        // sort the objects
                //        Stopwatch stopwatchSort = Stopwatch.StartNew();
                //        progress.Report($"Sorting images by date...");
                //        imageFiles.Sort(new FileInfoCreateDateComparer());
                //        stopwatchSort.Stop();
                //        _logger.LogInformation($"Sorted {imageCount} images by CreateDate in {stopwatchSort.ElapsedMilliseconds}ms");

                //        // get the face embedding
                //        var faceEmbedding = _faceDetectionService.GetFaceEmbedding(faceModel);

                //        // now we start trying to search through the images
                //        Stopwatch stopwatchSearch = Stopwatch.StartNew();
                //        int i = 0;
                //        foreach (FileInfo fileInfo in imageFiles)
                //        {
                //            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                //            progress.Report($"Analysing file {fileInfo.FullName}.");

                //            i++;
                //            progressPercentage = (double)i / imageCount;
                //            await ProgressBar.ProgressTo(progressPercentage, imageCount, Easing.Linear);

                //            FaceSearchResult searchResult = _faceDetectionService.SearchForFace(faceEmbedding, fileInfo.FullName);
                //            if (searchResult.FaceMatchProspect != FaceMatchProspect.None)
                //            {
                //                await MainThread.InvokeOnMainThreadAsync(() =>
                //                {
                //                    FaceSearchResults.Add(new FaceSearchResultItem()
                //                    {
                //                        Name = fileInfo.Name,
                //                        Path = fileInfo.FullName,
                //                        Source = ImageSource.FromFile(fileInfo.FullName),
                //                        MatchInfo = $"{searchResult.FaceMatchProspect.ToString()} match",
                //                        MatchColor = (searchResult.FaceMatchProspect == FaceMatchProspect.Probable) ? Colors.Green : Colors.Orange
                //                    });
                //                    faceMatchCount++;

                //                });
                //            }
                //            await MainThread.InvokeOnMainThreadAsync(() =>
                //            {
                //                ResultsLabel.Text = $"{totalFileCount} files, {imageCount} images, {faceMatchCount} facial matches.";
                //            });
                //        }
                //        stopwatchSearch.Stop();
                //        _logger.LogInformation($"Searched {imageCount} image files for face matches in {stopwatchSearch.ElapsedMilliseconds}ms");


                //    }, _cancellationTokenSource.Token);

                //    // Update UI when complete
                //    await MainThread.InvokeOnMainThreadAsync(() =>
                //    {
                //        ResultsLabel.Text = $"{totalFileCount} files, {imageCount} images, {faceMatchCount} facial matches.";
                //    });

                //}
                //catch (OperationCanceledException)
                //{
                //    await MainThread.InvokeOnMainThreadAsync(() =>
                //    {
                //        ProgressLabel.Text = "Image search cancelled.";
                //        ProgressLabel.TextColor = _resourceProvider.ErrorTextColor;
                //    });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return ex.Message;
            }
            finally
            {
                ToggleBusyStatus(false);
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }

            return null;
        }

        private void ToggleBusyStatus(bool isBusy)
        {
            Dispatcher.UIThread.Post(() => {
                this.IsAddFaceButtonEnabled = !isBusy;
                this.IsSearchButtonEnabled = !isBusy;
            });
        }
    }

    #region Design time mode

    public class FaceSearchPanelViewModelDesign : FaceSearchPanelViewModel
    {
        public FaceSearchPanelViewModelDesign() : base(
            new ViewModelProvider()
            , new FaceRepository(new AppSettings(), new FileSystemProvider())
            , new ImageProcessor()
            )
        {
        }
    }

    #endregion
}
