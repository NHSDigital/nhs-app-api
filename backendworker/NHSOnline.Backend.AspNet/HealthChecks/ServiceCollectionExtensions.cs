using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.AspNet.HealthChecks
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNhsAppHealthCheckService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHealthChecks();

            if (configuration.GetBoolOrFallback(Constants.HealthCheckConstants.HealthCheckLoggingEnabledConfigKeyName, true))
            {
                return services.AddTransient<IHealthCheckPublisher, SplunkHealthCheckPublisher>();
            }

            return services;
        }

        public static IServiceCollection AddNhsAppClientHealthCheck<TNhsAppHealthCheckClient>(
            this IServiceCollection services,
            string name,
            IEnumerable<string> tags,
            IConfiguration configuration)
            where TNhsAppHealthCheckClient : INhsAppHealthCheckClient
        {
            if (configuration.GetBoolOrFallback(Constants.HealthCheckConstants.HealthCheckLoggingEnabledConfigKeyName, true))
            {
                services
                    .AddHealthChecks()
                    .AddCheck<NhsAppServiceHealthCheck<TNhsAppHealthCheckClient>>(
                        name,
                        timeout: TimeSpan.FromSeconds(1),
                        tags: tags);
            }
            else
            {
                services.AddHealthChecks();
            }

            return services;
        }

        public static IServiceCollection AddNhsAppHealthCheck<TNhsAppCustomHealthCheck>(
            this IServiceCollection services,
            string name,
            IEnumerable<string> tags,
            IConfiguration configuration)
            where TNhsAppCustomHealthCheck : class, IHealthCheck
        {
            if (configuration.GetBoolOrFallback(Constants.HealthCheckConstants.HealthCheckLoggingEnabledConfigKeyName, true))
            {
                services
                    .AddHealthChecks()
                    .AddCheck<TNhsAppCustomHealthCheck>(
                        name,
                        timeout: TimeSpan.FromSeconds(1),
                        tags: tags);
            }
            else
            {
                services.AddHealthChecks();
            }

            return services;
        }

        public static IServiceCollection AddCustomNhsAppHealthCheck<TNhsAppCustomHealthCheck>(
            this IServiceCollection services,
            string name,
            IConfiguration configuration)
            where TNhsAppCustomHealthCheck : class, IHealthCheck
        {
            if (configuration.GetBoolOrFallback(Constants.HealthCheckConstants.HealthCheckLoggingEnabledConfigKeyName, true))
            {
                services
                    .AddHealthChecks()
                    .AddCheck<TNhsAppCustomHealthCheck>(name, timeout: TimeSpan.FromSeconds(1));
            }
            else
            {
                services.AddHealthChecks();
            }

            return services;
        }
    }
}
