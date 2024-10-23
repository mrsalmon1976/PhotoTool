using Microsoft.Maui.Controls;
using PhotoToolAI.Models;

namespace PhotoToolAI.Views.FaceSearch;

public partial class NameEntry : ContentView
{
	public NameEntry()
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
    public ImageSource FaceImage
	{
		get
		{
			return imgFace.Source;
		}
		set 
		{ 
			imgFace.Source = value;
		}
        
    }
}