using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using PhotoTool.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Shared.UI
{
    public interface IUIProvider
    {
        Window GetMainWindow();

        string GetVersionNumber();

        Window? GetWindow<T>() where T : Window;

        void InvokeOnUIThread(Action action);

        Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions filePickerOpenOptions);

        Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions folderPickerOpenOptions);

        void PostOnUIThread(Action action);

        Task ShowErrorDialog(string title, string message, Window? parentWindow = null);

        Task ShowErrorDialog(string title, ValidationException validationException, Window? parentWindow = null);

        Task ShowInfoDialog(string title, string message, Window? parentWindow = null);

        Task ShowSimpleDialog(string title, string message, Icon icon, Window? parentWindow = null);

        Task ShowWarningDialog(string title, string message, Window? parentWindow);
    }

    public class UIProvider : IUIProvider
    {
        public Window GetMainWindow()
        {
            var appLifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (appLifetime == null)
            {
                throw new InvalidOperationException("Could not find application lifetime");
            }
            if (appLifetime.MainWindow == null)
            {
                throw new InvalidOperationException("Could not find main window");
            }
            return appLifetime.MainWindow;
        }

        public TopLevel GetTopLevel()
        {
            return GetTopLevel(GetMainWindow());
        }

        public TopLevel GetTopLevel(Window window)
        {

            var topLevel = TopLevel.GetTopLevel(window);
            if (topLevel == null)
            {
                throw new InvalidOperationException("Could not find top level window");
            }
            return topLevel;
        }

        public string GetVersionNumber()
        {
            string result = String.Empty;
            string? versionNumber = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            if (versionNumber != null)
            {
                result = string.Join(".", versionNumber.Split('.').Take(3));
            }
            return result;
        }

        public Window? GetWindow<T>() where T : Window
        {
            var appLifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            return appLifetime!.Windows.FirstOrDefault(w => w is T)!;
        }


        public void InvokeOnUIThread(Action action)
        {
            Dispatcher.UIThread.Invoke(action);
        }

        public async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions filePickerOpenOptions)
        {
            var topLevel = this.GetTopLevel();
            return await topLevel!.StorageProvider.OpenFilePickerAsync(filePickerOpenOptions);
        }

        public async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions folderPickerOpenOptions)
        {
            var topLevel = this.GetTopLevel();
            return await topLevel!.StorageProvider.OpenFolderPickerAsync(folderPickerOpenOptions);
        }

        public void PostOnUIThread(Action action)
        {
            Dispatcher.UIThread.Post(action);
        }

        public async Task ShowErrorDialog(string title, string message, Window? parentWindow = null)
        {
            await ShowSimpleDialog(title, message, Icon.Error, parentWindow);
        }

        public async Task ShowErrorDialog(string title, ValidationException validationException, Window? parentWindow = null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string err in validationException.Errors)
            {
                sb.AppendLine().Append("\t * ").Append(err);
            }
            string message = $@"Please correct the following errors:{Environment.NewLine}{sb.ToString()}";
            await ShowSimpleDialog(title, message, Icon.Error, parentWindow);
        }

        public async Task ShowInfoDialog(string title, string message, Window? parentWindow = null)
        {
            await ShowSimpleDialog(title, message, Icon.Info, parentWindow);
        }

        public async Task ShowSimpleDialog(string title, string message, Icon icon, Window? parentWindow = null)
        {
            if (parentWindow == null)
            {
                parentWindow = GetMainWindow();
            }
            var box = MessageBoxManager.GetMessageBoxStandard(title, message, ButtonEnum.Ok, icon, WindowStartupLocation.CenterScreen);
            await box.ShowWindowDialogAsync(parentWindow);
        }

        public async Task ShowWarningDialog(string title, string message, Window? parentWindow)
        {
            await ShowSimpleDialog(title, message, Icon.Warning, parentWindow);
        }


    }
}
