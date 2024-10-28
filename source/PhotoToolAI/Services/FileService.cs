using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Services
{
	public interface IFileService
	{
		void CopyFile(string sourceFileName, string destFileName);

		IEnumerable<string> EnumerateFiles(string path, string searchPattern);

        IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);


        void EnsureDirectoryExists(string path);

        bool FileExists(string path);

		string GetRandomFileName(string extension);

        Task<string> ReadAllTextAsync(string filePath);

        Task WriteAllTextAsync(string filePath, string text);

	}

	public class FileService : IFileService
	{
		public void CopyFile(string sourceFileName, string destFileName)
		{
			File.Copy(sourceFileName, destFileName);
		}

		public void EnsureDirectoryExists(string path)
		{
			Directory.CreateDirectory(path);
		}

		public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
		{
			return EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

        }

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(path, searchPattern, searchOption);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }


        public string GetRandomFileName(string extension)
		{
            string fileName = Path.GetRandomFileName();
            return fileName.Replace(Path.GetExtension(fileName), ".json");
        }

        public async Task<string> ReadAllTextAsync(string filePath)
        {
            //return await File.ReadAllTextAsync(filePath);
            return await Task.Run(() => File.ReadAllText(filePath));
        }

        public async Task WriteAllTextAsync(string filePath, string text)
		{
            await File.WriteAllTextAsync(filePath, text);
        }

    }

}
