using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNhsAppHealthCheckService(
            this IServiceCollection services)
        {
            services.AddHealthChecks();

            return services.AddTransient<IHealthCheckPublisher, SplunkHealthCheckPublisher>();
        }

        public static IServiceCollection AddNhsAppClientHealthCheck<TNhsAppHealthCheckClient>(
            this IServiceCollection services,
            string name,
            IEnumerable<string> tags)
            where TNhsAppHealthCheckClient : INhsAppHealthCheckClient
        {
            services
                .AddHealthChecks()
                .AddCheck<NhsAppServiceHealthCheck<TNhsAppHealthCheckClient>>(
                    name,
                    timeout: TimeSpan.FromSeconds(1),
                    tags: tags);

            return services;
        }

        public static IServiceCollection AddNhsAppHealthCheck<TNhsAppCustomHealthCheck>(
            this IServiceCollection services,
            string name,
            IEnumerable<string> tags)
            where TNhsAppCustomHealthCheck : class, IHealthCheck
        {
            services
                .AddHealthChecks()
                .AddCheck<TNhsAppCustomHealthCheck>(
                    name,
                    timeout: TimeSpan.FromSeconds(1),
                    tags: tags);

            return services;
        }

        public static IServiceCollection AddCustomNhsAppHealthCheck<TNhsAppCustomHealthCheck>(
            this IServiceCollection services,
            string name)
            where TNhsAppCustomHealthCheck : class, IHealthCheck
        {
            services
                .AddHealthChecks()
                .AddCheck<TNhsAppCustomHealthCheck>(name, timeout: TimeSpan.FromSeconds(1));

            return services;
        }
    }
}
