using PhotoTool.Shared.UI;

namespace PhotoTool.Tests
{
    public class TestUIProvider : IUIProvider
    {
        public string GetVersionNumber()
        {
            return "1.0.0";
        }

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
