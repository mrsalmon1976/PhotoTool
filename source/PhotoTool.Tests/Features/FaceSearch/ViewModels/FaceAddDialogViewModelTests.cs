using Avalonia.Headless.NUnit;
using FaceAiSharp;
using NSubstitute;
using NUnit.Framework;
using PhotoTool.Features.BatchResizer.Validators;
using PhotoTool.Features.BatchResizer.ViewModels;
using PhotoTool.Features.FaceSearch.Repositories;
using PhotoTool.Features.FaceSearch.Services;
using PhotoTool.Features.FaceSearch.ViewModels;
using PhotoTool.Features.FaceSearch.Views;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.Resources;
using PhotoTool.Shared.UI;
using PhotoTool.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Test]
        public async Task LoadImage_FacesFound_AddsToDetectedFaces()
        {
            Assert.Fail();
        }

        [Test]
        public async Task LoadImage_WhenCompleteAndNoFacesDetected_ResetsControlProperties()
        {
            Assert.Fail();
        }

        [Test]
        public async Task LoadImage_WhenCompleteAndFacesDetected_ResetsControlProperties()
        {
            Assert.Fail();
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
