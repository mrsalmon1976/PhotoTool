using Avalonia.Controls;
using PhotoToolAvalonia.Models.FaceSearch;
using PhotoToolAvalonia.Providers;
using PhotoToolAvalonia.Utilities;
using PhotoToolAvalonia.Views.FaceSearch;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace PhotoToolAvalonia.ViewModels
{
    public partial class FaceSearchPanelViewModel : ReactiveObject
    {

        private string _facesLabelText = string.Empty;

        public FaceSearchPanelViewModel()
        {
            AddFaceButtonClickCommand = ReactiveCommand.Create(OnAddFaceButtonClick);
        }

        #region Control Properties


        public string FacesLabelText
        {
            get => _facesLabelText;
            private set => this.RaiseAndSetIfChanged(ref _facesLabelText, value);
        }

        public ObservableCollection<FaceModel> Faces { get; set; } = new ObservableCollection<FaceModel>();

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> AddFaceButtonClickCommand { get; }

        private async void OnAddFaceButtonClick()
        {
            // todo: dependency injection...how?  viewmodel factory?
            var faceAddDialog = new FaceAddDialog();
            faceAddDialog.DataContext = new FaceAddDialogViewModel(new AssetProvider() );
            faceAddDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = await faceAddDialog.ShowDialog<FaceAddDialogViewModel?>(AppUtils.GetMainWindow());

            //string s = "";
            // Code for executing the command here.
            //Dispatcher.UIThread.Post(() => Debug.WriteLine("Add Face Button Clicked"));

        }

        #endregion



        public async Task LoadFaces()
        {
            Faces.Add(new FaceModel() { Name = "Face 1" });
            Faces.Add(new FaceModel() { Name = "Face 2" });
            Faces.Add(new FaceModel() { Name = "Face 3" });
            Faces.Add(new FaceModel() { Name = "Face 4" });

            FacesLabelText = "No saved faces found - add faces on the right to begin searching.";
        }
    }

    #region Design time mode

    public class FaceSearchPanelViewModelDesign : FaceSearchPanelViewModel
    {
    }

    #endregion
}
