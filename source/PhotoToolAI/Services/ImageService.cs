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

		SKEncodedImageFormat GetImageFormatFromExtension(string extension);

		SKEncodedImageFormat GetImageFormatFromPath(string path);

        bool IsImageExtension(string extension);


    }

    internal class ImageService : IImageService
	{
		private readonly IAppSettings _appSettings;
		private readonly IFileService _fileService;

		private Dictionary<string, SKEncodedImageFormat> _imageExtensions = new Dictionary<string, SKEncodedImageFormat>();



		public ImageService(IAppSettings appSettings, IFileService fileService)
		{
			_appSettings = appSettings;
			_fileService = fileService;

			_imageExtensions.Add(".bmp", SKEncodedImageFormat.Bmp);
			_imageExtensions.Add(".dng", SKEncodedImageFormat.Dng);
            _imageExtensions.Add(".gif", SKEncodedImageFormat.Gif);
            _imageExtensions.Add(".heif", SKEncodedImageFormat.Heif);
            _imageExtensions.Add(".jpeg", SKEncodedImageFormat.Jpeg);
            _imageExtensions.Add(".jpg", SKEncodedImageFormat.Jpeg);
            _imageExtensions.Add(".ktx", SKEncodedImageFormat.Ktx);
            _imageExtensions.Add(".ico", SKEncodedImageFormat.Ico);
            _imageExtensions.Add(".pkm", SKEncodedImageFormat.Pkm);
            _imageExtensions.Add(".png", SKEncodedImageFormat.Png);
            _imageExtensions.Add(".wbmp", SKEncodedImageFormat.Wbmp);
            _imageExtensions.Add(".webp", SKEncodedImageFormat.Webp);
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

        public SKEncodedImageFormat GetImageFormatFromExtension(string extension)
        {
            var ext = extension.ToLowerInvariant();
			if (!_imageExtensions.ContainsKey(ext))
			{
				throw new NotSupportedException($"Extension {extension} is not a supported image extension");
			}
			return _imageExtensions[ext];
        }

        public SKEncodedImageFormat GetImageFormatFromPath(string path)
		{
			string extension = Path.GetExtension(path);
			return GetImageFormatFromExtension(extension);
		}


        public bool IsImageExtension(string extension)
		{
            var ext = extension.ToLowerInvariant();
            return _imageExtensions.ContainsKey(ext);
		}


    }
}
