using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.Api.Configuration
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationService(this IServiceCollection services)
        {
            return services
                .AddTransient<IConfigurationService, ConfigurationService>();
        }
    }
}
