using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.Home.Presenters;
using NHSOnline.App.Areas.Home.Views;
using NHSOnline.App.DependencyInjection;

namespace NHSOnline.App.Areas.Home
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddHomeArea(this IServiceCollection services)
        {
            return services
                .AddModelViewPresenter<NhsAppWebModel, NhsAppWebPage, NhsAppWebPresenter>();
        }
    }
}