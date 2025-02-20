using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAvalonia.Utilities
{
    class AppUtils
    {
        public static Window GetMainWindow()
        {
            var appLifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            return appLifetime!.MainWindow!;
        }
    }
}
