using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace PhotoTool.Shared.Resources
{
    public class StyleProvider
    {
        static StyleProvider()
        {
            SelectionBorderColorDefault = GetSolidColorBrushResource("SelectionBorderColorDefault", Colors.Black);
            SelectionBorderColorPrimary = GetSolidColorBrushResource("SelectionBorderColorPrimary", Colors.Red);
        }

        public static SolidColorBrush SelectionBorderColorDefault { get; private set; }

        public static SolidColorBrush SelectionBorderColorPrimary { get; private set; }

        private static SolidColorBrush GetSolidColorBrushResource(string key, Color defaultColor)
        {
            var brush = Application.Current?.Resources[key] as ImmutableSolidColorBrush;
            var color = brush?.Color ?? Colors.Black;
            return new SolidColorBrush(color);
        }
    }
}
