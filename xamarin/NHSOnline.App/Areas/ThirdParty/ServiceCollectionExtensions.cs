using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.ThirdParty.Models;
using NHSOnline.App.Areas.ThirdParty.Presenters;
using NHSOnline.App.Areas.ThirdParty.Views;
using NHSOnline.App.DependencyInjection;

namespace NHSOnline.App.Areas.ThirdParty
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddSilverWebService(this IServiceCollection services)
        {
            return services
                .AddModelViewPresenter<NhsAppSilverWebModel, NhsAppSilverWebPage, NhsAppSilverWebPresenter>();
        }
    }
}