using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.PreHome.Models;
using NHSOnline.App.Areas.PreHome.Presenters;
using NHSOnline.App.Areas.PreHome.Views;
using NHSOnline.App.DependencyInjection;

namespace NHSOnline.App.Areas.PreHome
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddPreHomeArea(this IServiceCollection services)
        {
            return services
                .AddModelViewPresenter<NhsAppPreHomeScreenWebModel, NhsAppPreHomeScreenWebPage, NhsAppPreHomeScreenWebPresenter>();
        }

    }
}