using NUnit.Framework;
using PhotoTool.Features.BatchResizer.Models;
using PhotoTool.Features.BatchResizer.ViewModels;
using PhotoTool.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Tests.Features.BatchResizer.Models
{
    [TestFixture]
    public class ImageResizeOptionsTests
    {
        [Test]
        public void ConvertFromViewModel_WithValidViewModel_MapsProperties()
        {
            // setup
            ImageResizeOptionsViewModel viewModel = new SubstituteBuilder<ImageResizeOptionsViewModel>().WithRandomProperties().Build();

            // execute
            ImageResizeOptions options = ImageResizeOptions.ConvertFromViewModel(viewModel);

            // assert
            Assert.That(options.GenerateThumbnails, Is.EqualTo(options.GenerateThumbnails));
            Assert.That(options.OverwriteFiles, Is.EqualTo(options.OverwriteFiles));
            Assert.That(options.ReplaceSpacesWithUnderscores, Is.EqualTo(options.ReplaceSpacesWithUnderscores));
        }

        [TestCase("-1")]
        [TestCase("0")]
        [TestCase("test")]
        public void ConvertFromViewModel_WithInvalidMaxImageLength_SetsValueZero(string maxLength)
        {
            // setup
            ImageResizeOptionsViewModel viewModel = new SubstituteBuilder<ImageResizeOptionsViewModel>()
                .WithRandomProperties()
                .WithProperty(x => x.MaxImageLength, maxLength)
                .Build();

            // execute
            ImageResizeOptions options = ImageResizeOptions.ConvertFromViewModel(viewModel);

            // assert
            Assert.That(options.MaxImageLength, Is.EqualTo(0));
        }

        [Test]
        public void ConvertFromViewModel_WithValidMaxImageLength_SetsValueCorrectly()
        {
            // setup
            ImageResizeOptionsViewModel viewModel = new SubstituteBuilder<ImageResizeOptionsViewModel>()
                .WithRandomProperties()
                .WithProperty(x => x.MaxImageLength, "1024")
                .Build();

            // execute
            ImageResizeOptions options = ImageResizeOptions.ConvertFromViewModel(viewModel);

            // assert
            Assert.That(options.MaxImageLength, Is.EqualTo(1024));
        }

        [TestCase("-1")]
        [TestCase("0")]
        [TestCase("test")]
        public void ConvertFromViewModel_WithInvalidMaxThumbnailLength_SetsValueZero(string maxThumbnailLength)
        {
            // setup
            ImageResizeOptionsViewModel viewModel = new SubstituteBuilder<ImageResizeOptionsViewModel>()
                .WithRandomProperties()
                .WithProperty(x => x.MaxThumbnailLength, maxThumbnailLength)
                .Build();

            // execute
            ImageResizeOptions options = ImageResizeOptions.ConvertFromViewModel(viewModel);

            // assert
            Assert.That(options.MaxThumbnailLength, Is.EqualTo(0));
        }


        [Test]
        public void ConvertFromViewModel_WithValidMaxThumbnailLength_SetsValueCorrectly()
        {
            // setup
            ImageResizeOptionsViewModel viewModel = new SubstituteBuilder<ImageResizeOptionsViewModel>()
                .WithRandomProperties()
                .WithProperty(x => x.MaxThumbnailLength, "101")
                .Build();

            // execute
            ImageResizeOptions options = ImageResizeOptions.ConvertFromViewModel(viewModel);

            // assert
            Assert.That(options.MaxThumbnailLength, Is.EqualTo(101));
        }

    }
}
