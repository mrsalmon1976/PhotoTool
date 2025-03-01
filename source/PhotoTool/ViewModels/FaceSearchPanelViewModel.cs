using Avalonia.Controls;
using Avalonia.Media.Imaging;
using DynamicData;
using PhotoTool.Configuration;
using PhotoTool.Providers;
using PhotoTool.Repositories;
using PhotoTool.Services;
using PhotoTool.Utilities;
using PhotoTool.Views.FaceSearch;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace PhotoTool.ViewModels
{
    public partial class FaceSearchPanelViewModel : ReactiveObject
    {
        private string _facesLabelText = string.Empty;
        private readonly IViewModelProvider _viewModelProvider;
        private readonly IFaceRepository _faceRepo;
        private readonly IImageService _imageService;
        private bool _IsFaceListVisible;

        public FaceSearchPanelViewModel(IViewModelProvider viewModelProvider, IFaceRepository faceRepo, IImageService imageService)
        {
            this._viewModelProvider = viewModelProvider;
            this._faceRepo = faceRepo;
            this._imageService = imageService;
            AddFaceButtonClickCommand = ReactiveCommand.Create(OnAddFaceButtonClick);
        }

        #region Control Properties

        public bool IsFaceListVisible
        {
            get => _IsFaceListVisible;
            private set => this.RaiseAndSetIfChanged(ref _IsFaceListVisible, value);
        }
        

        public ObservableCollection<FaceAddViewModel> SavedFaces { get; set; } = new ObservableCollection<FaceAddViewModel>();

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> AddFaceButtonClickCommand { get; }

        private async void OnAddFaceButtonClick()
        {
            var faceAddDialog = new FaceAddDialog();
            faceAddDialog.DataContext = _viewModelProvider.GetViewModel<FaceAddDialogViewModel>();
            faceAddDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = await faceAddDialog.ShowDialog<FaceAddDialogViewModel?>(AppUtils.GetMainWindow());
            await LoadFaces();

            //string s = "";
            // Code for executing the command here.
            //Dispatcher.UIThread.Post(() => Debug.WriteLine("Add Face Button Clicked"));

        }

        #endregion



        public async Task LoadFaces()
        {
            SavedFaces.Clear();
            var faces = await _faceRepo.GetAllAsync();
            if (faces.Count() > 0)
            {
                foreach (var f in faces)
                {
                    var imageData = f.GetImageDataAsBytes();
                    var image = new Bitmap(new MemoryStream(imageData));
                    var grayscaleImage = new Bitmap(new MemoryStream(_imageService.ConvertToGrayscale(imageData)));

                    SavedFaces.Add(new FaceAddViewModel()
                    {
                        ImageGrayscale = grayscaleImage,
                        Image = image,
                        Name = f.Name
                    });
                }
            }
            this.IsFaceListVisible = faces.Any();
        }
    }

    #region Design time mode

    public class FaceSearchPanelViewModelDesign : FaceSearchPanelViewModel
    {
        public FaceSearchPanelViewModelDesign() : base(
            new ViewModelProvider()
            , new FaceRepository(new AppSettings(), new FileService())
            , new ImageService()
            )
        {
        }
    }

    #endregion
}
