using System;
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

        public string Environment { get; set;}
        
        public EmisConfigurationSettings(Uri baseUrl, string applicationId, string version, string certificatePath, string certificatePassphrase,
            int emisExtendedHttpTimeoutSeconds, int defaultHttpTimeoutSeconds, int? coursesMaxCoursesLimit, int? prescriptionsMaxCoursesSoftLimit, string environment)
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
            Environment = environment;
        }
        public void Validate()
        {
            if(BaseUrl == null) 
            {
                throw new ConfigurationNotFoundException(nameof(BaseUrl));
            }
            
            if(String.IsNullOrEmpty(ApplicationId))
            {
                throw new ConfigurationNotFoundException($"{nameof(ApplicationId)} cannot be null or empty");
            }

            if(String.IsNullOrEmpty(Version))
            {
                throw new ConfigurationNotFoundException($"{nameof(Version)} cannot be null or empty");
            }

            if (EmisExtendedHttpTimeoutSeconds == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(EmisExtendedHttpTimeoutSeconds));
            }

            if (DefaultHttpTimeoutSeconds == default(int))
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
    }
}
