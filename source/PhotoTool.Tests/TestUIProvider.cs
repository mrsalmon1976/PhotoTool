using Avalonia.Threading;
using PhotoTool.Shared.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Tests
{
    public class TestUIProvider : IUIProvider
    {
        public void InvokeOnUIThread(Action action)
        {
            action();
        }

    }
}
