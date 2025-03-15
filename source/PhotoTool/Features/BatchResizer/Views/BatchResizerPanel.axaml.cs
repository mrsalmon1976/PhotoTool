using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DynamicData;
using PhotoTool.Features.BatchResizer.ViewModels;
using PhotoTool.Shared.ViewModels;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PhotoTool.Features.BatchResizer.Views;

public partial class BatchResizerPanel : UserControl
{
    private bool _isDeleting = false;

    public BatchResizerPanel()
    {
        InitializeComponent();
    }

    private void TextBoxKeyDownDigitsOnly(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        string s = e.KeySymbol ?? string.Empty;
        Regex regex = new Regex(@"^\d$");
        if (regex.IsMatch(s))
        {
            e.Handled = false;
        }
        else
        {
            e.Handled = true;
        }
    }

    private void DataGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        DataGrid dataGrid = (DataGrid)sender!;
        if (_isDeleting) return;

        UpdatePreviewImage(dataGrid);
    }

    private void DataGridKeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        DataGrid dataGrid = (DataGrid)sender!;
        if (e.Key == Avalonia.Input.Key.Delete && dataGrid.SelectedItems.Count > 0)
        {
            RemoveSelectedImages();
        }
    }

    private void RemoveSelectedImages()
    {
        _isDeleting = true;
        BatchResizerPanelViewModel? viewModel = this.DataContext as BatchResizerPanelViewModel;
        if (viewModel != null)
        {
            List<ImageViewModel> selectedItems = new List<ImageViewModel>();
            foreach (var item in ImageDataGrid.SelectedItems)
            {
                ImageViewModel? imageViewModel = item as ImageViewModel;
                if (imageViewModel != null)
                {
                    selectedItems.Add(imageViewModel);
                }
            }

            foreach (var item in selectedItems)
            {
                viewModel.SelectedImages.Remove(item);
                if (viewModel.PreviewImageModel != null && viewModel.PreviewImageModel.Path == item.FilePath)
                {
                    viewModel.PreviewImageModel = null;
                }
            }
        }
        _isDeleting = false;
        UpdatePreviewImage(ImageDataGrid);
    }

    private void UpdatePreviewImage(DataGrid dataGrid)
    {
        if (dataGrid.SelectedItems.Count == 0) return;

        BatchResizerPanelViewModel? viewModel = this.DataContext as BatchResizerPanelViewModel;
        ImageViewModel? imageViewModel = dataGrid.SelectedItems[0] as ImageViewModel;
        if (viewModel != null && imageViewModel != null)
        {
            if (dataGrid.SelectedItems.Count == 1)
            {
                viewModel.PreviewImageModel = new ImagePreviewViewModel()
                {
                    Dimensions = imageViewModel.Dimensions,
                    Image = imageViewModel.Image,
                    Name = imageViewModel.Name,
                    Path = imageViewModel.FilePath
                };
            }
            else
            {
                viewModel.PreviewImageModel = null;
            }
        }
    }

    private void RemoveFilesButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        RemoveSelectedImages();
    }
}