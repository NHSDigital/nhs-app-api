using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Etl.Logging
{
    internal static class LoggingServiceCollectionExtensions
    {
        internal static void AddEtlLogging(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(IEtlLogger<>), typeof(EtlLogger<>));
            serviceCollection.AddScoped<EtlLoggerState>();
        }
    }
}