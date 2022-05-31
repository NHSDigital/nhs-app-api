using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.UserInfo
{
    public class UserInfoConfiguration : IUserInfoConfiguration
    {
        public bool SaveToSecondaryContainers { get; }
        public bool ReadFromSecondaryContainers { get; }

        public UserInfoConfiguration(IConfiguration configuration, ILogger<UserInfoConfiguration> logger)
        {
            (SaveToSecondaryContainers, ReadFromSecondaryContainers) = GetConfigItems(configuration, logger);
        }

        public static bool IsSqlApiClientRequired(IConfiguration configuration, ILogger logger)
        {
            var (saveToSecondaryContainers, readFromSecondaryContainer) = GetConfigItems(configuration, logger);
            return saveToSecondaryContainers || readFromSecondaryContainer;
        }

        private static (bool saveToSecondaryContainers, bool readfromSecondaryContainer) GetConfigItems(
            IConfiguration configuration, ILogger logger)
        {
            var saveEnabled = bool.TrueString.Equals(configuration.GetOrWarn("USER_INFO_SAVE_TO_SECONDARY_CONTAINERS", logger), StringComparison.OrdinalIgnoreCase);
            var readEnabled = bool.TrueString.Equals(configuration.GetOrWarn("USER_INFO_READ_FROM_SECONDARY_CONTAINERS", logger), StringComparison.OrdinalIgnoreCase);

            return (saveEnabled, readEnabled);
        }
    }
}