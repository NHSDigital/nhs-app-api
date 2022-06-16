using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Areas.Errors.Presenters;
using NHSOnline.App.Areas.Errors.Views;
using NHSOnline.App.DependencyInjection;

namespace NHSOnline.App.Areas.Errors
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddErrorsArea(this IServiceCollection services)
        {
            return services
                .AddModelViewPresenter<CloseSlimBackToHomeNetworkErrorModel, CloseSlimBackToHomeNetworkErrorPage, CloseSlimBackToHomeNetworkErrorPresenter>()
                .AddModelViewPresenter<CloseSlimTryAgainNetworkErrorModel, CloseSlimTryAgainNetworkErrorPage, CloseSlimTryAgainNetworkErrorPresenter>()
                .AddModelViewPresenter<FullNavigationBackToHomeNetworkErrorModel, FullNavigationBackToHomeNetworkErrorPage, FullNavigationBackToHomeNetworkErrorPresenter>()
                .AddModelViewPresenter<PreHomeTryAgainNetworkErrorModel, PreHomeTryAgainNetworkErrorPage, PreHomeTryAgainNetworkErrorPresenter>()
                .AddModelViewPresenter<FullNavigationTryAgainFileDownloadErrorModel, FullNavigationTryAgainFileDownloadErrorPage, FullNavigationTryAgainFileDownloadErrorPresenter>()
                .AddModelViewPresenter<ServiceDownErrorModel, ServiceDownErrorPage, ServiceDownErrorPresenter>();
        }
    }
}