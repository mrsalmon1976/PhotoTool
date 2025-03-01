using NLog;
using NLog.Targets;
using System;
using System.IO;

namespace PhotoTool.Logging
{
    public class LoggingConfigurator
    {
        public static void ConfigureLogging()
        {
            var configuration = new NLog.Config.LoggingConfiguration();

            // we use startup date to create a new log file each day instead of the {shortdate} renderer - this 
            // ensures the header is always written if the file does not exist
            var dt = DateTime.Now.ToString("yyyy-MM-dd");

            string perfLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"perflog-{dt}.log");
            string appLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"infolog-{dt}.log");
            string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"errorlog-{dt}.log");

            WriteLogHeader(perfLogPath, $"LogDate|LogTime|LogSource|ProfileName|ExecutionTimeMilliseconds");
            WriteLogHeader(appLogPath, $"LogTime|Logger|LogLevel|Message|Exception");
            WriteLogHeader(errorLogPath, $"LogTime|Logger|LogLevel|Message|Exception");

            // set up performance logging
            var perfLog = CreateFileTarget("file-perflog", perfLogPath, "${date:format=yyyy-MM-dd}|${date:format=HH\\:mm\\:ss.fff}|${event-properties:item=LogSource}|${event-properties:item=ProfileName}|${message}");
            configuration.AddRule(LogLevel.Info, NLog.LogLevel.Fatal, perfLog, "Performance", true);

            // set up application logging
            var appLog = CreateFileTarget("file-applog", appLogPath, "${longdate}|${logger}|${uppercase:${level}}|${replace-newlines:${message}}|${exception:format=Message:replaceNewLines=true}");
            configuration.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, appLog, "*", false);

            // error logs to separate file with stacktrace
            var errorLog = CreateFileTarget("file-errorlog", errorLogPath, "${longdate}|${logger}|${uppercase:${level}}|${message}|${exception:format=ToString}");
            configuration.AddRule(LogLevel.Error, NLog.LogLevel.Fatal, errorLog, "*", false);

            LogManager.Setup().LoadConfiguration(configuration);
        }

        private static FileTarget CreateFileTarget(string targetName, string logFilePath, string layout)
        {
            return new NLog.Targets.FileTarget(targetName)
            {
                FileName = logFilePath,
                Layout = layout
            };
        }

        private static void WriteLogHeader(string logPath, string header)
        {
            string dir = Path.GetDirectoryName(logPath)!;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(logPath))
            {
                File.WriteAllText(logPath, $"{header}{Environment.NewLine}");
            }
        }
    }
}
