using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.FaceSearch.Models
{
    /// <summary>
    /// Model used for when an image is scanned for all faces within the image. Each item in Faces 
    /// collection represents a face found in the image.
    /// </summary>
    public class FaceDetectionResult
    {
        public byte[] OriginalImageData { get; set; } = new byte[] { };

        public Size? ImageSize { get; set; }

        public byte[] DecoratedImageData { get; set; } = new byte[] { };

        public List<FaceDetectionResultItem> Faces { get; } = new List<FaceDetectionResultItem>();
    }
}
