using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    public class Im1CacheServiceConfig : IIm1CacheServiceConfig
    {
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string SecondaryDatabaseName { get; }
        public string SecondaryCollectionName { get; }

        public Im1CacheServiceConfig(IConfiguration configuration, ILogger<Im1CacheServiceConfig> logger)
        {
            DatabaseName = configuration.GetOrThrow("IM1CACHE_MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("IM1CACHE_MONGO_DATABASE_COLLECTION", logger);
            SecondaryDatabaseName = configuration.GetOrThrow("IM1CACHE_MONGO_SECONDARY_DATABASE_NAME", logger);
            SecondaryCollectionName = configuration.GetOrThrow("IM1CACHE_MONGO_SECONDARY_DATABASE_COLLECTION", logger);
        }
    }
}