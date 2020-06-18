using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Settings
{
    public class ConfigurationSettings: IConfigurationSettings, IHttpTimeoutConfigurationSettings
    {
        public string CookieDomain { get; set; }

        public int? PrescriptionsDefaultLastNumberMonthsToDisplay { get; set; }

        public int DefaultSessionExpiryMinutes { get; set; }

        public int DefaultHttpTimeoutSeconds { get; set; }

        public int MinimumAppAge { get; set; }

        public int MinimumLinkageAge { get; set; }

        public ConfigurationSettings() {}

        public ConfigurationSettings(
            string cookieDomain,
            int? prescriptionsDefaultLastNumberMonthsToDisplay,
            int defaultSessionExpiryMinutes,
            int defaultHttpTimeoutSeconds,
            int minimumAppAge,
            int minimumLinkageAge)
        {
            CookieDomain = cookieDomain;
            PrescriptionsDefaultLastNumberMonthsToDisplay = prescriptionsDefaultLastNumberMonthsToDisplay;
            DefaultHttpTimeoutSeconds = defaultHttpTimeoutSeconds;
            DefaultSessionExpiryMinutes = defaultSessionExpiryMinutes;
            MinimumAppAge = minimumAppAge;
            MinimumLinkageAge = minimumLinkageAge;
        }

        public void Validate()
        {
            if (PrescriptionsDefaultLastNumberMonthsToDisplay == null)
            {
                throw new ConfigurationNotFoundException(nameof(PrescriptionsDefaultLastNumberMonthsToDisplay));
            }

            if (DefaultSessionExpiryMinutes == default)
            {
                throw new ConfigurationNotFoundException(nameof(DefaultSessionExpiryMinutes));
            }

            if (DefaultHttpTimeoutSeconds == default)
            {
                throw new ConfigurationNotFoundException(nameof(DefaultHttpTimeoutSeconds));
            }

            if (MinimumAppAge == default)
            {
                throw new ConfigurationNotFoundException(nameof(MinimumAppAge));
            }

            if (MinimumLinkageAge == default)
            {
                throw new ConfigurationNotFoundException(nameof(MinimumLinkageAge));
            }
        }

        public static ConfigurationSettings CreateAndValidate(IConfiguration configuration, ILogger logger)
        {
            var cookieDomain = configuration["ConfigurationSettings:CookieDomain"];
            var prescriptionsDefaultLastNumberMonthsToDisplay = configuration.GetIntOrThrow(
                "ConfigurationSettings:PrescriptionsDefaultLastNumberMonthsToDisplay",
                logger);
            var defaultSessionExpiryMinutes = configuration.GetIntOrThrow("ConfigurationSettings:DefaultSessionExpiryMinutes", logger);
            var defaultHttpTimeoutSeconds = configuration.GetIntOrThrow("ConfigurationSettings:DefaultHttpTimeoutSeconds", logger);
            var minimumAppAge = configuration.GetIntOrThrow("ConfigurationSettings:MinimumAppAge", logger);
            var minimumLinkageAge = configuration.GetIntOrThrow("ConfigurationSettings:MinimumLinkageAge", logger);

            var config = new ConfigurationSettings(
                cookieDomain,
                prescriptionsDefaultLastNumberMonthsToDisplay,
                defaultSessionExpiryMinutes,
                defaultHttpTimeoutSeconds,
                minimumAppAge,
                minimumLinkageAge);

            config.Validate();
            return config;
        }
    }
}
