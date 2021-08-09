using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Configuration;

namespace NHSOnline.Backend.Repository
{
    public static class StartupExtensions
    {
        public static void RegisterRepository<TRecord, TConfiguration>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TRecord : RepositoryRecord
            where TConfiguration: class, IRepositoryConfiguration, IValidatable

        {
            var isHealthCheckLoggingEnabled = configuration.GetBoolOrFallback(
                Constants.HealthCheckConstants.HealthCheckLoggingEnabledConfigKeyName, true);

            RegisterRepository<TRecord, TConfiguration>(services, isHealthCheckLoggingEnabled);
        }

        public static void RegisterRepository<TRecord, TConfiguration>(
            this IServiceCollection services,
            bool isHealthCheckLoggingEnabled)
            where TRecord : RepositoryRecord
            where TConfiguration: class, IRepositoryConfiguration, IValidatable

        {
            services.AddSingleton<TConfiguration>();
            services.AddTransient<IValidatable>(sp => sp.GetRequiredService<TConfiguration>());
            services.AddTransient<IRepository<TRecord>, MongoRepository<TConfiguration, TRecord>>();

            if (isHealthCheckLoggingEnabled)
            {
                services
                    .AddHealthChecks()
                    .AddCheck<ApiMongoClientHealthCheck<TConfiguration>>(typeof(TConfiguration).Name,
                        timeout: TimeSpan.FromSeconds(1));
            }
        }

        public static void RegisterDatabaseClient(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var connectionString = configuration.GetOrWarn("MONGO_CONNECTION_STRING", logger);
            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogInformation("No value for MONGO_CONNECTION_STRING, proceeding to register Azure Mongo services");

                services.AddSingleton<IAzureMongoClient, AzureMongoClient>();
                services.AddSingleton<IMongoClientCreator, MongoClientCreator>();
                services.AddSingleton<IMongoClientService, AzureMongoService>();
                services.AddHostedService<AzureMongoConnectionHealthBackgroundService>();
            }
            else
            {
                logger.LogInformation("Registering Local Mongo service");
                services.AddSingleton<IMongoClientCreator, MongoClientCreator>();
                services.AddSingleton<IMongoClientService, LocalMongoService>();
            }
        }
    }
}
