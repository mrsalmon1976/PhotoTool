using NUnit.Framework;
using PhotoTool.Features.FaceSearch.Models;
using PhotoTool.Features.FaceSearch.Repositories;
using PhotoTool.Shared.Configuration;
using PhotoTool.Shared.IO;
using PhotoTool.Test;
using PhotoTool.Tests.Random;
using NSubstitute;
using System.Text.Json;
using Bogus.Bson;

namespace PhotoTool.Tests.Features.FaceSearch.Repositories
{
    [TestFixture]
    public class FaceRepositoryTests
    {
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private IAppSettings _appSettings;
        private IFileSystemProvider _fileSystemProvider;
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [SetUp]
        public void SetUp()
        {
            _appSettings = new SubstituteBuilder<IAppSettings>().WithRandomProperties().Build();
            _fileSystemProvider = new SubstituteBuilder<IFileSystemProvider>().Build();
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsFaceModels()
        {
            // Arrange
            int count = RandomData.Number.Next(2, 8);
            var faceModels = TestDataUtils.CreateMany<FaceModel>(count).ToList();
            
            var faceFiles = TestDataUtils.CreateMany<IFileInfoWrapper>(count).ToList();
            _fileSystemProvider.EnumerateFiles(_appSettings.FaceDataDirectory, "*.json").Returns(faceFiles);

            for (int i = 0; i < count; i++)
            {
                IFileInfoWrapper fileInfo = faceFiles[i];
                _fileSystemProvider.ReadAllTextAsync(fileInfo.FullName).Returns(JsonSerializer.Serialize(faceModels[i]));
            }

            // Act
            IFaceRepository faceRepository = CreateFaceRepository();
            var result = await faceRepository.GetAllAsync();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(count));
            _fileSystemProvider.Received(1).EnsureDirectoryExists(_appSettings.FaceDataDirectory);
            _fileSystemProvider.Received(1).EnumerateFiles(_appSettings.FaceDataDirectory, "*.json");

        }

        [Test]
        public async Task GetAllAsync_WhenFacesLoaded_ReturnsFaceModelsOrderedByName()
        {
            // Arrange
            const int count = 3;
            var faceModels = new List<FaceModel>()
            {
                new SubstituteBuilder<FaceModel>().WithProperty(x => x.Name, "Z").Build(),
                new SubstituteBuilder<FaceModel>().WithProperty(x => x.Name, "F").Build(),
                new SubstituteBuilder<FaceModel>().WithProperty(x => x.Name, "A").Build(),
            };

            var faceFiles = TestDataUtils.CreateMany<IFileInfoWrapper>(count).ToList();
            _fileSystemProvider.EnumerateFiles(Arg.Any<string>(), Arg.Any<string>()).Returns(faceFiles);

            for (int i = 0; i < count; i++)
            {
                IFileInfoWrapper fileInfo = faceFiles[i];
                _fileSystemProvider.ReadAllTextAsync(fileInfo.FullName).Returns(JsonSerializer.Serialize(faceModels[i]));
            }

            // Act
            IFaceRepository faceRepository = CreateFaceRepository();
            var result = (await faceRepository.GetAllAsync()).ToList();

            // Assert
            Assert.That(result[0].Name, Is.EqualTo("A"));
            Assert.That(result[1].Name, Is.EqualTo("F"));
            Assert.That(result[2].Name, Is.EqualTo("Z"));
        }

        [Test]
        public async Task SaveAsync_WhenCalled_SavesFaceModel()
        {
            // Arrange
            const string fileName = "test.json";
            string filePath = Path.Combine(_appSettings.FaceDataDirectory, fileName);
            FaceModel faceModel = new SubstituteBuilder<FaceModel>().WithRandomProperties().Build();
            var json = JsonSerializer.Serialize(faceModel, new JsonSerializerOptions { WriteIndented = true });

            _fileSystemProvider.GetRandomFileName(".json").Returns(fileName);

            // Act
            IFaceRepository faceRepository = CreateFaceRepository();
            await faceRepository.SaveAsync(faceModel);

            // Assert
            _fileSystemProvider.Received(1).EnsureDirectoryExists(_appSettings.FaceDataDirectory);
            await _fileSystemProvider.Received(1).WriteAllTextAsync(filePath, json);
        }

        private IFaceRepository CreateFaceRepository()
        {
            return new FaceRepository(_appSettings, _fileSystemProvider);
        }

    }
}
