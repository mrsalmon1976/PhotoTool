namespace PhotoToolAI.Views.FaceSearch;

public partial class NameEntry : ContentView
{
	public NameEntry()
	{
		InitializeComponent();
	}

	public Brush? BorderColor 
	{ 
		get
		{
			return border.Stroke;
		}
		set
		{
			border.Stroke = value;
		}
	}
}