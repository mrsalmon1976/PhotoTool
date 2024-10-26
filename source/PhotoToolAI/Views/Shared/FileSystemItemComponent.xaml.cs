namespace PhotoToolAI.Views.Shared;

public partial class FileSystemItemComponent : ContentView
{
	public FileSystemItemComponent()
	{
		InitializeComponent();
	}

	public event EventHandler? CancelClicked;

	public bool IsCancelButtonVisible
	{
		get
		{
			return CancelButton.IsVisible;
		}
		set
		{
			CancelButton.IsVisible = value;

        }
	}

	public string Path
	{
		get
		{
			return lblPath.Text;
		}
		set
		{
			lblPath.Text = value;
		}
	}

	public string InfoText
	{
		get
		{
			return lblInfo.Text;

        }
		set
		{
			lblInfo.Text = value;
		}
	}

    public Color InfoTextColor
    {
        get
        {
            return lblInfo.TextColor;

        }
        set
        {
            lblInfo.TextColor = value;
        }
    }

    private void CancelButtonClicked(object sender, EventArgs e)
    {
		if (CancelClicked != null)
		{
			CancelClicked(sender, e);
		}

    }
}