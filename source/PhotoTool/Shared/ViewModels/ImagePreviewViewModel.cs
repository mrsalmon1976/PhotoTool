﻿using Avalonia.Media.Imaging;
using PhotoTool.Shared.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Shared.ViewModels
{
    public class ImagePreviewViewModel
    {
        public string Name { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public Bitmap? Image { get; set; }

        public string? Dimensions { get; set; }
    }
}
