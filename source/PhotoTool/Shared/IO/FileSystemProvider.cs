using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PhotoTool.Shared.IO
{
	public interface IFileSystemProvider
	{
		void CopyFile(string sourceFileName, string destFileName);

        void DeleteFile(string path);

        bool DirectoryExists(string path);

		IEnumerable<string> EnumerateFiles(string path, string searchPattern);

        IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);


        void EnsureDirectoryExists(string path);

        bool FileExists(string path);

        string GetFileSizeReadable(string path);

        string GetFileSizeReadable(long i);


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

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            return EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

        }

        public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.EnumerateFiles(path, searchPattern, searchOption);
        }

        public void EnsureDirectoryExists(string path)
		{
			Directory.CreateDirectory(path);
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

        public string GetFileSizeReadable(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return GetFileSizeReadable(fileInfo.Length);
        }

        public string GetFileSizeReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.##") + suffix;
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
