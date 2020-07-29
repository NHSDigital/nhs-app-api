using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.Home;
using NHSOnline.App.Areas.LoggedOut;
using NHSOnline.App.Areas.ThirdParty;

namespace NHSOnline.App.Areas
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddAreas(this IServiceCollection services)
        {
            return services
                .AddHomeArea()
                .AddLoggedOutArea()
                .AddSilverWebService();
        }
    }
}