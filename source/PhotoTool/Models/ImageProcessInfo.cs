using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoTool.Models
{
    public class ImageProcessInfo
    {
        public string OutputFolder { get; set; }

        public string[] Files { get; set; }

        public bool CreateThumbnails { get; set; }

        public bool ReplaceSpaces { get; set; }

        public int MaxLength { get; set; }

        public int Quality { get; set; }

        public int ThumbnailLength { get; set; }

        public int ErrorCount { get; set; }
    }
}
