using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace PhotoTool.Shared.Logging
{
    public interface IPerformanceLogger
    {
        IPerformanceLogger Start();

        IPerformanceLogger Stop();
    }

    public class PerformanceLogger : IPerformanceLogger
    {
        private Logger _logger;
        private Stopwatch? _stopwatch = null;

        private const string LoggerName = "Performance";

        private PerformanceLogger(Logger logger, string logSource, string profileName)
        {
            _logger = logger;
            LogSource = logSource;
            ProfileName = profileName;
        }

        public string ProfileName { get; set; }

        public string LogSource { get; set; }

        public IPerformanceLogger Start()
        {
            _stopwatch = Stopwatch.StartNew();
            return this;
        }

        public IPerformanceLogger Stop()
        {
            if (_stopwatch == null)
            {
                throw new InvalidOperationException("PerformanceLogger has not been started");
            }
            _stopwatch.Stop();

            LogEventInfo logEvent = new LogEventInfo(LogLevel.Info, LoggerName, _stopwatch.ElapsedMilliseconds.ToString());
            logEvent.Properties["LogSource"] = LogSource;
            logEvent.Properties["ProfileName"] = ProfileName;
            _logger.Log(logEvent);

            _stopwatch = null;
            return this;
        }


        public static IPerformanceLogger Create<T>(string profileName)
        {
            var logger = LogManager.GetLogger(LoggerName);
            return new PerformanceLogger(logger, typeof(T).FullName!, profileName);
        }

        public static IPerformanceLogger CreateAndStart<T>(string profileName)
        {
            return Create<T>(profileName).Start();
        }

    }
}
