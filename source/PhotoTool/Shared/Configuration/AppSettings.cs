using System;
using System.IO;

namespace PhotoTool.Shared.Configuration
{
	public interface IAppSettings
	{
		string BaseDirectory { get; }

		string FaceDataDirectory { get; }

		string WorkingDirectory { get; }
	}

	internal class AppSettings : IAppSettings
	{
		public AppSettings() 
		{
			FaceDataDirectory = Path.Combine(BaseDirectory, "Data\\Faces");
			WorkingDirectory = Path.Combine(BaseDirectory, "Data\\Working");
		}

		public string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

		public string FaceDataDirectory { get; private set; }

		public string WorkingDirectory { get; private set; }
	}
}
