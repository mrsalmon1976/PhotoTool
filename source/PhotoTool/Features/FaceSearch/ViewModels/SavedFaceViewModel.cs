using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace PhotoTool.Features.FaceSearch.ViewModels
{
    /// <summary>
    /// Viewmodel used to display faces saved to disk.
    /// </summary>
    public class SavedFaceViewModel : ReactiveObject
    {

        private Bitmap? _image = null;

        public string FilePath { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;


        public Bitmap? Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        public Bitmap? ImageColor { get; set; } = null;

        public Bitmap? ImageGrayscale { get; set; } = null;


    }
}
