using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.FaceSearch.Models
{
    public class FaceDetectionResult
    {
        public byte[] OriginalImageData { get; set; } = new byte[] { };

        public Size? ImageSize { get; set; }

        public byte[] DecoratedImageData { get; set; } = new byte[] { };

        public List<FaceDetectionResultItem> Faces { get; } = new List<FaceDetectionResultItem>();
    }
}
