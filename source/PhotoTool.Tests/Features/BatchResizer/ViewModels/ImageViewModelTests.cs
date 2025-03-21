using Avalonia;
using Avalonia.Headless.NUnit;
using Avalonia.Media.Imaging;
using NUnit.Framework;
using PhotoTool.Features.BatchResizer.ViewModels;
using PhotoTool.Tests.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Tests.Features.BatchResizer.ViewModels
{
    [TestFixture]
    public class ImageViewModelTests
    {
        [Test]
        public void Dimensons_WhenImageNull_ReturnsEmptyString()
        {
            // Arrange
            ImageViewModel imageView = new ImageViewModel();

            // Act
            string result = imageView.Dimensions;   

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [AvaloniaTest]
        public void Dimensons_WhenImageNotNull_ReturnsValidString()
        {
            // Arrange
            int width = RandomData.Number.Next(800, 1024);
            int height = RandomData.Number.Next(600, 768);
            using Bitmap image = new Bitmap(TestAssets.PhotoToolLogoPath);
            using Bitmap testImage = image.CreateScaledBitmap(new PixelSize(width, height));

            ImageViewModel imageView = new ImageViewModel();
            imageView.Image = testImage;

            // Act
            Assert.That(imageView.Dimensions, Is.EqualTo($"{width} x {height}"));

        }

    }
}
