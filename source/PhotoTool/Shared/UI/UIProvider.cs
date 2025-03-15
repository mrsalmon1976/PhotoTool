using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Shared.UI
{
    public interface IUIProvider
    {
        void InvokeOnUIThread(Action action);
    }
    public class UIProvider : IUIProvider
    {
        public void InvokeOnUIThread(Action action)
        {
            Dispatcher.UIThread.Invoke(action);
        }
    }
}
