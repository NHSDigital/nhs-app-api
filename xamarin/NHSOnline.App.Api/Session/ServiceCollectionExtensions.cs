using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.Api.Session
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSessionApiServices(this IServiceCollection services)
        {
            return services
                .AddTransient<ISessionService, SessionService>();
        }
    }
}
