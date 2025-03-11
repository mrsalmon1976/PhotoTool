using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.BatchResizer.ViewModels
{
    public class ImageResizeOptionsViewModel : ReactiveObject
    {
        private uint _imageQuality = 90;
        private bool _generateThumbnails = false;
        private uint _maxImageLength = 1024;
        private uint _maxThumbnailLength = 100;
        private bool _overwriteFiles = false;
        private bool _replaceSpacesWithUnderscores = false;

        public bool GenerateThumbnails
        {
            get => _generateThumbnails;
            set => this.RaiseAndSetIfChanged(ref _generateThumbnails, value);
        }

        public uint ImageQuality
        {
            get => _imageQuality;
            set => this.RaiseAndSetIfChanged(ref _imageQuality, value);
        }

        public uint MaxImageLength
        {
            get => _maxImageLength;
            set => this.RaiseAndSetIfChanged(ref _maxImageLength, value);
        }

        public uint MaxThumbnailLength
        {
            get => _maxThumbnailLength;
            set => this.RaiseAndSetIfChanged(ref _maxThumbnailLength, value);
        }

        public bool OverwriteFiles
        {
            get => _overwriteFiles;
            set => this.RaiseAndSetIfChanged(ref _overwriteFiles, value);
        }

        public bool ReplaceSpacesWithUnderscores
        {
            get => _replaceSpacesWithUnderscores;
            set => this.RaiseAndSetIfChanged(ref _replaceSpacesWithUnderscores, value);
        }
    }
}
