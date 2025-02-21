using Microsoft.Extensions.DependencyInjection;
using PhotoToolAvalonia.Configuration;
using PhotoToolAvalonia.Providers;
using PhotoToolAvalonia.ViewModels;

namespace PhotoToolAvalonia.BootStrapping
{
    public static class ServiceCollectionExtensions
    {
		public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAppSettings, AppSettings>();

            AddProviders(services);
            AddViewModels(services);
        }

        //      private static void AddRepositories(this IServiceCollection services)
        //      {
        //          services.AddTransient<IFaceRepository, FaceRepository>();
        //      }

        //      private static void AddServices(this IServiceCollection services)
        //      {
        //	services.AddTransient<IFaceDetectionService, FaceDetectionService>();
        //	services.AddTransient<IFileService, FileService>();
        //	services.AddTransient<IImageService, ImageService>();
        //}

        //private static void AddViews(this IServiceCollection services)
        //      {
        //          services.AddSingleton<BatchResizeView>();
        //          services.AddSingleton<FaceSearchView>();

        //          services.AddSingleton<MainPage>();

        //      }

        private static void AddProviders(this IServiceCollection services)
        {
            services.AddSingleton<IAssetProvider, AssetProvider>();
        }

        private static void AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<FaceSearchPanelViewModel>();
            services.AddTransient<MainWindowViewModel>();

        }


    }
}
