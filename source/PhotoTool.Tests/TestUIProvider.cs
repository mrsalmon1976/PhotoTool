using Avalonia.Controls;
using Avalonia.Platform.Storage;
using MsBox.Avalonia.Enums;
using PhotoTool.Shared.Exceptions;
using PhotoTool.Shared.UI;

namespace PhotoTool.Tests
{
    public class TestUIProvider : IUIProvider
    {
        public Window GetMainWindow()
        {
            throw new NotImplementedException();
        }

        public string GetVersionNumber()
        {
            return "1.0.0";
        }

        public Window? GetWindow<T>() where T : Window
        {
            throw new NotImplementedException();
        }

        public void InvokeOnUIThread(Action action)
        {
            action();
        }

        public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions filePickerOpenOptions)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions folderPickerOpenOptions)
        {
            throw new NotImplementedException();
        }

        public void PostOnUIThread(Action action)
        {
            action();
        }

        public Task ShowErrorDialog(string title, string message, Window? parentWindow = null)
        {
            throw new NotImplementedException();
        }

        public Task ShowErrorDialog(string title, ValidationException validationException, Window? parentWindow = null)
        {
            throw new NotImplementedException();
        }

        public Task ShowInfoDialog(string title, string message, Window? parentWindow = null)
        {
            throw new NotImplementedException();
        }

        public Task ShowSimpleDialog(string title, string message, Icon icon, Window? parentWindow = null)
        {
            throw new NotImplementedException();
        }

        public Task ShowWarningDialog(string title, string message, Window? parentWindow)
        {
            throw new NotImplementedException();
        }
    }
}
