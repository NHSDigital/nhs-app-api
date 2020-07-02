using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Auditing
{
    public class RepositoryDbAuditSinkConfiguration : IRepositoryConfiguration
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }

        public RepositoryDbAuditSinkConfiguration(IConfiguration configuration, ILogger<RepositoryDbAuditSinkConfiguration> logger)
        {
            ConnectionString = configuration.GetOrThrow("MONGO_CONNECTION_STRING", logger);
            DatabaseName = configuration.GetOrThrow("AUDIT_MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("AUDIT_MONGO_DATABASE_COLLECTION", logger);
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new ConfigurationNotFoundException(nameof(ConnectionString));
            }
            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                throw new ConfigurationNotFoundException(nameof(DatabaseName));
            }
            if (string.IsNullOrWhiteSpace(CollectionName))
            {
                throw new ConfigurationNotFoundException(nameof(CollectionName));
            }
        }
    }
}