using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using FaceAiSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Attempt4
    {
        public static void Run()
        {
            using var hc = new HttpClient();
            var groupPhoto = hc.GetByteArrayAsync(
                "https://raw.githubusercontent.com/georg-jung/FaceAiSharp/master/examples/obama_family.jpg").Result;
            var img = Image.Load<Rgb24>(groupPhoto);

            var det = FaceAiSharpBundleFactory.CreateFaceDetectorWithLandmarks();
            var rec = FaceAiSharpBundleFactory.CreateFaceEmbeddingsGenerator();

            var faces = det.DetectFaces(img);

            foreach (var face in faces)
            {
                Console.WriteLine($"Found a face with conficence {face.Confidence}: {face.Box}");
            }

            var first = faces.First();
            var second = faces.Skip(1).First();

            // AlignFaceUsingLandmarks is an in-place operation so we need to create a clone of img first
            var secondImg = img.Clone();
            rec.AlignFaceUsingLandmarks(img, first.Landmarks!);
            rec.AlignFaceUsingLandmarks(secondImg, second.Landmarks!);

            img.Save("aligned1.jpg");
            secondImg.Save("aligned2.jpg");
            Console.WriteLine($"Saved an aligned version of one of the faces found at \"./aligned.jpg\".");

            var embedding1 = rec.GenerateEmbedding(img);
            var embedding2 = rec.GenerateEmbedding(secondImg);

            var dot = embedding1.Dot(embedding2);

            Console.WriteLine($"Dot product: {dot}");
            if (dot >= 0.42)
            {
                Console.WriteLine("Assessment: Both pictures show the same person.");
            }
            else if (dot > 0.28 && dot < 0.42)
            {
                Console.WriteLine("Assessment: Hard to tell if the pictures show the same person.");
            }
            else if (dot <= 0.28)
            {
                Console.WriteLine("Assessment: These are two different people.");
            }
        }
    }
}
