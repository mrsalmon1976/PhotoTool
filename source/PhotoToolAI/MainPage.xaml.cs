using PhotoToolAI.Views.FaceSearch;
using PhotoToolAI.Views.BatchResize;

namespace PhotoToolAI
{
    public partial class MainPage : ContentPage
    {
        private readonly BatchResizeView batchResizeView = new BatchResizeView();
        private readonly FaceSearchView faceSearchView = new FaceSearchView();

        public MainPage()
        {
            InitializeComponent();
        }

		private void BtnBatchResizer_Clicked(object sender, EventArgs e)
		{
            scrollView.Content = batchResizeView;
		}

		private void BtnFaceSearch_Clicked(object sender, EventArgs e)
		{
			scrollView.Content = faceSearchView;
		}

	}

}
