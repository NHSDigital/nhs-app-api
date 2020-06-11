using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public sealed class VisionConfigurationSettings
    {
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
            if (VisionAppointmentSlotsRequestCount == default)
            {
                throw new ConfigurationNotFoundException(nameof(VisionAppointmentSlotsRequestCount));
            }

            if (PrescriptionsMaxCoursesSoftLimit.HasValue && PrescriptionsMaxCoursesSoftLimit <= 0)
            {
                throw new ConfigurationNotValidException(nameof(PrescriptionsMaxCoursesSoftLimit));
            }

            if (CoursesMaxCoursesLimit.HasValue && CoursesMaxCoursesLimit <= 0)
            {
                throw new ConfigurationNotValidException(nameof(CoursesMaxCoursesLimit));
            }

            if (string.IsNullOrEmpty(ApplicationProviderId))
            {
                throw new ConfigurationNotFoundException(nameof(ApplicationProviderId));
            }

            if (string.IsNullOrEmpty(RequestUsername))
            {
                throw new ConfigurationNotFoundException(nameof(RequestUsername));
            }

            if (string.IsNullOrEmpty(VisionSenderUserFullName))
            {
                throw new ConfigurationNotFoundException(nameof(VisionSenderUserFullName));
            }

            if (string.IsNullOrEmpty(VisionSenderUserName))
            {
                throw new ConfigurationNotFoundException(nameof(VisionSenderUserName));
            }

            if (string.IsNullOrEmpty(VisionSenderUserIdentity))
            {
                throw new ConfigurationNotFoundException(nameof(VisionSenderUserIdentity));
            }
        }
    }
}