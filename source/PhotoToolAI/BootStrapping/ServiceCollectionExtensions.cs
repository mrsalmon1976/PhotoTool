
using PhotoToolAI;
using PhotoToolAI.Views.BatchResize;
using PhotoToolAI.Views.FaceSearch;

namespace PhototoolAI.BootStrapping
{
    public static class ServiceCollectionExtensions
    {

        public static void AddViews(this IServiceCollection services)
        {
            services.AddSingleton<BatchResizeView>();
            services.AddSingleton<FaceSearchView>();

            services.AddSingleton<MainPage>();

        }


    }
}
