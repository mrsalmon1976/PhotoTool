using PhotoTool.Features.FaceSearch.Models;
using PhotoTool.Shared.Configuration;
using PhotoTool.Shared.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PhotoTool.Features.FaceSearch.Repositories
{
    public interface IFaceRepository
    {
        Task<IEnumerable<FaceModel>> GetAllAsync();

        Task SaveAsync(FaceModel faceModel);
    }

    public class FaceRepository : IFaceRepository
    {
        private readonly IAppSettings _appSettings;
        private readonly IFileSystemProvider _fileSystemProvider;

        public FaceRepository(IAppSettings appSettings, IFileSystemProvider fileSystemProvider)
        {
            _appSettings = appSettings;
            _fileSystemProvider = fileSystemProvider;
        }

        public async Task<IEnumerable<FaceModel>> GetAllAsync()
        {
            List<FaceModel> faceModels = new List<FaceModel>();

            _fileSystemProvider.EnsureDirectoryExists(_appSettings.FaceDataDirectory);
            IEnumerable<IFileInfoWrapper> faceFiles = _fileSystemProvider.EnumerateFiles(_appSettings.FaceDataDirectory, "*.json");
            foreach (IFileInfoWrapper fileInfo in faceFiles)
            {
                string json = await _fileSystemProvider.ReadAllTextAsync(fileInfo.FullName);
                FaceModel faceModel = JsonSerializer.Deserialize<FaceModel>(json)!;
                faceModel.FilePath = fileInfo.FullName;
                if (faceModel != null)
                {
                    faceModels.Add(faceModel);
                }
            }

            return faceModels.OrderBy(x => x.Name);
        }

        public async Task SaveAsync(FaceModel faceModel)
        {
            string filePath = Path.Combine(_appSettings.FaceDataDirectory, _fileSystemProvider.GetRandomFileName(".json"));

            var json = JsonSerializer.Serialize(faceModel, new JsonSerializerOptions { WriteIndented = true  });

            _fileSystemProvider.EnsureDirectoryExists(_appSettings.FaceDataDirectory);

            await _fileSystemProvider.WriteAllTextAsync(filePath, json);
        }
    }
}
