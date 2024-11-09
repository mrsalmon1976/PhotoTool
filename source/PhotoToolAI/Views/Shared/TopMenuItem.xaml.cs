using Microsoft.Maui.Handlers;
using PhotoToolAI.Constants;

namespace PhotoToolAI.Views.Shared;

public partial class TopMenuItem : ContentView
{

	private bool _isActive = false;

	public TopMenuItem()
	{
		InitializeComponent();

	}

	public event EventHandler? Clicked;

	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		set
		{
            _isActive = value;

            if (_isActive) 
			{
                button.Style = (Style)Application.Current!.Resources[AppStyles.TopMenuItemButton];
                line.Style = (Style)Application.Current.Resources[AppStyles.TopMenuItemBox];
                image.Source = this.ImageSource;
            }
            else
			{
                button.Style = (Style)Application.Current!.Resources[AppStyles.TopMenuItemButtonDisabled];
                line.Style = (Style)Application.Current.Resources[AppStyles.TopMenuItemBoxDisabled];
                image.Source = this.DisabledImageSource;
            }
        }
	}

    public string Text
    {
        get
        {
            return button.Text;
        }
        set
        {
            button.Text = value;
        }
    }

    public string ImageSource { get; set; }


    public string DisabledImageSource { get; set; }

    private void ButtonClicked(object sender, EventArgs e)
    {
		if (Clicked != null)
		{
            Clicked(sender, e);
		}

    }
}