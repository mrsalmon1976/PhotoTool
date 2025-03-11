using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.FaceSearch.Models
{
    /// <summary>
    /// Represents a face image saved to disk.
    /// </summary>
    public class FaceModel
    {
        public FaceModel()
        {
            Name = string.Empty;
            ImageData = string.Empty;
        }

        public string FilePath { get; set; } = String.Empty;

        public string Name { get; set; }

        public string ImageData { get; set; }

        public byte[] GetImageDataAsBytes()
        {
            return Convert.FromBase64String(ImageData);
        }
    }
}
