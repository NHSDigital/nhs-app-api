using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.AspNet.HealthChecks.PerformanceCounter
{
    public static class PerformanceCounterServiceExtensions
    {
        public static void AddPerformanceCounterService(
            this IServiceCollection services)
        {
            services.AddSingleton<PerformanceCounterConfiguration>();
            services.AddSingleton<IStatisticsStoreService, StatisticsStoreService>();
            services.AddSingleton<IPerformanceCounterStoreService, PerformanceCounterStoreService>();
            services.AddSingleton<IPerformanceCounterRegistry, PerformanceCounterRegistry>();

            services.AddTransient<IDateTimeHelperService, DateTimeHelperService>();

            services.AddNhsAppHealthCheck<PerformanceCounterHealthCheck>(
                "PERFORMANCE_COUNTERS", NhsAppHealthCheckTags.Readiness);
        }
    }
}