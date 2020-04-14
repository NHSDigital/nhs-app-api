using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public class MongoSessionCacheServiceConfig : IMongoSessionCacheServiceConfig
    {
        public string MongoDatabaseName { get; }
        public string MongoDatabaseIm1CollectionName { get; }

        public MongoSessionCacheServiceConfig(IConfiguration configuration, ILogger<MongoSessionCacheServiceConfig> logger)
        {
            MongoDatabaseName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_NAME", logger);
            MongoDatabaseIm1CollectionName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_COLLECTION", logger);
        }
    }
}