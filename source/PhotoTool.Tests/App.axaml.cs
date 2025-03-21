using Avalonia;
using Avalonia.Markup.Xaml;

namespace PhotoTool.Tests;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}