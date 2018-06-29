using Microsoft.Extensions.Configuration;

namespace NHSOnline.Backend.Worker.Settings
{
    public class ConfigurationSettings
    {
        public int? PrescriptionsDefaultLastNumberMonthsToDisplay { get; set; }

        public int? PrescriptionsMaxCoursesSoftLimit { get; set; }

        public int? CoursesMaxCoursesLimit { get; set; }

        public int DefaultSessionExpiryMinutes { get; set; }

        public int DefaultHttpTimeoutSeconds { get; set; }

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
        }
    }
}