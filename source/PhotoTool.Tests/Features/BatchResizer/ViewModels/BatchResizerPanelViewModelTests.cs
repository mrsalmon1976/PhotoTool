using Avalonia.Headless.NUnit;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using PhotoTool.Features.BatchResizer.Validators;
using PhotoTool.Features.BatchResizer.ViewModels;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.UI;
using PhotoTool.Test;
using PhotoTool.Tests.Random;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Tests.Features.BatchResizer.ViewModels
{
    [TestFixture]
    public class BatchResizerPanelViewModelTests
    {
        #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        private ImageResizeOptionsViewModel _imageResizeOptionsViewModel = null;
        private IFileSystemProvider _fileSystemProvider = null;
        private IUIProvider _uiProvider = null;
        private IImageProcessor _imageProcessor = null;
        private IImageResizeOptionsValidator _imageResizeOptionsValidator = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        [SetUp]
        public void SetUp()
        {
            _imageResizeOptionsViewModel = new SubstituteBuilder<ImageResizeOptionsViewModel>().WithRandomProperties().Build();
            _fileSystemProvider = new SubstituteBuilder<IFileSystemProvider>().Build();
            _uiProvider = new SubstituteBuilder<IUIProvider>().Build();
            _imageProcessor = new SubstituteBuilder<IImageProcessor>().Build();
            _imageResizeOptionsValidator = new SubstituteBuilder<IImageResizeOptionsValidator>().Build();
        }

        #region AddStorageItems Tests

        [AvaloniaTest]
        public async Task AddStorageItems_FilesNotImages_NotAddedToSelectedItems()
        {
            // Arrange
            IStorageItem storageItem1 = new SubstituteBuilder<IStorageItem>().WithRandomProperties().Build();
            IStorageItem storageItem2 = new SubstituteBuilder<IStorageItem>().WithRandomProperties().Build();
            List<IStorageItem> storageItems = new List<IStorageItem> { storageItem1, storageItem2 };

            IFileInfoWrapper file1 = new SubstituteBuilder<IFileInfoWrapper>().WithRandomProperties().Build();
            IFileInfoWrapper file2 = new SubstituteBuilder<IFileInfoWrapper>().WithRandomProperties().Build();

            _fileSystemProvider.GetFileInfo(storageItem1).Returns(file1);
            _fileSystemProvider.GetFileInfo(storageItem2).Returns(file2);
            _imageProcessor.IsImageExtension(Arg.Any<string>()).Returns(false);

            // Act
            var viewModel = CreateBatchResizerPanelViewModel(true);
            await viewModel.AddStorageItems(storageItems);

            // Assert
            Assert.That(viewModel.SelectedImages.Count, Is.EqualTo(0));
            _imageProcessor.Received(2).IsImageExtension(Arg.Any<string>());
        }

        [AvaloniaTest]
        public async Task AddStorageItems_WhenFilesSelected_AddedToSelectedImages()
        {
            // Arrange
            IStorageItem storageItem1 = new SubstituteBuilder<IStorageItem>().WithRandomProperties().Build();
            IStorageItem storageItem2 = new SubstituteBuilder<IStorageItem>().WithRandomProperties().Build();
            List<IStorageItem> storageItems = new List<IStorageItem> { storageItem1, storageItem2 };

            IFileInfoWrapper file1 = new SubstituteBuilder<IFileInfoWrapper>().WithRandomProperties().Build();
            IFileInfoWrapper file2 = new SubstituteBuilder<IFileInfoWrapper>().WithRandomProperties().Build();
            
            _fileSystemProvider.GetFileInfo(storageItem1).Returns(file1);
            _fileSystemProvider.GetFileInfo(storageItem2).Returns(file2);
            _imageProcessor.IsImageExtension(Arg.Any<string>()).Returns(true);

            // Act
            var viewModel = CreateBatchResizerPanelViewModel(true);
            await viewModel.AddStorageItems(storageItems);

            // Assert
            Assert.That(viewModel.SelectedImages.Count, Is.EqualTo(2));
            Assert.That(viewModel.SelectedImages.FirstOrDefault(x => x.FilePath == file1.FullName), Is.Not.Null);
            Assert.That(viewModel.SelectedImages.FirstOrDefault(x => x.FilePath == file2.FullName), Is.Not.Null);
        }

        [AvaloniaTest]
        public async Task AddStorageItems_WhenFoldersSelected_AddedToSelectedImages()
        {
            // Arrange
            IStorageItem storageItem1 = new SubstituteBuilder<IStorageItem>().WithRandomProperties().Build();
            IStorageItem storageItem2 = new SubstituteBuilder<IStorageItem>().WithRandomProperties().Build();
            List<IStorageItem> storageItems = new List<IStorageItem> { storageItem1, storageItem2 };

            IDirectoryInfoWrapper dir1 = new SubstituteBuilder<IDirectoryInfoWrapper>().WithRandomProperties().Build();
            IDirectoryInfoWrapper dir2 = new SubstituteBuilder<IDirectoryInfoWrapper>().WithRandomProperties().Build();

            IEnumerable<IFileInfoWrapper> files1 = CreateFileInfoItems(1);
            IEnumerable<IFileInfoWrapper> files2 = CreateFileInfoItems(3);

            _fileSystemProvider.GetFileInfo(Arg.Any<IStorageItem>()).ReturnsNull();
            _fileSystemProvider.GetDirectoryInfo(storageItem1).Returns(dir1);
            _fileSystemProvider.GetDirectoryInfo(storageItem2).Returns(dir2);
            _fileSystemProvider.EnumerateFiles(dir1.FullName, "*.*", SearchOption.AllDirectories).Returns(files1);
            _fileSystemProvider.EnumerateFiles(dir2.FullName, "*.*", SearchOption.AllDirectories).Returns(files2);

            _imageProcessor.IsImageExtension(Arg.Any<string>()).Returns(true);

            // Act
            var viewModel = CreateBatchResizerPanelViewModel(true);
            await viewModel.AddStorageItems(storageItems);

            // Assert
            Assert.That(viewModel.SelectedImages.Count, Is.EqualTo(4));
            foreach (IFileInfoWrapper file in files1.Union(files2))
            {
                Assert.That(viewModel.SelectedImages.FirstOrDefault(x => x.FilePath == file.FullName), Is.Not.Null);

            }
        }

        [Test]
        public async Task AddStorageItems_WhenUnhandledExceptionThrown_ErrorDialogShown()
        {
            // Arrange
            IStorageItem storageItem1 = new SubstituteBuilder<IStorageItem>().WithRandomProperties().Build();
            IStorageItem storageItem2 = new SubstituteBuilder<IStorageItem>().WithRandomProperties().Build();
            List<IStorageItem> storageItems = new List<IStorageItem> { storageItem1, storageItem2 };

            Exception ex = new Exception("Test exception");


            _fileSystemProvider.When(x => x.GetFileInfo(Arg.Any<IStorageItem>())).Throw(ex);

            // Act
            var viewModel = CreateBatchResizerPanelViewModel(false);
            await viewModel.AddStorageItems(storageItems);


            // Assert
            await _uiProvider.Received(1).ShowErrorDialog("Error", $"An unexpected error occurred: {ex.Message}");
        }


        #endregion

        #region ResizeImages Tests

        [Test]
        public void ResizeImages_WhenNoImagesSelected_NothingHappens()
        {
            Assert.Fail();
        }

        [Test]
        public void ResizeImages_WhenImagesSelected_ImagesResized()
        {
            Assert.Fail();
        }

        [Test]
        public void ResizeImages_WhenGenerateThumbnailsTrue_ImagesResizedWithThumbnails()
        {
            Assert.Fail();
        }

        [Test]
        public void ResizeImages_WhenUnhandledExceptionThrown_DialogShown()
        {
            Assert.Fail();
        }


        #endregion

        #region Private Methods

        private BatchResizerPanelViewModel CreateBatchResizerPanelViewModel(bool useTestUiProvider)
        {
            if (useTestUiProvider)
            {
                _uiProvider = new TestUIProvider();
            }
            return new BatchResizerPanelViewModel(_imageResizeOptionsViewModel!, _fileSystemProvider, _uiProvider, _imageProcessor, _imageResizeOptionsValidator);
        }

        private IEnumerable<IFileInfoWrapper> CreateFileInfoItems(int count)
        {
            List<IFileInfoWrapper> files = new List<IFileInfoWrapper>();
            for (int i=0; i < count; i++)
            {
                files.Add(new SubstituteBuilder<IFileInfoWrapper>().WithRandomProperties().Build());
            }
            return files;
        }

        #endregion

    }
}
