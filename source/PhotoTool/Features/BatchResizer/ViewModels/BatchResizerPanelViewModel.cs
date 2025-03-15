﻿using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using NLog.Filters;
using PhotoTool.Features.BatchResizer.Models;
using PhotoTool.Features.BatchResizer.Validators;
using PhotoTool.Features.FaceSearch.Services;
using PhotoTool.Features.FaceSearch.ViewModels;
using PhotoTool.Shared.Comparers;
using PhotoTool.Shared.Configuration;
using PhotoTool.Shared.Exceptions;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.Logging;
using PhotoTool.Shared.Resources;
using PhotoTool.Shared.UI;
using PhotoTool.Shared.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PhotoTool.Features.BatchResizer.ViewModels
{
    public class BatchResizerPanelViewModel : ReactiveObject
    {
        private static IAppLogger _logger = AppLogger.Create<BatchResizerPanelViewModel>();

        private CancellationTokenSource? _cancellationTokenSource = null;
        private string _infoText = "Add images to be resized.";

        private bool _isBusy = false;
        private bool _isBatchResizeInProgress = false;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IImageProcessor _imageProcessor;
        private readonly IImageResizeOptionsValidator _imageResizeOptionsValidator;
        private uint _imageResizeProgressValue = 0;
        private ImagePreviewViewModel? _previewImageModel;

        public BatchResizerPanelViewModel(ImageResizeOptionsViewModel resizeOptionsViewModel
            , IFileSystemProvider fileSystemProvider
            , IImageProcessor imageProcessor
            , IImageResizeOptionsValidator imageResizeOptionsValidator
            )
        {
            ImageResizeOptionsViewModel = resizeOptionsViewModel;
            _fileSystemProvider = fileSystemProvider;
            _imageProcessor = imageProcessor;
            _imageResizeOptionsValidator = imageResizeOptionsValidator;
            this.IsBusy = false;

            AddFileButtonClickCommand = ReactiveCommand.Create(OnAddFileButtonClick);
            AddFolderButtonClickCommand = ReactiveCommand.Create(OnAddFolderButtonClick);
            CancelButtonClickCommand = ReactiveCommand.Create(OnCancelButtonClick);
            ResizeButtonClickCommand = ReactiveCommand.Create(OnResizeButtonClick);
        }

        #region Commands

        public ReactiveCommand<Unit, Unit> AddFileButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> AddFolderButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> ResizeButtonClickCommand { get; }

        #endregion

        #region Properties

        public ImageResizeOptionsViewModel ImageResizeOptionsViewModel { get; set; }

        public uint ImageResizeProgressValue
        {
            get => _imageResizeProgressValue;
            private set => this.RaiseAndSetIfChanged(ref _imageResizeProgressValue, value);
        }

        public string InfoText
        {
            get => _infoText;
            private set => this.RaiseAndSetIfChanged(ref _infoText, value);
        }

        public bool IsBatchResizeInProgress
        {
            get => _isBatchResizeInProgress;
            private set => this.RaiseAndSetIfChanged(ref _isBatchResizeInProgress, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public ImagePreviewViewModel? PreviewImageModel
        {
            get => _previewImageModel;
            set
            {
                this.RaiseAndSetIfChanged(ref _previewImageModel, value);
            }
        }

        public ObservableCollection<ImageViewModel> SelectedImages { get; set; } = new ObservableCollection<ImageViewModel>();


        #endregion

        private async void OnAddFileButtonClick()
        {
            var topLevel = TopLevel.GetTopLevel(WindowUtils.GetMainWindow());

            // Start async operation to open the dialog.
            var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Image File",
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll },
                AllowMultiple = true
            });

            if (files.Count > 0)
            {
                await AddStorageItems(files);
            }
        }

        private async void OnAddFolderButtonClick()
        {
            var topLevel = TopLevel.GetTopLevel(WindowUtils.GetMainWindow());

            // Start async operation to open the dialog.
            var folder = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Folder",
                AllowMultiple = false
            });
            if (folder.Count > 0)
            {
                await AddStorageItems(folder);
            }
        }


        private void OnCancelButtonClick()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async void OnResizeButtonClick()
        {
            if (this.SelectedImages.Count == 0) return;

            var topLevel = TopLevel.GetTopLevel(WindowUtils.GetMainWindow());

            // Start async operation to open the dialog.
            var folder = await topLevel!.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Output Folder",
                AllowMultiple = false
            });
            if (folder.Count > 0)
            {
                string path = folder.First().TryGetLocalPath()!;
                await ResizeImages(path);
            }

        }

        private void AddFile(FileInfo fileInfo)
        {
            Dispatcher.UIThread.Invoke(() => {
                SelectedImages.Add(new ImageViewModel()
                {
                    FilePath = fileInfo.FullName,
                    Image = new Avalonia.Media.Imaging.Bitmap(fileInfo.FullName),
                    Name = fileInfo.Name,
                    FileSize = _fileSystemProvider.GetFileSizeReadable(fileInfo.Length)
                });
            });
        }

        public void RemoveImages(IEnumerable<ImageViewModel> imageViewModels)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                foreach (var item in imageViewModels)
                {
                    this.SelectedImages.Remove(item);
                    if (this.PreviewImageModel != null && this.PreviewImageModel.Path == item.FilePath)
                    {
                        this.PreviewImageModel = null;
                    }
                }
            });
            ResetInfoText();
        }

        public async Task AddStorageItems(IEnumerable<IStorageItem> storageItems)
        {
            int totalItemCount = 0;
            uint imageCount = 0;

            try
            {
                this.IsBusy = true;
                _cancellationTokenSource = new CancellationTokenSource();

                await Task.Run(() =>
                {

                    // enumerate the storage items
                    IPerformanceLogger perfLogger = PerformanceLogger.CreateAndStart<BatchResizerPanelViewModel>("AddFiles");
                    List<string> filesToAdd = new List<string>();

                    foreach (var item in storageItems)
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        string? itemPath = item.TryGetLocalPath();
                        if (itemPath == null) continue;
                        
                        bool isFile = _fileSystemProvider.FileExists(itemPath);

                        if (isFile)
                        {
                            filesToAdd.Add(itemPath!);
                            totalItemCount++;
                            continue;
                        }

                        bool isFolder = _fileSystemProvider.DirectoryExists(itemPath);
                        if (isFolder)
                        {
                            UpdateProgress($"Enumerating folder {itemPath}...", 0);
                            IEnumerable<string> files = _fileSystemProvider.EnumerateFiles(itemPath, "*.*", SearchOption.AllDirectories);
                            filesToAdd.AddRange(files);
                            totalItemCount += files.Count();
                        }
                    }

                    UpdateProgress($"{totalItemCount} files found, scanning for images...", 0);

                    foreach (string filePath in filesToAdd)
                    {
                        if (SelectedImages.Any(x => x.FilePath == filePath))
                        {
                            continue;
                        }

                        FileInfo fileInfo = new FileInfo(filePath);

                        if (_imageProcessor.IsImageExtension(fileInfo.Extension))
                        {
                            AddFile(fileInfo);
                            imageCount++;
                        }

                    }

                    perfLogger.Stop($"Processed {totalItemCount} files; {imageCount} images found");
                });
            }
            catch (OperationCanceledException)
            {
                _logger.Info("Image load operation cancelled");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                await WindowUtils.ShowErrorDialog("Error", $"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                ResetInfoText();
                this.IsBusy = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private string GetOutputThumbnailFilename(string filePath, string outFolder, ImageResizeOptions options)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string thumbFilePath = Path.Combine(outFolder, $"{fileName}_tn{extension}");
            return GetOutputFilename(thumbFilePath, outFolder, options);
        }

        private string GetOutputFilename(string filePath, string outFolder, ImageResizeOptions options)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            if (options.ReplaceSpacesWithUnderscores)
            {
                fileName = fileName.Replace(" ", "_");
            }

            string newFilePath = Path.Combine(outFolder, $"{fileName}{extension}");

            if (!options.OverwriteFiles)
            {
                int num = 1;
                while (_fileSystemProvider.FileExists(newFilePath))
                {
                    newFilePath = Path.Combine(outFolder, $"{fileName}_{num++}{extension}");
                }
            }

            return newFilePath;
        }

        private void ResetInfoText()
        {
            UpdateProgress($"{SelectedImages.Count} images loaded for resizing.", 0);
        }

        private async Task ResizeImages(string folder)
        {

            try
            {
                this.IsBatchResizeInProgress = true;
                this.IsBusy = true;

                UpdateProgress("Preparing to resize images...", 0);

                await Task.Run(() =>
                {
                    ImageResizeOptions options = ImageResizeOptions.FromViewModel(this.ImageResizeOptionsViewModel);
                    _imageResizeOptionsValidator.Validate(options);

                    uint num = 1;
                    foreach (var imageViewModel in SelectedImages)
                    {
                        UpdateProgress($"Resizing {imageViewModel.Name}", num++);

                        string outputPath = GetOutputFilename(imageViewModel.FilePath, folder, options);
                        _imageProcessor.ResizeImage(imageViewModel.FilePath, options.MaxImageLength, outputPath);

                        if (options.GenerateThumbnails)
                        {
                            string thumbPath = GetOutputThumbnailFilename(imageViewModel.FilePath, folder, options);
                            _imageProcessor.ResizeImage(imageViewModel.FilePath, options.MaxThumbnailLength, thumbPath);
                        }
                    }

                });

            }
            catch (OperationCanceledException)
            {
                _logger.Info("Resize operation cancelled");
            }
            catch (ValidationException ex)
            {
                await WindowUtils.ShowErrorDialog("Validation Error", ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                UpdateProgress($"Error: {ex.Message}", 0);
                await WindowUtils.ShowErrorDialog("Resize Error", $"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                this.IsBatchResizeInProgress = false;
                this.IsBusy = false;
            }
        } 

        private void UpdateProgress(string infoText, uint progress)
        {
            Dispatcher.UIThread.Post(() => {
                this.InfoText = infoText;
                this.ImageResizeProgressValue = progress;
            });
        }


    }

    #region Design time mode

    public class BatchResizerPanelViewModelDesign : BatchResizerPanelViewModel
    {
        public BatchResizerPanelViewModelDesign() : base(new ImageResizeOptionsViewModel()
            , new FileSystemProvider()
            , new ImageProcessor()
            , new ImageResizeOptionsValidator()
            )
        {
        }
    }

    #endregion
}
