using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoTool.Shared.IO
{
	public interface IFileSystemProvider
	{
		void CopyFile(string sourceFileName, string destFileName);

        void DeleteFile(string path);

        bool DirectoryExists(string path);

		IEnumerable<IFileInfoWrapper> EnumerateFiles(string path, string searchPattern);

        IEnumerable<IFileInfoWrapper> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);


        void EnsureDirectoryExists(string path);

        bool FileExists(string path);

        IDirectoryInfoWrapper? GetDirectoryInfo(IStorageItem storageItem);

        IFileInfoWrapper? GetFileInfo(string path);

        IFileInfoWrapper? GetFileInfo(IStorageItem storageItem);


        string GetFileSizeReadable(string path);

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

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public IEnumerable<IFileInfoWrapper> EnumerateFiles(string path, string searchPattern)
        {
            return EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

        }

        public IEnumerable<IFileInfoWrapper> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(path, searchPattern, searchOption);
            return files.Select(x => new FileInfoWrapper(x));
        }

        public void EnsureDirectoryExists(string path)
		{
			Directory.CreateDirectory(path);
		}

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public IDirectoryInfoWrapper? GetDirectoryInfo(IStorageItem storageItem)
        {
            string? path = storageItem.TryGetLocalPath();
            if (path != null && Directory.Exists(path))
            {
                return new DirectoryInfoWrapper(path);
            }
            return null;
        }


        public IFileInfoWrapper? GetFileInfo(string path)
        {
            if (File.Exists(path))
            {
                return new FileInfoWrapper(path);
            }
            return null;
        }

        public IFileInfoWrapper? GetFileInfo(IStorageItem storageItem)
        {
            string? path = storageItem.TryGetLocalPath();
            if (path != null && File.Exists(path))
            {
                return new FileInfoWrapper(path);
            }
            return null;
        }


        public string GetRandomFileName(string extension)
		{
            string fileName = Path.GetRandomFileName();
            return fileName.Replace(Path.GetExtension(fileName), extension);
        }

        public string GetFileSizeReadable(string path)
        {
            IFileInfoWrapper fileInfo = new FileInfoWrapper(path);
            return fileInfo.GetFileSizeReadable();
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
