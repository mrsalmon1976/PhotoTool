﻿
using PhotoToolAI;
using PhotoToolAI.Configuration;
using PhotoToolAI.Repositories;
using PhotoToolAI.Resources;
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
            services.AddSingleton<IResourceProvider, ResourceProvider>();

            AddRepositories(services);
            AddServices(services);
            AddViews(services);
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IFaceRepository, FaceRepository>();
        }

        private static void AddServices(this IServiceCollection services)
        {
			services.AddTransient<IFaceDetectionService, FaceDetectionService>();
			services.AddTransient<IFileService, FileService>();
			services.AddTransient<IImageService, ImageService>();
		}

		private static void AddViews(this IServiceCollection services)
        {
            services.AddSingleton<BatchResizeView>();
            services.AddSingleton<FaceSearchView>();

            services.AddSingleton<MainPage>();

        }


    }
}