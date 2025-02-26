using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAvalonia.ViewModels
{
    public class FaceAddViewModel
    {

        public string Name { get; set; } = string.Empty;


        public Bitmap? Image { get; set; } = null;

        public Bitmap? ImageGrayscale { get; set; } = null;

        public SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        public byte[]? GetImageDataAsByteArray()
        {
            if (Image == null)
            {
                return null;
            }
            using var memoryStream = new MemoryStream();
            Image.Save(memoryStream); 
            return memoryStream.ToArray();
        }

    }
}
