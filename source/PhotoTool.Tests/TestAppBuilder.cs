using Avalonia;
using Avalonia.Headless;
using PhotoTool.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace PhotoTool.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
