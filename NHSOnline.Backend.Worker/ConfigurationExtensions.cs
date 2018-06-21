using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker
{
    public static class ConfigurationExtensions
    {
        public static string GetOrWarn<T>(this IConfiguration configuration, string key, ILogger<T> logger)
        {
            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                logger.LogWarning($"Attempted to get a value for {key} from the environment but no value was found.", key);
            }

            return value;
        }

        public static string GetOrWarn<T>(this IConfigurationSection configuration, string key, ILogger<T> logger)
        {
            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                logger.LogWarning($"Attempted to get a value for {key} from the environment but no value was found.", key);
            }

            return value;
        }

        public static IConfigurationSection ConfigurationSettings(this IConfiguration configuration)
        {
            return configuration.GetSection("ConfigurationSettings");
        }

    }
}