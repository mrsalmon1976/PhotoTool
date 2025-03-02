using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace PhotoTool.Features.FaceSearch.ViewModels
{
    public class FaceSearchViewModel
    {
        public string Name { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public Bitmap? Image { get; set; }

        //public string MatchInfo { get; set; }

        //public Color MatchColor { get; set; } = Colors.Black;
    }
}
