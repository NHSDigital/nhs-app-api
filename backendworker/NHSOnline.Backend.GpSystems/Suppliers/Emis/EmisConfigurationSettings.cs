using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisConfigurationSettings
    {
        public Uri BaseUrl { get; set; }
        public string ApplicationId { get; set; }
        public string Version { get; set; }
        public string CertificatePath { get; }
        public string CertificatePassphrase { get; }
        public int EmisExtendedHttpTimeoutSeconds { get; set; }
        public int DefaultHttpTimeoutSeconds { get; set; }
        public int? CoursesMaxCoursesLimit { get; set; }
        public int? PrescriptionsMaxCoursesSoftLimit { get; set; }

        public EmisConfigurationSettings(
            Uri baseUrl,
            string applicationId,
            string version,
            string certificatePath,
            string certificatePassphrase,
            int emisExtendedHttpTimeoutSeconds,
            int defaultHttpTimeoutSeconds,
            int? coursesMaxCoursesLimit,
            int? prescriptionsMaxCoursesSoftLimit)
        {
            BaseUrl = baseUrl;
            ApplicationId = applicationId;
            Version = version;
            CertificatePath = certificatePath;
            CertificatePassphrase = certificatePassphrase;

            EmisExtendedHttpTimeoutSeconds = emisExtendedHttpTimeoutSeconds;
            DefaultHttpTimeoutSeconds = defaultHttpTimeoutSeconds;
            CoursesMaxCoursesLimit = coursesMaxCoursesLimit;
            PrescriptionsMaxCoursesSoftLimit = prescriptionsMaxCoursesSoftLimit;
        }

        public void Validate()
        {
            if(BaseUrl == null) 
            {
                throw new ConfigurationNotFoundException(nameof(BaseUrl));
            }
            
            if(string.IsNullOrEmpty(ApplicationId))
            {
                throw new ConfigurationNotFoundException($"{nameof(ApplicationId)} cannot be null or empty");
            }

            if(string.IsNullOrEmpty(Version))
            {
                throw new ConfigurationNotFoundException($"{nameof(Version)} cannot be null or empty");
            }

            if (EmisExtendedHttpTimeoutSeconds == default)
            {
                throw new ConfigurationNotFoundException(nameof(EmisExtendedHttpTimeoutSeconds));
            }

            if (DefaultHttpTimeoutSeconds == default)
            {
                throw new ConfigurationNotFoundException(nameof(DefaultHttpTimeoutSeconds));
            }

            if (PrescriptionsMaxCoursesSoftLimit == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(PrescriptionsMaxCoursesSoftLimit));
            }

            if (CoursesMaxCoursesLimit == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(CoursesMaxCoursesLimit));
            }

        }

        public static EmisConfigurationSettings CreateAndValidate(IConfiguration configuration, ILogger logger)
        {
            var emisBaseUrl = configuration.GetOrWarn("EMIS_BASE_URL", logger);
            var applicationId = configuration.GetOrWarn("EMIS_APPLICATION_ID", logger);
            var version = configuration.GetOrWarn("EMIS_VERSION", logger);
            var certificatePath = configuration.GetOrWarn("EMIS_CERTIFICATE_PATH", logger);
            var certificatePassphrase = configuration.GetOrWarn("EMIS_CERTIFICATE_PASSWORD", logger);

            var emisExtendedHttpTimeoutSeconds = configuration.GetIntOrWarn("ConfigurationSettings:EmisExtendedHttpTimeoutSeconds", logger);
            var defaultHttpTimeoutSeconds = configuration.GetIntOrWarn("ConfigurationSettings:DefaultHttpTimeoutSeconds", logger);
            var coursesMaxCoursesLimit = configuration.GetIntOrWarn("ConfigurationSettings:CoursesMaxCoursesLimit", logger);
            var prescriptionsMaxCoursesSoftLimit = configuration.GetIntOrWarn("ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit", logger);

            var config = new EmisConfigurationSettings(
                new Uri(emisBaseUrl, UriKind.Absolute),
                applicationId,
                version,
                certificatePath,
                certificatePassphrase,
                emisExtendedHttpTimeoutSeconds,
                defaultHttpTimeoutSeconds,
                coursesMaxCoursesLimit,
                prescriptionsMaxCoursesSoftLimit);

            config.Validate();

            return config;
        }
    }
}
