using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace PhototoolAI.BootStrapping
{
    public static class MauiApplicationExtensions
    {
        public static MauiAppBuilder InitialiseLogging(this MauiAppBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddNLog();

            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "phototool.log");

            NLog.LogManager.Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().FilterMinLevel(NLog.LogLevel.Info).WriteToConsole();
                builder.ForLogger().FilterMinLevel(NLog.LogLevel.Info).WriteToFile(
                    fileName: logFilePath,
                    encoding: System.Text.Encoding.UTF8,
                    archiveAboveSize: 100 * 1024,
                    maxArchiveFiles: 10);
            });

            return builder;
        }


    }
}
