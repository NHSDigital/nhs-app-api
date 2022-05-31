using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Repository.SqlApi;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UserInfo
{
    public class UserAndInfoRepositoryByNhsNumberConfiguration : ISqlApiRepositoryConfiguration
    {
        public string DatabaseName { get; private set; }
        public string ContainerName { get; private set; }

        public UserAndInfoRepositoryByNhsNumberConfiguration(
            IConfiguration configuration,
            ILogger<UserAndInfoRepositoryByNhsNumberConfiguration> logger)
        {
            DatabaseName = configuration.GetOrThrow("COMMS_SQL_API_DATABASE_NAME", logger);
            ContainerName = configuration.GetOrThrow("USERINFO_NHS_NUMBER_CONTAINER", logger);
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