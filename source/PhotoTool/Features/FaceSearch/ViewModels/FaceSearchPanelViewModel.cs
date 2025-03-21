﻿using Avalonia.Controls;
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
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using PhotoTool.Shared.Comparers;
using PhotoTool.Features.FaceSearch.Services;
using PhotoTool.Features.FaceSearch.Models;
using PhotoTool.Features.FaceSearch.Constants;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using PhotoTool.Shared.Resources;

namespace PhotoTool.Features.FaceSearch.ViewModels
{
    public partial class FaceSearchPanelViewModel : ReactiveObject
    {
        private static IAppLogger _logger = AppLogger.Create<FaceSearchPanelViewModel>();

        private readonly IViewModelProvider _viewModelProvider;
        private readonly IUIProvider _uiProvider;
        private readonly IFaceRepository _faceRepo;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IFaceDetectionService _faceDetectionService;
        private bool _isDeleteButtonEnabled;
        private bool _isFaceListVisible;
        private bool _isSearchActive = false;
        private CancellationTokenSource? _cancellationTokenSource = null;
        private string _infoText = "Select a folder and a face to begin searching.";
        private string _searchPath = String.Empty;
        private uint _searchImageCount = 0;
        private uint _searchImageProgressValue = 0;
        private SavedFaceViewModel? _selectedFace;
        private ImagePreviewViewModel? _previewImageModel;

        public FaceSearchPanelViewModel(IViewModelProvider viewModelProvider
            , IUIProvider uiProvider
            , IFaceRepository faceRepo
            , IImageProcessor imageProcessor
            , IFileSystemProvider fileSystemProvider
            , IFaceDetectionService faceDetectionService
            , IAssetProvider assetProvider)
        {
            _viewModelProvider = viewModelProvider;
            this._uiProvider = uiProvider;
            _faceRepo = faceRepo;
            _imageProcessor = imageProcessor;
            _fileSystemProvider = fileSystemProvider;
            _faceDetectionService = faceDetectionService;

            AddFaceButtonClickCommand = ReactiveCommand.Create(OnAddFaceButtonClick);
            CancelButtonClickCommand = ReactiveCommand.Create(OnCancelButtonClick);
            DeleteFaceButtonClickCommand = ReactiveCommand.Create(OnDeleteFaceButtonClick);
            SearchButtonClickCommand = ReactiveCommand.Create(OnSearchButtonClick);
            SelectFolderButtonClickCommand = ReactiveCommand.Create(OnSelectFolderButtonClickCommand);
        }

        #region Control Properties

        public string InfoText
        {
            get => _infoText;
            private set => this.RaiseAndSetIfChanged(ref _infoText, value);
        }

        public bool IsDeleteButtonEnabled
        {
            get => _isDeleteButtonEnabled;
            private set => this.RaiseAndSetIfChanged(ref _isDeleteButtonEnabled, value);
        }

        public bool IsFaceListVisible
        {
            get => _isFaceListVisible;
            private set => this.RaiseAndSetIfChanged(ref _isFaceListVisible, value);
        }

        public bool IsSearchActive
        {
            get => _isSearchActive;
            private set => this.RaiseAndSetIfChanged(ref _isSearchActive, value);
        }

        public ImagePreviewViewModel? PreviewImageModel
        {
            get => _previewImageModel;
            set {
                this.RaiseAndSetIfChanged(ref _previewImageModel, value);
            }
        }

        public string SearchPath
        {
            get => _searchPath;
            set => this.RaiseAndSetIfChanged(ref _searchPath, value);
        }

        public uint SearchImageCount
        {
            get => _searchImageCount;
            private set => this.RaiseAndSetIfChanged(ref _searchImageCount, value);
        }

        public uint SearchImageProgressValue
        {
            get => _searchImageProgressValue;
            private set => this.RaiseAndSetIfChanged(ref _searchImageProgressValue, value);
        }

        public SavedFaceViewModel? SelectedFace
        {
            get => _selectedFace;
            set 
            {
                _selectedFace = value;
                ToggleButtonStatus();
            }
        }


        public ObservableCollection<SavedFaceViewModel> SavedFaces { get; set; } = new ObservableCollection<SavedFaceViewModel>();

        public ObservableCollection<SearchFaceViewModel> SearchResults { get; set; } = new ObservableCollection<SearchFaceViewModel>();

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> AddFaceButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteFaceButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> SearchButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectFolderButtonClickCommand { get; }

        private async void OnAddFaceButtonClick()
        {
            var faceAddDialog = new FaceAddDialog();
            faceAddDialog.DataContext = _viewModelProvider.GetViewModel<FaceAddDialogViewModel>();
            faceAddDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = await faceAddDialog.ShowDialog<FaceAddDialogViewModel?>(_uiProvider.GetMainWindow());
            await LoadFaces();
        }

        private void OnCancelButtonClick()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async void OnDeleteFaceButtonClick()
        {
            if (this.SelectedFace == null)
            {
                await _uiProvider.ShowErrorDialog("Error", "You have not selected a face image to delete.");
                return;
            }

            var parentWindow = _uiProvider.GetMainWindow();
            var box = MessageBoxManager.GetMessageBoxStandard("Confirm Delete", $"Are you sure you want to delete the face image for '{SelectedFace.Name}'?", ButtonEnum.YesNo, Icon.Question, WindowStartupLocation.CenterScreen);
            ButtonResult result = await box.ShowWindowDialogAsync(parentWindow);
            if (result == ButtonResult.Yes)
            {
                _fileSystemProvider.DeleteFile(this.SelectedFace.FilePath);
                this.SelectedFace = null;
                await LoadFaces();
            }
        }

        private async void OnSelectFolderButtonClickCommand()
        {
            // Start async operation to open the dialog.
            var folder = await _uiProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Search Folder",
                AllowMultiple = false
            });
            if (folder.Count > 0)
            {
                string? path = folder.First().TryGetLocalPath();
                this.SearchPath = path ?? this.SearchPath;
            }
        }

        private async void OnSearchButtonClick()
        {
            await SearchFaces();
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
                    var grayscaleImage = new Bitmap(new MemoryStream(_imageProcessor.ConvertToGrayscale(imageData)));

                    SavedFaces.Add(new SavedFaceViewModel()
                    {
                        FilePath = f.FilePath,
                        ImageGrayscale = grayscaleImage,
                        ImageColor = image,
                        Image = grayscaleImage,
                        Name = f.Name
                    });
                }
            }
            IsFaceListVisible = faces.Any();
        }

        public async Task SearchFaces()
        {
            uint totalFileCount = 0;
            uint imageCount = 0;
            uint faceMatchCount = 0;

            if (!ValidateSearchInput())
            {
                await _uiProvider.ShowErrorDialog("Validation Error", "Please select a folder and face to search.");
                return;
            }

            try
            {
                this.IsSearchActive = true;
                ToggleButtonStatus();
                _cancellationTokenSource = new CancellationTokenSource();
                this.InfoText = "Searching for images";
                this.SearchResults.Clear();
                this.SearchImageProgressValue = 0;
                this.SearchImageCount = 1;          // set to 1 so progress bar looks incomplete

                await Task.Run(() =>
                {

                    // enumerate files
                    IEnumerable<IFileInfoWrapper> files = _fileSystemProvider.EnumerateFiles(SearchPath, "*.*", SearchOption.AllDirectories);
                    List<IFileInfoWrapper> imageFiles = new List<IFileInfoWrapper>();

                    // enumerate the files in the selected folder
                    IPerformanceLogger perfLogger = PerformanceLogger.CreateAndStart<FaceSearchPanelViewModel>("FolderImageSearch");
                    foreach (var fileInfo in files)
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (_imageProcessor.IsImageExtension(fileInfo.Extension))
                        {
                            imageFiles.Add(fileInfo);
                            imageCount++;
                        }

                        totalFileCount++;
                        UpdateProgress($"Searching for images...{imageCount} images found of {totalFileCount} files.", 0);
                    }
                    perfLogger.Stop($"Processed {totalFileCount} files in '{SearchPath}'; {imageCount} images found");
                    this.SearchImageCount = imageCount;

                    // sort the objects
                    perfLogger = PerformanceLogger.CreateAndStart<FaceSearchPanelViewModel>("FolderImageSort");
                    UpdateProgress($"Sorting {imageCount} images by date...", 0);
                    imageFiles.Sort(new FileInfoCreateDateComparer());
                    perfLogger.Stop($"Sorted {imageCount} images by CreateDate.");

                    // get the face embedding
                    var faceEmbedding = _faceDetectionService.GetFaceEmbedding(SelectedFace!.ImageColor!);

                    // now we start trying to search through the images
                    perfLogger = PerformanceLogger.CreateAndStart<FaceSearchPanelViewModel>("ImageFaceSearch");
                    uint progressValue = 0;
                    foreach (IFileInfoWrapper fileInfo in imageFiles)
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        
                        this.UpdateProgress($"Analysing {(progressValue + 1)} of {imageCount} files; {fileInfo.Name}...", progressValue++);

                        FaceComparison faceComparison = _faceDetectionService.SearchForFace(faceEmbedding, fileInfo.FullName);
                        if (faceComparison.FaceMatchProspect != FaceMatchProspect.None)
                        {
                            _uiProvider.InvokeOnUIThread(() =>
                            {
                                SearchResults.Add(new SearchFaceViewModel()
                                {
                                    Name = fileInfo.Name,
                                    FilePath = fileInfo.FullName,
                                    Image = new Bitmap(fileInfo.FullName)
                                });
                            });
                            faceMatchCount++;
                        }
                    }
                    UpdateProgress($"Search for {SelectedFace.Name} complete: {totalFileCount} files, {imageCount} images, {faceMatchCount} facial matches.", 0);
                    perfLogger.Stop($"{imageFiles.Count} images searched for face '{SelectedFace.Name}'");
                });
            }
            catch (OperationCanceledException)
            {
                _logger.Info("Search operation cancelled");
                UpdateProgress($"Search cancelled with {faceMatchCount} facial matches from {imageCount} images.", 0);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UpdateProgress($"Error: {ex.Message}.", 0);
                await _uiProvider.ShowErrorDialog("Search Error", $"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                this.IsSearchActive = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                ToggleButtonStatus();
            }
        }

        private void UpdateProgress(string infoText, uint progress)
        {
            _uiProvider.PostOnUIThread(() => {
                this.InfoText = infoText;
                this.SearchImageProgressValue = progress;
            });
        }

        private void ToggleButtonStatus()
        {
            _uiProvider.PostOnUIThread(() => {
                IsDeleteButtonEnabled = (!this.IsSearchActive && this.SelectedFace != null);
            });
        }

        public void UpdatePreviewImage(SearchFaceViewModel faceViewModel)
        {
            this.PreviewImageModel = new ImagePreviewViewModel()
            {
                Image = faceViewModel.Image,
                Name = faceViewModel.Name,
                Path = faceViewModel.FilePath,
                Dimensions = string.Format("{0}x{1} / {2}", faceViewModel.Image?.Size.Width, faceViewModel.Image?.Size.Height, _fileSystemProvider.GetFileSizeReadable(faceViewModel.FilePath))
            };

        }


        private bool ValidateSearchInput()
        {
            if (String.IsNullOrWhiteSpace(this.SearchPath)
                || !_fileSystemProvider.DirectoryExists(this.SearchPath)
                || SelectedFace == null)
            {
                return false;
            }
            return true;
        }
    }

    #region Design time mode

    public class FaceSearchPanelViewModelDesign : FaceSearchPanelViewModel
    {
        public FaceSearchPanelViewModelDesign() : base(
            new ViewModelProvider()
            , new UIProvider()
            , new FaceRepository(new AppSettings(), new FileSystemProvider())
            , new ImageProcessor()
            , new FileSystemProvider()
            , new FaceDetectionService(new ImageProcessor())
            , new AssetProvider()
            )
        {
        }
    }

    #endregion
}
