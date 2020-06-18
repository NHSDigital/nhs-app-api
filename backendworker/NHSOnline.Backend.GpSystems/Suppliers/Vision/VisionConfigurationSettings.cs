using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
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
            int? prescriptionsMaxCoursesSoftLimit)
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

        public static VisionConfigurationSettings CreateAndValidate(IConfiguration configuration, ILogger logger)
        {
            var applicationProviderId = configuration.GetOrWarn("VISION_APPLICATION_PROVIDER_ID", logger);
            var apiBaseUriString = configuration.GetOrWarn("VISION_BASE_URI", logger);
            var visionPfsPath = configuration.GetOrWarn("VISION_PFS_PATH", logger);
            var certificatePath = configuration.GetOrWarn("VISION_CERT_PATH", logger);
            var certificatePassphrase = configuration.GetOrWarn("VISION_CERT_PASSPHRASE", logger);
            var requestUsername = configuration.GetOrWarn("VISION_USERNAME", logger);
            var visionSenderUserName = configuration.GetOrWarn("VISION_SENDER_USERNAME", logger);
            var visionSenderUserFullName = configuration.GetOrWarn("VISION_SENDER_USERFULLNAME", logger);
            var visionSenderUserIdentity = configuration.GetOrWarn("VISION_SENDER_USERIDENTITY", logger);
            var visionSenderUserRole = configuration.GetOrWarn("VISION_SENDER_USERROLE", logger);

            var prescriptionsMaxCoursesSoftLimit = configuration.GetIntOrWarn("ConfigurationSettings:PrescriptionsMaxCoursesSoftLimit", logger);
            var coursesMaxCoursesLimit = configuration.GetIntOrWarn("ConfigurationSettings:CoursesMaxCoursesLimit", logger);
            var visionAppointmentSlotsRequestCount = configuration.GetIntOrWarn("ConfigurationSettings:VisionAppointmentSlotsRequestCount", logger);

            var config = new VisionConfigurationSettings(
                applicationProviderId,
                new Uri(apiBaseUriString + visionPfsPath, UriKind.Absolute),
                certificatePath,
                certificatePassphrase,
                requestUsername,
                visionSenderUserName,
                visionSenderUserFullName,
                visionSenderUserIdentity,
                visionSenderUserRole,
                visionAppointmentSlotsRequestCount,
                coursesMaxCoursesLimit,
                prescriptionsMaxCoursesSoftLimit);

            config.Validate();

            return config;
        }
    }
}