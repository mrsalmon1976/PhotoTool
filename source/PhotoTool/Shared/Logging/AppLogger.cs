using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Shared.Logging
{
    public interface IAppLogger
    {
        void Error(string message);

        void Error(Exception exception);

        void Error(Exception exception, string message);

        void Info(string message);
    }
    public class AppLogger : IAppLogger
    {
        private Logger _logger;

        private AppLogger(Logger logger)
        {
            _logger = logger;
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(Exception exception)
        {
            _logger.Error(exception, exception.Message);
        }

        public void Error(Exception exception, string message)
        {
            _logger.Error(exception, message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public static IAppLogger Create<T>()
        {
            var logger = LogManager.GetLogger(typeof(T).Name);
            return new AppLogger(logger);
        }
    }
}
