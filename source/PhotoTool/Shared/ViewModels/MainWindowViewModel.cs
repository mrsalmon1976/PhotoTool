using PhotoTool.Features.BatchResizer.ViewModels;
using PhotoTool.Features.FaceSearch.ViewModels;
using PhotoTool.Shared.UI;
using System.Diagnostics;
using System.Reflection;

namespace PhotoTool.Shared.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(BatchResizerPanelViewModel batchResizerPanelViewModel, FaceSearchPanelViewModel faceSearchViewModel, IUIProvider uiProvider)
        {
            BatchResizerPanelViewModel = batchResizerPanelViewModel;
            FaceSearchPanelViewModel = faceSearchViewModel;

            this.Title = $"PhotoTool v{uiProvider.GetVersionNumber()}";
        }

        public BatchResizerPanelViewModel BatchResizerPanelViewModel { get; set; }

        public FaceSearchPanelViewModel FaceSearchPanelViewModel { get; set; }

        public string Title { get; set; } = "PhotoTool";


    }

}
