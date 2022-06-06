using System;
using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Repository.SqlApi;
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

        public static void RegisterSqlApiRepository<TRecord, TConfiguration>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TRecord : RepositoryRecord
            where TConfiguration: class, ISqlApiRepositoryConfiguration, IValidatable

        {
            var isHealthCheckLoggingEnabled = configuration.GetBoolOrFallback(
                Constants.HealthCheckConstants.HealthCheckLoggingEnabledConfigKeyName, true);

            RegisterSqlApiRepository<TRecord, TConfiguration>(services, isHealthCheckLoggingEnabled);
        }

        public static void RegisterSqlApiRepository<TRecord, TConfiguration>(
            this IServiceCollection services,
            bool isHealthCheckLoggingEnabled)
            where TRecord : RepositoryRecord
            where TConfiguration: class, ISqlApiRepositoryConfiguration, IValidatable
        {
            services.AddSingleton<TConfiguration>();
            services.AddTransient<IValidatable>(sp => sp.GetRequiredService<TConfiguration>());
            services.AddTransient<ISqlApiRepository<TRecord>, SqlApiRepository<TConfiguration, TRecord>>();

            if (isHealthCheckLoggingEnabled)
            {
                services
                    .AddHealthChecks()
                    .AddCheck<SqlApiClientHealthCheck<TConfiguration>>(typeof(TConfiguration).Name,
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

        public static void RegisterSqlApiDatabaseClient(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var clientOptions = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway
            };

            var connectionString = configuration.GetOrWarn("COSMOS_SQL_API_CONNECTION_STRING", logger);
            if (connectionString.IsNullOrEmpty())
            {
                logger.LogInformation("Creating CosmosClient using role based access");

                var connectionUri = configuration.GetOrThrow("COSMOS_SQL_API_ENDPOINT", logger);
                var tenantId = configuration.GetOrThrow("KEYVAULT_TENANT_ID", logger);
                var nhsappSqlApplicationId = configuration.GetOrThrow("NHSAPP_SQL_APPLICATION_ID", logger);
                var nhsappSqlApplicationSecret = configuration.GetOrThrow("NHSAPP_SQL_APPLICATION_SECRET", logger);

                var credentials = new ClientSecretCredential(
                    tenantId, nhsappSqlApplicationId, nhsappSqlApplicationSecret);

                var cosmosWrapper = new CosmosClientWrapper(
                    new CosmosClient(connectionUri, credentials, clientOptions)
                );

                services.AddSingleton<ICosmosClientWrapper>(cosmosWrapper);
            }
            else
            {
                logger.LogInformation("Creating CosmosClient using connection string");
                var cosmosWrapper = new CosmosClientWrapper(
                    new CosmosClient(
                        connectionString,
                        clientOptions)
                );

                services.AddSingleton<ICosmosClientWrapper>(cosmosWrapper);
            }

            services.AddSingleton<ISqlApiClientService, SqlApiClientService>();
            services.AddSingleton<ICosmosLinqQuery, CosmosLinqQuery>();
        }
    }
}
