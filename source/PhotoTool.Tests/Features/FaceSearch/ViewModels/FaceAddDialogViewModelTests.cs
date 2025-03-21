using Avalonia.Headless.NUnit;
using NSubstitute;
using NUnit.Framework;
using PhotoTool.Features.FaceSearch.Models;
using PhotoTool.Features.FaceSearch.Repositories;
using PhotoTool.Features.FaceSearch.Services;
using PhotoTool.Features.FaceSearch.ViewModels;
using PhotoTool.Features.FaceSearch.Views;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.Resources;
using PhotoTool.Shared.UI;
using PhotoTool.Test;
using PhotoTool.Tests.Random;

namespace PhotoTool.Tests.Features.FaceSearch.ViewModels
{
    [TestFixture]
    public class FaceAddDialogViewModelTests
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private IAssetProvider _assetProvider = null;
        private IUIProvider _uiProvider = null;
        private IFaceDetectionService _faceDetectionService = null;
        private IFaceRepository _faceRepo = null;
        private IImageProcessor _imageProcessor = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        [SetUp]
        public void SetUp()
        {
            _assetProvider = new SubstituteBuilder<IAssetProvider>().WithRandomProperties().Build();
            _uiProvider = new SubstituteBuilder<IUIProvider>().Build();
            _faceDetectionService = new SubstituteBuilder<IFaceDetectionService>().Build();
            _faceRepo = new SubstituteBuilder<IFaceRepository>().Build();
            _imageProcessor = new SubstituteBuilder<IImageProcessor>().Build();
        }

        [AvaloniaTest]
        public async Task LoadImage_FacesFound_AddsToDetectedFaces()
        {
            // Arrange
            string filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            int detectedFaceCount = RandomData.Number.Next(3, 7);

            FaceDetectionResult faceDetectionResult = new FaceDetectionResult();
            faceDetectionResult.Faces.AddRange(TestDataUtils.CreateMany<FaceDetectionResultItem>(detectedFaceCount));

            _faceDetectionService.DecorateImageWithFaceDetections(filePath).Returns(faceDetectionResult);

            // Act
            var viewModel = CreateFaceAddDialogViewModel(false);
            await viewModel.LoadImage(filePath);

            // Assert
            Assert.That(viewModel.DetectedFaces.Count, Is.EqualTo(detectedFaceCount));
        }

        [AvaloniaTest]
        public async Task LoadImage_WhenCompleteAndNoFacesDetected_ResetsControlProperties()
        {
            // Arrange
            string filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            FaceDetectionResult faceDetectionResult = new FaceDetectionResult();

            _faceDetectionService.DecorateImageWithFaceDetections(filePath).Returns(faceDetectionResult);

            // Act
            var viewModel = CreateFaceAddDialogViewModel(false);
            await viewModel.LoadImage(filePath);

            // Assert
            Assert.That(viewModel.IsImageSelected, Is.True);
            Assert.That(viewModel.IsSaveButtonEnabled, Is.False);
        }

        [AvaloniaTest]
        public async Task LoadImage_WhenCompleteAndFacesDetected_ResetsControlProperties()
        {
            // Arrange
            string filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            int detectedFaceCount = RandomData.Number.Next(1, 3);

            FaceDetectionResult faceDetectionResult = new FaceDetectionResult();
            faceDetectionResult.Faces.AddRange(TestDataUtils.CreateMany<FaceDetectionResultItem>(detectedFaceCount));

            _faceDetectionService.DecorateImageWithFaceDetections(filePath).Returns(faceDetectionResult);

            // Act
            var viewModel = CreateFaceAddDialogViewModel(false);
            await viewModel.LoadImage(filePath);

            // Assert
            Assert.That(viewModel.IsImageSelected, Is.True);
            Assert.That(viewModel.IsSaveButtonEnabled, Is.True);
        }

        [AvaloniaTest]
        public async Task LoadImage_WhenUnhandledExceptionOccurs_ShowsErrorDialog()
        {
            // Arrange
            string filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var dialog = new Avalonia.Controls.Window();
            _uiProvider.GetWindow<FaceAddDialog>().Returns(dialog);

            Exception ex = new Exception("Test exception");
            _faceDetectionService.When(x => x.DecorateImageWithFaceDetections(Arg.Any<string>())).Throw(ex);

            // Act
            var viewModel = CreateFaceAddDialogViewModel(false);
            await viewModel.LoadImage(filePath);

            // Assert
            await _uiProvider.Received(1).ShowErrorDialog("Image Load Error", $"An error occurred loading the selected image: {ex.Message}", dialog);
        }

        private FaceAddDialogViewModel CreateFaceAddDialogViewModel(bool useTestUiProvider)
        {
            if (useTestUiProvider)
            {
                _uiProvider = new TestUIProvider();
            }
            return new FaceAddDialogViewModel(_assetProvider, _uiProvider, _faceDetectionService, _faceRepo, _imageProcessor);
        }

    }
}
