using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PhotoToolAvalonia.Configuration;
using PhotoToolAvalonia.ViewModels;
using ReactiveUI;
using System.Reactive;
using System.Windows.Input;

namespace PhotoToolAvalonia.Views.FaceSearch;

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