using NUnit.Framework;
using SAFish.PhotoTool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Test.Drawing
{
    [TestFixture]
    public class ImageServiceTests
    {
        private string _imagePath;
        private string _savedImagePath;

        [SetUp] 
        public void ImageServiceTests_SetUp() 
        {
            _imagePath = Path.Combine(System.AppContext.BaseDirectory, "TestImage.jpg");
            _savedImagePath = Path.Combine(System.AppContext.BaseDirectory, "TestImage_Saved.jpg");
        }

        [TearDown]
        public void ImageServiceTests_TearDown()
        {
            if (File.Exists(_imagePath)) 
            { 
                File.Delete(_imagePath); 
            }
            if (File.Exists(_savedImagePath))
            {
                File.Delete(_savedImagePath);
            }
        }

        [TestCase(10, 5)]
        [TestCase(6, 12)]
        [TestCase(1025, 768)]
        public void CreateBlankImage_RendersImage_CorrectSize(int width, int height)
        {
            ImageService imageService = new ImageService();
            using (Bitmap result = imageService.CreateBlankImage("Test image", width, height))
            {
                Assert.AreEqual(width, result.Width);
                Assert.AreEqual(height, result.Height);
            }
        }

        [Test]
        public void ResizeImage_DifferentSizeSpecifiedWideImage_DimensionsCorrectlyAdjusted()
        {
            // setup - create a blank image
            ImageService imageService = new ImageService();
            using (Bitmap img = imageService.CreateBlankImage("This is a test", 800, 600))
            {
                img.Save(_imagePath);
            }

            // resize it and check the parameters
            using (Image resizedImage = imageService.ResizeImage(_imagePath, 400, 50))
            {
                Assert.AreEqual(400, resizedImage.Width);
                Assert.AreEqual(300, resizedImage.Height);
            }
        }

        [Test]
        public void ResizeImage_DifferentSizeSpecifiedHighImage_DimensionsCorrectlyAdjusted()
        {
            // setup - create a blank image
            ImageService imageService = new ImageService();
            using (Bitmap img = imageService.CreateBlankImage("This is a test", 600, 800))
            {
                img.Save(_imagePath);
            }

            // resize it and check the parameters
            using (Image resizedImage = imageService.ResizeImage(_imagePath, 400, 50))
            {
                Assert.AreEqual(300, resizedImage.Width);
                Assert.AreEqual(400, resizedImage.Height);
            }
        }

        [Test]
        public void ResizeImage_ZeroLengthSpecified_OriginalDimensionsUsed()
        {
            // setup - create a blank image
            ImageService imageService = new ImageService();
            int width = new Random().Next(400, 500);
            int height = new Random().Next(201, 300);
            using (Bitmap img = imageService.CreateBlankImage("This is a test", width, height))
            {
                img.Save(_imagePath);
            }

            // resize it and check the parameters
            using (Image resizedImage = imageService.ResizeImage(_imagePath, 0, 50))
            {
                Assert.AreEqual(width, resizedImage.Width);
                Assert.AreEqual(height, resizedImage.Height);
            }
        }


        [Test]
        public void ResizeImage_WithOutputPath_ImageSavedToDisk()
        {
            // setup - create a blank image
            ImageService imageService = new ImageService();
            using (Bitmap img = imageService.CreateBlankImage("This is a test", 800, 600))
            {
                img.Save(_imagePath);
            }

            // resize it 
            imageService.ResizeImage(_imagePath, 800, 50, _savedImagePath);

            // check the file exists
            Assert.That(File.Exists(_savedImagePath));

            using (Image savedFile = Bitmap.FromFile(_savedImagePath))
            {
                Assert.AreEqual(800, savedFile.Width);
            }
        }

    }
}
