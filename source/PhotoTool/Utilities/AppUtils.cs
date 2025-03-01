using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Utilities
{
    class AppUtils
    {
        public static Window? GetWindow<T>() where T : Window
        {
            var appLifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            return appLifetime!.Windows.FirstOrDefault(w => w is T)!;
        }

        public static Window GetMainWindow()
        {
            var appLifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            return appLifetime!.MainWindow!;
        }

        public static async Task ShowErrorDialog(string title, string message, Window? parentWindow)
        {
            await ShowSimpleDialog(title, message, Icon.Error, parentWindow);
        }

        public static async Task ShowInfoDialog(string title, string message, Window? parentWindow)
        {
            await ShowSimpleDialog(title, message, Icon.Info, parentWindow);
        }

        public static async Task ShowSimpleDialog(string title, string message, Icon icon, Window? parentWindow)
        {
            if (parentWindow == null)
            {
                parentWindow = GetMainWindow();
            }
            var box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok, icon, WindowStartupLocation.CenterScreen);
            await box.ShowWindowDialogAsync(parentWindow);
        }

        public static async Task ShowWarningDialog(string title, string message, Window? parentWindow)
        {
            await ShowSimpleDialog(title, message, Icon.Warning, parentWindow);
        }


    }
}
