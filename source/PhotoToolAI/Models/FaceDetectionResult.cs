using SixLabors.ImageSharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Models
{
	internal class FaceDetectionResult
	{
		public FaceDetectionResult() 
		{
			this.Box = new SixLabors.ImageSharp.RectangleF();
			this.Color = SKColors.LightGreen;
		}

		public SixLabors.ImageSharp.RectangleF Box { get; set; }

		public SKColor Color { get; set; }

		public string? ImagePath { get; set; }
	}
}
