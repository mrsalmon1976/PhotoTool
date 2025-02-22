using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAvalonia.Logging
{
    public class LoggingConfigurator
    {
        public static void ConfigureLogging()
        {
            var configuration = new NLog.Config.LoggingConfiguration();

            // we use startup date to create a new log file each day instead of the {shortdate} renderer - this 
            // ensures the header is always written if the file does not exist
            var dt = DateTime.Now.ToString("yyyy-MM-dd");

            string perfLogPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\perflog-{dt}.log";
            string appLogPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\logs\\applog-{dt}.log";

            WriteLogHeader(perfLogPath, $"LogDate|LogTime|LogSource|ProfileName|ExecutionTimeMilliseconds");
            WriteLogHeader(appLogPath, $"LogTime|Logger|LogLevel|Message|Exception");

            // set up performance logging
            var perfLog = new NLog.Targets.FileTarget("file-perflog")
            {
                FileName = perfLogPath,
                Layout = "${date:format=yyyy-MM-dd}|${date:format=HH\\:mm\\:ss.fff}|${event-properties:item=LogSource}|${event-properties:item=ProfileName}|${message}"
            };
            configuration.AddRule(LogLevel.Info, NLog.LogLevel.Fatal, perfLog, "Performance", true);

            // set up application logging
            var appLog = new NLog.Targets.FileTarget("file-applog")
            {
                FileName = appLogPath,
                Layout = "${longdate}|${logger}|${uppercase:${level}}|${message}|${exception:format=ToString}"
            };
            configuration.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, appLog, "*", false);

            LogManager.Setup().LoadConfiguration(configuration);
        }

        private static void WriteLogHeader(string logPath, string header)
        {
            if (!File.Exists(logPath))
            {
                File.WriteAllText(logPath, $"{header}{Environment.NewLine}");
            }
        }
    }
}
