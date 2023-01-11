using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Logging
{
    public class LogService
    {
        private ILogger _logger;

        public LogService() 
        { 
            _logger = LogManager.GetCurrentClassLogger();
        }
        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Error(Exception ex)
        {
            _logger.Error(ex, ex?.Message);
        }

        public void Error(Exception ex, string message)
        {
            _logger.Error(ex, message);
        }


    }
}
