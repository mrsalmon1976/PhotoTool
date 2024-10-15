using Emgu.CV.CvEnum;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.Face;

namespace TestApp
{
    internal class Stuff
    {
        internal static Mat GetFaceDescriptor(LBPHFaceRecognizer recognizer, Mat faceImage)
        {
            // Train the recognizer with the face image
            // Convert the image to grayscale if it is not already
            var grayFace = new Mat();
            CvInvoke.CvtColor(faceImage, grayFace, ColorConversion.Bgr2Gray);

            // In a real scenario, you would train with multiple images for a subject
            // For demonstration purposes, we can treat this single image as part of training
            // Using a unique label for this face (you might use a person ID or similar)
            recognizer.Train(new Mat[] { grayFace }, new int[] { 1 });

            // Return the trained recognizer which holds the descriptor internally
            return grayFace;
        }

        internal static bool IsMatch(LBPHFaceRecognizer recognizer, Mat referenceDescriptor, Mat faceDescriptor)
        {
            // Predict the label of the face descriptor
            var result = recognizer.Predict(faceDescriptor);
            int predictedLabel = result.Label; // Predicted label
            double confidence = result.Distance; // Confidence score

            // Check if the prediction is the same as the reference face
            // You might want to compare the predicted label with your reference ID
            return predictedLabel == 1 && confidence < 50D;
        }

        

    

    }
}
