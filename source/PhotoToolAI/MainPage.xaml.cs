using PhotoToolAI.Views.FaceSearch;
using PhotoToolAI.Views.BatchResize;
using PhotoToolAI.Views.Shared;

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

            ToggleActiveControls(_batchResizeView, batchResizerMenuItem);
        }

		private void MenuItemBatchResizerClicked(object sender, EventArgs e)
		{
            ToggleActiveControls(_batchResizeView, batchResizerMenuItem);
		}

		private void MenuItemFaceSearchClicked(object sender, EventArgs e)
		{
            ToggleActiveControls(_faceSearchView, faceSearchMenuItem);
		}

        private void ToggleActiveControls(ContentView activeView, TopMenuItem activeMenuItem)
        {
            batchResizerMenuItem.IsActive = false;
            faceSearchMenuItem.IsActive = false;
            activeMenuItem.IsActive = true;
            scrollView.Content = activeView;
        }

	}

}
