using PhotoTool.Features.BatchResizer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.BatchResizer.Models
{
    public class ImageResizeOptions
    {
        public bool GenerateThumbnails { get; set; }

        public uint MaxImageLength { get; set; }

        public uint MaxThumbnailLength { get; set; }

        public bool OverwriteFiles { get; set; }

        public bool ReplaceSpacesWithUnderscores { get; set; }

        public static ImageResizeOptions FromViewModel(ImageResizeOptionsViewModel viewModel)
        {
            ImageResizeOptions options = new ImageResizeOptions()
            {
                GenerateThumbnails = viewModel.GenerateThumbnails,
                OverwriteFiles = viewModel.OverwriteFiles,
                ReplaceSpacesWithUnderscores = viewModel.ReplaceSpacesWithUnderscores
            };

            uint.TryParse(viewModel.MaxImageLength, out uint maxImageLength);
            uint.TryParse(viewModel.MaxThumbnailLength, out uint maxThumbnailImageLength);

            options.MaxImageLength = maxImageLength;
            options.MaxThumbnailLength = maxThumbnailImageLength;

            return options;
        }

    }
}
