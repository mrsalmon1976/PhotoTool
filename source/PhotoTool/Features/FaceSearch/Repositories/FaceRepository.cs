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
        private readonly IFileSystemProvider _fileService;

        public FaceRepository(IAppSettings appSettings, IFileSystemProvider fileService)
        {
            _appSettings = appSettings;
            _fileService = fileService;
        }

        public async Task<IEnumerable<FaceModel>> GetAllAsync()
        {
            List<FaceModel> faceModels = new List<FaceModel>();

            _fileService.EnsureDirectoryExists(_appSettings.FaceDataDirectory);
            IEnumerable<string> faceFiles = _fileService.EnumerateFiles(_appSettings.FaceDataDirectory, "*.json");
            foreach (string filePath in faceFiles)
            {
                string json = await _fileService.ReadAllTextAsync(filePath);
                FaceModel faceModel = JsonSerializer.Deserialize<FaceModel>(json)!;
                faceModel.FilePath = filePath;
                if (faceModel != null)
                {
                    faceModels.Add(faceModel);
                }
            }

            return faceModels.OrderBy(x => x.Name);
        }

        public async Task SaveAsync(FaceModel faceModel)
        {
            string filePath = Path.Combine(_appSettings.FaceDataDirectory, _fileService.GetRandomFileName(".json"));

            var json = JsonSerializer.Serialize(faceModel, new JsonSerializerOptions { WriteIndented = true  });

            _fileService.EnsureDirectoryExists(_appSettings.FaceDataDirectory);

            await _fileService.WriteAllTextAsync(filePath, json);
        }
    }
}
