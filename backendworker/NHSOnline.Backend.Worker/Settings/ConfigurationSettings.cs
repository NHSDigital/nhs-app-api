using System;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Worker.Settings
{
    public class ConfigurationSettings
    {
        public string CookieDomain { get; set; }
        
        public int? PrescriptionsDefaultLastNumberMonthsToDisplay { get; set; }

        public int? PrescriptionsMaxCoursesSoftLimit { get; set; }

        public int? CoursesMaxCoursesLimit { get; set; }

        public int DefaultSessionExpiryMinutes { get; set; }

        public int DefaultHttpTimeoutSeconds { get; set; }
        
        public int MinimumAppAge { get; set; }
        
        public int MinimumLinkageAge { get; set; }
        
        public DateTimeOffset? CurrentTermsConditionsEffectiveDate { get; set; }

        public string MinimumSupportedAndroidVersion { get; set; }

        public string MinimumSupportediOSVersion { get; set; }
        
        public string ThrottlingEnabled { get; set; }

        public Uri FidoServerUrl { get; set; }

        public const string ConfigurationSectionName = "ConfigurationSettings";

        public static ConfigurationSettings GetSettings(IConfiguration configuration)
        {
            var configurationSettings = configuration.GetSection(ConfigurationSectionName).Get<ConfigurationSettings>();
            configurationSettings.EnsureConfigurationSettingsPopulated();
            return configurationSettings;
        }

        private void EnsureConfigurationSettingsPopulated()
        {
            if (PrescriptionsDefaultLastNumberMonthsToDisplay == null)
            {
                throw new ConfigurationNotFoundException(nameof(PrescriptionsDefaultLastNumberMonthsToDisplay));
            }

            if (PrescriptionsMaxCoursesSoftLimit == null)
            {
                throw new ConfigurationNotFoundException(nameof(PrescriptionsMaxCoursesSoftLimit));
            }

            if (CoursesMaxCoursesLimit == null)
            {
                throw new ConfigurationNotFoundException(nameof(CoursesMaxCoursesLimit));
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

            if (MinimumSupportedAndroidVersion == null)
            {
                throw new ConfigurationNotFoundException(nameof(MinimumSupportedAndroidVersion));
            }

            if (MinimumSupportediOSVersion == null)
            {
                throw new ConfigurationNotFoundException(nameof(MinimumSupportediOSVersion));
            }
            
            if (CurrentTermsConditionsEffectiveDate == null)
            {
                throw new ConfigurationNotFoundException(nameof(CurrentTermsConditionsEffectiveDate));
            }

            if (ThrottlingEnabled == null)
            {
                throw new ConfigurationNotFoundException(nameof(ThrottlingEnabled));
            }

            if (FidoServerUrl == null)
            {
                throw new ConfigurationNotFoundException(nameof(FidoServerUrl));
            }
        }
    }
}
