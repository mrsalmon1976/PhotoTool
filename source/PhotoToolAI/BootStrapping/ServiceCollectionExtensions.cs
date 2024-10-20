
using PhotoToolAI;
using PhotoToolAI.Configuration;
using PhotoToolAI.Services;
using PhotoToolAI.Views.BatchResize;
using PhotoToolAI.Views.FaceSearch;

namespace PhototoolAI.BootStrapping
{
    public static class ServiceCollectionExtensions
    {
		public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAppSettings, AppSettings>();
            AddServices(services);
            AddViews(services);
        }

        private static void AddServices(this IServiceCollection services)
        {
			services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
			services.AddSingleton<IFileService, FileService>();
			services.AddSingleton<IImageService, ImageService>();
		}

		private static void AddViews(this IServiceCollection services)
        {
            services.AddSingleton<BatchResizeView>();
            services.AddSingleton<FaceSearchView>();

            services.AddSingleton<MainPage>();

        }


    }
}
