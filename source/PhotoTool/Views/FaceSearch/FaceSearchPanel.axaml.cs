using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhotoTool.Configuration;
using PhotoTool.ViewModels;
using ReactiveUI;
using System.Reactive;
using System.Windows.Input;

namespace PhotoTool.Views.FaceSearch;

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