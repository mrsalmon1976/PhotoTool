using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;
using PhotoToolAvalonia.Configuration;
using PhotoToolAvalonia.Providers;
using PhotoToolAvalonia.ViewModels;
using ReactiveUI;

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
