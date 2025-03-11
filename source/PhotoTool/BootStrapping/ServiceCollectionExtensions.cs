using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using PhotoTool.Features.BatchResizer.ViewModels;
using PhotoTool.Features.FaceSearch.Repositories;
using PhotoTool.Features.FaceSearch.Services;
using PhotoTool.Features.FaceSearch.ViewModels;
using PhotoTool.Shared.Configuration;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.Resources;
using PhotoTool.Shared.ViewModels;
using ReactiveUI;

namespace PhotoTool.BootStrapping
{
    public static class ServiceCollectionExtensions
    {
		public static void AddDependencies(this IServiceCollection services)
        {
            AddShared(services);
            AddBatchResizerFeature(services);
            AddFaceSearchFeature(services);
        }

        private static void AddShared(this IServiceCollection services)
        {
            // Configuration
            services.AddSingleton<IAppSettings, AppSettings>();

            // Graphics
            services.AddSingleton<IImageProcessor, ImageProcessor>();

            // IO
            services.AddSingleton<IFileSystemProvider, FileSystemProvider>();

            // Resources
            services.AddSingleton<IAssetProvider, AssetProvider>();

            // ViewModels
            services.AddTransient<MainWindowViewModel>();
            services.AddSingleton<IViewModelProvider, ViewModelProvider>();
        }

        private static void AddBatchResizerFeature(this IServiceCollection services)
        {
            // ViewModels
            services.AddTransient<ImageResizeOptionsViewModel>();
            services.AddTransient<ImageViewModel>();
            services.AddTransient<BatchResizerPanelViewModel>();
        }

        private static void AddFaceSearchFeature(this IServiceCollection services)
        {
            // Repositories
            services.AddSingleton<IFaceRepository, FaceRepository>();

            // Services
            services.AddSingleton<IFaceDetectionService, FaceDetectionService>();

            // ViewModels
            services.AddTransient<FaceAddDialogViewModel>();
            services.AddTransient<FaceSearchPanelViewModel>();
        }


    }
}
