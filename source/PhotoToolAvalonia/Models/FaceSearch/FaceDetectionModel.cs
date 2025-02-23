using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAvalonia.Models.FaceSearch
{
    public class FaceDetectionModel
    {
        public string Name { get; set; } = String.Empty;

        public Bitmap? Image { get; set; } = null;

        public SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush(Color.FromRgb(255, 0, 0));

        public byte[]? GetImageDataAsByteArray()
        {
            if (this.Image == null)
            {
                return null;
            }
            using var memoryStream = new MemoryStream();
            this.Image.Save(memoryStream); 
            return memoryStream.ToArray();
        }
    }
}
