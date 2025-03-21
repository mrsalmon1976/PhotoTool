using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PhotoTool.BootStrapping;
using CommunityToolkit.Mvvm.DependencyInjection;
using PhotoTool.Shared.Logging;
using PhotoTool.Shared.ViewModels;

namespace PhotoTool;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        DisableAvaloniaDataAnnotationValidation();

        // Register all the services needed for the application to run
        var collection = new ServiceCollection();
        collection.AddDependencies();

        var services = collection.BuildServiceProvider();
        Ioc.Default.ConfigureServices(services);

        var mainWindow = new MainWindow();
        mainWindow.DataContext = services.GetRequiredService<MainWindowViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();

        AppLogger.Create<App>().Info("Application started");
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}