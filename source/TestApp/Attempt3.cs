using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Face;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TestApp
{
    internal class Attempt3
    {
        public static void Run()
        {
            string searchFolderPath = @"C:\Temp\Test\Photos";
            string trainingFolderPath = @"C:\Temp\Test\Train";

            // Update these paths to where you saved the model files
            string faceDetectorConfigPath = @"C:\Temp\Test\Model\deploy.prototxt";
            string faceDetectorModelPath = @"C:\Temp\Test\Model\res10_300x300_ssd_iter_140000.caffemodel";

            if (!File.Exists(faceDetectorConfigPath) || !File.Exists(faceDetectorModelPath))
            {
                Console.WriteLine("One or more model files are missing. Please check the file paths.");
                return;
            }

            // Load face detection model
            using (var faceDetector = CvDnn.ReadNetFromCaffe(faceDetectorConfigPath, faceDetectorModelPath))
            {
                // Prepare training data
                var trainingImages = new List<Mat>();
                var trainingLabels = new List<int>();

                foreach (var imagePath in Directory.GetFiles(trainingFolderPath, "*.*")
                    .Where(f => new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(Path.GetExtension(f).ToLower())))
                {
                    using (var img = new Mat(imagePath))
                    {
                        var faces = DetectFaces(img, faceDetector);
                        if (faces.Count > 0)
                        {
                            var face = faces[0];
                            var faceImage = new Mat(img, face);
                            Cv2.CvtColor(faceImage, faceImage, ColorConversionCodes.BGR2GRAY);
                            Cv2.Resize(faceImage, faceImage, new OpenCvSharp.Size(100, 100));
                            trainingImages.Add(faceImage);
                            trainingLabels.Add(1);
                        }
                    }
                }

                if (trainingImages.Count == 0)
                {
                    Console.WriteLine("No faces detected in training images. Please check your training folder.");
                    return;
                }

                // Create and train the face recognizer
                using (var faceRecognizer = OpenCvSharp.Face.LBPHFaceRecognizer.Create())
                {
                    faceRecognizer.Train(trainingImages, trainingLabels);

                    // Process search images
                    foreach (var imagePath in Directory.GetFiles(searchFolderPath, "*.*")
                        .Where(f => new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(Path.GetExtension(f).ToLower())))
                    {
                        using (var img = new Mat(imagePath))
                        {
                            var faces = DetectFaces(img, faceDetector);
                            bool faceFound = false;

                            foreach (var face in faces)
                            {
                                using (var faceImage = new Mat(img, face))
                                {
                                    Cv2.CvtColor(faceImage, faceImage, ColorConversionCodes.BGR2GRAY);
                                    Cv2.Resize(faceImage, faceImage, new OpenCvSharp.Size(100, 100));

                                    var result = faceRecognizer.Predict(faceImage);

                                    var resultn = faceRecognizer.GetNeighbors();

                                    if (result < 70) // Adjust this threshold as needed
                                    {
                                        Console.WriteLine($"Face found in: {imagePath}");
                                        Cv2.Rectangle(img, face, Scalar.Red, 2);
                                        img.SaveImage(Path.Combine(Path.GetDirectoryName(imagePath), "found_" + Path.GetFileName(imagePath)));
                                        faceFound = true;
                                        break;
                                    }
                                }
                            }

                            if (!faceFound)
                            {
                                Console.WriteLine($"No matching face found in: {imagePath}");
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Processing complete.");
        }

        static List<Rect> DetectFaces(Mat image, Net faceDetector)
        {
            using (var inputBlob = CvDnn.BlobFromImage(image, 1.0, new OpenCvSharp.Size(300, 300), new Scalar(104, 177, 123), false, false))
            {
                faceDetector.SetInput(inputBlob, "data");
                using (var detection = faceDetector.Forward("detection_out"))
                {
                    var faces = new List<Rect>();
                    for (int i = 0; i < detection.Size(2); i++)
                    {
                        float confidence = detection.At<float>(0, 0, i, 2);
                        if (confidence > 0.5)
                        {
                            int x1 = (int)(detection.At<float>(0, 0, i, 3) * image.Width);
                            int y1 = (int)(detection.At<float>(0, 0, i, 4) * image.Height);
                            int x2 = (int)(detection.At<float>(0, 0, i, 5) * image.Width);
                            int y2 = (int)(detection.At<float>(0, 0, i, 6) * image.Height);
                            faces.Add(new Rect(x1, y1, x2 - x1, y2 - y1));
                        }
                    }
                    return faces;
                }
            }
        }
    }
}
