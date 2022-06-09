using NHSOnline.Backend.Repository.SqlApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Messages
{
    public class SenderRepositoryConfiguration : ISqlApiRepositoryConfiguration
    {
        public string DatabaseName { get; private set; }
        public string ContainerName { get; private set; }

        public SenderRepositoryConfiguration(IConfiguration configuration,
            ILogger<SenderRepositoryConfiguration> logger)
        {
            DatabaseName = configuration.GetOrThrow("COMMS_SQL_API_DATABASE_NAME", logger);
            ContainerName = configuration.GetOrThrow("SENDERS_CONTAINER", logger);
        }

        public void Validate()
        {
            if (DatabaseName == null)
            {
                throw new ConfigurationNotFoundException(nameof(DatabaseName));
            }

            if (ContainerName == null)
            {
                throw new ConfigurationNotFoundException(nameof(ContainerName));
            }
        }
    }
}