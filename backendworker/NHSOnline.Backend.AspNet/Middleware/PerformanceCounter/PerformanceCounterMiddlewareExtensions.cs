using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.AspNet.Middleware.PerformanceCounter
{
    public static class PerformanceCounterMiddlewareExtensions
    {
        /// <summary>
        /// Add the Middleware to record sample metrics for use in the PerformanceCounters.
        /// NB: Make sure this is added first to the Middleware pipeline as it is time dependent.
        /// </summary>
        public static IApplicationBuilder UsePerformanceCounterMiddleware(this IApplicationBuilder builder, IConfiguration configuration, ILogger logger)
        {
            // The custom Health Check builds and logs the metrics
            var isMetricLoggingEnabled = configuration.GetBoolOrDefault(
                $"{PerformanceCounterConfiguration.ConfigSectionName}:{nameof(PerformanceCounterConfiguration.IsMetricLoggingEnabled)}",
                logger);

            if (isMetricLoggingEnabled)
            {
                return builder.UseMiddleware<PerformanceCounterMiddleware>();
            }

            return builder;
        }
    }
}