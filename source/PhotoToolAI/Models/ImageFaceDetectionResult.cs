using FaceAiSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Models
{
	internal class ImageFaceDetectionResult
	{
		//public string? OriginalImagePath { get; set; }
		public byte[] OriginalImageData { get; set; }

		public Size? ImageSize { get; set; }

		//public string? DecoratedImagePath { get; set; }
		public byte[] DecoratedImageData { get; set;  }

		public List<FaceDetectionResult> Faces { get; } = new List<FaceDetectionResult>();


	}
}
