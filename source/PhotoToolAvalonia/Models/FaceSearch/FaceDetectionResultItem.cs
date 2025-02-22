using SixLabors.ImageSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAvalonia.Models.FaceSearch
{
    public class FaceDetectionResultItem
	{
		public RectangleF Box { get; set; }

		public SKColor Color { get; set; }

		public byte[]? ImageData { get; set; }
	}
}
