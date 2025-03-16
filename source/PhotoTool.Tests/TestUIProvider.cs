using PhotoTool.Shared.UI;

namespace PhotoTool.Tests
{
    public class TestUIProvider : IUIProvider
    {
        public void InvokeOnUIThread(Action action)
        {
            action();
        }

        public void PostOnUIThread(Action action)
        {
            action();
        }
    }
}
