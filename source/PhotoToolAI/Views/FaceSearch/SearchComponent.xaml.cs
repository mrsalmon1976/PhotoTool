using CommunityToolkit.Maui.Storage;
using PhotoToolAI.Models;
using PhotoToolAI.Views.Shared;
using System.Collections.ObjectModel;
using System.Threading;

namespace PhotoToolAI.Views.FaceSearch;

public partial class SearchComponent : ContentView
{
	public SearchComponent()
	{
		InitializeComponent();
	}

    public event EventHandler? BackButtonClick;

    public FaceModel? FaceModel { get; set; } = null;


    private async void SelectFolderButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var cancellationToken = new CancellationTokenSource().Token;

            var folderPicker = await FolderPicker.PickAsync(cancellationToken);

            if (folderPicker != null && folderPicker.Folder != null) 
            {
                SearchFolderControl item = new SearchFolderControl();
                sources.Children.Add(item);
                await item.SearchFolderForFace(this.FaceModel!, folderPicker.Folder.Path);
            }
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }

        ResetButtonStates();
    }

    private void ResetButtonStates()
    {
    }

    private void BtnBack_Clicked(object sender, EventArgs e)
    {
        if (BackButtonClick != null)
        {
            BackButtonClick(this, EventArgs.Empty);
        }

    }

}