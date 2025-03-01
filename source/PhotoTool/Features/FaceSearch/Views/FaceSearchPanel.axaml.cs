using Avalonia.Controls;
using PhotoTool.Features.FaceSearch.ViewModels;

namespace PhotoTool.Features.FaceSearch.Views;

public partial class FaceSearchPanel: UserControl
{
    private bool _isLoaded;

    public FaceSearchPanel()
    {
        InitializeComponent();
        this.Loaded += FaceSearchPanel_Loaded;

    }

    private async void FaceSearchPanel_Loaded(object? sender, System.EventArgs e)
    {
        if (!_isLoaded)
        {
            FaceSearchPanelViewModel? viewModel = this.DataContext as FaceSearchPanelViewModel;
            if (viewModel != null)
            {
                await viewModel.LoadFaces();
            }
            _isLoaded = true;
        }
    }

}