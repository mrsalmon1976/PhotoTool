using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PhotoTool.Shared.IO
{
	public interface IFileSystemProvider
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

	public class FileSystemProvider : IFileSystemProvider
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
            return fileName.Replace(Path.GetExtension(fileName), extension);
        }

        public async Task<string> ReadAllTextAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }

        public async Task WriteAllTextAsync(string filePath, string text)
		{
            await File.WriteAllTextAsync(filePath, text);
        }

    }

}
