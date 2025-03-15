using Avalonia;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace PhotoTool.Features.BatchResizer.ViewModels
{
    public class ImageViewModel : ReactiveObject
    {

        private Bitmap? _image = null;

        public string FilePath { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string FileSize { get; set; } = string.Empty;

        public string Dimensions
        {
            get
            {
                if (this.Image == null) return string.Empty;

                Size sz = this.Image.Size;
                return $"{sz.Width} x {sz.Height}";
            }
        }


        public Bitmap? Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }


    }
}
