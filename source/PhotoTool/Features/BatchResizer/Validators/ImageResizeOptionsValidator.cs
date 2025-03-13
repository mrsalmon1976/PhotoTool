using PhotoTool.Features.BatchResizer.Models;
using PhotoTool.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.BatchResizer.Validators
{
    public interface IImageResizeOptionsValidator
    {
        void Validate(ImageResizeOptions options);
    }

    public class ImageResizeOptionsValidator : IImageResizeOptionsValidator
    {
        public void Validate(ImageResizeOptions options)
        {
            List<string> errors = new List<string>();

            if (options.MaxImageLength < 1)
            {
                errors.Add("Image length must be a valid number");
            }
            if (options.GenerateThumbnails && options.MaxThumbnailLength < 1)
            {
                errors.Add("Thumbnail length must be a valid number");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }
    }
}
