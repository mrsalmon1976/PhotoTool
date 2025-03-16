using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace PhotoTool.Shared.UI
{
    public interface IUIProvider
    {
        string GetVersionNumber();

        void InvokeOnUIThread(Action action);
        void PostOnUIThread(Action action);
    }
    public class UIProvider : IUIProvider
    {
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

        public void InvokeOnUIThread(Action action)
        {
            Dispatcher.UIThread.Invoke(action);
        }

        public void PostOnUIThread(Action action)
        {
            Dispatcher.UIThread.Post(action);
        }

    }
}
