using System;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp {
    public class TppConfigurationSettings: IValidateable
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
        public string Environment { get; set; }

        public TppConfigurationSettings() {}
        
        public TppConfigurationSettings(Uri baseUrl, string apiVersion, string applicationName, string applicationVersion, 
            string applicationProviderId, string applicationDeviceType, string certificatePath, string certificatePassphrase,
            int? prescriptionsMaxCoursesSoftLimit, int? coursesMaxCoursesLimit, string environment)
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
            Environment = environment;
        }

        public void Validate()
        {

            if (PrescriptionsMaxCoursesSoftLimit == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(PrescriptionsMaxCoursesSoftLimit));
            }

            if (CoursesMaxCoursesLimit  == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(CoursesMaxCoursesLimit));
            }

            if(ApiUrl == null)
            {
                throw new ConfigurationNotFoundException("ApiUrl cannot be null or empty");
            }

            if(String.IsNullOrEmpty(CertificatePath))
            {
                throw new ConfigurationNotFoundException("Path cannot be null or empty");
            }

            if(string.IsNullOrEmpty(ApiVersion))
            {
                throw new ConfigurationNotFoundException("ApiVersion cannot be null or empty");
            }

            if(string.IsNullOrEmpty(ApplicationName))
            {
                throw new ConfigurationNotFoundException("ApplicationName cannot be null or empty");
            }

            if(string.IsNullOrEmpty(ApplicationProviderId))
            {
                throw new ConfigurationNotFoundException("ApplicationProviderId passphrase cannot be null or empty");
            }

            if(string.IsNullOrEmpty(ApplicationDeviceType))
            {
                throw new ConfigurationNotFoundException("ApplicationDeviceType cannot be null or empty");
            }
        }
    }
}