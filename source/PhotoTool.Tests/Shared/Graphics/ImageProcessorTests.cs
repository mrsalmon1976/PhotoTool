using Avalonia.Headless.NUnit;
using Avalonia.Media.Imaging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using PhotoTool.Shared.Graphics;
using SixLabors.ImageSharp;
using SkiaSharp;
using System.ComponentModel.DataAnnotations;

namespace PhotoTool.Tests.Shared.Graphics
{
    [TestFixture]
    public class ImageProcessorTests
    {
        private string _tempDirectory = String.Empty;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "PhotoToolTests");
            Directory.CreateDirectory(_tempDirectory);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }

        [AvaloniaTest]
        public void ConvertToGrayscale_WhenImageBytesAreValid_ReturnsGrayscaleImage()
        {
            // Arrange
            byte[] colorImageBytes = File.ReadAllBytes(TestAssets.PhotoToolLogoPath);

            // Act
            IImageProcessor imageProcessor = new ImageProcessor();
            byte[] grayscaleImageBytes = imageProcessor.ConvertToGrayscale(colorImageBytes);
            Image grayscaleImage = Image.Load(grayscaleImageBytes);

            // Assert
            Assert.That(grayscaleImage.Height, Is.GreaterThan(100));
            Assert.That(grayscaleImage.Width, Is.GreaterThan(100));
        }

        [Test]
        public void GetImageFormatFromExtension_WhenSupportedExtension_ReturnsImageFormat()
        {
            // Arrange

            // Act
            IImageProcessor imageProcessor = new ImageProcessor();
            foreach (string key in imageProcessor.ImageExtensions.Keys)
            {
                SKEncodedImageFormat imageFormat = imageProcessor.GetImageFormatFromExtension(key.ToLower());
                SKEncodedImageFormat imageFormat2 = imageProcessor.GetImageFormatFromExtension(key.ToUpper());
            }

            // Assert
            Assert.Pass();
        }

        [TestCase(".txt")]
        [TestCase(".TXT")]
        [TestCase(".zzz")]
        public void GetImageFormatFromExtension_WhenUnsupportedExtension_ThrowsException(string extension)
        {
            // Arrange

            // Act
            IImageProcessor imageProcessor = new ImageProcessor();
            var exception = Assert.Throws<NotSupportedException>(() => imageProcessor.GetImageFormatFromExtension(extension));

            // Assert
            Assert.Pass();
        }

        [Test]
        public void GetImageFormatFromPath_WhenSupportedExtension_ReturnsImageFormat()
        {
            // Act
            IImageProcessor imageProcessor = new ImageProcessor();
            foreach (string key in imageProcessor.ImageExtensions.Keys)
            {
                SKEncodedImageFormat imageFormat = imageProcessor.GetImageFormatFromPath($"C:\\Temp\\myfile.{key.ToLower()}");
                SKEncodedImageFormat imageFormat2 = imageProcessor.GetImageFormatFromPath($"C:\\Temp\\myfile.{key.ToUpper()}");
            }
        }

        [Test]
        public void GetImageFormatFromPath_WhenUnsupportedExtension_ThrowsException()
        {
            // Act
            IImageProcessor imageProcessor = new ImageProcessor();
            var exception = Assert.Throws<NotSupportedException>(() => imageProcessor.GetImageFormatFromPath("C:\\Temp\\myfile.txt"));
        }

        [Test]
        public void IsImageExtension_WhenSupportedExtension_ReturnsTrue()
        {
            // Act
            IImageProcessor imageProcessor = new ImageProcessor();
            foreach (string key in imageProcessor.ImageExtensions.Keys)
            {
                Assert.That(imageProcessor.IsImageExtension(key.ToLower()), Is.True);
                Assert.That(imageProcessor.IsImageExtension(key.ToUpper()), Is.True);
            }
        }

        [Test]
        public void IsImageExtension_WhenUnsupportedExtension_ReturnsFalse()
        {
            // Act
            IImageProcessor imageProcessor = new ImageProcessor();
            Assert.That(imageProcessor.IsImageExtension(".txt"), Is.False);
        }

        [Test]
        public void ResizeImage_WhenImageResized_SavesResizedImageToDisk()
        {
            // Arrange
            const int length = 100;
            string imagePath = TestAssets.PhotoToolLogoPath;
            string outputPath = Path.Combine(_tempDirectory, $"{Path.GetRandomFileName()}.png");

            // Act
            IImageProcessor imageProcessor = new ImageProcessor();
            imageProcessor.ResizeImage(imagePath, length, outputPath);

            // Assert
            using Image image = Image.Load(outputPath);
            Assert.That(image.Width, Is.EqualTo(length));
            Assert.That(image.Height, Is.EqualTo(length));
        }

    }
}
