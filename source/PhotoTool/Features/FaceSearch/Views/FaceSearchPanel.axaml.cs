using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using PhotoTool.Features.FaceSearch.ViewModels;
using PhotoTool.Shared.Graphics;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.Resources;
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

            SavedFaceViewModel? clickedFace = source.DataContext as SavedFaceViewModel;
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
                _selectedBorder.BorderBrush = StyleProvider.SelectionBorderColorDefault;
            }

            parentBorder.BorderThickness = new Avalonia.Thickness(2);
            parentBorder.BorderBrush = StyleProvider.SelectionBorderColorPrimary;
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
            SearchFaceViewModel? faceViewModel = (SearchFaceViewModel)source.DataContext!;
            viewModel.UpdatePreviewImage(faceViewModel);
        }
    }
}