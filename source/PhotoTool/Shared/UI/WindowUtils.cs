using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using PhotoTool.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Shared.UI
{
    // TODO: Move everything into UIProvider
    class WindowUtils
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

        public static async Task ShowErrorDialog(string title, string message, Window? parentWindow = null)
        {
            await ShowSimpleDialog(title, message, Icon.Error, parentWindow);
        }

        public static async Task ShowErrorDialog(string title, ValidationException validationException, Window? parentWindow = null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string err in validationException.Errors)
            {
                sb.AppendLine().Append("\t * ").Append(err);
            }
            string message = $@"Please correct the following errors:{Environment.NewLine}{sb.ToString()}";
            await ShowSimpleDialog(title, message, Icon.Error, parentWindow);
        }

        public static async Task ShowInfoDialog(string title, string message, Window? parentWindow = null)
        {
            await ShowSimpleDialog(title, message, Icon.Info, parentWindow);
        }

        public static async Task ShowSimpleDialog(string title, string message, Icon icon, Window? parentWindow = null)
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
