using ImageMagick;
using NLog;
using PhotoTool.Logging;
using SAFish.PhotoTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoTool
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LogService logger = null;
            try
            {
                var container = BootStrapper.Boot();
                logger = container.GetInstance<LogService>();

                FormMain formMain = container.GetInstance<FormMain>();
                Application.Run(formMain);
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.Error(ex, ex.Message);
                }
                Environment.Exit(1);
            }
        }

    }
}
