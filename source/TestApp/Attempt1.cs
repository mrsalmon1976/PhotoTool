using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Attempt1
    {
        public static void Run()
        {
            LBPHFaceRecognizer recognizer = new LBPHFaceRecognizer();

            string folderPath = @"C:\Temp\Test\Photos";

            // Load the Haar Cascade for face detection
            var faceCascade = new CascadeClassifier("C:\\Temp\\Test\\Model\\haarcascade_frontalface_default.xml");

            // Load the reference image (your face)
            var referenceImage = CvInvoke.Imread(@"C:\Temp\Test\\me.png");
            var referenceGray = new Mat();
            CvInvoke.CvtColor(referenceImage, referenceGray, ColorConversion.Bgr2Gray);
            var referenceFaces = faceCascade.DetectMultiScale(referenceGray, 1.1, 10);

            if (referenceFaces.Length == 0)
            {
                Console.WriteLine("No face found in the reference image.");
                return;
            }

            // Assuming you want to recognize the first detected face
            var referenceFace = referenceFaces[0];
            var referenceFaceImage = new Mat(referenceImage, referenceFace);
            var referenceFaceDescriptor = Stuff.GetFaceDescriptor(recognizer, referenceFaceImage); // Create your own descriptor extraction logic

            // Get all photo paths from the folder
            var photoPaths = Directory.GetFiles(folderPath, "*.jpg");

            foreach (var photoPath in photoPaths)
            {
                LBPHFaceRecognizer recognizer2 = new LBPHFaceRecognizer();

                // Load the image
                var image = CvInvoke.Imread(photoPath);
                var grayImage = new Mat();
                CvInvoke.CvtColor(image, grayImage, ColorConversion.Bgr2Gray);
                var detectedFaces = faceCascade.DetectMultiScale(grayImage, 1.1, 2);

                foreach (var detectedFace in detectedFaces)
                {
                    var faceImage = new Mat(image, detectedFace);
                    var faceDescriptor = Stuff.GetFaceDescriptor(recognizer2, faceImage); // Create your own descriptor extraction logic

                    // Compare face descriptors
                    if (Stuff.IsMatch(recognizer, referenceFaceDescriptor, faceDescriptor)) // Implement your own matching logic
                    {
                        Console.WriteLine($"Found a match in {Path.GetFileName(photoPath)}");
                    }
                }
            }
        }
    }
}
