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
        private bool _generateThumbnails = false;
        private string _maxImageLength = "1024";
        private string _maxThumbnailLength = "100";
        private bool _overwriteFiles = false;
        private bool _replaceSpacesWithUnderscores = false;

        public bool GenerateThumbnails
        {
            get => _generateThumbnails;
            set => this.RaiseAndSetIfChanged(ref _generateThumbnails, value);
        }

        public string MaxImageLength
        {
            get => _maxImageLength;
            set => this.RaiseAndSetIfChanged(ref _maxImageLength, value);
        }

        public string MaxThumbnailLength
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
