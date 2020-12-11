using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    public class Im1CacheServiceConfig : IIm1CacheServiceConfig
    {
        public string MongoDatabaseName { get; }
        public string MongoDatabaseIm1CollectionName { get; }

        public Im1CacheServiceConfig(IConfiguration configuration, ILogger<Im1CacheServiceConfig> logger)
        {
            MongoDatabaseName = configuration.GetOrThrow("IM1CACHE_MONGO_DATABASE_NAME", logger);
            MongoDatabaseIm1CollectionName = configuration.GetOrThrow("IM1CACHE_MONGO_DATABASE_COLLECTION", logger);
        }
    }
}