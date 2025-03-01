﻿using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using PhotoTool.Providers;
using PhotoTool.Repositories;
using PhotoTool.Services;
using PhotoTool.Shared.Configuration;
using PhotoTool.ViewModels;
using ReactiveUI;

namespace PhotoTool.BootStrapping
{
    public static class ServiceCollectionExtensions
    {
		public static void AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IAppSettings, AppSettings>();

            AddProviders(services);
            AddRepositories(services);
            AddServices(services);
            AddViewModels(services);
        }

        private static void AddProviders(this IServiceCollection services)
        {
            services.AddSingleton<IAssetProvider, AssetProvider>();
            services.AddSingleton<IViewModelProvider, ViewModelProvider>();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IFaceRepository, FaceRepository>();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IImageService, ImageService>();
        }

        private static void AddViewModels(this IServiceCollection services)
        {
            services.AddTransient<FaceAddDialogViewModel>();
            services.AddTransient<FaceSearchPanelViewModel>();
            services.AddTransient<MainWindowViewModel>();

        }


    }
}
