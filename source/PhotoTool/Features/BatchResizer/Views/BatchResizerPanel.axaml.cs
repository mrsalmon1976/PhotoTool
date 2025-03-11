using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Text.RegularExpressions;

namespace PhotoTool.Features.BatchResizer.Views;

public partial class BatchResizerPanel : UserControl
{
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
}