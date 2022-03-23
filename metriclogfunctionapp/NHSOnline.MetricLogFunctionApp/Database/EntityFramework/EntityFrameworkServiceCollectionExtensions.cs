using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Database.EntityFramework
{
    internal static class EntityFrameworkServiceCollectionExtensions
    {
        private static readonly List<string> _runEnvironments = new()
        {
            "dev"
        };

        internal static void AddEntityFramework(this IServiceCollection serviceCollection)
        {
            var sqlConnection = GetConnectionString();
            serviceCollection.AddDbContext<TransactionContext>(
                options => options.UseNpgsql(sqlConnection),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient);

            serviceCollection.AddTransient<IDbConnectionInterceptor, NotificationLoggingInterceptor>();
            serviceCollection.AddTransient<IDbConnectionInterceptor, AadAuthenticationInterceptor>();
        }

        private static string GetConnectionString()
        {
            var clusterName = Environment.GetEnvironmentVariable("ClusterName");
            var sqlConnection = Environment.GetEnvironmentVariable("AnalyticsSQLDbConnectionString");

            if (_runEnvironments.Contains(clusterName))
            {
                sqlConnection = sqlConnection.DecodeBase64();
            }

            return sqlConnection;
        }
    }
}
