using PhotoTool.Logging;
using SAFish.PhotoTool;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoTool
{
    internal class BootStrapper
    {
        public static Container Boot()
        {
            var container = new Container();

            // set up logging
            ConfigureLogging();
            container.Register<LogService>();

            // configure any other services
            container.Register<ImageService>();

            // windows components
            RegisterWindowsForm<FormMain>(container);

            container.Verify();
            return container;
        }

        private static void RegisterWindowsForm<T>(Container container)
        {
            var type = typeof(T);
            var registration = Lifestyle.Transient.CreateRegistration(type, container);

            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Forms should be disposed by app code; not by the container.");

            container.AddRegistration(type, registration);
        }

        private static void ConfigureLogging()
        {
            var configuration = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = "phototool.log",
                ArchiveAboveSize = 10000000
            };

            configuration.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = configuration;
        }
    }
}
