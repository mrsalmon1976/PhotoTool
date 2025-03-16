using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Tests.Features.BatchResizer.Validators
{
    [TestFixture]
    public class ImageResizeOptionsValidatorTests
    {
        [Test]
        public void Validate_ValidModel_NoExceptionThrown()
        {
            Assert.Fail();
        }

        [Test]
        public void Validate_InvalidMaxImageLength_ExceptionThrown()
        {
            Assert.Fail();
        }

        [Test]
        public void Validate_InvalidMaxThumbnailLength_ExceptionThrown()
        {
            Assert.Fail();
        }

        [Test]
        public void Validate_InvalidMaxThumbnailLengthButGenerateThumbnailsFalse_NoExceptionThrown()
        {
            Assert.Fail();
        }

    }
}
