using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class ConsentRepositoryConfiguration : IRepositoryConfiguration
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }

        public ConsentRepositoryConfiguration(IConfiguration configuration, ILogger<ConsentRepositoryConfiguration> logger)
        {
            ConnectionString = configuration.GetOrThrow("MONGO_CONNECTION_STRING", logger);
            DatabaseName = configuration.GetOrThrow("CONSENT_MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("CONSENT_MONGO_DATABASE_COLLECTION", logger);
        }

        public void Validate()
        {
            if (ConnectionString == null)
            {
                throw new ConfigurationNotFoundException(nameof(ConnectionString));
            }
            if (DatabaseName == null)
            {
                throw new ConfigurationNotFoundException(nameof(DatabaseName));
            }
            if (CollectionName == null)
            {
                throw new ConfigurationNotFoundException(nameof(CollectionName));
            }
        }
    }
}