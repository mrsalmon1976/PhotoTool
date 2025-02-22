using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;
using Avalonia.Platform.Storage;
using PhotoToolAvalonia.Utilities;
using Avalonia.Media.Imaging;
using PhotoToolAvalonia.Providers;
using PhotoToolAvalonia.Constants;
using System;
using MsBox.Avalonia;
using PhotoToolAvalonia.Views.FaceSearch;
using PhotoToolAvalonia.Services;
using System.IO;
using PhotoToolAvalonia.Logging;

namespace PhotoToolAvalonia.ViewModels
{
    public partial class FaceAddDialogViewModel : ReactiveObject
    {

        private static IAppLogger _logger = AppLogger.Create<FaceAddDialogViewModel>();

        private readonly IFaceDetectionService _faceDetectionService;
        private bool _isImageSelected;
        private Bitmap? _selectedImage = null;

        public FaceAddDialogViewModel(IAssetProvider assetProvider, IFaceDetectionService faceDetectionService)
        {
            SaveFacesButtonClickCommand = ReactiveCommand.Create(OnSaveFacesButtonClickCommand);
            SelectFileButtonClickCommand = ReactiveCommand.Create(OnSelectFileButtonClick);
            SelectedImage = assetProvider.GetImage(Assets.PhotoToolLogo_300x300_800bg);
            this._faceDetectionService = faceDetectionService;
        }

        #region Control Properties

        public bool IsImageSelected
        {
            get => _isImageSelected;
            private set => this.RaiseAndSetIfChanged(ref _isImageSelected, value);
        }

        public Bitmap? SelectedImage
        {
            get => _selectedImage;
            private set => this.RaiseAndSetIfChanged(ref _selectedImage, value);
        }



        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> SaveFacesButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectFileButtonClickCommand { get; }

        private async void OnSaveFacesButtonClickCommand()
        {

        }

        private async void OnSelectFileButtonClick()
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            //var topLevel = TopLevel.GetTopLevel(this);
            var topLevel = TopLevel.GetTopLevel(AppUtils.GetMainWindow());

            // Start async operation to open the dialog.
            var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Image File",
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll },
                AllowMultiple = false
            });

            if (files.Count >= 1)
            {
                try
                {
                    IPerformanceLogger profiler = PerformanceLogger.CreateAndStart<FaceAddDialogViewModel>("DecorateImageWithFaceDetections");
                    string filePath = files[0].Path.LocalPath;
                    var result = _faceDetectionService.DecorateImageWithFaceDetections(filePath);
                    profiler.Stop();

                    this.SelectedImage = new Bitmap(new MemoryStream(result.DecoratedImageData));
                    this.IsImageSelected = true;
                }
                catch (Exception ex)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("Image Load Error", $"An error occurred loading the selected image: {ex.Message}", MsBox.Avalonia.Enums.ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error, WindowStartupLocation.CenterScreen);
                    var parent = AppUtils.GetWindow<FaceAddDialog>();
                    await box.ShowWindowDialogAsync(parent);
                    // Handle errors - often due to permission issues
                    _logger.Error(ex, $"Image selection failed: {ex.Message}");
                }

            }

        }

        #endregion


    }

    #region Design time mode

    public class FaceAddDialogViewModelDesign : FaceAddDialogViewModel
    {
        public FaceAddDialogViewModelDesign() : base(new AssetProvider(), new FaceDetectionService(new ImageService()))
        {
        }
    }

    #endregion
}
