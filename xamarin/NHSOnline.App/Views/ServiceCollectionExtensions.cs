using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Presenters;

namespace NHSOnline.App.Views
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddViews(this IServiceCollection services)
        {
            return services
                .AddModelViewPresenter<MainModel, MainPage, MainPresenter>();
        }
    }
}