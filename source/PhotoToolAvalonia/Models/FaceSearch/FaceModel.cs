using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAvalonia.Models.FaceSearch
{
    public class FaceModel
    {
        public string Name { get; set; } = String.Empty;

        public Bitmap? Image { get; set; } = null;

        public SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush(Color.FromRgb(255, 0, 0));
    }
}
