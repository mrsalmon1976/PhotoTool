using PhotoTool.Features.BatchResizer.ViewModels;
using PhotoTool.Features.FaceSearch.ViewModels;

namespace PhotoTool.Shared.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(BatchResizerPanelViewModel batchResizerPanelViewModel, FaceSearchPanelViewModel faceSearchViewModel)
        {
            BatchResizerPanelViewModel = batchResizerPanelViewModel;
            FaceSearchPanelViewModel = faceSearchViewModel;
        }

        public BatchResizerPanelViewModel BatchResizerPanelViewModel { get; set; }

        public FaceSearchPanelViewModel FaceSearchPanelViewModel { get; set; }

    }
}
