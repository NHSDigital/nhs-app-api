using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Compute.Logging
{
    internal static class LoggingServiceCollectionExtensions
    {
        internal static void AddComputeLogging(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(IComputeLogger<>), typeof(ComputeLogger<>));
            serviceCollection.AddScoped<ComputeLoggerState>();
        }
    }
}