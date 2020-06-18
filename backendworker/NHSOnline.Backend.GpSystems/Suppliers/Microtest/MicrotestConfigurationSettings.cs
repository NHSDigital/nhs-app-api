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

        public string CertificatePath { get; set; }

        public string CertificatePassphrase { get; set; }

        public int? CoursesMaxCoursesLimit { get; set; }

        public int? PrescriptionsMaxCoursesSoftLimit { get; set; }

        public MicrotestConfigurationSettings(
            Uri baseUrl,
            string certificatePath,
            string certificatePassphrase,
            int? prescriptionsMaxCoursesSoftLimit,
            int? coursesMaxCoursesLimit)
        {
            BaseUrl = baseUrl;
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
            var certificatePath = configuration.GetOrWarn("MICROTEST_CERT_PATH", logger);
            var certificatePassphrase = configuration.GetOrWarn("MICROTEST_CERT_PASSPHRASE", logger);

            var prescriptionsMaxCoursesSoftLimit = configuration.GetIntOrWarn("ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit", logger);
            var coursesMaxCoursesLimit = configuration.GetIntOrWarn("ConfigurationSettings:CoursesMaxCoursesLimit", logger);

            var config = new MicrotestConfigurationSettings(
                new Uri(baseUrlstring),
                certificatePath,
                certificatePassphrase,
                prescriptionsMaxCoursesSoftLimit,
                coursesMaxCoursesLimit);

            config.Validate();

            return config;
        }
    }
}
