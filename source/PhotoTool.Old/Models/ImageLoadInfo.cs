using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Models
{
    public class ImageLoadInfo
    {
        public ImageLoadInfo(string file, MagickImage img)
        {
            this.File = file;
            this.Image = img;
        }

        public MagickImage Image { get; set; }

        public string File { get; set; }
    }
}
