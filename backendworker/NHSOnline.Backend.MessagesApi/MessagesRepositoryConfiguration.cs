using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.MessagesApi
{
    internal class MessagesRepositoryConfiguration : IRepositoryConfiguration
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }

        public MessagesRepositoryConfiguration(IConfiguration configuration, ILogger<MessagesRepositoryConfiguration> logger)
        {
            ConnectionString = configuration.GetOrThrow("MONGO_CONNECTION_STRING", logger);
            DatabaseName = configuration.GetOrThrow("MESSAGES_MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("MESSAGES_MONGO_DATABASE_MESSAGES_COLLECTION", logger);
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