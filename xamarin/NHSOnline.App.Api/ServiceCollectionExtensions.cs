using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Api.Client;
using NHSOnline.App.Api.Session;

namespace NHSOnline.App.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            return services
                .AddClientServices()
                .AddSessionApiServices();
        }
    }
}
