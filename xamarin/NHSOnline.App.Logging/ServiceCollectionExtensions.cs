using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Logging
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoggingServices(
            this IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            return services
                .AddTransient<ILoggerProvider, CloudLoggerProvider>()
                .AddSingleton(loggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        }
    }
}