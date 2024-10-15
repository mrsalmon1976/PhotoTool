using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.Util;

namespace TestApp
{
    internal class Attempt2
    {
        public static void Run()
        {
            string folderPath = @"C:\Temp\Test\Photos";

            string trainingFolderPath = @"C:\Temp\Test\Train";

            // Path to the image containing the face to search for
            //string targetFacePath = @"C:\Temp\Test\\me.png";

            string modelPath = "C:\\Temp\\Test\\Model\\haarcascade_frontalface_default.xml";

            // Load the target face image
            // Initialize face detector
            using (CascadeClassifier faceDetector = new CascadeClassifier(modelPath))
            {
                // Prepare training data
                List<Mat> trainingImages = new List<Mat>();
                List<int> trainingLabels = new List<int>();

                foreach (string imagePath in Directory.GetFiles(trainingFolderPath, "*.*")
                         .Where(file => new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(Path.GetExtension(file).ToLower())))
                {
                    using (Mat image = CvInvoke.Imread(imagePath, ImreadModes.Color))
                    {
                        Rectangle[] faces = faceDetector.DetectMultiScale(
                            image,
                            1.1,
                            3,
                            new Size(30, 30),
                            Size.Empty
                        );

                        if (faces.Length > 0)
                        {
                            Rectangle face = faces[0];
                            using (Mat faceImage = new Mat(image, face))
                            {
                                Mat faceGray = new Mat();
                                CvInvoke.CvtColor(faceImage, faceGray, ColorConversion.Bgr2Gray);
                                CvInvoke.EqualizeHist(faceGray, faceGray);
                                CvInvoke.Resize(faceGray, faceGray, new Size(100, 100));
                                trainingImages.Add(faceGray.Clone());
                                trainingLabels.Add(1);
                            }
                        }
                    }
                }

                if (trainingImages.Count == 0)
                {
                    Console.WriteLine("No faces detected in training images. Please check your training folder.");
                    return;
                }

                // Initialize and train face recognizer
                LBPHFaceRecognizer recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 100);
                recognizer.Train(new VectorOfMat(trainingImages.ToArray()), new VectorOfInt(trainingLabels.ToArray()));

                // Process all images in the search folder
                foreach (string imagePath in Directory.GetFiles(folderPath, "*.*")
                         .Where(file => new[] { ".jpg", ".jpeg", ".png", ".bmp" }.Contains(Path.GetExtension(file).ToLower())))
                {
                    using (Mat image = CvInvoke.Imread(imagePath, ImreadModes.Color))
                    {
                        // Create a list to store detected faces
                        List<Rectangle> detectedFaces = new List<Rectangle>();

                        // Detect faces at multiple scales
                        for (double scale = 0.8; scale <= 1.5; scale += 0.1)
                        {
                            using (Mat scaledImage = new Mat())
                            {
                                CvInvoke.Resize(image, scaledImage, Size.Empty, scale, scale, Inter.Linear);

                                Rectangle[] faces = faceDetector.DetectMultiScale(
                                    scaledImage,
                                    1.1,
                                    5,
                                    new Size(30, 30),
                                    Size.Empty
                                );

                                foreach (var face in faces)
                                {
                                    detectedFaces.Add(new Rectangle(
                                        (int)(face.X / scale),
                                        (int)(face.Y / scale),
                                        (int)(face.Width / scale),
                                        (int)(face.Height / scale)
                                    ));
                                }
                            }
                        }

                        bool faceFound = false;

                        foreach (Rectangle face in detectedFaces)
                        {
                            using (Mat faceImage = new Mat(image, face))
                            {
                                Mat faceGray = new Mat();
                                CvInvoke.CvtColor(faceImage, faceGray, ColorConversion.Bgr2Gray);
                                CvInvoke.EqualizeHist(faceGray, faceGray);
                                CvInvoke.Resize(faceGray, faceGray, new Size(100, 100));

                                try
                                {
                                    FaceRecognizer.PredictionResult result = recognizer.Predict(faceGray);

                                    if (result.Distance < 80) // Adjusted threshold
                                    {
                                        Console.WriteLine($"Face found in: {imagePath}");
                                        CvInvoke.Rectangle(image, face, new MCvScalar(0, 255, 0), 2);
                                        CvInvoke.Imwrite(Path.Combine(Path.GetDirectoryName(imagePath), "found_" + Path.GetFileName(imagePath)), image);
                                        faceFound = true;
                                        //break; // Stop after finding the first match
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error processing face in {imagePath}: {ex.Message}");
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
    }
}
