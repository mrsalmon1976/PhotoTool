using Avalonia.Controls;
using Avalonia.Media.Imaging;
using PhotoTool.Features.FaceSearch.ViewModels;
using PhotoTool.Shared.IO;
using PhotoTool.Shared.ViewModels;
using System.Linq;

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

        // todo: should be moved into viewmodel
        FaceSearchPanelViewModel? viewModel = this.DataContext as FaceSearchPanelViewModel;
        StackPanel? source = sender as StackPanel;
        if (viewModel != null && source != null)
        {
            FaceSearchViewModel? clickedImage = (FaceSearchViewModel)source.DataContext!;
            IFileSystemProvider fileSystemProvider = new FileSystemProvider();
            viewModel.PreviewImageModel = new ImagePreviewViewModel()
            {
                Image = clickedImage.Image,
                Name = clickedImage.Name,
                Path = clickedImage.Path,
                Dimensions = string.Format("{0}x{1} / {2}", clickedImage?.Image?.Size.Width, clickedImage?.Image?.Size.Height, fileSystemProvider.GetFileSizeReadable(clickedImage!.Path))
            };
        }
    }
}