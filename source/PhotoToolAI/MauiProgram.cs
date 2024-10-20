using NLog;
using PhototoolAI.BootStrapping;

namespace PhotoToolAI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .InitialiseLogging();

            Logger logger = LogManager.GetCurrentClassLogger();

            // add dependencies
            builder.Services.AddDependencies();

            logger.Info("Application initialisation complete");

            return builder.Build();

        }
    }
}
