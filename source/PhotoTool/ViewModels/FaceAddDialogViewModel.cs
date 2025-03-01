using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;
using Avalonia.Platform.Storage;
using PhotoTool.Utilities;
using Avalonia.Media.Imaging;
using PhotoTool.Providers;
using PhotoTool.Constants;
using System;
using MsBox.Avalonia;
using PhotoTool.Views.FaceSearch;
using PhotoTool.Services;
using System.IO;
using PhotoTool.Logging;
using System.Collections.ObjectModel;
using PhotoTool.Models.FaceSearch;
using Avalonia.Media;
using PhotoTool.Repositories;
using PhotoTool.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Tmds.DBus.Protocol;

namespace PhotoTool.ViewModels
{
    public partial class FaceAddDialogViewModel : ReactiveObject
    {

        private static IAppLogger _logger = AppLogger.Create<FaceAddDialogViewModel>();

        private readonly IFaceDetectionService _faceDetectionService;
        private readonly IFaceRepository _faceRepo;
        private bool _isImageSelected;
        private bool _isSaveButtonEnabled;
        private Bitmap? _selectedImage = null;

        public FaceAddDialogViewModel(IAssetProvider assetProvider, IFaceDetectionService faceDetectionService, IFaceRepository faceRepo)
        {
            SaveFacesButtonClickCommand = ReactiveCommand.Create(OnSaveFacesButtonClickCommand);
            SelectFileButtonClickCommand = ReactiveCommand.Create(OnSelectFileButtonClick);
            SelectedImage = assetProvider.GetImage(Assets.PhotoToolLogo_300x300_800bg);
            this._faceDetectionService = faceDetectionService;
            this._faceRepo = faceRepo;
        }

        #region Control Properties

        public bool IsImageSelected
        {
            get => _isImageSelected;
            private set => this.RaiseAndSetIfChanged(ref _isImageSelected, value);
        }

        public bool IsSaveButtonEnabled
        {
            get => _isSaveButtonEnabled;
            private set => this.RaiseAndSetIfChanged(ref _isSaveButtonEnabled, value);
        }

        public Bitmap? SelectedImage
        {
            get => _selectedImage;
            private set => this.RaiseAndSetIfChanged(ref _selectedImage, value);
        }

        public ObservableCollection<FaceAddViewModel> DetectedFaces { get; set; } = new ObservableCollection<FaceAddViewModel>();



        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> SaveFacesButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectFileButtonClickCommand { get; }

        private async void OnSaveFacesButtonClickCommand()
        {


            int facesSaved = 0;
            foreach (var detectedFace in this.DetectedFaces)
            {
                if (String.IsNullOrWhiteSpace(detectedFace.Name) || detectedFace.Image == null)
                {
                    continue;
                }

                FaceModel faceModel = new FaceModel()
                {
                    ImageData = Convert.ToBase64String(detectedFace.GetImageDataAsByteArray()!),
                    Name = detectedFace.Name
                };

                await _faceRepo.SaveAsync(faceModel);
                facesSaved++;
            }

            var dialog = AppUtils.GetWindow<FaceAddDialog>();

            if (facesSaved == 0)
            {
                await AppUtils.ShowErrorDialog("No Named Faces", "You need to assign names to at least one face.", dialog);
            }
            else
            {
                var message = (facesSaved == 1 ? "face has" : "faces have");
                message = $"{facesSaved} {message} been successfully saved";
                await AppUtils.ShowWarningDialog("Faces Saved", message, dialog);
                dialog!.Close();
            }

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

                    // reset the detected faces
                    DetectedFaces.Clear();
                    result.Faces.ForEach(f =>
                    {
                        Bitmap faceImage = new Bitmap(new MemoryStream(f.ImageData!));
                        uint brushColor = (uint)f.Color;
                        DetectedFaces.Add(new FaceAddViewModel() { Name = String.Empty, Image = faceImage, ColorBrush = new SolidColorBrush(brushColor) });
                    });


                    this.SelectedImage = new Bitmap(new MemoryStream(result.DecoratedImageData));
                    this.IsImageSelected = true;
                    this.IsSaveButtonEnabled = (DetectedFaces.Count > 0);
                }
                catch (Exception ex)
                {
                    string message = $"An error occurred loading the selected image: {ex.Message}";
                    var dialog = AppUtils.GetWindow<FaceAddDialog>();
                    await AppUtils.ShowErrorDialog("Image Load Error", message, dialog);

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
        public FaceAddDialogViewModelDesign() : base(
            new AssetProvider(),
            new FaceDetectionService(new ImageService()),
            new FaceRepository(new AppSettings(), new FileService())
            )
        {
        }
    }

    #endregion
}
