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
        [Test]
        public void FaceMatchProspect_WhenDotProductIndicatesProbable_ReturnsProbable()
        {
            Assert.Fail();
            //// Arrange
            //var faceComparison = new FaceComparison { DotProduct = 0.42f };
            //// Act
            //var result = faceComparison.FaceMatchProspect;
            //// Assert
            //Assert.AreEqual(FaceMatchProspect.Probable, result);
        }

        [Test]
        public void FaceMatchProspect_WhenDotProductIndicatesPossible_ReturnsPossible()
        {
            Assert.Fail();
        }

        [Test]
        public void FaceMatchProspect_WhenDotProductIndicatesUnlikely_ReturnsNone()
        {
            Assert.Fail();
        }

    }
}