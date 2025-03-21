using Avalonia.Media;
using System;

namespace PhotoTool.Shared.Graphics
{
    class ColorUtils
    {
        public static SolidColorBrush ConvertHexToColorBrush(string hexColor)
        {
            byte r = Convert.ToByte(hexColor.Substring(1, 2), 16);
            byte g = Convert.ToByte(hexColor.Substring(3, 2), 16);
            byte b = Convert.ToByte(hexColor.Substring(5, 2), 16);

            return new SolidColorBrush(Color.FromRgb(r, g, b));

        }
    }
}
