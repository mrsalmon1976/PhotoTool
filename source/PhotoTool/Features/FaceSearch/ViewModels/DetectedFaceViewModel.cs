using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace PhotoTool.Features.FaceSearch.ViewModels
{
    /// <summary>
    /// Viewmodel used to display faces detected in an image.
    /// </summary>
    public class DetectedFaceViewModel : ReactiveObject
    {

        private Bitmap? _image = null;

        public string Name { get; set; } = string.Empty;


        public Bitmap? Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        public SolidColorBrush ColorBrush { get; set; } = new SolidColorBrush(Color.FromRgb(255, 0, 0));


    }
}
