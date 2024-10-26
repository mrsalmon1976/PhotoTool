using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Resources
{
    public interface IResourceProvider
    {
        Color ErrorTextColor { get; }

        Color PrimaryDarkTextColor { get; }
    }

    public class ResourceProvider : IResourceProvider
    {
        public ResourceProvider()
        {
            ErrorTextColor = GetColor("ErrorText");
            PrimaryDarkTextColor = GetColor("PrimaryDarkText");
        }

        public Color ErrorTextColor { get; private set; }

        public Color PrimaryDarkTextColor { get; private set; }

        private Color GetColor(string resourceKey)
        {
            object? colorValue;
            Application.Current!.Resources.TryGetValue(resourceKey, out colorValue);

            if (colorValue != null && colorValue is Color)
            {
                return (Color)colorValue;
            }

            throw new ArgumentException($"Invalid resource key: {resourceKey}");
        }
    }
}
