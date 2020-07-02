using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UserInfoApi
{
    internal class UserAndInfoRepositoryConfiguration : IRepositoryConfiguration
    {
        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }

        public UserAndInfoRepositoryConfiguration(IConfiguration configuration, ILogger<UserAndInfoRepositoryConfiguration> logger)
        {
            ConnectionString = configuration.GetOrThrow("MONGO_CONNECTION_STRING", logger);
            DatabaseName = configuration.GetOrThrow("USERINFO_MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("USERINFO_MONGO_DATABASE_COLLECTION", logger);
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