using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Models
{
    public class FaceModel
    {
        public FaceModel()
        {
            this.Name = String.Empty;
            this.ImageData = string.Empty;
        }

        public string Name { get; set; }

        public string ImageData { get; set; }

        public byte[] GetImageDataAsBytes()
        {
            return Convert.FromBase64String(ImageData);
        }
    }
}
