using Avalonia.Controls;
using PhotoTool.Shared.ViewModels;
using System;

namespace PhotoTool.Shared.Controls;

public partial class ImagePreviewControl : UserControl
{
    public ImagePreviewControl()
    {
        InitializeComponent();
    }

    private void OnWindowOpenClick(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        ImagePreviewViewModel? viewModel = this.DataContext as ImagePreviewViewModel;
        if (viewModel != null)
        {
            if (OperatingSystem.IsWindows())
            {
                System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", viewModel.Path));
            }
        }
    }
}