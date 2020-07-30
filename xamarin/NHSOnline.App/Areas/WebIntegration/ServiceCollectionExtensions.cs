using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Areas.WebIntegration.Presenters;
using NHSOnline.App.Areas.WebIntegration.Views;
using NHSOnline.App.DependencyInjection;

namespace NHSOnline.App.Areas.WebIntegration
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddWebIntegrationArea(this IServiceCollection services)
        {
            return services
                .AddModelViewPresenter<WebIntegrationModel, WebIntegrationPage, WebIntegrationPresenter>();
        }
    }
}