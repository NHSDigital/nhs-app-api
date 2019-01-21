using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionPFSConfig : IVisionPFSConfig
    {
        public string ApplicationProviderId { get; }
        public Uri ApiUrl { get; }
        public string CertificatePath { get; }
        public string CertificatePassphrase { get; }
        public string RequestUsername { get; }
        public string VisionSenderUserName { get; }
        public string VisionSenderUserFullName { get; }
        public string VisionSenderUserIdentity { get; }
        public string VisionSenderUserRole { get; }

        public VisionPFSConfig(IConfiguration configuration, ILogger<VisionPFSConfig> logger)
        {
            ApplicationProviderId = configuration.GetOrWarn("VISION_APPLICATION_PROVIDER_ID", logger);
            
            var apiBaseUriString = configuration.GetOrWarn("VISION_BASE_URI", logger);
            var visionPFSPath = configuration.GetOrWarn("VISION_PFS_PATH", logger);

            if (!string.IsNullOrEmpty(apiBaseUriString) && !string.IsNullOrEmpty(visionPFSPath))
            {
                ApiUrl = new Uri($"{apiBaseUriString}{visionPFSPath}", UriKind.Absolute);
            }

            CertificatePath = configuration.GetOrWarn("VISION_CERT_PATH", logger);
            CertificatePassphrase = configuration.GetOrWarn("VISION_CERT_PASSPHRASE", logger);
            RequestUsername = configuration.GetOrWarn("VISION_USERNAME", logger);
            VisionSenderUserName = configuration.GetOrWarn("VISION_SENDER_USERNAME", logger);
            VisionSenderUserFullName = configuration.GetOrWarn("VISION_SENDER_USERFULLNAME", logger);
            VisionSenderUserIdentity = configuration.GetOrWarn("VISION_SENDER_USERIDENTITY", logger);
            VisionSenderUserRole = configuration.GetOrWarn("VISION_SENDER_USERROLE", logger);
        }
    }
}