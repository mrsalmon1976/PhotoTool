using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Threading;
using PhotoToolAvalonia.Configuration;
using PhotoToolAvalonia.Models.FaceSearch;
using PhotoToolAvalonia.Views.FaceSearch;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform.Storage;
using System.IO;
using PhotoToolAvalonia.Utilities;
using Avalonia.Media.Imaging;

namespace PhotoToolAvalonia.ViewModels
{
    public partial class FaceAddDialogViewModel : ReactiveObject
    {

        public FaceAddDialogViewModel()
        {
            SaveFacesButtonClickCommand = ReactiveCommand.Create(OnSaveFacesButtonClickCommand);
            SelectFileButtonClickCommand = ReactiveCommand.Create(OnSelectFileButtonClick);
        }

        #region Control Properties

        private Bitmap? _selectedImage = null;

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
            }

        }

        #endregion




    }
}
