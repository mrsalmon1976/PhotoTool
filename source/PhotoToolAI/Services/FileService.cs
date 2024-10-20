using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Services
{
	internal interface IFileService
	{
		void CopyFile(string sourceFileName, string destFileName);

		void EnsureDirectoryExists(string path);

	}

	internal class FileService : IFileService
	{
		public void CopyFile(string sourceFileName, string destFileName)
		{
			File.Copy(sourceFileName, destFileName);
		}

		public void EnsureDirectoryExists(string path)
		{
			Directory.CreateDirectory(path);
		}


	}

}
