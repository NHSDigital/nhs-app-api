using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users
{
    internal class FakeGpUserRepoConfiguration : IRepositoryConfiguration
    {
        public string DatabaseName { get; private set; }
        public string CollectionName { get; private set; }

        public FakeGpUserRepoConfiguration(IConfiguration configuration, ILogger<RepositoryDbAuditSinkConfiguration> logger)
        {
            DatabaseName = configuration.GetOrThrow("MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("FAKE_GP_USERS_DATABASE_COLLECTION", logger);
        }

        public void Validate()
        {
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
