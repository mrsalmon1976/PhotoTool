﻿using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;
using Avalonia.Platform.Storage;
using Avalonia.Media.Imaging;
using System;
using PhotoTool.Features.FaceSearch.Views;
using System.IO;
using System.Collections.ObjectModel;
using Avalonia.Media;
using PhotoTool.Shared.Configuration;
using PhotoTool.Shared.Constants;
using PhotoTool.Shared.Logging;
using PhotoTool.Features.FaceSearch.Models;
using PhotoTool.Shared.Resources;
using PhotoTool.Features.FaceSearch.Repositories;
using PhotoTool.Features.FaceSearch.Services;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.UI;

namespace PhotoTool.Features.FaceSearch.ViewModels
{
    public partial class FaceAddDialogViewModel : ReactiveObject
    {

        private static IAppLogger _logger = AppLogger.Create<FaceAddDialogViewModel>();

        private readonly IFaceDetector _faceDetector;
        private readonly IFaceRepository _faceRepo;
        private bool _isImageSelected;
        private bool _isSaveButtonEnabled;
        private Bitmap? _selectedImage = null;

        public FaceAddDialogViewModel(IAssetProvider assetProvider, IFaceDetector faceDetectionService, IFaceRepository faceRepo)
        {
            SaveFacesButtonClickCommand = ReactiveCommand.Create(OnSaveFacesButtonClickCommand);
            SelectFileButtonClickCommand = ReactiveCommand.Create(OnSelectFileButtonClick);
            SelectedImage = assetProvider.GetImage(Assets.PhotoToolLogo_300x300_800bg);
            _faceDetector = faceDetectionService;
            _faceRepo = faceRepo;
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
            foreach (var detectedFace in DetectedFaces)
            {
                if (string.IsNullOrWhiteSpace(detectedFace.Name) || detectedFace.Image == null)
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

            var dialog = WindowUtils.GetWindow<FaceAddDialog>();

            if (facesSaved == 0)
            {
                await WindowUtils.ShowErrorDialog("No Named Faces", "You need to assign names to at least one face.", dialog);
            }
            else
            {
                var message = facesSaved == 1 ? "face has" : "faces have";
                message = $"{facesSaved} {message} been successfully saved";
                await WindowUtils.ShowWarningDialog("Faces Saved", message, dialog);
                dialog!.Close();
            }

        }

        private async void OnSelectFileButtonClick()
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            //var topLevel = TopLevel.GetTopLevel(this);
            var topLevel = TopLevel.GetTopLevel(WindowUtils.GetMainWindow());

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
                    var result = _faceDetector.DecorateImageWithFaceDetections(filePath);
                    profiler.Stop();

                    // reset the detected faces
                    DetectedFaces.Clear();
                    result.Faces.ForEach(f =>
                    {
                        Bitmap faceImage = new Bitmap(new MemoryStream(f.ImageData!));
                        uint brushColor = (uint)f.Color;
                        DetectedFaces.Add(new FaceAddViewModel() { Name = string.Empty, Image = faceImage, ColorBrush = new SolidColorBrush(brushColor) });
                    });


                    SelectedImage = new Bitmap(new MemoryStream(result.DecoratedImageData));
                    IsImageSelected = true;
                    IsSaveButtonEnabled = DetectedFaces.Count > 0;
                }
                catch (Exception ex)
                {
                    string message = $"An error occurred loading the selected image: {ex.Message}";
                    var dialog = WindowUtils.GetWindow<FaceAddDialog>();
                    await WindowUtils.ShowErrorDialog("Image Load Error", message, dialog);

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
            new FaceDetector(new ImageProcessor()),
            new FaceRepository(new AppSettings(), new FileSystemProvider())
            )
        {
        }
    }

    #endregion
}
