using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision {
    public class VisionConfigurationSettings {
        public int VisionAppointmentSlotsRequestCount { get; set; }
        public int? CoursesMaxCoursesLimit { get; set; }
        public int? PrescriptionsMaxCoursesSoftLimit { get; set; }
        public string ApplicationProviderId { get; }
        public Uri ApiUrl { get; }
        public string CertificatePath { get; }
        public string CertificatePassphrase { get; }
        public string RequestUsername { get; }
        public string VisionSenderUserName { get; }
        public string VisionSenderUserFullName { get; }
        public string VisionSenderUserIdentity { get; }
        public string VisionSenderUserRole { get; }
        
        public string Environment { get; set; }

        public VisionConfigurationSettings(
            string applicationProviderId, 
            Uri apiUrl, 
            string certificatePath, 
            string certificatePassphrase, 
            string requestUsername, 
            string visionSenderUserName, 
            string visionSenderFullName,
            string visionSenderUserIdentity,
            string visionSenderUserRole,
            int visionAppointmentSlotsRequestCount,
            int? coursesMaxCoursesLimit,
            int? prescriptionsMaxCoursesSoftLimit,
            string environment)
        {
            ApplicationProviderId = applicationProviderId;
            ApiUrl = apiUrl;
            CertificatePath = certificatePath;
            CertificatePassphrase = certificatePassphrase;
            RequestUsername = requestUsername;
            VisionSenderUserName = visionSenderUserName;
            VisionSenderUserFullName = visionSenderFullName;
            VisionSenderUserIdentity = visionSenderUserIdentity;
            VisionSenderUserRole = visionSenderUserRole;
            VisionAppointmentSlotsRequestCount = visionAppointmentSlotsRequestCount;
            CoursesMaxCoursesLimit = coursesMaxCoursesLimit;
            PrescriptionsMaxCoursesSoftLimit = prescriptionsMaxCoursesSoftLimit;
            Environment = environment;
        }
        public void Validate()
        { 
            if (VisionAppointmentSlotsRequestCount == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(VisionAppointmentSlotsRequestCount));
            }

            if (PrescriptionsMaxCoursesSoftLimit == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(PrescriptionsMaxCoursesSoftLimit));
            }

            if (CoursesMaxCoursesLimit == default(int))
            {
                throw new ConfigurationNotFoundException(nameof(CoursesMaxCoursesLimit));
            }

            if(String.IsNullOrEmpty(ApplicationProviderId))
            {
                throw new ConfigurationNotFoundException("ApiVersion cannot be null or empty");
            }

            if(String.IsNullOrEmpty(RequestUsername))
            {
                throw new ConfigurationNotFoundException("ApplicationDeviceType cannot be null or empty");
            }

            if(String.IsNullOrEmpty(VisionSenderUserFullName))
            {
                throw new ConfigurationNotFoundException("ApplicationDeviceType cannot be null or empty");
            }

            if(String.IsNullOrEmpty(VisionSenderUserName))
            {
                throw new ConfigurationNotFoundException("ApplicationDeviceType cannot be null or empty");
            }

            if(String.IsNullOrEmpty(VisionSenderUserIdentity))
            {
                throw new ConfigurationNotFoundException("ApplicationDeviceType cannot be null or empty");
            }

            if(String.IsNullOrEmpty(VisionSenderUserIdentity))
            {
                throw new ConfigurationNotFoundException("ApplicationDeviceType cannot be null or empty");
            }
        }
    }
}