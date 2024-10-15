using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using FaceAiSharp.Extensions;
using System.Drawing.Imaging;

namespace TestApp
{
    internal class Attempt5
    {
        public static void Run()
        {
            string searchFolderPath = @"C:\Temp\Test\Photos";
            var img1 = Image.Load<Rgb24>("C:\\Temp\\Test\\Photos\\0001.jpg");

            var det = FaceAiSharpBundleFactory.CreateFaceDetectorWithLandmarks();
            var rec = FaceAiSharpBundleFactory.CreateFaceEmbeddingsGenerator();

            var faces1 = det.DetectFaces(img1);

            rec.AlignFaceUsingLandmarks(img1, faces1.First().Landmarks!);
            var embedding1 = rec.GenerateEmbedding(img1);

            // now we have our face, let's scan the other images
            var images = Directory.GetFiles(searchFolderPath, "*.*").Where(f => new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(Path.GetExtension(f).ToLower()));

            foreach (string image in images)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Scanning image {image}...");

                var img2 = Image.Load<Rgb24>(image);
                var faces2 = det.DetectFaces(img2);

                foreach (var f in faces2)
                {
                    var img = img2.Clone();
                    rec.AlignFaceUsingLandmarks(img, f.Landmarks!);

                    var embedding2 = rec.GenerateEmbedding(img);

                    var dot = embedding1.Dot(embedding2);

                    if (dot >= 0.42)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\tSame person found.");


                        //img.Save("C:\\Temp\\Test\\Photos\\face_thumbnail.jpg");

                        //System.Drawing.Bitmap b = new System.Drawing.Bitmap("C:\\Temp\\Test\\Photos\\0002.1.jpg");
                        //using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(b))
                        //{
                        //    System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
                        //    System.Drawing.Rectangle faceRect = new System.Drawing.Rectangle((int)f.Box.X, (int)f.Box.Y, (int)(int)f.Box.Width, (int)f.Box.Height);
                        //    graphics.DrawRectangle(pen, faceRect);
                        //}
                        //b.Save("C:\\Temp\\Test\\Photos\\face_highlighted.jpg", ImageFormat.Jpeg);
                    }
                    else if (dot > 0.28 && dot < 0.42)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\tPossibly the same person found...but not likely.");
                    }
                    else if (dot <= 0.28)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\tDifferent person found.");
                    }

                }

            }





            //var first = faces.First();
            //var second = faces.Skip(1).First();

            //// AlignFaceUsingLandmarks is an in-place operation so we need to create a clone of img first
            //var secondImg = img.Clone();
            //rec.AlignFaceUsingLandmarks(img, first.Landmarks!);
            //rec.AlignFaceUsingLandmarks(secondImg, second.Landmarks!);

            //img.Save("aligned1.jpg");
            //secondImg.Save("aligned2.jpg");
            //Console.WriteLine($"Saved an aligned version of one of the faces found at \"./aligned.jpg\".");

            //var embedding1 = rec.GenerateEmbedding(img);
            //var embedding2 = rec.GenerateEmbedding(secondImg);

            //var dot = embedding1.Dot(embedding2);

            //Console.WriteLine($"Dot product: {dot}");

        }
    }
}
