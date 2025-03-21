using Avalonia.Headless.NUnit;
using Avalonia.Media.Imaging;
using NSubstitute;
using NUnit.Framework;
using PhotoTool.Features.FaceSearch.Constants;
using PhotoTool.Features.FaceSearch.Models;
using PhotoTool.Features.FaceSearch.Repositories;
using PhotoTool.Features.FaceSearch.Services;
using PhotoTool.Features.FaceSearch.ViewModels;
using PhotoTool.Features.FaceSearch.Views;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.Resources;
using PhotoTool.Shared.UI;
using PhotoTool.Shared.ViewModels;
using PhotoTool.Test;
using PhotoTool.Tests.Random;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Tests.Features.FaceSearch.ViewModels
{
    [TestFixture]
    public class FaceSearchPanelViewModelTests
    {

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private IViewModelProvider _viewModelProvider = null;
        private IUIProvider _uiProvider = null;
        private IFaceRepository _faceRepo = null;
        private IImageProcessor _imageProcessor = null;
        private IFileSystemProvider _fileSystemProvider = null;
        private IFaceDetectionService _faceDetectionService = null;
        private IAssetProvider _assetProvider = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        [SetUp]
        public void SetUp()
        {
            _viewModelProvider = new SubstituteBuilder<IViewModelProvider>().WithRandomProperties().Build();
            _uiProvider = new SubstituteBuilder<IUIProvider>().Build();
            _faceRepo = new SubstituteBuilder<IFaceRepository>().Build();
            _imageProcessor = new SubstituteBuilder<IImageProcessor>().Build();
            _fileSystemProvider = new SubstituteBuilder<IFileSystemProvider>().Build();
            _faceDetectionService = new SubstituteBuilder<IFaceDetectionService>().Build();
            _assetProvider = new SubstituteBuilder<IAssetProvider>().WithRandomProperties().Build();
        }

        [AvaloniaTest]  
        public async Task LoadFaces_WhenFacesLoaded_AddsToSearchResults()
        {
            // Arrange
            var searchPath = $"C:\\Temp\\{Guid.NewGuid()}";
            int imageCount = RandomData.Number.Next(3, 7);

            var viewModel = CreateFaceSearchPanelViewModel(true);
            viewModel.SearchPath = searchPath;
            viewModel.SelectedFace = new SubstituteBuilder<SavedFaceViewModel>().WithRandomProperties().Build();

            _fileSystemProvider.DirectoryExists(searchPath).Returns(true);

            var files = TestDataUtils.CreateMany<IFileInfoWrapper>(imageCount);
            _fileSystemProvider.EnumerateFiles(searchPath, "*.*", SearchOption.AllDirectories).Returns(files);

            _imageProcessor.IsImageExtension(Arg.Any<string>()).Returns(true);

            float[] faceEmbedding = new float[] { 1 ,2, 3 };
            _faceDetectionService.GetFaceEmbedding(Arg.Any<Bitmap>()).Returns(faceEmbedding);

            FaceComparison faceComparison = new SubstituteBuilder<FaceComparison>().WithProperty(x => x.DotProduct, 0.5f).Build();
            _faceDetectionService.SearchForFace(faceEmbedding, Arg.Any<string>()).Returns(faceComparison);

            // Act
            await viewModel.SearchFaces();

            // Assert
            _fileSystemProvider.Received(1).DirectoryExists(viewModel.SearchPath);
            Assert.That(viewModel.SearchImageCount, Is.EqualTo(imageCount));
            Assert.That(viewModel.SearchResults.Count, Is.EqualTo(imageCount));
        }

        [AvaloniaTest]
        public async Task LoadFaces_WhenFacesLoaded_CorrectlySetsControlProperties()
        {
            // Arrange
            var searchPath = $"C:\\Temp\\{Guid.NewGuid()}";
            int imageCount = RandomData.Number.Next(2, 5);

            var viewModel = CreateFaceSearchPanelViewModel(true);
            viewModel.SearchPath = searchPath;
            viewModel.SelectedFace = new SubstituteBuilder<SavedFaceViewModel>().WithRandomProperties().Build();

            _fileSystemProvider.DirectoryExists(searchPath).Returns(true);

            var files = TestDataUtils.CreateMany<IFileInfoWrapper>(imageCount);
            _fileSystemProvider.EnumerateFiles(searchPath, "*.*", SearchOption.AllDirectories).Returns(files);

            _imageProcessor.IsImageExtension(Arg.Any<string>()).Returns(true);

            FaceComparison faceComparison = new SubstituteBuilder<FaceComparison>().WithProperty(x => x.DotProduct, 0.5f).Build();
            _faceDetectionService.SearchForFace(Arg.Any<float[]>(), Arg.Any<string>()).Returns(faceComparison);

            // Act
            await viewModel.SearchFaces();

            // Assert
            Assert.That(viewModel.SearchImageCount, Is.EqualTo(imageCount));
            Assert.That(viewModel.SearchResults.Count, Is.EqualTo(imageCount));
            Assert.That(viewModel.IsSearchActive, Is.False);
            Assert.That(viewModel.IsDeleteButtonEnabled, Is.True);
        }

        [AvaloniaTest]
        public async Task LoadFaces_WhenNoFacesLoaded_CorrectlySetsControlProperties()
        {
            // Arrange
            var searchPath = $"C:\\Temp\\{Guid.NewGuid()}";

            var viewModel = CreateFaceSearchPanelViewModel(true);
            viewModel.SearchPath = searchPath;
            viewModel.SelectedFace = new SubstituteBuilder<SavedFaceViewModel>().WithRandomProperties().Build();

            _fileSystemProvider.DirectoryExists(searchPath).Returns(true);

            var files = new List<IFileInfoWrapper>();
            _fileSystemProvider.EnumerateFiles(searchPath, "*.*", SearchOption.AllDirectories).Returns(files);

            _imageProcessor.IsImageExtension(Arg.Any<string>()).Returns(true);

            FaceComparison faceComparison = new SubstituteBuilder<FaceComparison>().WithProperty(x => x.DotProduct, 0.5f).Build();
            _faceDetectionService.SearchForFace(Arg.Any<float[]>(), Arg.Any<string>()).Returns(faceComparison);

            // Act
            await viewModel.SearchFaces();

            // Assert
            Assert.That(viewModel.SearchImageCount, Is.EqualTo(0));
            Assert.That(viewModel.SearchResults.Count, Is.EqualTo(0));
            Assert.That(viewModel.IsSearchActive, Is.False);
            Assert.That(viewModel.IsDeleteButtonEnabled, Is.True);
        }

        [Test]
        public async Task SearchFaces_WhenInvalidSearchEntries_ShowsErrorDialog()
        {
            // Arrange
            var viewModel = CreateFaceSearchPanelViewModel(false);

            // Act
            await viewModel.SearchFaces();

            // Assert
            await _uiProvider.Received(1).ShowErrorDialog("Validation Error", "Please select a folder and face to search.");
        }

        [AvaloniaTest]
        public async Task SearchFaces_WhenImagesProcessed_FaceDetectionExecutedOnAllImages()
        {
            // Arrange
            var searchPath = $"C:\\Temp\\{Guid.NewGuid()}";
            int imageCount = RandomData.Number.Next(3, 7);

            var viewModel = CreateFaceSearchPanelViewModel(true);
            viewModel.SearchPath = searchPath;
            viewModel.SelectedFace = new SubstituteBuilder<SavedFaceViewModel>().WithRandomProperties().Build();

            _fileSystemProvider.DirectoryExists(searchPath).Returns(true);

            var files = TestDataUtils.CreateMany<IFileInfoWrapper>(imageCount);
            _fileSystemProvider.EnumerateFiles(searchPath, "*.*", SearchOption.AllDirectories).Returns(files);

            _imageProcessor.IsImageExtension(Arg.Any<string>()).Returns(true);

            float[] faceEmbedding = new float[] { 1, 2, 3 };
            _faceDetectionService.GetFaceEmbedding(Arg.Any<Bitmap>()).Returns(faceEmbedding);

            FaceComparison faceComparison = new SubstituteBuilder<FaceComparison>().WithProperty(x => x.DotProduct, 0.5f).Build();
            _faceDetectionService.SearchForFace(faceEmbedding, Arg.Any<string>()).Returns(faceComparison);

            // Act
            await viewModel.SearchFaces();

            // Assert
            _faceDetectionService.Received(1).GetFaceEmbedding(Arg.Any<Bitmap>());
            _faceDetectionService.Received(imageCount).SearchForFace(Arg.Any<float[]>(), Arg.Any<string>());
        }

        [AvaloniaTest]
        public async Task SearchFaces_WhenImagesProcessed_OnlyImagesWithValidFaceDetectionsAdded()
        {
            // Arrange
            var searchPath = $"C:\\Temp\\{Guid.NewGuid()}";
            const int imageCount = 2;

            var viewModel = CreateFaceSearchPanelViewModel(true);
            viewModel.SearchPath = searchPath;
            viewModel.SelectedFace = new SubstituteBuilder<SavedFaceViewModel>().WithRandomProperties().Build();

            _fileSystemProvider.DirectoryExists(searchPath).Returns(true);

            var files = new List<IFileInfoWrapper>()
            {
                new SubstituteBuilder<IFileInfoWrapper>().WithRandomProperties().Build(),
                new SubstituteBuilder<IFileInfoWrapper>().WithRandomProperties().Build()
            };
            _fileSystemProvider.EnumerateFiles(searchPath, "*.*", SearchOption.AllDirectories).Returns(files);

            _imageProcessor.IsImageExtension(Arg.Any<string>()).Returns(true);

            FaceComparison faceComparisonSuccess = new SubstituteBuilder<FaceComparison>().WithProperty(x => x.DotProduct, 0.5f).Build();
            FaceComparison faceComparisonFail = new SubstituteBuilder<FaceComparison>().WithProperty(x => x.DotProduct, 0.1f).Build();
            _faceDetectionService.SearchForFace(Arg.Any<float[]>(), files[0].FullName).Returns(faceComparisonSuccess);
            _faceDetectionService.SearchForFace(Arg.Any<float[]>(), files[1].FullName).Returns(faceComparisonFail);

            // Act
            await viewModel.SearchFaces();

            // Assert
            Assert.That(viewModel.SearchImageCount, Is.EqualTo(imageCount));
            Assert.That(viewModel.SearchResults.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task SearchFaces_WhenUnhandledExceptionOccurs_DialogShown()
        {
            // Arrange
            Exception ex = new Exception("Test exception");
            _fileSystemProvider.When(x => x.EnumerateFiles(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<SearchOption>())).Throw(ex);

            var viewModel = CreateFaceSearchPanelViewModel(false);
            viewModel.SearchPath = "C:\\Test";
            viewModel.SelectedFace = new SavedFaceViewModel();

            _fileSystemProvider.DirectoryExists(Arg.Any<string>()).Returns(true);

            // Act
            await viewModel.SearchFaces();

            // Assert
            _fileSystemProvider.Received(1).DirectoryExists(Arg.Any<string>());
            await _uiProvider.Received(1).ShowErrorDialog("Search Error", $"An unexpected error occurred: {ex.Message}");

        }

        private FaceSearchPanelViewModel CreateFaceSearchPanelViewModel(bool useTestUiProvider)
        {
            if (useTestUiProvider)
            {
                _uiProvider = new TestUIProvider();
            }
            return new FaceSearchPanelViewModel(_viewModelProvider, _uiProvider, _faceRepo, _imageProcessor, _fileSystemProvider, _faceDetectionService, _assetProvider);
        }

    }
}
