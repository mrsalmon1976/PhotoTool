using PhotoToolAI.Configuration;
using PhotoToolAI.Models;
using PhotoToolAI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PhotoToolAI.Repositories
{
    public interface IFaceRepository
    {
        Task<IEnumerable<FaceModel>> GetAllAsync();

        Task SaveAsync(FaceModel faceModel);
    }

    public class FaceRepository : IFaceRepository
    {
        private readonly IAppSettings _appSettings;
        private readonly IFileService _fileService;

        public FaceRepository(IAppSettings appSettings, IFileService fileService)
        {
            this._appSettings = appSettings;
            this._fileService = fileService;
        }

        public async Task<IEnumerable<FaceModel>> GetAllAsync()
        {
            List<FaceModel> faceModels = new List<FaceModel>();
            IEnumerable<string> faceFiles = _fileService.EnumerateFiles(_appSettings.FaceDataDirectory, "*.json");
            foreach (string filePath in faceFiles)
            {
                string json = await _fileService.ReadAllTextAsync(filePath);
                FaceModel? faceModel = JsonSerializer.Deserialize<FaceModel>(json);
                if (faceModel != null)
                {
                    faceModels.Add(faceModel);
                }
            }
            return faceModels;
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
