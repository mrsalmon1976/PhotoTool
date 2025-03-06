using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using PhotoTool.Features.FaceSearch.ViewModels;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.ViewModels;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.Drawing;
using System.Linq;

namespace PhotoTool.Features.FaceSearch.Views;

public partial class FaceSearchPanel: UserControl
{
    private bool _isLoaded;
    private Border? _selectedBorder;

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

    private void OnSavedFacePointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {

        FaceSearchPanelViewModel? viewModel = this.DataContext as FaceSearchPanelViewModel;
        StackPanel? source = sender as StackPanel;
        if (viewModel != null && source != null)
        {
            if (viewModel.IsSearchActive) return;

            FaceAddViewModel? clickedFace = source.DataContext as FaceAddViewModel;
            if (clickedFace != null)
            {
                // exit if the current selection has already been clicked
                if (clickedFace.Image == clickedFace.ImageColor) return;

                clickedFace.Image = clickedFace.ImageColor;
            }

            // reset borders
            Border parentBorder = (Border)source.Parent!;
            if (_selectedBorder != null)
            {
                _selectedBorder.BorderThickness = new Avalonia.Thickness(1);
                // TODO: get this color from styles
                _selectedBorder.BorderBrush = ColorUtils.ConvertHexToColorBrush("#e0e0e0");
            }

            parentBorder.BorderThickness = new Avalonia.Thickness(2);
            // TODO: get this color from styles
            parentBorder.BorderBrush = ColorUtils.ConvertHexToColorBrush("#0078D4");
            _selectedBorder = parentBorder;

            // update the current selected item
            if (viewModel.SelectedFace != null)
            {
                viewModel.SelectedFace.Image = viewModel.SelectedFace.ImageGrayscale;
            }
            viewModel.SelectedFace = clickedFace;
        }
    }

    private void OnSearchResultPointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {

        FaceSearchPanelViewModel? viewModel = this.DataContext as FaceSearchPanelViewModel;
        StackPanel? source = sender as StackPanel;
        if (viewModel != null && source != null)
        {
            FaceSearchViewModel? clickedImageViewModel = (FaceSearchViewModel)source.DataContext!;
            viewModel.UpdatePreviewImage(clickedImageViewModel);
        }
    }
}