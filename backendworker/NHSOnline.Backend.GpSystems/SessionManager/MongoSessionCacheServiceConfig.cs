using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    internal sealed class MongoSessionCacheServiceConfig : IMongoSessionCacheServiceConfig
    {
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string SecondaryDatabaseName { get; }
        public string SecondaryCollectionName { get; }

        public MongoSessionCacheServiceConfig(IConfiguration configuration, ILogger<MongoSessionCacheServiceConfig> logger)
        {
            DatabaseName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_COLLECTION", logger);
            SecondaryDatabaseName = configuration.GetOrThrow("SESSION_MONGO_SECONDARY_DATABASE_NAME", logger);
            SecondaryCollectionName = configuration.GetOrThrow("SESSION_MONGO_SECONDARY_DATABASE_COLLECTION", logger);
        }
    }
}