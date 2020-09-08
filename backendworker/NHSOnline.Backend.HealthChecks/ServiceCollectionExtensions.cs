using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NHSOnline.Backend.HealthChecks
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNhsAppHealthCheckService(
            this IServiceCollection services)
        {
            services.AddHealthChecks();
            return services.AddTransient<IHealthCheckPublisher, SplunkHealthCheckPublisher>();
        }

        public static IServiceCollection AddNhsAppHealthCheck<TNhsAppHealthCheckClient>(
            this IServiceCollection services,
            string name)
            where TNhsAppHealthCheckClient : INhsAppHealthCheckClient
        {
            services
                .AddHealthChecks()
                .AddCheck<NhsAppServiceHealthCheck<TNhsAppHealthCheckClient>>(name, timeout: TimeSpan.FromSeconds(1));

            return services;
        }
    }
}