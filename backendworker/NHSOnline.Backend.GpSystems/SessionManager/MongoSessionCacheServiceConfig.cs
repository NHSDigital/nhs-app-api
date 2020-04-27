using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public class MongoSessionCacheServiceConfig : IMongoSessionCacheServiceConfig
    {
        public string MongoDatabaseName { get; }
        public string MongoDatabaseSessionCollectionName { get; }

        public MongoSessionCacheServiceConfig(IConfiguration configuration, ILogger<MongoSessionCacheServiceConfig> logger)
        {
            MongoDatabaseName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_NAME", logger);
            MongoDatabaseSessionCollectionName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_COLLECTION", logger);
        }
    }
}