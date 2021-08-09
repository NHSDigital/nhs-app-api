using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using ValidationOptions = NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    internal sealed class Im1CacheConfig : IRepositoryConfiguration
    {
        private readonly ILogger<Im1CacheConfig> _logger;

        public Im1CacheConfig(IConfiguration configuration, ILogger<Im1CacheConfig> logger)
        {
            _logger = logger;

            DatabaseName = configuration.GetOrWarn("MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrWarn("IM1CACHE_MONGO_DATABASE_COLLECTION", logger);
        }

        public string DatabaseName { get; }
        public string CollectionName { get; }

        public void Validate()
        {
            ValidateAndLog
                .Using(_logger)
                .IsNotNullOrWhitespace(DatabaseName, "MONGO_DATABASE_NAME", ValidationOptions.ThrowError)
                .IsNotNullOrWhitespace(CollectionName, "IM1CACHE_MONGO_DATABASE_COLLECTION", ValidationOptions.ThrowError)
                .IsValid();
        }
    }
}
