using System;

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

        public ConfigurationSettings(string cookieDomain, int? prescriptionsDefaultLastNumberMonthsToDisplay,
            int defaultSessionExpiryMinutes, int defaultHttpTimeoutSeconds, int minimumAppAge, int minimumLinkageAge)
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

            if (DefaultSessionExpiryMinutes == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(DefaultSessionExpiryMinutes));
            }

            if (DefaultHttpTimeoutSeconds == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(DefaultHttpTimeoutSeconds));
            }

            if (MinimumAppAge == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(MinimumAppAge));
            }

            if (MinimumLinkageAge == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(MinimumLinkageAge));
            }
        }
    }
}
