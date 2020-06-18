using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class TppConfigurationSettings: IValidatable
    {
        public Uri ApiUrl { get; set; }
        public string ApiVersion { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string ApplicationProviderId { get; set; }
        public string ApplicationDeviceType { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassphrase { get; set; }
        public int? PrescriptionsMaxCoursesSoftLimit { get; set; }
        public int? CoursesMaxCoursesLimit { get; set; }
        public bool SupportsLinkedAccounts { get; set; }

        public TppConfigurationSettings() {}
        
        public TppConfigurationSettings(
            Uri baseUrl,
            string apiVersion,
            string applicationName,
            string applicationVersion, 
            string applicationProviderId,
            string applicationDeviceType,
            string certificatePath,
            string certificatePassphrase,
            int? prescriptionsMaxCoursesSoftLimit,
            int? coursesMaxCoursesLimit,
            string supportsLinkedAccounts)
        {           
            ApiUrl = baseUrl;
            ApiVersion = apiVersion;
            ApplicationName = applicationName;
            ApplicationVersion = applicationVersion;
            ApplicationProviderId = applicationProviderId;
            ApplicationDeviceType = applicationDeviceType;
            CertificatePath = certificatePath;
            CertificatePassphrase = certificatePassphrase;
            PrescriptionsMaxCoursesSoftLimit = prescriptionsMaxCoursesSoftLimit;
            CoursesMaxCoursesLimit = coursesMaxCoursesLimit;

            SupportsLinkedAccounts = string.Equals("true", supportsLinkedAccounts, StringComparison.Ordinal);
        }

        public void Validate()
        {

            if (PrescriptionsMaxCoursesSoftLimit == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(PrescriptionsMaxCoursesSoftLimit));
            }

            if (CoursesMaxCoursesLimit == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(CoursesMaxCoursesLimit));
            }

            if (ApiUrl == null)
            {
                throw new ConfigurationNotFoundException("ApiUrl cannot be null or empty");
            }

            if (string.IsNullOrEmpty(CertificatePath))
            {
                throw new ConfigurationNotFoundException("CertificatePath cannot be null or empty");
            }

            if (string.IsNullOrEmpty(ApiVersion))
            {
                throw new ConfigurationNotFoundException("ApiVersion cannot be null or empty");
            }

            if (string.IsNullOrEmpty(ApplicationName))
            {
                throw new ConfigurationNotFoundException("ApplicationName cannot be null or empty");
            }

            if (string.IsNullOrEmpty(ApplicationProviderId))
            {
                throw new ConfigurationNotFoundException("ApplicationProviderId passphrase cannot be null or empty");
            }

            if (string.IsNullOrEmpty(ApplicationDeviceType))
            {
                throw new ConfigurationNotFoundException("ApplicationDeviceType cannot be null or empty");
            }
        }

        public static TppConfigurationSettings CreateAndValidate(IConfiguration configuration, ILogger logger)
        {
            var tppBaseUrl = configuration.GetOrWarn("TPP_BASE_URL", logger);
            var apiVersion = configuration.GetOrWarn("TPP_API_VERSION", logger);
            var applicationName = configuration.GetOrWarn("TPP_APPLICATION_NAME", logger);
            var applicationVersion = configuration.GetOrWarn("TPP_APPLICATION_VERSION", logger);
            var applicationProviderId = configuration.GetOrWarn("TPP_APPLICATION_PROVIDER_ID", logger);
            var applicationDeviceType = configuration.GetOrWarn("TPP_APPLICATION_DEVICE_TYPE", logger);
            var certificatePath = configuration.GetOrWarn("TPP_CERTIFICATE_PATH", logger);
            var certificatePassphrase = configuration.GetOrWarn("TPP_CERTIFICATE_PASSWORD", logger);
            var supportsLinkedAccounts = configuration.GetOrWarn("TPP_SUPPORTS_LINKED_ACCOUNTS", logger);

            var prescriptionsMaxCoursesSoftLimit = configuration.GetIntOrWarn("ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit", logger);
            var coursesMaxCoursesLimit = configuration.GetIntOrWarn("ConfigurationSettings:CoursesMaxCoursesLimit", logger);

            var config = new TppConfigurationSettings(
                new Uri(tppBaseUrl, UriKind.Absolute),
                apiVersion,
                applicationName,
                applicationVersion,
                applicationProviderId,
                applicationDeviceType,
                certificatePath,
                certificatePassphrase,
                prescriptionsMaxCoursesSoftLimit,
                coursesMaxCoursesLimit,
                supportsLinkedAccounts);

            config.Validate();

            return config;
        }
    }
}