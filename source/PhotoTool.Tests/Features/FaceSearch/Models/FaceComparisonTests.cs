using NUnit.Framework;
using PhotoTool.Features.FaceSearch.Constants;
using PhotoTool.Features.FaceSearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Tests.Features.FaceSearch.Models
{
    [TestFixture]
    public class FaceComparisonTests
    {
        [TestCase(0.61f)]
        [TestCase(0.53f)]
        [TestCase(0.42f)]
        public void FaceMatchProspect_WhenDotProductIndicatesProbable_ReturnsProbable(float dotProduct)
        {
            // Arrange
            var faceComparison = new FaceComparison { DotProduct = dotProduct };
            
            // Act
            var result = faceComparison.FaceMatchProspect;

            // Assert
            Assert.That(result, Is.EqualTo(FaceMatchProspect.Probable));
        }

        [TestCase(0.29f)]
        [TestCase(0.35f)]
        [TestCase(0.41f)]
        public void FaceMatchProspect_WhenDotProductIndicatesPossible_ReturnsPossible(float dotProduct)
        {
            // Arrange
            var faceComparison = new FaceComparison { DotProduct = dotProduct };

            // Act
            var result = faceComparison.FaceMatchProspect;

            // Assert
            Assert.That(result, Is.EqualTo(FaceMatchProspect.Possible));
        }

        [TestCase(0.28f)]
        [TestCase(0.17f)]
        [TestCase(0.09f)]
        public void FaceMatchProspect_WhenDotProductIndicatesUnlikely_ReturnsNone(float dotProduct)
        {
            // Arrange
            var faceComparison = new FaceComparison { DotProduct = dotProduct };

            // Act
            var result = faceComparison.FaceMatchProspect;

            // Assert
            Assert.That(result, Is.EqualTo(FaceMatchProspect.None));
        }

    }
}