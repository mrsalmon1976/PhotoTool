using Avalonia.Controls;
using ReactiveUI;
using System.Reactive;
using Avalonia.Platform.Storage;
using PhotoToolAvalonia.Utilities;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PhotoToolAvalonia.Providers;
using PhotoToolAvalonia.Constants;

namespace PhotoToolAvalonia.ViewModels
{
    public partial class FaceAddDialogViewModel : ReactiveObject
    {

        private readonly IAssetProvider _assetProvider;

        private bool _isImageSelected;
        private Bitmap? _selectedImage = null;

        public FaceAddDialogViewModel(IAssetProvider assetProvider)
        {
            SaveFacesButtonClickCommand = ReactiveCommand.Create(OnSaveFacesButtonClickCommand);
            SelectFileButtonClickCommand = ReactiveCommand.Create(OnSelectFileButtonClick);
            SelectedImage = assetProvider.GetImage(Assets.PhotoToolLogo_300x300_800bg);
            this._assetProvider = assetProvider;
        }

        #region Control Properties

        public bool IsImageSelected
        {
            get => _isImageSelected;
            private set => this.RaiseAndSetIfChanged(ref _isImageSelected, value);
        }

        public Bitmap? SelectedImage
        {
            get => _selectedImage;
            private set => this.RaiseAndSetIfChanged(ref _selectedImage, value);
        }



        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> SaveFacesButtonClickCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectFileButtonClickCommand { get; }

        private async void OnSaveFacesButtonClickCommand()
        {

        }

        private async void OnSelectFileButtonClick()
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            //var topLevel = TopLevel.GetTopLevel(this);
            var topLevel = TopLevel.GetTopLevel(AppUtils.GetMainWindow());

            // Start async operation to open the dialog.
            var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Image File",
                FileTypeFilter = new[] { FilePickerFileTypes.ImageAll },
                AllowMultiple = false
            });

            if (files.Count >= 1)
            {
                // Open reading stream from the first file.
                //await using var stream = await files[0].OpenReadAsync();
                //using var streamReader = new StreamReader(stream);
                // Reads all the content of file as a text.
                //var fileContent = await streamReader.ReadToEndAsync();
                //this.SelectedImagePath = files[0].Path.AbsolutePath;
                this.SelectedImage = new Bitmap(files[0].Path.LocalPath);
                this.IsImageSelected = true;
            }

        }

        #endregion




    }

    #region Design time mode

    public class FaceAddDialogViewModelDesign : FaceAddDialogViewModel
    {
        public FaceAddDialogViewModelDesign() : base(new AssetProvider())
        {

        }
    }

    #endregion
}
