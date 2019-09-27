using System.Globalization;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Support
{
    public static class ConfigurationExtensions
    {
        private const string LogMessage =
            "Attempted to get a value for {0} from the environment but no value was found.";

        private const string LogMessageParseError =
            "Attempted to parse value {0} for name {1} from the environment but encountered an error.";

        public static string GetOrWarn<T>(this IConfiguration configuration, string key, ILogger<T> logger)
        {
            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                logger.LogWarning(string.Format(CultureInfo.InvariantCulture, LogMessage, key));
            }

            return value;
        }

        public static string GetOrThrow<T>(this IConfiguration configuration, string key, ILogger<T> logger)
        {
            var value = configuration[key];

            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            logger.LogError(string.Format(CultureInfo.InvariantCulture, LogMessage, key));
            throw new ConfigurationNotFoundException(key);
        }

        public static string GetOrNull(this IConfiguration configuration, string key)
        {
            return configuration[key];
        }

        public static int GetIntOrThrow<T>(this IConfiguration configuration, string key, ILogger<T> logger)
        {
            var strValue = GetOrThrow(configuration, key, logger);

            if (int.TryParse(strValue, out var value))
            {
                return value;
            }

            logger.LogError(string.Format(CultureInfo.InvariantCulture, LogMessageParseError, strValue, key));
            throw new ConfigurationNotFoundException(key);
        }

        public static int GetIntOrDefault<T>(this IConfiguration configuration, string key, ILogger<T> logger)
        {
            var value = 0;
            var strValue = GetOrWarn(configuration, key, logger);

            if (strValue != null && !int.TryParse(strValue, out value))
            {
                logger.LogWarning(string.Format(CultureInfo.InvariantCulture, LogMessageParseError, strValue, key));
            }

            return value;
        }

        public static int GetIntOrWarn<T>(this IConfiguration configuration, string key, ILogger<T> logger)
        {
            var strValue = GetOrWarn(configuration, key, logger);

            if (int.TryParse(strValue, out var value))
            {
                return value;
            }

            logger.LogWarning(string.Format(CultureInfo.InvariantCulture, LogMessage, key));
            throw new ConfigurationNotFoundException(key);
        }

        public static IConfigurationSection ConfigurationSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("ConfigurationSettings");
        }

        public static string GetApiAppVersion(this IConfiguration configuration)
        {
            var apiVersionStringBuilder = new StringBuilder();
            apiVersionStringBuilder.Append(configuration[Constants.EnvironmentalVariables.VersionTag]);
            apiVersionStringBuilder.Append(" (commit:");
            apiVersionStringBuilder.Append(configuration[Constants.AppConfig.GitCommitId]);
            apiVersionStringBuilder.Append(")");

            return apiVersionStringBuilder.ToString();
        }
    }
}