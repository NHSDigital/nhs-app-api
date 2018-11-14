using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker
{
    public static class ConfigurationExtensions
    {
        private const string LogMessage = "Attempted to get a value for {0} from the environment but no value was found.";

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
            if (string.IsNullOrEmpty(value))
            {
                logger.LogError(string.Format(CultureInfo.InvariantCulture, LogMessage, key));
                throw new ConfigurationNotFoundException(key);
            }

            return value;
        }
        
        public static string GetOrNull(this IConfiguration configuration, string key)
        {
            return configuration[key];
        }
        
        public static int GetIntOrThrow<T>(this IConfiguration configuration, string key, ILogger<T> logger)
        {
            var strValue = GetOrThrow(configuration, key, logger);
            int value;
            if (!int.TryParse(strValue, out value))
            {
                logger.LogError(string.Format(CultureInfo.InvariantCulture, LogMessage, key));
                throw new ConfigurationNotFoundException(key);
            }
            return value;
        }

        public static IConfigurationSection ConfigurationSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("ConfigurationSettings");
        }
    }
}