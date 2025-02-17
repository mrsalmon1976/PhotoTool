using PhotoToolAvalonia.Configuration;

namespace PhotoToolAvalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(FaceSearchPanelViewModel faceSearchViewModel)
        {
            FaceSearchDataContext = faceSearchViewModel;
        }

        public FaceSearchPanelViewModel FaceSearchDataContext { get; set; }

        public string Greeting { get; } = "Welcome to Avalonia!";
    }
}
