using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestConfigurationSettings
    {
        public Uri BaseUrl { get; set; }

        public bool CertificateEnabled { get; set; }

        public string CertificatePath { get; set; }

        public string CertificatePassphrase { get; set; }

        public int? CoursesMaxCoursesLimit { get; set; }

        public int? PrescriptionsMaxCoursesSoftLimit { get; set; }

        public MicrotestConfigurationSettings(
            Uri baseUrl,
            bool certificateEnabled,
            string certificatePath,
            string certificatePassphrase,
            int? prescriptionsMaxCoursesSoftLimit,
            int? coursesMaxCoursesLimit)
        {
            BaseUrl = baseUrl;
            CertificateEnabled = certificateEnabled;
            CertificatePath = certificatePath;
            CertificatePassphrase = certificatePassphrase;
            PrescriptionsMaxCoursesSoftLimit = prescriptionsMaxCoursesSoftLimit;
            CoursesMaxCoursesLimit = coursesMaxCoursesLimit;
        }

        public void Validate()
        {
            if (BaseUrl == null)
            {
                throw new ConfigurationNotFoundException("BaseUrl cannot be null");
            }
        }

        public static MicrotestConfigurationSettings CreateAndValidate(IConfiguration configuration, ILogger logger)
        {
            var baseUrlstring = configuration.GetOrWarn("MICROTEST_BASE_URL", logger);
            var certificateEnabled = configuration.GetBoolOrThrow("MICROTEST_CERTIFICATE_ENABLED", logger);

            var certificatePath = certificateEnabled
                ? configuration.GetOrWarn("MICROTEST_CERT_PATH", logger)
                : string.Empty;
            var certificatePassphrase = certificateEnabled
                ? configuration.GetOrWarn("MICROTEST_CERT_PASSPHRASE", logger)
                : string.Empty;

            var prescriptionsMaxCoursesSoftLimit = configuration.GetIntOrWarn("ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit", logger);
            var coursesMaxCoursesLimit = configuration.GetIntOrWarn("ConfigurationSettings:CoursesMaxCoursesLimit", logger);

            var config = new MicrotestConfigurationSettings(
                new Uri(baseUrlstring),
                certificateEnabled,
                certificatePath,
                certificatePassphrase,
                prescriptionsMaxCoursesSoftLimit,
                coursesMaxCoursesLimit);

            config.Validate();

            return config;
        }
    }
}
