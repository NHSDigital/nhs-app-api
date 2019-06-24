using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestConfigurationSettings
    {
        public Uri BaseUrl { get; set; }

        public string CertificatePath { get; set; }

        public string CertificatePassphrase { get; set; }

        public string Environment { get; set; }

        public int? CoursesMaxCoursesLimit { get; set; }

        public int? PrescriptionsMaxCoursesSoftLimit { get; set; }

        public MicrotestConfigurationSettings(Uri baseUrl, string certificatePath, string certificatePassphrase, string environment, int? prescriptionsMaxCoursesSoftLimit, int? coursesMaxCoursesLimit)
        {
            BaseUrl = baseUrl;
            CertificatePath = certificatePath;
            CertificatePassphrase = certificatePassphrase;
            Environment = environment;
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
    }
}
