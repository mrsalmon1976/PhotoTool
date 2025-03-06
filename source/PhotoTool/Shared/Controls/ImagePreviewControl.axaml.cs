using Avalonia.Controls;
using PhotoTool.Shared.Logging;
using PhotoTool.Shared.ViewModels;
using System;

namespace PhotoTool.Shared.Controls;

public partial class ImagePreviewControl : UserControl
{
    private static IAppLogger _logger = AppLogger.Create<ImagePreviewControl>();

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
            else
            {
                _logger.Error("File launch processes are only supported on Windows.");
            }
        }
    }
}