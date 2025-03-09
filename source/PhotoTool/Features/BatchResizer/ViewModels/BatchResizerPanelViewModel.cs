using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using PhotoTool.Shared.Comparers;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.Logging;
using PhotoTool.Shared.UI;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private uint _imageResizeProgressValue = 0;

        public BatchResizerPanelViewModel(IFileSystemProvider fileSystemProvider, IImageProcessor imageProcessor)
        {
            _fileSystemProvider = fileSystemProvider;
            _imageProcessor = imageProcessor;

            this.IsBusy = false;

            AddFolderButtonClickCommand = ReactiveCommand.Create(OnAddFolderButtonClick);
            CancelButtonClickCommand = ReactiveCommand.Create(OnCancelButtonClick);

        }

        #region Commands

        public ReactiveCommand<Unit, Unit> AddFolderButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelButtonClickCommand { get; }


        #endregion

        #region Properties

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

        public ObservableCollection<ImageViewModel> SelectedImages { get; set; } = new ObservableCollection<ImageViewModel>();


        #endregion

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
                string? path = folder.First().TryGetLocalPath();
                await AddFolder(path!);
            }
        }

        private void OnCancelButtonClick()
        {
            _cancellationTokenSource?.Cancel();
        }


        private async Task AddFolder(string folder)
        {
            uint totalFileCount = 0;
            uint imageCount = 0;

            try
            {
                this.IsBusy = true;
                _cancellationTokenSource = new CancellationTokenSource();
                this.InfoText = "Searching for images";
                this.SelectedImages.Clear();

                await Task.Run(() =>
                {

                    // enumerate files
                    IEnumerable<string> files = _fileSystemProvider.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories);

                    // enumerate the files in the selected folder
                    IPerformanceLogger perfLogger = PerformanceLogger.CreateAndStart<BatchResizerPanelViewModel>("FolderImageSearch");
                    foreach (var file in files)
                    {
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        if (SelectedImages.Any(x => x.FilePath == file))
                        {
                            continue;
                        }

                        FileInfo fileInfo = new FileInfo(file);

                        if (_imageProcessor.IsImageExtension(fileInfo.Extension))
                        {
                            SelectedImages.Add(new ImageViewModel()
                            {
                                FilePath = file,
                                Image = new Avalonia.Media.Imaging.Bitmap(file),
                                Name = fileInfo.Name
                            });
                            imageCount++;
                        }

                        totalFileCount++;
                        UpdateProgress($"Searching {folder} for images...{imageCount} images found of {totalFileCount} files.", 0);
                    }
                    perfLogger.Stop($"Processed {totalFileCount} files in '{folder}'; {imageCount} images found");

                });
            }
            catch (OperationCanceledException)
            {
                _logger.Info("Image search operation cancelled");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                await WindowUtils.ShowErrorDialog("Error", $"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                UpdateProgress($"{SelectedImages.Count} images selected for resizing.", 0);
                this.IsBusy = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
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
}
