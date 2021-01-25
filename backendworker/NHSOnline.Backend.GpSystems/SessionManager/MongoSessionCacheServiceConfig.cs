using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    internal sealed class MongoSessionCacheServiceConfig : IMongoSessionCacheServiceConfig
    {
        public string DatabaseName { get; }
        public string CollectionName { get; }

        public MongoSessionCacheServiceConfig(IConfiguration configuration, ILogger<MongoSessionCacheServiceConfig> logger)
        {
            DatabaseName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_COLLECTION", logger);
        }
    }
}