using PhotoToolAvalonia.Configuration;
using PhotoToolAvalonia.Models.FaceSearch;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace PhotoToolAvalonia.ViewModels
{
    public partial class FaceSearchPanelViewModel : ViewModelBase
    {

        public FaceSearchPanelViewModel()
        {
            //RxApp.MainThreadScheduler.Schedule(LoadFaces);
        }

        public string AnotherGreeting { get; } = "Welcome to Face Search222";

        public ObservableCollection<FaceModel> Faces { get; set; } = new ObservableCollection<FaceModel>();

        public async Task LoadFaces()
        {
            Faces.Add(new FaceModel() { Name = "Face 1" });
            Faces.Add(new FaceModel() { Name = "Face 2" });
            Faces.Add(new FaceModel() { Name = "Face 3" });
            Faces.Add(new FaceModel() { Name = "Face 4" });
        }



    }
}
