using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.Services
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IUserPreferencesService, UserPreferencesService>();
        }
    }
}