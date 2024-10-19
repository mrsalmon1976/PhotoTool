using PhotoToolAI.Views.FaceSearch;
using PhotoToolAI.Views.BatchResize;

namespace PhotoToolAI
{
    public partial class MainPage : ContentPage
    {
        private readonly BatchResizeView _batchResizeView;
        private readonly FaceSearchView _faceSearchView;

        public MainPage(BatchResizeView batchResizeView, FaceSearchView faceSearchView)
        {
            this._batchResizeView = batchResizeView;
            this._faceSearchView = faceSearchView;

            InitializeComponent();
        }

		private void BtnBatchResizer_Clicked(object sender, EventArgs e)
		{
            scrollView.Content = _batchResizeView;
		}

		private void BtnFaceSearch_Clicked(object sender, EventArgs e)
		{
			scrollView.Content = _faceSearchView;
		}

	}

}
