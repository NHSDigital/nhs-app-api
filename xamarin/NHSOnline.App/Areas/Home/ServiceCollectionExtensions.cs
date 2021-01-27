using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.Home.Presenters;
using NHSOnline.App.DependencyInjection;
using NhsAppWebPage = NHSOnline.App.Areas.Home.Views.NhsAppWebPage;

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