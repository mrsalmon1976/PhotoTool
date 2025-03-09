using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.BatchResizer.ViewModels
{
    public class ImageResizeOptions
    {
        public bool GenerateThumbnails { get; set; } = false;

        public uint ImageQuality { get; set; } = 90;

        public uint MaxImageLength { get; set; } = 1024;

        public uint MaxThumbnailLength { get; set; } = 100;


        public bool OverwriteFiles { get; set; } = false;

        public bool ReplaceSpacesWithUnderscores { get; set; } = false;
    }
}
