using PhotoToolAI.Configuration;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Services
{
	internal interface IImageService
	{
		Color ConvertColor(SKColor color);

		string CopyToWorkingDirectory(string source);
	}

	internal class ImageService : IImageService
	{
		private readonly IAppSettings _appSettings;
		private readonly IFileService _fileService;

		public ImageService(IAppSettings appSettings, IFileService fileService)
		{
			_appSettings = appSettings;
			_fileService = fileService;
		}

		public Color ConvertColor(SKColor color)
		{
			return new Microsoft.Maui.Graphics.Color(
				color.Red / 255.0F,
				color.Green / 255.0F,
				color.Blue / 255.0F,
				color.Alpha / 255.0F
			);
		}

		public string CopyToWorkingDirectory(string source)
		{
			string workDir = _appSettings.WorkingDirectory;
			_fileService.EnsureDirectoryExists(workDir);

			string fileName = Path.GetFileNameWithoutExtension(source);
			string extension = Path.GetExtension(source);
			string newFileName = $"{fileName}_{Guid.NewGuid()}{extension}";

			string newFilePath = Path.Combine(workDir, newFileName);
			_fileService.CopyFile(source, newFilePath);

			return newFilePath;
		}
	}
}
