using NUnit.Framework;
using PhotoTool.Features.BatchResizer.Models;
using PhotoTool.Features.BatchResizer.Validators;
using PhotoTool.Shared.Exceptions;
using PhotoTool.Test;
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
            // setup
            ImageResizeOptions options = new SubstituteBuilder<ImageResizeOptions>()
                .WithRandomProperties()
                .WithProperty(x => x.MaxImageLength, (uint)1024)
                .WithProperty(x => x.MaxThumbnailLength, (uint)100)
                .Build();

            // execute
            IImageResizeOptionsValidator validator = new ImageResizeOptionsValidator();
            validator.Validate(options);

            // assert
            Assert.Pass();
        }

        [Test]
        public void Validate_InvalidMaxImageLength_ExceptionThrown()
        {
            // setup
            ImageResizeOptions options = new SubstituteBuilder<ImageResizeOptions>()
                .WithRandomProperties()
                .WithProperty(x => x.MaxImageLength, (uint)0)
                .WithProperty(x => x.MaxThumbnailLength, (uint)100)
                .Build();

            // execute
            IImageResizeOptionsValidator validator = new ImageResizeOptionsValidator();
            var exception = Assert.Throws<ValidationException>(() => validator.Validate(options));

            // assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception?.Errors.Count, Is.EqualTo(1));
            Assert.That(exception?.Errors[0], Does.Contain("Image length"));
        }

        [Test]
        public void Validate_InvalidMaxThumbnailLength_ExceptionThrown()
        {
            // setup
            ImageResizeOptions options = new SubstituteBuilder<ImageResizeOptions>()
                .WithRandomProperties()
                .WithProperty(x => x.MaxImageLength, (uint)1024)
                .WithProperty(x => x.GenerateThumbnails, true)
                .WithProperty(x => x.MaxThumbnailLength, (uint)0)
                .Build();

            // execute
            IImageResizeOptionsValidator validator = new ImageResizeOptionsValidator();
            var exception = Assert.Throws<ValidationException>(() => validator.Validate(options));

            // assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception?.Errors.Count, Is.EqualTo(1));
            Assert.That(exception?.Errors[0], Does.Contain("Thumbnail length"));
        }

        [Test]
        public void Validate_InvalidMaxThumbnailLengthButGenerateThumbnailsFalse_NoExceptionThrown()
        {
            // setup
            ImageResizeOptions options = new SubstituteBuilder<ImageResizeOptions>()
                .WithRandomProperties()
                .WithProperty(x => x.MaxImageLength, (uint)1024)
                .WithProperty(x => x.GenerateThumbnails, false)
                .WithProperty(x => x.MaxThumbnailLength, (uint)0)
                .Build();

            // execute
            IImageResizeOptionsValidator validator = new ImageResizeOptionsValidator();
            validator.Validate(options);

            // assert
            Assert.Pass();
        }

    }
}
