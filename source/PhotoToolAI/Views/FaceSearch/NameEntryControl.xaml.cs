using Microsoft.Maui.Controls;
using PhotoToolAI.Models;

namespace PhotoToolAI.Views.FaceSearch;

public partial class NameEntryControl : ContentView
{
	private byte[]? _faceImageData;

	public NameEntryControl()
	{
		InitializeComponent();
	}

	public Color? BorderColor 
	{ 
		get
		{
			return frame.BorderColor;
        }
        set
		{
            frame.BorderColor = value;
		}
	}

    public byte[]? FaceImageData
	{
		get
		{
			return _faceImageData;
		}
		set
		{
			_faceImageData = value;
			ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(_faceImageData));
			imgFace.Source = imageSource;
		}            
    }

	public string FaceName
	{
		get
		{
			return entry.Text;
		}
		set
		{
			entry.Text = value;
		}
	}
}