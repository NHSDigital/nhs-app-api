using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Repository.SqlApi;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.UserInfo
{
    /// <summary>
    /// IMPORTANT! The name of this class is purposefully named with "NhsNo" i.e. shorthand for "NhsNumber".
    /// Logs containing the term "NhsNumber" are deemed sensitive and can only be accessed by those with elevated
    /// privileges. By avoiding the word "NhsNumber", any logs written which include this class name are not deemed
    /// sensitive and can be accessed by any privilege level.
    /// </summary>
    public class UserAndInfoRepositoryByNhsNoConfiguration : ISqlApiRepositoryConfiguration
    {
        public string DatabaseName { get; private set; }
        public string ContainerName { get; private set; }

        public UserAndInfoRepositoryByNhsNoConfiguration(
            IConfiguration configuration,
            ILogger<UserAndInfoRepositoryByNhsNoConfiguration> logger)
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